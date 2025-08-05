using Core.DTOs.Cost.BudgetItems;

namespace Core.DTOs.Cost.Budgets;

public class BudgetComparisonDto
{
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public BudgetDto? BaselineBudget { get; set; }
    public BudgetDto? CurrentBudget { get; set; }
    public decimal VarianceAmount { get; set; }
    public decimal VariancePercentage { get; set; }
    public List<BudgetItemComparisonDto> ItemComparisons { get; set; } = new();
}