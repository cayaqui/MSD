using Domain.Entities.Cost.Commitments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Cost
{
    public class CommitmentConfiguration : IEntityTypeConfiguration<Commitment>
    {
        public void Configure(EntityTypeBuilder<Commitment> builder)
        {
            // Table name and schema
            builder.ToTable("Commitments", "Cost");

            // Primary key
            builder.HasKey(c => c.Id);

            // Indexes
            builder.HasIndex(c => c.CommitmentNumber).IsUnique();
            builder.HasIndex(c => c.ProjectId);
            builder.HasIndex(c => c.BudgetItemId);
            builder.HasIndex(c => c.ContractorId);
            builder.HasIndex(c => c.ControlAccountId);
            builder.HasIndex(c => c.Type);
            builder.HasIndex(c => c.Status);
            builder.HasIndex(c => c.ContractDate);
            builder.HasIndex(c => c.IsDeleted);
            builder.HasIndex(c => new { c.ProjectId, c.Status });
            builder.HasIndex(c => new { c.ContractorId, c.Status });

            // Commitment Information
            builder.Property(c => c.CommitmentNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.Title)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(c => c.Description)
                .HasMaxLength(2000);

            builder.Property(c => c.Type)
                .IsRequired();

            builder.Property(c => c.Status)
                .IsRequired();

            // Financial Information
            builder.Property(c => c.OriginalAmount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(c => c.RevisedAmount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(c => c.CommittedAmount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(c => c.InvoicedAmount)
                .IsRequired()
                .HasPrecision(18, 2)
                .HasDefaultValue(0);

            builder.Property(c => c.PaidAmount)
                .IsRequired()
                .HasPrecision(18, 2)
                .HasDefaultValue(0);

            builder.Property(c => c.RetentionAmount)
                .IsRequired()
                .HasPrecision(18, 2)
                .HasDefaultValue(0);

            builder.Property(c => c.Currency)
                .IsRequired()
                .HasMaxLength(3);

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
                .HasMaxLength(50);

            builder.Property(c => c.ContractNumber)
                .HasMaxLength(50);

            builder.Property(c => c.VendorReference)
                .HasMaxLength(100);

            builder.Property(c => c.AccountingReference)
                .HasMaxLength(100);

            // Approval Information
            builder.Property(c => c.ApprovedBy)
                .HasMaxLength(256);

            builder.Property(c => c.ApprovalNotes)
                .HasMaxLength(2000);

            // Performance
            builder.Property(c => c.PerformancePercentage)
                .HasPrecision(5, 2);

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

            // Soft Delete
            builder.Property(c => c.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(c => c.DeletedBy)
                .HasMaxLength(256);

            // Audit properties
            builder.Property(c => c.CreatedAt)
                .IsRequired();

            builder.Property(c => c.CreatedBy)
                .HasMaxLength(256);

            builder.Property(c => c.UpdatedAt);

            builder.Property(c => c.UpdatedBy)
                .HasMaxLength(256);

            // Relationships
            builder.HasOne(c => c.Project)
                .WithMany()
                .HasForeignKey(c => c.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.BudgetItem)
                .WithMany()
                .HasForeignKey(c => c.BudgetItemId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Contractor)
                .WithMany()
                .HasForeignKey(c => c.ContractorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.ControlAccount)
                .WithMany()
                .HasForeignKey(c => c.ControlAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Invoices)
                .WithOne(i => i.Commitment)
                .HasForeignKey(i => i.CommitmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Items)
                .WithOne(i => i.Commitment)
                .HasForeignKey(i => i.CommitmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.WorkPackageAllocations)
                .WithOne(w => w.Commitment)
                .HasForeignKey(w => w.CommitmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Revisions)
                .WithOne(r => r.Commitment)
                .HasForeignKey(r => r.CommitmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Calculated properties (ignored)
            builder.Ignore(c => c.TotalAmount);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Commitments_Amounts", 
                    "[OriginalAmount] >= 0 AND [RevisedAmount] >= 0 AND [CommittedAmount] >= 0 " +
                    "AND [InvoicedAmount] >= 0 AND [PaidAmount] >= 0 AND [RetentionAmount] >= 0");
                t.HasCheckConstraint("CK_Commitments_Dates", 
                    "[EndDate] > [StartDate]");
                t.HasCheckConstraint("CK_Commitments_Percentages", 
                    "[RetentionPercentage] IS NULL OR ([RetentionPercentage] >= 0 AND [RetentionPercentage] <= 100)");
                t.HasCheckConstraint("CK_Commitments_Performance", 
                    "[PerformancePercentage] IS NULL OR ([PerformancePercentage] >= 0 AND [PerformancePercentage] <= 100)");
                t.HasCheckConstraint("CK_Commitments_ContractType", 
                    "(([IsFixedPrice] = 1 AND [IsTimeAndMaterial] = 0) OR ([IsFixedPrice] = 0 AND [IsTimeAndMaterial] = 1))");
            });

            // Global query filter for soft delete
            builder.HasQueryFilter(c => !c.IsDeleted);
        }
    }
}