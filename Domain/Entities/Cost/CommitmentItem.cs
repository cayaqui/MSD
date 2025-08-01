using Domain.Common;
using System;

namespace Domain.Entities.Cost;

/// <summary>
/// Line items within a Commitment (Contract/Purchase Order)
/// </summary>
public class CommitmentItem : BaseEntity
{
    // Foreign Keys
    public Guid CommitmentId { get; private set; }

    // Item Information
    public int LineNumber { get; private set; }
    public string ItemCode { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string? DetailedDescription { get; private set; }

    // Quantities and Rates
    public decimal Quantity { get; private set; }
    public string UnitOfMeasure { get; private set; } = string.Empty;
    public decimal UnitRate { get; private set; }
    public decimal TotalAmount { get; private set; }

    // Classification
    public CostType CostType { get; private set; }
    public CostCategory Category { get; private set; }
    public string? AccountingCode { get; private set; }

    // Progress Tracking
    public decimal DeliveredQuantity { get; private set; }
    public decimal InvoicedQuantity { get; private set; }
    public decimal InvoicedAmount { get; private set; }
    public decimal RetainedAmount { get; private set; }

    // Status
    public CommitmentItemStatus Status { get; private set; }
    public bool IsComplete { get; private set; }
    public DateTime? CompletionDate { get; private set; }

    // Navigation Properties
    public Commitment Commitment { get; private set; } = null!;

    // Constructor for EF Core
    private CommitmentItem() { }

    public CommitmentItem(
        Guid commitmentId,
        int lineNumber,
        string itemCode,
        string description,
        decimal quantity,
        string unitOfMeasure,
        decimal unitRate,
        CostType costType,
        CostCategory category)
    {
        CommitmentId = commitmentId;
        LineNumber = lineNumber;
        ItemCode = itemCode ?? throw new ArgumentNullException(nameof(itemCode));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Quantity = quantity;
        UnitOfMeasure = unitOfMeasure ?? throw new ArgumentNullException(nameof(unitOfMeasure));
        UnitRate = unitRate;
        TotalAmount = quantity * unitRate;
        CostType = costType;
        Category = category;
        Status = CommitmentItemStatus.Active;
    }

    // Domain Methods
    public void UpdateDetails(
        string description,
        string? detailedDescription,
        string? accountingCode,
        string updatedBy)
    {
        Description = description ?? throw new ArgumentNullException(nameof(description));
        DetailedDescription = detailedDescription;
        AccountingCode = accountingCode;
        UpdatedBy = updatedBy;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateQuantityAndRate(
        decimal quantity,
        decimal unitRate,
        string updatedBy)
    {
        if (Status == CommitmentItemStatus.Locked)
            throw new InvalidOperationException("Cannot update locked commitment item");

        if (quantity < DeliveredQuantity)
            throw new InvalidOperationException($"Quantity cannot be less than delivered quantity of {DeliveredQuantity}");

        Quantity = quantity;
        UnitRate = unitRate;
        TotalAmount = quantity * unitRate;
        UpdatedBy = updatedBy;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RecordDelivery(decimal deliveredQty, string updatedBy)
    {
        if (deliveredQty < 0)
            throw new ArgumentException("Delivered quantity cannot be negative");

        if (DeliveredQuantity + deliveredQty > Quantity)
            throw new InvalidOperationException($"Total delivered would exceed ordered quantity of {Quantity}");

        DeliveredQuantity += deliveredQty;

        // Check if fully delivered
        if (DeliveredQuantity >= Quantity)
        {
            Status = CommitmentItemStatus.FullyDelivered;
        }
        else if (DeliveredQuantity > 0)
        {
            Status = CommitmentItemStatus.PartiallyDelivered;
        }

        UpdatedBy = updatedBy;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RecordInvoice(decimal invoicedQty, decimal invoicedAmt, decimal retainedAmt, string updatedBy)
    {
        if (invoicedQty < 0 || invoicedAmt < 0)
            throw new ArgumentException("Invoice values cannot be negative");

        if (InvoicedQuantity + invoicedQty > DeliveredQuantity)
            throw new InvalidOperationException("Cannot invoice more than delivered quantity");

        InvoicedQuantity += invoicedQty;
        InvoicedAmount += invoicedAmt;
        RetainedAmount += retainedAmt;

        // Check if fully invoiced
        if (InvoicedQuantity >= Quantity)
        {
            Status = CommitmentItemStatus.FullyInvoiced;
        }

        UpdatedBy = updatedBy;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete(string completedBy)
    {
        if (IsComplete)
            throw new InvalidOperationException("Item is already marked as complete");

        IsComplete = true;
        CompletionDate = DateTime.UtcNow;
        Status = CommitmentItemStatus.Completed;
        UpdatedBy = completedBy;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Lock(string lockedBy)
    {
        Status = CommitmentItemStatus.Locked;
        UpdatedBy = lockedBy;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel(string cancelledBy)
    {
        if (DeliveredQuantity > 0)
            throw new InvalidOperationException("Cannot cancel item with deliveries");

        Status = CommitmentItemStatus.Cancelled;
        UpdatedBy = cancelledBy;
        UpdatedAt = DateTime.UtcNow;
    }

    // Calculations
    public decimal GetRemainingQuantity() => Quantity - DeliveredQuantity;
    public decimal GetRemainingToInvoice() => DeliveredQuantity - InvoicedQuantity;
    public decimal GetRemainingAmount() => TotalAmount - InvoicedAmount;
    public decimal GetDeliveryPercentage() => Quantity > 0 ? (DeliveredQuantity / Quantity) * 100 : 0;
    public decimal GetInvoicePercentage() => Quantity > 0 ? (InvoicedQuantity / Quantity) * 100 : 0;
}