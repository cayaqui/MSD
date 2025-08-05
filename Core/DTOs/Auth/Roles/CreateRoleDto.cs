namespace Core.DTOs.Auth.Roles;

/// <summary>
/// DTO for creating a new role
/// </summary>
public class CreateRoleDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Type { get; set; } = string.Empty;
    public Guid? ParentRoleId { get; set; }
    public bool IsActive { get; set; } = true;
    public List<Guid>? PermissionIds { get; set; }
}