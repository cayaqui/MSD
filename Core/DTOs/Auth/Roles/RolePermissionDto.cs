namespace Core.DTOs.Auth.Roles;

/// <summary>
/// DTO for role permissions
/// </summary>
public class RolePermissionDto
{
    public Guid PermissionId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsInherited { get; set; }
    public Guid? InheritedFromRoleId { get; set; }
    public string? InheritedFromRoleName { get; set; }
}