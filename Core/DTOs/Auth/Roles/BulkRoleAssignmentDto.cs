namespace Core.DTOs.Auth.Roles;

/// <summary>
/// DTO for bulk role assignment
/// </summary>
public class BulkRoleAssignmentDto
{
    public Guid RoleId { get; set; }
    public List<Guid> UserIds { get; set; } = new();
    public Guid? ProjectId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Notes { get; set; }
}