using Domain.Entities.Risks.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Risks
{
    public class RiskResponseConfiguration : IEntityTypeConfiguration<RiskResponse>
    {
        public void Configure(EntityTypeBuilder<RiskResponse> builder)
        {
            // Table name and schema
            builder.ToTable("RiskResponses", "Risks");

            // Primary key
            builder.HasKey(r => r.Id);

            // Indexes
            builder.HasIndex(r => r.RiskId);
            builder.HasIndex(r => r.OwnerId);
            builder.HasIndex(r => r.Status);
            builder.HasIndex(r => r.DueDate);
            builder.HasIndex(r => new { r.RiskId, r.Status });

            // Foreign Keys
            builder.Property(r => r.RiskId)
                .IsRequired();

            builder.Property(r => r.OwnerId)
                .IsRequired();

            // Response Details
            builder.Property(r => r.Strategy)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(r => r.Description)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(r => r.Actions)
                .IsRequired()
                .HasMaxLength(4000);

            // Planning
            builder.Property(r => r.DueDate)
                .IsRequired();

            builder.Property(r => r.EstimatedCost)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(r => r.ActualCost)
                .HasPrecision(18, 2);

            // Expected outcome
            builder.Property(r => r.ExpectedProbability)
                .IsRequired();

            builder.Property(r => r.ExpectedImpact)
                .IsRequired();

            // Computed property - ignore
            builder.Ignore(r => r.ExpectedRiskScore);

            // Status tracking
            builder.Property(r => r.Status)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(r => r.StartDate);

            builder.Property(r => r.CompletedDate);

            builder.Property(r => r.CompletionNotes)
                .HasMaxLength(1000);

            // Effectiveness
            builder.Property(r => r.EffectivenessScore)
                .HasPrecision(5, 2);

            builder.Property(r => r.LessonsLearned)
                .HasMaxLength(2000);

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
                .WithMany(risk => risk.Responses)
                .HasForeignKey(r => r.RiskId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.Owner)
                .WithMany()
                .HasForeignKey(r => r.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Soft delete
            builder.HasQueryFilter(r => r.DeletedAt == null);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_RiskResponses_ExpectedProbability", 
                    "[ExpectedProbability] >= 1 AND [ExpectedProbability] <= 5");
                t.HasCheckConstraint("CK_RiskResponses_ExpectedImpact", 
                    "[ExpectedImpact] >= 1 AND [ExpectedImpact] <= 5");
                t.HasCheckConstraint("CK_RiskResponses_EstimatedCost", "[EstimatedCost] >= 0");
                t.HasCheckConstraint("CK_RiskResponses_ActualCost", "[ActualCost] IS NULL OR [ActualCost] >= 0");
                t.HasCheckConstraint("CK_RiskResponses_EffectivenessScore", 
                    "[EffectivenessScore] IS NULL OR ([EffectivenessScore] >= 0 AND [EffectivenessScore] <= 100)");
                t.HasCheckConstraint("CK_RiskResponses_CompletedDate", 
                    "[CompletedDate] IS NULL OR [CompletedDate] >= [StartDate]");
            });
        }
    }
}