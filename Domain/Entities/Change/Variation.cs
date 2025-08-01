using Core.Enums.Cost;
using Domain.Common;
using Domain.Entities.Projects;
using Domain.Entities.Security;
using Domain.Entities.Setup;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Domain.Entities.Change;

/// <summary>
/// Variation entity for tracking approved changes to contract scope, cost, or schedule
/// Represents formal contract variations/amendments after Change Order approval
/// </summary>
public class Variation : BaseEntity, IAuditable, ISoftDelete, ICodeEntity
{
    // Basic Information
    public string Code { get; set; } = string.Empty; // V-XXX-YYYY format
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    // Classification
    public Guid ProjectId { get; private set; }
    public Guid? ContractorId { get; private set; }
    public Guid? ChangeOrderId { get; private set; }
    public Guid? TrendId { get; private set; }
    public VariationType Type { get; private set; }
    public VariationStatus Status { get; private set; }
    public VariationCategory Category { get; private set; }

    // Contract Information
    public string ContractReference { get; private set; } = string.Empty;
    public string? ClientReferenceNumber { get; private set; }
    public string? ContractorReferenceNumber { get; private set; }
    public bool IsContractual { get; private set; }

    // Dates
    public DateTime IssuedDate { get; private set; }
    public DateTime? SubmittedDate { get; private set; }
    public DateTime? ReviewedDate { get; private set; }
    public DateTime? ApprovedDate { get; private set; }
    public DateTime? EffectiveDate { get; private set; }
    public DateTime? CompletionDate { get; private set; }

    // Financial Impact
    public decimal OriginalValue { get; private set; }
    public decimal? NegotiatedValue { get; private set; }
    public decimal ApprovedValue { get; private set; }
    public string Currency { get; private set; } = "USD";

    // Cost Breakdown
    public decimal? LaborCost { get; private set; }
    public decimal? MaterialCost { get; private set; }
    public decimal? EquipmentCost { get; private set; }
    public decimal? SubcontractorCost { get; private set; }
    public decimal? IndirectCost { get; private set; }
    public decimal? OverheadPercentage { get; private set; }
    public decimal? ProfitPercentage { get; private set; }

    // Schedule Impact
    public int? TimeExtensionDays { get; private set; }
    public DateTime? RevisedCompletionDate { get; private set; }
    public bool IsCriticalPathImpacted { get; private set; }

    // Quantity Changes
    public string? QuantityChanges { get; private set; } // JSON structure
    public string? RateAdjustments { get; private set; } // JSON structure

    // Approval Workflow
    public Guid? RequestedByUserId { get; private set; }
    public Guid? ReviewedByUserId { get; private set; }
    public Guid? ApprovedByUserId { get; private set; }
    public string? ApprovalComments { get; private set; }
    public int ApprovalLevel { get; private set; } // 1-5 based on value

    // Client Approval
    public bool RequiresClientApproval { get; private set; }
    public bool? ClientApproved { get; private set; }
    public DateTime? ClientApprovalDate { get; private set; }
    public string? ClientApprovalReference { get; private set; }
    public string? ClientComments { get; private set; }

    // Documentation
    public string? Justification { get; private set; }
    public string? ScopeOfWork { get; private set; }
    public string? ImpactAnalysis { get; private set; }
    public string? ContractualBasis { get; private set; }
    public string? DrawingReferences { get; private set; }
    public string? SpecificationReferences { get; private set; }

    // Payment Terms
    public PaymentMethod? PaymentMethod { get; private set; }
    public string? PaymentTerms { get; private set; }
    public decimal? AdvancePaymentPercentage { get; private set; }
    public decimal? RetentionPercentage { get; private set; }

    // Status Tracking
    public bool IsDisputed { get; private set; }
    public string? DisputeReason { get; private set; }
    public DateTime? DisputeDate { get; private set; }
    public bool IsResolved { get; private set; }
    public DateTime? ResolutionDate { get; private set; }

