using Domain.Entities.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Security;

/// <summary>
/// Entity configuration for User
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Table name and schema
        builder.ToTable("Users", "Security");

        // Primary key
        builder.HasKey(u => u.Id);

        // Indexes
        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.EntraId).IsUnique();
        builder.HasIndex(u => new { u.IsDeleted, u.IsActive });
        builder.HasIndex(u => u.IsDeleted);

        // Properties
        builder.Property(u => u.EntraId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(u => u.GivenName)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(u => u.Surname)
            .HasMaxLength(128);

        builder.Property(u => u.Name)
            .HasMaxLength(256);

        builder.Property(u => u.JobTitle)
            .HasMaxLength(256);
        builder.Property(u => u.PhotoUrl)
            .HasMaxLength(1500);


        builder.Property(u => u.PreferredLanguage)
            .HasMaxLength(10)
            .HasDefaultValue("en");

        // Audit properties
        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.Property(u => u.CreatedBy)
            .HasMaxLength(256);

        builder.Property(u => u.UpdatedAt);

        builder.Property(u => u.UpdatedBy)
            .HasMaxLength(256);

        // Soft delete properties
        builder.Property(u => u.DeletedAt);

        builder.Property(u => u.DeletedBy)
            .HasMaxLength(256);

        // Navigation properties
        builder.HasMany(u => u.ProjectTeamMembers)
            .WithOne(ptm => ptm.User)
            .HasForeignKey(ptm => ptm.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Global query filter for soft delete
        builder.HasQueryFilter(u => !u.IsDeleted);
    }
}