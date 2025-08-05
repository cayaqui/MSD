using Domain.Common;

namespace Domain.Entities.Contracts.Core;

public class ClaimChangeOrder : BaseEntity
{
    public Guid ClaimId { get; set; }
    public virtual Claim Claim { get; set; } = null!;
    
    public Guid ChangeOrderId { get; set; }
    public virtual ContractChangeOrder ChangeOrder { get; set; } = null!;
    
    public string RelationType { get; set; } = string.Empty; // Cause, Effect, Related
    public string Notes { get; set; } = string.Empty;
}