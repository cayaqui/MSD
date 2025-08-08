namespace Core.DTOs.Projects.PlanningPackage;

/// <summary>
/// DTO for bulk creating planning packages
/// </summary>
public class BulkCreatePlanningPackagesDto
{
    public List<CreatePlanningPackageDto> PlanningPackages { get; set; } = new();
}