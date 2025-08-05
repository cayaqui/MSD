using Domain.Entities.Documents.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Documents;

/// <summary>
/// Entity configuration for Document
/// </summary>
public class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        // Table name and schema
        builder.ToTable("Documents", "Documents");

        // Primary key
        builder.HasKey(d => d.Id);

        // Indexes
        builder.HasIndex(d => d.DocumentNumber).IsUnique();
        builder.HasIndex(d => d.ProjectId);
        builder.HasIndex(d => d.Type);
        builder.HasIndex(d => d.Category);
        builder.HasIndex(d => d.Status);  
        builder.HasIndex(d => new { d.ProjectId, d.Status });
        builder.HasIndex(d => new { d.Type, d.Status });
        builder.HasIndex(d => d.IsDeleted);
        builder.HasIndex(d => d.CurrentVersionId);
        builder.HasIndex(d => d.ReviewStatus);
        builder.HasIndex(d => d.ReviewDueDate);

        // Basic Properties
        builder.Property(d => d.DocumentNumber)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(d => d.Description)
            .HasMaxLength(2000);

        builder.Property(d => d.Type)
            .IsRequired();

        builder.Property(d => d.Category)
            .IsRequired();

        builder.Property(d => d.Status)
            .IsRequired();

        // Version control
        builder.Property(d => d.CurrentVersion)
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue("A");

        builder.Property(d => d.CurrentRevisionNumber)
            .IsRequired()
            .HasDefaultValue(0);

        // Metadata
        builder.Property(d => d.Author)
            .HasMaxLength(256);

        builder.Property(d => d.Originator)
            .HasMaxLength(256);

        builder.Property(d => d.ClientDocumentNumber)
            .HasMaxLength(100);

        builder.Property(d => d.ContractorDocumentNumber)
            .HasMaxLength(100);

        builder.Property(d => d.Confidentiality)
            .IsRequired();

        // Review and approval
        builder.Property(d => d.ReviewStatus)
            .IsRequired();

        builder.Property(d => d.ReviewComments)
            .HasMaxLength(2000);

        // Access control
        builder.Property(d => d.IsPublic)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(d => d.IsLocked)
            .IsRequired()
            .HasDefaultValue(false);

        // Tags and keywords (stored as JSON)
        builder.Property(d => d.TagsJson)
            .IsRequired()
            .HasDefaultValue("[]")
            .HasColumnType("nvarchar(max)");

        builder.Property(d => d.Keywords)
            .HasMaxLength(1000);

        // Retention
        builder.Property(d => d.RetentionPolicy)
            .HasMaxLength(256);

        // Statistics
        builder.Property(d => d.DownloadCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(d => d.ViewCount)
            .IsRequired()
            .HasDefaultValue(0);

        // Soft Delete
        builder.Property(d => d.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(d => d.DeletedBy)
            .HasMaxLength(256);

        // Audit properties
        builder.Property(d => d.CreatedAt)
            .IsRequired();

        builder.Property(d => d.CreatedBy)
            .HasMaxLength(256);

        builder.Property(d => d.UpdatedAt);

        builder.Property(d => d.UpdatedBy)
            .HasMaxLength(256);

        // Relationships
        builder.HasOne(d => d.Project)
            .WithMany()
            .HasForeignKey(d => d.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.Discipline)
            .WithMany()
            .HasForeignKey(d => d.DisciplineId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.Phase)
            .WithMany()
            .HasForeignKey(d => d.PhaseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.Package)
            .WithMany()
            .HasForeignKey(d => d.PackageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.AuthorUser)
            .WithMany()
            .HasForeignKey(d => d.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.ReviewedBy)
            .WithMany()
            .HasForeignKey(d => d.ReviewedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.LockedBy)
            .WithMany()
            .HasForeignKey(d => d.LockedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.CurrentVersionEntity)
            .WithMany()
            .HasForeignKey(d => d.CurrentVersionId)
            .OnDelete(DeleteBehavior.Restrict);

        // Navigation properties
        builder.HasMany(d => d.Versions)
            .WithOne(v => v.Document)
            .HasForeignKey(v => v.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(d => d.Distributions)
            .WithOne(dist => dist.Document)
            .HasForeignKey(dist => dist.DocumentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(d => d.Comments)
            .WithOne(c => c.Document)
            .HasForeignKey(c => c.DocumentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(d => d.RelatedDocuments)
            .WithOne(r => r.Document)
            .HasForeignKey(r => r.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(d => d.RelatedToDocuments)
            .WithOne(r => r.RelatedDocument)
            .HasForeignKey(r => r.RelatedDocumentId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(d => d.TransmittalDocuments)
            .WithOne(td => td.Document)
            .HasForeignKey(td => td.DocumentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(d => d.Permissions)
            .WithOne(p => p.Document)
            .HasForeignKey(p => p.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Check constraints
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_Documents_Statistics", 
                "[DownloadCount] >= 0 AND [ViewCount] >= 0");
            t.HasCheckConstraint("CK_Documents_Version", 
                "[CurrentRevisionNumber] >= 0");
        });

        // Global query filter for soft delete
        builder.HasQueryFilter(d => !d.IsDeleted);
    }
}