namespace Core.DTOs.Budget;

/// <summary>
/// DTO for Budget Revision
/// </summary>
public class BudgetRevisionDto
{
    public Guid Id { get; set; }
    public int RevisionNumber { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal PreviousAmount { get; set; }
    public decimal NewAmount { get; set; }
    public decimal ChangeAmount { get; set; }
    public DateTime RevisionDate { get; set; }
    public string RevisedBy { get; set; } = string.Empty;
    public bool IsApproved { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public string? ApprovedBy { get; set; }
}
