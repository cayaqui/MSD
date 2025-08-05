using Core.Enums.Contracts;

namespace Core.DTOs.Contracts.Contracts;

public class ContractSummaryDto
{
    public int TotalContracts { get; set; }
    public int ActiveContracts { get; set; }
    public int CompletedContracts { get; set; }
    public int DelayedContracts { get; set; }
    
    public decimal TotalOriginalValue { get; set; }
    public decimal TotalCurrentValue { get; set; }
    public decimal TotalChangeOrderValue { get; set; }
    public decimal TotalInvoicedAmount { get; set; }
    public decimal TotalPaidAmount { get; set; }
    public decimal TotalRetentionAmount { get; set; }
    public decimal TotalValue { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal TotalOutstanding { get; set; }
    
    public Dictionary<ContractType, int> ContractsByType { get; set; } = new();
    public Dictionary<ContractStatus, int> ContractsByStatus { get; set; } = new();
    public Dictionary<ContractRiskLevel, int> ContractsByRisk { get; set; } = new();
}