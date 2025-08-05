namespace Core.DTOs.Auth.Roles;

/// <summary>
/// DTO for assigning permissions to a role
/// </summary>
public class AssignPermissionsToRoleDto
{
    public List<Guid> PermissionIds { get; set; } = new();
    public bool ReplaceExisting { get; set; } = false;
}