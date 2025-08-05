using Domain.Entities.Cost.Control;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Cost
{
    public class ActualCostConfiguration : IEntityTypeConfiguration<ActualCost>
    {
        public void Configure(EntityTypeBuilder<ActualCost> builder)
        {
            // Table name and schema
            builder.ToTable("ActualCosts", "Cost");

            // Primary key
            builder.HasKey(a => a.Id);

            // Indexes
            builder.HasIndex(a => a.CostItemId);
            builder.HasIndex(a => a.ActualDate);
            builder.HasIndex(a => new { a.CostItemId, a.ActualDate });

            // Properties
            builder.Property(a => a.Amount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(a => a.ActualDate)
                .IsRequired();

            builder.Property(a => a.InvoiceReference)
                .HasMaxLength(100);

            builder.Property(a => a.Description)
                .HasMaxLength(500);

            builder.Property(a => a.ApprovedBy)
                .HasMaxLength(256);

            // Audit properties
            builder.Property(a => a.CreatedAt)
                .IsRequired();

            builder.Property(a => a.CreatedBy)
                .HasMaxLength(256);

            builder.Property(a => a.UpdatedAt);

            builder.Property(a => a.UpdatedBy)
                .HasMaxLength(256);

            // Relationships
            builder.HasOne(a => a.CostItem)
                .WithMany()
                .HasForeignKey(a => a.CostItemId)
                .OnDelete(DeleteBehavior.Restrict);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_ActualCosts_Amount", "[Amount] >= 0");
            });
        }
    }
}