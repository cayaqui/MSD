using Domain.Entities.Change.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Change
{
    public class TrendConfiguration : IEntityTypeConfiguration<Trend>
    {
        public void Configure(EntityTypeBuilder<Trend> builder)
        {
            // Table name and schema
            builder.ToTable("Trends", "Change");

            // Primary key
            builder.HasKey(t => t.Id);

            // Indexes
            builder.HasIndex(t => t.Code).IsUnique();
            builder.HasIndex(t => t.ProjectId);
            builder.HasIndex(t => t.Status);
            builder.HasIndex(t => t.Category);
            builder.HasIndex(t => t.Type);
            builder.HasIndex(t => t.Priority);
            builder.HasIndex(t => t.IdentifiedDate);
            builder.HasIndex(t => t.DueDate);
            builder.HasIndex(t => t.WBSElementId);
            builder.HasIndex(t => t.IsDeleted);
            builder.HasIndex(t => new { t.ProjectId, t.Status });

            // Basic Information
            builder.Property(t => t.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(4000);

            // Classification
            builder.Property(t => t.Category)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(t => t.Type)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(t => t.Status)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(t => t.Priority)
                .IsRequired()
                .HasMaxLength(50);

            // Dates
            builder.Property(t => t.IdentifiedDate)
                .IsRequired();

            builder.Property(t => t.DueDate)
                .IsRequired();

            // Impact Assessment
            builder.Property(t => t.EstimatedCostImpact)
                .HasPrecision(18, 2);

            builder.Property(t => t.EstimatedScheduleImpactDays);

            builder.Property(t => t.QualityImpact)
                .HasMaxLength(2000);

            builder.Property(t => t.ScopeImpact)
                .HasMaxLength(2000);

            builder.Property(t => t.ImpactLevel)
                .IsRequired()
                .HasMaxLength(50);

            // Financial Details
            builder.Property(t => t.MinCostImpact)
                .HasPrecision(18, 2);

            builder.Property(t => t.MaxCostImpact)
                .HasPrecision(18, 2);

            builder.Property(t => t.MostLikelyCostImpact)
                .HasPrecision(18, 2);

            builder.Property(t => t.Currency)
                .IsRequired()
                .HasMaxLength(3)
                .HasDefaultValue("USD");

            builder.Property(t => t.Probability)
                .IsRequired()
                .HasPrecision(5, 2)
                .HasDefaultValue(50);

            // Calculated property - ignore
            builder.Ignore(t => t.ExpectedValue);
            builder.Ignore(t => t.DaysOpen);
            builder.Ignore(t => t.IsOverdue);

            // Decision
            builder.Property(t => t.Decision)
                .HasMaxLength(50);

            builder.Property(t => t.DecisionRationale)
                .HasMaxLength(2000);

            builder.Property(t => t.MitigationStrategy)
                .HasMaxLength(2000);

            builder.Property(t => t.IsConvertedToChangeOrder)
                .IsRequired()
                .HasDefaultValue(false);

            // Documentation
            builder.Property(t => t.RootCause)
                .HasMaxLength(2000);

            builder.Property(t => t.ProposedSolution)
                .HasMaxLength(4000);

            builder.Property(t => t.AlternativeSolutions)
                .HasMaxLength(4000);

            builder.Property(t => t.Assumptions)
                .HasMaxLength(2000);

            builder.Property(t => t.Risks)
                .HasMaxLength(2000);

            // Approval Workflow
            builder.Property(t => t.RequiresClientApproval)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(t => t.ClientApproved);

            builder.Property(t => t.ClientApprovalDate);

            builder.Property(t => t.ClientComments)
                .HasMaxLength(2000);

            // Soft Delete
            builder.Property(t => t.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(t => t.DeletedAt);

            builder.Property(t => t.DeletedBy)
                .HasMaxLength(256);

            // Audit properties
            builder.Property(t => t.CreatedAt)
                .IsRequired();

            builder.Property(t => t.CreatedBy)
                .HasMaxLength(256);

            builder.Property(t => t.UpdatedAt);

            builder.Property(t => t.UpdatedBy)
                .HasMaxLength(256);

            // Relationships
            builder.HasOne(t => t.Project)
                .WithMany()
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(t => t.WBSElement)
                .WithMany()
                .HasForeignKey(t => t.WBSElementId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.RaisedByUser)
                .WithMany()
                .HasForeignKey(t => t.RaisedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.DecisionByUser)
                .WithMany()
                .HasForeignKey(t => t.DecisionByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.ChangeOrder)
                .WithMany()
                .HasForeignKey(t => t.ChangeOrderId)
                .OnDelete(DeleteBehavior.Restrict);

            // Soft delete filter
            builder.HasQueryFilter(t => !t.IsDeleted);

            // Check constraints
            builder.ToTable(tb =>
            {
                tb.HasCheckConstraint("CK_Trends_EstimatedCostImpact",
                    "[EstimatedCostImpact] IS NULL OR [EstimatedCostImpact] >= 0");
                tb.HasCheckConstraint("CK_Trends_MinMaxCostImpact",
                    "[MinCostImpact] IS NULL OR [MaxCostImpact] IS NULL OR [MinCostImpact] <= [MaxCostImpact]");
                tb.HasCheckConstraint("CK_Trends_Probability",
                    "[Probability] >= 0 AND [Probability] <= 100");
                tb.HasCheckConstraint("CK_Trends_EstimatedScheduleImpact",
                    "[EstimatedScheduleImpactDays] IS NULL OR [EstimatedScheduleImpactDays] >= 0");
            });
        }
    }
}