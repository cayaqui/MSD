using Domain.Entities.Projects;

namespace Domain.Entities.Cost;

/// <summary>
/// Project Budget entity with versioning support
/// </summary>
public class Budget : BaseEntity, IAuditable, ISoftDelete
{
    // Basic Information
    public Guid ProjectId { get; private set; }
    public string Version { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    // Status and Type
    public BudgetStatus Status { get; private set; }
    public BudgetType Type { get; private set; }
    public bool IsBaseline { get; private set; }
    public DateTime? BaselineDate { get; private set; }

    // Financial Information
    public string Currency { get; private set; } = "USD";
    public decimal ExchangeRate { get; private set; } = 1.0m;
    public decimal TotalAmount { get; private set; }
    public decimal ContingencyAmount { get; private set; }
    public decimal ManagementReserve { get; private set; }

    // Approval Information
    public DateTime? SubmittedDate { get; private set; }
    public string? SubmittedBy { get; private set; }
    public DateTime? ApprovalDate { get; private set; }
    public string? ApprovedBy { get; private set; }
    public string? ApprovalComments { get; private set; }

    // Calculated Fields
    public decimal TotalBudget => TotalAmount + ContingencyAmount + ManagementReserve;
    public decimal AllocatedAmount => BudgetItems.Sum(bi => bi.Amount);
    public decimal UnallocatedAmount => TotalAmount - AllocatedAmount;

    // Soft Delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // Navigation Properties
    public Project Project { get; private set; } = null!;
    public ICollection<BudgetItem> BudgetItems { get; private set; } = new List<BudgetItem>();
    public ICollection<BudgetRevision> Revisions { get; private set; } = new List<BudgetRevision>();

    // Constructor for EF Core
    private Budget() { }

    public Budget(
        Guid projectId,
        string version,
        string name,
        BudgetType type,
        decimal totalAmount,
        string currency)
    {
        ProjectId = projectId;
        Version = version ?? throw new ArgumentNullException(nameof(version));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Type = type;
        TotalAmount = totalAmount;
        Currency = currency ?? throw new ArgumentNullException(nameof(currency));

        Status = BudgetStatus.Draft;
        ExchangeRate = 1.0m;
        CreatedAt = DateTime.UtcNow;

        Validate();
    }

    // Methods
    public void UpdateBasicInfo(string name, string? description)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateFinancials(decimal totalAmount, decimal contingency, decimal managementReserve)
    {
        if (Status == BudgetStatus.Approved)
            throw new InvalidOperationException("Cannot update approved budget");

        TotalAmount = totalAmount;
        ContingencyAmount = contingency;
        ManagementReserve = managementReserve;
        UpdatedAt = DateTime.UtcNow;

        Validate();
    }

    public void SetExchangeRate(decimal rate)
    {
        if (rate <= 0)
            throw new ArgumentException("Exchange rate must be positive");

        ExchangeRate = rate;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Submit(string userId)
    {
        if (Status != BudgetStatus.Draft)
            throw new InvalidOperationException("Only draft budgets can be submitted");

        Status = BudgetStatus.UnderReview;
        SubmittedDate = DateTime.UtcNow;
        SubmittedBy = userId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Approve(string userId, string? comments)
    {
        if (Status != BudgetStatus.UnderReview)
            throw new InvalidOperationException("Only budgets under review can be approved");

        Status = BudgetStatus.Approved;
        ApprovalDate = DateTime.UtcNow;
        ApprovedBy = userId;
        ApprovalComments = comments;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reject(string userId, string reason)
    {
        if (Status != BudgetStatus.UnderReview)
            throw new InvalidOperationException("Only budgets under review can be rejected");

        Status = BudgetStatus.Rejected;
        ApprovalDate = DateTime.UtcNow;
        ApprovedBy = userId;
        ApprovalComments = reason;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetAsBaseline()
    {
        if (Status != BudgetStatus.Approved)
            throw new InvalidOperationException("Only approved budgets can be set as baseline");

        IsBaseline = true;
        BaselineDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Lock()
    {
        if (Status != BudgetStatus.Approved)
            throw new InvalidOperationException("Only approved budgets can be locked");

        Status = BudgetStatus.Locked;
        UpdatedAt = DateTime.UtcNow;
    }

    // Private Methods
    private void Validate()
    {
        if (TotalAmount < 0)
            throw new ArgumentException("Total amount cannot be negative");

        if (ContingencyAmount < 0)
            throw new ArgumentException("Contingency amount cannot be negative");

        if (ManagementReserve < 0)
            throw new ArgumentException("Management reserve cannot be negative");
    }
}
