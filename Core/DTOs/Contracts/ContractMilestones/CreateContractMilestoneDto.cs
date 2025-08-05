using Core.Enums.Contracts;

namespace Core.DTOs.Contracts.ContractMilestones;

public class CreateContractMilestoneDto
{
    public string MilestoneCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    public Guid ContractId { get; set; }
    public MilestoneType Type { get; set; }
    public int SequenceNumber { get; set; }
    
    public DateTime PlannedDate { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    
    public string CompletionCriteria { get; set; } = string.Empty;
    public string Deliverables { get; set; } = string.Empty;
    public bool RequiresClientApproval { get; set; }
    public bool IsPaymentMilestone { get; set; }
    
    public List<Guid> PredecessorIds { get; set; } = new();
    
    public bool IsCritical { get; set; }
    public string Notes { get; set; } = string.Empty;
    public Dictionary<string, object> Metadata { get; set; } = new();
}
