using Domain.Entities.Configuration.Templates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Configuration
{
    public class CBSTemplateElementConfiguration : IEntityTypeConfiguration<CBSTemplateElement>
    {
        public void Configure(EntityTypeBuilder<CBSTemplateElement> builder)
        {
            // Table name and schema
            builder.ToTable("CBSTemplateElements", "Configuration");

            // Primary key
            builder.HasKey(cte => cte.Id);

            // Indexes
            builder.HasIndex(cte => new { cte.CBSTemplateId, cte.Code }).IsUnique();
            builder.HasIndex(cte => cte.CBSTemplateId);
            builder.HasIndex(cte => cte.ParentId);
            builder.HasIndex(cte => cte.Level);
            builder.HasIndex(cte => cte.HierarchyPath);
            builder.HasIndex(cte => cte.CostType);
            builder.HasIndex(cte => cte.IsControlAccount);
            builder.HasIndex(cte => new { cte.CBSTemplateId, cte.Level });
            builder.HasIndex(cte => new { cte.CBSTemplateId, cte.IsControlAccount });

            // Properties
            builder.Property(cte => cte.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(cte => cte.Name)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(cte => cte.Description)
                .HasMaxLength(500);

            builder.Property(cte => cte.Level)
                .IsRequired();

            builder.Property(cte => cte.HierarchyPath)
                .IsRequired()
                .HasMaxLength(500);

            // Cost configuration
            builder.Property(cte => cte.CostType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(cte => cte.Unit)
                .HasMaxLength(50);

            builder.Property(cte => cte.UnitRate)
                .HasPrecision(18, 4);

            builder.Property(cte => cte.IsControlAccount)
                .IsRequired()
                .HasDefaultValue(false);

            // Audit properties
            builder.Property(cte => cte.CreatedAt)
                .IsRequired();

            builder.Property(cte => cte.CreatedBy)
                .HasMaxLength(256);

            builder.Property(cte => cte.UpdatedAt);

            builder.Property(cte => cte.UpdatedBy)
                .HasMaxLength(256);

            // Relationships
            builder.HasOne(cte => cte.CBSTemplate)
                .WithMany(ct => ct.Elements)
                .HasForeignKey(cte => cte.CBSTemplateId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(cte => cte.Parent)
                .WithMany(p => p.Children)
                .HasForeignKey(cte => cte.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_CBSTemplateElements_UnitRate",
                    "[UnitRate] IS NULL OR [UnitRate] >= 0");
                t.HasCheckConstraint("CK_CBSTemplateElements_Level",
                    "[Level] >= 1");
            });
        }
    }
}