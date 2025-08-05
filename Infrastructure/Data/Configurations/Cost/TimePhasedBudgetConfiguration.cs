using Domain.Entities.Cost.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Cost
{
    public class TimePhasedBudgetConfiguration : IEntityTypeConfiguration<TimePhasedBudget>
    {
        public void Configure(EntityTypeBuilder<TimePhasedBudget> builder)
        {
            // Table name and schema
            builder.ToTable("TimePhasedBudgets", "Cost");

            // Primary key
            builder.HasKey(t => t.Id);

            // Indexes
            builder.HasIndex(t => t.ControlAccountId);
            builder.HasIndex(t => t.PeriodStart);
            builder.HasIndex(t => t.PeriodEnd);
            builder.HasIndex(t => t.PeriodType);
            builder.HasIndex(t => new { t.ControlAccountId, t.PeriodStart });

            // Period Information
            builder.Property(t => t.PeriodStart)
                .IsRequired();

            builder.Property(t => t.PeriodEnd)
                .IsRequired();

            builder.Property(t => t.PeriodType)
                .IsRequired()
                .HasMaxLength(50);

            // Budget values
            builder.Property(t => t.PlannedValue)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(t => t.CumulativePlannedValue)
                .IsRequired()
                .HasPrecision(18, 2);

            // Resource allocation
            builder.Property(t => t.PlannedLaborHours)
                .IsRequired()
                .HasPrecision(10, 2);

            builder.Property(t => t.PlannedLaborCost)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(t => t.PlannedMaterialCost)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(t => t.PlannedEquipmentCost)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(t => t.PlannedSubcontractCost)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(t => t.PlannedOtherCost)
                .IsRequired()
                .HasPrecision(18, 2);

            // Status
            builder.Property(t => t.IsBaseline)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(t => t.RevisionNumber);

            // Calculated property (ignored)
            builder.Ignore(t => t.TotalPlannedCost);

            // Audit properties
            builder.Property(t => t.CreatedAt)
                .IsRequired();

            builder.Property(t => t.CreatedBy)
                .HasMaxLength(256);

            builder.Property(t => t.UpdatedAt);

            builder.Property(t => t.UpdatedBy)
                .HasMaxLength(256);

            // Relationships
            builder.HasOne(t => t.ControlAccount)
                .WithMany(ca => ca.TimePhasedBudgets)
                .HasForeignKey(t => t.ControlAccountId)
                .OnDelete(DeleteBehavior.Cascade);

            // Check constraints
            builder.ToTable(tb =>
            {
                tb.HasCheckConstraint("CK_TimePhasedBudgets_Period", "[PeriodEnd] >= [PeriodStart]");
                tb.HasCheckConstraint("CK_TimePhasedBudgets_PlannedValue", "[PlannedValue] >= 0");
                tb.HasCheckConstraint("CK_TimePhasedBudgets_CumulativePlannedValue", "[CumulativePlannedValue] >= 0");
                tb.HasCheckConstraint("CK_TimePhasedBudgets_PlannedCosts", 
                    "[PlannedLaborCost] >= 0 AND [PlannedMaterialCost] >= 0 AND " +
                    "[PlannedEquipmentCost] >= 0 AND [PlannedSubcontractCost] >= 0 AND [PlannedOtherCost] >= 0");
                tb.HasCheckConstraint("CK_TimePhasedBudgets_PlannedLaborHours", "[PlannedLaborHours] >= 0");
            });
        }
    }
}