using Domain.Entities.WBS;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Projects
{
    public class PlanningPackageConfiguration : IEntityTypeConfiguration<PlanningPackage>
    {
        public void Configure(EntityTypeBuilder<PlanningPackage> builder)
        {
            // Table name and schema
            builder.ToTable("PlanningPackages", "Projects");

            // Primary key
            builder.HasKey(pp => pp.Id);

            // Indexes
            builder.HasIndex(pp => pp.Code).IsUnique();
            builder.HasIndex(pp => pp.ControlAccountId);
            builder.HasIndex(pp => pp.ProjectId);
            builder.HasIndex(pp => pp.PhaseId);
            builder.HasIndex(pp => pp.Status);
            builder.HasIndex(pp => pp.Priority);
            builder.HasIndex(pp => pp.PlannedConversionDate);
            builder.HasIndex(pp => pp.IsConverted);
            builder.HasIndex(pp => pp.IsDeleted);
            builder.HasIndex(pp => pp.IsActive);
            builder.HasIndex(pp => new { pp.ProjectId, pp.Code }).IsUnique();
            builder.HasIndex(pp => new { pp.Status, pp.PlannedConversionDate });
            builder.HasIndex(pp => new { pp.ControlAccountId, pp.IsConverted });

            // Properties
            builder.Property(pp => pp.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(pp => pp.Name)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(pp => pp.Description)
                .HasMaxLength(4000);

            // Planning Information
            builder.Property(pp => pp.PlannedStartDate)
                .IsRequired();

            builder.Property(pp => pp.PlannedEndDate)
                .IsRequired();

            builder.Property(pp => pp.EstimatedBudget)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(pp => pp.EstimatedHours)
                .IsRequired()
                .HasPrecision(10, 2)
                .HasDefaultValue(0);

            // Conversion Information
            builder.Property(pp => pp.PlannedConversionDate)
                .IsRequired();

            builder.Property(pp => pp.IsConverted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(pp => pp.ConversionNotes)
                .HasMaxLength(4000);

            builder.Property(pp => pp.ConvertedBy)
                .HasMaxLength(256);

            // Status
            builder.Property(pp => pp.Status)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(pp => pp.Priority)
                .IsRequired()
                .HasDefaultValue(99);

            // Soft Delete
            builder.Property(pp => pp.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(pp => pp.DeletedBy)
                .HasMaxLength(256);

            // Activatable
            builder.Property(pp => pp.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Audit properties
            builder.Property(pp => pp.CreatedAt)
                .IsRequired();

            builder.Property(pp => pp.CreatedBy)
                .HasMaxLength(256);

            builder.Property(pp => pp.UpdatedAt);

            builder.Property(pp => pp.UpdatedBy)
                .HasMaxLength(256);

            // Relationships
            builder.HasOne(pp => pp.ControlAccount)
                .WithMany(ca => ca.PlanningPackages)
                .HasForeignKey(pp => pp.ControlAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(pp => pp.Project)
                .WithMany(p => p.PlanningPackages)
                .HasForeignKey(pp => pp.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pp => pp.Phase)
                .WithMany(ph => ph.PlanningPackages)
                .HasForeignKey(pp => pp.PhaseId)
                .OnDelete(DeleteBehavior.Restrict);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_PlanningPackages_Dates", 
                    "[PlannedEndDate] > [PlannedStartDate]");
                t.HasCheckConstraint("CK_PlanningPackages_ConversionDate", 
                    "[PlannedConversionDate] <= [PlannedStartDate]");
                t.HasCheckConstraint("CK_PlanningPackages_EstimatedBudget", 
                    "[EstimatedBudget] >= 0");
                t.HasCheckConstraint("CK_PlanningPackages_EstimatedHours", 
                    "[EstimatedHours] >= 0");
                t.HasCheckConstraint("CK_PlanningPackages_Priority", 
                    "[Priority] >= 1 AND [Priority] <= 99");
            });

            // Global query filter for soft delete
            builder.HasQueryFilter(pp => !pp.IsDeleted);
        }
    }
}