using Domain.Entities.Cost.Budget;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Cost
{
    public class BudgetRevisionConfiguration : IEntityTypeConfiguration<BudgetRevision>
    {
        public void Configure(EntityTypeBuilder<BudgetRevision> builder)
        {
            // Table name and schema
            builder.ToTable("BudgetRevisions", "Cost");

            // Primary key
            builder.HasKey(br => br.Id);

            // Indexes
            builder.HasIndex(br => br.BudgetId);
            builder.HasIndex(br => br.RevisionNumber);
            builder.HasIndex(br => br.RevisionDate);
            builder.HasIndex(br => br.IsApproved);
            builder.HasIndex(br => new { br.BudgetId, br.RevisionNumber }).IsUnique();

            // Properties
            builder.Property(br => br.RevisionNumber)
                .IsRequired();

            builder.Property(br => br.Reason)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(br => br.Description)
                .HasMaxLength(2000);

            builder.Property(br => br.PreviousAmount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(br => br.NewAmount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(br => br.RevisionDate)
                .IsRequired();

            builder.Property(br => br.RevisedBy)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(br => br.IsApproved)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(br => br.ApprovedBy)
                .HasMaxLength(256);

            // Calculated property
            builder.Ignore(br => br.ChangeAmount);

            // Relationships
            builder.HasOne(br => br.Budget)
                .WithMany(b => b.Revisions)
                .HasForeignKey(br => br.BudgetId)
                .OnDelete(DeleteBehavior.Cascade);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_BudgetRevisions_RevisionNumber", "[RevisionNumber] > 0");
            });
        }
    }
}