using Domain.Entities.Cost.Budget;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Cost
{
    public class BudgetItemConfiguration : IEntityTypeConfiguration<BudgetItem>
    {
        public void Configure(EntityTypeBuilder<BudgetItem> builder)
        {
            // Table name and schema
            builder.ToTable("BudgetItems", "Cost");

            // Primary key
            builder.HasKey(bi => bi.Id);

            // Indexes
            builder.HasIndex(bi => bi.BudgetId);
            builder.HasIndex(bi => bi.ControlAccountId);
            builder.HasIndex(bi => bi.ItemCode);
            builder.HasIndex(bi => bi.CostType);
            builder.HasIndex(bi => bi.Category);
            builder.HasIndex(bi => bi.IsDeleted);
            builder.HasIndex(bi => new { bi.BudgetId, bi.ItemCode }).IsUnique();
            builder.HasIndex(bi => new { bi.BudgetId, bi.SortOrder });
            builder.HasIndex(bi => new { bi.ControlAccountId, bi.BudgetId });

            // Basic Information
            builder.Property(bi => bi.ItemCode)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(bi => bi.Description)
                .IsRequired()
                .HasMaxLength(500);

            // Cost Information
            builder.Property(bi => bi.CostType)
                .IsRequired();

            builder.Property(bi => bi.Category)
                .IsRequired();

            builder.Property(bi => bi.Amount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(bi => bi.Quantity)
                .IsRequired()
                .HasPrecision(18, 4);

            builder.Property(bi => bi.UnitRate)
                .IsRequired()
                .HasPrecision(18, 4);

            builder.Property(bi => bi.UnitOfMeasure)
                .HasMaxLength(50);

            // Additional Information
            builder.Property(bi => bi.AccountingCode)
                .HasMaxLength(50);

            builder.Property(bi => bi.Notes)
                .HasMaxLength(1000);

            builder.Property(bi => bi.SortOrder)
                .IsRequired()
                .HasDefaultValue(0);

            // Soft Delete
            builder.Property(bi => bi.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(bi => bi.DeletedBy)
                .HasMaxLength(256);

            // Audit properties
            builder.Property(bi => bi.CreatedAt)
                .IsRequired();

            builder.Property(bi => bi.CreatedBy)
                .HasMaxLength(256);

            builder.Property(bi => bi.UpdatedAt);

            builder.Property(bi => bi.UpdatedBy)
                .HasMaxLength(256);

            // Relationships
            builder.HasOne(bi => bi.Budget)
                .WithMany(b => b.BudgetItems)
                .HasForeignKey(bi => bi.BudgetId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(bi => bi.ControlAccount)
                .WithMany()
                .HasForeignKey(bi => bi.ControlAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_BudgetItems_Amount", "[Amount] >= 0");
                t.HasCheckConstraint("CK_BudgetItems_Quantity", "[Quantity] >= 0");
                t.HasCheckConstraint("CK_BudgetItems_UnitRate", "[UnitRate] >= 0");
            });

            // Global query filter for soft delete
            builder.HasQueryFilter(bi => !bi.IsDeleted);
        }
    }
}