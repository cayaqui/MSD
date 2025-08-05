namespace Core.DTOs.Contracts.ChangeOrders;

public class ChangeOrderSummaryDto
{
    public Guid ContractId { get; set; }
    public int TotalChangeOrders { get; set; }
    public int ApprovedChangeOrders { get; set; }
    public int PendingChangeOrders { get; set; }
    public decimal TotalValue { get; set; }
    public decimal PendingValue { get; set; }
    public int ScheduleImpactDays { get; set; }
    public decimal PercentageOfOriginalContract { get; set; }
}