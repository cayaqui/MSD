// Auth/UserDto.cs
namespace Core.DTOs.Auth;

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
}