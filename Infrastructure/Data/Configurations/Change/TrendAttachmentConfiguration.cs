using Domain.Entities.Change.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Change
{
    public class TrendAttachmentConfiguration : IEntityTypeConfiguration<TrendAttachment>
    {
        public void Configure(EntityTypeBuilder<TrendAttachment> builder)
        {
            // Table name and schema
            builder.ToTable("TrendAttachments", "Change");

            // Primary key
            builder.HasKey(t => t.Id);

            // Indexes
            builder.HasIndex(t => t.TrendId);
            builder.HasIndex(t => t.UploadedBy);
            builder.HasIndex(t => t.UploadedDate);

            // Foreign Keys
            builder.Property(t => t.TrendId)
                .IsRequired();

            builder.Property(t => t.UploadedBy)
                .IsRequired();

            // Properties
            builder.Property(t => t.FileName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(t => t.FilePath)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(t => t.FileSize)
                .IsRequired();

            builder.Property(t => t.FileType)
                .HasMaxLength(100);

            builder.Property(t => t.UploadedDate)
                .IsRequired();

            // Audit properties
            builder.Property(t => t.CreatedAt)
                .IsRequired();

            builder.Property(t => t.CreatedBy)
                .HasMaxLength(256);

            builder.Property(t => t.UpdatedAt);

            builder.Property(t => t.UpdatedBy)
                .HasMaxLength(256);

            // Relationships
            builder.HasOne(t => t.Trend)
                .WithMany(trend => trend.Attachments)
                .HasForeignKey(t => t.TrendId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(t => t.UploadedByUser)
                .WithMany()
                .HasForeignKey(t => t.UploadedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // Check constraints
            builder.ToTable(tb =>
            {
                tb.HasCheckConstraint("CK_TrendAttachments_FileSize", 
                    "[FileSize] > 0");
            });
        }
    }
}