// WBSElementConfiguration.cs - Corregida
using Domain.Entities.Projects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Projects;

public class WBSElementConfiguration : IEntityTypeConfiguration<WBSElement>
{
    public void Configure(EntityTypeBuilder<WBSElement> builder)
    {
        // Table name and schema with check constraints
        builder.ToTable("WBSElements", "Projects", t =>
        {
            t.HasCheckConstraint("CK_WBSElements_Level",
                "[Level] >= 1 AND [Level] <= 10");
            t.HasCheckConstraint("CK_WBSElements_PercentComplete",
                "[PercentComplete] >= 0 AND [PercentComplete] <= 100");
            t.HasCheckConstraint("CK_WBSElements_Budget",
                "[Budget] IS NULL OR [Budget] >= 0");
            t.HasCheckConstraint("CK_WBSElements_ActualCost",
                "[ActualCost] >= 0");
            t.HasCheckConstraint("CK_WBSElements_EarnedValue",
                "[EarnedValue] >= 0");
        });

        // Primary key
        builder.HasKey(wbs => wbs.Id);

        // Indexes
        builder.HasIndex(wbs => new { wbs.ProjectId, wbs.Code }).IsUnique();
        builder.HasIndex(wbs => wbs.ProjectId);
        builder.HasIndex(wbs => wbs.ParentId);
        builder.HasIndex(wbs => wbs.Level);
        builder.HasIndex(wbs => wbs.IsDeleted);
        builder.HasIndex(wbs => new { wbs.IsActive, wbs.ElementType });

        // Properties
        builder.Property(wbs => wbs.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(wbs => wbs.Name)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(wbs => wbs.Description)
            .HasMaxLength(1000);

        builder.Property(wbs => wbs.Level)
            .IsRequired();

        builder.Property(wbs => wbs.ElementType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);


        // Audit properties
        builder.Property(wbs => wbs.CreatedAt)
            .IsRequired();

        builder.Property(wbs => wbs.CreatedBy)
            .HasMaxLength(256);

        builder.Property(wbs => wbs.UpdatedAt);

        builder.Property(wbs => wbs.UpdatedBy)
            .HasMaxLength(256);

        // Soft delete properties
        builder.Property(wbs => wbs.DeletedAt);

        builder.Property(wbs => wbs.DeletedBy)
            .HasMaxLength(256);

        // Foreign key relationships
        builder.HasOne(wbs => wbs.Project)
            .WithMany(p => p.WBSCode)
            .HasForeignKey(wbs => wbs.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(wbs => wbs.Parent)
            .WithMany(wbs => wbs.Children)
            .HasForeignKey(wbs => wbs.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        
        builder.HasOne(wbs => wbs.ControlAccount)
            .WithMany(ca => ca.WorkPackages)
            .HasForeignKey(wbs => wbs.ControlAccountId)
            .OnDelete(DeleteBehavior.SetNull);


        // Global query filter for soft delete
        builder.HasQueryFilter(wbs => !wbs.IsDeleted);
    }
}
