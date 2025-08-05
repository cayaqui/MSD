using Domain.Entities.Configuration.Templates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Configuration
{
    public class WBSTemplateElementConfiguration : IEntityTypeConfiguration<WBSTemplateElement>
    {
        public void Configure(EntityTypeBuilder<WBSTemplateElement> builder)
        {
            // Table name and schema
            builder.ToTable("WBSTemplateElements", "Configuration");

            // Primary key
            builder.HasKey(wte => wte.Id);

            // Indexes
            builder.HasIndex(wte => new { wte.WBSTemplateId, wte.Code }).IsUnique();
            builder.HasIndex(wte => wte.WBSTemplateId);
            builder.HasIndex(wte => wte.ParentId);
            builder.HasIndex(wte => wte.Level);
            builder.HasIndex(wte => wte.SequenceNumber);
            builder.HasIndex(wte => wte.HierarchyPath);
            builder.HasIndex(wte => wte.ElementType);
            builder.HasIndex(wte => new { wte.WBSTemplateId, wte.Level });

            // Properties
            builder.Property(wte => wte.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(wte => wte.Name)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(wte => wte.Description)
                .HasMaxLength(500);

            builder.Property(wte => wte.Level)
                .IsRequired();

            builder.Property(wte => wte.SequenceNumber)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(wte => wte.HierarchyPath)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(wte => wte.ElementType)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("Phase");

            builder.Property(wte => wte.IsOptional)
                .IsRequired()
                .HasDefaultValue(false);

            // Default values
            builder.Property(wte => wte.DefaultBudgetPercentage)
                .HasPrecision(5, 2);

            builder.Property(wte => wte.DefaultDurationDays);

            builder.Property(wte => wte.DefaultDiscipline)
                .HasMaxLength(100);

            // Audit properties
            builder.Property(wte => wte.CreatedAt)
                .IsRequired();

            builder.Property(wte => wte.CreatedBy)
                .HasMaxLength(256);

            builder.Property(wte => wte.UpdatedAt);

            builder.Property(wte => wte.UpdatedBy)
                .HasMaxLength(256);

            // Relationships
            builder.HasOne(wte => wte.WBSTemplate)
                .WithMany(wt => wt.Elements)
                .HasForeignKey(wte => wte.WBSTemplateId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(wte => wte.Parent)
                .WithMany(p => p.Children)
                .HasForeignKey(wte => wte.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_WBSTemplateElements_DefaultBudgetPercentage",
                    "[DefaultBudgetPercentage] IS NULL OR ([DefaultBudgetPercentage] >= 0 AND [DefaultBudgetPercentage] <= 100)");
                t.HasCheckConstraint("CK_WBSTemplateElements_DefaultDurationDays",
                    "[DefaultDurationDays] IS NULL OR [DefaultDurationDays] >= 0");
                t.HasCheckConstraint("CK_WBSTemplateElements_Level",
                    "[Level] >= 1");
                t.HasCheckConstraint("CK_WBSTemplateElements_SequenceNumber",
                    "[SequenceNumber] >= 0");
            });
        }
    }
}