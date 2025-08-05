using Domain.Common;
using Domain.Entities.Organization.Core;
using Core.Enums.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Entities.Contracts.Core;

public class Contract : BaseAuditableEntity
{
    public string ContractNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // Contract Type and Status
    public ContractType Type { get; set; }
    public ContractStatus Status { get; set; }
    public ContractCategory Category { get; set; }
    public string SubCategory { get; set; } = string.Empty;

    // Project Information
    public Guid ProjectId { get; set; }
    public virtual Project Project { get; set; } = null!;

    // Contractor Information
    public Guid ContractorId { get; set; }
    public virtual Company Contractor { get; set; } = null!;
    public string ContractorReference { get; set; } = string.Empty;

    // Contract Value
    public decimal OriginalValue { get; set; }
    public decimal CurrentValue { get; set; }
    public string Currency { get; set; } = "USD";
    public decimal ExchangeRate { get; set; } = 1.0m;

    // Dates
    public DateTime ContractDate { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime OriginalEndDate { get; set; }
    public DateTime CurrentEndDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; }

    // Performance
    public decimal PercentageComplete { get; set; }
    public decimal AmountInvoiced { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal RetentionAmount { get; set; }
    public decimal RetentionPercentage { get; set; }

    // Payment Terms
    public PaymentTerms PaymentTerms { get; set; }
    public int PaymentDays { get; set; }
    public string PaymentSchedule { get; set; } = string.Empty;

    // Bonds and Insurance
    public bool RequiresPerformanceBond { get; set; }
    public decimal? PerformanceBondAmount { get; set; }
    public DateTime? PerformanceBondExpiry { get; set; }
    public bool RequiresPaymentBond { get; set; }
    public decimal? PaymentBondAmount { get; set; }
    public DateTime? PaymentBondExpiry { get; set; }
    public string InsuranceRequirements { get; set; } = string.Empty;

    // Risk and Issues
    public ContractRiskLevel RiskLevel { get; set; }

    // Documents
    public string ContractDocumentUrl { get; set; } = string.Empty;
    public DateTime? LastDocumentUpdate { get; set; }

    // Approval
    public bool IsApproved { get; set; }
    public string ApprovedBy { get; set; } = string.Empty;
    public DateTime? ApprovalDate { get; set; }
    public string ApprovalComments { get; set; } = string.Empty;

    // Additional Information
    public string Scope { get; set; } = string.Empty;
    public string Exclusions { get; set; } = string.Empty;
    public string SpecialConditions { get; set; } = string.Empty;
    public string PenaltyClauses { get; set; } = string.Empty;
    public string TerminationClauses { get; set; } = string.Empty;

    // Audit
    public bool IsActive { get; set; } = true;
    public string Notes { get; set; } = string.Empty;
    public string? Metadata { get; set; }

    // Navigation Properties
    public virtual ICollection<ContractChangeOrder> ChangeOrders { get; set; } = new List<ContractChangeOrder>();
    public virtual ICollection<ContractMilestone> Milestones { get; set; } =
        new List<ContractMilestone>();
    public virtual ICollection<Valuation> Valuations { get; set; } = new List<Valuation>();
    public virtual ICollection<Claim> Claims { get; set; } = new List<Claim>();
    public virtual ICollection<ContractDocument> Documents { get; set; } =
        new List<ContractDocument>();

    // Calculated Properties
    public decimal ChangeOrderValue =>
        ChangeOrders
            ?.Where(co => co.Status == ChangeOrderStatus.Approved)
            .Sum(co => co.Approve) ?? 0;
    public int ChangeOrderCount => ChangeOrders?.Count(co => co.IsActive) ?? 0;
    public decimal PendingChangeOrderValue =>
        ChangeOrders
            ?.Where(co =>
                co.Status == ChangeOrderStatus.Submitted
                || co.Status == ChangeOrderStatus.UnderReview
            )
            .Sum(co => co.Estimate) ?? 0;

    public int TotalMilestones => Milestones?.Count(m => m.IsActive) ?? 0;
    public int CompletedMilestones =>
        Milestones?.Count(m => m.IsActive && m.Status == MilestoneStatus.Completed) ?? 0;
    public DateTime? NextMilestoneDate =>
        Milestones
            ?.Where(m => m.IsActive && m.Status != MilestoneStatus.Completed)
            .OrderBy(m => m.PlannedDate)
            .FirstOrDefault()
            ?.PlannedDate;
    public string NextMilestoneName =>
        Milestones
            ?.Where(m => m.IsActive && m.Status != MilestoneStatus.Completed)
            .OrderBy(m => m.PlannedDate)
            .FirstOrDefault()
            ?.Name ?? string.Empty;

    public int OpenIssuesCount =>
        Claims?.Count(c =>
            c.IsActive && c.Status != ClaimStatus.Closed && c.Status != ClaimStatus.Settled
        ) ?? 0;
    public int OpenClaimsCount =>
        Claims?.Count(c =>
            c.IsActive
            && c.Status != ClaimStatus.Closed
            && c.Status != ClaimStatus.Settled
            && c.Status != ClaimStatus.Withdrawn
        ) ?? 0;
    public decimal TotalClaimsValue =>
        Claims?.Where(c => c.IsActive).Sum(c => c.ClaimedAmount) ?? 0;

    public int AttachmentCount => Documents?.Count(d => d.IsActive) ?? 0;

    // Methods
    public void UpdateCurrentValue()
    {
        CurrentValue = OriginalValue + ChangeOrderValue;
    }

    public void UpdateEndDate(int extensionDays)
    {
        CurrentEndDate = CurrentEndDate.AddDays(extensionDays);
    }

    public bool IsDelayed()
    {
        return Status == ContractStatus.Active
            && DateTime.Now > CurrentEndDate
            && PercentageComplete < 100;
    }

    public bool HasExpiredBonds()
    {
        var now = DateTime.Now;
        return 
                RequiresPerformanceBond
                && PerformanceBondExpiry.HasValue
                && PerformanceBondExpiry.Value < now
            
            || RequiresPaymentBond && PaymentBondExpiry.HasValue && PaymentBondExpiry.Value < now;
    }
}
