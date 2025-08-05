using Domain.Entities.Contracts.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Contracts
{
    public class ValuationItemConfiguration : IEntityTypeConfiguration<ValuationItem>
    {
        public void Configure(EntityTypeBuilder<ValuationItem> builder)
        {
            // Table name and schema
            builder.ToTable("ValuationItems", "Contracts");

            // Primary key
            builder.HasKey(vi => vi.Id);

            // Indexes
            builder.HasIndex(vi => new { vi.ValuationId, vi.ItemCode }).IsUnique();
            builder.HasIndex(vi => vi.ValuationId);
            builder.HasIndex(vi => vi.WorkPackageCode);
            builder.HasIndex(vi => vi.IsActive);

            // Properties
            builder.Property(vi => vi.ItemCode)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(vi => vi.Description)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(vi => vi.Unit)
                .IsRequired()
                .HasMaxLength(20);

            // Quantities and amounts
            builder.Property(vi => vi.ContractQuantity)
                .HasPrecision(18, 4);

            builder.Property(vi => vi.UnitRate)
                .HasPrecision(18, 4);

            builder.Property(vi => vi.ContractAmount)
                .HasPrecision(18, 2);

            builder.Property(vi => vi.PreviousQuantity)
                .HasPrecision(18, 4);

            builder.Property(vi => vi.PreviousAmount)
                .HasPrecision(18, 2);

            builder.Property(vi => vi.CurrentQuantity)
                .HasPrecision(18, 4);

            builder.Property(vi => vi.CurrentAmount)
                .HasPrecision(18, 2);

            builder.Property(vi => vi.TotalQuantity)
                .HasPrecision(18, 4);

            builder.Property(vi => vi.TotalAmount)
                .HasPrecision(18, 2);

            builder.Property(vi => vi.PercentageComplete)
                .HasPrecision(5, 2);

            // Other properties
            builder.Property(vi => vi.WorkPackageCode)
                .HasMaxLength(50);

            builder.Property(vi => vi.Location)
                .HasMaxLength(256);

            builder.Property(vi => vi.Comments)
                .HasMaxLength(2000);

            builder.Property(vi => vi.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Base audit properties
            builder.Property(vi => vi.CreatedAt)
                .IsRequired();

            builder.Property(vi => vi.CreatedBy)
                .HasMaxLength(256);

            builder.Property(vi => vi.UpdatedAt);

            builder.Property(vi => vi.UpdatedBy)
                .HasMaxLength(256);

            // Relationships
            builder.HasOne(vi => vi.Valuation)
                .WithMany(v => v.Items)
                .HasForeignKey(vi => vi.ValuationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_ValuationItems_Quantities",
                    "[ContractQuantity] >= 0 AND [PreviousQuantity] >= 0 AND [CurrentQuantity] >= 0 AND [TotalQuantity] >= 0");
                t.HasCheckConstraint("CK_ValuationItems_Amounts",
                    "[UnitRate] >= 0 AND [ContractAmount] >= 0 AND [PreviousAmount] >= 0 AND [CurrentAmount] >= 0 AND [TotalAmount] >= 0");
                t.HasCheckConstraint("CK_ValuationItems_PercentageComplete",
                    "[PercentageComplete] >= 0 AND [PercentageComplete] <= 100");
            });
        }
    }
}