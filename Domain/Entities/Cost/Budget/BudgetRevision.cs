using Domain.Common;

namespace Domain.Entities.Cost.Budget;

/// <summary>
/// Budget Revision for tracking changes
/// </summary>
public class BudgetRevision : BaseEntity
{
    public Guid BudgetId { get; private set; }
    public int RevisionNumber { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public decimal PreviousAmount { get; private set; }
    public decimal NewAmount { get; private set; }
    public decimal ChangeAmount => NewAmount - PreviousAmount;
    public DateTime RevisionDate { get; private set; }
    public string RevisedBy { get; private set; } = string.Empty;
    public bool IsApproved { get; private set; }
    public DateTime? ApprovalDate { get; private set; }
    public string? ApprovedBy { get; private set; }

    // Navigation Properties
    public Budget Budget { get; private set; } = null!;

    // Constructor for EF Core
    private BudgetRevision() { }

    public BudgetRevision(
        Guid budgetId,
        int revisionNumber,
        string reason,
        decimal previousAmount,
        decimal newAmount,
        string revisedBy,
        string? description = null)
    {
        if (budgetId == Guid.Empty)
            throw new ArgumentException("BudgetId cannot be empty", nameof(budgetId));
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Reason is required", nameof(reason));
        if (string.IsNullOrWhiteSpace(revisedBy))
            throw new ArgumentException("RevisedBy is required", nameof(revisedBy));

        BudgetId = budgetId;
        RevisionNumber = revisionNumber;
        Reason = reason.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        PreviousAmount = previousAmount;
        NewAmount = newAmount;
        RevisedBy = revisedBy.Trim();
        RevisionDate = DateTime.UtcNow;
        IsApproved = false;
    }

    public void Approve(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("UserId is required", nameof(userId));

        IsApproved = true;
        ApprovalDate = DateTime.UtcNow;
        ApprovedBy = userId.Trim();
    }
}