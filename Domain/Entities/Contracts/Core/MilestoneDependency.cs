using Domain.Common;

namespace Domain.Entities.Contracts.Core;

public class MilestoneDependency : BaseEntity
{
    public Guid PredecessorId { get; set; }
    public virtual ContractMilestone Predecessor { get; set; } = null!;
    
    public Guid SuccessorId { get; set; }
    public virtual ContractMilestone Successor { get; set; } = null!;
    
    public string DependencyType { get; set; } = "FS"; // FS, SS, FF, SF
    public int LagDays { get; set; }
}
