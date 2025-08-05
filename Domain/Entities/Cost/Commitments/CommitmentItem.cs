using Domain.Common;
using Domain.Entities.Cost.Budget;
using Domain.Entities.Cost.Core;
using Core.Enums.Cost;

namespace Domain.Entities.Cost.Commitments;

/// <summary>
/// Represents individual line items within a commitment
/// </summary>
public class CommitmentItem : BaseEntity, IAuditable, ISoftDelete
{
    // Foreign Keys
    public Guid CommitmentId { get; private set; }
    public Commitment Commitment { get; private set; } = null!;

    public Guid? BudgetItemId { get; private set; }
    public BudgetItem? BudgetItem { get; private set; }

    // Item Information
    public int ItemNumber { get; private set; }
    public string ItemCode { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string? DetailedDescription { get; private set; }
    public string? Specifications { get; private set; }

    // Quantity and Unit
    public decimal Quantity { get; private set; }
    public string UnitOfMeasure { get; private set; } = string.Empty;
    public decimal UnitPrice { get; private set; }
    public string Currency { get; private set; } = string.Empty;

    // Financial Information
    public decimal TotalPrice { get; private set; }
    public decimal? DiscountPercentage { get; private set; }
    public decimal? DiscountAmount { get; private set; }
    public decimal NetAmount { get; private set; }
    public decimal? TaxRate { get; private set; }
    public decimal? TaxAmount { get; private set; }
    public decimal LineTotal { get; private set; }

    // Delivery Information
    public DateTime? RequiredDate { get; private set; }
    public DateTime? PromisedDate { get; private set; }
    public string? DeliveryLocation { get; private set; }
    public string? DeliveryInstructions { get; private set; }

    // Status and Progress
    public CommitmentItemStatus Status { get; private set; }
    public decimal DeliveredQuantity { get; private set; }
    public decimal InvoicedQuantity { get; private set; }
    public decimal InvoicedAmount { get; private set; }
    public decimal PaidAmount { get; private set; }

    // References
    public string? DrawingNumber { get; private set; }
    public string? SpecificationReference { get; private set; }
    public string? MaterialCode { get; private set; }
    public string? VendorItemCode { get; private set; }

    // ISoftDelete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // Navigation Properties
    public ICollection<InvoiceItem> InvoiceItems { get; private set; } = new List<InvoiceItem>();

    private CommitmentItem() { } // EF Core

    public CommitmentItem(
        Guid commitmentId,
        int itemNumber,
        string itemCode,
        string description,
        decimal quantity,
        string unitOfMeasure,
        decimal unitPrice,
        string currency)
    {
        CommitmentId = commitmentId;
        ItemNumber = itemNumber;
        ItemCode = itemCode ?? throw new ArgumentNullException(nameof(itemCode));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Quantity = quantity;
        UnitOfMeasure = unitOfMeasure ?? throw new ArgumentNullException(nameof(unitOfMeasure));
        UnitPrice = unitPrice;
        Currency = currency ?? throw new ArgumentNullException(nameof(currency));

        Status = CommitmentItemStatus.Active;
        DeliveredQuantity = 0;
        InvoicedQuantity = 0;
        InvoicedAmount = 0;
        PaidAmount = 0;

        CalculateTotals();
        CreatedAt = DateTime.UtcNow;
    }

    // Methods
    public void UpdateDetails(string description, string? detailedDescription, string? specifications)
    {
        Description = description ?? throw new ArgumentNullException(nameof(description));
        DetailedDescription = detailedDescription;
        Specifications = specifications;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateQuantityAndPrice(decimal quantity, decimal unitPrice)
    {
        if (Status == CommitmentItemStatus.Locked)
            throw new InvalidOperationException("Cannot update locked commitment items");

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

        if (unitPrice < 0)
            throw new ArgumentException("Unit price cannot be negative", nameof(unitPrice));

        Quantity = quantity;
        UnitPrice = unitPrice;
        CalculateTotals();
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetDiscount(decimal? discountPercentage, decimal? discountAmount)
    {
        if (discountPercentage.HasValue && discountAmount.HasValue)
            throw new ArgumentException("Cannot set both percentage and amount discount");

        if (discountPercentage.HasValue && (discountPercentage < 0 || discountPercentage > 100))
            throw new ArgumentException("Discount percentage must be between 0 and 100");

        if (discountAmount.HasValue && discountAmount < 0)
            throw new ArgumentException("Discount amount cannot be negative");

        DiscountPercentage = discountPercentage;
        DiscountAmount = discountAmount;
        CalculateTotals();
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetTax(decimal? taxRate)
    {
        if (taxRate.HasValue && (taxRate < 0 || taxRate > 100))
            throw new ArgumentException("Tax rate must be between 0 and 100");

        TaxRate = taxRate;
        CalculateTotals();
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetDeliveryInfo(DateTime? requiredDate, DateTime? promisedDate, string? location, string? instructions)
    {
        RequiredDate = requiredDate;
        PromisedDate = promisedDate;
        DeliveryLocation = location;
        DeliveryInstructions = instructions;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetReferences(string? drawingNumber, string? specReference, string? materialCode, string? vendorCode)
    {
        DrawingNumber = drawingNumber;
        SpecificationReference = specReference;
        MaterialCode = materialCode;
        VendorItemCode = vendorCode;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RecordDelivery(decimal deliveredQuantity)
    {
        if (deliveredQuantity < 0)
            throw new ArgumentException("Delivered quantity cannot be negative");

        if (DeliveredQuantity + deliveredQuantity > Quantity)
            throw new InvalidOperationException("Delivered quantity exceeds ordered quantity");

        DeliveredQuantity += deliveredQuantity;
        UpdateStatus();
        UpdatedAt = DateTime.UtcNow;
    }

    public void RecordInvoice(decimal invoicedQuantity, decimal invoicedAmount)
    {
        if (invoicedQuantity < 0 || invoicedAmount < 0)
            throw new ArgumentException("Invoiced values cannot be negative");

        if (InvoicedQuantity + invoicedQuantity > Quantity)
            throw new InvalidOperationException("Invoiced quantity exceeds ordered quantity");

        InvoicedQuantity += invoicedQuantity;
        InvoicedAmount += invoicedAmount;
        UpdateStatus();
        UpdatedAt = DateTime.UtcNow;
    }

    public void RecordPayment(decimal paidAmount)
    {
        if (paidAmount < 0)
            throw new ArgumentException("Paid amount cannot be negative");

        if (PaidAmount + paidAmount > InvoicedAmount)
            throw new InvalidOperationException("Paid amount exceeds invoiced amount");

        PaidAmount += paidAmount;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == CommitmentItemStatus.Cancelled)
            throw new InvalidOperationException("Item is already cancelled");

        if (InvoicedAmount > 0)
            throw new InvalidOperationException("Cannot cancel item with invoiced amounts");

        Status = CommitmentItemStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Lock()
    {
        Status = CommitmentItemStatus.Locked;
        UpdatedAt = DateTime.UtcNow;
    }

    // Private Methods
    private void CalculateTotals()
    {
        TotalPrice = Quantity * UnitPrice;

        // Apply discount
        decimal discountValue = 0;
        if (DiscountPercentage.HasValue)
        {
            discountValue = TotalPrice * (DiscountPercentage.Value / 100);
        }
        else if (DiscountAmount.HasValue)
        {
            discountValue = DiscountAmount.Value;
        }

        NetAmount = TotalPrice - discountValue;

        // Apply tax
        if (TaxRate.HasValue)
        {
            TaxAmount = NetAmount * (TaxRate.Value / 100);
            LineTotal = NetAmount + TaxAmount.Value;
        }
        else
        {
            TaxAmount = 0;
            LineTotal = NetAmount;
        }
    }

    private void UpdateStatus()
    {
        if (DeliveredQuantity == 0 && InvoicedQuantity == 0)
        {
            Status = CommitmentItemStatus.Active;
        }
        else if (DeliveredQuantity < Quantity)
        {
            Status = CommitmentItemStatus.PartiallyDelivered;
        }
        else if (DeliveredQuantity >= Quantity && InvoicedQuantity < Quantity)
        {
            Status = CommitmentItemStatus.FullyDelivered;
        }
        else if (InvoicedQuantity < Quantity)
        {
            Status = CommitmentItemStatus.PartiallyInvoiced;
        }
        else if (InvoicedQuantity >= Quantity)
        {
            Status = CommitmentItemStatus.FullyInvoiced;
            if (PaidAmount >= InvoicedAmount)
            {
                Status = CommitmentItemStatus.Completed;
            }
        }
    }

    // Calculated Properties

    public decimal GetRemainingQuantity() => Quantity - DeliveredQuantity;
    public decimal GetUninvoicedQuantity() => Quantity - InvoicedQuantity;
    public decimal GetUnpaidAmount() => InvoicedAmount - PaidAmount;

    public decimal GetDeliveryPercentage() => Quantity > 0 ? DeliveredQuantity / Quantity * 100 : 0;
    public decimal GetInvoicedPercentage() => Quantity > 0 ? InvoicedQuantity / Quantity * 100 : 0;
    public decimal GetPaidPercentage() => InvoicedAmount > 0 ? PaidAmount / InvoicedAmount * 100 : 0;

    public bool IsFullyDelivered() => DeliveredQuantity >= Quantity;
    public bool IsFullyInvoiced() => InvoicedQuantity >= Quantity;
    public bool IsFullyPaid() => PaidAmount >= InvoicedAmount;
    public bool IsOverdue() => RequiredDate.HasValue && DateTime.UtcNow > RequiredDate && !IsFullyDelivered();
}