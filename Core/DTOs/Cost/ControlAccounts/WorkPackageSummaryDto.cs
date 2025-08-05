using Core.Enums.Progress;

namespace Core.DTOs.Cost.ControlAccounts;

/// <summary>
/// Summary DTO for Work Packages
/// </summary>
public class WorkPackageSummaryDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Budget { get; set; }
    public decimal ActualCost { get; set; }
    public decimal ProgressPercentage { get; set; }
    public WorkPackageStatus Status { get; set; }
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
}
