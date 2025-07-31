

using Domain.Entities.Projects;

namespace Domain.Entities.Cost;

/// <summary>
/// Represents a financial commitment (PO, Contract, etc.) according to PMBOK cost management
/// </summary>
public class Commitment : BaseEntity, IAuditable, ISoftDelete
{
    // Foreign Keys
    public Guid ProjectId { get; private set; }
    public Project Project { get; private set; } = null!;

    public Guid? BudgetItemId { get; private set; }
    public BudgetItem? BudgetItem { get; private set; }

    public Guid? ContractorId { get; private set; }
    public Contractor? Contractor { get; private set; }

    // Commitment Information
    public string CommitmentNumber { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public CommitmentType Type { get; private set; }
    public CommitmentStatus Status { get; private set; }

    // Financial Information
    public decimal OriginalAmount { get; private set; }
    public decimal RevisedAmount { get; private set; }
    public decimal CommittedAmount { get; private set; }
    public decimal InvoicedAmount { get; private set; }
    public decimal PaidAmount { get; private set; }
    public decimal RetentionAmount { get; private set; }
    public string Currency { get; private set; } = string.Empty;

    // Contract Terms
    public DateTime ContractDate { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public int? PaymentTermsDays { get; private set; }
    public decimal? RetentionPercentage { get; private set; }
    public decimal? AdvancePaymentAmount { get; private set; }

    // References
    public string? PurchaseOrderNumber { get; private set; }
    public string? ContractNumber { get; private set; }
    public string? VendorReference { get; private set; }
    public string? AccountingReference { get; private set; }

    // Approval Information
    public DateTime? ApprovalDate { get; private set; }
    public string? ApprovedBy { get; private set; }
    public string? ApprovalNotes { get; private set; }

    // Performance
    public decimal? PerformancePercentage { get; private set; }
    public DateTime? LastInvoiceDate { get; private set; }
    public DateTime? ExpectedCompletionDate { get; private set; }

    // Additional Information
    public string? Terms { get; private set; }
    public string? ScopeOfWork { get; private set; }
    public string? Deliverables { get; private set; }
    public bool IsFixedPrice { get; private set; }
    public bool IsTimeAndMaterial { get; private set; }

    // ISoftDelete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // Navigation Properties
    public ICollection<Invoice> Invoices { get; private set; } = new List<Invoice>();
    public ICollection<CommitmentWorkPackage> WorkPackages { get; private set; } = new List<CommitmentWorkPackage>();
    public ICollection<CommitmentRevision> Revisions { get; private set; } =
        new List<CommitmentRevision>();

    private Commitment() { } // EF Core

    public Commitment(
        Guid projectId,
        string commitmentNumber,
        string title,
        CommitmentType type,
        decimal originalAmount,
        string currency,
        DateTime contractDate,
        DateTime startDate,
        DateTime endDate
    )
    {
        ProjectId = projectId;
        CommitmentNumber =
            commitmentNumber ?? throw new ArgumentNullException(nameof(commitmentNumber));
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Type = type;
        OriginalAmount = originalAmount;
        RevisedAmount = originalAmount;
        CommittedAmount = originalAmount;
        Currency = currency ?? throw new ArgumentNullException(nameof(currency));
        ContractDate = contractDate;
        StartDate = startDate;
        EndDate = endDate;
        Status = CommitmentStatus.Draft;
        InvoicedAmount = 0;
        PaidAmount = 0;
        RetentionAmount = 0;
        CreatedAt = DateTime.UtcNow;
    }

    // Methods
    public void AssignToContractor(Guid contractorId)
    {
        ContractorId = contractorId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AssignToBudget(Guid budgetItemId)
    {
        BudgetItemId = budgetItemId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddWorkPackage(Guid wbsElementId, decimal allocatedAmount)
    {
        var commitmentWP = new CommitmentWorkPackage(Id, wbsElementId, allocatedAmount);
        WorkPackages.Add(commitmentWP);
        UpdatedAt = DateTime.UtcNow;
    }
    public void UpdateDetails(
        string title,
        string? description,
        string? scopeOfWork,
        string? deliverables
    )
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description;
        ScopeOfWork = scopeOfWork;
        Deliverables = deliverables;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetContractReferences(
        string? poNumber,
        string? contractNumber,
        string? vendorRef,
        string? accountingRef
    )
    {
        PurchaseOrderNumber = poNumber;
        ContractNumber = contractNumber;
        VendorReference = vendorRef;
        AccountingReference = accountingRef;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetPaymentTerms(
        int? paymentDays,
        decimal? retentionPercentage,
        decimal? advancePayment
    )
    {
        PaymentTermsDays = paymentDays;
        RetentionPercentage = retentionPercentage;
        AdvancePaymentAmount = advancePayment;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetContractType(bool isFixedPrice, bool isTimeAndMaterial)
    {
        IsFixedPrice = isFixedPrice;
        IsTimeAndMaterial = isTimeAndMaterial;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SubmitForApproval()
    {
        if (Status != CommitmentStatus.Draft)
            throw new InvalidOperationException(
                "Only draft commitments can be submitted for approval"
            );

        Status = CommitmentStatus.PendingApproval;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Approve(string approvedBy, string? notes = null)
    {
        if (Status != CommitmentStatus.PendingApproval)
            throw new InvalidOperationException("Only pending commitments can be approved");

        Status = CommitmentStatus.Approved;
        ApprovalDate = DateTime.UtcNow;
        ApprovedBy = approvedBy;
        ApprovalNotes = notes;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        if (Status != CommitmentStatus.Approved)
            throw new InvalidOperationException("Only approved commitments can be activated");

        Status = CommitmentStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Revise(decimal revisedAmount, string reason)
    {
        if (Status != CommitmentStatus.Active && Status != CommitmentStatus.PartiallyInvoiced)
            throw new InvalidOperationException("Only active commitments can be revised");

        var revision = new CommitmentRevision(Id, RevisedAmount, revisedAmount, reason);
        Revisions.Add(revision);

        RevisedAmount = revisedAmount;
        CommittedAmount = revisedAmount;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RecordInvoice(decimal invoiceAmount, decimal paidAmount, decimal retentionAmount)
    {
        InvoicedAmount += invoiceAmount;
        PaidAmount += paidAmount;
        RetentionAmount += retentionAmount;
        LastInvoiceDate = DateTime.UtcNow;

        UpdateStatus();
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePerformance(
        decimal performancePercentage,
        DateTime? expectedCompletion = null
    )
    {
        PerformancePercentage = performancePercentage;
        ExpectedCompletionDate = expectedCompletion;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Close()
    {
        if (Status == CommitmentStatus.Closed)
            throw new InvalidOperationException("Commitment is already closed");

        Status = CommitmentStatus.Closed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel(string reason)
    {
        if (Status == CommitmentStatus.Closed || Status == CommitmentStatus.FullyInvoiced)
            throw new InvalidOperationException(
                "Cannot cancel closed or fully invoiced commitments"
            );

        Status = CommitmentStatus.Cancelled;
        ApprovalNotes = $"Cancelled: {reason}";
        UpdatedAt = DateTime.UtcNow;
    }

    // Private Methods
    private void UpdateStatus()
    {
        if (InvoicedAmount >= CommittedAmount)
        {
            Status = CommitmentStatus.FullyInvoiced;
        }
        else if (InvoicedAmount > 0)
        {
            Status = CommitmentStatus.PartiallyInvoiced;
        }
    }

    // Calculated Properties
    public Money GetOriginalAmountMoney() => new(OriginalAmount, Currency);

    public Money GetRevisedAmountMoney() => new(RevisedAmount, Currency);

    public Money GetCommittedAmountMoney() => new(CommittedAmount, Currency);

    public Money GetInvoicedAmountMoney() => new(InvoicedAmount, Currency);

    public Money GetPaidAmountMoney() => new(PaidAmount, Currency);

    public Money GetRetentionAmountMoney() => new(RetentionAmount, Currency);

    public decimal GetRemainingAmount() => CommittedAmount - InvoicedAmount;

    public decimal GetUnpaidAmount() => InvoicedAmount - PaidAmount;

    public decimal GetInvoicedPercentage() =>
        CommittedAmount > 0 ? (InvoicedAmount / CommittedAmount) * 100 : 0;

    public decimal GetPaidPercentage() =>
        InvoicedAmount > 0 ? (PaidAmount / InvoicedAmount) * 100 : 0;

    public bool IsOverCommitted() => InvoicedAmount > CommittedAmount;

    public bool IsExpired() => DateTime.UtcNow > EndDate && Status == CommitmentStatus.Active;

    public DateRange GetContractPeriod() => new(StartDate, EndDate);
}