    // Implementation
    public bool IsImplemented { get; private set; }
    public decimal? ImplementedValue { get; private set; }
    public decimal? CertifiedValue { get; private set; }
    public decimal? PaidValue { get; private set; }

    // Calculated Fields
    public decimal PendingValue => ApprovedValue - (CertifiedValue ?? 0);
    public decimal OutstandingValue => (CertifiedValue ?? 0) - (PaidValue ?? 0);
    public bool IsFullyPaid => PaidValue >= ApprovedValue;

    // Soft Delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // Navigation Properties
    public Project Project { get; private set; } = null!;
    public Contractor? Contractor { get; private set; }
    public ChangeOrder? ChangeOrder { get; private set; }
    public Trend? Trend { get; private set; }
    public User? RequestedByUser { get; private set; }
    public User? ReviewedByUser { get; private set; }
    public User? ApprovedByUser { get; private set; }
    public ICollection<VariationItem> Items { get; private set; } = new List<VariationItem>();
    public ICollection<VariationAttachment> Attachments { get; private set; } = new List<VariationAttachment>();
    public ICollection<Invoice> Invoices { get; private set; } = new List<Invoice>();

    // Constructor for EF Core
    private Variation() { }

    public Variation(
        string code,
        string title,
        string description,
        Guid projectId,
        string contractReference,
        VariationType type,
        VariationCategory category,
        decimal originalValue,
        string currency)
    {
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        ProjectId = projectId;
        ContractReference = contractReference ?? throw new ArgumentNullException(nameof(contractReference));
        Type = type;
        Category = category;
        OriginalValue = originalValue;
        Currency = currency ?? throw new ArgumentNullException(nameof(currency));

        IssuedDate = DateTime.UtcNow;
        Status = VariationStatus.Draft;
        IsContractual = true;
        RequiresClientApproval = originalValue > 0; // Positive variations need approval
        ApprovalLevel = DetermineApprovalLevel(originalValue);

        CreatedAt = DateTime.UtcNow;
    }

