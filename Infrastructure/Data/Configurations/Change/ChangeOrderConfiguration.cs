using Domain.Entities.Change.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Change
{
    public class ChangeOrderConfiguration : IEntityTypeConfiguration<ChangeOrder>
    {
        public void Configure(EntityTypeBuilder<ChangeOrder> builder)
        {
            // Table name and schema
            builder.ToTable("ChangeOrders", "Change");

            // Primary key
            builder.HasKey(c => c.Id);

            // Indexes
            builder.HasIndex(c => c.ChangeOrderNumber).IsUnique();
            builder.HasIndex(c => c.ProjectId);
            builder.HasIndex(c => c.Status);
            builder.HasIndex(c => c.Type);
            builder.HasIndex(c => c.Category);
            builder.HasIndex(c => c.Priority);
            builder.HasIndex(c => c.RequestDate);
            builder.HasIndex(c => c.ControlAccountId);
            builder.HasIndex(c => c.CommitmentId);
            builder.HasIndex(c => c.WBSElementId);
            builder.HasIndex(c => new { c.ProjectId, c.Status });
            builder.HasIndex(c => new { c.ProjectId, c.Type });
            builder.HasIndex(c => c.IsDeleted);

            // Basic Information
            builder.Property(c => c.ChangeOrderNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(c => c.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.Description)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(c => c.Justification)
                .IsRequired()
                .HasMaxLength(2000);

            // Classification
            builder.Property(c => c.Type)
                .IsRequired();

            builder.Property(c => c.Category)
                .IsRequired();

            builder.Property(c => c.Priority)
                .IsRequired();

            builder.Property(c => c.Source)
                .IsRequired();

            // Impact Assessment
            builder.Property(c => c.EstimatedCostImpact)
                .IsRequired()
                .HasPrecision(18, 2)
                .HasDefaultValue(0);

            builder.Property(c => c.ScheduleImpactDays)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(c => c.ScopeImpact)
                .HasMaxLength(1000);

            builder.Property(c => c.QualityImpact)
                .HasMaxLength(1000);

            builder.Property(c => c.RiskImpact)
                .HasMaxLength(1000);

            // Financial Details
            builder.Property(c => c.ApprovedAmount)
                .HasPrecision(18, 2);

            builder.Property(c => c.Currency)
                .IsRequired()
                .HasMaxLength(3)
                .HasDefaultValue("USD");

            builder.Property(c => c.ContingencyUsed)
                .HasPrecision(18, 2);

            // Status and Workflow
            builder.Property(c => c.Status)
                .IsRequired();

            builder.Property(c => c.CurrentApprovalLevel)
                .HasMaxLength(100);

            builder.Property(c => c.ApprovalSequence)
                .IsRequired()
                .HasDefaultValue(0);

            // Request Information
            builder.Property(c => c.RequestedById)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(c => c.RequestDate)
                .IsRequired();

            builder.Property(c => c.RequestorDepartment)
                .HasMaxLength(100);

            // Approval Information
            builder.Property(c => c.SubmittedById)
                .HasMaxLength(256);

            builder.Property(c => c.SubmittedDate);

            builder.Property(c => c.ReviewedById)
                .HasMaxLength(256);

            builder.Property(c => c.ReviewedDate);

            builder.Property(c => c.ApprovedById)
                .HasMaxLength(256);

            builder.Property(c => c.ApprovalDate);

            builder.Property(c => c.RejectedById)
                .HasMaxLength(256);

            builder.Property(c => c.RejectionDate);

            builder.Property(c => c.RejectionReason)
                .HasMaxLength(1000);

            // Implementation
            builder.Property(c => c.ImplementationStartDate);
            builder.Property(c => c.ImplementationEndDate);

            builder.Property(c => c.ImplementationNotes)
                .HasMaxLength(2000);

            // Supporting Documentation
            builder.Property(c => c.AttachmentsJson)
                .HasMaxLength(4000)
                .HasComment("JSON array of document references");

            builder.Property(c => c.ImpactAnalysisDocument)
                .HasMaxLength(500);

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
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.ControlAccount)
                .WithMany()
                .HasForeignKey(c => c.ControlAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Commitment)
                .WithMany()
                .HasForeignKey(c => c.CommitmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.WBSElement)
                .WithMany()
                .HasForeignKey(c => c.WBSElementId)
                .OnDelete(DeleteBehavior.Restrict);

            // Note: RequestedById is a string (likely Entra ID) and cannot be linked to User.Id (Guid)
            builder.Ignore(c => c.RequestedBy);

            // Note: SubmittedById is a string (likely Entra ID) and cannot be linked to User.Id (Guid)
            builder.Ignore(c => c.SubmittedBy);

            // Note: ReviewedById is a string (likely Entra ID) and cannot be linked to User.Id (Guid)
            builder.Ignore(c => c.ReviewedBy);

            // Note: ApprovedById is a string (likely Entra ID) and cannot be linked to User.Id (Guid)
            builder.Ignore(c => c.ApprovedBy);

            // Note: RejectedById is a string (likely Entra ID) and cannot be linked to User.Id (Guid)
            builder.Ignore(c => c.RejectedBy);

            // Soft delete filter
            builder.HasQueryFilter(c => !c.IsDeleted);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_ChangeOrders_EstimatedCostImpact", 
                    "[EstimatedCostImpact] >= 0");
                t.HasCheckConstraint("CK_ChangeOrders_ApprovedAmount", 
                    "[ApprovedAmount] IS NULL OR [ApprovedAmount] >= 0");
                t.HasCheckConstraint("CK_ChangeOrders_ContingencyUsed", 
                    "[ContingencyUsed] IS NULL OR [ContingencyUsed] >= 0");
                t.HasCheckConstraint("CK_ChangeOrders_ScheduleImpactDays", 
                    "[ScheduleImpactDays] >= 0");
                t.HasCheckConstraint("CK_ChangeOrders_ImplementationDates", 
                    "[ImplementationEndDate] IS NULL OR [ImplementationStartDate] IS NULL OR [ImplementationEndDate] >= [ImplementationStartDate]");
            });
        }
    }
}