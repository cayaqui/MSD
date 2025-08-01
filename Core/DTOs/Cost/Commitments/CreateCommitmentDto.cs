using Core.Enums.Cost;
using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.Cost.Commitments;

/// <summary>
/// DTO for creating a new Commitment
/// </summary>
public class CreateCommitmentDto
{
    // Project Assignment
    [Required(ErrorMessage = "Project is required")]
    public Guid ProjectId { get; set; }

    // Optional Assignments
    public Guid? BudgetItemId { get; set; }
    public Guid? ContractorId { get; set; }
    public Guid? ControlAccountId { get; set; }

    // Commitment Information
    [Required(ErrorMessage = "Commitment number is required")]
    [StringLength(50, ErrorMessage = "Commitment number cannot exceed 50 characters")]
    public string CommitmentNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Commitment type is required")]
    public CommitmentType Type { get; set; }

    // Financial Information
    [Required(ErrorMessage = "Original amount is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Original amount must be greater than zero")]
    public decimal OriginalAmount { get; set; }

    [Required(ErrorMessage = "Currency is required")]
    [StringLength(3, MinimumLength = 3, ErrorMessage = "Currency code must be 3 characters")]
    public string Currency { get; set; } = "USD";

    // Contract Terms
    [Required(ErrorMessage = "Contract date is required")]
    public DateTime ContractDate { get; set; }

    [Required(ErrorMessage = "Start date is required")]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "End date is required")]
    public DateTime EndDate { get; set; }

    [Range(1, 365, ErrorMessage = "Payment terms must be between 1 and 365 days")]
    public int? PaymentTermsDays { get; set; }

    [Range(0, 100, ErrorMessage = "Retention percentage must be between 0 and 100")]
    public decimal? RetentionPercentage { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Advance payment cannot be negative")]
    public decimal? AdvancePaymentAmount { get; set; }

    // References
    [StringLength(50, ErrorMessage = "Purchase order number cannot exceed 50 characters")]
    public string? PurchaseOrderNumber { get; set; }

    [StringLength(50, ErrorMessage = "Contract number cannot exceed 50 characters")]
    public string? ContractNumber { get; set; }

    [StringLength(50, ErrorMessage = "Vendor reference cannot exceed 50 characters")]
    public string? VendorReference { get; set; }

    [StringLength(50, ErrorMessage = "Accounting reference cannot exceed 50 characters")]
    public string? AccountingReference { get; set; }

    // Additional Information
    public string? Terms { get; set; }
    public string? ScopeOfWork { get; set; }
    public string? Deliverables { get; set; }
    public bool IsFixedPrice { get; set; }
    public bool IsTimeAndMaterial { get; set; }

    // Work Package Allocations (optional at creation)
    public List<CommitmentWorkPackageAllocationDto>? WorkPackageAllocations { get; set; }

    // Custom validation
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndDate < StartDate)
        {
            yield return new ValidationResult(
                "End date must be after or equal to start date",
                new[] { nameof(EndDate) });
        }

        if (StartDate < ContractDate)
        {
            yield return new ValidationResult(
                "Start date cannot be before contract date",
                new[] { nameof(StartDate) });
        }

        if (IsFixedPrice && IsTimeAndMaterial)
        {
            yield return new ValidationResult(
                "Commitment cannot be both Fixed Price and Time & Material",
                new[] { nameof(IsFixedPrice), nameof(IsTimeAndMaterial) });
        }

        if (!IsFixedPrice && !IsTimeAndMaterial)
        {
            yield return new ValidationResult(
                "Commitment must be either Fixed Price or Time & Material",
                new[] { nameof(IsFixedPrice), nameof(IsTimeAndMaterial) });
        }
    }
}

/// <summary>
/// DTO for allocating commitment amounts to work packages
/// </summary>
public class CommitmentWorkPackageAllocationDto
{
    [Required(ErrorMessage = "WBS Element is required")]
    public Guid WBSElementId { get; set; }

    [Required(ErrorMessage = "Allocated amount is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Allocated amount must be greater than zero")]
    public decimal AllocatedAmount { get; set; }
}