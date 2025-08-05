using Core.DTOs.Common;
using Core.DTOs.Contracts.Contracts;
using Core.Enums.Change;
using Core.Enums.Cost;

namespace Core.DTOs.Contracts.ChangeOrders;

public class ChangeOrderDto : BaseDto
{
    public string ChangeOrderNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    public Guid ContractId { get; set; }
    public ContractDto? Contract { get; set; }
    
    public ChangeOrderType Type { get; set; }
    public ChangeOrderStatus Status { get; set; }
    public ChangeOrderPriority Priority { get; set; }
    
    // Change Details
    public string Justification { get; set; } = string.Empty;
    public string ScopeImpact { get; set; } = string.Empty;
    public string ScheduleImpact { get; set; } = string.Empty;
    
    // Financial Impact
    public decimal EstimatedCost { get; set; }
    public decimal ApprovedCost { get; set; }
    public decimal ActualCost { get; set; }
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
    public int AttachmentCount { get; set; }
    public bool HasSupportingDocuments { get; set; }
    public bool HasCostBreakdown { get; set; }
    public bool HasScheduleAnalysis { get; set; }
    
    // Related Items
    public List<Guid> RelatedChangeOrderIds { get; set; } = new();
    public List<Guid> AffectedMilestoneIds { get; set; } = new();
    public List<Guid> AffectedWorkPackageIds { get; set; } = new();
    
    // Risk Assessment
    public ChangeOrderRisk RiskAssessment { get; set; }
    public string RiskMitigationPlan { get; set; } = string.Empty;
    
    public bool IsActive { get; set; } = true;
    public string Notes { get; set; } = string.Empty;
    public Dictionary<string, object> Metadata { get; set; } = new();
}
