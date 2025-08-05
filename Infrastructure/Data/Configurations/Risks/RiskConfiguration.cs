using Domain.Entities.Risks.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Risks
{
    public class RiskConfiguration : IEntityTypeConfiguration<Risk>
    {
        public void Configure(EntityTypeBuilder<Risk> builder)
        {
            // Table name and schema
            builder.ToTable("Risks", "Risks");

            // Primary key
            builder.HasKey(r => r.Id);

            // Indexes
            builder.HasIndex(r => r.Code).IsUnique();
            builder.HasIndex(r => r.ProjectId);
            builder.HasIndex(r => r.Type);
            builder.HasIndex(r => r.Status);
            builder.HasIndex(r => r.Category);
            builder.HasIndex(r => r.ResponseOwnerId);
            builder.HasIndex(r => r.IdentifiedById);
            builder.HasIndex(r => r.IdentifiedDate);
            builder.HasIndex(r => new { r.ProjectId, r.Status });
            builder.HasIndex(r => new { r.ProjectId, r.Type });

            // Basic Information
            builder.Property(r => r.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(r => r.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(r => r.Description)
                .IsRequired()
                .HasMaxLength(2000);

            // Risk Assessment
            builder.Property(r => r.Category)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(r => r.Type)
                .IsRequired();

            builder.Property(r => r.Probability)
                .IsRequired();

            builder.Property(r => r.Impact)
                .IsRequired();

            // Computed properties - ignore
            builder.Ignore(r => r.RiskScore);
            builder.Ignore(r => r.RiskLevel);
            builder.Ignore(r => r.ResidualRiskScore);
            builder.Ignore(r => r.ResidualRiskLevel);
            builder.Ignore(r => r.IsActive);

            // Risk Details
            builder.Property(r => r.Cause)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(r => r.Effect)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(r => r.CostImpact)
                .HasPrecision(18, 2);

            builder.Property(r => r.ScheduleImpact);

            builder.Property(r => r.QualityImpact)
                .HasMaxLength(500);

            // Risk Response
            builder.Property(r => r.ResponseStrategy);

            builder.Property(r => r.ResponsePlan)
                .HasMaxLength(2000);

            builder.Property(r => r.ResponseDueDate);

            builder.Property(r => r.ResponseCost)
                .HasPrecision(18, 2);

            // Residual Risk
            builder.Property(r => r.ResidualProbability);
            builder.Property(r => r.ResidualImpact);

            // Status and Tracking
            builder.Property(r => r.Status)
                .IsRequired();

            builder.Property(r => r.IdentifiedDate)
                .IsRequired();

            builder.Property(r => r.ClosedDate);

            builder.Property(r => r.ClosureReason)
                .HasMaxLength(500);

            // Monitoring
            builder.Property(r => r.LastReviewDate);
            builder.Property(r => r.NextReviewDate);

            builder.Property(r => r.TriggerIndicators)
                .HasMaxLength(1000);

            // Audit properties
            builder.Property(r => r.CreatedAt)
                .IsRequired();

            builder.Property(r => r.CreatedBy)
                .HasMaxLength(256);

            builder.Property(r => r.UpdatedAt);

            builder.Property(r => r.UpdatedBy)
                .HasMaxLength(256);

            builder.Property(r => r.DeletedAt);

            builder.Property(r => r.DeletedBy)
                .HasMaxLength(256);

            // Relationships
            builder.HasOne(r => r.Project)
                .WithMany()
                .HasForeignKey(r => r.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.ResponseOwner)
                .WithMany()
                .HasForeignKey(r => r.ResponseOwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.IdentifiedBy)
                .WithMany()
                .HasForeignKey(r => r.IdentifiedById)
                .OnDelete(DeleteBehavior.Restrict);

            // Soft delete
            builder.HasQueryFilter(r => r.DeletedAt == null);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Risks_Probability", "[Probability] >= 1 AND [Probability] <= 5");
                t.HasCheckConstraint("CK_Risks_Impact", "[Impact] >= 1 AND [Impact] <= 5");
                t.HasCheckConstraint("CK_Risks_ResidualProbability", 
                    "[ResidualProbability] IS NULL OR ([ResidualProbability] >= 1 AND [ResidualProbability] <= 5)");
                t.HasCheckConstraint("CK_Risks_ResidualImpact", 
                    "[ResidualImpact] IS NULL OR ([ResidualImpact] >= 1 AND [ResidualImpact] <= 5)");
                t.HasCheckConstraint("CK_Risks_CostImpact", "[CostImpact] IS NULL OR [CostImpact] >= 0");
                t.HasCheckConstraint("CK_Risks_ScheduleImpact", "[ScheduleImpact] IS NULL OR [ScheduleImpact] >= 0");
                t.HasCheckConstraint("CK_Risks_ResponseCost", "[ResponseCost] IS NULL OR [ResponseCost] >= 0");
                t.HasCheckConstraint("CK_Risks_ClosedDate", "[ClosedDate] IS NULL OR [ClosedDate] >= [IdentifiedDate]");
            });
        }
    }
}