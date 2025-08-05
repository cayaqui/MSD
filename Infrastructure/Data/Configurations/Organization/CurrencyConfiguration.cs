using Domain.Entities.Organization.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Organization
{
    public class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
    {
        public void Configure(EntityTypeBuilder<Currency> builder)
        {
            // Table name and schema
            builder.ToTable("Currencies", "Organization");

            // Primary key
            builder.HasKey(c => c.Id);

            // Indexes
            builder.HasIndex(c => c.Code).IsUnique();
            builder.HasIndex(c => c.NumericCode).IsUnique();
            builder.HasIndex(c => c.IsActive);
            builder.HasIndex(c => c.IsBaseCurrency);
            builder.HasIndex(c => new { c.IsEnabledForProjects, c.IsActive });
            builder.HasIndex(c => new { c.IsEnabledForCommitments, c.IsActive });

            // Properties
            builder.Property(c => c.Code)
                .IsRequired()
                .HasMaxLength(3); // ISO 4217 code

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Symbol)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(c => c.SymbolNative)
                .HasMaxLength(10);

            builder.Property(c => c.NumericCode)
                .IsRequired();

            builder.Property(c => c.DecimalDigits)
                .IsRequired()
                .HasDefaultValue(2);

            builder.Property(c => c.Rounding)
                .HasPrecision(10, 4);

            builder.Property(c => c.PluralName)
                .HasMaxLength(100);

            builder.Property(c => c.DecimalSeparator)
                .HasMaxLength(5)
                .HasDefaultValue(".");

            builder.Property(c => c.ThousandsSeparator)
                .HasMaxLength(5)
                .HasDefaultValue(",");

            builder.Property(c => c.PositivePattern)
                .HasMaxLength(20);

            builder.Property(c => c.NegativePattern)
                .HasMaxLength(20);

            builder.Property(c => c.ExchangeRate)
                .HasPrecision(18, 6)
                .HasDefaultValue(1m);

            builder.Property(c => c.ExchangeRateDate);

            builder.Property(c => c.ExchangeRateSource)
                .HasMaxLength(100);

            builder.Property(c => c.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(c => c.IsEnabledForProjects)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(c => c.IsEnabledForCommitments)
                .IsRequired()
                .HasDefaultValue(true);

            // Audit properties
            builder.Property(c => c.CreatedAt)
                .IsRequired();

            builder.Property(c => c.CreatedBy)
                .HasMaxLength(256);

            builder.Property(c => c.UpdatedAt);

            builder.Property(c => c.UpdatedBy)
                .HasMaxLength(256);

            // Navigation properties
            builder.HasMany(c => c.Projects)
                .WithOne()
                .HasForeignKey("CurrencyCode")
                .HasPrincipalKey(c => c.Code)
                .OnDelete(DeleteBehavior.Restrict);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Currencies_ExchangeRate", "[ExchangeRate] > 0");
                t.HasCheckConstraint("CK_Currencies_DecimalDigits", "[DecimalDigits] >= 0 AND [DecimalDigits] <= 4");
            });
        }
    }
}