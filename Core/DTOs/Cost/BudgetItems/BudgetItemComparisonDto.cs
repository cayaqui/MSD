namespace Core.DTOs.Cost.BudgetItems;

public class BudgetItemComparisonDto
{
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public decimal BaselineAmount { get; set; }
    public decimal CurrentAmount { get; set; }
    public decimal VarianceAmount { get; set; }
    public decimal VariancePercentage { get; set; }
    public string? Reason { get; set; }
}