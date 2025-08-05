using Core.Enums.Contracts;

namespace Core.DTOs.Contracts.ContractMilestones;

public class UpdateContractMilestoneDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public MilestoneStatus Status { get; set; }
    
    public DateTime? RevisedDate { get; set; }
    public DateTime? ActualDate { get; set; }
    public DateTime? ForecastDate { get; set; }
    
    public string CompletionCriteria { get; set; } = string.Empty;
    public string Deliverables { get; set; } = string.Empty;
    public bool RequiresClientApproval { get; set; }
    
    public decimal PercentageComplete { get; set; }
    public string ProgressComments { get; set; } = string.Empty;
    
    public string VarianceExplanation { get; set; } = string.Empty;
    
    public List<Guid> PredecessorIds { get; set; } = new();
    
    public bool IsCritical { get; set; }
    public string Notes { get; set; } = string.Empty;
    public Dictionary<string, object> Metadata { get; set; } = new();
}
