namespace Core.DTOs.Projects.PlanningPackage;

/// <summary>
/// DTO for bulk converting planning packages to work packages
/// </summary>
public class BulkConvertPlanningPackagesDto
{
    public List<ConvertPlanningPackageItemDto> Items { get; set; } = new();
}