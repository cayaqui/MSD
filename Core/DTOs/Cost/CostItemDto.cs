using Core.Enums.Cost;

namespace Core.DTOs.Cost;

/// <summary>
/// DTO for Cost Item list view
/// </summary>
public class CostItemDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public Guid? WorkPackageId { get; set; }
    public string? WorkPackageCode { get; set; }
    public Guid? ControlAccountId { get; set; }
    public string? ControlAccountCode { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public CostType Type { get; set; }
    public CostCategory Category { get; set; }
    public decimal PlannedCost { get; set; }
    public decimal ActualCost { get; set; }
    public decimal CommittedCost { get; set; }
    public decimal ForecastCost { get; set; }
    public string Currency { get; set; } = string.Empty;
    public CostItemStatus Status { get; set; }
    public bool IsApproved { get; set; }
    public DateTime? TransactionDate { get; set; }
    public DateTime CreatedAt { get; set; }
}
