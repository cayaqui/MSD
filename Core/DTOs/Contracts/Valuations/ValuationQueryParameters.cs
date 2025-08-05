using Core.DTOs.Common;
using Core.Enums.Contracts;

namespace Core.DTOs.Contracts.Valuations;

public class ValuationQueryParameters : SimpleQueryParameters
{
    public Guid? ContractId { get; set; }
    public ValuationStatus? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool? IsPaid { get; set; }
}