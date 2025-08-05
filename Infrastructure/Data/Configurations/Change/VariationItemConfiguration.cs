using Domain.Entities.Change.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Change
{
    public class VariationItemConfiguration : IEntityTypeConfiguration<VariationItem>
    {
        public void Configure(EntityTypeBuilder<VariationItem> builder)
        {
            // Table name and schema
            builder.ToTable("VariationItems", "Change");

            // Primary key
            builder.HasKey(v => v.Id);

            // Indexes
            builder.HasIndex(v => v.VariationId);
            builder.HasIndex(v => v.ItemCode);

            // Foreign Keys
            builder.Property(v => v.VariationId)
                .IsRequired();

            // Properties
            builder.Property(v => v.ItemCode)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(v => v.Description)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(v => v.Unit)
                .HasMaxLength(20);

            builder.Property(v => v.OriginalQuantity)
                .HasPrecision(18, 4);

            builder.Property(v => v.RevisedQuantity)
                .HasPrecision(18, 4);

            builder.Property(v => v.UnitRate)
                .IsRequired()
                .HasPrecision(18, 4);

            builder.Property(v => v.Amount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(v => v.Remarks)
                .HasMaxLength(1000);

            // Calculated property - ignore
            builder.Ignore(v => v.QuantityChange);

            // Audit properties
            builder.Property(v => v.CreatedAt)
                .IsRequired();

            builder.Property(v => v.CreatedBy)
                .HasMaxLength(256);

            builder.Property(v => v.UpdatedAt);

            builder.Property(v => v.UpdatedBy)
                .HasMaxLength(256);

            // Relationships
            builder.HasOne(v => v.Variation)
                .WithMany(var => var.Items)
                .HasForeignKey(v => v.VariationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_VariationItems_Quantities",
                    "([OriginalQuantity] IS NULL OR [OriginalQuantity] >= 0) AND " +
                    "([RevisedQuantity] IS NULL OR [RevisedQuantity] >= 0)");
                t.HasCheckConstraint("CK_VariationItems_UnitRate",
                    "[UnitRate] >= 0");
            });
        }
    }
}