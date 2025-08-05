using Core.DTOs.Common;
using Core.Enums.Contracts;

namespace Core.DTOs.Contracts.ContractMilestones;

public class MilestoneQueryParameters : SimpleQueryParameters
{
    public Guid? ContractId { get; set; }
    public MilestoneStatus? Status { get; set; }
    public MilestoneType? Type { get; set; }
}