using Domain.Entities.Cost.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Cost
{
    public class ExchangeRateConfiguration : IEntityTypeConfiguration<ExchangeRate>
    {
        public void Configure(EntityTypeBuilder<ExchangeRate> builder)
        {
            // Table name and schema
            builder.ToTable("ExchangeRates", "Cost");

            // Primary key
            builder.HasKey(e => e.Id);

            // Indexes
            builder.HasIndex(e => e.ProjectId);
            builder.HasIndex(e => e.CurrencyFrom);
            builder.HasIndex(e => e.CurrencyTo);
            builder.HasIndex(e => e.Date);
            builder.HasIndex(e => e.IsOfficial);
            builder.HasIndex(e => new { e.ProjectId, e.CurrencyFrom, e.CurrencyTo, e.Date });
            builder.HasIndex(e => new { e.CurrencyFrom, e.CurrencyTo, e.Date });

            // Currency Information
            builder.Property(e => e.CurrencyFrom)
                .IsRequired()
                .HasMaxLength(3);

            builder.Property(e => e.CurrencyTo)
                .IsRequired()
                .HasMaxLength(3);

            // Rate Information
            builder.Property(e => e.Rate)
                .IsRequired()
                .HasPrecision(12, 6);

            builder.Property(e => e.UFValue)
                .HasPrecision(12, 6);

            // Date
            builder.Property(e => e.Date)
                .IsRequired();

            // Source Information
            builder.Property(e => e.Source)
                .IsRequired()
                .HasMaxLength(100);

            // Status
            builder.Property(e => e.IsOfficial)
                .IsRequired()
                .HasDefaultValue(true);

            // Audit properties
            builder.Property(e => e.CreatedAt)
                .IsRequired();

            builder.Property(e => e.CreatedBy)
                .HasMaxLength(256);

            builder.Property(e => e.UpdatedAt);

            builder.Property(e => e.UpdatedBy)
                .HasMaxLength(256);

            // Relationships
            builder.HasOne(e => e.Project)
                .WithMany(p => p.ExchangeRates)
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_ExchangeRates_Rate", "[Rate] > 0");
                t.HasCheckConstraint("CK_ExchangeRates_UFValue", "[UFValue] IS NULL OR [UFValue] > 0");
                t.HasCheckConstraint("CK_ExchangeRates_Currencies", 
                    "[CurrencyFrom] != [CurrencyTo]");
            });
        }
    }
}