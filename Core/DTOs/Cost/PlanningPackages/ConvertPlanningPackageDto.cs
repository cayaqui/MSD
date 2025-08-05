using Core.DTOs.Projects.WorkPackageDetails;

namespace Core.DTOs.Cost.PlanningPackages;

/// <summary>
/// DTO for converting Planning Package to Work Packages
/// </summary>
public class ConvertPlanningPackageDto
{
    public List<ConvertPlanningToWorkPackageDto> WorkPackages { get; set; } = new();
}
