using Domain.Entities.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Security;

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
            "[ValidTo] IS NULL OR [ValidTo] > [ValidFrom]");
        });

        // Primary key
        builder.HasKey(upp => upp.Id);

        // Indexes
        builder.HasIndex(upp => new { upp.UserId, upp.ProjectId, upp.PermissionId }).IsUnique();
        builder.HasIndex(upp => upp.UserId);
        builder.HasIndex(upp => upp.ProjectId);
        builder.HasIndex(upp => upp.PermissionId);
        builder.HasIndex(upp => new { upp.IsActive, upp.GrantedAt, upp.ExpiresAt});

        // Properties
        builder.Property(upp => upp.GrantedAt)
            .IsRequired();

        builder.Property(upp => upp.ExpiresAt);

        builder.Property(upp => upp.GrantedBy)
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

        // Foreign key relationships
        builder.HasOne(upp => upp.User)
            .WithMany(u => u.UserProjectPermissions)
            .HasForeignKey(upp => upp.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(upp => upp.Project)
            .WithMany(p => p.UserProjectPermissions)
            .HasForeignKey(upp => upp.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(upp => upp.Permission)
            .WithMany(p => p.UserProjectPermissions)
            .HasForeignKey(upp => upp.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}