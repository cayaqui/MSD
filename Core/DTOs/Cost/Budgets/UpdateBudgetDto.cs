namespace Core.DTOs.Cost.Budgets;

/// <summary>
/// DTO for updating a Budget
/// </summary>
public class UpdateBudgetDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? TotalAmount { get; set; }
    public decimal? ContingencyAmount { get; set; }
    public decimal? ContingencyPercentage { get; set; }
    public decimal? ManagementReserve { get; set; }
    public decimal? ManagementReservePercentage { get; set; }
    public decimal? ExchangeRate { get; set; }
}
