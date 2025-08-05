using Domain.Entities.Organization.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Organization;

/// <summary>
/// Entity configuration for Package
/// </summary>
public class PackageConfiguration : IEntityTypeConfiguration<Package>
{
    public void Configure(EntityTypeBuilder<Package> builder)
    {
        // Table name and schema with check constraints
        builder.ToTable("Packages", "Organization", t =>
        {
            t.HasCheckConstraint("CK_Packages_ContractValue",
                "[ContractValue] >= 0");
            t.HasCheckConstraint("CK_Packages_ProgressPercentage",
                "[ProgressPercentage] >= 0 AND [ProgressPercentage] <= 100");
            t.HasCheckConstraint("CK_Packages_PlannedDates",
                "[PlannedEndDate] >= [PlannedStartDate]");
            t.HasCheckConstraint("CK_Packages_ActualDates",
                "([ActualStartDate] IS NULL AND [ActualEndDate] IS NULL) OR " +
                "([ActualStartDate] IS NOT NULL AND ([ActualEndDate] IS NULL OR [ActualEndDate] >= [ActualStartDate]))");
            t.HasCheckConstraint("CK_Packages_Assignment",
                "([PhaseId] IS NULL AND [WBSElementId] IS NOT NULL) OR " +
                "([PhaseId] IS NOT NULL AND [WBSElementId] IS NULL) OR " +
                "([PhaseId] IS NULL AND [WBSElementId] IS NULL)");
        });

        // Primary key
        builder.HasKey(p => p.Id);

        // Indexes
        builder.HasIndex(p => p.Code).IsUnique();
        builder.HasIndex(p => p.WBSCode);
        builder.HasIndex(p => p.PackageType);
        builder.HasIndex(p => p.ContractorId);
        builder.HasIndex(p => p.PhaseId);
        builder.HasIndex(p => p.WBSElementId);
        builder.HasIndex(p => p.IsDeleted);
        builder.HasIndex(p => p.ProgressPercentage);
        builder.HasIndex(p => p.PlannedStartDate);
        builder.HasIndex(p => p.PlannedEndDate);

        // Properties
        builder.Property(p => p.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(p => p.Description)
            .HasMaxLength(1000);

        builder.Property(p => p.WBSCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.PackageType)
            .IsRequired()
            .HasMaxLength(50);

        // Contract Information
        builder.Property(p => p.ContractNumber)
            .HasMaxLength(100);

        builder.Property(p => p.ContractType)
            .HasMaxLength(50);

        builder.Property(p => p.ContractValue)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.Currency)
            .IsRequired()
            .HasMaxLength(3)
            .HasDefaultValue("USD");

        // Dates
        builder.Property(p => p.PlannedStartDate)
            .IsRequired();

        builder.Property(p => p.PlannedEndDate)
            .IsRequired();

        // Progress
        builder.Property(p => p.ProgressPercentage)
            .HasPrecision(5, 2)
            .IsRequired()
            .HasDefaultValue(0);

        // Audit properties
        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.CreatedBy)
            .HasMaxLength(256);

        builder.Property(p => p.UpdatedAt);

        builder.Property(p => p.UpdatedBy)
            .HasMaxLength(256);

        // Soft delete properties
        builder.Property(p => p.DeletedAt);

        builder.Property(p => p.DeletedBy)
            .HasMaxLength(256);

        // Foreign key relationships
        builder.HasOne(p => p.Phase)
            .WithMany()
            .HasForeignKey(p => p.PhaseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.WBSElement)
            .WithMany()
            .HasForeignKey(p => p.WBSElementId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Contractor)
            .WithMany()
            .HasForeignKey(p => p.ContractorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Global query filter for soft delete
        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}