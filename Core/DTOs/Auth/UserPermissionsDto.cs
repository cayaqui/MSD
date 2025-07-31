namespace Core.DTOs.Auth;

/// <summary>
/// DTO containing all user permissions across the system
/// </summary>
public class UserPermissionsDto
{
    /// <summary>
    /// User information
    /// </summary>
    public UserDto User { get; set; } = null!;

    /// <summary>
    /// Global permissions (system-wide)
    /// </summary>
    public List<string> GlobalPermissions { get; set; } = new();

    /// <summary>
    /// Permissions grouped by project
    /// </summary>
    public List<ProjectPermissionDto> ProjectPermissions { get; set; } = new();

    /// <summary>
    /// Timestamp when permissions were retrieved
    /// </summary>
    public DateTime RetrievedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// System permissions (alias for GlobalPermissions for compatibility)
    /// </summary>
    public List<string> SystemPermissions
    {
        get => GlobalPermissions;
        set => GlobalPermissions = value;
    }

    /// <summary>
    /// Check if user has a specific global/system permission
    /// </summary>
    public bool HasGlobalPermission(string permission)
    {
        return GlobalPermissions.Contains(permission, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Check if user has a specific system permission (alias)
    /// </summary>
    public bool HasSystemPermission(string permission)
    {
        return HasGlobalPermission(permission);
    }

    /// <summary>
    /// Get permissions for a specific project
    /// </summary>
    public ProjectPermissionDto? GetProjectPermissions(Guid projectId)
    {
        return ProjectPermissions.FirstOrDefault(p => p.ProjectId == projectId);
    }

    /// <summary>
    /// Check if user has any permission in a specific project
    /// </summary>
    public bool HasProjectAccess(Guid projectId)
    {
        return ProjectPermissions.Any(p => p.ProjectId == projectId && p.IsActive);
    }
}