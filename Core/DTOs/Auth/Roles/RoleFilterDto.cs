namespace Core.DTOs.Auth.Roles;

/// <summary>
/// Filter criteria for searching roles
/// </summary>
public class RoleFilterDto
{
    public string? SearchTerm { get; set; }
    public string? Type { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsSystem { get; set; }
    public Guid? ParentRoleId { get; set; }
    public int? Level { get; set; }
}