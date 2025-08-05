using Domain.Common;
using Core.Enums.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Entities.Contracts.Core;

public class ContractChangeOrder : BaseAuditableEntity
{
    public string ChangeOrderNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public Guid ContractId { get; set; }
    public virtual Contract Contract { get; set; } = null!;

    public ChangeOrderType Type { get; set; }
    public ChangeOrderStatus Status { get; set; }
    public ChangeOrderPriority Priority { get; set; }

    // Change Details
    public string Justification { get; set; } = string.Empty;
    public string ScopeImpact { get; set; } = string.Empty;
    public string ScheduleImpact { get; set; } = string.Empty;

    // Financial Impact
    public decimal Estimate{ get; set; }
    public decimal Approve{ get; set; }
    public decimal Actua{ get; set; }
    public string Currency { get; set; } = "USD";

    // Schedule Impact
    public int ScheduleImpactDays { get; set; }
    public DateTime? RevisedEndDate { get; set; }

    // Approval Process
    public DateTime SubmissionDate { get; set; }
    public string SubmittedBy { get; set; } = string.Empty;
    public DateTime? ReviewDate { get; set; }
    public string ReviewedBy { get; set; } = string.Empty;
    public string ReviewComments { get; set; } = string.Empty;
    public DateTime? ApprovalDate { get; set; }
    public string ApprovedBy { get; set; } = string.Empty;
    public string ApprovalComments { get; set; } = string.Empty;
    public DateTime? RejectionDate { get; set; }
    public string RejectedBy { get; set; } = string.Empty;
    public string RejectionReason { get; set; } = string.Empty;

    // Implementation
    public DateTime? ImplementationStartDate { get; set; }
    public DateTime? ImplementationEndDate { get; set; }
    public decimal PercentageComplete { get; set; }

    // Documents
    public bool HasSupportingDocuments { get; set; }
    public bool Hareakdown { get; set; }
    public bool HasScheduleAnalysis { get; set; }

    // Risk Assessment
    public ChangeOrderRisk RiskAssessment { get; set; }
    public string RiskMitigationPlan { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
    public string Notes { get; set; } = string.Empty;
    public string? Metadata { get; set; }

    // Navigation Properties
    public virtual ICollection<ChangeOrderDocument> Documents { get; set; } =
        new List<ChangeOrderDocument>();
    public virtual ICollection<ChangeOrderRelation> RelatedChangeOrders { get; set; } =
        new List<ChangeOrderRelation>();
    public virtual ICollection<ChangeOrderMilestone> AffectedMilestones { get; set; } =
        new List<ChangeOrderMilestone>();
    public virtual ICollection<ClaimChangeOrder> RelatedClaims { get; set; } =
        new List<ClaimChangeOrder>();

    // Calculated Properties
    public int AttachmentCount => Documents?.Count(d => d.IsActive) ?? 0;

    // Methods
    public void ApproveOrder(string approvedBy, decimal approve, string comments = "")
    {
        Status = ChangeOrderStatus.Approved;
        ApprovalDate = DateTime.UtcNow;
        ApprovedBy = approvedBy;
        Approve = approve;
        ApprovalComments = comments;
    }

    public void Reject(string rejectedBy, string reason)
    {
        Status = ChangeOrderStatus.Rejected;
        RejectionDate = DateTime.UtcNow;
        RejectedBy = rejectedBy;
        RejectionReason = reason;
    }

    public void StartImplementation()
    {
        Status = ChangeOrderStatus.InProgress;
        ImplementationStartDate = DateTime.UtcNow;
    }

    public void CompleteImplementation()
    {
        Status = ChangeOrderStatus.Implemented;
        ImplementationEndDate = DateTime.UtcNow;
        PercentageComplete = 100;
    }
}
