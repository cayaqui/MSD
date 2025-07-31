using Core.Enums.Cost;

namespace Core.DTOs.Budget;

/// <summary>
/// DTO for creating a Budget
/// </summary>
public class CreateBudgetDto
{
    public Guid ProjectId { get; set; }
    public string Version { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public BudgetType Type { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal ContingencyAmount { get; set; }
    public decimal ManagementReserve { get; set; }
    public string Currency { get; set; } = string.Empty;
    public decimal ExchangeRate { get; set; } = 1.0m;
}
