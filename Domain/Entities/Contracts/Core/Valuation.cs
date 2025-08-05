using Domain.Common;
using Core.Enums.Contracts;

namespace Domain.Entities.Contracts.Core;

public class Valuation : BaseAuditableEntity
{
    public string ValuationNumber { get; set; } = string.Empty;
    public int ValuationPeriod { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    public Guid ContractId { get; set; }
    public virtual Contract Contract { get; set; } = null!;
    
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
    
    // Documents
    public bool HasProgressPhotos { get; set; }
    public bool HasMeasurementSheets { get; set; }
    public bool HasQualityDocuments { get; set; }
    
    public bool IsActive { get; set; } = true;
    public string Notes { get; set; } = string.Empty;
    public string? Metadata { get; set; }
    
    // Navigation Properties
    public virtual ICollection<ValuationItem> Items { get; set; } = new List<ValuationItem>();
    public virtual ICollection<ValuationDocument> Documents { get; set; } = new List<ValuationDocument>();
    
    // Calculated Properties
    public int AttachmentCount => Documents?.Count(d => d.IsActive) ?? 0;
    
    // Methods
    public void CalculateAmounts()
    {
        // Calculate work done
        CurrentPeriodWork = Items?.Where(i => i.IsActive).Sum(i => i.CurrentAmount) ?? 0;
        TotalCompletedWork = PreviouslyCompletedWork + CurrentPeriodWork;
        
        // Calculate gross valuation
        GrossValuation = TotalCompletedWork + TotalMaterials + ApprovedVariations;
        
        // Calculate retention
        if (Contract != null)
        {
            RetentionAmount = GrossValuation * (Contract.RetentionPercentage / 100);
        }
        
        // Calculate deductions
        TotalDeductions = AdvancePaymentRecovery + Penalties + OtherDeductions;
        
        // Calculate net valuation
        NetValuation = GrossValuation - RetentionAmount - LessPreviousCertificates;
        
        // Calculate amount due
        AmountDue = NetValuation - TotalDeductions;
        
        // Update percentage complete
        if (Contract != null && Contract.CurrentValue > 0)
        {
            PercentageComplete = TotalCompletedWork / Contract.CurrentValue * 100;
        }
    }
    
    public void Submit(string submittedBy)
    {
        Status = ValuationStatus.Submitted;
        SubmissionDate = DateTime.UtcNow;
        CreatedBy = submittedBy;
    }
    
    public void Approve(string approvedBy, string comments = "", decimal? adjustedAmount = null)
    {
        IsApproved = true;
        Status = ValuationStatus.Approved;
        ApprovalDate = DateTime.UtcNow;
        ApprovedBy = approvedBy;
        ApprovalComments = comments;
        
        if (adjustedAmount.HasValue)
        {
            AmountDue = adjustedAmount.Value;
        }
    }
    
    public void Reject(string rejectedBy, string reason)
    {
        IsApproved = false;
        Status = ValuationStatus.Rejected;
        RejectionDate = DateTime.UtcNow;
        RejectedBy = rejectedBy;
        RejectionReason = reason;
    }
    
    public void Certify()
    {
        Status = ValuationStatus.Certified;
    }
    
    public void RecordInvoice(string invoiceNumber)
    {
        IsInvoiced = true;
        InvoiceNumber = invoiceNumber;
        InvoiceDate = DateTime.UtcNow;
        Status = ValuationStatus.Invoiced;
    }
    
    public void RecordPayment(decimal amount)
    {
        IsPaid = true;
        PaymentDate = DateTime.UtcNow;
        PaymentAmount = amount;
        Status = ValuationStatus.Paid;
    }
}