using Core.Enums.Progress;

namespace Core.DTOs.Projects.PlanningPackage;

/// <summary>
/// DTO for individual planning package conversion item
/// </summary>
public class ConvertPlanningPackageItemDto
{
    public Guid PlanningPackageId { get; set; }
    public ProgressMethod ProgressMethod { get; set; }
    public Guid? ResponsibleUserId { get; set; }
    public Guid? PrimaryDisciplineId { get; set; }
}