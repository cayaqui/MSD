// Application/Services/Auth/AuthService.cs
using Core.DTOs.Auth;
using Domain.Entities.Security;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Application.Interfaces.Auth;

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
            permissions.GlobalPermissions.AddRange(GetAllSystemPermissions());
        }
        else if (user.ProjectTeamMembers.Any())
        {
            // Basic permissions for all authenticated users with projects
            permissions.GlobalPermissions.Add("user.view");
            permissions.GlobalPermissions.Add("user.edit.own");
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

        var permissions = new ProjectPermissionsDto
        {
            ProjectId = membership.ProjectId,
            ProjectName = membership.Project.Name,
            UserRole = membership.Role,
            Permissions = GetPermissionsForRole(membership.Role)
                .ToDictionary(p => p, p => true)
        };

        return permissions;
    }

    public async Task<UserDto?> SyncCurrentUserWithAzureAsync()
    {
        if (!_currentUserService.IsAuthenticated)
            return null;

        // Here you would call Microsoft Graph API to get updated user info
        // For now, we'll just update the last sync timestamp
        var user = await _unitOfWork.Repository<User>()
            .Query()
            .FirstOrDefaultAsync(u => u.EntraId == _currentUserService.UserId);

        if (user == null)
            return null;

        // Update user properties from Azure AD claims
        if (!string.IsNullOrEmpty(_currentUserService.UserName) && user.Name != _currentUserService.UserName)
        {
            user.UpdateProfile(_currentUserService.UserName, null, null);
            user.UpdatedAt = DateTime.UtcNow;
        }

        if (!string.IsNullOrEmpty(_currentUserService.Email) && user.Email != _currentUserService.Email)
        {
            user.UpdateEmail(_currentUserService.Email);
            user.UpdatedAt = DateTime.UtcNow;
        }

        user.RecordLogin();
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Synced user {UserId} with Azure AD", user.Id);

        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto?> SyncUserWithAzureAsync(string entraId, string email, string displayName)
    {
        try
        {
            // Check if user already exists
            var existingUser = await _unitOfWork.Repository<User>()
                .Query()
                .FirstOrDefaultAsync(u => u.EntraId == entraId || u.Email == email);

            if (existingUser != null)
            {
                existingUser.UpdateProfile(displayName);
                existingUser.UpdateEmail(email);
                // Update existing user
                existingUser.UpdateEntraId(entraId);
                existingUser.UpdatedAt = DateTime.UtcNow;
                existingUser.RecordLogin();

                await _unitOfWork.SaveChangesAsync();
                return _mapper.Map<UserDto>(existingUser);
            }

            // Create new user
            var newUser = new User(entraId, email, displayName);

            newUser.RecordLogin();

            await _unitOfWork.Repository<User>().AddAsync(newUser);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Created new user {UserId} from Azure AD", newUser.Id);

            return _mapper.Map<UserDto>(newUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing user {EntraId} with Azure AD", entraId);
            throw;
        }
    }

    private List<string> GetPermissionsForRole(string role)
    {
        return role switch
        {
            ProjectRoles.ProjectManager => PredefinedRoles.ProjectManager.Permissions.ToList(),
            ProjectRoles.CostController => PredefinedRoles.CostController.Permissions.ToList(),
            ProjectRoles.TeamLead => PredefinedRoles.CostController.Permissions.ToList(),
            ProjectRoles.Viewer => PredefinedRoles.Viewer.Permissions.ToList(),
            _ => new List<string>()
        };
    }

    private List<string> GetAllSystemPermissions()
    {
        var permissions = new List<string>();

        // Add all permissions from all modules
        foreach (var module in PermissionModules.All)
        {
            foreach (var resource in GetResourcesForModule(module))
            {
                foreach (var action in PermissionActions.All)
                {
                    permissions.Add($"{resource.ToLower()}.{action}");
                }
            }
        }

        return permissions;
    }

    private string[] GetResourcesForModule(string module)
    {
        return module switch
        {
            PermissionModules.Setup => new[] {
                PermissionResources.Company,
                PermissionResources.Operation,
                PermissionResources.Project,
                PermissionResources.Phase,
                PermissionResources.WorkPackage
            },
            PermissionModules.Cost => new[] {
                PermissionResources.Budget,
                PermissionResources.Trend,
                PermissionResources.Commitment,
                PermissionResources.Invoice,
                PermissionResources.Contingency
            },
            PermissionModules.Progress => new[] {
                PermissionResources.Schedule,
                PermissionResources.ActualProgress,
                PermissionResources.PlanProgress
            },
            PermissionModules.Reports => new[] {
                PermissionResources.Dashboard,
                PermissionResources.MonthlyReport,
                PermissionResources.Analytics
            },
            PermissionModules.Admin => new[] {
                PermissionResources.User,
                PermissionResources.Role,
                PermissionResources.Permission
            },
            _ => Array.Empty<string>()
        };
    }
}