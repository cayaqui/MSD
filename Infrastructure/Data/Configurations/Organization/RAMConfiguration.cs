using Domain.Entities.Organization.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Organization
{
    public class RAMConfiguration : IEntityTypeConfiguration<RAM>
    {
        public void Configure(EntityTypeBuilder<RAM> builder)
        {
            builder.ToTable("RAMAssignments", "Organization");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.ResponsibilityType)
                .HasMaxLength(1)
                .IsRequired();

            builder.Property(e => e.AllocatedHours)
                .HasPrecision(10, 2);

            builder.Property(e => e.AllocatedPercentage)
                .HasPrecision(5, 2);

            builder.Property(e => e.StartDate);

            builder.Property(e => e.EndDate);

            builder.Property(e => e.Notes)
                .HasMaxLength(1000);

            builder.Property(e => e.IsActive)
                .IsRequired();

            // Relationships
            builder.HasOne(e => e.Project)
                .WithMany(p => p.RAMAssignments)
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.WBSElement)
                .WithMany()
                .HasForeignKey(e => e.WBSElementId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.OBSNode)
                .WithMany()
                .HasForeignKey(e => e.OBSNodeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.ControlAccount)
                .WithMany()
                .HasForeignKey(e => e.ControlAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(e => e.ProjectId);
            builder.HasIndex(e => e.WBSElementId);
            builder.HasIndex(e => e.OBSNodeId);
            builder.HasIndex(e => e.ResponsibilityType);
            builder.HasIndex(e => new { e.ProjectId, e.WBSElementId, e.OBSNodeId, e.ResponsibilityType })
                .IsUnique();
        }
    }
}