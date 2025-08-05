using Domain.Entities.Documents.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Documents;

/// <summary>
/// Entity configuration for DocumentComment
/// </summary>
public class DocumentCommentConfiguration : IEntityTypeConfiguration<DocumentComment>
{
    public void Configure(EntityTypeBuilder<DocumentComment> builder)
    {
        // Table name and schema
        builder.ToTable("DocumentComments", "Documents");

        // Primary key
        builder.HasKey(dc => dc.Id);

        // Indexes
        builder.HasIndex(dc => dc.DocumentId);
        builder.HasIndex(dc => dc.DocumentVersionId);
        builder.HasIndex(dc => dc.ParentCommentId);
        builder.HasIndex(dc => dc.AuthorId);
        builder.HasIndex(dc => dc.Type);
        builder.HasIndex(dc => dc.Status);
        builder.HasIndex(dc => dc.IsResolved);
        builder.HasIndex(dc => dc.CommentDate);
        builder.HasIndex(dc => dc.IsDeleted);
        builder.HasIndex(dc => new { dc.DocumentId, dc.Status });
        builder.HasIndex(dc => new { dc.AuthorId, dc.CommentDate });

        // Basic Properties
        builder.Property(dc => dc.Comment)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(dc => dc.Type)
            .IsRequired();
            

        builder.Property(dc => dc.Status)
            .IsRequired();
            

        builder.Property(dc => dc.CommentDate)
            .IsRequired();

        // Review comment specific fields
        builder.Property(dc => dc.Section)
            .HasMaxLength(100);

        builder.Property(dc => dc.PageNumber);

        builder.Property(dc => dc.Reference)
            .HasMaxLength(256);

        
        // Resolution
        builder.Property(dc => dc.IsResolved)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(dc => dc.Resolution)
            .HasMaxLength(2000);

        // Soft Delete
        builder.Property(dc => dc.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(dc => dc.DeletedBy)
            .HasMaxLength(256);

        // Audit properties
        builder.Property(dc => dc.CreatedAt)
            .IsRequired();

        builder.Property(dc => dc.CreatedBy)
            .HasMaxLength(256);

        builder.Property(dc => dc.UpdatedAt);

        builder.Property(dc => dc.UpdatedBy)
            .HasMaxLength(256);

        // Relationships
        builder.HasOne(dc => dc.Document)
            .WithMany(d => d.Comments)
            .HasForeignKey(dc => dc.DocumentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(dc => dc.DocumentVersion)
            .WithMany(dv => dv.Comments)
            .HasForeignKey(dc => dc.DocumentVersionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(dc => dc.ParentComment)
            .WithMany(pc => pc.Replies)
            .HasForeignKey(dc => dc.ParentCommentId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(dc => dc.Author)
            .WithMany()
            .HasForeignKey(dc => dc.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(dc => dc.ResolvedBy)
            .WithMany()
            .HasForeignKey(dc => dc.ResolvedById)
            .OnDelete(DeleteBehavior.Restrict);

        // Navigation properties
        builder.HasMany(dc => dc.Replies)
            .WithOne(r => r.ParentComment)
            .HasForeignKey(r => r.ParentCommentId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(dc => dc.Attachments)
            .WithOne(a => a.Comment)
            .HasForeignKey(a => a.CommentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Check constraints
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_DocumentComments_PageNumber", 
                "[PageNumber] IS NULL OR [PageNumber] > 0");
        });

        // Global query filter for soft delete
        builder.HasQueryFilter(dc => !dc.IsDeleted);
    }
}