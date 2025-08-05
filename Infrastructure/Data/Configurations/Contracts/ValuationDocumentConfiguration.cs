using Domain.Entities.Contracts.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Contracts
{
    public class ValuationDocumentConfiguration : IEntityTypeConfiguration<ValuationDocument>
    {
        public void Configure(EntityTypeBuilder<ValuationDocument> builder)
        {
            // Table name and schema
            builder.ToTable("ValuationDocuments", "Contracts");

            // Primary key
            builder.HasKey(vd => vd.Id);

            // Indexes
            builder.HasIndex(vd => vd.ValuationId);
            builder.HasIndex(vd => vd.DocumentId);
            builder.HasIndex(vd => vd.DocumentType);
            builder.HasIndex(vd => vd.IsActive);
            builder.HasIndex(vd => vd.UploadedDate);
            builder.HasIndex(vd => new { vd.ValuationId, vd.DocumentType, vd.IsActive });

            // Properties
            builder.Property(vd => vd.DocumentType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(vd => vd.FileName)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(vd => vd.FileUrl)
                .IsRequired()
                .HasMaxLength(2048);

            builder.Property(vd => vd.FileSize)
                .IsRequired();

            builder.Property(vd => vd.UploadedDate)
                .IsRequired();

            builder.Property(vd => vd.UploadedBy)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(vd => vd.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Relationships
            builder.HasOne(vd => vd.Valuation)
                .WithMany(v => v.Documents)
                .HasForeignKey(vd => vd.ValuationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_ValuationDocuments_FileSize", "[FileSize] > 0");
            });
        }
    }
}