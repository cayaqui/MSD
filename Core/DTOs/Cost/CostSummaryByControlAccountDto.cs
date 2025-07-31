namespace Core.DTOs.Cost;

/// <summary>
/// DTO for Cost Summary by Control Account
/// </summary>
public class CostSummaryByControlAccountDto
{
    public Guid ControlAccountId { get; set; }
    public string ControlAccountCode { get; set; } = string.Empty;
    public string ControlAccountName { get; set; } = string.Empty;
    public decimal Budget { get; set; }
    public decimal PlannedCost { get; set; }
    public decimal ActualCost { get; set; }
    public decimal CommittedCost { get; set; }
    public decimal ForecastCost { get; set; }
    public decimal CostVariance { get; set; }
    public decimal CostVariancePercentage { get; set; }
    public List<CostSummaryByCategoryDto> CategoriesBreakdown { get; set; } = new();
}
