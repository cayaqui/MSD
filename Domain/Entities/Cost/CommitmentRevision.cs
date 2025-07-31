

namespace Domain.Entities.Cost;

/// <summary>
/// Represents a revision history for commitments
/// </summary>
public class CommitmentRevision : BaseEntity
{
    // Foreign Keys
    public Guid CommitmentId { get; private set; }
    public Commitment Commitment { get; private set; } = null!;

    // Revision Information
    public int RevisionNumber { get; private set; }
    public DateTime RevisionDate { get; private set; }
    public decimal PreviousAmount { get; private set; }
    public decimal RevisedAmount { get; private set; }
    public decimal ChangeAmount { get; private set; }
    public decimal ChangePercentage { get; private set; }

    // Justification
    public string Reason { get; private set; } = string.Empty;
    public string? ChangeOrderReference { get; private set; }
    public string? ApprovedBy { get; private set; }
    public DateTime? ApprovalDate { get; private set; }

    private CommitmentRevision() { } // EF Core

    public CommitmentRevision(
        Guid commitmentId,
        decimal previousAmount,
        decimal revisedAmount,
        string reason
    )
    {
        CommitmentId = commitmentId;
        PreviousAmount = previousAmount;
        RevisedAmount = revisedAmount;
        ChangeAmount = revisedAmount - previousAmount;
        ChangePercentage =
            previousAmount > 0 ? ((revisedAmount - previousAmount) / previousAmount) * 100 : 0;
        Reason = reason ?? throw new ArgumentNullException(nameof(reason));
        RevisionDate = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
    }

    // Methods
    public void SetApproval(string approvedBy, DateTime? approvalDate = null)
    {
        ApprovedBy = approvedBy;
        ApprovalDate = approvalDate ?? DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetChangeOrderReference(string changeOrderReference)
    {
        ChangeOrderReference = changeOrderReference;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetRevisionNumber(int revisionNumber)
    {
        RevisionNumber = revisionNumber;
        UpdatedAt = DateTime.UtcNow;
    }

    // Calculated Properties
    public bool IsIncrease() => ChangeAmount > 0;

    public bool IsDecrease() => ChangeAmount < 0;

    public bool IsSignificantChange() => Math.Abs(ChangePercentage) > 10;
}
