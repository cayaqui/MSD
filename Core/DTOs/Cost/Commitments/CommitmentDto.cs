using Core.Enums.Cost;

namespace Core.DTOs.Cost.Commitments;

/// <summary>
/// DTO for Commitment entity
/// </summary>
public class CommitmentDto
{
    public Guid Id { get; set; }

    // Foreign Keys
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string ProjectCode { get; set; } = string.Empty;

    public Guid? BudgetItemId { get; set; }
    public string? BudgetItemCode { get; set; }
    public string? BudgetItemName { get; set; }

    public Guid? ContractorId { get; set; }
    public string? ContractorCode { get; set; }
    public string? ContractorName { get; set; }

    public Guid? ControlAccountId { get; set; }
    public string? ControlAccountCode { get; set; }
    public string? ControlAccountName { get; set; }

    // Commitment Information
    public string CommitmentNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public CommitmentType Type { get; set; }
    public string TypeDescription => Type.ToString();
    public CommitmentStatus Status { get; set; }
    public string StatusDescription => Status.ToString();

    // Financial Information
    public decimal OriginalAmount { get; set; }
    public decimal RevisedAmount { get; set; }
    public decimal CommittedAmount { get; set; }
    public decimal InvoicedAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal RetentionAmount { get; set; }
    public string Currency { get; set; } = string.Empty;

    // Contract Terms
    public DateTime ContractDate { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int? PaymentTermsDays { get; set; }
    public decimal? RetentionPercentage { get; set; }
    public decimal? AdvancePaymentAmount { get; set; }

    // References
    public string? PurchaseOrderNumber { get; set; }
    public string? ContractNumber { get; set; }
    public string? VendorReference { get; set; }
    public string? AccountingReference { get; set; }

    // Approval Information
    public DateTime? ApprovalDate { get; set; }
    public string? ApprovedBy { get; set; }
    public string? ApprovalNotes { get; set; }

    // Performance
    public decimal? PerformancePercentage { get; set; }
    public DateTime? LastInvoiceDate { get; set; }
    public DateTime? ExpectedCompletionDate { get; set; }

    // Additional Information
    public string? Terms { get; set; }
    public string? ScopeOfWork { get; set; }
    public string? Deliverables { get; set; }
    public bool IsFixedPrice { get; set; }
    public bool IsTimeAndMaterial { get; set; }

    // Calculated Properties
    public decimal RemainingAmount => CommittedAmount - InvoicedAmount;
    public decimal UnpaidAmount => InvoicedAmount - PaidAmount;
    public decimal InvoicedPercentage => CommittedAmount > 0 ? InvoicedAmount / CommittedAmount * 100 : 0;
    public decimal PaidPercentage => InvoicedAmount > 0 ? PaidAmount / InvoicedAmount * 100 : 0;
    public bool IsOverCommitted => InvoicedAmount > CommittedAmount;
    public bool IsExpired => DateTime.UtcNow > EndDate && Status == CommitmentStatus.Active;
    public int DaysRemaining => (EndDate - DateTime.UtcNow).Days;
    public decimal BudgetVariance => RevisedAmount - OriginalAmount;
    public decimal BudgetVariancePercentage => OriginalAmount > 0 ? BudgetVariance / OriginalAmount * 100 : 0;

    // Metadata
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsActive { get; set; }
}