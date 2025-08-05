using Domain.Entities.Documents.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Documents;

/// <summary>
/// Entity configuration for DocumentVersion
/// </summary>
public class DocumentVersionConfiguration : IEntityTypeConfiguration<DocumentVersion>
{
    public void Configure(EntityTypeBuilder<DocumentVersion> builder)
    {
        // Table name and schema
        builder.ToTable("DocumentVersions", "Documents");

        // Primary key
        builder.HasKey(dv => dv.Id);

        // Indexes
        builder.HasIndex(dv => dv.DocumentId);
        builder.HasIndex(dv => new { dv.DocumentId, dv.Version, dv.RevisionNumber }).IsUnique();
        builder.HasIndex(dv => dv.IsCurrent);
        builder.HasIndex(dv => dv.IsSuperseded);
        builder.HasIndex(dv => dv.IssueDate);
        builder.HasIndex(dv => dv.ReviewStatus);
        builder.HasIndex(dv => dv.BlobName);

        // Basic Properties
        builder.Property(dv => dv.Version)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(dv => dv.RevisionNumber)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(dv => dv.IssueDate)
            .IsRequired();

        builder.Property(dv => dv.IssuePurpose)
            .HasMaxLength(256);

        builder.Property(dv => dv.VersionComments)
            .HasMaxLength(2000);

        // File information
        builder.Property(dv => dv.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(dv => dv.FileExtension)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(dv => dv.FileSize)
            .IsRequired();

        builder.Property(dv => dv.ContentType)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(dv => dv.FileHash)
            .HasMaxLength(64); // SHA256 hash length

        // Azure Blob Storage information
        builder.Property(dv => dv.BlobContainerName)
            .IsRequired()
            .HasMaxLength(63); // Azure Blob container name max length

        builder.Property(dv => dv.BlobName)
            .IsRequired()
            .HasMaxLength(1024); // Azure Blob name max length

        builder.Property(dv => dv.BlobStorageUrl)
            .HasMaxLength(2048);

        // Version metadata
        builder.Property(dv => dv.UploadedDate)
            .IsRequired();

        builder.Property(dv => dv.IsCurrent)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(dv => dv.IsSuperseded)
            .IsRequired()
            .HasDefaultValue(false);

        // Review status
        builder.Property(dv => dv.ReviewStatus)
            .IsRequired();

        builder.Property(dv => dv.ReviewComments)
            .HasMaxLength(2000);

        // Statistics
        builder.Property(dv => dv.DownloadCount)
            .IsRequired()
            .HasDefaultValue(0);

        // Audit properties
        builder.Property(dv => dv.CreatedAt)
            .IsRequired();

        builder.Property(dv => dv.CreatedBy)
            .HasMaxLength(256);

        builder.Property(dv => dv.UpdatedAt);

        builder.Property(dv => dv.UpdatedBy)
            .HasMaxLength(256);

        // Relationships
        builder.HasOne(dv => dv.Document)
            .WithMany(d => d.Versions)
            .HasForeignKey(dv => dv.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(dv => dv.UploadedBy)
            .WithMany()
            .HasForeignKey(dv => dv.UploadedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(dv => dv.ReviewedBy)
            .WithMany()
            .HasForeignKey(dv => dv.ReviewedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(dv => dv.SupersededBy)
            .WithMany()
            .HasForeignKey(dv => dv.SupersededById)
            .OnDelete(DeleteBehavior.Restrict);

        // Navigation properties
        builder.HasMany(dv => dv.Distributions)
            .WithOne(d => d.DocumentVersion)
            .HasForeignKey(d => d.DocumentVersionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(dv => dv.Comments)
            .WithOne(c => c.DocumentVersion)
            .HasForeignKey(c => c.DocumentVersionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(dv => dv.TransmittalDocuments)
            .WithOne(td => td.DocumentVersion)
            .HasForeignKey(td => td.DocumentVersionId)
            .OnDelete(DeleteBehavior.Restrict);

        // Check constraints
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_DocumentVersions_FileSize", 
                "[FileSize] > 0");
            t.HasCheckConstraint("CK_DocumentVersions_RevisionNumber", 
                "[RevisionNumber] >= 0");
            t.HasCheckConstraint("CK_DocumentVersions_DownloadCount", 
                "[DownloadCount] >= 0");
        });
    }
}