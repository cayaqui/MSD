namespace Core.DTOs.Cost.BudgetRevisions;

/// <summary>
/// DTO for creating a Budget Revision
/// </summary>
public class CreateBudgetRevisionDto
{
    public int RevisionNumber { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal NewTotalAmount { get; set; }
}
