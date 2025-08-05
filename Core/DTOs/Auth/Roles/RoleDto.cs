namespace Core.DTOs.Auth.Roles;

/// <summary>
/// Data transfer object for Role
/// </summary>
public class RoleDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Type { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsSystem { get; set; }
    public Guid? ParentRoleId { get; set; }
    public string? ParentRoleName { get; set; }
    public int Level { get; set; }
    public string HierarchyPath { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}