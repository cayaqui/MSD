namespace Core.DTOs.Cost.CostControlReports;

/// <summary>
/// DTO for cost summary by control account
/// </summary>
public class CostSummaryByControlAccountDto
{
    public Guid ControlAccountId { get; set; }
    public string ControlAccountCode { get; set; } = string.Empty;
    public string ControlAccountName { get; set; } = string.Empty;
    public int ItemCount { get; set; }
    public decimal PlannedCost { get; set; }
    public decimal ActualCost { get; set; }
    public decimal CommittedCost { get; set; }
    public decimal ForecastCost { get; set; }
    public decimal Variance { get; set; }
    public decimal VariancePercentage { get; set; }
}