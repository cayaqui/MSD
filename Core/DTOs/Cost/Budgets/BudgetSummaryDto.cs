namespace Core.DTOs.Cost.Budgets;

public class BudgetSummaryDto
{
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public decimal TotalBudget { get; set; }
    public decimal ActualCost { get; set; }
    public decimal CommittedCost { get; set; }
    public decimal ForecastAtCompletion { get; set; }
    public decimal VarianceAtCompletion { get; set; }
    public decimal CostPerformanceIndex { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; }
}