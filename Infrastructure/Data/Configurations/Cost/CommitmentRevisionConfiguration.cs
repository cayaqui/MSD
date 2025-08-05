using Domain.Common;
using Domain.Entities.Cost.Commitments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Cost
{
    public class CommitmentRevisionConfiguration : IEntityTypeConfiguration<CommitmentRevision>
    {
        public void Configure(EntityTypeBuilder<CommitmentRevision> builder)
        {
            // Table name and schema
            builder.ToTable("CommitmentRevisions", "Cost");

            // Primary key
            builder.HasKey(c => c.Id);

            // Indexes
            builder.HasIndex(c => c.CommitmentId);
            builder.HasIndex(c => c.RevisionNumber);
            builder.HasIndex(c => c.RevisionDate);
            builder.HasIndex(c => new { c.CommitmentId, c.RevisionNumber });

            // Revision Information
            builder.Property(c => c.RevisionNumber)
                .IsRequired();

            builder.Property(c => c.RevisionDate)
                .IsRequired();

            // Financial Changes
            builder.Property(c => c.PreviousAmount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(c => c.RevisedAmount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(c => c.ChangeAmount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(c => c.ChangePercentage)
                .IsRequired()
                .HasPrecision(5, 2);

            // Justification
            builder.Property(c => c.Reason)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(c => c.ChangeOrderReference)
                .HasMaxLength(100);

            builder.Property(c => c.ApprovedBy)
                .HasMaxLength(256);

            // Audit properties
            builder.Property(c => c.CreatedAt)
                .IsRequired();

            builder.Property(c => c.CreatedBy)
                .HasMaxLength(256);

            builder.Property(c => c.UpdatedAt);

            builder.Property(c => c.UpdatedBy)
                .HasMaxLength(256);

            // Relationships
            builder.HasOne(c => c.Commitment)
                .WithMany(com => com.Revisions)
                .HasForeignKey(c => c.CommitmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_CommitmentRevisions_RevisionNumber", "[RevisionNumber] >= 0");
                t.HasCheckConstraint("CK_CommitmentRevisions_Amounts", "[PreviousAmount] >= 0 AND [RevisedAmount] >= 0");
            });
        }
    }
}