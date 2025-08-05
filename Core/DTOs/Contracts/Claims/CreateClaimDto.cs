using Core.Enums.Contracts;

namespace Core.DTOs.Contracts.Claims;

public class CreateClaimDto
{
    public string ClaimNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    public Guid ContractId { get; set; }
    public ClaimType Type { get; set; }
    public ClaimPriority Priority { get; set; }
    public ClaimDirection Direction { get; set; }
    
    public string ClaimBasis { get; set; } = string.Empty;
    public string ContractualReference { get; set; } = string.Empty;
    public string FactualBasis { get; set; } = string.Empty;
    public string LegalBasis { get; set; } = string.Empty;
    
    public DateTime EventDate { get; set; }
    public DateTime NotificationDate { get; set; }
    public DateTime? ResponseDueDate { get; set; }
    
    public decimal ClaimedAmount { get; set; }
    public string Currency { get; set; } = "USD";
    public int ClaimedTimeExtension { get; set; }
    
    public string ClaimantName { get; set; } = string.Empty;
    public string RespondentName { get; set; } = string.Empty;
    
    public List<Guid> RelatedChangeOrderIds { get; set; } = new();
    public List<Guid> RelatedClaimIds { get; set; } = new();
    
    public string Notes { get; set; } = string.Empty;
    public Dictionary<string, object> Metadata { get; set; } = new();
}
