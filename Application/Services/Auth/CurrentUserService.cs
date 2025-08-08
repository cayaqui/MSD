using Application.Interfaces.Auth;
using Core.Constants;
using Domain.Entities.Auth.Security;
using System.Security.Claims;

namespace Application.Services.Auth;

public class CurrentUserService : ICurrentUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CurrentUserService> _logger;
    private User? _cachedUser;

    // These properties are set by the CurrentUserMiddleware
    public string? UserId { get;  set; }
    public string? UserName { get;  set; }
    public string? Email { get;  set; }
    public bool IsAuthenticated { get;  set; }
    public ClaimsPrincipal? Principal { get;  set; }
    
    // Simplified system role properties
    public bool IsAdmin => SystemRole == SimplifiedRoles.System.Admin;
    public bool IsSupport => SystemRole == SimplifiedRoles.System.Support;
    public string? SystemRole { get; private set; }

    public CurrentUserService(IUnitOfWork unitOfWork, ILogger<CurrentUserService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<bool> IsInRoleAsync(string role)
    {
        // Since we're using project-based roles, this checks if user has this role in ANY project
        if (!IsAuthenticated || string.IsNullOrEmpty(UserId))
            return false;

        var user = await _unitOfWork.Repository<User>()
            .Query()
            .Include(u => u.ProjectTeamMembers)
            .FirstOrDefaultAsync(u => u.EntraId == UserId);

        if (user == null)
            return false;

        return user.ProjectTeamMembers.Any(ptm => ptm.Role == role && ptm.IsActive);
    }

    public async Task<bool> HasPermissionAsync(string permission)
    {
        if (!IsAuthenticated || string.IsNullOrEmpty(UserId))
            return false;

        var user = await _unitOfWork.Repository<User>()
            .Query()
            .Include(u => u.ProjectTeamMembers)
            .FirstOrDefaultAsync(u => u.EntraId == UserId && !u.IsDeleted);

        if (user == null)
            return false;

        // Check if user has support role (all permissions)
        if (user.IsSupport())
            return true;

        // Check if permission is granted through any project role
        foreach (var membership in user.ProjectTeamMembers.Where(ptm => ptm.IsActive))
        {
            var rolePermissions = Core.Constants.ProjectRoles.GetPermissionsForRole(membership.Role);
            if (rolePermissions.Contains(permission))
                return true;
        }

        return false;
    }

    public async Task<bool> HasProjectAccessAsync(Guid projectId, string? requiredRole = null)
    {
        if (!IsAuthenticated || string.IsNullOrEmpty(UserId))
            return false;

        var user = await _unitOfWork.Repository<User>()
            .Query()
            .FirstOrDefaultAsync(u => u.EntraId == UserId);

        if (user == null)
            return false;

        var projectMember = await _unitOfWork.Repository<ProjectTeamMember>()
            .Query()
            .FirstOrDefaultAsync(ptm =>
                ptm.UserId == user.Id &&
                ptm.ProjectId == projectId &&
                ptm.IsActive);

        if (projectMember == null)
            return false;

        // If no specific role required, just check if user is a member
        if (string.IsNullOrEmpty(requiredRole))
            return true;

        // Check if user has the required role or higher
        return HasRequiredOrHigherRole(projectMember.Role, requiredRole);
    }

    private bool HasRequiredOrHigherRole(string userRole, string requiredRole)
    {
        var roleHierarchy = new Dictionary<string, int>
        {
            { SimplifiedRoles.Project.Viewer, 1 },
            { SimplifiedRoles.Project.TeamMember, 2 },
            { SimplifiedRoles.Project.TeamLead, 3 },
            { SimplifiedRoles.Project.ProjectManager, 4 }
        };

        if (!roleHierarchy.TryGetValue(userRole, out var userLevel) ||
            !roleHierarchy.TryGetValue(requiredRole, out var requiredLevel))
        {
            return false;
        }

        return userLevel >= requiredLevel;
    }

    public async Task<string?> GetProjectRoleAsync(Guid projectId)
    {
        if (string.IsNullOrEmpty(UserId))
            return null;

        var user = await _unitOfWork.Repository<User>()
            .Query()
            .Include(u => u.ProjectTeamMembers)
            .FirstOrDefaultAsync(u => u.EntraId == UserId && !u.IsDeleted);
            
        if (user == null)
            return null;
            
        return user.ProjectTeamMembers
            .Where(ur => ur.ProjectId == projectId && ur.IsActive)
            .Select(ur => ur.Role)
            .FirstOrDefault();
    }

    public async Task<List<Guid>> GetUserProjectIdsAsync()
    {
        if (string.IsNullOrEmpty(UserId))
            return new List<Guid>();

        var user = await GetCurrentUserAsync();
            
        if (user == null)
            return new List<Guid>();
            
        return user.ProjectTeamMembers
            .Where(ptm => ptm.IsActive)
            .Select(ptm => ptm.ProjectId)
            .ToList();
    }
    
    // New simplified methods
    public async Task<bool> HasSystemAccessAsync()
    {
        var user = await GetCurrentUserAsync();
        return user != null && (user.SystemRole == SimplifiedRoles.System.Admin || 
                                user.SystemRole == SimplifiedRoles.System.Support);
    }
    
    public async Task<bool> IsSystemRoleAsync()
    {
        return await HasSystemAccessAsync();
    }
    
    public async Task<bool> CanEditProjectAsync(Guid projectId)
    {
        // System roles can edit any project
        if (await HasSystemAccessAsync())
            return true;
            
        var projectRole = await GetProjectRoleAsync(projectId);
        return projectRole != null && SimplifiedRoles.Project.CanEdit.Contains(projectRole);
    }
    
    public async Task<bool> CanViewProjectAsync(Guid projectId)
    {
        // System roles can view any project
        if (await HasSystemAccessAsync())
            return true;
            
        // Any project member can view
        return await HasProjectAccessAsync(projectId);
    }
    
    private async Task<User?> GetCurrentUserAsync()
    {
        if (!IsAuthenticated || string.IsNullOrEmpty(UserId))
            return null;
            
        if (_cachedUser != null)
            return _cachedUser;
            
        _cachedUser = await _unitOfWork.Repository<User>()
            .Query()
            .Include(u => u.ProjectTeamMembers)
            .FirstOrDefaultAsync(u => u.EntraId == UserId && !u.IsDeleted);
            
        if (_cachedUser != null)
        {
            SystemRole = _cachedUser.SystemRole;
        }
            
        return _cachedUser;
    }
}