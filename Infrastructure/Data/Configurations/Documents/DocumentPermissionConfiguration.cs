using Domain.Entities.Documents.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Documents;

/// <summary>
/// Entity configuration for DocumentPermission
/// </summary>
public class DocumentPermissionConfiguration : IEntityTypeConfiguration<DocumentPermission>
{
    public void Configure(EntityTypeBuilder<DocumentPermission> builder)
    {
        // Table name and schema
        builder.ToTable("DocumentPermissions", "Documents");

        // Primary key
        builder.HasKey(dp => dp.Id);

        // Indexes
        builder.HasIndex(dp => dp.DocumentId);
        builder.HasIndex(dp => dp.UserId);
        builder.HasIndex(dp => dp.RoleId);
        builder.HasIndex(dp => dp.GrantedById);
        builder.HasIndex(dp => dp.GrantedDate);
        builder.HasIndex(dp => dp.ValidFrom);
        builder.HasIndex(dp => dp.ValidTo);
        builder.HasIndex(dp => new { dp.DocumentId, dp.UserId }).IsUnique().HasFilter("[UserId] IS NOT NULL");
        builder.HasIndex(dp => new { dp.DocumentId, dp.RoleId }).IsUnique().HasFilter("[RoleId] IS NOT NULL");

        // Permission can be for a user or a role
        builder.Property(dp => dp.RoleName)
            .HasMaxLength(128);

        // Permission levels
        builder.Property(dp => dp.CanView)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(dp => dp.CanDownload)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(dp => dp.CanEdit)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(dp => dp.CanDelete)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(dp => dp.CanComment)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(dp => dp.CanDistribute)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(dp => dp.CanManagePermissions)
            .IsRequired()
            .HasDefaultValue(false);

        // Grant information
        builder.Property(dp => dp.GrantedDate)
            .IsRequired();

        builder.Property(dp => dp.Comments)
            .HasMaxLength(1000);

        // Audit properties
        builder.Property(dp => dp.CreatedAt)
            .IsRequired();

        builder.Property(dp => dp.CreatedBy)
            .HasMaxLength(256);

        builder.Property(dp => dp.UpdatedAt);

        builder.Property(dp => dp.UpdatedBy)
            .HasMaxLength(256);

        // Relationships
        builder.HasOne(dp => dp.Document)
            .WithMany(d => d.Permissions)
            .HasForeignKey(dp => dp.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(dp => dp.User)
            .WithMany()
            .HasForeignKey(dp => dp.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Check constraints
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_DocumentPermissions_UserOrRole", 
                "[UserId] IS NOT NULL OR [RoleId] IS NOT NULL");
            t.HasCheckConstraint("CK_DocumentPermissions_ValidPeriod", 
                "[ValidFrom] IS NULL OR [ValidTo] IS NULL OR [ValidTo] > [ValidFrom]");
        });
    }
}