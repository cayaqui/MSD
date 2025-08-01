using Domain.Entities.Cost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Cost;

/// <summary>
/// Entity configuration for BudgetItem
/// </summary>
public class BudgetItemConfiguration : IEntityTypeConfiguration<BudgetItem>
{
    public void Configure(EntityTypeBuilder<BudgetItem> builder)
    {
        // Table name and schema with check constraints
        builder.ToTable("BudgetItems", "Cost", t =>
        {
            t.HasCheckConstraint("CK_BudgetItems_Amount",
                "[Amount] >= 0");
            t.HasCheckConstraint("CK_BudgetItems_Quantity",
                "[Quantity] > 0");
            t.HasCheckConstraint("CK_BudgetItems_UnitRate",
                "[UnitRate] >= 0");
            t.HasCheckConstraint("CK_BudgetItems_SortOrder",
                "[SortOrder] >= 0");
        });

        // Primary key
        builder.HasKey(bi => bi.Id);

        // Indexes
        builder.HasIndex(bi => bi.BudgetId);
        builder.HasIndex(bi => bi.ControlAccountId);
        builder.HasIndex(bi => new { bi.BudgetId, bi.ItemCode }).IsUnique();
        builder.HasIndex(bi => bi.CostType);
        builder.HasIndex(bi => bi.Category);
        builder.HasIndex(bi => new { bi.BudgetId, bi.SortOrder });

        // Properties
        builder.Property(bi => bi.ItemCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(bi => bi.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(bi => bi.CostType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(bi => bi.Category)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(bi => bi.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(bi => bi.Quantity)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(bi => bi.UnitRate)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(bi => bi.UnitOfMeasure)
            .HasMaxLength(20);

        builder.Property(bi => bi.AccountingCode)
            .HasMaxLength(50);

        builder.Property(bi => bi.Notes)
            .HasMaxLength(1000);

        builder.Property(bi => bi.SortOrder)
            .IsRequired()
            .HasDefaultValue(0);

        // Audit properties
        builder.Property(bi => bi.CreatedAt)
            .IsRequired();

        builder.Property(bi => bi.CreatedBy)
            .HasMaxLength(256);

        builder.Property(bi => bi.UpdatedAt);

        builder.Property(bi => bi.UpdatedBy)
            .HasMaxLength(256);

        // Foreign key relationships
        builder.HasOne(bi => bi.Budget)
            .WithMany(b => b.BudgetItems)
            .HasForeignKey(bi => bi.BudgetId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(bi => bi.ControlAccount)
            .WithMany()
            .HasForeignKey(bi => bi.ControlAccountId)
            .OnDelete(DeleteBehavior.SetNull);

    }
}