using Core.Enums.Progress;

namespace Core.DTOs.WorkPackages;

/// <summary>
/// DTO for creating a Work Package
/// </summary>
public class CreateWorkPackageDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid ControlAccountId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? DisciplineId { get; set; }
    public string? ResponsibleUserId { get; set; }
    public decimal Budget { get; set; }
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public ProgressMethod ProgressMethod { get; set; }
    public decimal? WeightFactor { get; set; }
}
