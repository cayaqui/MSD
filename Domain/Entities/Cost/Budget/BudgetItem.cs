using ClosedXML.Excel;
using Core.Helpers;
using Domain.Entities.Cost.Control;
using Domain.Common;
using Core.Enums.Cost;

namespace Domain.Entities.Cost.Budget;

/// <summary>
/// Budget Item - detailed line item in a budget with comprehensive cost tracking capabilities
/// </summary>
public class BudgetItem : BaseEntity, IAuditable, ISoftDelete
{
    #region Constants
    private const string DEFAULT_CREATED_BY = "system";
    private const int MAX_ITEM_CODE_LENGTH = 50;
    private const int MAX_DESCRIPTION_LENGTH = 500;
    private const int MAX_NOTES_LENGTH = 1000;
    #endregion

    #region Basic Information
    public Guid BudgetId { get; private set; }
    public Guid? ControlAccountId { get; private set; }
    public string ItemCode { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    #endregion

    #region Cost Information
    public CostType CostType { get; private set; }
    public CostCategory Category { get; private set; }
    public decimal Amount { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal UnitRate { get; private set; }
    public string? UnitOfMeasure { get; private set; }
    #endregion

    #region Additional Information
    public string? AccountingCode { get; private set; }
    public string? Notes { get; private set; }
    public int SortOrder { get; private set; }
    #endregion

    #region Soft Delete Properties
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
    #endregion

    #region Navigation Properties
    public Budget Budget { get; private set; } = null!;
    public ControlAccount? ControlAccount { get; private set; }
    #endregion

    #region Constructors
    /// <summary>
    /// Private constructor for EF Core
    /// </summary>
    private BudgetItem() { }

    /// <summary>
    /// Creates a new budget item with minimal required information
    /// </summary>
    /// <param name="budgetId">The budget this item belongs to</param>
    /// <param name="itemCode">Unique code for this budget item</param>
    /// <param name="createdBy">User who created this item</param>
    /// <exception cref="ArgumentException">Thrown when itemCode is null, empty, or exceeds maximum length</exception>
    public BudgetItem(Guid budgetId, string itemCode, string? createdBy = null)
    {
        ValidateItemCode(itemCode);

        BudgetId = budgetId;
        ItemCode = itemCode;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy ?? DEFAULT_CREATED_BY;
    }

    /// <summary>
    /// Creates a budget item from Excel row data
    /// </summary>
    /// <param name="budgetId">The budget this item belongs to</param>
    /// <param name="itemCode">Unique code for this budget item</param>
    /// <param name="row">Excel row containing budget item data</param>
    /// <exception cref="ArgumentNullException">Thrown when row is null</exception>
    /// <exception cref="InvalidOperationException">Thrown when Excel data is invalid</exception>
    public BudgetItem(Guid budgetId, string itemCode, IXLRow row)
        : this(budgetId, itemCode)
    {
        if (row == null)
            throw new ArgumentNullException(nameof(row));

        try
        {
            Description = ValidateAndTrimDescription(row.Cell(3).GetString());
            CostType = ParseCostTypeFromExcel(row.Cell(6).GetString());
            UnitOfMeasure = row.Cell(7).GetString()?.Trim();
            Quantity = row.Cell(8).GetValue<decimal>();
            UnitRate = row.Cell(9).GetValue<decimal>();
            Amount = row.Cell(10).GetValue<decimal>();

            ValidateAmounts();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Failed to create BudgetItem from Excel row: {ex.Message}",
                ex
            );
        }
    }

    /// <summary>
    /// Creates a complete budget item with all properties
    /// </summary>
    public BudgetItem(
        Guid budgetId,
        Guid? controlAccountId,
        string itemCode,
        string description,
        CostType costType,
        CostCategory category,
        decimal quantity,
        decimal unitRate,
        string? unitOfMeasure = null,
        decimal? amount = null,
        string? accountingCode = null,
        string? notes = null,
        string? createdBy = null
    )
    {
        ValidateItemCode(itemCode);

        BudgetId = budgetId;
        ControlAccountId = controlAccountId;
        ItemCode = itemCode;
        Description = ValidateAndTrimDescription(description);
        CostType = costType;
        Category = category;
        Quantity = quantity;
        UnitRate = unitRate;
        UnitOfMeasure = unitOfMeasure?.Trim();
        AccountingCode = accountingCode?.Trim();
        Notes = ValidateAndTrimNotes(notes);

        // Calculate amount if not provided, otherwise use provided value
        Amount = amount ?? quantity * unitRate;

        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy ?? DEFAULT_CREATED_BY;

        ValidateAmounts();
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Updates the quantity and unit rate, automatically recalculating the amount
    /// </summary>
    /// <param name="quantity">New quantity</param>
    /// <param name="unitRate">New unit rate</param>
    /// <param name="updatedBy">User making the update</param>
    /// <exception cref="ArgumentException">Thrown when values are negative</exception>
    public void UpdateAmount(decimal quantity, decimal unitRate, string? updatedBy = null)
    {
        if (quantity < 0)
            throw new ArgumentException("Quantity cannot be negative", nameof(quantity));
        if (unitRate < 0)
            throw new ArgumentException("Unit rate cannot be negative", nameof(unitRate));

        Quantity = quantity;
        UnitRate = unitRate;
        Amount = quantity * unitRate;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    /// <summary>
    /// Updates the amount directly without changing quantity or unit rate
    /// </summary>
    /// <param name="amount">New amount</param>
    /// <param name="updatedBy">User making the update</param>
    /// <exception cref="ArgumentException">Thrown when amount is negative</exception>
    public void UpdateAmountDirectly(decimal amount, string? updatedBy = null)
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative", nameof(amount));

        Amount = amount;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    /// <summary>
    /// Assigns this budget item to a control account
    /// </summary>
    /// <param name="controlAccountId">ID of the control account</param>
    /// <param name="updatedBy">User making the update</param>
    public void AssignToControlAccount(Guid controlAccountId, string? updatedBy = null)
    {
        ControlAccountId = controlAccountId;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    /// <summary>
    /// Removes the control account assignment
    /// </summary>
    /// <param name="updatedBy">User making the update</param>
    public void RemoveFromControlAccount(string? updatedBy = null)
    {
        ControlAccountId = null;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    /// <summary>
    /// Updates the description of the budget item
    /// </summary>
    /// <param name="description">New description</param>
    /// <param name="updatedBy">User making the update</param>
    public void UpdateDescription(string description, string? updatedBy = null)
    {
        Description = ValidateAndTrimDescription(description);
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    /// <summary>
    /// Updates the notes for this budget item
    /// </summary>
    /// <param name="notes">New notes</param>
    /// <param name="updatedBy">User making the update</param>
    public void UpdateNotes(string? notes, string? updatedBy = null)
    {
        Notes = ValidateAndTrimNotes(notes);
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    /// <summary>
    /// Updates the sort order for this budget item
    /// </summary>
    /// <param name="sortOrder">New sort order</param>
    /// <param name="updatedBy">User making the update</param>
    public void UpdateSortOrder(int sortOrder, string? updatedBy = null)
    {
        SortOrder = sortOrder;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    /// <summary>
    /// Soft deletes this budget item
    /// </summary>
    /// <param name="deletedBy">User performing the deletion</param>
    public void SoftDelete(string? deletedBy = null)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    /// <summary>
    /// Restores a soft-deleted budget item
    /// </summary>
    /// <param name="restoredBy">User performing the restoration</param>
    public void Restore(string? restoredBy = null)
    {
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = restoredBy;
    }
    #endregion

    #region Private Validation Methods
    private static void ValidateItemCode(string itemCode)
    {
        if (string.IsNullOrWhiteSpace(itemCode))
            throw new ArgumentException("Item code cannot be null or empty", nameof(itemCode));

        if (itemCode.Length > MAX_ITEM_CODE_LENGTH)
            throw new ArgumentException(
                $"Item code cannot exceed {MAX_ITEM_CODE_LENGTH} characters",
                nameof(itemCode)
            );
    }

    private static string ValidateAndTrimDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be null or empty", nameof(description));

        var trimmed = description.Trim();
        if (trimmed.Length > MAX_DESCRIPTION_LENGTH)
            throw new ArgumentException(
                $"Description cannot exceed {MAX_DESCRIPTION_LENGTH} characters",
                nameof(description)
            );

        return trimmed;
    }

    private static string? ValidateAndTrimNotes(string? notes)
    {
        if (string.IsNullOrWhiteSpace(notes))
            return null;

        var trimmed = notes.Trim();
        if (trimmed.Length > MAX_NOTES_LENGTH)
            throw new ArgumentException(
                $"Notes cannot exceed {MAX_NOTES_LENGTH} characters",
                nameof(notes)
            );

        return trimmed;
    }

    private static CostType ParseCostTypeFromExcel(string costTypeString)
    {
        if (string.IsNullOrWhiteSpace(costTypeString))
            throw new ArgumentException("Cost type cannot be empty");

        return EnumHelper.GetEnumFromString<CostType>(costTypeString.Trim());
    }

    private void ValidateAmounts()
    {
        if (Amount < 0)
            throw new ArgumentException("Amount cannot be negative");
        if (UnitRate < 0)
            throw new ArgumentException("Unit rate cannot be negative");
        if (Quantity < 0)
            throw new ArgumentException("Quantity cannot be negative");
    }
    #endregion
}
