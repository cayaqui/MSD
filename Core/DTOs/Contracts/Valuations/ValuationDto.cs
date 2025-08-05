using Core.DTOs.Common;
using Core.DTOs.Contracts.Contracts;
using Core.DTOs.Contracts.ValuationItems;
using Core.Enums.Contracts;

namespace Core.DTOs.Contracts.Valuations;

public class ValuationDto : BaseDto
{
    public string ValuationNumber { get; set; } = string.Empty;
    public int ValuationPeriod { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    public Guid ContractId { get; set; }
    public ContractDto? Contract { get; set; }
    
    public ValuationStatus Status { get; set; }
    
    // Period
    public DateTime PeriodStartDate { get; set; }
    public DateTime PeriodEndDate { get; set; }
    public DateTime ValuationDate { get; set; }
    public DateTime SubmissionDate { get; set; }
    public DateTime? DueDate { get; set; }
    
    // Work Done
    public decimal PreviouslyCompletedWork { get; set; }
    public decimal CurrentPeriodWork { get; set; }
    public decimal TotalCompletedWork { get; set; }
    public decimal PercentageComplete { get; set; }
    
    // Materials
    public decimal MaterialsOnSite { get; set; }
    public decimal MaterialsOffSite { get; set; }
    public decimal TotalMaterials { get; set; }
    
    // Variations/Change Orders
    public decimal ApprovedVariations { get; set; }
    public decimal PendingVariations { get; set; }
    
    // Amounts
    public decimal GrossValuation { get; set; }
    public decimal LessRetention { get; set; }
    public decimal RetentionAmount { get; set; }
    public decimal LessPreviousCertificates { get; set; }
    public decimal NetValuation { get; set; }
    public string Currency { get; set; } = "USD";
    
    // Deductions
    public decimal AdvancePaymentRecovery { get; set; }
    public decimal Penalties { get; set; }
    public decimal OtherDeductions { get; set; }
    public decimal TotalDeductions { get; set; }
    
    // Payment
    public decimal AmountDue { get; set; }
    public bool IsInvoiced { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime? InvoiceDate { get; set; }
    public bool IsPaid { get; set; }
    public DateTime? PaymentDate { get; set; }
    public decimal PaymentAmount { get; set; }
    
    // Approval
    public bool IsApproved { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public string ApprovedBy { get; set; } = string.Empty;
    public string ApprovalComments { get; set; } = string.Empty;
    public DateTime? RejectionDate { get; set; }
    public string RejectedBy { get; set; } = string.Empty;
    public string RejectionReason { get; set; } = string.Empty;
    
    // Work Items
    public List<ValuationItemDto> Items { get; set; } = new();
    
    // Documents
    public int AttachmentCount { get; set; }
    public bool HasProgressPhotos { get; set; }
    public bool HasMeasurementSheets { get; set; }
    public bool HasQualityDocuments { get; set; }
    
    public bool IsActive { get; set; } = true;
    public string Notes { get; set; } = string.Empty;
    public Dictionary<string, object> Metadata { get; set; } = new();
}
