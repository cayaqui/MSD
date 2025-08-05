using Core.DTOs.Cost.CostItems;
using Core.Enums.Progress;

namespace Core.DTOs.WorkPackages;

/// <summary>
/// DTO for Work Package detail view
/// </summary>
public class WorkPackageDetailDto : WorkPackageDto
{
    public int WBSLevel { get; set; }
    public Guid? DisciplineId { get; set; }
    public string? DisciplineName { get; set; }
    public int PlannedDuration { get; set; }
    public int? ActualDuration { get; set; }
    public ProgressMethod ProgressMethod { get; set; }
    public decimal? WeightFactor { get; set; }
    public DateTime? BaselineDate { get; set; }
    public decimal RemainingCost { get; set; }
    public decimal CostVariance { get; set; }
    public decimal ScheduleVariance { get; set; }
    public List<ActivityDto> Activities { get; set; } = new();
    public List<CostItemDto> CostItems { get; set; } = new();
    public List<ResourceDto> Resources { get; set; } = new();
    public WorkPackageProgressDto? LatestProgress { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
