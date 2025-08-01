using Domain.Entities.Cost;
using Domain.Entities.Projects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Cost;

/// <summary>
/// Entity configuration for PlanningPackage
/// </summary>
public class PlanningPackageConfiguration : IEntityTypeConfiguration<PlanningPackage>
{
    public void Configure(EntityTypeBuilder<PlanningPackage> builder)
    {
        // Table name and schema
        builder.ToTable("PlanningPackages", "Cost");

        // Primary key
        builder.HasKey(pp => pp.Id);

        // Indexes
        builder.HasIndex(pp => new { pp.ControlAccountId, pp.Code }).IsUnique();
        builder.HasIndex(pp => pp.ControlAccountId);
        builder.HasIndex(pp => pp.ProjectId);
        builder.HasIndex(pp => pp.IsDeleted);
        builder.HasIndex(pp => new { pp.IsConverted, pp.PlannedStartDate });

        // Properties
        builder.Property(pp => pp.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(pp => pp.Name)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(pp => pp.Description)
            .HasMaxLength(1000);

        builder.Property(pp => pp.PlannedStartDate)
            .IsRequired();

        builder.Property(pp => pp.PlannedEndDate)
            .IsRequired();

        builder.Property(pp => pp.EstimatedBudget)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(pp => pp.ConversionDate);


        builder.Property(pp => pp.ConvertedBy)
            .HasMaxLength(256);

        builder.Property(pp => pp.ConversionNotes)
            .HasMaxLength(1000);

        // Audit properties
        builder.Property(pp => pp.CreatedAt)
            .IsRequired();

        builder.Property(pp => pp.CreatedBy)
            .HasMaxLength(256);

        builder.Property(pp => pp.UpdatedAt);

        builder.Property(pp => pp.UpdatedBy)
            .HasMaxLength(256);

        // Soft delete properties
        builder.Property(pp => pp.DeletedAt);

        builder.Property(pp => pp.DeletedBy)
            .HasMaxLength(256);

        // Foreign key relationships
        builder.HasOne(pp => pp.ControlAccount)
            .WithMany(ca => ca.PlanningPackages)
            .HasForeignKey(pp => pp.ControlAccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pp => pp.Project)
            .WithMany()
            .HasForeignKey(pp => pp.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // Global query filter for soft delete
        builder.HasQueryFilter(pp => !pp.IsDeleted);
    }
}