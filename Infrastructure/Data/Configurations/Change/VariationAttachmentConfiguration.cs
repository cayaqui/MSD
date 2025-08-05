using Domain.Entities.Change.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Change
{
    public class VariationAttachmentConfiguration : IEntityTypeConfiguration<VariationAttachment>
    {
        public void Configure(EntityTypeBuilder<VariationAttachment> builder)
        {
            // Table name and schema
            builder.ToTable("VariationAttachments", "Change");

            // Primary key
            builder.HasKey(v => v.Id);

            // Indexes
            builder.HasIndex(v => v.VariationId);
            builder.HasIndex(v => v.UploadedBy);
            builder.HasIndex(v => v.Category);
            builder.HasIndex(v => v.UploadedDate);

            // Foreign Keys
            builder.Property(v => v.VariationId)
                .IsRequired();

            builder.Property(v => v.UploadedBy)
                .IsRequired();

            // Properties
            builder.Property(v => v.FileName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(v => v.FilePath)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(v => v.FileType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(v => v.FileSize)
                .IsRequired();

            builder.Property(v => v.Category)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(v => v.UploadedDate)
                .IsRequired();

            // Audit properties
            builder.Property(v => v.CreatedAt)
                .IsRequired();

            builder.Property(v => v.CreatedBy)
                .HasMaxLength(256);

            builder.Property(v => v.UpdatedAt);

            builder.Property(v => v.UpdatedBy)
                .HasMaxLength(256);

            // Relationships
            builder.HasOne(v => v.Variation)
                .WithMany(var => var.Attachments)
                .HasForeignKey(v => v.VariationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(v => v.UploadedByUser)
                .WithMany()
                .HasForeignKey(v => v.UploadedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_VariationAttachments_FileSize", 
                    "[FileSize] > 0");
            });
        }
    }
}