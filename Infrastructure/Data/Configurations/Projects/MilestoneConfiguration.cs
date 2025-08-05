using Domain.Entities.Progress;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Projects
{
    public class MilestoneConfiguration : IEntityTypeConfiguration<Milestone>
    {
        public void Configure(EntityTypeBuilder<Milestone> builder)
        {
            // Table name and schema
            builder.ToTable("Milestones", "Projects");

            // Primary key
            builder.HasKey(m => m.Id);

            // Indexes
            builder.HasIndex(m => m.MilestoneCode).IsUnique();
            builder.HasIndex(m => m.ProjectId);
            builder.HasIndex(m => m.PhaseId);
            builder.HasIndex(m => m.WorkPackageId);
            builder.HasIndex(m => m.Type);
            builder.HasIndex(m => m.IsCritical);
            builder.HasIndex(m => m.IsContractual);
            builder.HasIndex(m => m.PlannedDate);
            builder.HasIndex(m => m.IsCompleted);
            builder.HasIndex(m => m.IsDeleted);
            builder.HasIndex(m => new { m.ProjectId, m.MilestoneCode }).IsUnique();
            builder.HasIndex(m => new { m.ProjectId, m.Type });
            builder.HasIndex(m => new { m.ProjectId, m.PlannedDate });

            // Properties
            builder.Property(m => m.MilestoneCode)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(m => m.Name)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(m => m.Description)
                .HasMaxLength(4000);

            builder.Property(m => m.Type)
                .IsRequired();

            builder.Property(m => m.IsCritical)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(m => m.IsContractual)
                .IsRequired()
                .HasDefaultValue(false);

            // Schedule Information
            builder.Property(m => m.PlannedDate)
                .IsRequired();

            // Status
            builder.Property(m => m.IsCompleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(m => m.CompletionPercentage)
                .IsRequired()
                .HasPrecision(5, 2)
                .HasDefaultValue(0);

            builder.Property(m => m.CompletionCriteria)
                .HasMaxLength(4000);

            // Financial Impact
            builder.Property(m => m.PaymentAmount)
                .HasPrecision(18, 2);

            builder.Property(m => m.PaymentCurrency)
                .HasMaxLength(3);

            builder.Property(m => m.IsPaymentTriggered)
                .IsRequired()
                .HasDefaultValue(false);

            // Dependencies (JSON)
            builder.Property(m => m.PredecessorMilestones)
                .HasMaxLength(4000);

            builder.Property(m => m.SuccessorMilestones)
                .HasMaxLength(4000);

            // Approval
            builder.Property(m => m.RequiresApproval)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(m => m.IsApproved)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(m => m.ApprovedBy)
                .HasMaxLength(256);

            // Documentation
            builder.Property(m => m.Deliverables)
                .HasMaxLength(4000);

            builder.Property(m => m.AcceptanceCriteria)
                .HasMaxLength(4000);

            // Soft Delete
            builder.Property(m => m.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(m => m.DeletedBy)
                .HasMaxLength(256);

            // Audit properties
            builder.Property(m => m.CreatedAt)
                .IsRequired();

            builder.Property(m => m.CreatedBy)
                .HasMaxLength(256);

            builder.Property(m => m.UpdatedAt);

            builder.Property(m => m.UpdatedBy)
                .HasMaxLength(256);

            // Relationships
            builder.HasOne(m => m.Project)
                .WithMany(p => p.Milestones)
                .HasForeignKey(m => m.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.Phase)
                .WithMany(p => p.Milestones)
                .HasForeignKey(m => m.PhaseId)
                .OnDelete(DeleteBehavior.Restrict);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Milestones_CompletionPercentage", 
                    "[CompletionPercentage] >= 0 AND [CompletionPercentage] <= 100");
                t.HasCheckConstraint("CK_Milestones_PaymentAmount", 
                    "[PaymentAmount] IS NULL OR [PaymentAmount] > 0");
            });

            // Global query filter for soft delete
            builder.HasQueryFilter(m => !m.IsDeleted);
        }
    }
}