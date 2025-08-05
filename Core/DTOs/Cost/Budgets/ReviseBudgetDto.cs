namespace Core.DTOs.Cost.Budgets;

/// <summary>
/// DTO for revising a budget
/// </summary>
public class ReviseBudgetDto
{
    public string RevisionNumber { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }
}