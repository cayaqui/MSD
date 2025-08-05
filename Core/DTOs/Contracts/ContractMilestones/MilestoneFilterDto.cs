using Core.Enums.Contracts;

namespace Core.DTOs.Contracts.ContractMilestones;

public class MilestoneFilterDto
{
    public string? SearchTerm { get; set; }
    public Guid? ContractId { get; set; }
    public MilestoneStatus? Status { get; set; }
    public MilestoneType? Type { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public bool? IsCriticalPath { get; set; }
    public bool? IsDelayed { get; set; }
}