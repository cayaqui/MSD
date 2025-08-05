using Domain.Entities.Cost.Control;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Cost
{
    public class CostItemConfiguration : IEntityTypeConfiguration<CostItem>
    {
        public void Configure(EntityTypeBuilder<CostItem> builder)
        {
            // Table name and schema
            builder.ToTable("CostItems", "Cost");

            // Primary key
            builder.HasKey(c => c.Id);

            // Indexes
            builder.HasIndex(c => c.ProjectId);
            builder.HasIndex(c => c.WBSElementId);
            builder.HasIndex(c => c.ControlAccountId);
            builder.HasIndex(c => c.CBSId);
            builder.HasIndex(c => c.ItemCode);
            builder.HasIndex(c => c.Type);
            builder.HasIndex(c => c.Category);
            builder.HasIndex(c => c.Status);
            builder.HasIndex(c => c.IsDeleted);
            builder.HasIndex(c => new { c.ProjectId, c.ItemCode }).IsUnique();
            builder.HasIndex(c => new { c.ControlAccountId, c.Type });
            builder.HasIndex(c => new { c.WBSElementId, c.Category });

            // Basic Information
            builder.Property(c => c.ItemCode)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.Description)
                .IsRequired()
                .HasMaxLength(500);

            // Cost Classification
            builder.Property(c => c.Type)
                .IsRequired();

            builder.Property(c => c.Category)
                .IsRequired();

            builder.Property(c => c.AccountCode)
                .HasMaxLength(50);

            builder.Property(c => c.CostCenter)
                .HasMaxLength(50);

            // Financial Information
            builder.Property(c => c.PlannedCost)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(c => c.ActualCost)
                .IsRequired()
                .HasPrecision(18, 2)
                .HasDefaultValue(0);

            builder.Property(c => c.CommittedCost)
                .IsRequired()
                .HasPrecision(18, 2)
                .HasDefaultValue(0);

            builder.Property(c => c.ForecastCost)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(c => c.Currency)
                .IsRequired()
                .HasMaxLength(3);

            builder.Property(c => c.ExchangeRate)
                .IsRequired()
                .HasPrecision(12, 6)
                .HasDefaultValue(1.0m);

            // Reference Information
            builder.Property(c => c.ReferenceType)
                .HasMaxLength(50);

            builder.Property(c => c.ReferenceNumber)
                .HasMaxLength(100);

            builder.Property(c => c.VendorId)
                .HasMaxLength(100);

            // Status
            builder.Property(c => c.Status)
                .IsRequired();

            builder.Property(c => c.IsApproved)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(c => c.ApprovedBy)
                .HasMaxLength(256);

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
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.WBSElement)
                .WithMany()
                .HasForeignKey(c => c.WBSElementId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.ControlAccount)
                .WithMany(ca => ca.CostItems)
                .HasForeignKey(c => c.ControlAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.CBS)
                .WithMany(cbs => cbs.CostItems)
                .HasForeignKey(c => c.CBSId)
                .OnDelete(DeleteBehavior.Restrict);

            // Calculated properties (ignored)
            builder.Ignore(c => c.Variance);
            builder.Ignore(c => c.CostVariancePercentage);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_CostItems_Costs", 
                    "[PlannedCost] >= 0 AND [ActualCost] >= 0 AND [CommittedCost] >= 0 AND [ForecastCost] >= 0");
                t.HasCheckConstraint("CK_CostItems_ExchangeRate", "[ExchangeRate] > 0");
            });

            // Global query filter for soft delete
            builder.HasQueryFilter(c => !c.IsDeleted);
        }
    }
}