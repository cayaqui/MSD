namespace Core.DTOs.Cost;

/// <summary>
/// DTO for updating a Planning Package
/// </summary>
public class UpdatePlanningPackageDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Budget { get; set; }
    public DateTime? PlannedStartDate { get; set; }
    public DateTime? PlannedEndDate { get; set; }
}
