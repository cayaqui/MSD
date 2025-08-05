using Domain.Entities.Contracts.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Contracts
{
    public class ClaimDocumentConfiguration : IEntityTypeConfiguration<ClaimDocument>
    {
        public void Configure(EntityTypeBuilder<ClaimDocument> builder)
        {
            // Table name and schema
            builder.ToTable("ClaimDocuments", "Contracts");

            // Primary key
            builder.HasKey(cd => cd.Id);

            // Indexes
            builder.HasIndex(cd => cd.ClaimId);
            builder.HasIndex(cd => cd.DocumentId);
            builder.HasIndex(cd => cd.DocumentType);
            builder.HasIndex(cd => cd.IsActive);
            builder.HasIndex(cd => cd.UploadedDate);
            builder.HasIndex(cd => new { cd.ClaimId, cd.DocumentType, cd.IsActive });

            // Properties
            builder.Property(cd => cd.DocumentType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(cd => cd.FileName)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(cd => cd.FileUrl)
                .IsRequired()
                .HasMaxLength(2048);

            builder.Property(cd => cd.FileSize)
                .IsRequired();

            builder.Property(cd => cd.UploadedDate)
                .IsRequired();

            builder.Property(cd => cd.UploadedBy)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(cd => cd.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Relationships
            builder.HasOne(cd => cd.Claim)
                .WithMany(c => c.Documents)
                .HasForeignKey(cd => cd.ClaimId)
                .OnDelete(DeleteBehavior.Cascade);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_ClaimDocuments_FileSize", "[FileSize] > 0");
            });
        }
    }
}