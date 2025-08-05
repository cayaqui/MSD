using Domain.Common;
using Core.Enums.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Entities.Contracts.Core;

public class ContractMilestone : BaseAuditableEntity
{
    public string MilestoneCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public Guid ContractId { get; set; }
    public virtual Contract Contract { get; set; } = null!;

    public MilestoneType Type { get; set; }
    public MilestoneStatus Status { get; set; }
    public int SequenceNumber { get; set; }

    // Dates
    public DateTime PlannedDate { get; set; }
    public DateTime? RevisedDate { get; set; }
    public DateTime? ActualDate { get; set; }
    public DateTime? ForecastDate { get; set; }

    // Value
    public decimal Amount { get; set; }
    public decimal PercentageOfContract { get; set; }
    public string Currency { get; set; } = "USD";

    // Completion Criteria
    public string CompletionCriteria { get; set; } = string.Empty;
    public string Deliverables { get; set; } = string.Empty;
    public bool RequiresClientApproval { get; set; }
    public bool IsPaymentMilestone { get; set; }

    // Progress
    public decimal PercentageComplete { get; set; }
    public string ProgressComments { get; set; } = string.Empty;
    public DateTime? LastProgressUpdate { get; set; }

    // Approval
    public bool IsApproved { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public string ApprovedBy { get; set; } = string.Empty;
    public string ApprovalComments { get; set; } = string.Empty;

    // Payment
    public bool IsInvoiced { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime? InvoiceDate { get; set; }
    public decimal InvoiceAmount { get; set; }
    public bool IsPaid { get; set; }
    public DateTime? PaymentDate { get; set; }
    public decimal PaymentAmount { get; set; }

    // Variance Analysis
    public string VarianceExplanation { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
    public bool IsCritical { get; set; }
    public string Notes { get; set; } = string.Empty;
    public string? Metadata { get; set; }

    // Navigation Properties
    public virtual ICollection<MilestoneDocument> Documents { get; set; } = new List<MilestoneDocument>();
    public virtual ICollection<MilestoneDependency> Predecessors { get; set; } = new List<MilestoneDependency>();
    public virtual ICollection<MilestoneDependency> Successors { get; set; } = new List<MilestoneDependency>();
    public virtual ICollection<ChangeOrderMilestone> ChangeOrders { get; set; } = new List<ChangeOrderMilestone>();

    // Calculated Properties
    public int AttachmentCount => Documents?.Count(d => d.IsActive) ?? 0;
    public bool HasSupportingDocuments => Documents?.Any(d => d.IsActive) ?? false;

    public int ScheduleVarianceDays
    {
        get
        {
            if (ActualDate.HasValue)
                return (ActualDate.Value - PlannedDate).Days;
            if (ForecastDate.HasValue)
                return (ForecastDate.Value - PlannedDate).Days;
            return (DateTime.Now - PlannedDate).Days;
        }
    }

    public decimal CostVariance
    {
        get
        {
            if (IsInvoiced)
                return InvoiceAmount - Amount;
            return 0;
        }
    }

    // Methods
    public void UpdateProgress(decimal percentageComplete, string comments)
    {
        PercentageComplete = percentageComplete;
        ProgressComments = comments;
        LastProgressUpdate = DateTime.UtcNow;

        if (percentageComplete >= 100)
        {
            Status = MilestoneStatus.Submitted;
        }
        else if (percentageComplete > 0)
        {
            Status = MilestoneStatus.InProgress;
        }
    }

    public void Approve(string approvedBy, string comments = "")
    {
        IsApproved = true;
        Status = MilestoneStatus.Approved;
        ApprovalDate = DateTime.UtcNow;
        ApprovedBy = approvedBy;
        ApprovalComments = comments;

        if (PercentageComplete >= 100)
        {
            Status = MilestoneStatus.Completed;
            ActualDate = DateTime.UtcNow;
        }
    }

    public void Reject(string rejectedBy, string reason)
    {
        IsApproved = false;
        Status = MilestoneStatus.Rejected;
        ApprovalDate = DateTime.UtcNow;
        ApprovedBy = rejectedBy;
        ApprovalComments = reason;
    }

    public void RecordInvoice(string invoiceNumber, decimal amount)
    {
        IsInvoiced = true;
        InvoiceNumber = invoiceNumber;
        InvoiceDate = DateTime.UtcNow;
        InvoiceAmount = amount;
    }

    public void RecordPayment(decimal amount)
    {
        IsPaid = true;
        PaymentDate = DateTime.UtcNow;
        PaymentAmount = amount;
    }

    public bool IsOverdue()
    {
        var referenceDate = RevisedDate ?? PlannedDate;
        return Status != MilestoneStatus.Completed && DateTime.Now > referenceDate;
    }
}