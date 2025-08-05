using Core.Enums.Cost;

namespace Core.DTOs.Cost.CostControlReports;

/// <summary>
/// DTO for cost summary by category
/// </summary>
public class CostSummaryByCategoryDto
{
    public CostCategory Category { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int ItemCount { get; set; }
    public decimal PlannedCost { get; set; }
    public decimal ActualCost { get; set; }
    public decimal CommittedCost { get; set; }
    public decimal ForecastCost { get; set; }
    public decimal Variance { get; set; }
    public decimal VariancePercentage { get; set; }
}