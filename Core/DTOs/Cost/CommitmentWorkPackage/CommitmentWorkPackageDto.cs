namespace Core.DTOs.Cost.CommitmentWorkPackage;

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
