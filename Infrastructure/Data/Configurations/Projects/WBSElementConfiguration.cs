using Domain.Entities.Projects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Projects;

/// <summary>
/// Entity Framework configuration for WBSElement
/// </summary>
public class WBSElementConfiguration : IEntityTypeConfiguration<WBSElement>
{
    public void Configure(EntityTypeBuilder<WBSElement> builder)
    {
        // Table name and schema
        builder.ToTable("WBSElements", "Projects");

        // Primary Key
        builder.HasKey(e => e.Id);

        // Properties
        builder.Property(e => e.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Description)
            .HasMaxLength(1000);

        builder.Property(e => e.FullPath)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.Level)
            .IsRequired();

        builder.Property(e => e.SequenceNumber)
            .IsRequired();

        builder.Property(e => e.ElementType)
            .IsRequired()
            .HasConversion<string>();

        // WBS Dictionary fields
        builder.Property(e => e.DeliverableDescription)
            .HasMaxLength(2000);

        builder.Property(e => e.AcceptanceCriteria)
            .HasMaxLength(2000);

        builder.Property(e => e.Assumptions)
            .HasMaxLength(2000);

        builder.Property(e => e.Constraints)
            .HasMaxLength(2000);

        builder.Property(e => e.ExclusionsInclusions)
            .HasMaxLength(2000);

        // Soft Delete
        builder.Property(e => e.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.DeletedBy)
            .HasMaxLength(100);

        // Activatable
        builder.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Audit fields
        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.CreatedBy)
            .HasMaxLength(100);

        builder.Property(e => e.UpdatedBy)
            .HasMaxLength(100);

        // Indexes
        builder.HasIndex(e => new { e.ProjectId, e.Code })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(e => new { e.ProjectId, e.ParentId, e.SequenceNumber });

        builder.HasIndex(e => e.ControlAccountId)
            .HasFilter("[ControlAccountId] IS NOT NULL");

        builder.HasIndex(e => e.ElementType);

        builder.HasIndex(e => e.IsDeleted);

        // Relationships
        builder.HasOne(e => e.Project)
            .WithMany(p => p.WBSElements)
            .HasForeignKey(e => e.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Parent)
            .WithMany(p => p.Children)
            .HasForeignKey(e => e.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.ControlAccount)
            .WithMany(ca => ca.WorkPackages)
            .HasForeignKey(e => e.ControlAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.WorkPackageDetails)
            .WithOne(wp => wp.WBSElement)
            .HasForeignKey<WorkPackageDetails>(wp => wp.WBSElementId)
            .OnDelete(DeleteBehavior.Cascade);

        // Query Filters
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
