using Domain.Entities.Progress;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Projects
{
    public class ScheduleVersionConfiguration : IEntityTypeConfiguration<ScheduleVersion>
    {
        public void Configure(EntityTypeBuilder<ScheduleVersion> builder)
        {
            // Table name and schema
            builder.ToTable("ScheduleVersions", "Projects");

            // Primary key
            builder.HasKey(s => s.Id);

            // Indexes
            builder.HasIndex(s => s.ProjectId);
            builder.HasIndex(s => s.Version);
            builder.HasIndex(s => s.Status);
            builder.HasIndex(s => s.IsBaseline);
            builder.HasIndex(s => s.DataDate);
            builder.HasIndex(s => new { s.ProjectId, s.Version }).IsUnique();
            builder.HasIndex(s => new { s.ProjectId, s.IsBaseline });

            // Basic Information
            builder.Property(s => s.Version)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(s => s.Description)
                .HasMaxLength(1000);

            // Status
            builder.Property(s => s.Status)
                .IsRequired();

            builder.Property(s => s.IsBaseline)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(s => s.BaselineDate);

            // Schedule Dates
            builder.Property(s => s.PlannedStartDate)
                .IsRequired();

            builder.Property(s => s.PlannedEndDate)
                .IsRequired();

            // Computed property - ignore
            builder.Ignore(s => s.TotalDuration);

            // Statistics
            builder.Property(s => s.TotalActivities)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(s => s.CriticalActivities)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(s => s.TotalFloat)
                .IsRequired()
                .HasPrecision(18, 2)
                .HasDefaultValue(0);

            builder.Property(s => s.DataDate);

            // Approval
            builder.Property(s => s.SubmittedDate);

            builder.Property(s => s.SubmittedBy)
                .HasMaxLength(256);

            builder.Property(s => s.ApprovalDate);

            builder.Property(s => s.ApprovedBy)
                .HasMaxLength(256);

            builder.Property(s => s.ApprovalComments)
                .HasMaxLength(1000);

            // Import Information
            builder.Property(s => s.SourceSystem)
                .HasMaxLength(100);

            builder.Property(s => s.ImportDate);

            builder.Property(s => s.ImportedBy)
                .HasMaxLength(256);

            // Audit properties
            builder.Property(s => s.CreatedAt)
                .IsRequired();

            builder.Property(s => s.CreatedBy)
                .HasMaxLength(256);

            builder.Property(s => s.UpdatedAt);

            builder.Property(s => s.UpdatedBy)
                .HasMaxLength(256);

            // Relationships
            builder.HasOne(s => s.Project)
                .WithMany()
                .HasForeignKey(s => s.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_ScheduleVersions_Dates", 
                    "[PlannedEndDate] >= [PlannedStartDate]");
                t.HasCheckConstraint("CK_ScheduleVersions_TotalActivities", 
                    "[TotalActivities] >= 0");
                t.HasCheckConstraint("CK_ScheduleVersions_CriticalActivities", 
                    "[CriticalActivities] >= 0 AND [CriticalActivities] <= [TotalActivities]");
                t.HasCheckConstraint("CK_ScheduleVersions_TotalFloat", 
                    "[TotalFloat] >= 0");
            });
        }
    }
}