// ProjectConfiguration.cs - Corregida
using Domain.Entities.Projects;

namespace Infrastructure.Data.Configurations.Projects;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        // Table name and schema with check constraints
        builder.ToTable("Projects", "Setup", t =>
        {
            t.HasCheckConstraint("CK_Projects_PlannedDates",
                "[PlannedEndDate] > [PlannedStartDate]");
            t.HasCheckConstraint("CK_Projects_ActualDates",
                "[ActualEndDate] IS NULL OR [ActualStartDate] IS NULL OR [ActualEndDate] >= [ActualStartDate]");
            t.HasCheckConstraint("CK_Projects_PercentComplete",
                "[PercentComplete] >= 0 AND [PercentComplete] <= 100");
            t.HasCheckConstraint("CK_Projects_Budget",
                "[Budget] IS NULL OR [Budget] >= 0");
            t.HasCheckConstraint("CK_Projects_ApprovedBudget",
                "[ApprovedBudget] IS NULL OR [ApprovedBudget] >= 0");
        });

        // Primary key
        builder.HasKey(p => p.Id);

        // Indexes
        builder.HasIndex(p => new { p.OperationId, p.Code }).IsUnique();
        builder.HasIndex(p => p.OperationId);
        builder.HasIndex(p => p.ProjectManagerId);
        builder.HasIndex(p => p.Status);
        builder.HasIndex(p => p.IsDeleted);
        builder.HasIndex(p => new { p.IsActive, p.PlannedStartDate, p.PlannedEndDate });

        // Properties
        builder.Property(p => p.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(p => p.Description)
            .HasMaxLength(1000);

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(p => p.PlannedStartDate)
            .IsRequired();

        builder.Property(p => p.PlannedEndDate)
            .IsRequired();

        builder.Property(p => p.ActualStartDate);

        builder.Property(p => p.ActualEndDate);

        builder.Property(p => p.TotalBudget)
            .HasPrecision(18, 2);

        builder.Property(p => p.ApprovedBudget)
            .HasPrecision(18, 2);
        builder.Property(p => p.CommittedCost)
            .HasPrecision(18, 2);
        builder.Property(p => p.ActualCost)
            .HasPrecision(18, 2);


        builder.Property(p => p.Currency)
            .IsRequired()
            .HasMaxLength(3)
            .HasDefaultValue("USD");

        builder.Property(p => p.Location)
            .HasMaxLength(256);

        builder.Property(p => p.ContractNumber)
            .HasMaxLength(100);

        builder.Property(p => p.ProgressPercentage)
            .HasPrecision(5, 2)
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
        builder.HasOne(p => p.Operation)
            .WithMany(o => o.Projects)
            .HasForeignKey(p => p.OperationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.ProjectManager)
            .WithMany()
            .HasForeignKey(p => p.ProjectManagerId)
            .OnDelete(DeleteBehavior.SetNull);

        // Navigation properties
        builder.HasMany(p => p.ProjectTeamMembers)
            .WithOne(ptm => ptm.Project)
            .HasForeignKey(ptm => ptm.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Phases)
            .WithOne(ph => ph.Project)
            .HasForeignKey(ph => ph.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.WBSElements)
            .WithOne(wbs => wbs.Project)
            .HasForeignKey(wbs => wbs.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.ControlAccounts)
            .WithOne(ca => ca.Project)
            .HasForeignKey(ca => ca.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.UserProjectPermissions)
            .WithOne(upp => upp.Project)
            .HasForeignKey(upp => upp.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // Global query filter for soft delete
        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}