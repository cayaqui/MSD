using Domain.Entities.Progress;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Progress
{
    public class WBSElementProgressConfiguration : IEntityTypeConfiguration<WBSElementProgress>
    {
        public void Configure(EntityTypeBuilder<WBSElementProgress> builder)
        {
            builder.ToTable("WBSElementProgress", "Progress", t=>
            {
                t.HasCheckConstraint("CK_WBSElementProgress_Progress",
                "[CurrentProgress] >= 0 AND [CurrentProgress] <= 100 AND [PreviousProgress] >= 0 AND [PreviousProgress] <= 100");
                t.HasCheckConstraint("CK_WBSElementProgress_Costs",
                "[CurrentActualCost] >= 0 AND [PreviousActualCost] >= 0 AND [CommittedCost] >= 0 AND [ForecastToComplete] >= 0");
                t.HasCheckConstraint("CK_WBSElementProgress_Resources",
                "([LaborHoursUsed] IS NULL OR [LaborHoursUsed] >= 0) AND ([EquipmentHoursUsed] IS NULL OR [EquipmentHoursUsed] >= 0) AND ([MaterialQuantityUsed] IS NULL OR [MaterialQuantityUsed] >= 0)");
            });

            // Primary Key
            builder.HasKey(p => p.Id);

            // Properties
            builder.Property(p => p.ProgressDate)
                .IsRequired();

            builder.Property(p => p.Year)
                .IsRequired();

            builder.Property(p => p.Month)
                .IsRequired();

            builder.Property(p => p.Week)
                .IsRequired();

            builder.Property(p => p.PreviousProgress)
                .HasPrecision(5, 2)
                .IsRequired();

            builder.Property(p => p.CurrentProgress)
                .HasPrecision(5, 2)
                .IsRequired();

            builder.Property(p => p.MeasurementMethod)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(p => p.PhysicalProgress)
                .HasPrecision(5, 2);

            builder.Property(p => p.PhysicalProgressDescription)
                .HasMaxLength(500);

            // Cost properties
            builder.Property(p => p.PreviousActualCost)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(p => p.CurrentActualCost)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(p => p.CommittedCost)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(p => p.ForecastToComplete)
                .HasPrecision(18, 2)
                .IsRequired();

            // Schedule properties
            builder.Property(p => p.DelayReason)
                .HasMaxLength(500);

            // EVM properties
            builder.Property(p => p.EarnedValue)
                .HasPrecision(18, 2);

            builder.Property(p => p.PlannedValue)
                .HasPrecision(18, 2);

            // Comments and text fields
            builder.Property(p => p.Comments)
                .HasMaxLength(2000);

            builder.Property(p => p.Issues)
                .HasMaxLength(2000);

            builder.Property(p => p.Risks)
                .HasMaxLength(2000);

            builder.Property(p => p.MitigationActions)
                .HasMaxLength(2000);

            // Approval fields
            builder.Property(p => p.IsApproved)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(p => p.ApprovedBy)
                .HasMaxLength(450);

            builder.Property(p => p.ApprovalComments)
                .HasMaxLength(1000);

            builder.Property(p => p.RequiresReview)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(p => p.ReviewReason)
                .HasMaxLength(500);

            // Reporting fields
            builder.Property(p => p.ReportedBy)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(p => p.ReportedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(p => p.VerifiedBy)
                .HasMaxLength(450);

            // Documentation fields (JSON)
            builder.Property(p => p.PhotoReferences)
                .HasMaxLength(2000);

            builder.Property(p => p.DocumentReferences)
                .HasMaxLength(2000);

            builder.Property(p => p.MilestoneReferences)
                .HasMaxLength(2000);

            // Resource fields
            builder.Property(p => p.LaborHoursUsed)
                .HasPrecision(10, 2);

            builder.Property(p => p.EquipmentHoursUsed)
                .HasPrecision(10, 2);

            builder.Property(p => p.MaterialQuantityUsed)
                .HasPrecision(18, 3);

            builder.Property(p => p.ResourceNotes)
                .HasMaxLength(1000);

            // Rollup information
            builder.Property(p => p.IsRollupProgress)
                .IsRequired()
                .HasDefaultValue(false);

            // Audit properties
            builder.Property(p => p.CreatedBy)
                .HasMaxLength(450);

            builder.Property(p => p.UpdatedBy)
                .HasMaxLength(450);

            // Relationships
            builder.HasOne(p => p.WBSElement)
                .WithMany()
                .HasForeignKey(p => p.WBSElementId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(p => new { p.WBSElementId, p.ProgressDate })
                .HasDatabaseName("IX_WBSElementProgress_WBSElement_Date");

            builder.HasIndex(p => new { p.WBSElementId, p.Year, p.Month })
                .HasDatabaseName("IX_WBSElementProgress_WBSElement_Period");

            builder.HasIndex(p => new { p.Year, p.Week })
                .HasDatabaseName("IX_WBSElementProgress_YearWeek");

            builder.HasIndex(p => p.ProgressDate)
                .HasDatabaseName("IX_WBSElementProgress_Date");

            builder.HasIndex(p => p.IsApproved)
                .HasDatabaseName("IX_WBSElementProgress_IsApproved");

            builder.HasIndex(p => p.RequiresReview)
                .HasDatabaseName("IX_WBSElementProgress_RequiresReview");

            builder.HasIndex(p => p.ReportedBy)
                .HasDatabaseName("IX_WBSElementProgress_ReportedBy");

            builder.HasIndex(p => new { p.CurrentProgress, p.IsApproved })
                .HasDatabaseName("IX_WBSElementProgress_Progress_Approved");

            // Unique constraint to prevent duplicate progress entries
            builder.HasIndex(p => new { p.WBSElementId, p.ProgressDate, p.IsApproved })
                .IsUnique()
                .HasFilter("[IsApproved] = 1")
                .HasDatabaseName("UX_WBSElementProgress_Approved_Progress");
        }
    }
}