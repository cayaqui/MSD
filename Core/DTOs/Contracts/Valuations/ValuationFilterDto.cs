using Core.Enums.Contracts;

namespace Core.DTOs.Contracts.Valuations;

public class ValuationFilterDto
{
    public string? SearchTerm { get; set; }
    public Guid? ContractId { get; set; }
    public ValuationStatus? Status { get; set; }
    public DateTime? PeriodStartFrom { get; set; }
    public DateTime? PeriodStartTo { get; set; }
    public DateTime? ValuationDateFrom { get; set; }
    public DateTime? ValuationDateTo { get; set; }
    public bool? IsInvoiced { get; set; }
    public bool? IsPaid { get; set; }
    public bool? IsActive { get; set; }
}
