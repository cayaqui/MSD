using Domain.Entities.Configuration.Templates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Configuration
{
    public class WBSTemplateConfiguration : IEntityTypeConfiguration<WBSTemplate>
    {
        public void Configure(EntityTypeBuilder<WBSTemplate> builder)
        {
            // Table name and schema
            builder.ToTable("WBSTemplates", "Configuration");

            // Primary key
            builder.HasKey(wt => wt.Id);

            // Indexes
            builder.HasIndex(wt => wt.Code).IsUnique();
            builder.HasIndex(wt => wt.IndustryType);
            builder.HasIndex(wt => wt.ProjectType);
            builder.HasIndex(wt => new { wt.IndustryType, wt.ProjectType });
            builder.HasIndex(wt => wt.IsPublic);
            builder.HasIndex(wt => wt.IsActive);
            builder.HasIndex(wt => wt.IsDeleted);
            builder.HasIndex(wt => new { wt.IsActive, wt.IsPublic, wt.IsDeleted });

            // Properties
            builder.Property(wt => wt.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(wt => wt.Name)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(wt => wt.Description)
                .HasMaxLength(500);

            builder.Property(wt => wt.IndustryType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(wt => wt.ProjectType)
                .IsRequired()
                .HasMaxLength(100);

            // Settings
            builder.Property(wt => wt.CodingScheme)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("Numeric");

            builder.Property(wt => wt.Delimiter)
                .IsRequired()
                .HasMaxLength(5)
                .HasDefaultValue(".");

            builder.Property(wt => wt.CodeLength)
                .IsRequired()
                .HasDefaultValue(3);

            builder.Property(wt => wt.AutoGenerateCodes)
                .IsRequired()
                .HasDefaultValue(true);

            // Usage
            builder.Property(wt => wt.IsPublic)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(wt => wt.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(wt => wt.UsageCount)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(wt => wt.LastUsedDate);

            // Soft delete
            builder.Property(wt => wt.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(wt => wt.DeletedBy)
                .HasMaxLength(256);

            // Audit properties
            builder.Property(wt => wt.CreatedAt)
                .IsRequired();

            builder.Property(wt => wt.CreatedBy)
                .HasMaxLength(256);

            builder.Property(wt => wt.UpdatedAt);

            builder.Property(wt => wt.UpdatedBy)
                .HasMaxLength(256);

            // Navigation properties
            builder.HasMany(wt => wt.Elements)
                .WithOne(e => e.WBSTemplate)
                .HasForeignKey(e => e.WBSTemplateId)
                .OnDelete(DeleteBehavior.Cascade);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_WBSTemplates_CodeLength",
                    "[CodeLength] >= 1 AND [CodeLength] <= 10");
                t.HasCheckConstraint("CK_WBSTemplates_CodingScheme",
                    "[CodingScheme] IN ('Numeric', 'Alphabetic', 'Alphanumeric')");
            });

            // Global query filter for soft delete
            builder.HasQueryFilter(wt => !wt.IsDeleted);
        }
    }
}