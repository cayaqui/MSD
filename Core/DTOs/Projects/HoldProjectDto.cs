namespace Core.DTOs.Projects;

/// <summary>
/// DTO for holding a project
/// </summary>
public class HoldProjectDto
{
    public string Reason { get; set; } = string.Empty;
    public DateTime? ExpectedResumeDate { get; set; }
}
