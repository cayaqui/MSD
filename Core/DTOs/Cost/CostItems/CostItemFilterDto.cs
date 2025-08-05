using Core.DTOs.Common;

namespace Core.DTOs.Cost.CostItems;

/// <summary>
/// Filter DTO for cost items
/// </summary>
public class CostItemFilterDto : QueryParameters
{
    public Guid? ProjectId { get; set; }
    public Guid? ControlAccountId { get; set; }
    public Guid? WorkPackageId { get; set; }
    public string? CostType { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public decimal? AmountFrom { get; set; }
    public decimal? AmountTo { get; set; }
    public bool? HasActualCost { get; set; }
    public bool? HasCommitment { get; set; }
    public bool? IsOverBudget { get; set; }
}