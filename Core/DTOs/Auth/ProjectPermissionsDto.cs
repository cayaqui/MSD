namespace Core.DTOs.Auth;

/// <summary>
/// DTO for project-specific permissions
/// </summary>
public class ProjectPermissionsDto
{
    /// <summary>
    /// Project ID
    /// </summary>
    public Guid ProjectId { get; set; }

    /// <summary>
    /// Project name
    /// </summary>
    public string ProjectName { get; set; } = string.Empty;

    /// <summary>
    /// Project code (optional)
    /// </summary>
    public string? ProjectCode { get; set; }

    /// <summary>
    /// User's role in the project
    /// </summary>
    public string UserRole { get; set; } = string.Empty;

    /// <summary>
    /// User's roles in the project (can have multiple)
    /// </summary>
    public List<string> UserRoles { get; set; } = new();

    /// <summary>
    /// Dictionary of permissions and their values
    /// Key: permission name (e.g., "budget.view")
    /// Value: whether the user has the permission
    /// </summary>
    public Dictionary<string, bool> Permissions { get; set; } = new();

    /// <summary>
    /// Is the user active in this project
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Date when user was added to the project
    /// </summary>
    public DateTime? JoinedAt { get; set; }

    /// <summary>
    /// Check if user has a specific permission in this project
    /// </summary>
    public bool HasPermission(string permission)
    {
        return Permissions.ContainsKey(permission) && Permissions[permission];
    }

    /// <summary>
    /// Check if user has any of the specified permissions
    /// </summary>
    public bool HasAnyPermission(params string[] permissions)
    {
        return permissions.Any(p => HasPermission(p));
    }

    /// <summary>
    /// Check if user has all of the specified permissions
    /// </summary>
    public bool HasAllPermissions(params string[] permissions)
    {
        return permissions.All(p => HasPermission(p));
    }

    /// <summary>
    /// Get all granted permissions
    /// </summary>
    public List<string> GetGrantedPermissions()
    {
        return Permissions
            .Where(p => p.Value)
            .Select(p => p.Key)
            .ToList();
    }
}