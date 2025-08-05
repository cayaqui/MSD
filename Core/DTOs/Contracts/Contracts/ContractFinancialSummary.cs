namespace Core.DTOs.Contracts.Contracts;

public class ContractFinancialSummary
{
    public Guid ContractId { get; set; }
    public decimal OriginalValue { get; set; }
    public decimal CurrentValue { get; set; }
    public decimal ChangeOrderValue { get; set; }
    public decimal PendingChangeOrderValue { get; set; }
    public decimal AmountInvoiced { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal RetentionAmount { get; set; }
    public decimal OutstandingAmount { get; set; }
    public decimal PercentageComplete { get; set; }
    public decimal TotalClaims { get; set; }
    public int OpenClaims { get; set; }
}