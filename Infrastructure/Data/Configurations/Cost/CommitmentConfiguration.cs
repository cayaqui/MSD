using Domain.Entities.Cost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Cost;

/// <summary>
/// Entity configuration for Commitment (PO, Contract, etc.)
/// </summary>
public class CommitmentConfiguration : IEntityTypeConfiguration<Commitment>
{
    public void Configure(EntityTypeBuilder<Commitment> builder)
    {
        // Table name and schema with check constraints
        builder.ToTable("Commitments", "Cost", t =>
        {
            t.HasCheckConstraint("CK_Commitments_OriginalAmount",
                "[OriginalAmount] > 0");
            t.HasCheckConstraint("CK_Commitments_RevisedAmount",
                "[RevisedAmount] > 0");
            t.HasCheckConstraint("CK_Commitments_CommittedAmount",
                "[CommittedAmount] >= 0");
            t.HasCheckConstraint("CK_Commitments_InvoicedAmount",
                "[InvoicedAmount] >= 0");
            t.HasCheckConstraint("CK_Commitments_PaidAmount",
                "[PaidAmount] >= 0 AND [PaidAmount] <= [InvoicedAmount]");
            t.HasCheckConstraint("CK_Commitments_RetentionAmount",
                "[RetentionAmount] >= 0");
            t.HasCheckConstraint("CK_Commitments_RetentionPercentage",
                "[RetentionPercentage] IS NULL OR ([RetentionPercentage] >= 0 AND [RetentionPercentage] <= 100)");
            t.HasCheckConstraint("CK_Commitments_AdvancePaymentAmount",
                "[AdvancePaymentAmount] IS NULL OR [AdvancePaymentAmount] >= 0");
            t.HasCheckConstraint("CK_Commitments_PerformancePercentage",
                "[PerformancePercentage] IS NULL OR ([PerformancePercentage] >= 0 AND [PerformancePercentage] <= 100)");
            t.HasCheckConstraint("CK_Commitments_PaymentTermsDays",
                "[PaymentTermsDays] IS NULL OR [PaymentTermsDays] > 0");
            t.HasCheckConstraint("CK_Commitments_ContractDates",
                "[EndDate] >= [StartDate] AND [StartDate] >= [ContractDate]");
            t.HasCheckConstraint("CK_Commitments_ExpectedCompletionDate",
                "[ExpectedCompletionDate] IS NULL OR [ExpectedCompletionDate] >= [StartDate]");
            t.HasCheckConstraint("CK_Commitments_LastInvoiceDate",
                "[LastInvoiceDate] IS NULL OR [LastInvoiceDate] >= [ContractDate]");
        });

        // Primary key
        builder.HasKey(c => c.Id);

        // Indexes
        builder.HasIndex(c => new { c.ProjectId, c.CommitmentNumber }).IsUnique();
        builder.HasIndex(c => c.ProjectId);
        builder.HasIndex(c => c.BudgetItemId);
        builder.HasIndex(c => c.ContractorId);
        builder.HasIndex(c => c.Type);
        builder.HasIndex(c => c.Status);
        builder.HasIndex(c => c.IsDeleted);
        builder.HasIndex(c => new { c.ContractDate, c.StartDate, c.EndDate });
        builder.HasIndex(c => c.PurchaseOrderNumber);
        builder.HasIndex(c => c.ContractNumber);
        builder.HasIndex(c => c.ApprovalDate);
        builder.HasIndex(c => c.ExpectedCompletionDate);

        // Properties
        builder.Property(c => c.CommitmentNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.Title)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(c => c.Description)
            .HasMaxLength(2000);

        builder.Property(c => c.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(c => c.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(30);

        // Financial Information
        builder.Property(c => c.OriginalAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(c => c.RevisedAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(c => c.CommittedAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(c => c.InvoicedAmount)
            .HasPrecision(18, 2)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(c => c.PaidAmount)
            .HasPrecision(18, 2)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(c => c.RetentionAmount)
            .HasPrecision(18, 2)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(c => c.Currency)
            .IsRequired()
            .HasMaxLength(3)
            .HasDefaultValue("USD");

        // Contract Terms
        builder.Property(c => c.ContractDate)
            .IsRequired();

        builder.Property(c => c.StartDate)
            .IsRequired();

        builder.Property(c => c.EndDate)
            .IsRequired();

        builder.Property(c => c.PaymentTermsDays);

        builder.Property(c => c.RetentionPercentage)
            .HasPrecision(5, 2);

        builder.Property(c => c.AdvancePaymentAmount)
            .HasPrecision(18, 2);

        // References
        builder.Property(c => c.PurchaseOrderNumber)
            .HasMaxLength(100);

        builder.Property(c => c.ContractNumber)
            .HasMaxLength(100);

        builder.Property(c => c.VendorReference)
            .HasMaxLength(100);

        builder.Property(c => c.AccountingReference)
            .HasMaxLength(100);

        // Approval Information
        builder.Property(c => c.ApprovalDate);

        builder.Property(c => c.ApprovedBy)
            .HasMaxLength(256);

        builder.Property(c => c.ApprovalNotes)
            .HasMaxLength(2000);

        // Performance
        builder.Property(c => c.PerformancePercentage)
            .HasPrecision(5, 2);

        builder.Property(c => c.LastInvoiceDate);

        builder.Property(c => c.ExpectedCompletionDate);

        // Additional Information
        builder.Property(c => c.Terms)
            .HasMaxLength(4000);

        builder.Property(c => c.ScopeOfWork)
            .HasMaxLength(4000);

        builder.Property(c => c.Deliverables)
            .HasMaxLength(4000);

        builder.Property(c => c.IsFixedPrice)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(c => c.IsTimeAndMaterial)
            .IsRequired()
            .HasDefaultValue(false);

        // Audit properties
        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.CreatedBy)
            .HasMaxLength(256);

        builder.Property(c => c.UpdatedAt);

        builder.Property(c => c.UpdatedBy)
            .HasMaxLength(256);

        // Soft delete properties
        builder.Property(c => c.DeletedAt);

        builder.Property(c => c.DeletedBy)
            .HasMaxLength(256);

        // Foreign key relationships

        builder.HasOne(c => c.BudgetItem)
            .WithMany()
            .HasForeignKey(c => c.BudgetItemId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(c => c.Contractor)
            .WithMany()
            .HasForeignKey(c => c.ContractorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Navigation properties
        builder.HasMany(c => c.Invoices)
            .WithOne(i => i.Commitment)
            .HasForeignKey(i => i.CommitmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Revisions)
            .WithOne(cr => cr.Commitment)
            .HasForeignKey(cr => cr.CommitmentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Global query filter for soft delete
        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}