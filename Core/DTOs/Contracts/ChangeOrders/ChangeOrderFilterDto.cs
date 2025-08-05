using Core.Enums.Change;
using Core.Enums.Cost;

namespace Core.DTOs.Contracts.ChangeOrders;

public class ChangeOrderFilterDto
{
    public string? SearchTerm { get; set; }
    public Guid? ContractId { get; set; }
    public ChangeOrderType? Type { get; set; }
    public ChangeOrderStatus? Status { get; set; }
    public ChangeOrderPriority? Priority { get; set; }
    public ChangeOrderRisk? RiskAssessment { get; set; }
    public DateTime? SubmissionDateFrom { get; set; }
    public DateTime? SubmissionDateTo { get; set; }
    public decimal? MinCost { get; set; }
    public decimal? MaxCost { get; set; }
    public bool? IsActive { get; set; }
}
