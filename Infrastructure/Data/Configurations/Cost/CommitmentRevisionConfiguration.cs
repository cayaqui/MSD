using Domain.Entities.Cost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Cost;

/// <summary>
/// Entity configuration for CommitmentRevision
/// </summary>
public class CommitmentRevisionConfiguration : IEntityTypeConfiguration<CommitmentRevision>
{
    public void Configure(EntityTypeBuilder<CommitmentRevision> builder)
    {
        // Table name and schema with check constraints
        builder.ToTable("CommitmentRevisions", "Cost", t =>
        {
            t.HasCheckConstraint("CK_CommitmentRevisions_RevisionNumber",
                "[RevisionNumber] > 0");
            t.HasCheckConstraint("CK_CommitmentRevisions_PreviousAmount",
                "[PreviousAmount] >= 0");
            t.HasCheckConstraint("CK_CommitmentRevisions_RevisedAmount",
                "[RevisedAmount] >= 0");
            t.HasCheckConstraint("CK_CommitmentRevisions_RevisionDate",
                "[RevisionDate] <= GETUTCDATE()");
            t.HasCheckConstraint("CK_CommitmentRevisions_Approval",
                "([ApprovalDate] IS NULL AND [ApprovedBy] IS NULL) OR " +
                "([ApprovalDate] IS NOT NULL AND [ApprovedBy] IS NOT NULL)");
            t.HasCheckConstraint("CK_CommitmentRevisions_ApprovalDate",
                "[ApprovalDate] IS NULL OR [ApprovalDate] >= [RevisionDate]");
        });

        // Primary key
        builder.HasKey(cr => cr.Id);

        // Indexes
        builder.HasIndex(cr => new { cr.CommitmentId, cr.RevisionNumber }).IsUnique();
        builder.HasIndex(cr => cr.CommitmentId);
        builder.HasIndex(cr => cr.RevisionNumber);
        builder.HasIndex(cr => cr.RevisionDate);
        builder.HasIndex(cr => cr.ApprovalDate);
        builder.HasIndex(cr => cr.ApprovedBy);
        builder.HasIndex(cr => cr.ChangeOrderReference);
        builder.HasIndex(cr => new { cr.ChangeAmount, cr.ChangePercentage }); // For filtering by impact

        // Properties
        builder.Property(cr => cr.RevisionNumber)
            .IsRequired();

        builder.Property(cr => cr.RevisionDate)
            .IsRequired();

        builder.Property(cr => cr.PreviousAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(cr => cr.RevisedAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        // Note: ChangeAmount and ChangePercentage are calculated in the entity constructor
        // They are not computed columns as they need to be set during entity creation
        builder.Property(cr => cr.ChangeAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(cr => cr.ChangePercentage)
            .HasPrecision(8, 4)
            .IsRequired();

        // Justification
        builder.Property(cr => cr.Reason)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(cr => cr.ChangeOrderReference)
            .HasMaxLength(100);

        builder.Property(cr => cr.ApprovedBy)
            .HasMaxLength(256);

        builder.Property(cr => cr.ApprovalDate);

        // Audit properties (inherited from BaseEntity)
        builder.Property(cr => cr.CreatedAt)
            .IsRequired();

        // Foreign key relationships
        builder.HasOne(cr => cr.Commitment)
            .WithMany(c => c.Revisions)
            .HasForeignKey(cr => cr.CommitmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}