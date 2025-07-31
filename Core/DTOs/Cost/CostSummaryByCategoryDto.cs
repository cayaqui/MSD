using Core.Enums.Cost;

namespace Core.DTOs.Cost;

/// <summary>
/// DTO for Cost Summary by category
/// </summary>
public class CostSummaryByCategoryDto
{
    public CostCategory Category { get; set; }
    public decimal PlannedCost { get; set; }
    public decimal ActualCost { get; set; }
    public decimal CommittedCost { get; set; }
    public decimal ForecastCost { get; set; }
    public decimal Variance { get; set; }
    public decimal VariancePercentage { get; set; }
    public int ItemCount { get; set; }
}
