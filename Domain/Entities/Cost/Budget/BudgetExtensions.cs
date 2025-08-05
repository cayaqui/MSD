using Domain.Common;

namespace Domain.Entities.Cost.Budget;

/// <summary>
/// Extension methods for Budget entity
/// </summary>
public static class BudgetExtensions
{
    #region Financial Analysis Extensions

    /// <summary>
    /// Checks if the budget is over-allocated beyond a specified tolerance
    /// </summary>
    /// <param name="budget">The budget to check</param>
    /// <param name="tolerancePercentage">Tolerance percentage (default 1%)</param>
    /// <returns>True if over-allocated beyond tolerance</returns>
    public static bool IsOverAllocatedBeyondTolerance(this Budget budget, decimal tolerancePercentage = 1.0m)
    {
        if (budget.TotalAmount == 0) return false;

        var tolerance = budget.TotalAmount * (tolerancePercentage / 100m);
        return budget.AllocatedAmount > budget.TotalAmount + tolerance;
    }

    /// <summary>
    /// Checks if the budget is fully allocated within tolerance
    /// </summary>
    /// <param name="budget">The budget to check</param>
    /// <param name="tolerancePercentage">Tolerance percentage (default 1%)</param>
    /// <returns>True if fully allocated within tolerance</returns>
    public static bool IsFullyAllocated(this Budget budget, decimal tolerancePercentage = 1.0m)
    {
        var tolerance = budget.TotalAmount * (tolerancePercentage / 100m);
        return Math.Abs(budget.UnallocatedAmount) <= tolerance;
    }

    /// <summary>
    /// Gets the variance between allocated and total amount
    /// </summary>
    /// <param name="budget">The budget</param>
    /// <returns>Variance amount (positive = over-allocated, negative = under-allocated)</returns>
    public static decimal GetAllocationVariance(this Budget budget)
    {
        return budget.AllocatedAmount - budget.TotalAmount;
    }

    /// <summary>
    /// Gets the variance percentage
    /// </summary>
    /// <param name="budget">The budget</param>
    /// <returns>Variance percentage</returns>
    public static decimal GetAllocationVariancePercentage(this Budget budget)
    {
        if (budget.TotalAmount == 0) return 0;
        return budget.GetAllocationVariance() / budget.TotalAmount * 100;
    }

    /// <summary>
    /// Gets the total budget in a specific currency using exchange rate
    /// </summary>
    /// <param name="budget">The budget</param>
    /// <param name="targetCurrency">Target currency</param>
    /// <param name="exchangeRate">Exchange rate to target currency</param>
    /// <returns>Total budget in target currency</returns>
    public static decimal GetTotalBudgetInCurrency(this Budget budget, string targetCurrency, decimal exchangeRate)
    {
        if (exchangeRate <= 0)
            throw new ArgumentException("Exchange rate must be positive", nameof(exchangeRate));

        return budget.TotalBudget * exchangeRate;
    }

    #endregion

    #region Status and Workflow Extensions

    /// <summary>
    /// Checks if the budget can be submitted for approval
    /// </summary>
    /// <param name="budget">The budget to check</param>
    /// <returns>True if can be submitted</returns>
    public static bool CanBeSubmitted(this Budget budget)
    {
        return budget.Status == BudgetStatus.Draft &&
               !budget.IsLocked &&
               budget.BudgetItems.Any(bi => !bi.IsDeleted);
    }

    /// <summary>
    /// Checks if the budget is in a final state (approved or baseline)
    /// </summary>
    /// <param name="budget">The budget to check</param>
    /// <returns>True if in final state</returns>
    public static bool IsInFinalState(this Budget budget)
    {
        return budget.Status == BudgetStatus.Approved || budget.IsBaseline;
    }

    /// <summary>
    /// Gets the number of days since last modification
    /// </summary>
    /// <param name="budget">The budget</param>
    /// <returns>Days since last update</returns>
    public static int GetDaysSinceLastUpdate(this Budget budget)
    {
        var lastUpdate = budget.UpdatedAt ?? budget.CreatedAt;
        return (DateTime.UtcNow - lastUpdate).Days;
    }

    #endregion

    #region Reporting Extensions

    /// <summary>
    /// Gets a summary of budget items grouped by cost type
    /// </summary>
    /// <param name="budget">The budget</param>
    /// <returns>Dictionary with cost type as key and total amount as value</returns>
    public static Dictionary<CostType, decimal> GetCostTypeSummary(this Budget budget)
    {
        return budget.BudgetItems
            .Where(bi => !bi.IsDeleted)
            .GroupBy(bi => bi.CostType)
            .ToDictionary(g => g.Key, g => g.Sum(bi => bi.Amount));
    }

