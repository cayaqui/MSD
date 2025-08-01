using Domain.Entities.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Auth
{
    public class UserProjectPermissionConfiguration : IEntityTypeConfiguration<UserProjectPermission>
    {
        public void Configure(EntityTypeBuilder<UserProjectPermission> builder)
        {
            builder.ToTable("UserProjectPermissions", "Security");

            // Primary Key
            builder.HasKey(upp => upp.Id);

            // Properties
            builder.Property(upp => upp.PermissionCode)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(upp => upp.IsGranted)
                .IsRequired();

            builder.Property(upp => upp.Reason)
                .HasMaxLength(500);

            builder.Property(upp => upp.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(upp => upp.GrantedAt)
                .IsRequired();

            builder.Property(upp => upp.GrantedBy)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(upp => upp.RevokedBy)
                .HasMaxLength(450);

            // Relationships
            builder.HasOne(upp => upp.User)
                .WithMany()
                .HasForeignKey(upp => upp.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(upp => upp.Project)
                .WithMany()
                .HasForeignKey(upp => upp.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(upp => upp.Permission)
                .WithMany(p => p.UserProjectPermissions)
                .HasForeignKey(upp => upp.PermissionId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            // Indexes
            builder.HasIndex(upp => new { upp.UserId, upp.ProjectId, upp.PermissionCode })
                .HasDatabaseName("IX_UserProjectPermissions_User_Project_Permission")
                .IsUnique()
                .HasFilter("[IsActive] = 1");

            builder.HasIndex(upp => upp.UserId)
                .HasDatabaseName("IX_UserProjectPermissions_UserId");

            builder.HasIndex(upp => upp.ProjectId)
                .HasDatabaseName("IX_UserProjectPermissions_ProjectId");

            builder.HasIndex(upp => upp.PermissionCode)
                .HasDatabaseName("IX_UserProjectPermissions_PermissionCode");

            builder.HasIndex(upp => upp.IsActive)
                .HasDatabaseName("IX_UserProjectPermissions_IsActive");

            builder.HasIndex(upp => upp.ExpiresAt)
                .HasDatabaseName("IX_UserProjectPermissions_ExpiresAt")
                .HasFilter("[ExpiresAt] IS NOT NULL");

            // Query Filters
            builder.HasQueryFilter(upp => upp.IsActive);
        }
    }
}