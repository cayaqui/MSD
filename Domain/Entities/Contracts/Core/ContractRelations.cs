using Domain.Common;

namespace Domain.Entities.Contracts.Core;

public class ChangeOrderRelation : BaseEntity
{
    public Guid ChangeOrderId { get; set; }
    public virtual ContractChangeOrder ChangeOrder { get; set; } = null!;
    
    public Guid RelatedChangeOrderId { get; set; }
    public virtual ContractChangeOrder RelatedChangeOrder { get; set; } = null!;
    
    public string RelationType { get; set; } = string.Empty; // Predecessor, Successor, Related, Supersedes, etc.
    public string Notes { get; set; } = string.Empty;
}
