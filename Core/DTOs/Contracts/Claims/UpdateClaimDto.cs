using Core.Enums.Contracts;

namespace Core.DTOs.Contracts.Claims;

public class UpdateClaimDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ClaimStatus Status { get; set; }
    public ClaimPriority Priority { get; set; }
    
    public string ClaimBasis { get; set; } = string.Empty;
    public string ContractualReference { get; set; } = string.Empty;
    public string FactualBasis { get; set; } = string.Empty;
    public string LegalBasis { get; set; } = string.Empty;
    
    public DateTime? ResponseDueDate { get; set; }
    public DateTime? ActualResponseDate { get; set; }
    
    public decimal ClaimedAmount { get; set; }
    public int ClaimedTimeExtension { get; set; }
    
    public List<Guid> RelatedChangeOrderIds { get; set; } = new();
    public List<Guid> RelatedClaimIds { get; set; } = new();
    
    public string Notes { get; set; } = string.Empty;
    public Dictionary<string, object> Metadata { get; set; } = new();
}
