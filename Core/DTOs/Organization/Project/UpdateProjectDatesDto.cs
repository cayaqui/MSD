namespace Core.DTOs.Organization.Project;

/// <summary>
/// DTO for updating project dates
/// </summary>
public class UpdateProjectDatesDto
{
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
}