namespace Core.DTOs.Contracts.ChangeOrders;

public class ApproveChangeOrderDto
{
    public decimal ApprovedCost { get; set; }
    public string ApprovalComments { get; set; } = string.Empty;
    public DateTime? RevisedEndDate { get; set; }
}
