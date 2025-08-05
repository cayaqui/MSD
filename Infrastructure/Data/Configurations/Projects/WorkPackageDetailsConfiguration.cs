using Domain.Entities.WBS;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Projects
{
    public class WorkPackageDetailsConfiguration : IEntityTypeConfiguration<WorkPackageDetails>
    {
        public void Configure(EntityTypeBuilder<WorkPackageDetails> builder)
        {
            // Table name and schema
            builder.ToTable("WorkPackageDetails", "Projects");

            // Primary key
            builder.HasKey(wpd => wpd.Id);

            // Indexes
            builder.HasIndex(wpd => wpd.WBSElementId).IsUnique();
            builder.HasIndex(wpd => wpd.ResponsibleUserId);
            builder.HasIndex(wpd => wpd.PrimaryDisciplineId);
            builder.HasIndex(wpd => wpd.Status);
            builder.HasIndex(wpd => wpd.PlannedStartDate);
            builder.HasIndex(wpd => wpd.PlannedEndDate);
            builder.HasIndex(wpd => wpd.IsBaselined);
            builder.HasIndex(wpd => wpd.IsCriticalPath);
            builder.HasIndex(wpd => new { wpd.Status, wpd.PlannedStartDate });
            builder.HasIndex(wpd => new { wpd.ResponsibleUserId, wpd.Status });

            // Schedule Properties
            builder.Property(wpd => wpd.PlannedStartDate)
                .IsRequired();

            builder.Property(wpd => wpd.PlannedEndDate)
                .IsRequired();

            builder.Property(wpd => wpd.PlannedDuration)
                .IsRequired();

            builder.Property(wpd => wpd.TotalFloat)
                .HasPrecision(10, 2);

            builder.Property(wpd => wpd.FreeFloat)
                .HasPrecision(10, 2);

            builder.Property(wpd => wpd.IsCriticalPath)
                .IsRequired()
                .HasDefaultValue(false);

            // Budget Properties
            builder.Property(wpd => wpd.Budget)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(wpd => wpd.ActualCost)
                .IsRequired()
                .HasPrecision(18, 2)
                .HasDefaultValue(0);

            builder.Property(wpd => wpd.CommittedCost)
                .IsRequired()
                .HasPrecision(18, 2)
                .HasDefaultValue(0);

            builder.Property(wpd => wpd.ForecastCost)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(wpd => wpd.Currency)
                .IsRequired()
                .HasMaxLength(3)
                .HasDefaultValue("USD");

            // Progress Properties
            builder.Property(wpd => wpd.ProgressPercentage)
                .IsRequired()
                .HasPrecision(5, 2)
                .HasDefaultValue(0);

            builder.Property(wpd => wpd.PhysicalProgressPercentage)
                .IsRequired()
                .HasPrecision(5, 2)
                .HasDefaultValue(0);

            builder.Property(wpd => wpd.ProgressMethod)
                .IsRequired();

            builder.Property(wpd => wpd.Status)
                .IsRequired();

            builder.Property(wpd => wpd.WeightFactor)
                .HasPrecision(10, 4);

            // Performance Properties
            builder.Property(wpd => wpd.CPI)
                .HasPrecision(5, 2);

            builder.Property(wpd => wpd.SPI)
                .HasPrecision(5, 2);

            builder.Property(wpd => wpd.EarnedValue)
                .IsRequired()
                .HasPrecision(18, 2)
                .HasDefaultValue(0);

            builder.Property(wpd => wpd.PlannedValue)
                .IsRequired()
                .HasPrecision(18, 2)
                .HasDefaultValue(0);

            // Control Properties
            builder.Property(wpd => wpd.IsBaselined)
                .IsRequired()
                .HasDefaultValue(false);

            // Tags
            builder.Property(wpd => wpd.Tags)
                .HasMaxLength(4000);

            // Audit properties
            builder.Property(wpd => wpd.CreatedAt)
                .IsRequired();

            builder.Property(wpd => wpd.CreatedBy)
                .HasMaxLength(256);

            builder.Property(wpd => wpd.UpdatedAt);

            builder.Property(wpd => wpd.UpdatedBy)
                .HasMaxLength(256);

            // Relationships
            builder.HasOne(wpd => wpd.ResponsibleUser)
                .WithMany()
                .HasForeignKey(wpd => wpd.ResponsibleUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(wpd => wpd.PrimaryDiscipline)
                .WithMany()
                .HasForeignKey(wpd => wpd.PrimaryDisciplineId)
                .OnDelete(DeleteBehavior.Restrict);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_WorkPackageDetails_Dates", 
                    "[PlannedEndDate] > [PlannedStartDate]");
                t.HasCheckConstraint("CK_WorkPackageDetails_Progress", 
                    "[ProgressPercentage] >= 0 AND [ProgressPercentage] <= 100");
                t.HasCheckConstraint("CK_WorkPackageDetails_PhysicalProgress", 
                    "[PhysicalProgressPercentage] >= 0 AND [PhysicalProgressPercentage] <= 100");
                t.HasCheckConstraint("CK_WorkPackageDetails_Budget", 
                    "[Budget] >= 0");
                t.HasCheckConstraint("CK_WorkPackageDetails_Costs", 
                    "[ActualCost] >= 0 AND [CommittedCost] >= 0 AND [ForecastCost] >= 0");
                t.HasCheckConstraint("CK_WorkPackageDetails_Duration", 
                    "[PlannedDuration] > 0");
            });
        }
    }
}