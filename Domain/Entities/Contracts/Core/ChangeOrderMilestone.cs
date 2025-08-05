using Domain.Common;

namespace Domain.Entities.Contracts.Core;

public class ChangeOrderMilestone : BaseEntity
{
    public Guid ChangeOrderId { get; set; }
    public virtual ContractChangeOrder ChangeOrder { get; set; } = null!;
    
    public Guid MilestoneId { get; set; }
    public virtual ContractMilestone Milestone { get; set; } = null!;
    
    public string ImpactType { get; set; } = string.Empty; // Schedule, Cost, Scope
    public string ImpactDescription { get; set; } = string.Empty;
}
