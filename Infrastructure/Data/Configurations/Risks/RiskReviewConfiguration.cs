using Domain.Entities.Risks.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Risks
{
    public class RiskReviewConfiguration : IEntityTypeConfiguration<RiskReview>
    {
        public void Configure(EntityTypeBuilder<RiskReview> builder)
        {
            // Table name and schema
            builder.ToTable("RiskReviews", "Risks");

            // Primary key
            builder.HasKey(r => r.Id);

            // Indexes
            builder.HasIndex(r => r.RiskId);
            builder.HasIndex(r => r.ReviewedById);
            builder.HasIndex(r => r.ReviewDate);
            builder.HasIndex(r => new { r.RiskId, r.ReviewDate });

            // Foreign Keys
            builder.Property(r => r.RiskId)
                .IsRequired();

            builder.Property(r => r.ReviewedById)
                .IsRequired();

            // Review Information
            builder.Property(r => r.ReviewDate)
                .IsRequired();

            // Assessment Changes
            builder.Property(r => r.PreviousProbability)
                .IsRequired();

            builder.Property(r => r.PreviousImpact)
                .IsRequired();

            builder.Property(r => r.NewProbability)
                .IsRequired();

            builder.Property(r => r.NewImpact)
                .IsRequired();

            // Review Details
            builder.Property(r => r.ReviewNotes)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(r => r.ChangesIdentified)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(r => r.ActionsRequired)
                .IsRequired()
                .HasMaxLength(2000);

            // Status Tracking
            builder.Property(r => r.StatusChanged)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(r => r.PreviousStatus)
                .HasMaxLength(50);

            builder.Property(r => r.NewStatus)
                .HasMaxLength(50);

            // Response Strategy
            builder.Property(r => r.ResponseStrategyChanged)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(r => r.UpdatedResponsePlan)
                .HasMaxLength(2000);

            // Next Review
            builder.Property(r => r.NextReviewDate);

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
            builder.HasOne(r => r.Risk)
                .WithMany(risk => risk.Reviews)
                .HasForeignKey(r => r.RiskId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.ReviewedBy)
                .WithMany()
                .HasForeignKey(r => r.ReviewedById)
                .OnDelete(DeleteBehavior.Restrict);

            // Soft delete
            builder.HasQueryFilter(r => r.DeletedAt == null);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_RiskReviews_PreviousProbability", 
                    "[PreviousProbability] >= 1 AND [PreviousProbability] <= 5");
                t.HasCheckConstraint("CK_RiskReviews_PreviousImpact", 
                    "[PreviousImpact] >= 1 AND [PreviousImpact] <= 5");
                t.HasCheckConstraint("CK_RiskReviews_NewProbability", 
                    "[NewProbability] >= 1 AND [NewProbability] <= 5");
                t.HasCheckConstraint("CK_RiskReviews_NewImpact", 
                    "[NewImpact] >= 1 AND [NewImpact] <= 5");
                t.HasCheckConstraint("CK_RiskReviews_NextReviewDate", 
                    "[NextReviewDate] IS NULL OR [NextReviewDate] > [ReviewDate]");
            });
        }
    }
}