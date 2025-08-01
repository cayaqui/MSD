using Domain.Entities.Cost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Cost;

/// <summary>
/// Entity configuration for ControlAccount
/// </summary>
public class ControlAccountConfiguration : IEntityTypeConfiguration<ControlAccount>
{
    public void Configure(EntityTypeBuilder<ControlAccount> builder)
    {
        // Table name and schema
        builder.ToTable("ControlAccounts", "Cost");

        // Primary key
        builder.HasKey(ca => ca.Id);

        // Indexes
        builder.HasIndex(ca => new { ca.ProjectId, ca.Code }).IsUnique();
        builder.HasIndex(ca => ca.ProjectId);
        builder.HasIndex(ca => ca.PhaseId);
        builder.HasIndex(ca => ca.CAMUserId);
        builder.HasIndex(ca => ca.Status);
        builder.HasIndex(ca => ca.IsDeleted);
        builder.HasIndex(ca => new { ca.IsActive, ca.BaselineDate });

        // Properties
        builder.Property(ca => ca.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(ca => ca.Name)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(ca => ca.Description)
            .HasMaxLength(1000);

        builder.Property(ca => ca.BAC)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(ca => ca.ContingencyReserve)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(ca => ca.ManagementReserve)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(ca => ca.MeasurementMethod)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(ca => ca.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(ca => ca.BaselineDate)
            .IsRequired();

        builder.Property(ca => ca.PercentComplete)
            .HasPrecision(5, 2)
            .HasDefaultValue(0);

        // Computed column for TotalBudget
        builder.Property(ca => ca.TotalBudget)
            .HasComputedColumnSql("[BAC] + [ContingencyReserve] + [ManagementReserve]", stored: false);

        // Audit properties
        builder.Property(ca => ca.CreatedAt)
            .IsRequired();

        builder.Property(ca => ca.CreatedBy)
            .HasMaxLength(256);

        builder.Property(ca => ca.UpdatedAt);

        builder.Property(ca => ca.UpdatedBy)
            .HasMaxLength(256);

        // Soft delete properties
        builder.Property(ca => ca.DeletedAt);

        builder.Property(ca => ca.DeletedBy)
            .HasMaxLength(256);

  
        builder.HasOne(ca => ca.Phase)
            .WithMany(p => p.ControlAccounts)
            .HasForeignKey(ca => ca.PhaseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ca => ca.CAMUser)
            .WithMany()
            .HasForeignKey(ca => ca.CAMUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Navigation properties
        builder.HasMany(ca => ca.WorkPackages)
            .WithOne(wp => wp.ControlAccount)
            .HasForeignKey(wp => wp.ControlAccountId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(ca => ca.PlanningPackages)
            .WithOne(pp => pp.ControlAccount)
            .HasForeignKey(pp => pp.ControlAccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(ca => ca.EVMRecords)
            .WithOne(evm => evm.ControlAccount)
            .HasForeignKey(evm => evm.ControlAccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(ca => ca.Assignments)
            .WithOne(caa => caa.ControlAccount)
            .HasForeignKey(caa => caa.ControlAccountId)
            .OnDelete(DeleteBehavior.Cascade);

        // Global query filter for soft delete
        builder.HasQueryFilter(ca => !ca.IsDeleted);
    }
}