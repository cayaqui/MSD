namespace Core.DTOs.Organization.Project;

/// <summary>
/// DTO for updating project costs
/// </summary>
public class UpdateProjectCostsDto
{
    public decimal? ActualCost { get; set; }
    public decimal? CommittedCost { get; set; }
}