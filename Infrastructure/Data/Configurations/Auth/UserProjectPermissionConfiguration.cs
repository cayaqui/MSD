using Domain.Entities.Auth.Permissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Auth;

/// <summary>
/// Entity configuration for UserProjectPermission
/// </summary>
public class UserProjectPermissionConfiguration : IEntityTypeConfiguration<UserProjectPermission>
{
    public void Configure(EntityTypeBuilder<UserProjectPermission> builder)
    {
        // Table name and schema
        builder.ToTable("UserProjectPermissions", "Security", t=>
        {
            t.HasCheckConstraint("CK_UserProjectPermissions_ValidDates",
            "[ExpiresAt] IS NULL OR [ExpiresAt] > [GrantedAt]");
        });

        // Primary key
        builder.HasKey(upp => upp.Id);

        // Indexes
        builder.HasIndex(upp => new { upp.UserId, upp.ProjectId, upp.PermissionCode }).IsUnique();
        builder.HasIndex(upp => upp.UserId);
        builder.HasIndex(upp => upp.ProjectId);
        builder.HasIndex(upp => upp.PermissionId);
        builder.HasIndex(upp => new { upp.IsActive, upp.GrantedAt, upp.ExpiresAt});

        // Properties
        builder.Property(upp => upp.GrantedAt)
            .IsRequired();

        builder.Property(upp => upp.ExpiresAt);

        builder.Property(upp => upp.PermissionCode)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(upp => upp.GrantedBy)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(upp => upp.Reason)
            .HasMaxLength(500);

        // Audit properties
        builder.Property(upp => upp.CreatedAt)
            .IsRequired();

        builder.Property(upp => upp.CreatedBy)
            .HasMaxLength(256);

        builder.Property(upp => upp.UpdatedAt);

        builder.Property(upp => upp.UpdatedBy)
            .HasMaxLength(256);

        builder.Property(upp => upp.RevokedBy)
            .HasMaxLength(256);

        builder.Property(upp => upp.RevokedAt);

        // Foreign key relationships
        builder.HasOne(upp => upp.User)
            .WithMany(u => u.UserProjectPermissions)
            .HasForeignKey(upp => upp.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(upp => upp.Project)
            .WithMany(p => p.UserProjectPermissions)
            .HasForeignKey(upp => upp.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(upp => upp.Permission)
            .WithMany(p => p.UserProjectPermissions)
            .HasForeignKey(upp => upp.PermissionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}