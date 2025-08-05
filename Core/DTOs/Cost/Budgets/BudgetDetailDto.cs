using Core.DTOs.Cost.BudgetItems;
using Core.DTOs.Cost.BudgetRevisions;

namespace Core.DTOs.Cost.Budgets;

/// <summary>
/// DTO for Budget detail view
/// </summary>
public class BudgetDetailDto : BudgetDto
{
    public decimal ExchangeRate { get; set; }
    public decimal AllocatedAmount { get; set; }
    public decimal UnallocatedAmount { get; set; }
    public decimal AllocationPercentage { get; set; }
    public DateTime? BaselineDate { get; set; }
    public DateTime? SubmittedDate { get; set; }
    public string? SubmittedBy { get; set; }
    public string? ApprovalComments { get; set; }
    public List<BudgetItemDto> BudgetItems { get; set; } = new();
    public List<BudgetRevisionDto> Revisions { get; set; } = new();
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public int ItemCount { get; set; }
}
