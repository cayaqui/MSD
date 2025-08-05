using Domain.Common;
using Domain.Entities.Auth.Security;
using Core.Enums.Risk;

namespace Domain.Entities.Risks.Core;

public class RiskReview : BaseAuditableEntity
{
    public Guid RiskId { get; set; }
    public virtual Risk Risk { get; set; } = null!;
    
    public DateTime ReviewDate { get; set; }
    public Guid ReviewedById { get; set; }
    public virtual User ReviewedBy { get; set; } = null!;
    
    // Updated Assessment
    public int PreviousProbability { get; set; }
    public int PreviousImpact { get; set; }
    public int NewProbability { get; set; }
    public int NewImpact { get; set; }
    
    public string ReviewNotes { get; set; } = string.Empty;
    public string ChangesIdentified { get; set; } = string.Empty;
    public string ActionsRequired { get; set; } = string.Empty;
    
    public bool StatusChanged { get; set; }
    public RiskStatus? PreviousStatus { get; set; }
    public RiskStatus? NewStatus { get; set; }
    
    public bool ResponseStrategyChanged { get; set; }
    public string? UpdatedResponsePlan { get; set; }
    
    public DateTime? NextReviewDate { get; set; }
}