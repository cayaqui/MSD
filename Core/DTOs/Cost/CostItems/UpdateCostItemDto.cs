namespace Core.DTOs.Cost.CostItems;

/// <summary>
/// DTO for updating a Cost Item
/// </summary>
public class UpdateCostItemDto
{
    public string? Description { get; set; }
    public decimal? PlannedCost { get; set; }
    public string? AccountCode { get; set; }
    public string? CostCenter { get; set; }
    public Guid? WorkPackageId { get; set; }
    public Guid? ControlAccountId { get; set; }
}
