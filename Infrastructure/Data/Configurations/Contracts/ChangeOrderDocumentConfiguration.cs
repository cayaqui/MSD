using Domain.Entities.Contracts.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Contracts
{
    public class ChangeOrderDocumentConfiguration : IEntityTypeConfiguration<ChangeOrderDocument>
    {
        public void Configure(EntityTypeBuilder<ChangeOrderDocument> builder)
        {
            // Table name and schema
            builder.ToTable("ChangeOrderDocuments", "Contracts");

            // Primary key
            builder.HasKey(cod => cod.Id);

            // Indexes
            builder.HasIndex(cod => cod.ChangeOrderId);
            builder.HasIndex(cod => cod.DocumentId);
            builder.HasIndex(cod => cod.DocumentType);
            builder.HasIndex(cod => cod.IsActive);
            builder.HasIndex(cod => cod.UploadedDate);
            builder.HasIndex(cod => new { cod.ChangeOrderId, cod.DocumentType, cod.IsActive });

            // Properties
            builder.Property(cod => cod.DocumentType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(cod => cod.FileName)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(cod => cod.FileUrl)
                .IsRequired()
                .HasMaxLength(2048);

            builder.Property(cod => cod.FileSize)
                .IsRequired();

            builder.Property(cod => cod.UploadedDate)
                .IsRequired();

            builder.Property(cod => cod.UploadedBy)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(cod => cod.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Relationships
            builder.HasOne(cod => cod.ChangeOrder)
                .WithMany(co => co.Documents)
                .HasForeignKey(cod => cod.ChangeOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_ChangeOrderDocuments_FileSize", "[FileSize] > 0");
            });
        }
    }
}