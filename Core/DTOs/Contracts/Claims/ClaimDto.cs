using Core.DTOs.Common;
using Core.DTOs.Contracts.Contracts;
using Core.Enums.Contracts;

namespace Core.DTOs.Contracts.Claims;

public class ClaimDto : BaseDto
{
    public string ClaimNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    public Guid ContractId { get; set; }
    public ContractDto? Contract { get; set; }
    
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
    public ClaimDirection Direction { get; set; } // Client to Contractor or Contractor to Client
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
    public int AttachmentCount { get; set; }
    public bool HasNoticeOfClaim { get; set; }
    public bool HasDetailedParticulars { get; set; }
    public bool HasSupportingDocuments { get; set; }
    public bool HasExpertReport { get; set; }
    
    // Related Items
    public List<Guid> RelatedChangeOrderIds { get; set; } = new();
    public List<Guid> RelatedClaimIds { get; set; } = new();
    
    public bool IsActive { get; set; } = true;
    public string Notes { get; set; } = string.Empty;
    public Dictionary<string, object> Metadata { get; set; } = new();
}
