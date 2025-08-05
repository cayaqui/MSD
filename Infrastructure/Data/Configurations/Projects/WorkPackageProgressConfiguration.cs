using Domain.Entities.Progress;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Projects
{
    public class WorkPackageProgressConfiguration : IEntityTypeConfiguration<WorkPackageProgress>
    {
        public void Configure(EntityTypeBuilder<WorkPackageProgress> builder)
        {
            // Table name and schema
            builder.ToTable("WorkPackageProgress", "Projects");

            // Primary key
            builder.HasKey(w => w.Id);

            // Indexes
            builder.HasIndex(w => w.WorkPackageId);
            builder.HasIndex(w => w.ProgressDate);
            builder.HasIndex(w => w.Year);
            builder.HasIndex(w => w.ProgressPeriod);
            builder.HasIndex(w => w.IsApproved);
            builder.HasIndex(w => new { w.WorkPackageId, w.ProgressDate });
            builder.HasIndex(w => new { w.WorkPackageId, w.Year, w.ProgressPeriod });

            // Basic Information
            builder.Property(w => w.WorkPackageId)
                .IsRequired();

            builder.Property(w => w.ProgressDate)
                .IsRequired();

            builder.Property(w => w.ProgressPeriod)
                .IsRequired();

            builder.Property(w => w.Year)
                .IsRequired();

            // Progress Information
            builder.Property(w => w.PreviousProgress)
                .IsRequired()
                .HasPrecision(5, 2);

            builder.Property(w => w.CurrentProgress)
                .IsRequired()
                .HasPrecision(5, 2);

            builder.Property(w => w.MeasurementMethod)
                .IsRequired()
                .HasMaxLength(50);

            // Calculated properties - ignore
            builder.Ignore(w => w.ProgressDelta);
            builder.Ignore(w => w.CostDelta);
            builder.Ignore(w => w.ScheduleVariance);

            // Cost Information
            builder.Property(w => w.PreviousActualCost)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(w => w.CurrentActualCost)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(w => w.CommittedCost)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(w => w.ForecastCost)
                .IsRequired()
                .HasPrecision(18, 2);

            // Schedule Information
            builder.Property(w => w.ActualStartDate);
            builder.Property(w => w.ActualEndDate);
            builder.Property(w => w.DaysDelayed);

            // Earned Value
            builder.Property(w => w.EarnedValue)
                .HasPrecision(18, 2);

            builder.Property(w => w.PlannedValue)
                .HasPrecision(18, 2);

            // Details
            builder.Property(w => w.Comments)
                .HasMaxLength(2000);

            builder.Property(w => w.Issues)
                .HasMaxLength(2000);

            builder.Property(w => w.Risks)
                .HasMaxLength(2000);

            builder.Property(w => w.IsApproved)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(w => w.ApprovalDate);

            builder.Property(w => w.ApprovedBy)
                .HasMaxLength(256);

            // Supporting Documentation
            builder.Property(w => w.PhotoReferences)
                .HasMaxLength(4000)
                .HasComment("JSON array of photo IDs");

            builder.Property(w => w.DocumentReferences)
                .HasMaxLength(4000)
                .HasComment("JSON array of document IDs");

            // Audit properties
            builder.Property(w => w.CreatedAt)
                .IsRequired();

            builder.Property(w => w.CreatedBy)
                .HasMaxLength(256);

            builder.Property(w => w.UpdatedAt);

            builder.Property(w => w.UpdatedBy)
                .HasMaxLength(256);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_WorkPackageProgress_Progress",
                    "[CurrentProgress] >= 0 AND [CurrentProgress] <= 100 AND " +
                    "[PreviousProgress] >= 0 AND [PreviousProgress] <= 100 AND " +
                    "[CurrentProgress] >= [PreviousProgress]");
                t.HasCheckConstraint("CK_WorkPackageProgress_Costs",
                    "[PreviousActualCost] >= 0 AND [CurrentActualCost] >= 0 AND " +
                    "[CommittedCost] >= 0 AND [ForecastCost] >= 0");
                t.HasCheckConstraint("CK_WorkPackageProgress_EVM",
                    "([EarnedValue] IS NULL OR [EarnedValue] >= 0) AND " +
                    "([PlannedValue] IS NULL OR [PlannedValue] >= 0)");
                t.HasCheckConstraint("CK_WorkPackageProgress_DaysDelayed",
                    "[DaysDelayed] IS NULL OR [DaysDelayed] >= 0");
            });
        }
    }
}