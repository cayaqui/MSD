using Domain.Entities.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Security
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users", "Security");

            // Primary Key
            builder.HasKey(u => u.Id);

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

            builder.Property(u => u.JobTitle)
                .HasMaxLength(100);

            builder.Property(u => u.CompanyId)
                .HasMaxLength(50);

            builder.Property(u => u.PreferredLanguage)
                .HasMaxLength(10)
                .HasDefaultValue("en");

            builder.Property(u => u.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(u => u.LoginCount)
                .IsRequired()
                .HasDefaultValue(0);

            // Soft Delete properties
            builder.Property(u => u.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(u => u.DeletedBy)
                .HasMaxLength(450);

            // Audit properties
            builder.Property(u => u.CreatedBy)
                .HasMaxLength(450);

            builder.Property(u => u.UpdatedBy)
                .HasMaxLength(450);

            // Relationships
            builder.HasMany(u => u.ProjectTeamMembers)
                .WithOne(ptm => ptm.User)
                .HasForeignKey(ptm => ptm.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.UserProjectPermissions)
                .WithOne(upp => upp.User)
                .HasForeignKey(upp => upp.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(u => u.EntraId)
                .IsUnique()
                .HasDatabaseName("IX_Users_EntraId");

            builder.HasIndex(u => u.Email)
                .IsUnique()
                .HasDatabaseName("IX_Users_Email");

            builder.HasIndex(u => new { u.Email, u.IsActive })
                .HasDatabaseName("IX_Users_Email_IsActive");

            builder.HasIndex(u => u.IsActive)
                .HasDatabaseName("IX_Users_IsActive");

            builder.HasIndex(u => u.IsDeleted)
                .HasDatabaseName("IX_Users_IsDeleted");

            builder.HasIndex(u => u.LastLoginAt)
                .HasDatabaseName("IX_Users_LastLoginAt");

            builder.HasIndex(u => u.CompanyId)
                .HasDatabaseName("IX_Users_CompanyId")
                .HasFilter("[CompanyId] IS NOT NULL");

            // Query Filters
            builder.HasQueryFilter(u => !u.IsDeleted);

            // Computed columns (optional - for performance)
            builder.Property(u => u.Name)
                .HasComputedColumnSql("CASE WHEN [GivenName] IS NOT NULL OR [Surname] IS NOT NULL THEN TRIM(ISNULL([GivenName], '') + ' ' + ISNULL([Surname], '')) ELSE [Name] END", stored: false);

            // Value conversions
            builder.Property(u => u.Email)
                .HasConversion(
                    v => v.ToLowerInvariant(),
                    v => v);
        }

    }
}