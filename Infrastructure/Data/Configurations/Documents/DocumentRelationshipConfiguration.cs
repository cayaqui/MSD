using Domain.Entities.Documents.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Documents;

/// <summary>
/// Entity configuration for DocumentRelationship
/// </summary>
public class DocumentRelationshipConfiguration : IEntityTypeConfiguration<DocumentRelationship>
{
    public void Configure(EntityTypeBuilder<DocumentRelationship> builder)
    {
        // Table name and schema
        builder.ToTable("DocumentRelationships", "Documents");

        // Primary key
        builder.HasKey(dr => dr.Id);

        // Indexes
        builder.HasIndex(dr => dr.DocumentId);
        builder.HasIndex(dr => dr.RelatedDocumentId);
        builder.HasIndex(dr => dr.RelationshipType);
        builder.HasIndex(dr => dr.EstablishedDate);
        builder.HasIndex(dr => dr.EstablishedById);
        builder.HasIndex(dr => new { dr.DocumentId, dr.RelatedDocumentId, dr.RelationshipType })
            .IsUnique()
            .HasDatabaseName("IX_DocumentRelationships_Unique");

        // Properties
        builder.Property(dr => dr.RelationshipType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(dr => dr.Description)
            .HasMaxLength(500);

        builder.Property(dr => dr.EstablishedDate)
            .IsRequired();

        // Audit properties
        builder.Property(dr => dr.CreatedAt)
            .IsRequired();

        builder.Property(dr => dr.CreatedBy)
            .HasMaxLength(256);

        builder.Property(dr => dr.UpdatedAt);

        builder.Property(dr => dr.UpdatedBy)
            .HasMaxLength(256);

        // Relationships
        builder.HasOne(dr => dr.Document)
            .WithMany(d => d.RelatedDocuments)
            .HasForeignKey(dr => dr.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(dr => dr.RelatedDocument)
            .WithMany(d => d.RelatedToDocuments)
            .HasForeignKey(dr => dr.RelatedDocumentId)
            .OnDelete(DeleteBehavior.NoAction);

        // Check constraints
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_DocumentRelationships_NotSelfReferencing", 
                "[DocumentId] <> [RelatedDocumentId]");
        });
    }
}