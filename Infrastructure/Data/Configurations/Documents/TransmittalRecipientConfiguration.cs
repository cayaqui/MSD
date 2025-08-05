using Domain.Entities.Documents.Transmittals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Documents;

/// <summary>
/// Entity configuration for TransmittalRecipient
/// </summary>
public class TransmittalRecipientConfiguration : IEntityTypeConfiguration<TransmittalRecipient>
{
    public void Configure(EntityTypeBuilder<TransmittalRecipient> builder)
    {
        // Table name and schema
        builder.ToTable("TransmittalRecipients", "Documents");

        // Primary key
        builder.HasKey(tr => tr.Id);

        // Indexes
        builder.HasIndex(tr => tr.TransmittalId);
        builder.HasIndex(tr => tr.UserId);
        builder.HasIndex(tr => tr.CompanyId);
        builder.HasIndex(tr => tr.Type);
        builder.HasIndex(tr => tr.RequiresAcknowledgment);
        builder.HasIndex(tr => tr.IsAcknowledged);
        builder.HasIndex(tr => tr.IsDelivered);
        builder.HasIndex(tr => tr.Email);
        builder.HasIndex(tr => new { tr.TransmittalId, tr.Type });

        // Basic Properties
        builder.Property(tr => tr.Type)
            .IsRequired()
            .HasMaxLength(50);

        // Recipient information
        builder.Property(tr => tr.Email)
            .HasMaxLength(256);

        builder.Property(tr => tr.Role)
            .HasMaxLength(128);

        builder.Property(tr => tr.Name)
            .HasMaxLength(256);

        // Acknowledgment
        builder.Property(tr => tr.RequiresAcknowledgment)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(tr => tr.IsAcknowledged)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(tr => tr.AcknowledgmentComments)
            .HasMaxLength(1000);

        builder.Property(tr => tr.AcknowledgmentToken)
            .HasMaxLength(128);

        // Delivery status
        builder.Property(tr => tr.IsDelivered)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(tr => tr.DeliveryStatus)
            .HasMaxLength(100);

        builder.Property(tr => tr.DeliveryError)
            .HasMaxLength(500);

        // Audit properties
        builder.Property(tr => tr.CreatedAt)
            .IsRequired();

        builder.Property(tr => tr.CreatedBy)
            .HasMaxLength(256);

        builder.Property(tr => tr.UpdatedAt);

        builder.Property(tr => tr.UpdatedBy)
            .HasMaxLength(256);

        // Relationships
        builder.HasOne(tr => tr.Transmittal)
            .WithMany(t => t.Recipients)
            .HasForeignKey(tr => tr.TransmittalId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(tr => tr.User)
            .WithMany()
            .HasForeignKey(tr => tr.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(tr => tr.Company)
            .WithMany()
            .HasForeignKey(tr => tr.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        // Check constraints
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_TransmittalRecipients_Recipient", 
                "[UserId] IS NOT NULL OR [Email] IS NOT NULL OR [Name] IS NOT NULL");
        });
    }
}