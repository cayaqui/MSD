using Domain.Entities.Documents.Transmittals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Documents;

/// <summary>
/// Entity configuration for Transmittal
/// </summary>
public class TransmittalConfiguration : IEntityTypeConfiguration<Transmittal>
{
    public void Configure(EntityTypeBuilder<Transmittal> builder)
    {
        // Table name and schema
        builder.ToTable("Transmittals", "Documents");

        // Primary key
        builder.HasKey(t => t.Id);

        // Indexes
        builder.HasIndex(t => t.TransmittalNumber).IsUnique();
        builder.HasIndex(t => t.ProjectId);
        builder.HasIndex(t => t.FromCompanyId);
        builder.HasIndex(t => t.ToCompanyId);
        builder.HasIndex(t => t.Status);
        builder.HasIndex(t => t.Priority);
        builder.HasIndex(t => t.TransmittalDate);
        builder.HasIndex(t => t.PreparedById);
        builder.HasIndex(t => t.ApprovedById);
        builder.HasIndex(t => t.SentById);
        builder.HasIndex(t => t.IsDeleted);
        builder.HasIndex(t => new { t.ProjectId, t.Status });
        builder.HasIndex(t => new { t.FromCompanyId, t.TransmittalDate });

        // Basic Properties
        builder.Property(t => t.TransmittalNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(t => t.TransmittalDate)
            .IsRequired();

        builder.Property(t => t.Status)
            .IsRequired();

        builder.Property(t => t.Priority)
            .IsRequired();
        // From/To information
        builder.Property(t => t.FromContact)
            .HasMaxLength(256);

        builder.Property(t => t.FromEmail)
            .HasMaxLength(256);

        builder.Property(t => t.FromPhone)
            .HasMaxLength(50);

        builder.Property(t => t.ToContact)
            .HasMaxLength(256);

        builder.Property(t => t.ToEmail)
            .HasMaxLength(256);

        builder.Property(t => t.ToPhone)
            .HasMaxLength(50);

        // Transmittal details
        builder.Property(t => t.Subject)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(t => t.Description)
            .HasMaxLength(2000);

        builder.Property(t => t.Purpose)
            .HasMaxLength(500);

        builder.Property(t => t.ResponseRequired)
            .HasMaxLength(256);

        // Delivery information
        builder.Property(t => t.DeliveryMethod)
            .IsRequired();
        builder.Property(t => t.TrackingNumber)
            .HasMaxLength(100);

        builder.Property(t => t.IsDelivered)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(t => t.DeliveryConfirmedBy)
            .HasMaxLength(256);

        builder.Property(t => t.PreparedDate)
            .IsRequired();

        // Approval
        builder.Property(t => t.RequiresApproval)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(t => t.ApprovalComments)
            .HasMaxLength(1000);

        // Soft Delete
        builder.Property(t => t.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(t => t.DeletedBy)
            .HasMaxLength(256);

        // Audit properties
        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.CreatedBy)
            .HasMaxLength(256);

        builder.Property(t => t.UpdatedAt);

        builder.Property(t => t.UpdatedBy)
            .HasMaxLength(256);

        // Relationships
        builder.HasOne(t => t.Project)
            .WithMany()
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.FromCompany)
            .WithMany()
            .HasForeignKey(t => t.FromCompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.ToCompany)
            .WithMany()
            .HasForeignKey(t => t.ToCompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.PreparedBy)
            .WithMany()
            .HasForeignKey(t => t.PreparedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.ApprovedBy)
            .WithMany()
            .HasForeignKey(t => t.ApprovedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.SentBy)
            .WithMany()
            .HasForeignKey(t => t.SentById)
            .OnDelete(DeleteBehavior.Restrict);

        // Navigation properties
        builder.HasMany(t => t.Documents)
            .WithOne(td => td.Transmittal)
            .HasForeignKey(td => td.TransmittalId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.Recipients)
            .WithOne(tr => tr.Transmittal)
            .HasForeignKey(tr => tr.TransmittalId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.Attachments)
            .WithOne(ta => ta.Transmittal)
            .HasForeignKey(ta => ta.TransmittalId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.DocumentDistributions)
            .WithOne(dd => dd.Transmittal)
            .HasForeignKey(dd => dd.TransmittalId)
            .OnDelete(DeleteBehavior.Restrict);

        // Check constraints
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_Transmittals_ResponseDue", 
                "[ResponseDueDate] IS NULL OR [ResponseDueDate] > [TransmittalDate]");
        });

        // Global query filter for soft delete
        builder.HasQueryFilter(t => !t.IsDeleted);
    }
}