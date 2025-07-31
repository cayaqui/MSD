using Domain.Entities.Setup;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Setup;

/// <summary>
/// Entity configuration for Company
/// </summary>
public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        // Table name
        builder.ToTable("Companies", "Setup");

        // Primary key
        builder.HasKey(c => c.Id);

        // Indexes
        builder.HasIndex(c => c.Code).IsUnique();
        builder.HasIndex(c => c.TaxId).IsUnique();
        builder.HasIndex(c => c.IsDeleted);

        // Properties
        builder.Property(c => c.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(c => c.Description)
            .HasMaxLength(500);

        builder.Property(c => c.TaxId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.Address)
            .HasMaxLength(500);

        builder.Property(c => c.City)
            .HasMaxLength(100);

        builder.Property(c => c.State)
            .HasMaxLength(100);

        builder.Property(c => c.Country)
            .HasMaxLength(100);

        builder.Property(c => c.PostalCode)
            .HasMaxLength(20);

        builder.Property(c => c.Phone)
            .HasMaxLength(50);

        builder.Property(c => c.Email)
            .HasMaxLength(256);

        builder.Property(c => c.Website)
            .HasMaxLength(256);

        builder.Property(c => c.DefaultCurrency)
            .IsRequired()
            .HasMaxLength(3)
            .HasDefaultValue("USD");

        builder.Property(c => c.FiscalYearStart)
            .HasMaxLength(5); // MM-DD format

        builder.Property(c => c.LogoContentType)
            .HasMaxLength(50);

        // Relationships
        builder.HasMany(c => c.Operations)
            .WithOne(o => o.Company)
            .HasForeignKey(o => o.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}