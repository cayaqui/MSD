using Domain.Entities.Progress;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Projects
{
    public class WBSElementProgressConfiguration : IEntityTypeConfiguration<WBSElementProgress>
    {
        public void Configure(EntityTypeBuilder<WBSElementProgress> builder)
        {
            // Table name and schema
            builder.ToTable("WBSElementProgress", "Projects");

            // Primary key
            builder.HasKey(w => w.Id);

            // Indexes
            builder.HasIndex(w => w.WBSElementId);
            builder.HasIndex(w => w.ProgressDate);
            builder.HasIndex(w => w.Year);
            builder.HasIndex(w => w.Month);
            builder.HasIndex(w => w.Week);
            builder.HasIndex(w => w.IsApproved);
            builder.HasIndex(w => w.RequiresReview);
            builder.HasIndex(w => w.ReportedBy);
            builder.HasIndex(w => new { w.WBSElementId, w.ProgressDate });
            builder.HasIndex(w => new { w.WBSElementId, w.Year, w.Month });

            // Foreign Keys
            builder.Property(w => w.WBSElementId)
                .IsRequired();

            // Progress Information
            builder.Property(w => w.ProgressDate)
                .IsRequired();

            builder.Property(w => w.Year)
                .IsRequired();

            builder.Property(w => w.Month)
                .IsRequired();

            builder.Property(w => w.Week)
                .IsRequired();

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
            builder.Ignore(w => w.EstimateAtCompletion);
            builder.Ignore(w => w.ScheduleVariance);
            builder.Ignore(w => w.CostVariance);
            builder.Ignore(w => w.SPI);
            builder.Ignore(w => w.CPI);
            builder.Ignore(w => w.IsDelayed);
            builder.Ignore(w => w.IsOverBudget);
            builder.Ignore(w => w.IsBehindSchedule);
            builder.Ignore(w => w.RequiresAttention);
            builder.Ignore(w => w.ProgressEfficiency);

            // Physical Progress
            builder.Property(w => w.PhysicalProgress)
                .HasPrecision(5, 2);

            builder.Property(w => w.PhysicalProgressDescription)
                .HasMaxLength(500);

            // Cost Information
            builder.Property(w => w.PreviousActualCost)
                .IsRequired()
                .HasPrecision(18, 2)
                .HasDefaultValue(0);

            builder.Property(w => w.CurrentActualCost)
                .IsRequired()
                .HasPrecision(18, 2)
                .HasDefaultValue(0);

            builder.Property(w => w.CommittedCost)
                .IsRequired()
                .HasPrecision(18, 2)
                .HasDefaultValue(0);

            builder.Property(w => w.ForecastToComplete)
                .IsRequired()
                .HasPrecision(18, 2)
                .HasDefaultValue(0);

            // Schedule Information
            builder.Property(w => w.ActualStartDate);
            builder.Property(w => w.ForecastEndDate);
            builder.Property(w => w.DaysDelayed);

            builder.Property(w => w.DelayReason)
                .HasMaxLength(500);

            // Earned Value Metrics
            builder.Property(w => w.EarnedValue)
                .HasPrecision(18, 2);

            builder.Property(w => w.PlannedValue)
                .HasPrecision(18, 2);

            // Progress Details
            builder.Property(w => w.Comments)
                .HasMaxLength(2000);

            builder.Property(w => w.Issues)
                .HasMaxLength(2000);

            builder.Property(w => w.Risks)
                .HasMaxLength(2000);

            builder.Property(w => w.MitigationActions)
                .HasMaxLength(2000);

            // Approval Workflow
            builder.Property(w => w.IsApproved)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(w => w.ApprovalDate);

            builder.Property(w => w.ApprovedBy)
                .HasMaxLength(256);

            builder.Property(w => w.ApprovalComments)
                .HasMaxLength(1000);

            builder.Property(w => w.RequiresReview)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(w => w.ReviewReason)
                .HasMaxLength(500);

            // Reporting
            builder.Property(w => w.ReportedBy)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(w => w.ReportedAt)
                .IsRequired();

            builder.Property(w => w.VerifiedBy)
                .HasMaxLength(256);

            builder.Property(w => w.VerifiedAt);

            // Supporting Documentation
            builder.Property(w => w.PhotoReferences)
                .HasMaxLength(4000)
                .HasComment("JSON array of photo IDs");

            builder.Property(w => w.DocumentReferences)
                .HasMaxLength(4000)
                .HasComment("JSON array of document IDs");

            builder.Property(w => w.MilestoneReferences)
                .HasMaxLength(4000)
                .HasComment("JSON array of milestone codes achieved");

            // Resource Information
            builder.Property(w => w.LaborHoursUsed)
                .HasPrecision(18, 2);

            builder.Property(w => w.EquipmentHoursUsed)
                .HasPrecision(18, 2);

            builder.Property(w => w.MaterialQuantityUsed)
                .HasPrecision(18, 4);

            builder.Property(w => w.ResourceNotes)
                .HasMaxLength(1000);

            // Summary/Phase Level
            builder.Property(w => w.ChildrenCount);
            builder.Property(w => w.CompletedChildrenCount);

            builder.Property(w => w.IsRollupProgress)
                .IsRequired()
                .HasDefaultValue(false);

            // Audit properties
            builder.Property(w => w.CreatedAt)
                .IsRequired();

            builder.Property(w => w.CreatedBy)
                .HasMaxLength(256);

            builder.Property(w => w.UpdatedAt);

            builder.Property(w => w.UpdatedBy)
                .HasMaxLength(256);

            // Relationships
            builder.HasOne(w => w.WBSElement)
                .WithMany()
                .HasForeignKey(w => w.WBSElementId)
                .OnDelete(DeleteBehavior.Restrict);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_WBSElementProgress_Progress",
                    "[CurrentProgress] >= 0 AND [CurrentProgress] <= 100 AND " +
                    "[PreviousProgress] >= 0 AND [PreviousProgress] <= 100");
                t.HasCheckConstraint("CK_WBSElementProgress_PhysicalProgress",
                    "[PhysicalProgress] IS NULL OR ([PhysicalProgress] >= 0 AND [PhysicalProgress] <= 100)");
                t.HasCheckConstraint("CK_WBSElementProgress_Costs",
                    "[PreviousActualCost] >= 0 AND [CurrentActualCost] >= 0 AND " +
                    "[CommittedCost] >= 0 AND [ForecastToComplete] >= 0");
                t.HasCheckConstraint("CK_WBSElementProgress_EVM",
                    "([EarnedValue] IS NULL OR [EarnedValue] >= 0) AND " +
                    "([PlannedValue] IS NULL OR [PlannedValue] >= 0)");
                t.HasCheckConstraint("CK_WBSElementProgress_Resources",
                    "([LaborHoursUsed] IS NULL OR [LaborHoursUsed] >= 0) AND " +
                    "([EquipmentHoursUsed] IS NULL OR [EquipmentHoursUsed] >= 0) AND " +
                    "([MaterialQuantityUsed] IS NULL OR [MaterialQuantityUsed] >= 0)");
                t.HasCheckConstraint("CK_WBSElementProgress_Children",
                    "([CompletedChildrenCount] IS NULL OR [ChildrenCount] IS NULL OR " +
                    "[CompletedChildrenCount] <= [ChildrenCount])");
            });
        }
    }
}