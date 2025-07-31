// Auth/UserDto.cs
namespace Core.DTOs.Auth;

/// <summary>
/// DTO for grouping permissions by module or resource
/// </summary>
public class PermissionGroupDto
{
    /// <summary>
    /// Group name (module or resource)
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Group display name
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Group description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Icon for the group
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Order for display
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Permissions in this group
    /// </summary>
    public List<PermissionDto> Permissions { get; set; } = new();

    /// <summary>
    /// Check if all permissions in the group are granted
    /// </summary>
    public bool AllPermissionsGranted => Permissions.All(p => p.IsGranted);

    /// <summary>
    /// Check if any permission in the group is granted
    /// </summary>
    public bool AnyPermissionGranted => Permissions.Any(p => p.IsGranted);

    /// <summary>
    /// Get count of granted permissions
    /// </summary>
    public int GrantedPermissionCount => Permissions.Count(p => p.IsGranted);
}
