// Application/Services/Auth/AuthService.cs
using Core.DTOs.Auth;
using Domain.Entities.Security;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Application.Interfaces.Auth;
using Core.Constants;

namespace Application.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IMapper mapper,
        ILogger<AuthService> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _mapper = mapper;
        _logger = logger;
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
            // Try to sync with Azure AD if user doesn't exist
            if (!string.IsNullOrEmpty(_currentUserService.Email) && !string.IsNullOrEmpty(_currentUserService.UserName))
            {
                return await SyncUserWithAzureAsync(
                    _currentUserService.UserId,
                    _currentUserService.Email,
                    _currentUserService.UserName);
            }
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

    public async Task<ProjectPermissionsDto?> GetUserProjectPermissionsAsync(Guid projectId)
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

        return new ProjectPermissionsDto
        {
            ProjectId = membership.ProjectId,
            ProjectName = membership.Project.Name,
            ProjectCode = membership.Project.Code,
            UserRole = membership.Role,
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
                // Create new user
                user = new User(entraId, email, displayName);
                await _unitOfWork.Repository<User>().AddAsync(user);
            }
            else
            {
                user.UpdateProfile(displayName);
                user.RecordLogin();
                _unitOfWork.Repository<User>().Update(user);
            }

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
        if (activeRoles.Contains(ProjectRoles.ProjectManager))
            return ProjectRoles.ProjectManager;
        if (activeRoles.Contains(ProjectRoles.ProjectController))
            return ProjectRoles.ProjectController;
        if (activeRoles.Contains(ProjectRoles.TeamLead))
            return ProjectRoles.TeamLead;
        if (activeRoles.Contains(ProjectRoles.TeamMember))
            return ProjectRoles.TeamMember;

        return ProjectRoles.Viewer;
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
        return ProjectRoles.GetPermissionsForRole(role).ToList();
    }

    #endregion
}