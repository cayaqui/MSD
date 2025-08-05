using Core.Enums.Change;

namespace Core.DTOs.Contracts.ChangeOrders;

public class CreateChangeOrderDto
{
    public string ChangeOrderNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    public Guid ContractId { get; set; }
    public ChangeOrderType Type { get; set; }
    public ChangeOrderPriority Priority { get; set; }
    
    public string Justification { get; set; } = string.Empty;
    public string ScopeImpact { get; set; } = string.Empty;
    public string ScheduleImpact { get; set; } = string.Empty;
    
    public decimal EstimatedCost { get; set; }
    public string Currency { get; set; } = "USD";
    public int ScheduleImpactDays { get; set; }
    
    public ChangeOrderRisk RiskAssessment { get; set; }
    public string RiskMitigationPlan { get; set; } = string.Empty;
    
    public List<Guid> RelatedChangeOrderIds { get; set; } = new();
    public List<Guid> AffectedMilestoneIds { get; set; } = new();
    public List<Guid> AffectedWorkPackageIds { get; set; } = new();
    
    public string Notes { get; set; } = string.Empty;
    public Dictionary<string, object> Metadata { get; set; } = new();
}
