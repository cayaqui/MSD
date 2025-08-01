using Core.Enums.Cost;

namespace Core.DTOs.Cost.Commitments;

/// <summary>
/// Detailed DTO for Commitment including all related information
/// </summary>
public class CommitmentDetailDto : CommitmentDto
{
    // Related Collections
    public List<CommitmentWorkPackageDto> WorkPackageAllocations { get; set; } = new();
    public List<CommitmentRevisionDto> Revisions { get; set; } = new();
    public List<CommitmentInvoiceDto> Invoices { get; set; } = new();

    // Summary Information
    public CommitmentFinancialSummary FinancialSummary { get; set; } = new();
    public CommitmentPerformanceMetrics PerformanceMetrics { get; set; } = new();

    // Audit Trail
    public List<CommitmentAuditDto> AuditTrail { get; set; } = new();
}

/// <summary>
/// DTO for Commitment Work Package allocations
/// </summary>
public class CommitmentWorkPackageDto
{
    public Guid Id { get; set; }
    public Guid CommitmentId { get; set; }
    public Guid WBSElementId { get; set; }

    // WBS Information
    public string WBSCode { get; set; } = string.Empty;
    public string WBSName { get; set; } = string.Empty;
    public string? WBSDescription { get; set; }

    // Financial Information
    public decimal AllocatedAmount { get; set; }
    public decimal InvoicedAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal RetainedAmount { get; set; }
    public decimal PendingAmount => AllocatedAmount - InvoicedAmount;

    // Progress
    public decimal ProgressPercentage { get; set; }
    public DateTime? LastProgressUpdate { get; set; }

    // Calculated Properties
    public decimal UtilizationPercentage => AllocatedAmount > 0 ? InvoicedAmount / AllocatedAmount * 100 : 0;
    public decimal PaymentPercentage => InvoicedAmount > 0 ? PaidAmount / InvoicedAmount * 100 : 0;
    public bool IsFullyInvoiced => InvoicedAmount >= AllocatedAmount;
    public bool IsFullyPaid => PaidAmount >= InvoicedAmount - RetainedAmount;

    // Metadata
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// DTO for Commitment Revisions
/// </summary>
public class CommitmentRevisionDto
{
    public Guid Id { get; set; }
    public int RevisionNumber { get; set; }
    public DateTime RevisionDate { get; set; }

    // Amount Changes
    public decimal PreviousAmount { get; set; }
    public decimal RevisedAmount { get; set; }
    public decimal ChangeAmount { get; set; }
    public decimal ChangePercentage { get; set; }

    // Justification
    public string Reason { get; set; } = string.Empty;
    public string? ChangeOrderReference { get; set; }

    // Approval
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovalDate { get; set; }

    // Calculated Properties
    public bool IsIncrease => ChangeAmount > 0;
    public bool IsDecrease => ChangeAmount < 0;
    public bool IsSignificantChange => Math.Abs(ChangePercentage) > 10;
}

/// <summary>
/// DTO for related Invoices summary
/// </summary>
public class CommitmentInvoiceDto
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public InvoiceStatus Status { get; set; }
    public string StatusDescription => Status.ToString();

    // Amounts
    public decimal GrossAmount { get; set; }
    public decimal NetAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal RetentionAmount { get; set; }

    // Payment Information
    public DateTime? PaidDate { get; set; }
    public string? PaymentReference { get; set; }

    // Status Indicators
    public bool IsFullyPaid => PaidAmount >= TotalAmount;
    public bool IsOverdue => DateTime.UtcNow > DueDate && !IsFullyPaid;
    public DateTime DueDate { get; set; }
}

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

/// <summary>
/// Audit entry for commitment changes
/// </summary>
public class CommitmentAuditDto
{
    public DateTime Timestamp { get; set; }
    public string Action { get; set; } = string.Empty;
    public string User { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? Comments { get; set; }
}