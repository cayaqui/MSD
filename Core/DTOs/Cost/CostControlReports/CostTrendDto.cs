namespace Core.DTOs.Cost.CostControlReports;

/// <summary>
/// DTO for cost trend data
/// </summary>
public class CostTrendDto
{
    public DateTime Date { get; set; }
    public decimal PlannedCost { get; set; }
    public decimal ActualCost { get; set; }
    public decimal CommittedCost { get; set; }
    public decimal ForecastCost { get; set; }
    public decimal EarnedValue { get; set; }
}