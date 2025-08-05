using Domain.Entities.Change.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Change
{
    public class ChangeRequestConfiguration : IEntityTypeConfiguration<ChangeRequest>
    {
        public void Configure(EntityTypeBuilder<ChangeRequest> builder)
        {
            // Table name and schema
            builder.ToTable("ChangeRequests", "Change");

            // Primary key
            builder.HasKey(c => c.Id);

            // Indexes
            builder.HasIndex(c => c.Code).IsUnique();
            builder.HasIndex(c => c.ProjectId);
            builder.HasIndex(c => c.Status);
            builder.HasIndex(c => c.Type);
            builder.HasIndex(c => c.Category);
            builder.HasIndex(c => c.Priority);
            builder.HasIndex(c => c.RequestDate);
            builder.HasIndex(c => c.RequiredByDate);
            builder.HasIndex(c => c.TrendId);
            builder.HasIndex(c => c.WBSElementId);
            builder.HasIndex(c => c.ControlAccountId);
            builder.HasIndex(c => c.ContractorId);
            builder.HasIndex(c => c.IsDeleted);
            builder.HasIndex(c => new { c.ProjectId, c.Status });
            builder.HasIndex(c => new { c.ProjectId, c.Type });

            // Basic Information
            builder.Property(c => c.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.Description)
                .IsRequired()
                .HasMaxLength(4000);

            builder.Property(c => c.Priority)
                .IsRequired()
                .HasMaxLength(50);

            // Classification
            builder.Property(c => c.Type)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.Category)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.Status)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.Source)
                .IsRequired()
                .HasMaxLength(50);

            // Affected Areas
            builder.Property(c => c.AffectedPackages)
                .HasMaxLength(4000)
                .HasComment("JSON array of package IDs");

            builder.Property(c => c.AffectedDisciplines)
                .HasMaxLength(4000)
                .HasComment("JSON array of discipline IDs");

            // Dates
            builder.Property(c => c.RequestDate)
                .IsRequired();

            builder.Property(c => c.RequiredByDate)
                .IsRequired();

            // Impact Assessment
            builder.Property(c => c.HasCostImpact)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(c => c.HasScheduleImpact)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(c => c.HasQualityImpact)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(c => c.HasScopeImpact)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(c => c.HasSafetyImpact)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(c => c.OverallImpactLevel)
                .IsRequired()
                .HasMaxLength(50);

            // Cost Impact
            builder.Property(c => c.EstimatedCostImpact)
                .HasPrecision(18, 2);

            builder.Property(c => c.ApprovedCostImpact)
                .HasPrecision(18, 2);

            builder.Property(c => c.Currency)
                .IsRequired()
                .HasMaxLength(3)
                .HasDefaultValue("USD");

            builder.Property(c => c.CostImpactType)
                .HasMaxLength(50);

            builder.Property(c => c.CostBreakdown)
                .HasMaxLength(4000)
                .HasComment("JSON structure");

            // Schedule Impact
            builder.Property(c => c.EstimatedScheduleImpactDays);
            builder.Property(c => c.ApprovedScheduleImpactDays);
            builder.Property(c => c.CurrentCompletionDate);
            builder.Property(c => c.ProposedCompletionDate);

            builder.Property(c => c.AffectsCriticalPath)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(c => c.AffectedMilestones)
                .HasMaxLength(4000)
                .HasComment("JSON array");

            // Technical Details
            builder.Property(c => c.TechnicalJustification)
                .HasMaxLength(4000);

            builder.Property(c => c.ProposedSolution)
                .HasMaxLength(4000);

            builder.Property(c => c.AlternativeOptions)
                .HasMaxLength(4000);

            builder.Property(c => c.RiskAssessment)
                .HasMaxLength(4000);

            builder.Property(c => c.QualityImpactDescription)
                .HasMaxLength(2000);

            builder.Property(c => c.SafetyImpactDescription)
                .HasMaxLength(2000);

            // Approval Workflow
            builder.Property(c => c.ApprovalRoute)
                .HasMaxLength(4000)
                .HasComment("JSON array of approval steps");

            builder.Property(c => c.CurrentApprovalLevel)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(c => c.RequiredApprovalLevel)
                .IsRequired()
                .HasDefaultValue(1);

            builder.Property(c => c.RequiresTechnicalReview)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(c => c.RequiresClientApproval)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(c => c.IsUrgent)
                .IsRequired()
                .HasDefaultValue(false);

            // Review Details
            builder.Property(c => c.TechnicalReviewComments)
                .HasMaxLength(2000);

            builder.Property(c => c.CostReviewComments)
                .HasMaxLength(2000);

            builder.Property(c => c.ScheduleReviewComments)
                .HasMaxLength(2000);

            // Approval Details
            builder.Property(c => c.ApprovalComments)
                .HasMaxLength(2000);

            builder.Property(c => c.ApprovalConditions)
                .HasMaxLength(2000);

            builder.Property(c => c.RejectionReason)
                .HasMaxLength(2000);

            // Client Details
            builder.Property(c => c.ClientReferenceNumber)
                .HasMaxLength(100);

            builder.Property(c => c.ClientComments)
                .HasMaxLength(2000);

            // Implementation
            builder.Property(c => c.IsImplemented)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(c => c.ActualCostImpact)
                .HasPrecision(18, 2);

            builder.Property(c => c.ImplementationNotes)
                .HasMaxLength(2000);

            builder.Property(c => c.LessonsLearned)
                .HasMaxLength(4000);

            builder.Property(c => c.IsConvertedToChangeOrder)
                .IsRequired()
                .HasDefaultValue(false);

            // Supporting Documentation
            builder.Property(c => c.DrawingReferences)
                .HasMaxLength(4000)
                .HasComment("JSON array");

            builder.Property(c => c.SpecificationReferences)
                .HasMaxLength(4000)
                .HasComment("JSON array");

            builder.Property(c => c.StandardReferences)
                .HasMaxLength(4000)
                .HasComment("JSON array");

            // Calculated properties - ignore
            builder.Ignore(c => c.DaysInReview);
            builder.Ignore(c => c.IsOverdue);
            builder.Ignore(c => c.CostVariance);

            // Soft Delete
            builder.Property(c => c.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(c => c.DeletedAt);

            builder.Property(c => c.DeletedBy)
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
            builder.HasOne(c => c.Project)
                .WithMany()
                .HasForeignKey(c => c.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Trend)
                .WithMany()
                .HasForeignKey(c => c.TrendId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Requestor)
                .WithMany()
                .HasForeignKey(c => c.RequestorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.WBSElement)
                .WithMany()
                .HasForeignKey(c => c.WBSElementId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.ControlAccount)
                .WithMany()
                .HasForeignKey(c => c.ControlAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Contractor)
                .WithMany()
                .HasForeignKey(c => c.ContractorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.TechnicalReviewer)
                .WithMany()
                .HasForeignKey(c => c.TechnicalReviewerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.CostReviewer)
                .WithMany()
                .HasForeignKey(c => c.CostReviewerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.ScheduleReviewer)
                .WithMany()
                .HasForeignKey(c => c.ScheduleReviewerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.ApprovedBy)
                .WithMany()
                .HasForeignKey(c => c.ApprovedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.ChangeOrder)
                .WithMany()
                .HasForeignKey(c => c.ChangeOrderId)
                .OnDelete(DeleteBehavior.Restrict);

            // Soft delete filter
            builder.HasQueryFilter(c => !c.IsDeleted);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_ChangeRequests_EstimatedCostImpact",
                    "[EstimatedCostImpact] IS NULL OR [EstimatedCostImpact] >= 0");
                t.HasCheckConstraint("CK_ChangeRequests_ApprovedCostImpact",
                    "[ApprovedCostImpact] IS NULL OR [ApprovedCostImpact] >= 0");
                t.HasCheckConstraint("CK_ChangeRequests_ActualCostImpact",
                    "[ActualCostImpact] IS NULL OR [ActualCostImpact] >= 0");
                t.HasCheckConstraint("CK_ChangeRequests_ScheduleImpactDays",
                    "[EstimatedScheduleImpactDays] IS NULL OR [EstimatedScheduleImpactDays] >= 0");
                t.HasCheckConstraint("CK_ChangeRequests_ApprovalLevels",
                    "[CurrentApprovalLevel] >= 0 AND [RequiredApprovalLevel] >= 1");
            });
        }
    }
}