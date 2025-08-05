using Domain.Entities.Organization.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Organization;

/// <summary>
/// Entity configuration for Phase
/// </summary>
public class PhaseConfiguration : IEntityTypeConfiguration<Phase>
{
    public void Configure(EntityTypeBuilder<Phase> builder)
    {
        // Table name and schema
        builder.ToTable("Phases", "Organization", t =>
        {
            t.HasCheckConstraint("CK_Phases_PlannedDates",
            "[PlannedEndDate] > [PlannedStartDate]");
            t.HasCheckConstraint("CK_Phases_ActualDates",
            "[ActualEndDate] IS NULL OR [ActualStartDate] IS NULL OR [ActualEndDate] >= [ActualStartDate]");
            t.HasCheckConstraint("CK_Phases_ProgressPercentage",
            "[ProgressPercentage] >= 0 AND [ProgressPercentage] <= 100");
            t.HasCheckConstraint("CK_Phases_PlannedBudget",
            "[PlannedBudget] IS NULL OR [PlannedBudget] >= 0");
        });

        // Primary key
        builder.HasKey(p => p.Id);

        // Indexes
        builder.HasIndex(p => new { p.ProjectId, p.Code }).IsUnique();
        builder.HasIndex(p => p.ProjectId);
        builder.HasIndex(p => p.PhaseType);
        builder.HasIndex(p => p.IsDeleted);
        builder.HasIndex(p => new { p.IsActive, p.PlannedStartDate, p.PlannedEndDate});

        // Properties
        builder.Property(p => p.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(p => p.Description)
            .HasMaxLength(1000);

        builder.Property(p => p.PhaseType)
            .IsRequired();
        builder.Property(p => p.Status)
           .IsRequired();

        builder.Property(p => p.PlannedStartDate)
            .IsRequired();

        builder.Property(p => p.PlannedEndDate)
            .IsRequired();

        builder.Property(p => p.ActualStartDate);

        builder.Property(p => p.ActualEndDate);

        builder.Property(p => p.PlannedBudget)
            .HasPrecision(18, 2);

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

        builder.HasMany(p => p.ControlAccounts)
            .WithOne(ca => ca.Phase)
            .HasForeignKey(ca => ca.PhaseId)
            .OnDelete(DeleteBehavior.Restrict);

        // Global query filter for soft delete
        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}