namespace Core.DTOs.Contracts.ChangeOrders;

public class ReviewChangeOrderDto
{
    public string ReviewedBy { get; set; } = string.Empty;
    public string Comments { get; set; } = string.Empty;
}