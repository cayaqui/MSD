using Domain.Entities.Cost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Cost;

/// <summary>
/// Entity configuration for BudgetRevision
/// </summary>
public class BudgetRevisionConfiguration : IEntityTypeConfiguration<BudgetRevision>
{
    public void Configure(EntityTypeBuilder<BudgetRevision> builder)
    {
        // Table name and schema with check constraints
        builder.ToTable("BudgetRevisions", "Cost", t =>
        {
            t.HasCheckConstraint("CK_BudgetRevisions_RevisionNumber",
                "[RevisionNumber] > 0");
            t.HasCheckConstraint("CK_BudgetRevisions_PreviousAmount",
                "[PreviousAmount] >= 0");
            t.HasCheckConstraint("CK_BudgetRevisions_NewAmount",
                "[NewAmount] >= 0");
            t.HasCheckConstraint("CK_BudgetRevisions_RevisionDate",
                "[RevisionDate] <= GETUTCDATE()");
            t.HasCheckConstraint("CK_BudgetRevisions_Approval",
                "([IsApproved] = 0 AND [ApprovalDate] IS NULL AND [ApprovedBy] IS NULL) OR " +
                "([IsApproved] = 1 AND [ApprovalDate] IS NOT NULL AND [ApprovedBy] IS NOT NULL)");
            t.HasCheckConstraint("CK_BudgetRevisions_ApprovalDate",
                "[ApprovalDate] IS NULL OR [ApprovalDate] >= [RevisionDate]");
        });

        // Primary key
        builder.HasKey(br => br.Id);

        // Indexes
        builder.HasIndex(br => new { br.BudgetId, br.RevisionNumber }).IsUnique();
        builder.HasIndex(br => br.BudgetId);
        builder.HasIndex(br => br.RevisionNumber);
        builder.HasIndex(br => br.RevisionDate);
        builder.HasIndex(br => br.IsApproved);
        builder.HasIndex(br => br.ApprovalDate);
        builder.HasIndex(br => br.RevisedBy);
        builder.HasIndex(br => br.ApprovedBy);

        // Properties
        builder.Property(br => br.RevisionNumber)
            .IsRequired();

        builder.Property(br => br.Reason)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(br => br.Description)
            .HasMaxLength(2000);

        builder.Property(br => br.PreviousAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(br => br.NewAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        // Computed column for ChangeAmount
        builder.Property(br => br.ChangeAmount)
            .HasComputedColumnSql("[NewAmount] - [PreviousAmount]", stored: false);

        builder.Property(br => br.RevisionDate)
            .IsRequired();

        builder.Property(br => br.RevisedBy)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(br => br.IsApproved)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(br => br.ApprovalDate);

        builder.Property(br => br.ApprovedBy)
            .HasMaxLength(256);

        // Audit properties (inherited from BaseEntity)
        builder.Property(br => br.CreatedAt)
            .IsRequired();

        // Foreign key relationships
        builder.HasOne(br => br.Budget)
            .WithMany(b => b.Revisions)
            .HasForeignKey(br => br.BudgetId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}