namespace Core.DTOs.Organization.Project;

/// <summary>
/// DTO for updating project charter information
/// </summary>
public class UpdateProjectCharterDto
{
    public string? ProjectCharter { get; set; }
    public string? BusinessCase { get; set; }
    public string? Objectives { get; set; }
    public string? Scope { get; set; }
    public string? Deliverables { get; set; }
    public string? SuccessCriteria { get; set; }
    public string? Assumptions { get; set; }
    public string? Constraints { get; set; }
}