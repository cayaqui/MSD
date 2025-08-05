using Domain.Entities.Configuration.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Configuration
{
    public class ProjectTypeConfiguration : IEntityTypeConfiguration<ProjectType>
    {
        public void Configure(EntityTypeBuilder<ProjectType> builder)
        {
            // Table name and schema
            builder.ToTable("ProjectTypes", "Configuration");

            // Primary key
            builder.HasKey(pt => pt.Id);

            // Indexes
            builder.HasIndex(pt => pt.Code).IsUnique();
            builder.HasIndex(pt => pt.IsActive);
            builder.HasIndex(pt => pt.IsDeleted);
            builder.HasIndex(pt => new { pt.IsActive, pt.IsDeleted });

            // Properties
            builder.Property(pt => pt.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(pt => pt.Name)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(pt => pt.Description)
                .HasMaxLength(500);

            builder.Property(pt => pt.Icon)
                .HasMaxLength(50);

            builder.Property(pt => pt.Color)
                .HasMaxLength(20);

            // Module requirements
            builder.Property(pt => pt.RequiresWBS)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(pt => pt.RequiresCBS)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(pt => pt.RequiresOBS)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(pt => pt.RequiresSchedule)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(pt => pt.RequiresBudget)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(pt => pt.RequiresRiskManagement)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(pt => pt.RequiresDocumentControl)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(pt => pt.RequiresChangeManagement)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(pt => pt.RequiresQualityControl)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(pt => pt.RequiresHSE)
                .IsRequired()
                .HasDefaultValue(false);

            // Default settings
            builder.Property(pt => pt.DefaultDurationUnit)
                .IsRequired()
                .HasDefaultValue(1);

            builder.Property(pt => pt.DefaultCurrency)
                .IsRequired()
                .HasMaxLength(3)
                .HasDefaultValue("USD");

            builder.Property(pt => pt.DefaultProgressMethod)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("Physical");

            builder.Property(pt => pt.DefaultContingencyPercentage)
                .HasPrecision(5, 2)
                .HasDefaultValue(10);

            builder.Property(pt => pt.DefaultReportingPeriod)
                .IsRequired()
                .HasDefaultValue(7);

            // JSON fields
            builder.Property(pt => pt.ApprovalLevelsJson)
                .HasColumnType("nvarchar(max)");

            builder.Property(pt => pt.WorkflowStagesJson)
                .HasColumnType("nvarchar(max)");

            // Status
            builder.Property(pt => pt.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Soft delete
            builder.Property(pt => pt.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(pt => pt.DeletedBy)
                .HasMaxLength(256);

            // Audit properties
            builder.Property(pt => pt.CreatedAt)
                .IsRequired();

            builder.Property(pt => pt.CreatedBy)
                .HasMaxLength(256);

            builder.Property(pt => pt.UpdatedAt);

            builder.Property(pt => pt.UpdatedBy)
                .HasMaxLength(256);

            // Navigation properties
            builder.HasMany(pt => pt.Projects)
                .WithOne()
                .HasForeignKey("ProjectTypeId")
                .OnDelete(DeleteBehavior.Restrict);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_ProjectTypes_DefaultDurationUnit", 
                    "[DefaultDurationUnit] >= 1 AND [DefaultDurationUnit] <= 4");
                t.HasCheckConstraint("CK_ProjectTypes_DefaultContingencyPercentage", 
                    "[DefaultContingencyPercentage] >= 0 AND [DefaultContingencyPercentage] <= 100");
                t.HasCheckConstraint("CK_ProjectTypes_DefaultReportingPeriod", 
                    "[DefaultReportingPeriod] >= 1");
            });

            // Global query filter for soft delete
            builder.HasQueryFilter(pt => !pt.IsDeleted);
        }
    }
}