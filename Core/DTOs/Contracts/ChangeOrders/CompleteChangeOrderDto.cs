namespace Core.DTOs.Contracts.ChangeOrders;

public class CompleteChangeOrderDto
{
    public decimal ActualCost { get; set; }
    public string CompletionNotes { get; set; } = string.Empty;
}