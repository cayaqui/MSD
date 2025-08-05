using Domain.Entities.Contracts.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Contracts
{
    public class ContractChangeOrderConfiguration : IEntityTypeConfiguration<ContractChangeOrder>
    {
        public void Configure(EntityTypeBuilder<ContractChangeOrder> builder)
        {
            // Table name and schema
            builder.ToTable("ContractChangeOrders", "Contracts");

            // Primary key
            builder.HasKey(cco => cco.Id);

            // Indexes
            builder.HasIndex(cco => cco.ChangeOrderNumber).IsUnique();
            builder.HasIndex(cco => cco.ContractId);
            builder.HasIndex(cco => cco.Type);
            builder.HasIndex(cco => cco.Status);
            builder.HasIndex(cco => cco.Priority);
            builder.HasIndex(cco => cco.IsActive);
            builder.HasIndex(cco => new { cco.ContractId, cco.Status });
            builder.HasIndex(cco => cco.SubmissionDate);
            builder.HasIndex(cco => cco.ApprovalDate);

            // Properties
            builder.Property(cco => cco.ChangeOrderNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(cco => cco.Title)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(cco => cco.Description)
                .IsRequired()
                .HasMaxLength(4000);

            builder.Property(cco => cco.Type)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(cco => cco.Status)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(cco => cco.Priority)
                .IsRequired()
                .HasMaxLength(50);

            // Change Details
            builder.Property(cco => cco.Justification)
                .IsRequired()
                .HasMaxLength(4000);

            builder.Property(cco => cco.ScopeImpact)
                .HasMaxLength(2000);

            builder.Property(cco => cco.ScheduleImpact)
                .HasMaxLength(2000);

            // Financial Impact
            builder.Property(cco => cco.Estimate)
                .HasPrecision(18, 2);

            builder.Property(cco => cco.Approve)
                .HasPrecision(18, 2);

            builder.Property(cco => cco.Actua)
                .HasPrecision(18, 2);

            builder.Property(cco => cco.Currency)
                .IsRequired()
                .HasMaxLength(3)
                .HasDefaultValue("USD");

            // Schedule Impact
            builder.Property(cco => cco.ScheduleImpactDays)
                .IsRequired()
                .HasDefaultValue(0);

            // Approval Process
            builder.Property(cco => cco.SubmittedBy)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(cco => cco.ReviewedBy)
                .HasMaxLength(256);

            builder.Property(cco => cco.ReviewComments)
                .HasMaxLength(2000);

            builder.Property(cco => cco.ApprovedBy)
                .HasMaxLength(256);

            builder.Property(cco => cco.ApprovalComments)
                .HasMaxLength(2000);

            builder.Property(cco => cco.RejectedBy)
                .HasMaxLength(256);

            builder.Property(cco => cco.RejectionReason)
                .HasMaxLength(2000);

            // Implementation
            builder.Property(cco => cco.PercentageComplete)
                .HasPrecision(5, 2)
                .HasDefaultValue(0);

            // Risk Assessment
            builder.Property(cco => cco.RiskAssessment)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(cco => cco.RiskMitigationPlan)
                .HasMaxLength(2000);

            // Audit
            builder.Property(cco => cco.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(cco => cco.Notes)
                .HasMaxLength(4000);

            builder.Property(cco => cco.Metadata)
                .HasColumnType("nvarchar(max)");

            // Base audit properties
            builder.Property(cco => cco.CreatedAt)
                .IsRequired();

            builder.Property(cco => cco.CreatedBy)
                .HasMaxLength(256);

            builder.Property(cco => cco.UpdatedAt);

            builder.Property(cco => cco.UpdatedBy)
                .HasMaxLength(256);

            // Relationships
            builder.HasOne(cco => cco.Contract)
                .WithMany(c => c.ChangeOrders)
                .HasForeignKey(cco => cco.ContractId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(cco => cco.Documents)
                .WithOne(d => d.ChangeOrder)
                .HasForeignKey(d => d.ChangeOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(cco => cco.AffectedMilestones)
                .WithOne(m => m.ChangeOrder)
                .HasForeignKey(m => m.ChangeOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_ContractChangeOrders_PercentageComplete", 
                    "[PercentageComplete] >= 0 AND [PercentageComplete] <= 100");
                t.HasCheckConstraint("CK_ContractChangeOrders_Amounts", 
                    "[Estimate] >= 0 AND [Approve] >= 0 AND [Actua] >= 0");
                t.HasCheckConstraint("CK_ContractChangeOrders_ApprovalDates",
                    "([ApprovalDate] IS NULL AND [RejectionDate] IS NULL) OR " +
                    "([ApprovalDate] IS NOT NULL AND [RejectionDate] IS NULL) OR " +
                    "([ApprovalDate] IS NULL AND [RejectionDate] IS NOT NULL)");
            });

            // Ignore calculated properties
            builder.Ignore(cco => cco.AttachmentCount);
        }
    }
}