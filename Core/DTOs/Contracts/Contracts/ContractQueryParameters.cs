using Core.DTOs.Common;
using Core.Enums.Contracts;

namespace Core.DTOs.Contracts.Contracts;

public class ContractQueryParameters : SimpleQueryParameters
{
    public Guid? ProjectId { get; set; }
    public Guid? ContractorId { get; set; }
    public ContractStatus? Status { get; set; }
    public ContractType? Type { get; set; }
}