using Core.Enums.Contracts;

namespace Core.DTOs.Contracts.Contracts;

public class ContractFilterDto
{
    public string? SearchTerm { get; set; }
    public Guid? ProjectId { get; set; }
    public Guid? ContractorId { get; set; }
    public ContractType? Type { get; set; }
    public ContractStatus? Status { get; set; }
    public ContractCategory? Category { get; set; }
    public ContractRiskLevel? RiskLevel { get; set; }
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateTo { get; set; }
    public DateTime? EndDateFrom { get; set; }
    public DateTime? EndDateTo { get; set; }
    public decimal? MinValue { get; set; }
    public decimal? MaxValue { get; set; }
    public bool? IsActive { get; set; }
    public bool? HasChangeOrders { get; set; }
    public bool? HasClaims { get; set; }
}
