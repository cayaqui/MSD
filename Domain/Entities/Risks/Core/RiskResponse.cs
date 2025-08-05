using Domain.Common;
using Domain.Entities.Auth.Security;
using Core.Enums.Risk;

namespace Domain.Entities.Risks.Core;

public class RiskResponse : BaseAuditableEntity
{
    public Guid RiskId { get; set; }
    public virtual Risk Risk { get; set; } = null!;
    
    public string Strategy { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Actions { get; set; } = string.Empty;
    
    public Guid OwnerId { get; set; }
    public virtual User Owner { get; set; } = null!;
    public DateTime DueDate { get; set; }
    public decimal EstimatedCost { get; set; }
    public decimal? ActualCost { get; set; }
    
    // Expected outcome
    public int ExpectedProbability { get; set; }
    public int ExpectedImpact { get; set; }
    public int ExpectedRiskScore => ExpectedProbability * ExpectedImpact;
    
    public RiskResponseStatus Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string? CompletionNotes { get; set; }
    
    public decimal? EffectivenessScore { get; set; } // 0-100%
    public string? LessonsLearned { get; set; }
}