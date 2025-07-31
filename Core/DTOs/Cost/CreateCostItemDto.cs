using Core.Enums.Cost;

namespace Core.DTOs.Cost;

/// <summary>
/// DTO for creating a Cost Item
/// </summary>
public class CreateCostItemDto
{
    public Guid ProjectId { get; set; }
    public Guid? WorkPackageId { get; set; }
    public Guid? ControlAccountId { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public CostType Type { get; set; }
    public CostCategory Category { get; set; }
    public decimal PlannedCost { get; set; }
    public string Currency { get; set; } = "USD";
    public string? AccountCode { get; set; }
    public string? CostCenter { get; set; }
}
