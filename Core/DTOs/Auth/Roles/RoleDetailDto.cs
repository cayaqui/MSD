namespace Core.DTOs.Auth.Roles;

/// <summary>
/// Detailed role information including permissions and assignments
/// </summary>
public class RoleDetailDto : RoleDto
{
    public List<RolePermissionDto> Permissions { get; set; } = new();
    public List<UserRoleAssignmentDto> Assignments { get; set; } = new();
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public List<RoleDto> ChildRoles { get; set; } = new();
}