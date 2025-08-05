using Domain.Entities.Documents.Transmittals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Documents;

/// <summary>
/// Entity configuration for TransmittalAttachment
/// </summary>
public class TransmittalAttachmentConfiguration : IEntityTypeConfiguration<TransmittalAttachment>
{
    public void Configure(EntityTypeBuilder<TransmittalAttachment> builder)
    {
        // Table name and schema
        builder.ToTable("TransmittalAttachments", "Documents");

        // Primary key
        builder.HasKey(ta => ta.Id);

        // Indexes
        builder.HasIndex(ta => ta.TransmittalId);
        builder.HasIndex(ta => ta.BlobName);
        builder.HasIndex(ta => ta.UploadedDate);
        builder.HasIndex(ta => ta.UploadedById);

        // File information
        builder.Property(ta => ta.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(ta => ta.FileExtension)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(ta => ta.FileSize)
            .IsRequired();

        builder.Property(ta => ta.ContentType)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(ta => ta.Description)
            .HasMaxLength(500);

        // Azure Blob Storage
        builder.Property(ta => ta.BlobContainerName)
            .IsRequired()
            .HasMaxLength(63); // Azure Blob container name max length

        builder.Property(ta => ta.BlobName)
            .IsRequired()
            .HasMaxLength(1024); // Azure Blob name max length

        builder.Property(ta => ta.BlobStorageUrl)
            .HasMaxLength(2048);

        builder.Property(ta => ta.UploadedDate)
            .IsRequired();

        // Audit properties
        builder.Property(ta => ta.CreatedAt)
            .IsRequired();

        builder.Property(ta => ta.CreatedBy)
            .HasMaxLength(256);

        builder.Property(ta => ta.UpdatedAt);

        builder.Property(ta => ta.UpdatedBy)
            .HasMaxLength(256);

        // Relationships
        builder.HasOne(ta => ta.Transmittal)
            .WithMany(t => t.Attachments)
            .HasForeignKey(ta => ta.TransmittalId)
            .OnDelete(DeleteBehavior.Cascade);

        // Check constraints
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_TransmittalAttachments_FileSize", 
                "[FileSize] > 0");
        });
    }
}