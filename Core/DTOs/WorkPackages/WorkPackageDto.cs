using Core.Enums.Progress;

namespace Core.DTOs.WorkPackages;

/// <summary>
/// DTO for Work Package list view
/// </summary>
public class WorkPackageDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid ControlAccountId { get; set; }
    public string ControlAccountCode { get; set; } = string.Empty;
    public string ControlAccountName { get; set; } = string.Empty;
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string? ResponsibleUserId { get; set; }
    public string? ResponsibleUserName { get; set; }
    public decimal Budget { get; set; }
    public decimal ActualCost { get; set; }
    public decimal CommittedCost { get; set; }
    public decimal ForecastCost { get; set; }
    public decimal ProgressPercentage { get; set; }
    public WorkPackageStatus Status { get; set; }
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public bool IsBaselined { get; set; }
    public bool IsActive { get; set; }
}
