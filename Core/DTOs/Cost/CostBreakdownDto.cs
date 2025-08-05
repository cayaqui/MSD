namespace Core.DTOs.Cost;

/// <summary>
/// DTO for cost breakdown by category
/// </summary>
public class CostBreakdownDto
{
    public string Category { get; set; } = string.Empty;
    public decimal PlannedCost { get; set; }
    public decimal ActualCost { get; set; }
    public decimal CommittedCost { get; set; }
    public decimal ForecastCost { get; set; }
    public int Count { get; set; }
    public decimal Variance => ActualCost - PlannedCost;
    public decimal VariancePercentage => PlannedCost > 0 ? Variance / PlannedCost * 100 : 0;
}