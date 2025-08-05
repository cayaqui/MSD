namespace Core.DTOs.Contracts.ChangeOrders;

public class ChangeOrderTrendData
{
    public string Period { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal TotalValue { get; set; }
    public decimal ApprovedValue { get; set; }
}