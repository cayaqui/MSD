using Core.Enums.Change;

namespace Core.DTOs.Contracts.ChangeOrders;

public class ChangeOrderImpactAnalysis
{
    public Guid ChangeOrderId { get; set; }
    public decimal CostImpact { get; set; }
    public int ScheduleImpactDays { get; set; }
    public decimal PercentageOfContractValue { get; set; }
    public ImpactCategory ImpactCategory { get; set; }
    public ImpactSeverity ImpactSeverity { get; set; }
    public int AffectedMilestoneCount { get; set; }
    public string ImpactDescription { get; set; } = string.Empty;
    public string MitigationPlan { get; set; } = string.Empty;
}