    /// <summary>
    /// Gets a summary of budget items grouped by cost category
    /// </summary>
    /// <param name="budget">The budget</param>
    /// <returns>Dictionary with cost category as key and total amount as value</returns>
    public static Dictionary<CostCategory, decimal> GetCostCategorySummary(this Budget budget)
    {
        return budget.BudgetItems
            .Where(bi => !bi.IsDeleted)
            .GroupBy(bi => bi.Category)
            .ToDictionary(g => g.Key, g => g.Sum(bi => bi.Amount));
    }

    /// <summary>
    /// Gets budget items that exceed a specified amount threshold
    /// </summary>
    /// <param name="budget">The budget</param>
    /// <param name="threshold">Amount threshold</param>
    /// <returns>Budget items exceeding threshold</returns>
    public static IEnumerable<BudgetItem> GetHighValueItems(this Budget budget, decimal threshold)
    {
        return budget.BudgetItems
            .Where(bi => !bi.IsDeleted && bi.Amount >= threshold)
            .OrderByDescending(bi => bi.Amount);
    }

    #endregion

    #region Validation Extensions

    /// <summary>
    /// Validates the budget for common issues
    /// </summary>
    /// <param name="budget">The budget to validate</param>
    /// <returns>List of validation issues</returns>
    public static List<string> ValidateForIssues(this Budget budget)
    {
        var issues = new List<string>();

        if (budget.IsOverAllocated)
            issues.Add($"Budget is over-allocated by {budget.GetAllocationVariance():C}");

        if (!budget.BudgetItems.Any(bi => !bi.IsDeleted))
            issues.Add("Budget has no active items");

        if (budget.TotalAmount <= 0)
            issues.Add("Total amount must be greater than zero");

        if (budget.ContingencyPercentage > 20)
            issues.Add($"Contingency percentage ({budget.ContingencyPercentage}%) seems high");

        if (budget.ManagementReservePercentage > 15)
            issues.Add($"Management reserve percentage ({budget.ManagementReservePercentage}%) seems high");

        return issues;
    }

    #endregion
}

/// <summary>
/// Extension methods for BudgetItem entity
/// </summary>
public static class BudgetItemExtensions
{
    #region Calculation Extensions

    /// <summary>
    /// Calculates the unit rate from amount and quantity
    /// </summary>
    /// <param name="item">The budget item</param>
    /// <returns>Calculated unit rate</returns>
    public static decimal GetCalculatedUnitRate(this BudgetItem item)
    {
        return item.Quantity > 0 ? item.Amount / item.Quantity : 0;
    }

    /// <summary>
    /// Checks if the item's amount matches quantity * unit rate
    /// </summary>
    /// <param name="item">The budget item</param>
    /// <param name="tolerance">Tolerance for comparison (default 0.01)</param>
    /// <returns>True if amounts match within tolerance</returns>
    public static bool IsAmountConsistent(this BudgetItem item, decimal tolerance = 0.01m)
    {
        var calculatedAmount = item.Quantity * item.UnitRate;
        return Math.Abs(item.Amount - calculatedAmount) <= tolerance;
    }

    #endregion

    #region Comparison Extensions

    /// <summary>
    /// Compares this item with another budget item
    /// </summary>
    /// <param name="item">The budget item</param>
    /// <param name="otherItem">Item to compare with</param>
    /// <returns>Comparison result</returns>
    public static BudgetItemComparison CompareTo(this BudgetItem item, BudgetItem otherItem)
    {
        return new BudgetItemComparison
        {
            ItemCode = item.ItemCode,
            OriginalAmount = otherItem.Amount,
            NewAmount = item.Amount,
            AmountDifference = item.Amount - otherItem.Amount,
            QuantityDifference = item.Quantity - otherItem.Quantity,
            UnitRateDifference = item.UnitRate - otherItem.UnitRate
        };
    }

    #endregion

    #region Validation Extensions

    /// <summary>
    /// Validates the budget item for common issues
    /// </summary>
    /// <param name="item">The budget item to validate</param>
    /// <returns>List of validation issues</returns>
    public static List<string> ValidateForIssues(this BudgetItem item)
    {
        var issues = new List<string>();

        if (item.Amount <= 0)
            issues.Add("Amount must be greater than zero");

        if (item.Quantity <= 0)
            issues.Add("Quantity must be greater than zero");

        if (item.UnitRate <= 0)
            issues.Add("Unit rate must be greater than zero");

        if (!item.IsAmountConsistent())
            issues.Add("Amount is not consistent with quantity � unit rate");

        if (string.IsNullOrWhiteSpace(item.Description))
            issues.Add("Description is required");

        return issues;
    }

    #endregion
}
