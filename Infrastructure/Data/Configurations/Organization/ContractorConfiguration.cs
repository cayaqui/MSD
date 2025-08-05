using Domain.Entities.Organization.Core;

namespace Infrastructure.Data.Configurations.Setup;

/// <summary>
/// Entity configuration for Contractor
/// </summary>
public class ContractorConfiguration : IEntityTypeConfiguration<Contractor>
{
    public void Configure(EntityTypeBuilder<Contractor> builder)
    {
        // Table name and schema with check constraints
        builder.ToTable("Contractors", "Organization", t =>
        {
            t.HasCheckConstraint("CK_Contractors_CreditLimit",
                "[CreditLimit] >= 0");
            t.HasCheckConstraint("CK_Contractors_PerformanceRating",
                "[PerformanceRating] >= 0 AND [PerformanceRating] <= 5 OR [PerformanceRating] IS NULL");
            t.HasCheckConstraint("CK_Contractors_InsuranceAmount",
                "[InsuranceAmount] >= 0 OR [InsuranceAmount] IS NULL");
        });

        // Primary key
        builder.HasKey(c => c.Id);

        // Indexes
        builder.HasIndex(c => c.Code).IsUnique();
        builder.HasIndex(c => c.TaxId).IsUnique();
        builder.HasIndex(c => c.Name);
        builder.HasIndex(c => c.Type);
        builder.HasIndex(c => c.Classification);
        builder.HasIndex(c => c.Status);
        builder.HasIndex(c => c.IsDeleted);
        builder.HasIndex(c => c.IsActive);
        builder.HasIndex(c => c.IsPrequalified);

        // Properties
        builder.Property(c => c.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(c => c.TradeName)
            .HasMaxLength(256);

        builder.Property(c => c.TaxId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.Type)
            .IsRequired();

        builder.Property(c => c.Classification)
            .IsRequired();

        // Contact Information
        builder.Property(c => c.ContactName)
            .HasMaxLength(100);

        builder.Property(c => c.ContactTitle)
            .HasMaxLength(100);

        builder.Property(c => c.Email)
            .HasMaxLength(256);

        builder.Property(c => c.Phone)
            .HasMaxLength(50);

        builder.Property(c => c.MobilePhone)
            .HasMaxLength(50);

        builder.Property(c => c.Website)
            .HasMaxLength(256);

        // Address
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

        // Financial Information
        builder.Property(c => c.BankName)
            .HasMaxLength(100);

        builder.Property(c => c.BankAccount)
            .HasMaxLength(100);

        builder.Property(c => c.BankRoutingNumber)
            .HasMaxLength(50);

        builder.Property(c => c.PaymentTerms)
            .HasMaxLength(200);

        builder.Property(c => c.CreditLimit)
            .HasPrecision(18, 2)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(c => c.DefaultCurrency)
            .IsRequired()
            .HasMaxLength(3)
            .HasDefaultValue("USD");

        // Qualification
        builder.Property(c => c.IsPrequalified)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(c => c.PrequalificationNotes)
            .HasMaxLength(1000);

        builder.Property(c => c.Status)
            .IsRequired();

        builder.Property(c => c.PerformanceRating)
            .HasPrecision(3, 2);

        // Insurance & Compliance
        builder.Property(c => c.HasInsurance)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(c => c.InsuranceAmount)
            .HasPrecision(18, 2);

        builder.Property(c => c.InsuranceCompany)
            .HasMaxLength(100);

        builder.Property(c => c.InsurancePolicyNumber)
            .HasMaxLength(100);

        // Certifications - JSON fields
        builder.Property(c => c.Certifications)
            .HasMaxLength(2000);

        builder.Property(c => c.SpecialtyAreas)
            .HasMaxLength(2000);

        // Audit properties
        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.CreatedBy)
            .HasMaxLength(256);

        builder.Property(c => c.UpdatedAt);

        builder.Property(c => c.UpdatedBy)
            .HasMaxLength(256);

        // Soft delete properties
        builder.Property(c => c.DeletedAt);

        builder.Property(c => c.DeletedBy)
            .HasMaxLength(256);

        // Activatable
        builder.Property(c => c.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Navigation properties
        builder.HasMany(c => c.Commitments)
            .WithOne(co => co.Contractor)
            .HasForeignKey(co => co.ContractorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Invoices)
            .WithOne(i => i.Contractor)
            .HasForeignKey(i => i.ContractorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Global query filter for soft delete
        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}