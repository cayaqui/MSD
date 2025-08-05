namespace Core.DTOs.Auth.Roles;

/// <summary>
/// DTO for assigning a role to a user
/// </summary>
public class AssignRoleToUserDto
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public Guid? ProjectId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Notes { get; set; }
}