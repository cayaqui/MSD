namespace Core.DTOs.Cost.Commitments;

/// <summary>
/// DTO for Commitment Revisions
/// </summary>
public class CommitmentRevisionDto
{
    public Guid Id { get; set; }
    public int RevisionNumber { get; set; }
    public DateTime RevisionDate { get; set; }

    // Amount Changes
    public decimal PreviousAmount { get; set; }
    public decimal RevisedAmount { get; set; }
    public decimal ChangeAmount { get; set; }
    public decimal ChangePercentage { get; set; }

    // Justification
    public string Reason { get; set; } = string.Empty;
    public string? ChangeOrderReference { get; set; }

    // Approval
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovalDate { get; set; }

    // Calculated Properties
    public bool IsIncrease => ChangeAmount > 0;
    public bool IsDecrease => ChangeAmount < 0;
    public bool IsSignificantChange => Math.Abs(ChangePercentage) > 10;
}
