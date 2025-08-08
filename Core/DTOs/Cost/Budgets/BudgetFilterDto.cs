using Core.DTOs.Common;

namespace Core.DTOs.Cost.Budgets;

public class BudgetFilterDto : BaseFilterDto
{
    public string? Status { get; set; }
    public bool? IsBaseline { get; set; }
    public bool? IsApproved { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
    public override string? SortBy { get; set; } = "Name";
}