namespace Core.DTOs.Organization.Project;

public class BulkAssignProjectTeamDto
{
    public Guid ProjectId { get; set; }
    public List<AssignProjectTeamMemberDto> Assignments { get; set; } = new();
}