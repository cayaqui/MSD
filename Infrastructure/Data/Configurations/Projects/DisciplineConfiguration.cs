using Domain.Entities.Setup;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Setup;

/// <summary>
/// Entity configuration for Discipline
/// </summary>
public class DisciplineConfiguration : IEntityTypeConfiguration<Discipline>
{
    public void Configure(EntityTypeBuilder<Discipline> builder)
    {
        // Table name and schema
        builder.ToTable("Disciplines", "Setup");

        // Primary key
        builder.HasKey(d => d.Id);

        // Indexes
        builder.HasIndex(d => d.Code).IsUnique();
        builder.HasIndex(d => d.Name);
        builder.HasIndex(d => d.IsDeleted);

        // Properties
        builder.Property(d => d.Code)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(d => d.Description)
            .HasMaxLength(500);

        builder.Property(d => d.ColorHex)
            .HasMaxLength(7); // For hex color codes #FFFFFF

        builder.Property(d => d.Icon)
            .HasMaxLength(50);

        builder.Property(d => d.Order)
            .HasDefaultValue(0);

        // Audit properties
        builder.Property(d => d.CreatedAt)
            .IsRequired();

        builder.Property(d => d.CreatedBy)
            .HasMaxLength(256);

        builder.Property(d => d.UpdatedAt);

        builder.Property(d => d.UpdatedBy)
            .HasMaxLength(256);

        // Soft delete properties
        builder.Property(d => d.DeletedAt);

        builder.Property(d => d.DeletedBy)
            .HasMaxLength(256);

        // Navigation properties
        builder.HasMany(d => d.WorkPackageDisciplines)
            .WithOne(wp => wp.Discipline)
            .HasForeignKey(wp => wp.DisciplineId)
            .OnDelete(DeleteBehavior.SetNull);


        // Global query filter for soft delete
        builder.HasQueryFilter(d => !d.IsDeleted);
    }
}