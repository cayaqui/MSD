namespace Core.DTOs.Cost.ControlAccounts;

/// <summary>
/// DTO for Control Account detail view
/// </summary>
public class ControlAccountDetailDto : ControlAccountDto
{
    public decimal ContingencyReserve { get; set; }
    public decimal ManagementReserve { get; set; }
    public int WorkPackageCount { get; set; }
    public int PlanningPackageCount { get; set; }
    public decimal ActualCost { get; set; }
    public decimal EarnedValue { get; set; }
    public decimal PlannedValue { get; set; }
    public decimal CPI { get; set; }
    public decimal SPI { get; set; }
    public List<WorkPackageSummaryDto> WorkPackages { get; set; } = new();
    public List<ControlAccountAssignmentDto> Assignments { get; set; } = new();
    public EVMSummaryDto? LatestEVM { get; set; }
}
