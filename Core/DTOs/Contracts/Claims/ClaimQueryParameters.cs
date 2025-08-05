using Core.DTOs.Common;
using Core.Enums.Contracts;

namespace Core.DTOs.Contracts.Claims;

public class ClaimQueryParameters : SimpleQueryParameters
{
    public Guid? ContractId { get; set; }
    public ClaimStatus? Status { get; set; }
    public ClaimType? Type { get; set; }
}