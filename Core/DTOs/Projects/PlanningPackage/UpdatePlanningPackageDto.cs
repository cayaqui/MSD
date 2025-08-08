namespace Core.DTOs.Projects.PlanningPackage;

/// <summary>
/// DTO for updating planning package basic information
/// </summary>
public class UpdatePlanningPackageDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
}