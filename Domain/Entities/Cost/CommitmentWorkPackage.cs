using Domain.Common;
using Domain.Entities.Projects;
using System;

namespace Domain.Entities.Cost;

/// <summary>
/// Relación entre Commitment y Work Packages para distribución de compromisos contractuales
/// </summary>
public class CommitmentWorkPackage : BaseEntity
{
    public Guid CommitmentId { get; private set; }
    public Guid WBSElementId { get; private set; }
    public decimal AllocatedAmount { get; private set; }
    public decimal InvoicedAmount { get; private set; }
    public decimal RetainedAmount { get; private set; }

    // Progress tracking
    public decimal ProgressPercentage { get; private set; }
    public DateTime? LastProgressUpdate { get; private set; }

    // Payment tracking
    public decimal PaidAmount { get; private set; }
    public decimal PendingAmount => AllocatedAmount - InvoicedAmount;

    // Navigation
    public Commitment Commitment { get; private set; } = null!;
    public WBSElement WBSElement { get; private set; } = null!;

    private CommitmentWorkPackage() { } // EF Core

    public CommitmentWorkPackage(Guid commitmentId, Guid wbsElementId, decimal allocatedAmount)
    {
        CommitmentId = commitmentId;
        WBSElementId = wbsElementId;
        AllocatedAmount = allocatedAmount;
        InvoicedAmount = 0;
        RetainedAmount = 0;
        PaidAmount = 0;
        ProgressPercentage = 0;
        CreatedAt = DateTime.UtcNow;

        Validate();
    }

    public void UpdateAllocation(decimal newAmount)
    {
        if (InvoicedAmount > newAmount)
            throw new InvalidOperationException($"Cannot reduce allocation below invoiced amount ({InvoicedAmount})");

        AllocatedAmount = newAmount;
        UpdatedAt = DateTime.UtcNow;

        Validate();
    }

    public void RecordInvoice(decimal invoiceAmount, decimal retentionAmount = 0)
    {
        if (invoiceAmount < 0)
            throw new ArgumentException("Invoice amount cannot be negative");

        if (retentionAmount < 0)
            throw new ArgumentException("Retention amount cannot be negative");

        if (InvoicedAmount + invoiceAmount > AllocatedAmount)
            throw new InvalidOperationException("Invoice amount exceeds allocated amount");

        InvoicedAmount += invoiceAmount;
        RetainedAmount += retentionAmount;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RecordPayment(decimal paymentAmount)
    {
        if (paymentAmount < 0)
            throw new ArgumentException("Payment amount cannot be negative");

        if (PaidAmount + paymentAmount > InvoicedAmount)
            throw new InvalidOperationException("Payment amount exceeds invoiced amount");

        PaidAmount += paymentAmount;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateProgress(decimal percentage)
    {
        if (percentage < 0 || percentage > 100)
            throw new ArgumentException("Progress must be between 0 and 100");

        ProgressPercentage = percentage;
        LastProgressUpdate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ReleaseRetention(decimal amount)
    {
        if (amount < 0)
            throw new ArgumentException("Release amount cannot be negative");

        if (amount > RetainedAmount)
            throw new InvalidOperationException("Cannot release more than retained amount");

        RetainedAmount -= amount;
        UpdatedAt = DateTime.UtcNow;
    }

    private void Validate()
    {
        if (AllocatedAmount < 0)
            throw new ArgumentException("Allocated amount cannot be negative");
    }

    // Calculated properties
    public decimal GetUtilizationPercentage() => AllocatedAmount > 0 ? (InvoicedAmount / AllocatedAmount) * 100 : 0;

    public decimal GetPaymentPercentage() => InvoicedAmount > 0 ? (PaidAmount / InvoicedAmount) * 100 : 0;

    public bool IsFullyInvoiced() => InvoicedAmount >= AllocatedAmount;

    public bool IsFullyPaid() => PaidAmount >= (InvoicedAmount - RetainedAmount);
}