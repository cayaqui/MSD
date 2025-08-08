namespace Core.DTOs.Auth.ProjectTeamMembers;

public class AssignmentValidationResultDto
{
    public bool CanAssign { get; set; }
    public string? Reason { get; set; }
    public decimal? CurrentAllocation { get; set; }
    public decimal? AvailableAllocation { get; set; }
    public List<string> ConflictingProjects { get; set; } = new();
}