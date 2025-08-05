using Core.Helpers;
using Domain.Common;
using Domain.Entities.Organization.Core;
using Core.Enums.Cost;

namespace Domain.Entities.Cost.Budget;

/// <summary>
/// Project Budget entity with comprehensive versioning, approval workflow, and financial tracking
/// </summary>
public class Budget : BaseEntity, IAuditable, ISoftDelete
{
    #region Constants
    private const string DEFAULT_CURRENCY = "USD";
    private const decimal DEFAULT_EXCHANGE_RATE = 1.0m;
    private const string DEFAULT_CREATED_BY = "system";
    private const int MAX_NAME_LENGTH = 200;
    private const int MAX_DESCRIPTION_LENGTH = 1000;
    private const int MAX_VERSION_LENGTH = 20;
    private const int MAX_COMMENTS_LENGTH = 2000;
    private const decimal MAX_PERCENTAGE = 100m;
    private const decimal MIN_PERCENTAGE = 0m;
    #endregion

    #region Basic Information
    public Guid ProjectId { get; private set; }
    public string Version { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    #endregion

    #region Status and Type
    public BudgetStatus Status { get; private set; }
    public BudgetType Type { get; private set; }
    public bool IsBaseline { get; private set; }
    public DateTime? BaselineDate { get; private set; }
    public bool IsLocked { get; private set; }
    public DateTime? LockedDate { get; private set; }
    public string? LockedBy { get; private set; }
    #endregion

    #region Financial Information
    public string Currency { get; private set; } = DEFAULT_CURRENCY;
    public decimal ExchangeRate { get; private set; } = DEFAULT_EXCHANGE_RATE;
    public decimal TotalAmount { get; private set; }
    public decimal ContingencyAmount { get; private set; }
    public decimal ContingencyPercentage { get; private set; }
    public decimal ManagementReserve { get; private set; }
    public decimal ManagementReservePercentage { get; private set; }
    #endregion

    #region Approval Information
    public DateTime? SubmittedDate { get; private set; }
    public string? SubmittedBy { get; private set; }
    public DateTime? ApprovalDate { get; private set; }
    public string? ApprovedBy { get; private set; }
    public string? ApprovalComments { get; private set; }
    #endregion

    #region Revision Information
    public Guid? ParentBudgetId { get; private set; }
    public DateTime? LastRevisionDate { get; private set; }
    public int RevisionCount { get; private set; }
    #endregion

    #region Calculated Properties (Used by Extensions)
    /// <summary>
    /// Total budget including contingency and management reserve
    /// </summary>
    public decimal TotalBudget => TotalAmount + ContingencyAmount + ManagementReserve;

    /// <summary>
    /// Amount allocated to budget items
    /// </summary>
    public decimal AllocatedAmount => BudgetItems?.Where(bi => !bi.IsDeleted).Sum(bi => bi.Amount) ?? 0m;

    /// <summary>
    /// Amount not yet allocated to budget items
    /// </summary>
    public decimal UnallocatedAmount => TotalAmount - AllocatedAmount;

    /// <summary>
    /// Percentage of budget allocated
    /// </summary>
    public decimal AllocationPercentage => TotalAmount > 0 ? AllocatedAmount / TotalAmount * 100 : 0;

    /// <summary>
    /// Indicates if budget is over-allocated
    /// </summary>
    public bool IsOverAllocated => AllocatedAmount > TotalAmount;

    /// <summary>
    /// Indicates if budget can be modified
    /// </summary>
    public bool CanBeModified => Status == BudgetStatus.Draft && !IsLocked;
    #endregion

    #region Soft Delete Properties
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
    #endregion

    #region Navigation Properties
    public Project Project { get; private set; } = null!;
    public ICollection<BudgetItem> BudgetItems { get; private set; } = new List<BudgetItem>();
    public ICollection<BudgetRevision> Revisions { get; private set; } = new List<BudgetRevision>();
    public Budget? ParentBudget { get; private set; }
    public ICollection<Budget> ChildBudgets { get; private set; } = new List<Budget>();
    #endregion

    #region Constructors
    /// <summary>
    /// Private constructor for EF Core
    /// </summary>
    private Budget() { }

    /// <summary>
    /// Creates a new budget with basic information
    /// </summary>
    /// <param name="projectId">Project this budget belongs to</param>
    /// <param name="version">Budget version</param>
    /// <param name="name">Budget name</param>
    /// <param name="type">Budget type</param>
    /// <param name="totalAmount">Total budget amount</param>
    /// <param name="currency">Currency code</param>
    /// <param name="createdBy">User creating the budget</param>
    public Budget(
        Guid projectId,
        string version,
        string name,
        BudgetType type,
        decimal totalAmount,
        string? currency = null,
        string? createdBy = null)
    {
        ValidateConstructorParameters(projectId, version, name, totalAmount);

        ProjectId = projectId;
        Version = version.Trim();
        Name = name.Trim();
        Type = type;
        TotalAmount = totalAmount;
        Currency = currency?.Trim() ?? DEFAULT_CURRENCY;
        Status = BudgetStatus.Draft;
        ExchangeRate = DEFAULT_EXCHANGE_RATE;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy ?? DEFAULT_CREATED_BY;

        ValidateFinancialAmounts();
    }

    /// <summary>
    /// Creates a budget revision from an existing budget
    /// </summary>
    /// <param name="parentBudget">Parent budget to create revision from</param>
    /// <param name="newVersion">New version number</param>
    /// <param name="revisionReason">Reason for revision</param>
    /// <param name="createdBy">User creating the revision</param>
    public Budget(Budget parentBudget, string newVersion, string revisionReason, string? createdBy = null)
        : this(parentBudget.ProjectId, newVersion, parentBudget.Name, parentBudget.Type, parentBudget.TotalAmount, parentBudget.Currency, createdBy)
    {
        if (parentBudget == null)
            throw new ArgumentNullException(nameof(parentBudget));

        ParentBudgetId = parentBudget.Id;
        Description = parentBudget.Description;
        ContingencyPercentage = parentBudget.ContingencyPercentage;
        ContingencyAmount = parentBudget.ContingencyAmount;
        ManagementReservePercentage = parentBudget.ManagementReservePercentage;
        ManagementReserve = parentBudget.ManagementReserve;
        ExchangeRate = parentBudget.ExchangeRate;

        // Create revision record
        CreateRevisionRecord(RevisionCount + 1, revisionReason, createdBy ?? DEFAULT_CREATED_BY);
    }
    #endregion

    #region Basic Information Methods
    /// <summary>
    /// Updates basic budget information
    /// </summary>
    /// <param name="name">New budget name</param>
    /// <param name="description">New description</param>
    /// <param name="updatedBy">User making the update</param>
    public void UpdateBasicInfo(string name, string? description = null, string? updatedBy = null)
    {
        ValidateCanBeModified();

        Name = ValidateAndTrimName(name);
        Description = ValidateAndTrimDescription(description);
        UpdateAuditFields(updatedBy);
    }

    /// <summary>
    /// Updates the budget name
    /// </summary>
    /// <param name="name">New name</param>
    /// <param name="updatedBy">User making the update</param>
    public void UpdateName(string name, string? updatedBy = null)
    {
        ValidateCanBeModified();

        Name = ValidateAndTrimName(name);
        UpdateAuditFields(updatedBy);
    }

    /// <summary>
    /// Updates the budget description
    /// </summary>
    /// <param name="description">New description</param>
    /// <param name="updatedBy">User making the update</param>
    public void UpdateDescription(string? description, string? updatedBy = null)
    {
        ValidateCanBeModified();

        Description = ValidateAndTrimDescription(description);
        UpdateAuditFields(updatedBy);
    }
    #endregion

    #region Financial Methods
    /// <summary>
    /// Updates financial amounts
    /// </summary>
    /// <param name="totalAmount">New total amount</param>
    /// <param name="contingencyPercentage">Contingency percentage</param>
    /// <param name="managementReservePercentage">Management reserve percentage</param>
    /// <param name="updatedBy">User making the update</param>
    public void UpdateFinancials(
        decimal totalAmount,
        decimal contingencyPercentage = 0,
        decimal managementReservePercentage = 0,
        string? updatedBy = null)
    {
        ValidateCanBeModified();
        ValidatePercentage(contingencyPercentage, nameof(contingencyPercentage));
        ValidatePercentage(managementReservePercentage, nameof(managementReservePercentage));

        TotalAmount = totalAmount;
        SetContingencyPercentage(contingencyPercentage);
        SetManagementReservePercentage(managementReservePercentage);

        ValidateFinancialAmounts();
        UpdateAuditFields(updatedBy);
    }

    /// <summary>
    /// Updates the total amount
    /// </summary>
    /// <param name="totalAmount">New total amount</param>
    /// <param name="updatedBy">User making the update</param>
    public void UpdateTotalAmount(decimal totalAmount, string? updatedBy = null)
    {
        ValidateCanBeModified();

        if (totalAmount < 0)
            throw new ArgumentException("Total amount cannot be negative", nameof(totalAmount));

        TotalAmount = totalAmount;
        RecalculateFinancialAmounts();
        ValidateFinancialAmounts();
        UpdateAuditFields(updatedBy);
    }

    /// <summary>
    /// Sets the contingency percentage and recalculates amount
    /// </summary>
    /// <param name="percentage">Contingency percentage (0-100)</param>
    /// <param name="updatedBy">User making the update</param>
    public void SetContingencyPercentage(decimal percentage, string? updatedBy = null)
    {
        ValidateCanBeModified();
        ValidatePercentage(percentage, nameof(percentage));

        ContingencyPercentage = percentage;
        ContingencyAmount = TotalAmount * (percentage / 100m);
        UpdateAuditFields(updatedBy);
    }

    /// <summary>
    /// Sets the management reserve percentage and recalculates amount
    /// </summary>
    /// <param name="percentage">Management reserve percentage (0-100)</param>
    /// <param name="updatedBy">User making the update</param>
    public void SetManagementReservePercentage(decimal percentage, string? updatedBy = null)
    {
        ValidateCanBeModified();
        ValidatePercentage(percentage, nameof(percentage));

        ManagementReservePercentage = percentage;
        ManagementReserve = TotalAmount * (percentage / 100m);
        UpdateAuditFields(updatedBy);
    }

    /// <summary>
    /// Sets the exchange rate
    /// </summary>
    /// <param name="rate">Exchange rate</param>
    /// <param name="updatedBy">User making the update</param>
    public void SetExchangeRate(decimal rate, string? updatedBy = null)
    {
        if (rate <= 0)
            throw new ArgumentException("Exchange rate must be positive", nameof(rate));

        ExchangeRate = rate;
        UpdateAuditFields(updatedBy);
    }

    /// <summary>
    /// Updates the currency
    /// </summary>
    /// <param name="currency">New currency code</param>
    /// <param name="updatedBy">User making the update</param>
    public void UpdateCurrency(string currency, string? updatedBy = null)
    {
        ValidateCanBeModified();

        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency cannot be null or empty", nameof(currency));

        Currency = currency.Trim().ToUpperInvariant();
        UpdateAuditFields(updatedBy);
    }
    #endregion

    #region Status Management Methods
    /// <summary>
    /// Submits the budget for approval
    /// </summary>
    /// <param name="userId">User submitting the budget</param>
    public void SubmitForApproval(string userId)
    {
        ValidateUserId(userId);

        if (Status != BudgetStatus.Draft)
            throw new InvalidOperationException("Only draft budgets can be submitted for approval");

        Status = BudgetStatus.UnderReview;
        SubmittedDate = DateTime.UtcNow;
        SubmittedBy = userId;
        UpdateAuditFields(userId);
    }

    /// <summary>
    /// Approves the budget
    /// </summary>
    /// <param name="userId">User approving the budget</param>
    /// <param name="comments">Approval comments</param>
    public void Approve(string userId, string? comments = null)
    {
        ValidateUserId(userId);

        if (Status != BudgetStatus.UnderReview)
            throw new InvalidOperationException("Only budgets under review can be approved");

        Status = BudgetStatus.Approved;
        ApprovalDate = DateTime.UtcNow;
        ApprovedBy = userId;
        ApprovalComments = ValidateAndTrimComments(comments);
        UpdateAuditFields(userId);
    }

    /// <summary>
    /// Rejects the budget
    /// </summary>
    /// <param name="userId">User rejecting the budget</param>
    /// <param name="reason">Rejection reason</param>
    public void Reject(string userId, string reason)
    {
        ValidateUserId(userId);

        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Rejection reason is required", nameof(reason));

        if (Status != BudgetStatus.UnderReview)
            throw new InvalidOperationException("Only budgets under review can be rejected");

        Status = BudgetStatus.Rejected;
        ApprovalDate = DateTime.UtcNow;
        ApprovedBy = userId;
        ApprovalComments = ValidateAndTrimComments(reason);
        UpdateAuditFields(userId);
    }

    /// <summary>
    /// Returns the budget to draft status
    /// </summary>
    /// <param name="userId">User returning the budget to draft</param>
    public void ReturnToDraft(string userId)
    {
        ValidateUserId(userId);

        if (Status != BudgetStatus.Rejected && Status != BudgetStatus.UnderReview)
            throw new InvalidOperationException("Only rejected or under review budgets can be returned to draft");

        Status = BudgetStatus.Draft;
        SubmittedDate = null;
        SubmittedBy = null;
        ApprovalDate = null;
        ApprovedBy = null;
        ApprovalComments = null;
        UpdateAuditFields(userId);
    }
    #endregion

    #region Baseline and Lock Methods
    /// <summary>
    /// Sets this budget as the baseline
    /// </summary>
    /// <param name="userId">User setting the baseline</param>
    public void SetAsBaseline(string userId)
    {
        ValidateUserId(userId);

        if (Status != BudgetStatus.Approved)
            throw new InvalidOperationException("Only approved budgets can be set as baseline");

        IsBaseline = true;
        BaselineDate = DateTime.UtcNow;
        UpdateAuditFields(userId);
    }

    /// <summary>
    /// Removes the baseline status
    /// </summary>
    /// <param name="userId">User removing the baseline status</param>
    public void RemoveBaselineStatus(string userId)
    {
        ValidateUserId(userId);

        IsBaseline = false;
        BaselineDate = null;
        UpdateAuditFields(userId);
    }

    /// <summary>
    /// Locks the budget to prevent modifications
    /// </summary>
    /// <param name="userId">User locking the budget</param>
    public void Lock(string userId)
    {
        ValidateUserId(userId);

        if (Status != BudgetStatus.Approved && !IsBaseline)
            throw new InvalidOperationException("Only approved or baseline budgets can be locked");

        IsLocked = true;
        LockedDate = DateTime.UtcNow;
        LockedBy = userId;
        UpdateAuditFields(userId);
    }

    /// <summary>
    /// Unlocks the budget to allow modifications
    /// </summary>
    /// <param name="userId">User unlocking the budget</param>
    public void Unlock(string userId)
    {
        ValidateUserId(userId);

        IsLocked = false;
        LockedDate = null;
        LockedBy = null;
        UpdateAuditFields(userId);
    }
    #endregion

    #region Budget Items Management
    /// <summary>
    /// Adds a new budget item
    /// </summary>
    /// <param name="itemCode">Item code</param>
    /// <param name="description">Item description</param>
    /// <param name="costType">Cost type</param>
    /// <param name="costCategory">Cost category</param>
    /// <param name="quantity">Quantity</param>
    /// <param name="unitRate">Unit rate</param>
    /// <param name="controlAccountId">Control account ID (optional)</param>
    /// <param name="unitOfMeasure">Unit of measure (optional)</param>
    /// <param name="accountingCode">Accounting code (optional)</param>
    /// <param name="notes">Notes (optional)</param>
    /// <param name="createdBy">User creating the item</param>
    /// <returns>The created budget item</returns>
    public BudgetItem AddBudgetItem(
        string itemCode,
        string description,
        CostType costType,
        CostCategory costCategory,
        decimal quantity,
        decimal unitRate,
        Guid? controlAccountId = null,
        string? unitOfMeasure = null,
        string? accountingCode = null,
        string? notes = null,
        string? createdBy = null)
    {
        ValidateCanBeModified();

        var budgetItem = new BudgetItem(
            Id,
            controlAccountId,
            itemCode,
            description,
            costType,
            costCategory,
            quantity,
            unitRate,
            unitOfMeasure,
            null, // Let it calculate the amount
            accountingCode,
            notes,
            createdBy);

        BudgetItems.Add(budgetItem);
        UpdateAuditFields(createdBy);

        return budgetItem;
    }

    /// <summary>
    /// Removes a budget item
    /// </summary>
    /// <param name="budgetItemId">Budget item ID to remove</param>
    /// <param name="deletedBy">User deleting the item</param>
    public void RemoveBudgetItem(Guid budgetItemId, string? deletedBy = null)
    {
        ValidateCanBeModified();

        var item = BudgetItems.FirstOrDefault(bi => bi.Id == budgetItemId && !bi.IsDeleted);
        if (item == null)
            throw new InvalidOperationException("Budget item not found or already deleted");

        item.SoftDelete(deletedBy);
        UpdateAuditFields(deletedBy);
    }

    /// <summary>
    /// Gets a budget item by ID
    /// </summary>
    /// <param name="budgetItemId">Budget item ID</param>
    /// <returns>Budget item if found</returns>
    public BudgetItem? GetBudgetItem(Guid budgetItemId)
    {
        return BudgetItems.FirstOrDefault(bi => bi.Id == budgetItemId && !bi.IsDeleted);
    }
    #endregion

    #region Revision Management
    /// <summary>
    /// Creates a revision record
    /// </summary>
    /// <param name="revisionNumber">Revision number</param>
    /// <param name="reason">Revision reason</param>
    /// <param name="userId">User creating the revision</param>
    /// <param name="description">Optional description</param>
    /// <returns>The created revision</returns>
    public BudgetRevision CreateRevisionRecord(int revisionNumber, string reason, string userId, string? description = null)
    {
        ValidateUserId(userId);

        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Revision reason cannot be null or empty", nameof(reason));

        var revision = new BudgetRevision(
            Id,
            revisionNumber,
            reason,
            TotalAmount, // Previous amount
            TotalAmount, // New amount (will be updated later)
            userId,
            description);

        Revisions.Add(revision);
        LastRevisionDate = revision.RevisionDate;
        RevisionCount = Revisions.Count;
        UpdateAuditFields(userId);

        return revision;
    }
    #endregion

    #region Soft Delete Methods
    /// <summary>
    /// Soft deletes the budget
    /// </summary>
    /// <param name="deletedBy">User deleting the budget</param>
    public void SoftDelete(string? deletedBy = null)
    {
        if (Status == BudgetStatus.Approved || IsBaseline)
            throw new InvalidOperationException("Cannot delete approved or baseline budgets");

        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    /// <summary>
    /// Restores a soft-deleted budget
    /// </summary>
    /// <param name="restoredBy">User restoring the budget</param>
    public void Restore(string? restoredBy = null)
    {
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
        UpdateAuditFields(restoredBy);
    }
    #endregion

    #region Private Helper Methods
    private void ValidateConstructorParameters(Guid projectId, string version, string name, decimal totalAmount)
    {
        if (projectId == Guid.Empty)
            throw new ArgumentException("Project ID cannot be empty", nameof(projectId));

        ValidateAndTrimVersion(version);
        ValidateAndTrimName(name);

        if (totalAmount < 0)
            throw new ArgumentException("Total amount cannot be negative", nameof(totalAmount));
    }

    private void ValidateCanBeModified()
    {
        if (!CanBeModified)
            throw new InvalidOperationException("Budget cannot be modified in current status or when locked");
    }

    private void ValidateUserId(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User ID cannot be null or empty", nameof(userId));
    }

    private void ValidatePercentage(decimal percentage, string parameterName)
    {
        if (percentage < MIN_PERCENTAGE || percentage > MAX_PERCENTAGE)
            throw new ArgumentException($"Percentage must be between {MIN_PERCENTAGE} and {MAX_PERCENTAGE}", parameterName);
    }

    private string ValidateAndTrimVersion(string version)
    {
        if (string.IsNullOrWhiteSpace(version))
            throw new ArgumentException("Version cannot be null or empty", nameof(version));

        var trimmed = version.Trim();
        if (trimmed.Length > MAX_VERSION_LENGTH)
            throw new ArgumentException($"Version cannot exceed {MAX_VERSION_LENGTH} characters", nameof(version));

        return trimmed;
    }

    private string ValidateAndTrimName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or empty", nameof(name));

        var trimmed = name.Trim();
        if (trimmed.Length > MAX_NAME_LENGTH)
            throw new ArgumentException($"Name cannot exceed {MAX_NAME_LENGTH} characters", nameof(name));

        return trimmed;
    }

    private string? ValidateAndTrimDescription(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
            return null;

        var trimmed = description.Trim();
        if (trimmed.Length > MAX_DESCRIPTION_LENGTH)
            throw new ArgumentException($"Description cannot exceed {MAX_DESCRIPTION_LENGTH} characters", nameof(description));

        return trimmed;
    }

    private string? ValidateAndTrimComments(string? comments)
    {
        if (string.IsNullOrWhiteSpace(comments))
            return null;

        var trimmed = comments.Trim();
        if (trimmed.Length > MAX_COMMENTS_LENGTH)
            throw new ArgumentException($"Comments cannot exceed {MAX_COMMENTS_LENGTH} characters", nameof(comments));

        return trimmed;
    }

    private void ValidateFinancialAmounts()
    {
        if (TotalAmount < 0)
            throw new ArgumentException("Total amount cannot be negative");

        if (ContingencyAmount < 0)
            throw new ArgumentException("Contingency amount cannot be negative");

        if (ManagementReserve < 0)
            throw new ArgumentException("Management reserve cannot be negative");

        if (ExchangeRate <= 0)
            throw new ArgumentException("Exchange rate must be positive");
    }

    private void RecalculateFinancialAmounts()
    {
        ContingencyAmount = TotalAmount * (ContingencyPercentage / 100m);
        ManagementReserve = TotalAmount * (ManagementReservePercentage / 100m);
    }

    private void UpdateAuditFields(string? updatedBy)
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }
    #endregion

    #region Override Methods
    /// <summary>
    /// Returns a string representation of the budget
    /// </summary>
    public override string ToString()
    {
        return $"Budget: {Name} (v{Version}) - {Status} - {Currency} {TotalBudget:N2}";
    }
    #endregion
}