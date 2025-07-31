namespace Core.DTOs.Projects;

/// <summary>
/// DTO for project team assignment
/// </summary>
public class AssignProjectTeamMemberDto
{
    public Guid UserId { get; set; }
    public string Role { get; set; } = string.Empty;
    public decimal? AllocationPercentage { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}