using Application.Interfaces.Auth;
using Core.Constants;
using Core.DTOs.Auth.Permissions;
using Core.DTOs.Auth.Users;
using Domain.Entities.Auth.Security;

namespace Application.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;
    private readonly ILogger<AuthService> _logger;
    private readonly IGraphApiService? _graphApiService;

    public AuthService(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IMapper mapper,
        ILogger<AuthService> logger,
        IGraphApiService? graphApiService = null)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _mapper = mapper;
        _logger = logger;
        _graphApiService = graphApiService;
    }

    public async Task<UserDto?> GetCurrentUserAsync()
    {
        if (!_currentUserService.IsAuthenticated || string.IsNullOrEmpty(_currentUserService.UserId))
            return null;

        var user = await _unitOfWork.Repository<User>()
            .Query()
            .Include(u => u.ProjectTeamMembers)
                .ThenInclude(ptm => ptm.Project)
            .FirstOrDefaultAsync(u => u.EntraId == _currentUserService.UserId);

        if (user == null)
        {
            // User doesn't exist in database - return null
            // User must be created by an administrator
            _logger.LogWarning("User with EntraId {EntraId} not found in database", _currentUserService.UserId);
            return null;
        }

        // Update last login
        user.RecordLogin();
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserPermissionsDto?> GetUserPermissionsAsync()
    {
        if (!_currentUserService.IsAuthenticated || string.IsNullOrEmpty(_currentUserService.UserId))
            return null;

        var user = await _unitOfWork.Repository<User>()
            .Query()
            .Include(u => u.ProjectTeamMembers)
                .ThenInclude(ptm => ptm.Project)
            .FirstOrDefaultAsync(u => u.EntraId == _currentUserService.UserId);

        if (user == null)
            return null;

        var permissions = new UserPermissionsDto
        {
            User = _mapper.Map<UserDto>(user),
            GlobalPermissions = new List<string>(),
            ProjectPermissions = new List<ProjectPermissionDto>()
        };

        // Add global permissions based on user type
        if (user.IsSupport())
        {
            // Support users get all system permissions
            permissions.GlobalPermissions.AddRange(GetAllSystemPermissions());
        }
        else if (user.ProjectTeamMembers.Any())
        {
            // Regular users get permissions based on their highest project role
            var highestRole = GetHighestProjectRole(user.ProjectTeamMembers);
            permissions.GlobalPermissions.AddRange(GetGlobalPermissionsForProjectRole(highestRole));
        }
        else
        {
            // Users without projects get basic permissions
            permissions.GlobalPermissions.AddRange(GetBasicUserPermissions());
        }

        // Add project-specific permissions
        foreach (var membership in user.ProjectTeamMembers.Where(ptm => ptm.IsActive))
        {
            var projectPermissions = new ProjectPermissionDto
            {
                ProjectId = membership.ProjectId,
                ProjectName = membership.Project.Name,
                ProjectCode = membership.Project.Code,
                Role = membership.Role,
                IsActive = membership.IsActive,
                Permissions = GetPermissionsForRole(membership.Role)
            };

            permissions.ProjectPermissions.Add(projectPermissions);
        }

        return permissions;
    }

    public async Task<ProjectPermissionDto?> GetUserProjectPermissionsAsync(Guid projectId)
    {
        if (!_currentUserService.IsAuthenticated || string.IsNullOrEmpty(_currentUserService.UserId))
            return null;

        var user = await _unitOfWork.Repository<User>()
            .Query()
            .Include(u => u.ProjectTeamMembers)
                .ThenInclude(ptm => ptm.Project)
            .FirstOrDefaultAsync(u => u.EntraId == _currentUserService.UserId);

        if (user == null)
            return null;

        var membership = user.ProjectTeamMembers.FirstOrDefault(ptm => ptm.ProjectId == projectId);
        if (membership == null)
            return null;

        return new ProjectPermissionDto
        {
            ProjectId = membership.ProjectId,
            ProjectName = membership.Project.Name,
            ProjectCode = membership.Project.Code,
            Role = membership.Role,
            IsActive = membership.IsActive,
            Permissions = GetPermissionsForRole(membership.Role)
        };
    }

    public async Task<UserDto?> SyncCurrentUserWithAzureAsync()
    {
        if (!_currentUserService.IsAuthenticated || string.IsNullOrEmpty(_currentUserService.UserId))
            return null;

        return await SyncUserWithAzureAsync(
            _currentUserService.UserId,
            _currentUserService.Email ?? "",
            _currentUserService.UserName ?? "");
    }

    public async Task<UserDto?> SyncUserWithAzureAsync(string entraId, string email, string displayName)
    {
        try
        {
            var user = await _unitOfWork.Repository<User>()
                .Query()
                .FirstOrDefaultAsync(u => u.EntraId == entraId);

            if (user == null)
            {
                // User must exist in database before syncing
                // Only administrators can create new users
                _logger.LogWarning("Cannot sync user {EntraId} - user does not exist in database", entraId);
                return null;
            }

            // Update existing user with latest Azure AD info
            user.UpdateProfile(displayName);
            user.RecordLogin();
            
            // Try to sync additional data from Graph API if available
            if (_graphApiService != null)
            {
                try
                {
                    var graphUser = await _graphApiService.GetUserByObjectIdAsync(entraId);
                    if (graphUser != null)
                    {
                        // Update user with Graph data
                        if (!string.IsNullOrEmpty(graphUser.GivenName))
                            user.UpdateProfile(graphUser.DisplayName, graphUser.GivenName, graphUser.Surname);
                        
                        if (!string.IsNullOrEmpty(graphUser.JobTitle))
                            user.JobTitle = graphUser.JobTitle;
                            
                        if (!string.IsNullOrEmpty(graphUser.Department))
                            user.Department = graphUser.Department;
                            
                        if (!string.IsNullOrEmpty(graphUser.OfficeLocation))
                            user.OfficeLocation = graphUser.OfficeLocation;
                            
                        if (!string.IsNullOrEmpty(graphUser.MobilePhone))
                            user.MobilePhone = graphUser.MobilePhone;
                        
                        // Try to get and save user photo

                        var photoUrl = await _graphApiService.GetUserPhotoAsDataUrlAsync(entraId);
                        if (!string.IsNullOrEmpty(photoUrl))
                        {
                            // Store as data URL for easy display
                            user.PhotoUrl = photoUrl;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to sync additional user data from Graph API for {EntraId}", entraId);
                    // Continue without Graph data
                }
            }
            
            _unitOfWork.Repository<User>().Update(user);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<UserDto>(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing user {EntraId} with Azure AD", entraId);
            throw;
        }
    }

    #region Private Helper Methods

    private string GetHighestProjectRole(IEnumerable<ProjectTeamMember> memberships)
    {
        var activeRoles = memberships
            .Where(m => m.IsActive)
            .Select(m => m.Role)
            .ToList();

        // Return the highest role based on hierarchy
        // Check both new constants and actual DB values for compatibility
        if (activeRoles.Any(r => r == ProjectRoles.ProjectManager || r == "PROJECT_MANAGER"))
            return ProjectRoles.ProjectManager;
        if (activeRoles.Any(r => r == ProjectRoles.ProjectEngineer || r == "PROJECT_ENGINEER"))
            return ProjectRoles.ProjectEngineer;
        if (activeRoles.Any(r => r == ProjectRoles.CostController || r == "COST_CONTROLLER"))
            return ProjectRoles.CostController;
        if (activeRoles.Any(r => r == ProjectRoles.Planner || r == "PLANNER"))
            return ProjectRoles.Planner;
        if (activeRoles.Any(r => r == ProjectRoles.QaQc || r == "QA_QC"))
            return ProjectRoles.QaQc;
        if (activeRoles.Any(r => r == ProjectRoles.DocumentController || r == "DOCUMENT_CONTROLLER"))
            return ProjectRoles.DocumentController;
        if (activeRoles.Any(r => r == ProjectRoles.TeamMember || r == "TEAM_MEMBER"))
            return ProjectRoles.TeamMember;

        return ProjectRoles.Observer;
    }

    private List<string> GetGlobalPermissionsForProjectRole(string role)
    {
        // Map project roles to global navigation permissions
        return role switch
        {
            ProjectRoles.ProjectManager => new List<string>
            {
                // Configuration
                "company.view",
                "system.users.view",
                "system.roles.view",
                
                // Projects
                "project.view",
                "project.create",
                "project.team.view",
                
                // Budget & Cost
                "budget.view",
                "cost.view",
                
                // Schedule
                "schedule.view",
                
                // Documents
                "document.view",
                
                // Reports
                "report.dashboard.view",
                "report.executive.view",
                "report.project.view",
                "report.kpi.view"
            },

            ProjectRoles.ProjectEngineer => new List<string>
            {
                // Configuration
                "company.view",
                "system.users.view",
                
                // Projects
                "project.view",
                "project.team.view",
                
                // Budget & Cost
                "budget.view",
                "cost.view",
                
                // Schedule
                "schedule.view",
                
                // Documents
                "document.view",
                
                // Reports
                "report.dashboard.view",
                "report.project.view"
            },
            
            ProjectRoles.CostController => new List<string>
            {
                // Configuration
                "company.view",
                
                // Projects
                "project.view",
                
                // Budget & Cost - Full access
                "budget.view",
                "budget.create",
                "budget.edit",
                "cost.view",
                "cost.create",
                "cost.edit",
                
                // Reports
                "report.dashboard.view",
                "report.project.view",
                "report.kpi.view"
            },
            
            ProjectRoles.Planner => new List<string>
            {
                // Projects
                "project.view",
                
                // Schedule - Full access
                "schedule.view",
                "schedule.create",
                "schedule.edit",
                
                // Reports
                "report.dashboard.view",
                "report.project.view"
            },
            
            ProjectRoles.ProjectController => new List<string>
            {
                // Configuration
                "company.view",
                "system.users.view",
                
                // Projects
                "project.view",
                "project.team.view",
                
                // Budget & Cost
                "budget.view",
                "cost.view",
                
                // Schedule
                "schedule.view",
                
                // Documents
                "document.view",
                
                // Reports
                "report.dashboard.view",
                "report.project.view",
                "report.kpi.view"
            },

            ProjectRoles.TeamLead => new List<string>
            {
                // Projects
                "project.view",
                "project.team.view",
                
                // Budget & Cost
                "budget.view",
                "cost.view",
                
                // Schedule
                "schedule.view",
                
                // Documents
                "document.view",
                
                // Reports
                "report.dashboard.view",
                "report.project.view"
            },

            ProjectRoles.QaQc => new List<string>
            {
                // Projects
                "project.view",
                
                // Quality
                "quality.view",
                "quality.create",
                "quality.edit",
                
                // Documents
                "document.view",
                "document.create",
                
                // Reports
                "report.dashboard.view",
                "report.project.view"
            },
            
            ProjectRoles.DocumentController => new List<string>
            {
                // Projects
                "project.view",
                
                // Documents - Full access
                "document.view",
                "document.create",
                "document.edit",
                "document.distribute",
                
                // Reports
                "report.dashboard.view",
                "report.project.view"
            },

            ProjectRoles.TeamMember => new List<string>
            {
                // Projects
                "project.view",
                
                // Budget & Cost
                "budget.view",
                "cost.view",
                
                // Schedule
                "schedule.view",
                
                // Documents
                "document.view",
                
                // Reports
                "report.dashboard.view"
            },
            
            ProjectRoles.Observer => new List<string>
            {
                // Projects - View only
                "project.view",
                
                // Budget & Cost - View only
                "budget.view",
                "cost.view",
                
                // Schedule - View only
                "schedule.view",
                
                // Documents - View only
                "document.view",
                
                // Reports - Basic view
                "report.dashboard.view"
            },

            _ => GetBasicUserPermissions()
        };
    }

    private List<string> GetBasicUserPermissions()
    {
        return new List<string>
        {
            "user.view",
            "user.edit.own",
            "project.view",
            "report.dashboard.view"
        };
    }

    private List<string> GetAllSystemPermissions()
    {
        // For support users - all permissions
        return new List<string>
        {
            // System
            "system.configuration.view",
            "system.configuration.edit",
            "system.users.view",
            "system.users.manage",
            "system.roles.view",
            "system.roles.manage",
            "system.permissions.view",
            "system.permissions.manage",
            
            // Company
            "company.view",
            "company.create",
            "company.edit",
            "company.delete",
            "company.operations.view",
            "company.operations.manage",
            
            // Projects
            "project.view",
            "project.create",
            "project.edit",
            "project.delete",
            "project.team.view",
            "project.team.manage",
            "project.settings.view",
            "project.settings.manage",
            
            // Budget & Cost
            "budget.view",
            "budget.create",
            "budget.edit",
            "budget.delete",
            "budget.approve",
            "cost.view",
            "cost.create",
            "cost.edit",
            "cost.delete",
            "cost.approve",
            
            // Schedule
            "schedule.view",
            "schedule.create",
            "schedule.edit",
            "schedule.delete",
            "schedule.approve",
            
            // Contracts
            "contract.view",
            "contract.create",
            "contract.edit",
            "contract.delete",
            "contract.approve",
            
            // Documents
            "document.view",
            "document.create",
            "document.edit",
            "document.delete",
            "document.approve",
            
            // Reports
            "report.dashboard.view",
            "report.executive.view",
            "report.project.view",
            "report.kpi.view",
            "report.create",
            "report.export",
            
            // Risk
            "risk.view",
            "risk.create",
            "risk.edit",
            "risk.delete",
            
            // Quality
            "quality.view",
            "quality.create",
            "quality.edit",
            "quality.delete"
        };
    }

    private List<string> GetPermissionsForRole(string role)
    {
        return PermissionConstants.GetPermissionsForRole(role).ToList();
    }

    #endregion
}