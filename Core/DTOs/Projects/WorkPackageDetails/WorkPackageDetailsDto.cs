using Core.Enums.Progress;

namespace Core.DTOs.Projects.WorkPackageDetails;

/// <summary>
/// DTO for Work Package specific details
/// </summary>
public class WorkPackageDetailsDto
{
    public Guid WBSElementId { get; set; }
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public DateTime? BaselineStartDate { get; set; }
    public DateTime? BaselineEndDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public DateTime? ForecastStartDate { get; set; }
    public DateTime? ForecastEndDate { get; set; }
    public int PlannedDuration { get; set; }
    public int? ActualDuration { get; set; }
    public int? RemainingDuration { get; set; }
    public decimal Budget { get; set; }
    public decimal ActualCost { get; set; }
    public decimal CommittedCost { get; set; }
    public decimal ForecastCost { get; set; }
    public string Currency { get; set; } = "USD";
    public decimal ProgressPercentage { get; set; }
    public decimal PhysicalProgressPercentage { get; set; }
    public ProgressMethod ProgressMethod { get; set; }
    public WorkPackageStatus Status { get; set; }
    public decimal? WeightFactor { get; set; }
    public Guid? ResponsibleUserId { get; set; }
    public string? ResponsibleUserName { get; set; }
    public Guid? PrimaryDisciplineId { get; set; }
    public string? PrimaryDisciplineName { get; set; }
    public decimal? CPI { get; set; }
    public decimal? SPI { get; set; }
    public decimal EarnedValue { get; set; }
    public decimal PlannedValue { get; set; }
    public bool IsBaselined { get; set; }
    public DateTime? BaselineDate { get; set; }
    public bool IsCriticalPath { get; set; }
    public decimal TotalFloat { get; set; }
    public decimal FreeFloat { get; set; }
}
