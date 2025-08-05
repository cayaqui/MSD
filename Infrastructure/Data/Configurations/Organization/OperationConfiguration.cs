using Domain.Entities.Organization.Core;

namespace Infrastructure.Data.Configurations.Setup;

/// <summary>
/// Entity configuration for Operation
/// </summary>
public class OperationConfiguration : IEntityTypeConfiguration<Operation>
{
    public void Configure(EntityTypeBuilder<Operation> builder)
    {
        // Table name
        builder.ToTable("Operations", "Organization");

        // Primary key
        builder.HasKey(o => o.Id);

        // Indexes
        builder.HasIndex(o => new { o.CompanyId, o.Code }).IsUnique();
        builder.HasIndex(o => o.IsDeleted);

        // Properties
        builder.Property(o => o.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(o => o.Name)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(o => o.Description)
            .HasMaxLength(500);

        builder.Property(o => o.Location)
            .HasMaxLength(256);

        builder.Property(o => o.Address)
            .HasMaxLength(500);

        builder.Property(o => o.City)
            .HasMaxLength(100);

        builder.Property(o => o.State)
            .HasMaxLength(100);

        builder.Property(o => o.Country)
            .HasMaxLength(100);

        builder.Property(o => o.ManagerName)
            .HasMaxLength(256);

        builder.Property(o => o.ManagerEmail)
            .HasMaxLength(256);

        builder.Property(o => o.CostCenter)
            .HasMaxLength(50);

        // Relationships
        builder.HasOne(o => o.Company)
            .WithMany(c => c.Operations)
            .HasForeignKey(o => o.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(o => o.Projects)
            .WithOne(p => p.Operation)
            .HasForeignKey(p => p.OperationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}