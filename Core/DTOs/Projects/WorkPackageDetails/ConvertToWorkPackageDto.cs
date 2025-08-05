using Core.Enums.Progress;

namespace Core.DTOs.Projects.WorkPackageDetails;

/// <summary>
/// DTO for converting to Work Package
/// </summary>
public class ConvertToWorkPackageDto
{
    public Guid ControlAccountId { get; set; }
    public ProgressMethod ProgressMethod { get; set; }
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public decimal Budget { get; set; }
    public string Currency { get; set; } = "USD";
    public Guid? ResponsibleUserId { get; set; }
    public Guid? PrimaryDisciplineId { get; set; }
    public decimal? WeightFactor { get; set; }
}
