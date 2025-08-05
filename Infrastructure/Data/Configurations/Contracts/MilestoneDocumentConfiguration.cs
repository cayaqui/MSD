using Domain.Entities.Contracts.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Contracts
{
    public class MilestoneDocumentConfiguration : IEntityTypeConfiguration<MilestoneDocument>
    {
        public void Configure(EntityTypeBuilder<MilestoneDocument> builder)
        {
            // Table name and schema
            builder.ToTable("MilestoneDocuments", "Contracts");

            // Primary key
            builder.HasKey(md => md.Id);

            // Indexes
            builder.HasIndex(md => md.MilestoneId);
            builder.HasIndex(md => md.DocumentId);
            builder.HasIndex(md => md.DocumentType);
            builder.HasIndex(md => md.IsActive);
            builder.HasIndex(md => md.UploadedDate);
            builder.HasIndex(md => new { md.MilestoneId, md.DocumentType, md.IsActive });

            // Properties
            builder.Property(md => md.DocumentType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(md => md.FileName)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(md => md.FileUrl)
                .IsRequired()
                .HasMaxLength(2048);

            builder.Property(md => md.FileSize)
                .IsRequired();

            builder.Property(md => md.UploadedDate)
                .IsRequired();

            builder.Property(md => md.UploadedBy)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(md => md.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Relationships
            builder.HasOne(md => md.Milestone)
                .WithMany(m => m.Documents)
                .HasForeignKey(md => md.MilestoneId)
                .OnDelete(DeleteBehavior.Cascade);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_MilestoneDocuments_FileSize", "[FileSize] > 0");
            });
        }
    }
}