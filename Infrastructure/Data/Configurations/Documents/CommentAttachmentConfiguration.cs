using Domain.Entities.Documents.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Documents;

/// <summary>
/// Entity configuration for CommentAttachment
/// </summary>
public class CommentAttachmentConfiguration : IEntityTypeConfiguration<CommentAttachment>
{
    public void Configure(EntityTypeBuilder<CommentAttachment> builder)
    {
        // Table name and schema
        builder.ToTable("CommentAttachments", "Documents");

        // Primary key
        builder.HasKey(ca => ca.Id);

        // Indexes
        builder.HasIndex(ca => ca.CommentId);
        builder.HasIndex(ca => ca.BlobName);
        builder.HasIndex(ca => ca.UploadedDate);

        // File information
        builder.Property(ca => ca.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(ca => ca.FileExtension)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(ca => ca.FileSize)
            .IsRequired();

        builder.Property(ca => ca.ContentType)
            .IsRequired()
            .HasMaxLength(255);

        // Azure Blob Storage
        builder.Property(ca => ca.BlobContainerName)
            .IsRequired()
            .HasMaxLength(63); // Azure Blob container name max length

        builder.Property(ca => ca.BlobName)
            .IsRequired()
            .HasMaxLength(1024); // Azure Blob name max length

        builder.Property(ca => ca.BlobStorageUrl)
            .HasMaxLength(2048);

        builder.Property(ca => ca.UploadedDate)
            .IsRequired();

        // Audit properties
        builder.Property(ca => ca.CreatedAt)
            .IsRequired();

        builder.Property(ca => ca.CreatedBy)
            .HasMaxLength(256);

        builder.Property(ca => ca.UpdatedAt);

        builder.Property(ca => ca.UpdatedBy)
            .HasMaxLength(256);

        // Relationships
        builder.HasOne(ca => ca.Comment)
            .WithMany(c => c.Attachments)
            .HasForeignKey(ca => ca.CommentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Check constraints
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_CommentAttachments_FileSize", 
                "[FileSize] > 0");
        });
    }
}