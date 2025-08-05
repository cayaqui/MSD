using Domain.Entities.Change.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Change
{
    public class ChangeRequestAttachmentConfiguration : IEntityTypeConfiguration<ChangeRequestAttachment>
    {
        public void Configure(EntityTypeBuilder<ChangeRequestAttachment> builder)
        {
            // Table name and schema
            builder.ToTable("ChangeRequestAttachments", "Change");

            // Primary key
            builder.HasKey(c => c.Id);

            // Indexes
            builder.HasIndex(c => c.ChangeRequestId);
            builder.HasIndex(c => c.UploadedBy);
            builder.HasIndex(c => c.Category);
            builder.HasIndex(c => c.UploadedDate);

            // Foreign Keys
            builder.Property(c => c.ChangeRequestId)
                .IsRequired();

            builder.Property(c => c.UploadedBy)
                .IsRequired();

            // Properties
            builder.Property(c => c.FileName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(c => c.FilePath)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(c => c.FileType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.FileSize)
                .IsRequired();

            builder.Property(c => c.Category)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.UploadedDate)
                .IsRequired();

            // Audit properties
            builder.Property(c => c.CreatedAt)
                .IsRequired();

            builder.Property(c => c.CreatedBy)
                .HasMaxLength(256);

            builder.Property(c => c.UpdatedAt);

            builder.Property(c => c.UpdatedBy)
                .HasMaxLength(256);

            // Relationships
            builder.HasOne(c => c.ChangeRequest)
                .WithMany(cr => cr.Attachments)
                .HasForeignKey(c => c.ChangeRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.UploadedByUser)
                .WithMany()
                .HasForeignKey(c => c.UploadedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_ChangeRequestAttachments_FileSize", 
                    "[FileSize] > 0");
            });
        }
    }
}