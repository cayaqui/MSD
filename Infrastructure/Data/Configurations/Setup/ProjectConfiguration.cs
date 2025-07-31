using Domain.Entities.Projects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Setup;

/// <summary>
/// Entity configuration for Project
/// </summary>
public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        // Table name
        builder.ToTable("Projects", "Setup");

        // Primary key
        builder.HasKey(p => p.Id);

        // Indexes
        builder.HasIndex(p => p.Code).IsUnique();
        builder.HasIndex(p => p.WBSCode);
        builder.HasIndex(p => new { p.OperationId, p.Status });
        builder.HasIndex(p => new { p.IsDeleted, p.IsActive });

        // Properties
        builder.Property(p => p.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(p => p.Description)
            .HasMaxLength(2000);

        builder.Property(p => p.WBSCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.ProjectCharter)
            .HasMaxLength(4000);

        builder.Property(p => p.Objectives)
            .HasMaxLength(4000);

        builder.Property(p => p.Scope)
            .HasMaxLength(4000);

        builder.Property(p => p.Deliverables)
            .HasMaxLength(4000);

        builder.Property(p => p.Currency)
            .IsRequired()
            .HasMaxLength(3)
            .HasDefaultValue("USD");

        builder.Property(p => p.TotalBudget)
            .HasPrecision(18, 2);

        builder.Property(p => p.ActualCost)
            .HasPrecision(18, 2);

        builder.Property(p => p.ProgressPercentage)
            .HasPrecision(5, 2)
            .HasDefaultValue(0);

        builder.Property(p => p.Location)
            .HasMaxLength(256);

        builder.Property(p => p.Client)
            .HasMaxLength(256);

        builder.Property(p => p.ContractNumber)
            .HasMaxLength(100);

        builder.Property(p => p.ProjectManagerId)
            .HasMaxLength(128);

        builder.Property(p => p.Status)
            .HasConversion<int>();

        // Relationships
        builder.HasOne(p => p.Operation)
            .WithMany(o => o.Projects)
            .HasForeignKey(p => p.OperationId)
            .OnDelete(DeleteBehavior.Restrict);

        //builder.HasMany(p => p.Phases)
        //    .WithOne(ph => ph.Project)
        //    .HasForeignKey(ph => ph.ProjectId)
        //    .OnDelete(DeleteBehavior.Cascade);

        //builder.HasMany(p => p.WorkPackages)
        //    .WithOne(wp => wp.Project)
        //    .HasForeignKey(wp => wp.ProjectId)
        //    .OnDelete(DeleteBehavior.Cascade);

        //builder.HasMany(p => p.Packages)
        //    .WithOne()
        //    .HasForeignKey("ProjectId")
        //    .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.ProjectTeamMembers)
            .WithOne(ptm => ptm.Project)
            .HasForeignKey(ptm => ptm.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        //builder.HasMany(p => p.BudgetVersions)
        //    .WithOne(b => b.Project)
        //    .HasForeignKey(b => b.ProjectId)
        //    .OnDelete(DeleteBehavior.Cascade);

        //builder.HasMany(p => p.ScheduleVersions)
        //    .WithOne(s => s.Project)
        //    .HasForeignKey(s => s.ProjectId)
        //    .OnDelete(DeleteBehavior.Cascade);
    }
}
