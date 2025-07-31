

namespace Infrastructure.Data.Configurations.UI;

/// <summary>
/// Entity configuration for Notification
/// </summary>
public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        // Table name
        builder.ToTable("Notifications", "UIUX", t =>
        {
            t.HasCheckConstraint("CK_Notification_ExpiresAt", "ExpiresAt >= CreatedAt");
            t.HasCheckConstraint("CK_Notification_ReadDate", "ReadAt >= CreatedAt");
        });

        // Primary key
        builder.HasKey(n => n.Id);

        // Indexes for performance
        builder.HasIndex(n => n.UserId);
        builder.HasIndex(n => new { n.UserId, n.Status });
        builder.HasIndex(n => new { n.UserId, n.IsImportant });
        builder.HasIndex(n => n.ProjectId);
        builder.HasIndex(n => n.CompanyId);
        builder.HasIndex(n => n.Type);
        builder.HasIndex(n => n.Priority);
        builder.HasIndex(n => n.Status);
        builder.HasIndex(n => n.CreatedAt);
        builder.HasIndex(n => n.ExpiresAt);
        builder.HasIndex(n => new { n.IsDeleted, n.Status });
        builder.HasIndex(n => new { n.UserId, n.Status, n.CreatedAt })
            .HasName("IX_Notifications_User_Status_Date");

        // Properties
        builder.Property(n => n.Title)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(n => n.Message)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(n => n.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(n => n.Priority)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(n => n.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(n => n.IsImportant)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(n => n.MetadataJson)
            .HasColumnType("ntext");

        builder.Property(n => n.ActionUrl)
            .HasMaxLength(500);

        // Soft Delete properties
        builder.Property(n => n.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(n => n.DeletedBy)
            .HasMaxLength(256);

        // Audit fields
        builder.Property(n => n.CreatedAt)
            .IsRequired();

        builder.Property(n => n.CreatedBy)
            .HasMaxLength(256);

        builder.Property(n => n.UpdatedBy)
            .HasMaxLength(256);

        // Relationships
        builder.HasOne(n => n.User)
            .WithMany()
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(n => n.Project)
            .WithMany()
            .HasForeignKey(n => n.ProjectId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(n => n.Company)
            .WithMany()
            .HasForeignKey(n => n.CompanyId)
            .OnDelete(DeleteBehavior.SetNull);

       
    }
}