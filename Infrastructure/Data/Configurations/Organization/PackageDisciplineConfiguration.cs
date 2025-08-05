using Domain.Entities.Organization.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Organization;

/// <summary>
/// Entity configuration for PackageDiscipline
/// </summary>
public class PackageDisciplineConfiguration : IEntityTypeConfiguration<PackageDiscipline>
{
    public void Configure(EntityTypeBuilder<PackageDiscipline> builder)
    {
        // Table name and schema with check constraints
        builder.ToTable("PackageDisciplines", "Organization", t =>
        {
            t.HasCheckConstraint("CK_PackageDisciplines_EstimatedHours",
                "[EstimatedHours] >= 0");
            t.HasCheckConstraint("CK_PackageDisciplines_ActualHours",
                "[ActualHours] >= 0");
            t.HasCheckConstraint("CK_PackageDisciplines_EstimatedCost",
                "[EstimatedCost] >= 0");
            t.HasCheckConstraint("CK_PackageDisciplines_ActualCost",
                "[ActualCost] >= 0");
            t.HasCheckConstraint("CK_PackageDisciplines_ProgressPercentage",
                "[ProgressPercentage] >= 0 AND [ProgressPercentage] <= 100");
        });

        // Primary key
        builder.HasKey(pd => pd.Id);

        // Indexes
        builder.HasIndex(pd => new { pd.PackageId, pd.DisciplineId }).IsUnique();
        builder.HasIndex(pd => pd.PackageId);
        builder.HasIndex(pd => pd.DisciplineId);
        builder.HasIndex(pd => pd.LeadEngineerId);
        builder.HasIndex(pd => pd.IsLeadDiscipline);

        // Properties
        builder.Property(pd => pd.EstimatedHours)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(pd => pd.ActualHours)
            .HasPrecision(10, 2)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(pd => pd.IsLeadDiscipline)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(pd => pd.Notes)
            .HasMaxLength(1000);

        builder.Property(pd => pd.ProgressPercentage)
            .HasPrecision(5, 2)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(pd => pd.EstimatedCost)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(pd => pd.ActualCost)
            .HasPrecision(18, 2)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(pd => pd.Currency)
            .IsRequired()
            .HasMaxLength(3)
            .HasDefaultValue("USD");

        // Audit properties
        builder.Property(pd => pd.CreatedAt)
            .IsRequired();

        builder.Property(pd => pd.CreatedBy)
            .HasMaxLength(256);

        builder.Property(pd => pd.UpdatedAt);

        builder.Property(pd => pd.UpdatedBy)
            .HasMaxLength(256);

        // Foreign key relationships
        builder.HasOne(pd => pd.Package)
            .WithMany()
            .HasForeignKey(pd => pd.PackageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pd => pd.Discipline)
            .WithMany()
            .HasForeignKey(pd => pd.DisciplineId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(pd => pd.LeadEngineer)
            .WithMany()
            .HasForeignKey(pd => pd.LeadEngineerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}