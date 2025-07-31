namespace Core.DTOs.Budget;

/// <summary>
/// DTO for creating a Budget Revision
/// </summary>
public class CreateBudgetRevisionDto
{
    public string Reason { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal NewTotalAmount { get; set; }
}
