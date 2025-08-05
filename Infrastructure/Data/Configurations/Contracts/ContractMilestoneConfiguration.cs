using Domain.Entities.Contracts.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Contracts
{
    public class ContractMilestoneConfiguration : IEntityTypeConfiguration<ContractMilestone>
    {
        public void Configure(EntityTypeBuilder<ContractMilestone> builder)
        {
            // Table name and schema
            builder.ToTable("ContractMilestones", "Contracts");

            // Primary key
            builder.HasKey(cm => cm.Id);

            // Indexes
            builder.HasIndex(cm => new { cm.ContractId, cm.MilestoneCode }).IsUnique();
            builder.HasIndex(cm => cm.ContractId);
            builder.HasIndex(cm => cm.Type);
            builder.HasIndex(cm => cm.Status);
            builder.HasIndex(cm => cm.PlannedDate);
            builder.HasIndex(cm => cm.IsActive);
            builder.HasIndex(cm => cm.IsPaymentMilestone);
            builder.HasIndex(cm => cm.IsCritical);
            builder.HasIndex(cm => new { cm.ContractId, cm.Status });
            builder.HasIndex(cm => new { cm.ContractId, cm.SequenceNumber });

            // Properties
            builder.Property(cm => cm.MilestoneCode)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(cm => cm.Name)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(cm => cm.Description)
                .HasMaxLength(4000);

            builder.Property(cm => cm.Type)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(cm => cm.Status)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(cm => cm.SequenceNumber)
                .IsRequired();

            // Value
            builder.Property(cm => cm.Amount)
                .HasPrecision(18, 2);

            builder.Property(cm => cm.PercentageOfContract)
                .HasPrecision(5, 2);

            builder.Property(cm => cm.Currency)
                .IsRequired()
                .HasMaxLength(3)
                .HasDefaultValue("USD");

            // Completion Criteria
            builder.Property(cm => cm.CompletionCriteria)
                .HasMaxLength(4000);

            builder.Property(cm => cm.Deliverables)
                .HasMaxLength(4000);

            // Progress
            builder.Property(cm => cm.PercentageComplete)
                .HasPrecision(5, 2)
                .HasDefaultValue(0);

            builder.Property(cm => cm.ProgressComments)
                .HasMaxLength(2000);

            // Approval
            builder.Property(cm => cm.ApprovedBy)
                .HasMaxLength(256);

            builder.Property(cm => cm.ApprovalComments)
                .HasMaxLength(2000);

            // Payment
            builder.Property(cm => cm.InvoiceNumber)
                .HasMaxLength(50);

            builder.Property(cm => cm.InvoiceAmount)
                .HasPrecision(18, 2);

            builder.Property(cm => cm.PaymentAmount)
                .HasPrecision(18, 2);

            // Variance
            builder.Property(cm => cm.VarianceExplanation)
                .HasMaxLength(2000);

            // Audit
            builder.Property(cm => cm.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(cm => cm.Notes)
                .HasMaxLength(4000);

            builder.Property(cm => cm.Metadata)
                .HasColumnType("nvarchar(max)");

            // Base audit properties
            builder.Property(cm => cm.CreatedAt)
                .IsRequired();

            builder.Property(cm => cm.CreatedBy)
                .HasMaxLength(256);

            builder.Property(cm => cm.UpdatedAt);

            builder.Property(cm => cm.UpdatedBy)
                .HasMaxLength(256);

            // Relationships
            builder.HasOne(cm => cm.Contract)
                .WithMany(c => c.Milestones)
                .HasForeignKey(cm => cm.ContractId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(cm => cm.Documents)
                .WithOne(d => d.Milestone)
                .HasForeignKey(d => d.MilestoneId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(cm => cm.Predecessors)
                .WithOne(d => d.Successor)
                .HasForeignKey(d => d.SuccessorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(cm => cm.Successors)
                .WithOne(d => d.Predecessor)
                .HasForeignKey(d => d.PredecessorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(cm => cm.ChangeOrders)
                .WithOne(co => co.Milestone)
                .HasForeignKey(co => co.MilestoneId)
                .OnDelete(DeleteBehavior.Restrict);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_ContractMilestones_PercentageComplete",
                    "[PercentageComplete] >= 0 AND [PercentageComplete] <= 100");
                t.HasCheckConstraint("CK_ContractMilestones_PercentageOfContract",
                    "[PercentageOfContract] >= 0 AND [PercentageOfContract] <= 100");
                t.HasCheckConstraint("CK_ContractMilestones_Dates",
                    "[ActualDate] IS NULL OR [ActualDate] >= [PlannedDate]");
                t.HasCheckConstraint("CK_ContractMilestones_Amounts",
                    "[Amount] >= 0 AND [InvoiceAmount] >= 0 AND [PaymentAmount] >= 0");
            });

            // Ignore calculated properties
            builder.Ignore(cm => cm.AttachmentCount);
            builder.Ignore(cm => cm.HasSupportingDocuments);
            builder.Ignore(cm => cm.ScheduleVarianceDays);
            builder.Ignore(cm => cm.CostVariance);
        }
    }
}