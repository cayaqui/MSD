using Domain.Entities.Documents.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Documents;

/// <summary>
/// Entity configuration for DocumentDistribution
/// </summary>
public class DocumentDistributionConfiguration : IEntityTypeConfiguration<DocumentDistribution>
{
    public void Configure(EntityTypeBuilder<DocumentDistribution> builder)
    {
        // Table name and schema
        builder.ToTable("DocumentDistributions", "Documents");

        // Primary key
        builder.HasKey(dd => dd.Id);

        // Indexes
        builder.HasIndex(dd => dd.DocumentId);
        builder.HasIndex(dd => dd.DocumentVersionId);
        builder.HasIndex(dd => dd.RecipientUserId);
        builder.HasIndex(dd => dd.RecipientCompanyId);
        builder.HasIndex(dd => dd.DistributedById);
        builder.HasIndex(dd => dd.TransmittalId);
        builder.HasIndex(dd => dd.Method);
        builder.HasIndex(dd => dd.Purpose);
        builder.HasIndex(dd => dd.DistributionDate);
        builder.HasIndex(dd => dd.RequiresAcknowledgment);
        builder.HasIndex(dd => dd.IsAcknowledged);
        builder.HasIndex(dd => dd.IsDownloaded);
        builder.HasIndex(dd => dd.ExpiryDate);
        builder.HasIndex(dd => new { dd.DocumentId, dd.Method });
        builder.HasIndex(dd => new { dd.RecipientUserId, dd.DistributionDate });

        // Distribution details
        builder.Property(dd => dd.Method)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(dd => dd.Purpose)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(dd => dd.DistributionDate)
            .IsRequired();

        builder.Property(dd => dd.Comments)
            .HasMaxLength(1000);

        // Recipient
        builder.Property(dd => dd.RecipientEmail)
            .HasMaxLength(256);

        builder.Property(dd => dd.RecipientRole)
            .HasMaxLength(128);

        // Acknowledgment
        builder.Property(dd => dd.RequiresAcknowledgment)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(dd => dd.IsAcknowledged)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(dd => dd.AcknowledgedBy)
            .HasMaxLength(256);

        builder.Property(dd => dd.AcknowledgmentComments)
            .HasMaxLength(1000);

        // Download tracking
        builder.Property(dd => dd.IsDownloaded)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(dd => dd.DownloadCount)
            .IsRequired()
            .HasDefaultValue(0);

        // Access control
        builder.Property(dd => dd.AccessToken)
            .HasMaxLength(128);

        // Audit properties
        builder.Property(dd => dd.CreatedAt)
            .IsRequired();

        builder.Property(dd => dd.CreatedBy)
            .HasMaxLength(256);

        builder.Property(dd => dd.UpdatedAt);

        builder.Property(dd => dd.UpdatedBy)
            .HasMaxLength(256);

        // Relationships
        builder.HasOne(dd => dd.Document)
            .WithMany(d => d.Distributions)
            .HasForeignKey(dd => dd.DocumentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(dd => dd.DocumentVersion)
            .WithMany(dv => dv.Distributions)
            .HasForeignKey(dd => dd.DocumentVersionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(dd => dd.RecipientUser)
            .WithMany()
            .HasForeignKey(dd => dd.RecipientUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(dd => dd.RecipientCompany)
            .WithMany()
            .HasForeignKey(dd => dd.RecipientCompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(dd => dd.DistributedBy)
            .WithMany()
            .HasForeignKey(dd => dd.DistributedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(dd => dd.Transmittal)
            .WithMany(t => t.DocumentDistributions)
            .HasForeignKey(dd => dd.TransmittalId)
            .OnDelete(DeleteBehavior.Restrict);

        // Check constraints
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_DocumentDistributions_DownloadCount", 
                "[DownloadCount] >= 0");
            t.HasCheckConstraint("CK_DocumentDistributions_Recipient", 
                "[RecipientUserId] IS NOT NULL OR [RecipientEmail] IS NOT NULL");
        });
    }
}