using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.Cost.Commitments;

/// <summary>
/// DTO for updating an existing Commitment
/// </summary>
public class UpdateCommitmentDto
{
    // Optional updates to assignments
    public Guid? BudgetItemId { get; set; }
    public Guid? ContractorId { get; set; }
    public Guid? ControlAccountId { get; set; }

    // Commitment Information updates
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string? Title { get; set; }

    [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
    public string? Description { get; set; }

    // Contract Terms updates (only allowed in Draft/Approved status)
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    [Range(1, 365, ErrorMessage = "Payment terms must be between 1 and 365 days")]
    public int? PaymentTermsDays { get; set; }

    [Range(0, 100, ErrorMessage = "Retention percentage must be between 0 and 100")]
    public decimal? RetentionPercentage { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Advance payment cannot be negative")]
    public decimal? AdvancePaymentAmount { get; set; }

    // References updates
    [StringLength(50, ErrorMessage = "Purchase order number cannot exceed 50 characters")]
    public string? PurchaseOrderNumber { get; set; }

    [StringLength(50, ErrorMessage = "Contract number cannot exceed 50 characters")]
    public string? ContractNumber { get; set; }

    [StringLength(50, ErrorMessage = "Vendor reference cannot exceed 50 characters")]
    public string? VendorReference { get; set; }

    [StringLength(50, ErrorMessage = "Accounting reference cannot exceed 50 characters")]
    public string? AccountingReference { get; set; }

    // Additional Information updates
    public string? Terms { get; set; }
    public string? ScopeOfWork { get; set; }
    public string? Deliverables { get; set; }
    public bool? IsFixedPrice { get; set; }
    public bool? IsTimeAndMaterial { get; set; }

    // Performance updates
    [Range(0, 100, ErrorMessage = "Performance percentage must be between 0 and 100")]
    public decimal? PerformancePercentage { get; set; }
    public DateTime? ExpectedCompletionDate { get; set; }

    // Custom validation
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartDate.HasValue && EndDate.HasValue && EndDate < StartDate)
        {
            yield return new ValidationResult(
                "End date must be after or equal to start date",
                new[] { nameof(EndDate) });
        }

        if (IsFixedPrice == true && IsTimeAndMaterial == true)
        {
            yield return new ValidationResult(
                "Commitment cannot be both Fixed Price and Time & Material",
                new[] { nameof(IsFixedPrice), nameof(IsTimeAndMaterial) });
        }
    }
}

/// <summary>
/// DTO for revising commitment amount
/// </summary>
public class ReviseCommitmentDto
{
    [Required(ErrorMessage = "Revised amount is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Revised amount must be greater than zero")]
    public decimal RevisedAmount { get; set; }

    [Required(ErrorMessage = "Reason for revision is required")]
    [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters")]
    public string Reason { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "Change order reference cannot exceed 50 characters")]
    public string? ChangeOrderReference { get; set; }
}

/// <summary>
/// DTO for approving a commitment
/// </summary>
public class ApproveCommitmentDto
{
    [StringLength(500, ErrorMessage = "Approval notes cannot exceed 500 characters")]
    public string? ApprovalNotes { get; set; }
}

/// <summary>
/// DTO for rejecting/cancelling a commitment
/// </summary>
public class CancelCommitmentDto
{
    [Required(ErrorMessage = "Reason is required")]
    [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters")]
    public string Reason { get; set; } = string.Empty;
}