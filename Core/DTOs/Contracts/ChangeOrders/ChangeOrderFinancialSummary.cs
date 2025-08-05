namespace Core.DTOs.Contracts.ChangeOrders;

public class ChangeOrderFinancialSummary
{
    public Guid ContractId { get; set; }
    public int TotalChangeOrders { get; set; }
    public int ApprovedChangeOrders { get; set; }
    public decimal TotalEstimated { get; set; }
    public decimal TotalApproved { get; set; }
    public decimal TotalActual { get; set; }
    public decimal PendingApproval { get; set; }
    public int ScheduleImpactDays { get; set; }
}