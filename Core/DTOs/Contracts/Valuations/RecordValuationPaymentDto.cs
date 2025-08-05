namespace Core.DTOs.Contracts.Valuations;

public class RecordValuationPaymentDto
{
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    public string PaymentReference { get; set; } = string.Empty;
}