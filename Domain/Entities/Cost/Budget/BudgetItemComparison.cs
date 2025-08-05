namespace Domain.Entities.Cost.Budget;

/// <summary>
/// Result of budget item comparison
/// </summary>
public class BudgetItemComparison
{
    public string ItemCode { get; set; } = string.Empty;
    public decimal OriginalAmount { get; set; }
    public decimal NewAmount { get; set; }
    public decimal AmountDifference { get; set; }
    public decimal QuantityDifference { get; set; }
    public decimal UnitRateDifference { get; set; }

    public decimal AmountChangePercentage =>
        OriginalAmount != 0 ? AmountDifference / OriginalAmount * 100 : 0;
}