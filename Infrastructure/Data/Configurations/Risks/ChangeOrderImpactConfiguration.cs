using Domain.Entities.Risks.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Risks
{
    public class ChangeOrderImpactConfiguration : IEntityTypeConfiguration<ChangeOrderImpact>
    {
        public void Configure(EntityTypeBuilder<ChangeOrderImpact> builder)
        {
            // Table name and schema
            builder.ToTable("ChangeOrderImpacts", "Risks");

            // Primary key
            builder.HasKey(c => c.Id);

            // Indexes
            builder.HasIndex(c => c.ChangeOrderId);
            builder.HasIndex(c => c.Area);
            builder.HasIndex(c => c.Severity);
            builder.HasIndex(c => new { c.ChangeOrderId, c.Area });

            // Basic Information
            builder.Property(c => c.ChangeOrderId)
                .IsRequired();

            builder.Property(c => c.Area)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.Severity)
                .IsRequired()
                .HasMaxLength(50);

            // Impact Details
            builder.Property(c => c.Description)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(c => c.CostImpact)
                .HasPrecision(18, 2);

            builder.Property(c => c.ScheduleImpactDays);

            builder.Property(c => c.AffectedWBSCodes)
                .HasMaxLength(4000)
                .HasComment("JSON array of affected WBS codes");

            builder.Property(c => c.AffectedStakeholders)
                .HasMaxLength(4000)
                .HasComment("JSON array of affected stakeholder IDs");

            // Mitigation
            builder.Property(c => c.MitigationPlan)
                .HasMaxLength(2000);

            builder.Property(c => c.MitigationCost)
                .HasPrecision(18, 2);

            // Audit properties
            builder.Property(c => c.CreatedAt)
                .IsRequired();

            builder.Property(c => c.CreatedBy)
                .HasMaxLength(256);

            builder.Property(c => c.UpdatedAt);

            builder.Property(c => c.UpdatedBy)
                .HasMaxLength(256);

            // Relationships
            builder.HasOne(c => c.ChangeOrder)
                .WithMany()
                .HasForeignKey(c => c.ChangeOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_ChangeOrderImpacts_CostImpact", 
                    "[CostImpact] IS NULL OR [CostImpact] >= 0");
                t.HasCheckConstraint("CK_ChangeOrderImpacts_ScheduleImpact", 
                    "[ScheduleImpactDays] IS NULL OR [ScheduleImpactDays] >= 0");
                t.HasCheckConstraint("CK_ChangeOrderImpacts_MitigationCost", 
                    "[MitigationCost] IS NULL OR [MitigationCost] >= 0");
            });
        }
    }
}