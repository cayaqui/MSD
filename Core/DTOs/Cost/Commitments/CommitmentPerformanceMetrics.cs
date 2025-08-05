namespace Core.DTOs.Cost.Commitments;

/// <summary>
/// Performance metrics for a commitment
/// </summary>
public class CommitmentPerformanceMetrics
{
    // Schedule Performance
    public int TotalDays => (EndDate - StartDate).Days;
    public int ElapsedDays => (DateTime.UtcNow - StartDate).Days;
    public int RemainingDays => (EndDate - DateTime.UtcNow).Days;
    public decimal TimeElapsedPercentage => TotalDays > 0 ? ElapsedDays / (decimal)TotalDays * 100 : 0;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    // Financial Performance
    public decimal InvoicingEfficiency { get; set; } // Invoiced vs Time Elapsed
    public decimal PaymentEfficiency { get; set; } // Paid vs Invoiced
    public decimal CostPerformanceIndex { get; set; } // CPI if applicable

    // Quality Metrics
    public int TotalInvoices { get; set; }
    public int ApprovedInvoices { get; set; }
    public int RejectedInvoices { get; set; }
    public decimal InvoiceApprovalRate { get; set; }

    // Risk Indicators
    public bool IsDelayed { get; set; }
    public bool IsBudgetExceeded { get; set; }
    public decimal RiskScore { get; set; } // 0-100 scale
}
