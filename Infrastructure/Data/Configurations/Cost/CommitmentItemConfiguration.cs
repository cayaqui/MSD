using Domain.Entities.Cost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Cost;

/// <summary>
/// Entity configuration for CommitmentItem
/// </summary>
public class CommitmentItemConfiguration : IEntityTypeConfiguration<CommitmentItem>
{
    public void Configure(EntityTypeBuilder<CommitmentItem> builder)
    {
        // Table name and schema with check constraints
        builder.ToTable("CommitmentItems", "Cost", t =>
        {
            t.HasCheckConstraint("CK_CommitmentItems_ItemNumber",
                "[ItemNumber] > 0");
            t.HasCheckConstraint("CK_CommitmentItems_Quantity",
                "[Quantity] > 0");
            t.HasCheckConstraint("CK_CommitmentItems_UnitPrice",
                "[UnitPrice] >= 0");
            t.HasCheckConstraint("CK_CommitmentItems_TotalPrice",
                "[TotalPrice] >= 0");
            t.HasCheckConstraint("CK_CommitmentItems_NetAmount",
                "[NetAmount] >= 0");
            t.HasCheckConstraint("CK_CommitmentItems_LineTotal",
                "[LineTotal] >= 0");
            t.HasCheckConstraint("CK_CommitmentItems_DiscountPercentage",
                "[DiscountPercentage] IS NULL OR ([DiscountPercentage] >= 0 AND [DiscountPercentage] <= 100)");
            t.HasCheckConstraint("CK_CommitmentItems_DiscountAmount",
                "[DiscountAmount] IS NULL OR [DiscountAmount] >= 0");
            t.HasCheckConstraint("CK_CommitmentItems_TaxRate",
                "[TaxRate] IS NULL OR ([TaxRate] >= 0 AND [TaxRate] <= 100)");
            t.HasCheckConstraint("CK_CommitmentItems_TaxAmount",
                "[TaxAmount] IS NULL OR [TaxAmount] >= 0");
            t.HasCheckConstraint("CK_CommitmentItems_DeliveredQuantity",
                "[DeliveredQuantity] >= 0 AND [DeliveredQuantity] <= [Quantity]");
            t.HasCheckConstraint("CK_CommitmentItems_InvoicedQuantity",
                "[InvoicedQuantity] >= 0 AND [InvoicedQuantity] <= [Quantity]");
            t.HasCheckConstraint("CK_CommitmentItems_InvoicedAmount",
                "[InvoicedAmount] >= 0");
            t.HasCheckConstraint("CK_CommitmentItems_PaidAmount",
                "[PaidAmount] >= 0 AND [PaidAmount] <= [InvoicedAmount]");
            t.HasCheckConstraint("CK_CommitmentItems_DeliveryDates",
                "[PromisedDate] IS NULL OR [RequiredDate] IS NULL OR [PromisedDate] >= [RequiredDate]");
        });

        // Primary Key
        builder.HasKey(e => e.Id);

        // Properties
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.ItemCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.DetailedDescription)
            .HasMaxLength(2000);

        builder.Property(e => e.Specifications)
            .HasMaxLength(4000);

        builder.Property(e => e.UnitOfMeasure)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(e => e.Currency)
            .IsRequired()
            .HasMaxLength(3)
            .IsFixedLength();

        builder.Property(e => e.Quantity)
            .HasPrecision(18, 4);

        builder.Property(e => e.UnitPrice)
            .HasPrecision(18, 4);

        builder.Property(e => e.TotalPrice)
            .HasPrecision(18, 2);

        builder.Property(e => e.DiscountPercentage)
            .HasPrecision(5, 2);

        builder.Property(e => e.DiscountAmount)
            .HasPrecision(18, 2);

        builder.Property(e => e.NetAmount)
            .HasPrecision(18, 2);

        builder.Property(e => e.TaxRate)
            .HasPrecision(5, 2);

        builder.Property(e => e.TaxAmount)
            .HasPrecision(18, 2);

        builder.Property(e => e.LineTotal)
            .HasPrecision(18, 2);

        builder.Property(e => e.DeliveredQuantity)
            .HasPrecision(18, 4);

        builder.Property(e => e.InvoicedQuantity)
            .HasPrecision(18, 4);

        builder.Property(e => e.InvoicedAmount)
            .HasPrecision(18, 2);

        builder.Property(e => e.PaidAmount)
            .HasPrecision(18, 2);

        builder.Property(e => e.DeliveryLocation)
            .HasMaxLength(200);

        builder.Property(e => e.DeliveryInstructions)
            .HasMaxLength(1000);

        builder.Property(e => e.DrawingNumber)
            .HasMaxLength(50);

        builder.Property(e => e.SpecificationReference)
            .HasMaxLength(100);

        builder.Property(e => e.MaterialCode)
            .HasMaxLength(50);

        builder.Property(e => e.VendorItemCode)
            .HasMaxLength(50);

        builder.Property(e => e.Status)
            .IsRequired()
            .HasConversion<int>();

        // Indexes
        builder.HasIndex(e => new { e.CommitmentId, e.ItemNumber })
            .IsUnique()
            .HasDatabaseName("IX_CommitmentItems_CommitmentId_ItemNumber");

        builder.HasIndex(e => e.ItemCode)
            .HasDatabaseName("IX_CommitmentItems_ItemCode");

        builder.HasIndex(e => e.MaterialCode)
            .HasDatabaseName("IX_CommitmentItems_MaterialCode");

        builder.HasIndex(e => e.Status)
            .HasDatabaseName("IX_CommitmentItems_Status");

        builder.HasIndex(e => new { e.IsDeleted, e.Status })
            .HasDatabaseName("IX_CommitmentItems_IsDeleted_Status");

        // Relationships
        builder.HasOne(e => e.Commitment)
            .WithMany(c => c.Items)
            .HasForeignKey(e => e.CommitmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.BudgetItem)
            .WithMany()
            .HasForeignKey(e => e.BudgetItemId)
            .OnDelete(DeleteBehavior.Restrict);

        // Global Query Filter
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}