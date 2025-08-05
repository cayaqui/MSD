namespace Core.DTOs.Contracts.ContractMilestones;

public class MilestoneFinancialSummary
{
    public Guid ContractId { get; set; }
    public decimal TotalMilestoneValue { get; set; }
    public decimal CompletedMilestoneValue { get; set; }
    public decimal InvoicedAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal PendingInvoiceAmount { get; set; }
    public decimal OutstandingAmount { get; set; }
}