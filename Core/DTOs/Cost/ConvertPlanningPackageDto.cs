namespace Core.DTOs.Cost;

/// <summary>
/// DTO for converting Planning Package to Work Packages
/// </summary>
public class ConvertPlanningPackageDto
{
    public List<CreateWorkPackageFromPlanningDto> WorkPackages { get; set; } = new();
}
