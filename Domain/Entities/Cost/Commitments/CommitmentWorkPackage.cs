using Domain.Common;
using Domain.Entities.WBS;

namespace Domain.Entities.Cost.Commitments;

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

    // Constructor for EF Core
    private CommitmentWorkPackage() { }

    public CommitmentWorkPackage(Guid commitmentId, Guid wbsElementId, decimal allocatedAmount)
    {
        CommitmentId = commitmentId;
        WBSElementId = wbsElementId;
        AllocatedAmount = allocatedAmount;
        InvoicedAmount = 0;
        RetainedAmount = 0;
        PaidAmount = 0;
        ProgressPercentage = 0;
    }

    // Domain Methods
    public void UpdateAllocation(decimal newAmount, string updatedBy)
    {
        if (InvoicedAmount > newAmount)
            throw new InvalidOperationException($"Cannot reduce allocation below invoiced amount of {InvoicedAmount}");

        AllocatedAmount = newAmount;
        UpdatedBy = updatedBy;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RecordInvoice(decimal invoiceAmount, decimal retentionAmount, string updatedBy)
    {
        if (invoiceAmount < 0)
            throw new ArgumentException("Invoice amount cannot be negative");

        if (InvoicedAmount + invoiceAmount > AllocatedAmount)
            throw new InvalidOperationException($"Invoice amount would exceed allocated amount of {AllocatedAmount}");

        InvoicedAmount += invoiceAmount;
        RetainedAmount += retentionAmount;
        UpdatedBy = updatedBy;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RecordPayment(decimal paymentAmount, string updatedBy)
    {
        if (paymentAmount < 0)
            throw new ArgumentException("Payment amount cannot be negative");

        if (PaidAmount + paymentAmount > InvoicedAmount - RetainedAmount)
            throw new InvalidOperationException("Payment would exceed invoiced amount minus retention");

        PaidAmount += paymentAmount;
        UpdatedBy = updatedBy;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ReleaseRetention(decimal amount, string updatedBy)
    {
        if (amount > RetainedAmount)
            throw new InvalidOperationException($"Cannot release more than retained amount of {RetainedAmount}");

        RetainedAmount -= amount;
        UpdatedBy = updatedBy;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateProgress(decimal percentage, string updatedBy)
    {
        if (percentage < 0 || percentage > 100)
            throw new ArgumentException("Progress percentage must be between 0 and 100");

        ProgressPercentage = percentage;
        LastProgressUpdate = DateTime.UtcNow;
        UpdatedBy = updatedBy;
        UpdatedAt = DateTime.UtcNow;
    }

    public decimal GetBalanceToInvoice() => AllocatedAmount - InvoicedAmount;
    public decimal GetBalanceToPay() => InvoicedAmount - RetainedAmount - PaidAmount;
    public bool IsFullyInvoiced() => InvoicedAmount >= AllocatedAmount;
    public bool IsFullyPaid() => PaidAmount >= InvoicedAmount - RetainedAmount;

    public decimal GetUtilizationPercentage()
    {
        return 0; //TODO 
    }
}