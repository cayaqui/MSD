using Application.Interfaces.Auth;
using System.Security.Claims;

namespace Application.Services.Auth;

public class CurrentUserService : ICurrentUserService
{
    private readonly IUnitOfWork _unitOfWork;

    // These properties are set by the CurrentUserMiddleware
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public bool IsAuthenticated { get; set; }
    public ClaimsPrincipal? Principal { get; set; }

    public CurrentUserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
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
        // This is a simplified version - in reality, you'd check against project context
        // For now, just check if user is authenticated
        return IsAuthenticated;
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
            { ProjectRoles.Viewer, 1 },
            { ProjectRoles.TeamMember, 2 },
            { ProjectRoles.TeamLead, 3 },
            { ProjectRoles.ProjectManager, 4 }
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
        var userId = UserId;
        if (string.IsNullOrEmpty(userId))
        {
            return null;
        }
        var gUserId = Guid.Parse(userId);
        var user = await _unitOfWork.Repository<User>()
            .Query()
            .Include(u => u.ProjectTeamMembers)
            .FirstOrDefaultAsync(u => u.Id == gUserId);
        if (user == null)
        {
            return null;
        }
        return user.ProjectTeamMembers
            .Where(ur => ur.ProjectId == projectId && ur.IsActive)
            .Select(ur => ur.Role)
            .FirstOrDefault();
    }

    public async Task<List<Guid>> GetUserProjectIdsAsync()
    {
        var userId = UserId;
        if (string.IsNullOrEmpty(userId))
        { 
            return new List<Guid>(); 
        }
        var gUserId = Guid.Parse(userId);   
        var user = await _unitOfWork.Repository<User>()
            .Query()
            .Include(u => u.ProjectTeamMembers)
            .FirstOrDefaultAsync(u => u.Id == gUserId);
        if (user == null)
        {
            return new List<Guid>();
        }
        return user.ProjectTeamMembers
            .Where(ptm => ptm.IsActive)
            .Select(ptm => ptm.ProjectId)
            .ToList();
    }
}