namespace Core.DTOs.Cost;

/// <summary>
/// DTO for Project Cost Report
/// </summary>
public class ProjectCostReportDto
{
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public DateTime ReportDate { get; set; }
    public string Currency { get; set; } = string.Empty;
    public decimal TotalBudget { get; set; }
    public decimal TotalPlannedCost { get; set; }
    public decimal TotalActualCost { get; set; }
    public decimal TotalCommittedCost { get; set; }
    public decimal TotalForecastCost { get; set; }
    public decimal TotalCostVariance { get; set; }
    public decimal TotalCostVariancePercentage { get; set; }
    public decimal ContingencyReserve { get; set; }
    public decimal ContingencyUsed { get; set; }
    public decimal ManagementReserve { get; set; }
    public List<CostSummaryByControlAccountDto> ControlAccountsBreakdown { get; set; } = new();
    public List<CostTrendDto> CostTrends { get; set; } = new();
}
