namespace Domain.Entities.Cost;

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
        string revisedBy)
    {
        BudgetId = budgetId;
        RevisionNumber = revisionNumber;
        Reason = reason ?? throw new ArgumentNullException(nameof(reason));
        PreviousAmount = previousAmount;
        NewAmount = newAmount;
        RevisedBy = revisedBy ?? throw new ArgumentNullException(nameof(revisedBy));

        RevisionDate = DateTime.UtcNow;
        IsApproved = false;
    }

    public void Approve(string userId)
    {
        IsApproved = true;
        ApprovalDate = DateTime.UtcNow;
        ApprovedBy = userId;
    }
}