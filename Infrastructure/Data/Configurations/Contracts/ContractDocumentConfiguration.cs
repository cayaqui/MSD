using Domain.Entities.Contracts.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Contracts
{
    public class ContractDocumentConfiguration : IEntityTypeConfiguration<ContractDocument>
    {
        public void Configure(EntityTypeBuilder<ContractDocument> builder)
        {
            // Table name and schema
            builder.ToTable("ContractDocuments", "Contracts");

            // Primary key
            builder.HasKey(cd => cd.Id);

            // Indexes
            builder.HasIndex(cd => cd.ContractId);
            builder.HasIndex(cd => cd.DocumentId);
            builder.HasIndex(cd => cd.DocumentType);
            builder.HasIndex(cd => cd.IsActive);
            builder.HasIndex(cd => cd.UploadedDate);
            builder.HasIndex(cd => new { cd.ContractId, cd.DocumentType, cd.IsActive });

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
            builder.HasOne(cd => cd.Contract)
                .WithMany(c => c.Documents)
                .HasForeignKey(cd => cd.ContractId)
                .OnDelete(DeleteBehavior.Cascade);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_ContractDocuments_FileSize", "[FileSize] > 0");
            });
        }
    }
}