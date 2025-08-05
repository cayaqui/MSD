using Domain.Entities.Configuration.Templates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Configuration
{
    public class CBSTemplateConfiguration : IEntityTypeConfiguration<CBSTemplate>
    {
        public void Configure(EntityTypeBuilder<CBSTemplate> builder)
        {
            // Table name and schema
            builder.ToTable("CBSTemplates", "Configuration");

            // Primary key
            builder.HasKey(ct => ct.Id);

            // Indexes
            builder.HasIndex(ct => ct.Code).IsUnique();
            builder.HasIndex(ct => ct.IndustryType);
            builder.HasIndex(ct => ct.CostType);
            builder.HasIndex(ct => new { ct.IndustryType, ct.CostType });
            builder.HasIndex(ct => ct.IsPublic);
            builder.HasIndex(ct => ct.IsActive);
            builder.HasIndex(ct => ct.IsDeleted);
            builder.HasIndex(ct => new { ct.IsActive, ct.IsPublic, ct.IsDeleted });

            // Properties
            builder.Property(ct => ct.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(ct => ct.Name)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(ct => ct.Description)
                .HasMaxLength(500);

            builder.Property(ct => ct.IndustryType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(ct => ct.CostType)
                .IsRequired()
                .HasMaxLength(50);

            // Settings
            builder.Property(ct => ct.CodingScheme)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("Numeric");

            builder.Property(ct => ct.Delimiter)
                .IsRequired()
                .HasMaxLength(5)
                .HasDefaultValue(".");

            builder.Property(ct => ct.IncludesIndirectCosts)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(ct => ct.IncludesContingency)
                .IsRequired()
                .HasDefaultValue(true);

            // Usage
            builder.Property(ct => ct.IsPublic)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(ct => ct.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(ct => ct.UsageCount)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(ct => ct.LastUsedDate);

            // Soft delete
            builder.Property(ct => ct.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(ct => ct.DeletedBy)
                .HasMaxLength(256);

            // Audit properties
            builder.Property(ct => ct.CreatedAt)
                .IsRequired();

            builder.Property(ct => ct.CreatedBy)
                .HasMaxLength(256);

            builder.Property(ct => ct.UpdatedAt);

            builder.Property(ct => ct.UpdatedBy)
                .HasMaxLength(256);

            // Navigation properties
            builder.HasMany(ct => ct.Elements)
                .WithOne(e => e.CBSTemplate)
                .HasForeignKey(e => e.CBSTemplateId)
                .OnDelete(DeleteBehavior.Cascade);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_CBSTemplates_CodingScheme",
                    "[CodingScheme] IN ('Numeric', 'Alphabetic', 'Alphanumeric')");
            });

            // Global query filter for soft delete
            builder.HasQueryFilter(ct => !ct.IsDeleted);
        }
    }
}