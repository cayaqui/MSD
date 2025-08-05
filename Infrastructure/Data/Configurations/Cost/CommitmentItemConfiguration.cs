using Domain.Entities.Cost.Commitments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Cost
{
    public class CommitmentItemConfiguration : IEntityTypeConfiguration<CommitmentItem>
    {
        public void Configure(EntityTypeBuilder<CommitmentItem> builder)
        {
            // Table name and schema
            builder.ToTable("CommitmentItems", "Cost");

            // Primary key
            builder.HasKey(ci => ci.Id);

            // Indexes
            builder.HasIndex(ci => ci.CommitmentId);
            builder.HasIndex(ci => ci.BudgetItemId);
            builder.HasIndex(ci => ci.ItemNumber);
            builder.HasIndex(ci => ci.ItemCode);
            builder.HasIndex(ci => ci.Status);
            builder.HasIndex(ci => ci.IsDeleted);
            builder.HasIndex(ci => new { ci.CommitmentId, ci.ItemNumber }).IsUnique();
            builder.HasIndex(ci => new { ci.CommitmentId, ci.Status });

            // Item Information
            builder.Property(ci => ci.ItemNumber)
                .IsRequired();

            builder.Property(ci => ci.ItemCode)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(ci => ci.Description)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(ci => ci.DetailedDescription)
                .HasMaxLength(2000);

            builder.Property(ci => ci.Specifications)
                .HasMaxLength(2000);

            // Quantity and Unit
            builder.Property(ci => ci.Quantity)
                .IsRequired()
                .HasPrecision(18, 4);

            builder.Property(ci => ci.UnitOfMeasure)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(ci => ci.UnitPrice)
                .IsRequired()
                .HasPrecision(18, 4);

            builder.Property(ci => ci.Currency)
                .IsRequired()
                .HasMaxLength(3);

            // Financial Information
            builder.Property(ci => ci.TotalPrice)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(ci => ci.DiscountPercentage)
                .HasPrecision(5, 2);

            builder.Property(ci => ci.DiscountAmount)
                .HasPrecision(18, 2);

            builder.Property(ci => ci.NetAmount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(ci => ci.TaxRate)
                .HasPrecision(5, 2);

            builder.Property(ci => ci.TaxAmount)
                .HasPrecision(18, 2);

            builder.Property(ci => ci.LineTotal)
                .IsRequired()
                .HasPrecision(18, 2);

            // Delivery Information
            builder.Property(ci => ci.DeliveryLocation)
                .HasMaxLength(500);

            builder.Property(ci => ci.DeliveryInstructions)
                .HasMaxLength(2000);

            // Status and Progress
            builder.Property(ci => ci.Status)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(ci => ci.DeliveredQuantity)
                .IsRequired()
                .HasPrecision(18, 4)
                .HasDefaultValue(0);

            builder.Property(ci => ci.InvoicedQuantity)
                .IsRequired()
                .HasPrecision(18, 4)
                .HasDefaultValue(0);

            builder.Property(ci => ci.InvoicedAmount)
                .IsRequired()
                .HasPrecision(18, 2)
                .HasDefaultValue(0);

            builder.Property(ci => ci.PaidAmount)
                .IsRequired()
                .HasPrecision(18, 2)
                .HasDefaultValue(0);

            // References
            builder.Property(ci => ci.DrawingNumber)
                .HasMaxLength(100);

            builder.Property(ci => ci.SpecificationReference)
                .HasMaxLength(100);

            builder.Property(ci => ci.MaterialCode)
                .HasMaxLength(50);

            builder.Property(ci => ci.VendorItemCode)
                .HasMaxLength(100);

            // Soft Delete
            builder.Property(ci => ci.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(ci => ci.DeletedBy)
                .HasMaxLength(256);

            // Audit properties
            builder.Property(ci => ci.CreatedAt)
                .IsRequired();

            builder.Property(ci => ci.CreatedBy)
                .HasMaxLength(256);

            builder.Property(ci => ci.UpdatedAt);

            builder.Property(ci => ci.UpdatedBy)
                .HasMaxLength(256);

            // Relationships
            builder.HasOne(ci => ci.Commitment)
                .WithMany(c => c.Items)
                .HasForeignKey(ci => ci.CommitmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ci => ci.BudgetItem)
                .WithMany()
                .HasForeignKey(ci => ci.BudgetItemId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(ci => ci.InvoiceItems)
                .WithOne(ii => ii.CommitmentItem)
                .HasForeignKey(ii => ii.CommitmentItemId)
                .OnDelete(DeleteBehavior.Restrict);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_CommitmentItems_Quantity", "[Quantity] > 0");
                t.HasCheckConstraint("CK_CommitmentItems_UnitPrice", "[UnitPrice] >= 0");
                t.HasCheckConstraint("CK_CommitmentItems_Amounts", 
                    "[TotalPrice] >= 0 AND [NetAmount] >= 0 AND [LineTotal] >= 0");
                t.HasCheckConstraint("CK_CommitmentItems_Progress", 
                    "[DeliveredQuantity] >= 0 AND [InvoicedQuantity] >= 0 AND " +
                    "[InvoicedAmount] >= 0 AND [PaidAmount] >= 0");
                t.HasCheckConstraint("CK_CommitmentItems_Percentages", 
                    "[DiscountPercentage] IS NULL OR ([DiscountPercentage] >= 0 AND [DiscountPercentage] <= 100)");
                t.HasCheckConstraint("CK_CommitmentItems_TaxRate", 
                    "[TaxRate] IS NULL OR ([TaxRate] >= 0 AND [TaxRate] <= 100)");
            });

            // Global query filter for soft delete
            builder.HasQueryFilter(ci => !ci.IsDeleted);
        }
    }
}