using Core.Enums.Cost;

namespace Core.DTOs.Cost.BudgetItems;

/// <summary>
/// DTO for Budget Item
/// </summary>
public class BudgetItemDto
{
    public Guid Id { get; set; }
    public Guid? ControlAccountId { get; set; }
    public string? ControlAccountCode { get; set; }
    public Guid? WorkPackageId { get; set; }
    public string? WorkPackageCode { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public CostType CostType { get; set; }
    public CostCategory Category { get; set; }
    public decimal Amount { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitRate { get; set; }
    public string? UnitOfMeasure { get; set; }
    public string? AccountingCode { get; set; }
    public string? Notes { get; set; }
}
