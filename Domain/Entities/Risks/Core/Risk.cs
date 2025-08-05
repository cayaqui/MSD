using Domain.Common;
using Domain.Entities.Auth.Security;
using Domain.Entities.Organization.Core;
using Core.Enums.Risk;

namespace Domain.Entities.Risks.Core;

public class Risk : BaseAuditableEntity
{
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    public Guid ProjectId { get; set; }
    public virtual Project Project { get; set; } = null!;
    
    // Risk Assessment
    public string Category { get; set; } = string.Empty;
    public RiskType Type { get; set; }
    public int Probability { get; set; } // 1-5
    public int Impact { get; set; } // 1-5
    public int RiskScore => Probability * Impact;
    public RiskLevel RiskLevel => CalculateRiskLevel();
    
    // Risk Details
    public string Cause { get; set; } = string.Empty;
    public string Effect { get; set; } = string.Empty;
    public decimal? CostImpact { get; set; }
    public int? ScheduleImpact { get; set; } // Days
    public string? QualityImpact { get; set; }
    
    // Risk Response
    public ResponseStrategy? ResponseStrategy { get; set; }
    public string? ResponsePlan { get; set; }
    public Guid? ResponseOwnerId { get; set; }
    public virtual User? ResponseOwner { get; set; }
    public DateTime? ResponseDueDate { get; set; }
    public decimal? ResponseCost { get; set; }
    
    // Residual Risk
    public int? ResidualProbability { get; set; }
    public int? ResidualImpact { get; set; }
    public int? ResidualRiskScore => ResidualProbability * ResidualImpact;
    public RiskLevel? ResidualRiskLevel => CalculateResidualRiskLevel();
    
    // Status and Tracking
    public RiskStatus Status { get; set; }
    public DateTime IdentifiedDate { get; set; }
    public Guid IdentifiedById { get; set; }
    public virtual User IdentifiedBy { get; set; } = null!;
    public DateTime? ClosedDate { get; set; }
    public string? ClosureReason { get; set; }
    
    // Monitoring
    public DateTime? LastReviewDate { get; set; }
    public DateTime? NextReviewDate { get; set; }
    public string? TriggerIndicators { get; set; }
    public bool IsActive => Status != RiskStatus.Closed;
    
    // Navigation properties
    public virtual ICollection<RiskResponse> Responses { get; set; } = new List<RiskResponse>();
    public virtual ICollection<RiskReview> Reviews { get; set; } = new List<RiskReview>();
    
    private RiskLevel CalculateRiskLevel()
    {
        return RiskScore switch
        {
            >= 20 => RiskLevel.Critical,
            >= 12 => RiskLevel.High,
            >= 6 => RiskLevel.Medium,
            _ => RiskLevel.Low
        };
    }
    
    private RiskLevel? CalculateResidualRiskLevel()
    {
        if (!ResidualRiskScore.HasValue) return null;
        
        return ResidualRiskScore.Value switch
        {
            >= 20 => RiskLevel.Critical,
            >= 12 => RiskLevel.High,
            >= 6 => RiskLevel.Medium,
            _ => RiskLevel.Low
        };
    }
}