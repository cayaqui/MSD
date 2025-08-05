namespace Core.DTOs.Contracts.ChangeOrders;

public class ChangeOrderComparisonResult
{
    public ChangeOrderDto ChangeOrder1 { get; set; } = null!;
    public ChangeOrderDto ChangeOrder2 { get; set; } = null!;
    public decimal CostDifference { get; set; }
    public int ScheduleDifference { get; set; }
    public bool TypeMatch { get; set; }
    public bool CategoryMatch { get; set; }
    public bool PriorityMatch { get; set; }
}