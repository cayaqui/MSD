using Core.DTOs.Common;
using Core.DTOs.Contracts.Contracts;
using Core.Enums.Contracts;

namespace Core.DTOs.Contracts.ContractMilestones;

public class ContractMilestoneDto : BaseDto
{
    public string MilestoneCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    public Guid ContractId { get; set; }
    public ContractDto? Contract { get; set; }
    
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
    
    // Dependencies
    public List<Guid> PredecessorIds { get; set; } = new();
    public List<Guid> SuccessorIds { get; set; } = new();
    
    // Documents
    public int AttachmentCount { get; set; }
    public bool HasSupportingDocuments { get; set; }
    
    // Variance Analysis
    public int ScheduleVarianceDays { get; set; }
    public decimal CostVariance { get; set; }
    public string VarianceExplanation { get; set; } = string.Empty;
    
    public bool IsActive { get; set; } = true;
    public bool IsCritical { get; set; }
    public string Notes { get; set; } = string.Empty;
    public Dictionary<string, object> Metadata { get; set; } = new();
}
