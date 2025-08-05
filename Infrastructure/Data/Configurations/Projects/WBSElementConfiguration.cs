using Domain.Entities.WBS;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Projects
{
    public class WBSElementConfiguration : IEntityTypeConfiguration<WBSElement>
    {
        public void Configure(EntityTypeBuilder<WBSElement> builder)
        {
            // Table name and schema
            builder.ToTable("WBSElements", "Projects");

            // Primary key
            builder.HasKey(w => w.Id);

            // Indexes
            builder.HasIndex(w => w.Code).IsUnique();
            builder.HasIndex(w => w.ProjectId);
            builder.HasIndex(w => w.ParentId);
            builder.HasIndex(w => w.ControlAccountId);
            builder.HasIndex(w => w.ElementType);
            builder.HasIndex(w => w.Level);
            builder.HasIndex(w => w.IsDeleted);
            builder.HasIndex(w => w.IsActive);
            builder.HasIndex(w => new { w.ProjectId, w.Code }).IsUnique();
            builder.HasIndex(w => new { w.ProjectId, w.Level });
            builder.HasIndex(w => new { w.ProjectId, w.ElementType });

            // Properties
            builder.Property(w => w.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(w => w.Name)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(w => w.Description)
                .HasMaxLength(4000);

            builder.Property(w => w.Level)
                .IsRequired();

            builder.Property(w => w.SequenceNumber)
                .IsRequired();

            builder.Property(w => w.FullPath)
                .IsRequired()
                .HasMaxLength(2048);

            builder.Property(w => w.ElementType)
                .IsRequired();

            // WBS Dictionary fields
            builder.Property(w => w.DeliverableDescription)
                .HasMaxLength(4000);

            builder.Property(w => w.AcceptanceCriteria)
                .HasMaxLength(4000);

            builder.Property(w => w.Assumptions)
                .HasMaxLength(4000);

            builder.Property(w => w.Constraints)
                .HasMaxLength(4000);

            builder.Property(w => w.ExclusionsInclusions)
                .HasMaxLength(4000);

            // Soft Delete
            builder.Property(w => w.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(w => w.DeletedBy)
                .HasMaxLength(256);

            // Activatable
            builder.Property(w => w.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Audit properties
            builder.Property(w => w.CreatedAt)
                .IsRequired();

            builder.Property(w => w.CreatedBy)
                .HasMaxLength(256);

            builder.Property(w => w.UpdatedAt);

            builder.Property(w => w.UpdatedBy)
                .HasMaxLength(256);

            // Relationships
            builder.HasOne(w => w.Project)
                .WithMany(p => p.WBSElements)
                .HasForeignKey(w => w.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(w => w.Parent)
                .WithMany(p => p.Children)
                .HasForeignKey(w => w.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(w => w.ControlAccount)
                .WithMany(ca => ca.WorkPackages)
                .HasForeignKey(w => w.ControlAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(w => w.WorkPackageDetails)
                .WithOne(wpd => wpd.WBSElement)
                .HasForeignKey<WorkPackageDetails>(wpd => wpd.WBSElementId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(w => w.CBSMappings)
                .WithOne(m => m.WBSElement)
                .HasForeignKey(m => m.WBSElementId)
                .OnDelete(DeleteBehavior.Cascade);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_WBSElements_Level", "[Level] >= 0");
                t.HasCheckConstraint("CK_WBSElements_SequenceNumber", "[SequenceNumber] > 0");
            });

            // Global query filter for soft delete
            builder.HasQueryFilter(w => !w.IsDeleted);
        }
    }
}