    // Domain Methods
    public void LinkToChangeOrder(Guid changeOrderId)
    {
        if (ChangeOrderId.HasValue)
            throw new InvalidOperationException("Variation is already linked to a change order");

        ChangeOrderId = changeOrderId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void LinkToTrend(Guid trendId)
    {
        if (TrendId.HasValue)
            throw new InvalidOperationException("Variation is already linked to a trend");

        TrendId = trendId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetCostBreakdown(
        decimal? labor,
        decimal? material,
        decimal? equipment,
        decimal? subcontractor,
        decimal? indirect,
        decimal? overheadPerc,
        decimal? profitPerc)
    {
        LaborCost = labor;
        MaterialCost = material;
        EquipmentCost = equipment;
        SubcontractorCost = subcontractor;
        IndirectCost = indirect;
        OverheadPercentage = overheadPerc;
        ProfitPercentage = profitPerc;

        ValidateCostBreakdown();
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetScheduleImpact(int? extensionDays, DateTime? revisedCompletion, bool criticalPathImpacted)
    {
        TimeExtensionDays = extensionDays;
        RevisedCompletionDate = revisedCompletion;
        IsCriticalPathImpacted = criticalPathImpacted;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Submit(Guid submittedBy)
    {
        if (Status != VariationStatus.Draft)
            throw new InvalidOperationException("Only draft variations can be submitted");

        Status = VariationStatus.Submitted;
        SubmittedDate = DateTime.UtcNow;
        RequestedByUserId = submittedBy;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Review(Guid reviewedBy, decimal? negotiatedValue, string? comments)
    {
        if (Status != VariationStatus.Submitted)
            throw new InvalidOperationException("Only submitted variations can be reviewed");

        Status = VariationStatus.UnderReview;
        ReviewedDate = DateTime.UtcNow;
        ReviewedByUserId = reviewedBy;
        NegotiatedValue = negotiatedValue;

        if (!string.IsNullOrWhiteSpace(comments))
        {
            ApprovalComments = comments;
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public void Approve(Guid approvedBy, decimal approvedValue, string? comments)
    {
        if (Status != VariationStatus.UnderReview && Status != VariationStatus.Submitted)
            throw new InvalidOperationException("Variation must be under review to be approved");

        Status = VariationStatus.Approved;
        ApprovedDate = DateTime.UtcNow;
        ApprovedByUserId = approvedBy;
        ApprovedValue = approvedValue;
        ApprovalComments = comments;
        EffectiveDate = DateTime.UtcNow;

        UpdatedAt = DateTime.UtcNow;
    }

    public void Reject(Guid rejectedBy, string reason)
    {
        if (Status == VariationStatus.Approved || Status == VariationStatus.Implemented)
            throw new InvalidOperationException("Cannot reject an approved or implemented variation");

        Status = VariationStatus.Rejected;
        ReviewedByUserId = rejectedBy;
        ReviewedDate = DateTime.UtcNow;
        ApprovalComments = reason ?? throw new ArgumentNullException(nameof(reason));

        UpdatedAt = DateTime.UtcNow;
    }

    public void RecordClientApproval(bool approved, string? reference, string? comments)
    {
        if (!RequiresClientApproval)
            throw new InvalidOperationException("This variation does not require client approval");

        ClientApproved = approved;
        ClientApprovalDate = DateTime.UtcNow;
        ClientApprovalReference = reference;
        ClientComments = comments;

        if (!approved)
        {
            Status = VariationStatus.ClientRejected;
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public void RaiseDispute(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentNullException(nameof(reason));

        IsDisputed = true;
        DisputeReason = reason;
        DisputeDate = DateTime.UtcNow;
        Status = VariationStatus.Disputed;

        UpdatedAt = DateTime.UtcNow;
    }

    public void ResolveDispute(string resolution)
    {
        if (!IsDisputed)
            throw new InvalidOperationException("Variation is not in dispute");

        IsResolved = true;
        ResolutionDate = DateTime.UtcNow;
        ApprovalComments = $"Dispute Resolution: {resolution}";
        Status = VariationStatus.Approved;

        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsImplemented()
    {
        if (Status != VariationStatus.Approved)
            throw new InvalidOperationException("Only approved variations can be implemented");

        IsImplemented = true;
        Status = VariationStatus.Implemented;
        CompletionDate = DateTime.UtcNow;
        ImplementedValue = ApprovedValue;

        UpdatedAt = DateTime.UtcNow;
    }

    public void RecordCertification(decimal certifiedAmount)
    {
        if (!IsImplemented)
            throw new InvalidOperationException("Variation must be implemented before certification");

        if (certifiedAmount > ApprovedValue)
            throw new ArgumentException("Certified amount cannot exceed approved value");

        CertifiedValue = (CertifiedValue ?? 0) + certifiedAmount;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RecordPayment(decimal paidAmount)
    {
        if (paidAmount > (CertifiedValue ?? 0))
            throw new ArgumentException("Paid amount cannot exceed certified value");

        PaidValue = (PaidValue ?? 0) + paidAmount;
        UpdatedAt = DateTime.UtcNow;
    }

    private int DetermineApprovalLevel(decimal value)
    {
        return value switch
        {
            <= 10000 => 1,      // Project Manager
            <= 50000 => 2,      // Operations Manager
            <= 100000 => 3,     // Director
            <= 500000 => 4,     // VP
            _ => 5              // CEO/Board
        };
    }

    private void ValidateCostBreakdown()
    {
        var directCosts = (LaborCost ?? 0) + (MaterialCost ?? 0) +
                         (EquipmentCost ?? 0) + (SubcontractorCost ?? 0);

        var overhead = directCosts * ((OverheadPercentage ?? 0) / 100);
        var profit = (directCosts + overhead) * ((ProfitPercentage ?? 0) / 100);

        var calculatedTotal = directCosts + (IndirectCost ?? 0) + overhead + profit;

        // Allow 1% tolerance for rounding
        if (Math.Abs(calculatedTotal - OriginalValue) > (OriginalValue * 0.01m))
        {
            throw new InvalidOperationException("Cost breakdown does not match original value");
        }
    }
}
