using Domain.Entities.Cost.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Cost
{
    public class InvoiceItemConfiguration : IEntityTypeConfiguration<InvoiceItem>
    {
        public void Configure(EntityTypeBuilder<InvoiceItem> builder)
        {
            // Table name and schema
            builder.ToTable("InvoiceItems", "Cost");

            // Primary key
            builder.HasKey(i => i.Id);

            // Indexes
            builder.HasIndex(i => i.InvoiceId);
            builder.HasIndex(i => i.BudgetItemId);
            builder.HasIndex(i => i.CommitmentItemId);
            builder.HasIndex(i => i.ItemCode);
            builder.HasIndex(i => new { i.InvoiceId, i.ItemNumber }).IsUnique();

            // Item Information
            builder.Property(i => i.ItemNumber)
                .IsRequired();

            builder.Property(i => i.ItemCode)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(i => i.Description)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(i => i.CostAccount)
                .HasMaxLength(50);

            // Quantities and Amounts
            builder.Property(i => i.Quantity)
                .IsRequired()
                .HasPrecision(18, 4);

            builder.Property(i => i.UnitOfMeasure)
                .HasMaxLength(20);

            builder.Property(i => i.UnitPrice)
                .IsRequired()
                .HasPrecision(18, 4);

            builder.Property(i => i.Amount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(i => i.DiscountAmount)
                .HasPrecision(18, 2);

            builder.Property(i => i.NetAmount)
                .IsRequired()
                .HasPrecision(18, 2);

            // Progress Information
            builder.Property(i => i.PreviousQuantity)
                .HasPrecision(18, 4);

            builder.Property(i => i.CumulativeQuantity)
                .HasPrecision(18, 4);

            builder.Property(i => i.CompletionPercentage)
                .HasPrecision(5, 2);

            // References
            builder.Property(i => i.WorkOrderNumber)
                .HasMaxLength(100);

            builder.Property(i => i.DeliveryTicket)
                .HasMaxLength(100);

            // Audit properties
            builder.Property(i => i.CreatedAt)
                .IsRequired();

            builder.Property(i => i.CreatedBy)
                .HasMaxLength(256);

            builder.Property(i => i.UpdatedAt);

            builder.Property(i => i.UpdatedBy)
                .HasMaxLength(256);

            // Relationships
            builder.HasOne(i => i.Invoice)
                .WithMany(inv => inv.InvoiceItems)
                .HasForeignKey(i => i.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(i => i.BudgetItem)
                .WithMany()
                .HasForeignKey(i => i.BudgetItemId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(i => i.CommitmentItem)
                .WithMany(ci => ci.InvoiceItems)
                .HasForeignKey(i => i.CommitmentItemId)
                .OnDelete(DeleteBehavior.Restrict);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_InvoiceItems_Quantity", "[Quantity] >= 0");
                t.HasCheckConstraint("CK_InvoiceItems_UnitPrice", "[UnitPrice] >= 0");
                t.HasCheckConstraint("CK_InvoiceItems_Amounts", "[Amount] >= 0 AND [NetAmount] >= 0");
                t.HasCheckConstraint("CK_InvoiceItems_DiscountAmount", "[DiscountAmount] IS NULL OR [DiscountAmount] >= 0");
                t.HasCheckConstraint("CK_InvoiceItems_Progress", 
                    "([PreviousQuantity] IS NULL OR [PreviousQuantity] >= 0) AND " +
                    "([CumulativeQuantity] IS NULL OR [CumulativeQuantity] >= 0) AND " +
                    "([CompletionPercentage] IS NULL OR ([CompletionPercentage] >= 0 AND [CompletionPercentage] <= 100))");
            });
        }
    }
}