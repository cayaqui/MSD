using Core.Enums.Contracts;

namespace Core.DTOs.Contracts.ContractMilestones;

public class ContractMilestoneFilterDto
{
    public string? SearchTerm { get; set; }
    public Guid? ContractId { get; set; }
    public MilestoneType? Type { get; set; }
    public MilestoneStatus? Status { get; set; }
    public DateTime? PlannedDateFrom { get; set; }
    public DateTime? PlannedDateTo { get; set; }
    public bool? IsPaymentMilestone { get; set; }
    public bool? IsOverdue { get; set; }
    public bool? IsCritical { get; set; }
    public bool? IsActive { get; set; }
}
