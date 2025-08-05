using Domain.Entities.Cost.Control;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Cost
{
    public class ControlAccountConfiguration : IEntityTypeConfiguration<ControlAccount>
    {
        public void Configure(EntityTypeBuilder<ControlAccount> builder)
        {
            // Table name and schema
            builder.ToTable("ControlAccounts", "Cost");

            // Primary key
            builder.HasKey(ca => ca.Id);

            // Indexes
            builder.HasIndex(ca => ca.Code).IsUnique();
            builder.HasIndex(ca => ca.ProjectId);
            builder.HasIndex(ca => ca.PhaseId);
            builder.HasIndex(ca => ca.CAMUserId);
            builder.HasIndex(ca => ca.Status);
            builder.HasIndex(ca => ca.IsDeleted);
            builder.HasIndex(ca => ca.IsActive);
            builder.HasIndex(ca => new { ca.ProjectId, ca.Code }).IsUnique();
            builder.HasIndex(ca => new { ca.ProjectId, ca.Status });
            builder.HasIndex(ca => new { ca.CAMUserId, ca.Status });

            // Basic Information
            builder.Property(ca => ca.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(ca => ca.Name)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(ca => ca.Description)
                .HasMaxLength(2000);

            // Budget Information
            builder.Property(ca => ca.BAC)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(ca => ca.ContingencyReserve)
                .IsRequired()
                .HasPrecision(18, 2)
                .HasDefaultValue(0);

            builder.Property(ca => ca.ManagementReserve)
                .IsRequired()
                .HasPrecision(18, 2)
                .HasDefaultValue(0);

            // Control Information
            builder.Property(ca => ca.MeasurementMethod)
                .IsRequired();

            builder.Property(ca => ca.Status)
                .IsRequired();

            builder.Property(ca => ca.BaselineDate)
                .IsRequired();

            // Calculated Fields
            builder.Property(ca => ca.PercentComplete)
                .IsRequired()
                .HasPrecision(5, 2)
                .HasDefaultValue(0);

            // Soft Delete
            builder.Property(ca => ca.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(ca => ca.DeletedBy)
                .HasMaxLength(256);

            // Activatable
            builder.Property(ca => ca.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Audit properties
            builder.Property(ca => ca.CreatedAt)
                .IsRequired();

            builder.Property(ca => ca.CreatedBy)
                .HasMaxLength(256);

            builder.Property(ca => ca.UpdatedAt);

            builder.Property(ca => ca.UpdatedBy)
                .HasMaxLength(256);

            // Relationships
            builder.HasOne(ca => ca.Project)
                .WithMany(p => p.ControlAccounts)
                .HasForeignKey(ca => ca.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ca => ca.Phase)
                .WithMany(p => p.ControlAccounts)
                .HasForeignKey(ca => ca.PhaseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ca => ca.CAMUser)
                .WithMany()
                .HasForeignKey(ca => ca.CAMUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(ca => ca.WorkPackages)
                .WithOne(wp => wp.ControlAccount)
                .HasForeignKey(wp => wp.ControlAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(ca => ca.PlanningPackages)
                .WithOne(pp => pp.ControlAccount)
                .HasForeignKey(pp => pp.ControlAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(ca => ca.EVMRecords)
                .WithOne(evm => evm.ControlAccount)
                .HasForeignKey(evm => evm.ControlAccountId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(ca => ca.Assignments)
                .WithOne(a => a.ControlAccount)
                .HasForeignKey(a => a.ControlAccountId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(ca => ca.CostControlReports)
                .WithOne(ccr => ccr.ControlAccount)
                .HasForeignKey(ccr => ccr.ControlAccountId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(ca => ca.TimePhasedBudgets)
                .WithOne(tpb => tpb.ControlAccount)
                .HasForeignKey(tpb => tpb.ControlAccountId)
                .OnDelete(DeleteBehavior.Cascade);

            // Calculated property
            builder.Ignore(ca => ca.TotalBudget);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_ControlAccounts_BAC", "[BAC] >= 0");
                t.HasCheckConstraint("CK_ControlAccounts_ContingencyReserve", "[ContingencyReserve] >= 0");
                t.HasCheckConstraint("CK_ControlAccounts_ManagementReserve", "[ManagementReserve] >= 0");
                t.HasCheckConstraint("CK_ControlAccounts_PercentComplete", 
                    "[PercentComplete] >= 0 AND [PercentComplete] <= 100");
            });

            // Global query filter for soft delete
            builder.HasQueryFilter(ca => !ca.IsDeleted);
        }
    }
}