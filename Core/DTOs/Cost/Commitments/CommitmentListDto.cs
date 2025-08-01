using Core.Enums.Cost;

namespace Core.DTOs.Cost.Commitments;

/// <summary>
/// Simplified DTO for Commitment lists and grids
/// </summary>
public class CommitmentListDto
{
    public Guid Id { get; set; }

    // Basic Information
    public string CommitmentNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public CommitmentType Type { get; set; }
    public string TypeDescription => Type.ToString();
    public CommitmentStatus Status { get; set; }
    public string StatusDescription => Status.ToString();

    // Contractor
    public Guid? ContractorId { get; set; }
    public string? ContractorCode { get; set; }
    public string? ContractorName { get; set; }

    // Financial Summary
    public decimal OriginalAmount { get; set; }
    public decimal RevisedAmount { get; set; }
    public decimal CommittedAmount { get; set; }
    public decimal InvoicedAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public string Currency { get; set; } = string.Empty;

    // Key Dates
    public DateTime ContractDate { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    // Performance Indicators
    public decimal InvoicedPercentage => CommittedAmount > 0 ? InvoicedAmount / CommittedAmount * 100 : 0;
    public decimal PaidPercentage => InvoicedAmount > 0 ? PaidAmount / InvoicedAmount * 100 : 0;
    public decimal RemainingAmount => CommittedAmount - InvoicedAmount;
    public bool IsOverCommitted => InvoicedAmount > CommittedAmount;
    public bool IsExpired => DateTime.UtcNow > EndDate && Status == CommitmentStatus.Active;

    // References
    public string? PurchaseOrderNumber { get; set; }
    public string? ContractNumber { get; set; }

    // Status Flags
    public bool HasRetention => RetentionPercentage > 0;
    public decimal? RetentionPercentage { get; set; }
    public bool IsActive { get; set; }

    // Metadata
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Filter DTO for searching commitments
/// </summary>
public class CommitmentFilterDto
{
    public Guid? ProjectId { get; set; }
    public Guid? ContractorId { get; set; }
    public Guid? ControlAccountId { get; set; }
    public Guid? BudgetItemId { get; set; }

    public CommitmentType? Type { get; set; }
    public CommitmentStatus? Status { get; set; }

    public string? SearchText { get; set; } // Search in number, title, references
    public string? CommitmentNumber { get; set; }
    public string? ContractNumber { get; set; }
    public string? PurchaseOrderNumber { get; set; }

    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }

    public decimal? AmountMin { get; set; }
    public decimal? AmountMax { get; set; }

    public bool? IsActive { get; set; }
    public bool? IsOverCommitted { get; set; }
    public bool? IsExpired { get; set; }

    // Pagination
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;

    // Sorting
    public string? SortBy { get; set; } = "CommitmentNumber";
    public bool SortDescending { get; set; } = false;
}

/// <summary>
/// Summary DTO for commitment statistics
/// </summary>
public class CommitmentSummaryDto
{
    public Guid ProjectId { get; set; }

    // Counts by Status
    public int TotalCommitments { get; set; }
    public int DraftCount { get; set; }
    public int ActiveCount { get; set; }
    public int ClosedCount { get; set; }

    // Financial Summary
    public decimal TotalOriginalAmount { get; set; }
    public decimal TotalRevisedAmount { get; set; }
    public decimal TotalCommittedAmount { get; set; }
    public decimal TotalInvoicedAmount { get; set; }
    public decimal TotalPaidAmount { get; set; }
    public decimal TotalRetentionAmount { get; set; }

    // Performance Metrics
    public decimal AverageInvoicedPercentage { get; set; }
    public decimal AveragePaidPercentage { get; set; }
    public int OverCommittedCount { get; set; }
    public int ExpiredCount { get; set; }

    // By Type
    public Dictionary<string, int> CountByType { get; set; } = new();
    public Dictionary<string, decimal> AmountByType { get; set; } = new();
}