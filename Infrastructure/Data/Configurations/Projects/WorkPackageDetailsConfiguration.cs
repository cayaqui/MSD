using Domain.Entities.Projects;

namespace Infrastructure.Data.Configurations.Projects;

/// <summary>
/// Entity Framework configuration for WorkPackageDetails
/// </summary>
public class WorkPackageDetailsConfiguration : IEntityTypeConfiguration<WorkPackageDetails>
{
    public void Configure(EntityTypeBuilder<WorkPackageDetails> builder)
    {
        // Table name and schema
        builder.ToTable("WorkPackageDetails", "Projects");

        // Primary Key
        builder.HasKey(e => e.Id);

        // Properties - Schedule
        builder.Property(e => e.PlannedStartDate)
            .IsRequired();

        builder.Property(e => e.PlannedEndDate)
            .IsRequired();

        builder.Property(e => e.PlannedDuration)
            .IsRequired();

        // Properties - Budget
        builder.Property(e => e.Budget)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(e => e.ActualCost)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(e => e.CommittedCost)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(e => e.ForecastCost)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(e => e.Currency)
            .IsRequired()
            .HasMaxLength(3)
            .HasDefaultValue("USD");

        // Properties - Progress
        builder.Property(e => e.ProgressPercentage)
            .IsRequired()
            .HasPrecision(5, 2);

        builder.Property(e => e.PhysicalProgressPercentage)
            .IsRequired()
            .HasPrecision(5, 2);

        builder.Property(e => e.ProgressMethod)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(e => e.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(e => e.WeightFactor)
            .HasPrecision(5, 2);

        // Properties - Performance
        builder.Property(e => e.CPI)
            .HasPrecision(5, 2);

        builder.Property(e => e.SPI)
            .HasPrecision(5, 2);

        builder.Property(e => e.EarnedValue)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(e => e.PlannedValue)
            .IsRequired()
            .HasPrecision(18, 2);

        // Properties - Float
        builder.Property(e => e.TotalFloat)
            .IsRequired()
            .HasPrecision(10, 2);

        builder.Property(e => e.FreeFloat)
            .IsRequired()
            .HasPrecision(10, 2);

        builder.Property(e => e.IsCriticalPath)
            .IsRequired()
            .HasDefaultValue(false);

        // Properties - Control
        builder.Property(e => e.IsBaselined)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.Tags)
            .HasMaxLength(500);

        // Audit fields
        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.CreatedBy)
            .HasMaxLength(100);

        builder.Property(e => e.UpdatedBy)
            .HasMaxLength(100);

        // Indexes
        builder.HasIndex(e => e.WBSElementId)
            .IsUnique();

        builder.HasIndex(e => e.ResponsibleUserId);

        builder.HasIndex(e => e.PrimaryDisciplineId);

        builder.HasIndex(e => e.Status);

        builder.HasIndex(e => e.PlannedStartDate);

        builder.HasIndex(e => e.PlannedEndDate);

        // Relationships
        builder.HasOne(e => e.ResponsibleUser)
            .WithMany()
            .HasForeignKey(e => e.ResponsibleUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.PrimaryDiscipline)
            .WithMany()
            .HasForeignKey(e => e.PrimaryDisciplineId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
