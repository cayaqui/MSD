using Domain.Common;
using Domain.Entities.Cost.Commitments;
using Domain.Entities.Organization.Core;
using Core.Enums.Cost;

namespace Domain.Entities.Cost.Core;

/// <summary>
/// Represents an invoice for cost tracking according to PMBOK
/// </summary>
public class Invoice : BaseEntity, IAuditable, ISoftDelete
{
    // Foreign Keys
    public Guid CommitmentId { get; private set; }
    public Commitment Commitment { get; private set; } = null!;

    public Guid? ContractorId { get; private set; }
    public Contractor? Contractor { get; private set; }

    // Invoice Information
    public string InvoiceNumber { get; private set; } = string.Empty;
    public string? VendorInvoiceNumber { get; private set; }
    public InvoiceType Type { get; private set; }
    public InvoiceStatus Status { get; private set; }

    // Dates
    public DateTime InvoiceDate { get; private set; }
    public DateTime ReceivedDate { get; private set; }
    public DateTime DueDate { get; private set; }
    public DateTime? ApprovedDate { get; private set; }
    public DateTime? PaidDate { get; private set; }

    // Financial Information
    public decimal GrossAmount { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal NetAmount { get; private set; }
    public decimal RetentionAmount { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public decimal PaidAmount { get; private set; }
    public string Currency { get; private set; } = string.Empty;

    // Tax Information
    public decimal? TaxRate { get; private set; }
    public string? TaxCode { get; private set; }
    public bool IsTaxExempt { get; private set; }

    // Period Information
    public DateTime PeriodStartDate { get; private set; }
    public DateTime PeriodEndDate { get; private set; }
    public int? InvoiceSequence { get; private set; }

    // Payment Information
    public string? PaymentReference { get; private set; }
    public DateTime? PaymentDate { get; private set; }
    public string? PaymentMethod { get; private set; }
    public string? BankReference { get; private set; }

    // Approval Workflow
    public string? SubmittedBy { get; private set; }
    public DateTime? SubmittedDate { get; private set; }
    public string? ReviewedBy { get; private set; }
    public DateTime? ReviewedDate { get; private set; }
    public string? ApprovedBy { get; private set; }
    public string? ApprovalNotes { get; private set; }
    public string? RejectionReason { get; private set; }

    // Supporting Documents
    public string? Description { get; private set; }
    public string? SupportingDocuments { get; private set; }
    public bool HasBackup { get; private set; }
    public bool IsVerified { get; private set; }

    // ISoftDelete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // Navigation Properties
    public ICollection<InvoiceItem> InvoiceItems { get; private set; } = new List<InvoiceItem>();

    private Invoice() { } // EF Core

    public Invoice(
        Guid commitmentId,
        string invoiceNumber,
        InvoiceType type,
        DateTime invoiceDate,
        DateTime dueDate,
        decimal grossAmount,
        string currency,
        DateTime periodStartDate,
        DateTime periodEndDate
    )
    {
        CommitmentId = commitmentId;
        InvoiceNumber = invoiceNumber ?? throw new ArgumentNullException(nameof(invoiceNumber));
        Type = type;
        InvoiceDate = invoiceDate;
        ReceivedDate = DateTime.UtcNow;
        DueDate = dueDate;
        GrossAmount = grossAmount;
        NetAmount = grossAmount;
        TotalAmount = grossAmount;
        Currency = currency ?? throw new ArgumentNullException(nameof(currency));
        PeriodStartDate = periodStartDate;
        PeriodEndDate = periodEndDate;
        Status = InvoiceStatus.Draft;
        TaxAmount = 0;
        RetentionAmount = 0;
        DiscountAmount = 0;
        PaidAmount = 0;
        CreatedAt = DateTime.UtcNow;
    }

    // Methods
    public void AssignToContractor(Guid contractorId)
    {
        ContractorId = contractorId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetVendorReference(string? vendorInvoiceNumber)
    {
        VendorInvoiceNumber = vendorInvoiceNumber;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateFinancialAmounts(
        decimal grossAmount,
        decimal taxAmount,
        decimal retentionAmount,
        decimal discountAmount
    )
    {
        if (Status != InvoiceStatus.Draft && Status != InvoiceStatus.Rejected)
            throw new InvalidOperationException("Cannot modify amounts after submission");

        GrossAmount = grossAmount;
        TaxAmount = taxAmount;
        RetentionAmount = retentionAmount;
        DiscountAmount = discountAmount;

        NetAmount = grossAmount - discountAmount;
        TotalAmount = NetAmount + taxAmount - retentionAmount;

        UpdatedAt = DateTime.UtcNow;
    }

    public void SetTaxInformation(decimal? taxRate, string? taxCode, bool isTaxExempt)
    {
        TaxRate = taxRate;
        TaxCode = taxCode;
        IsTaxExempt = isTaxExempt;

        if (isTaxExempt)
        {
            TaxAmount = 0;
            TotalAmount = NetAmount - RetentionAmount;
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDescription(string? description, string? supportingDocuments)
    {
        Description = description;
        SupportingDocuments = supportingDocuments;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetDocumentationStatus(bool hasBackup, bool isVerified)
    {
        HasBackup = hasBackup;
        IsVerified = isVerified;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Submit(string submittedBy)
    {
        if (Status != InvoiceStatus.Draft)
            throw new InvalidOperationException("Only draft invoices can be submitted");

        Status = InvoiceStatus.Submitted;
        SubmittedBy = submittedBy;
        SubmittedDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void StartReview(string reviewedBy)
    {
        if (Status != InvoiceStatus.Submitted)
            throw new InvalidOperationException("Only submitted invoices can be reviewed");

        Status = InvoiceStatus.UnderReview;
        ReviewedBy = reviewedBy;
        ReviewedDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Approve(string approvedBy, string? notes = null)
    {
        if (Status != InvoiceStatus.UnderReview)
            throw new InvalidOperationException("Only invoices under review can be approved");

        Status = InvoiceStatus.Approved;
        ApprovedBy = approvedBy;
        ApprovedDate = DateTime.UtcNow;
        ApprovalNotes = notes;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reject(string rejectedBy, string reason)
    {
        if (Status != InvoiceStatus.UnderReview)
            throw new InvalidOperationException("Only invoices under review can be rejected");

        Status = InvoiceStatus.Rejected;
        ReviewedBy = rejectedBy;
        ReviewedDate = DateTime.UtcNow;
        RejectionReason = reason;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RecordPayment(
        decimal paidAmount,
        string paymentReference,
        DateTime paymentDate,
        string? paymentMethod = null
    )
    {
        if (Status != InvoiceStatus.Approved && Status != InvoiceStatus.PartiallyPaid)
            throw new InvalidOperationException("Only approved invoices can be paid");

        PaidAmount += paidAmount;
        PaymentReference = paymentReference;
        PaymentDate = paymentDate;
        PaidDate = paymentDate;
        PaymentMethod = paymentMethod;

        if (PaidAmount >= TotalAmount)
        {
            Status = InvoiceStatus.Paid;
        }
        else if (PaidAmount > 0)
        {
            Status = InvoiceStatus.PartiallyPaid;
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsOverdue()
    {
        if (Status == InvoiceStatus.Paid || Status == InvoiceStatus.Cancelled)
            return;

        if (DateTime.UtcNow > DueDate)
        {
            Status = InvoiceStatus.Overdue;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void Cancel(string reason)
    {
        if (Status == InvoiceStatus.Paid)
            throw new InvalidOperationException("Cannot cancel paid invoices");

        Status = InvoiceStatus.Cancelled;
        RejectionReason = $"Cancelled: {reason}";
        UpdatedAt = DateTime.UtcNow;
    }

    // Calculated Properties

    public decimal GetOutstandingAmount() => TotalAmount - PaidAmount;

    public decimal GetPaymentPercentage() => TotalAmount > 0 ? PaidAmount / TotalAmount * 100 : 0;

    public int GetDaysOverdue() =>
        Status == InvoiceStatus.Overdue ? (DateTime.UtcNow - DueDate).Days : 0;

    public bool IsFullyPaid() => PaidAmount >= TotalAmount;

    public bool IsOverdue() => DateTime.UtcNow > DueDate && !IsFullyPaid();

    public int GetPeriodDays() => (PeriodEndDate - PeriodStartDate).Days;
}
