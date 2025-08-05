using Domain.Common;
using Core.Enums.Contracts;

namespace Domain.Entities.Contracts.Core;

public class Claim : BaseAuditableEntity
{
    public string ClaimNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    public Guid ContractId { get; set; }
    public virtual Contract Contract { get; set; } = null!;
    
    public ClaimType Type { get; set; }
    public ClaimStatus Status { get; set; }
    public ClaimPriority Priority { get; set; }
    
    // Claim Details
    public string ClaimBasis { get; set; } = string.Empty;
    public string ContractualReference { get; set; } = string.Empty;
    public string FactualBasis { get; set; } = string.Empty;
    public string LegalBasis { get; set; } = string.Empty;
    
    // Dates
    public DateTime EventDate { get; set; }
    public DateTime NotificationDate { get; set; }
    public DateTime SubmissionDate { get; set; }
    public DateTime? ResponseDueDate { get; set; }
    public DateTime? ActualResponseDate { get; set; }
    
    // Financial
    public decimal ClaimedAmount { get; set; }
    public decimal AssessedAmount { get; set; }
    public decimal ApprovedAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public string Currency { get; set; } = "USD";
    
    // Time Impact
    public int ClaimedTimeExtension { get; set; }
    public int AssessedTimeExtension { get; set; }
    public int ApprovedTimeExtension { get; set; }
    
    // Parties
    public ClaimDirection Direction { get; set; }
    public string ClaimantName { get; set; } = string.Empty;
    public string RespondentName { get; set; } = string.Empty;
    
    // Assessment
    public bool IsTimeBarred { get; set; }
    public bool HasMerit { get; set; }
    public decimal LiabilityPercentage { get; set; }
    public string AssessmentComments { get; set; } = string.Empty;
    
    // Resolution
    public ClaimResolution Resolution { get; set; }
    public DateTime? ResolutionDate { get; set; }
    public string ResolutionDetails { get; set; } = string.Empty;
    public string SettlementTerms { get; set; } = string.Empty;
    
    // Documents
    public bool HasNoticeOfClaim { get; set; }
    public bool HasDetailedParticulars { get; set; }
    public bool HasSupportingDocuments { get; set; }
    public bool HasExpertReport { get; set; }
    
    public bool IsActive { get; set; } = true;
    public string Notes { get; set; } = string.Empty;
    public string? Metadata { get; set; }
    
    // Navigation Properties
    public virtual ICollection<ClaimDocument> Documents { get; set; } = new List<ClaimDocument>();
    public virtual ICollection<ClaimRelation> RelatedClaims { get; set; } = new List<ClaimRelation>();
    public virtual ICollection<ClaimChangeOrder> RelatedChangeOrders { get; set; } = new List<ClaimChangeOrder>();
    
    // Calculated Properties
    public int AttachmentCount => Documents?.Count(d => d.IsActive) ?? 0;
    
    public bool IsOverdue => ResponseDueDate.HasValue && !ActualResponseDate.HasValue && DateTime.Now > ResponseDueDate.Value;
    
    // Methods
    public void Submit()
    {
        Status = ClaimStatus.Submitted;
        SubmissionDate = DateTime.UtcNow;
    }
    
    public void Assess(decimal assessedAmount, int assessedTimeExtension, bool hasMerit, decimal liabilityPercentage, string comments)
    {
        Status = ClaimStatus.UnderAssessment;
        AssessedAmount = assessedAmount;
        AssessedTimeExtension = assessedTimeExtension;
        HasMerit = hasMerit;
        LiabilityPercentage = liabilityPercentage;
        AssessmentComments = comments;
    }
    
    public void Resolve(ClaimResolution resolution, decimal approvedAmount, int approvedTimeExtension, string details, string settlementTerms = "")
    {
        Resolution = resolution;
        ResolutionDate = DateTime.UtcNow;
        ApprovedAmount = approvedAmount;
        ApprovedTimeExtension = approvedTimeExtension;
        ResolutionDetails = details;
        SettlementTerms = settlementTerms;
        
        Status = resolution switch
        {
            ClaimResolution.FullyAccepted => ClaimStatus.Approved,
            ClaimResolution.PartiallyAccepted => ClaimStatus.PartiallyApproved,
            ClaimResolution.Rejected => ClaimStatus.Rejected,
            ClaimResolution.Negotiated => ClaimStatus.Settled,
            ClaimResolution.Withdrawn => ClaimStatus.Withdrawn,
            ClaimResolution.TimeBarred => ClaimStatus.Rejected,
            _ => ClaimStatus.Closed
        };
    }
    
    public void RecordPayment(decimal amount)
    {
        PaidAmount += amount;
        if (PaidAmount >= ApprovedAmount)
        {
            Status = ClaimStatus.Closed;
        }
    }
    
    public void CheckTimeBar(int notificationPeriodDays)
    {
        var notificationDeadline = EventDate.AddDays(notificationPeriodDays);
        IsTimeBarred = NotificationDate > notificationDeadline;
    }
}