namespace Core.DTOs.Cost.Commitments;

/// <summary>
/// Financial summary for a commitment
/// </summary>
public class CommitmentFinancialSummary
{
    // Budget vs Actual
    public decimal OriginalBudget { get; set; }
    public decimal CurrentBudget { get; set; }
    public decimal BudgetVariance { get; set; }
    public decimal BudgetVariancePercentage { get; set; }

    // Commitment Progress
    public decimal TotalCommitted { get; set; }
    public decimal TotalInvoiced { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal TotalRetention { get; set; }
    public decimal TotalOutstanding { get; set; }

    // By Work Package
    public int TotalWorkPackages { get; set; }
    public int ActiveWorkPackages { get; set; }
    public decimal AverageWorkPackageUtilization { get; set; }
}
