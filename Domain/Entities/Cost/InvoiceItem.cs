

namespace Domain.Entities.Cost;

/// <summary>
/// Represents an individual line item within an invoice
/// </summary>
public class InvoiceItem : BaseEntity, IAuditable
{
    // Foreign Keys
    public Guid InvoiceId { get; private set; }
    public Invoice Invoice { get; private set; } = null!;

    public Guid? BudgetItemId { get; private set; }
    public BudgetItem? BudgetItem { get; private set; }

    // Item Information
    public int ItemNumber { get; private set; }
    public string ItemCode { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string? CostAccount { get; private set; }

    // Quantities and Amounts
    public decimal Quantity { get; private set; }
    public string? UnitOfMeasure { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal Amount { get;  set; }
    public decimal? DiscountAmount { get; private set; }
    public decimal NetAmount { get; set; }

    // Progress Information
    public decimal? PreviousQuantity { get; private set; }
    public decimal? CumulativeQuantity { get; private set; }
    public decimal? CompletionPercentage { get; private set; }

    // References
    public string? WorkOrderNumber { get; private set; }
    public string? DeliveryTicket { get; private set; }
    public DateTime? ServiceDate { get; private set; }

    private InvoiceItem() { } // EF Core

    public InvoiceItem(
        Guid invoiceId,
        int itemNumber,
        string itemCode,
        string description,
        decimal quantity,
        decimal unitPrice
    )
    {
        InvoiceId = invoiceId;
        ItemNumber = itemNumber;
        ItemCode = itemCode ?? throw new ArgumentNullException(nameof(itemCode));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Quantity = quantity;
        UnitPrice = unitPrice;
        Amount = quantity * unitPrice;
        NetAmount = Amount;
        CreatedAt = DateTime.UtcNow;
    }

    // Methods
    public void LinkToBudget(Guid budgetItemId)
    {
        BudgetItemId = budgetItemId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateQuantityAndPrice(decimal quantity, decimal unitPrice)
    {
        Quantity = quantity;
        UnitPrice = unitPrice;
        Amount = quantity * unitPrice;
        NetAmount = Amount - (DiscountAmount ?? 0);
        UpdatedAt = DateTime.UtcNow;
    }

    public void ApplyDiscount(decimal discountAmount)
    {
        DiscountAmount = discountAmount;
        NetAmount = Amount - discountAmount;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetProgressInformation(
        decimal? previousQuantity,
        decimal? cumulativeQuantity,
        decimal? completionPercentage
    )
    {
        PreviousQuantity = previousQuantity;
        CumulativeQuantity = cumulativeQuantity;
        CompletionPercentage = completionPercentage;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetReferences(
        string? workOrderNumber,
        string? deliveryTicket,
        DateTime? serviceDate
    )
    {
        WorkOrderNumber = workOrderNumber;
        DeliveryTicket = deliveryTicket;
        ServiceDate = serviceDate;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetCostAccount(string costAccount)
    {
        CostAccount = costAccount;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetUnitOfMeasure(string unitOfMeasure)
    {
        UnitOfMeasure = unitOfMeasure;
        UpdatedAt = DateTime.UtcNow;
    }

    // Calculated Properties
    public decimal GetTotalValue() => NetAmount;

    public decimal GetDiscountPercentage() =>
        Amount > 0 && DiscountAmount.HasValue ? (DiscountAmount.Value / Amount) * 100 : 0;

    public decimal GetProgressQuantity() => CumulativeQuantity ?? Quantity;

    public bool HasDiscount() => DiscountAmount > 0;
}
