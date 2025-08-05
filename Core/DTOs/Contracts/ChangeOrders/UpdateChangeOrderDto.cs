using Core.Enums.Change;
using Core.Enums.Cost;

namespace Core.DTOs.Contracts.ChangeOrders;

public class UpdateChangeOrderDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ChangeOrderStatus Status { get; set; }
    public ChangeOrderPriority Priority { get; set; }
    
    public string Justification { get; set; } = string.Empty;
    public string ScopeImpact { get; set; } = string.Empty;
    public string ScheduleImpact { get; set; } = string.Empty;
    
    public decimal EstimatedCost { get; set; }
    public decimal ApprovedCost { get; set; }
    public int ScheduleImpactDays { get; set; }
    public DateTime? RevisedEndDate { get; set; }
    
    public DateTime? ImplementationStartDate { get; set; }
    public DateTime? ImplementationEndDate { get; set; }
    public decimal PercentageComplete { get; set; }
    
    public ChangeOrderRisk RiskAssessment { get; set; }
    public string RiskMitigationPlan { get; set; } = string.Empty;
    
    public List<Guid> RelatedChangeOrderIds { get; set; } = new();
    public List<Guid> AffectedMilestoneIds { get; set; } = new();
    public List<Guid> AffectedWorkPackageIds { get; set; } = new();
    
    public string Notes { get; set; } = string.Empty;
    public Dictionary<string, object> Metadata { get; set; } = new();
}
