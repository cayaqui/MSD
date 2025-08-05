using Domain.Entities.Cost.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Cost
{
    public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            // Table name and schema
            builder.ToTable("Invoices", "Cost");

            // Primary key
            builder.HasKey(i => i.Id);

            // Indexes
            builder.HasIndex(i => i.InvoiceNumber).IsUnique();
            builder.HasIndex(i => i.CommitmentId);
            builder.HasIndex(i => i.ContractorId);
            builder.HasIndex(i => i.InvoiceDate);
            builder.HasIndex(i => i.DueDate);
            builder.HasIndex(i => i.Status);
            builder.HasIndex(i => i.Type);
            builder.HasIndex(i => i.IsDeleted);
            builder.HasIndex(i => new { i.CommitmentId, i.InvoiceDate });

            // Invoice Information
            builder.Property(i => i.InvoiceNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(i => i.VendorInvoiceNumber)
                .HasMaxLength(100);

            builder.Property(i => i.Type)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(i => i.Status)
                .IsRequired()
                .HasMaxLength(50);

            // Dates
            builder.Property(i => i.InvoiceDate)
                .IsRequired();

            builder.Property(i => i.ReceivedDate)
                .IsRequired();

            builder.Property(i => i.DueDate)
                .IsRequired();

            // Financial Information
            builder.Property(i => i.GrossAmount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(i => i.TaxAmount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(i => i.NetAmount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(i => i.RetentionAmount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(i => i.DiscountAmount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(i => i.TotalAmount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(i => i.PaidAmount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(i => i.Currency)
                .IsRequired()
                .HasMaxLength(3);

            // Tax Information
            builder.Property(i => i.TaxRate)
                .HasPrecision(5, 2);

            builder.Property(i => i.TaxCode)
                .HasMaxLength(50);

            builder.Property(i => i.IsTaxExempt)
                .IsRequired()
                .HasDefaultValue(false);

            // Period Information
            builder.Property(i => i.PeriodStartDate)
                .IsRequired();

            builder.Property(i => i.PeriodEndDate)
                .IsRequired();

            // Payment Information
            builder.Property(i => i.PaymentReference)
                .HasMaxLength(200);

            builder.Property(i => i.PaymentMethod)
                .HasMaxLength(50);

            builder.Property(i => i.BankReference)
                .HasMaxLength(100);

            // Approval Workflow
            builder.Property(i => i.SubmittedBy)
                .HasMaxLength(256);

            builder.Property(i => i.ReviewedBy)
                .HasMaxLength(256);

            builder.Property(i => i.ApprovedBy)
                .HasMaxLength(256);

            builder.Property(i => i.ApprovalNotes)
                .HasMaxLength(1000);

            builder.Property(i => i.RejectionReason)
                .HasMaxLength(1000);

            // Supporting Documents
            builder.Property(i => i.Description)
                .HasMaxLength(2000);

            builder.Property(i => i.SupportingDocuments)
                .HasMaxLength(2000);

            builder.Property(i => i.HasBackup)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(i => i.IsVerified)
                .IsRequired()
                .HasDefaultValue(false);

            // Soft Delete
            builder.Property(i => i.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(i => i.DeletedBy)
                .HasMaxLength(256);

            // Audit properties
            builder.Property(i => i.CreatedAt)
                .IsRequired();

            builder.Property(i => i.CreatedBy)
                .HasMaxLength(256);

            builder.Property(i => i.UpdatedAt);

            builder.Property(i => i.UpdatedBy)
                .HasMaxLength(256);

            // Relationships
            builder.HasOne(i => i.Commitment)
                .WithMany(c => c.Invoices)
                .HasForeignKey(i => i.CommitmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(i => i.Contractor)
                .WithMany()
                .HasForeignKey(i => i.ContractorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(i => i.InvoiceItems)
                .WithOne(ii => ii.Invoice)
                .HasForeignKey(ii => ii.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Invoices_Amounts", 
                    "[GrossAmount] >= 0 AND [TotalAmount] >= 0 AND [NetAmount] >= 0 AND [PaidAmount] >= 0");
                t.HasCheckConstraint("CK_Invoices_TaxRate", 
                    "[TaxRate] IS NULL OR ([TaxRate] >= 0 AND [TaxRate] <= 100)");
                t.HasCheckConstraint("CK_Invoices_Dates", 
                    "[DueDate] >= [InvoiceDate] AND [PeriodEndDate] >= [PeriodStartDate]");
            });

            // Global query filter for soft delete
            builder.HasQueryFilter(i => !i.IsDeleted);
        }
    }
}