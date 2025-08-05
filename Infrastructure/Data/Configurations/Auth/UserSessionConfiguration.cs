using Domain.Entities.Auth.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Auth;

/// <summary>
/// Entity configuration for UserSession
/// </summary>
public class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
{
    public void Configure(EntityTypeBuilder<UserSession> builder)
    {
        // Table name and schema
        builder.ToTable("UserSessions", "Security");

        // Primary key
        builder.HasKey(us => us.Id);

        // Indexes
        builder.HasIndex(us => us.SessionId).IsUnique();
        builder.HasIndex(us => us.UserId);
        builder.HasIndex(us => new { us.UserId, us.IsActive });
        builder.HasIndex(us => us.StartedAt);
        builder.HasIndex(us => us.LastActivityAt);

        // Properties
        builder.Property(us => us.SessionId)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(us => us.StartedAt)
            .IsRequired();

        builder.Property(us => us.LastActivityAt)
            .IsRequired();

        builder.Property(us => us.IpAddress)
            .HasMaxLength(45); // IPv6 max length

        builder.Property(us => us.UserAgent)
            .HasMaxLength(500);

        builder.Property(us => us.DeviceInfo)
            .HasMaxLength(500);

        builder.Property(us => us.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Audit properties
        builder.Property(us => us.CreatedAt)
            .IsRequired();

        builder.Property(us => us.CreatedBy)
            .HasMaxLength(256);

        builder.Property(us => us.UpdatedAt);

        builder.Property(us => us.UpdatedBy)
            .HasMaxLength(256);

        // Relationships
        builder.HasOne(us => us.User)
            .WithMany()
            .HasForeignKey(us => us.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}