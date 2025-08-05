using Domain.Entities.Progress;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Projects
{
    public class ResourceConfiguration : IEntityTypeConfiguration<Resource>
    {
        public void Configure(EntityTypeBuilder<Resource> builder)
        {
            // Table name and schema
            builder.ToTable("Resources", "Projects");

            // Primary key
            builder.HasKey(r => r.Id);

            // Indexes
            builder.HasIndex(r => r.ResourceCode).IsUnique();
            builder.HasIndex(r => r.ActivityId);
            builder.HasIndex(r => r.UserId);
            builder.HasIndex(r => r.ContractorId);
            builder.HasIndex(r => r.EquipmentId);
            builder.HasIndex(r => r.Type);
            builder.HasIndex(r => r.IsActive);
            builder.HasIndex(r => r.IsOverAllocated);
            builder.HasIndex(r => r.IsDeleted);
            builder.HasIndex(r => new { r.ActivityId, r.Type });
            builder.HasIndex(r => new { r.UserId, r.IsActive });

            // Properties
            builder.Property(r => r.ResourceCode)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(r => r.ResourceName)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(r => r.Description)
                .HasMaxLength(4000);

            builder.Property(r => r.Type)
                .IsRequired();

            // Quantity and Cost Information
            builder.Property(r => r.PlannedQuantity)
                .IsRequired()
                .HasPrecision(18, 4)
                .HasDefaultValue(0);

            builder.Property(r => r.ActualQuantity)
                .IsRequired()
                .HasPrecision(18, 4)
                .HasDefaultValue(0);

            builder.Property(r => r.UnitOfMeasure)
                .HasMaxLength(50);

            builder.Property(r => r.UnitRate)
                .HasPrecision(18, 4);

            builder.Property(r => r.Currency)
                .IsRequired()
                .HasMaxLength(3)
                .HasDefaultValue("USD");

            // Time Information
            builder.Property(r => r.PlannedHours)
                .IsRequired()
                .HasPrecision(10, 2)
                .HasDefaultValue(0);

            builder.Property(r => r.ActualHours)
                .IsRequired()
                .HasPrecision(10, 2)
                .HasDefaultValue(0);

            // Status
            builder.Property(r => r.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(r => r.IsOverAllocated)
                .IsRequired()
                .HasDefaultValue(false);

            // Soft Delete
            builder.Property(r => r.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(r => r.DeletedBy)
                .HasMaxLength(256);

            // Audit properties
            builder.Property(r => r.CreatedAt)
                .IsRequired();

            builder.Property(r => r.CreatedBy)
                .HasMaxLength(256);

            builder.Property(r => r.UpdatedAt);

            builder.Property(r => r.UpdatedBy)
                .HasMaxLength(256);

            // Relationships
            builder.HasOne(r => r.Activity)
                .WithMany(a => a.Resources)
                .HasForeignKey(r => r.ActivityId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.Contractor)
                .WithMany()
                .HasForeignKey(r => r.ContractorId)
                .OnDelete(DeleteBehavior.Restrict);

            // TODO: Uncomment when Equipment navigation property is added to Resource entity
            // builder.HasOne(r => r.Equipment)
            //     .WithMany()
            //     .HasForeignKey(r => r.EquipmentId)
            //     .OnDelete(DeleteBehavior.Restrict);

            // Calculated properties (ignored)
            builder.Ignore(r => r.PlannedCost);
            builder.Ignore(r => r.ActualCost);
            builder.Ignore(r => r.CostVariance);
            builder.Ignore(r => r.QuantityVariance);
            builder.Ignore(r => r.UtilizationRate);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Resources_PlannedQuantity", 
                    "[PlannedQuantity] >= 0");
                t.HasCheckConstraint("CK_Resources_ActualQuantity", 
                    "[ActualQuantity] >= 0");
                t.HasCheckConstraint("CK_Resources_PlannedHours", 
                    "[PlannedHours] >= 0");
                t.HasCheckConstraint("CK_Resources_ActualHours", 
                    "[ActualHours] >= 0");
                t.HasCheckConstraint("CK_Resources_UnitRate", 
                    "[UnitRate] IS NULL OR [UnitRate] >= 0");
                t.HasCheckConstraint("CK_Resources_Dates", 
                    "[EndDate] IS NULL OR [StartDate] IS NULL OR [EndDate] >= [StartDate]");
            });

            // Global query filter for soft delete
            builder.HasQueryFilter(r => !r.IsDeleted);
        }
    }
}