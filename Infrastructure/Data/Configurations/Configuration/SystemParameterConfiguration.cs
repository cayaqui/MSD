using Domain.Entities.Configuration.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Configuration
{
    public class SystemParameterConfiguration : IEntityTypeConfiguration<SystemParameter>
    {
        public void Configure(EntityTypeBuilder<SystemParameter> builder)
        {
            // Table name and schema
            builder.ToTable("SystemParameters", "Configuration");

            // Primary key
            builder.HasKey(sp => sp.Id);

            // Indexes
            builder.HasIndex(sp => new { sp.Category, sp.Key }).IsUnique();
            builder.HasIndex(sp => sp.Category);
            builder.HasIndex(sp => sp.IsSystem);
            builder.HasIndex(sp => sp.IsPublic);
            builder.HasIndex(sp => new { sp.IsPublic, sp.Category });

            // Properties
            builder.Property(sp => sp.Category)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(sp => sp.Key)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(sp => sp.Value)
                .IsRequired()
                .HasMaxLength(4000);

            builder.Property(sp => sp.DisplayName)
                .HasMaxLength(256);

            builder.Property(sp => sp.Description)
                .HasMaxLength(500);

            builder.Property(sp => sp.DataType)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("String");

            builder.Property(sp => sp.IsEncrypted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(sp => sp.IsRequired)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(sp => sp.IsSystem)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(sp => sp.IsPublic)
                .IsRequired()
                .HasDefaultValue(false);

            // Validation
            builder.Property(sp => sp.ValidationRule)
                .HasMaxLength(500);

            builder.Property(sp => sp.DefaultValue)
                .HasMaxLength(4000);

            builder.Property(sp => sp.MinValue);

            builder.Property(sp => sp.MaxValue);

            builder.Property(sp => sp.AllowedValuesJson)
                .HasColumnType("nvarchar(max)");

            // Audit
            builder.Property(sp => sp.LastModifiedDate);

            builder.Property(sp => sp.LastModifiedBy)
                .HasMaxLength(256);

            // Base audit properties
            builder.Property(sp => sp.CreatedAt)
                .IsRequired();

            builder.Property(sp => sp.CreatedBy)
                .HasMaxLength(256);

            builder.Property(sp => sp.UpdatedAt);

            builder.Property(sp => sp.UpdatedBy)
                .HasMaxLength(256);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_SystemParameters_DataType",
                    "[DataType] IN ('String', 'Number', 'Boolean', 'Date', 'Json')");
                t.HasCheckConstraint("CK_SystemParameters_MinMaxValue",
                    "[MinValue] IS NULL OR [MaxValue] IS NULL OR [MinValue] <= [MaxValue]");
            });
        }
    }
}