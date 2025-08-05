// Auth/UserDto.cs
namespace Core.DTOs.Auth.Permissions;

/// <summary>
/// DTO for individual permission
/// </summary>
public class PermissionDto
{
    /// <summary>
    /// Permission key (e.g., "project.create")
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Display name
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Resource this permission applies to
    /// </summary>
    public string Resource { get; set; } = string.Empty;

    /// <summary>
    /// Action this permission allows
    /// </summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// Module this permission belongs to
    /// </summary>
    public string Module { get; set; } = string.Empty;

    /// <summary>
    /// Whether the current context has this permission
    /// </summary>
    public bool IsGranted { get; set; }

    /// <summary>
    /// Whether this permission is inherited from a role
    /// </summary>
    public bool IsInherited { get; set; }

    /// <summary>
    /// Source of the permission (role name if inherited)
    /// </summary>
    public string? Source { get; set; }
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public bool IsSystemPermission { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsSystem { get; set; } // System permissions cannot be deleted
    public int RoleCount { get; set; } // Number of roles using this permission
}