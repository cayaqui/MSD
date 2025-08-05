using Domain.Common;

namespace Domain.Entities.Contracts.Core;

public class ClaimRelation : BaseEntity
{
    public Guid ClaimId { get; set; }
    public virtual Claim Claim { get; set; } = null!;
    
    public Guid RelatedClaimId { get; set; }
    public virtual Claim RelatedClaim { get; set; } = null!;
    
    public string RelationType { get; set; } = string.Empty; // Counter-claim, Related, Supersedes, etc.
    public string Notes { get; set; } = string.Empty;
}
