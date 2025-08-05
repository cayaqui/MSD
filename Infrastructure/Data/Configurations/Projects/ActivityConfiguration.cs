using Domain.Entities.Progress;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Projects
{
    public class ActivityConfiguration : IEntityTypeConfiguration<Activity>
    {
        public void Configure(EntityTypeBuilder<Activity> builder)
        {
            // Table name and schema
            builder.ToTable("Activities", "Projects");

            // Primary key
            builder.HasKey(a => a.Id);

            // Indexes
            builder.HasIndex(a => a.WBSElementId);
            builder.HasIndex(a => a.ActivityCode).IsUnique();
            builder.HasIndex(a => a.Status);
            builder.HasIndex(a => a.PlannedStartDate);
            builder.HasIndex(a => a.PlannedEndDate);
            builder.HasIndex(a => new { a.WBSElementId, a.ActivityCode }).IsUnique();
            builder.HasIndex(a => new { a.Status, a.PlannedStartDate });
            builder.HasIndex(a => new { a.WBSElementId, a.Status });

            // Properties
            builder.Property(a => a.ActivityCode)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(a => a.Description)
                .HasMaxLength(4000);

            // Schedule Information
            builder.Property(a => a.PlannedStartDate)
                .IsRequired();

            builder.Property(a => a.PlannedEndDate)
                .IsRequired();

            builder.Property(a => a.DurationDays)
                .IsRequired();

            // Progress Information
            builder.Property(a => a.ProgressPercentage)
                .IsRequired()
                .HasPrecision(5, 2)
                .HasDefaultValue(0);

            builder.Property(a => a.Status)
                .IsRequired();

            // Resources
            builder.Property(a => a.PlannedHours)
                .IsRequired()
                .HasPrecision(10, 2)
                .HasDefaultValue(0);

            builder.Property(a => a.ActualHours)
                .IsRequired()
                .HasPrecision(10, 2)
                .HasDefaultValue(0);

            builder.Property(a => a.ResourceRate)
                .HasPrecision(10, 2);

            // Dependencies (JSON)
            builder.Property(a => a.PredecessorActivities)
                .HasMaxLength(4000);

            builder.Property(a => a.SuccessorActivities)
                .HasMaxLength(4000);

            // Audit properties
            builder.Property(a => a.CreatedAt)
                .IsRequired();

            builder.Property(a => a.CreatedBy)
                .HasMaxLength(256);

            builder.Property(a => a.UpdatedAt);

            builder.Property(a => a.UpdatedBy)
                .HasMaxLength(256);

            // Relationships
            builder.HasOne(a => a.WBSElement)
                .WithMany()
                .HasForeignKey(a => a.WBSElementId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(a => a.Resources)
                .WithOne(r => r.Activity)
                .HasForeignKey(r => r.ActivityId)
                .OnDelete(DeleteBehavior.Cascade);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Activities_Dates", 
                    "[PlannedEndDate] > [PlannedStartDate]");
                t.HasCheckConstraint("CK_Activities_Progress", 
                    "[ProgressPercentage] >= 0 AND [ProgressPercentage] <= 100");
                t.HasCheckConstraint("CK_Activities_Hours", 
                    "[PlannedHours] >= 0 AND [ActualHours] >= 0");
                t.HasCheckConstraint("CK_Activities_DurationDays", 
                    "[DurationDays] > 0");
            });
        }
    }
}