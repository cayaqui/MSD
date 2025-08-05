namespace Core.DTOs.Auth.Roles;

/// <summary>
/// DTO for hierarchical permission tree
/// </summary>
public class RolePermissionTreeDto
{
    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public List<PermissionCategoryDto> Categories { get; set; } = new();
}

public class PermissionCategoryDto
{
    public string Category { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public List<PermissionNodeDto> Permissions { get; set; } = new();
}

public class PermissionNodeDto
{
    public Guid PermissionId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsAssigned { get; set; }
    public bool IsInherited { get; set; }
    public Guid? InheritedFromRoleId { get; set; }
    public string? InheritedFromRoleName { get; set; }
    public List<PermissionNodeDto> Children { get; set; } = new();
}