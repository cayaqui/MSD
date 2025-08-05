using Domain.Entities.Organization.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Organization
{
    public class OBSNodeConfiguration : IEntityTypeConfiguration<OBSNode>
    {
        public void Configure(EntityTypeBuilder<OBSNode> builder)
        {
            // Table name and schema
            builder.ToTable("OBSNodes", "Organization");

            // Primary key
            builder.HasKey(o => o.Id);

            // Indexes
            builder.HasIndex(o => o.Code);
            builder.HasIndex(o => new { o.ProjectId, o.Code }).IsUnique();
            builder.HasIndex(o => o.HierarchyPath);
            builder.HasIndex(o => o.ParentId);
            builder.HasIndex(o => o.ProjectId);
            builder.HasIndex(o => o.ManagerId);
            builder.HasIndex(o => o.NodeType);
            builder.HasIndex(o => o.IsActive);
            builder.HasIndex(o => new { o.ProjectId, o.IsActive });

            // Properties
            builder.Property(o => o.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(o => o.Name)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(o => o.Description)
                .HasMaxLength(500);

            builder.Property(o => o.NodeType)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("Department");

            builder.Property(o => o.Level)
                .IsRequired();

            builder.Property(o => o.HierarchyPath)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(o => o.CostCenter)
                .HasMaxLength(50);

            builder.Property(o => o.TotalFTE)
                .HasPrecision(10, 2);

            builder.Property(o => o.AvailableFTE)
                .HasPrecision(10, 2);

            builder.Property(o => o.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Audit properties
            builder.Property(o => o.CreatedAt)
                .IsRequired();

            builder.Property(o => o.CreatedBy)
                .HasMaxLength(256);

            builder.Property(o => o.UpdatedAt);

            builder.Property(o => o.UpdatedBy)
                .HasMaxLength(256);

            // Relationships
            builder.HasOne(o => o.Parent)
                .WithMany(p => p.Children)
                .HasForeignKey(o => o.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(o => o.Project)
                .WithMany(p => p.OBSNodes)
                .HasForeignKey(o => o.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(o => o.Manager)
                .WithMany()
                .HasForeignKey(o => o.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(o => o.Members)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "OBSNodeMembers",
                    j => j.HasOne<Domain.Entities.Auth.Security.User>()
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict),
                    j => j.HasOne<OBSNode>()
                        .WithMany()
                        .HasForeignKey("OBSNodeId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey("OBSNodeId", "UserId");
                        j.ToTable("OBSNodeMembers", "Organization");
                    });

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_OBSNodes_NodeType",
                    "[NodeType] IN ('Company', 'Division', 'Department', 'Team', 'Role')");
                t.HasCheckConstraint("CK_OBSNodes_FTE",
                    "[TotalFTE] IS NULL OR [TotalFTE] >= 0");
                t.HasCheckConstraint("CK_OBSNodes_AvailableFTE",
                    "[AvailableFTE] IS NULL OR ([AvailableFTE] >= 0 AND ([TotalFTE] IS NULL OR [AvailableFTE] <= [TotalFTE]))");
            });
        }
    }
}