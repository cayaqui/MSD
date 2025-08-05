namespace Core.DTOs.Contracts.ContractMilestones;

public class RecordMilestonePaymentDto
{
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    public string PaymentReference { get; set; } = string.Empty;
}