using Domain.Entities.Documents.Transmittals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Documents;

/// <summary>
/// Entity configuration for TransmittalDocument
/// </summary>
public class TransmittalDocumentConfiguration : IEntityTypeConfiguration<TransmittalDocument>
{
    public void Configure(EntityTypeBuilder<TransmittalDocument> builder)
    {
        // Table name and schema
        builder.ToTable("TransmittalDocuments", "Documents");

        // Primary key
        builder.HasKey(td => td.Id);

        // Indexes
        builder.HasIndex(td => td.TransmittalId);
        builder.HasIndex(td => td.DocumentId);
        builder.HasIndex(td => td.DocumentVersionId);
        builder.HasIndex(td => td.AddedById);
        builder.HasIndex(td => td.IsIncluded);
        builder.HasIndex(td => td.SortOrder);
        builder.HasIndex(td => new { td.TransmittalId, td.DocumentId, td.DocumentVersionId }).IsUnique();

        // Transmittal specific information
        builder.Property(td => td.Copies)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(td => td.Format)
            .HasMaxLength(50);

        builder.Property(td => td.Purpose)
            .HasMaxLength(256);

        builder.Property(td => td.Comments)
            .HasMaxLength(1000);

        builder.Property(td => td.SortOrder)
            .IsRequired()
            .HasDefaultValue(0);

        // Tracking
        builder.Property(td => td.IsIncluded)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(td => td.AddedDate)
            .IsRequired();

        // Audit properties
        builder.Property(td => td.CreatedAt)
            .IsRequired();

        builder.Property(td => td.CreatedBy)
            .HasMaxLength(256);

        builder.Property(td => td.UpdatedAt);

        builder.Property(td => td.UpdatedBy)
            .HasMaxLength(256);

        // Relationships
        builder.HasOne(td => td.Transmittal)
            .WithMany(t => t.Documents)
            .HasForeignKey(td => td.TransmittalId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(td => td.Document)
            .WithMany(d => d.TransmittalDocuments)
            .HasForeignKey(td => td.DocumentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(td => td.DocumentVersion)
            .WithMany(dv => dv.TransmittalDocuments)
            .HasForeignKey(td => td.DocumentVersionId)
            .OnDelete(DeleteBehavior.Restrict);

        // Check constraints
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_TransmittalDocuments_Copies", 
                "[Copies] > 0");
        });
    }
}