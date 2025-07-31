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
        // Table name
        builder.ToTable("Users", "Security");

        // Primary key
        builder.HasKey(u => u.Id);

        // Indexes
        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.EntraId).IsUnique();
        builder.HasIndex(u => new { u.IsDeleted, u.IsActive });

        // Properties
        builder.Property(u => u.EntraId)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(u => u.GivenName)
            .HasMaxLength(128);

        builder.Property(u => u.Surname)
            .HasMaxLength(128);

        builder.Property(u => u.PhoneNumber)
            .HasMaxLength(50);

        builder.Property(u => u.PreferredLanguage)
            .HasMaxLength(10);

        builder.Property(u => u.CreatedBy)
            .HasMaxLength(256);

        builder.Property(u => u.UpdatedBy)
            .HasMaxLength(256);

        builder.Property(u => u.DeletedBy)
            .HasMaxLength(256);

        // Relationships
        builder.HasMany(u => u.ProjectTeamMembers)
            .WithOne(ur => ur.User)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
