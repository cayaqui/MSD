namespace Core.DTOs.Auth.ProjectTeamMembers;

public class BulkAssignResultDto
{
    public int SuccessCount { get; set; }
    public int TotalCount { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<Guid> SuccessfulAssignments { get; set; } = new();
}