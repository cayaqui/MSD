namespace Core.DTOs.Contracts.Valuations;

public class ValuationFinancialSummary
{
    public Guid ContractId { get; set; }
    public int TotalValuations { get; set; }
    public int ApprovedValuations { get; set; }
    public decimal TotalValuedAmount { get; set; }
    public decimal TotalApprovedAmount { get; set; }
    public decimal TotalCertifiedAmount { get; set; }
    public decimal TotalPaidAmount { get; set; }
    public decimal RetentionHeld { get; set; }
    public decimal PendingApproval { get; set; }
}