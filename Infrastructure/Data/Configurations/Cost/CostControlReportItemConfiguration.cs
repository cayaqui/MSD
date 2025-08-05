using Domain.Entities.Cost.Core;

namespace Infrastructure.Data.Configurations.Cost
{
    public class CostControlReportItemConfiguration : IEntityTypeConfiguration<CostControlReportItem>
    {
        public void Configure(EntityTypeBuilder<CostControlReportItem> builder)
        {
            // Table name and schema
            builder.ToTable("CostControlReportItems", "Cost");

            // Primary key
            builder.HasKey(c => c.Id);

            // Indexes
            builder.HasIndex(c => c.CostControlReportId);
            builder.HasIndex(c => c.WBSElementId);
            builder.HasIndex(c => c.WorkPackageId);
            builder.HasIndex(c => c.SequenceNumber);
            builder.HasIndex(c => new { c.CostControlReportId, c.SequenceNumber });

            // Item identification
            builder.Property(c => c.ItemCode)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.Description)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(c => c.SequenceNumber)
                .IsRequired();

            // 9 Column Values
            builder.Property(c => c.BudgetedCost)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(c => c.PhysicalProgressPercentage)
                .IsRequired()
                .HasPrecision(5, 2);

            builder.Property(c => c.EarnedValue)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(c => c.ActualCost)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(c => c.CostVariance)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(c => c.ScheduleVariance)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(c => c.CostPerformanceIndex)
                .IsRequired()
                .HasPrecision(5, 3);

            builder.Property(c => c.EstimateAtCompletion)
                .IsRequired()
                .HasPrecision(18, 2);

            // Additional tracking
            builder.Property(c => c.ResponsiblePerson)
                .HasMaxLength(256);

            builder.Property(c => c.CostCategory)
                .HasMaxLength(100);

            builder.Property(c => c.IsCritical)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(c => c.VarianceExplanation)
                .HasMaxLength(2000);

            // Audit properties
            builder.Property(c => c.CreatedAt)
                .IsRequired();

            builder.Property(c => c.CreatedBy)
                .HasMaxLength(256);

            builder.Property(c => c.UpdatedAt);

            builder.Property(c => c.UpdatedBy)
                .HasMaxLength(256);

            // Relationships
            builder.HasOne(c => c.CostControlReport)
                .WithMany(r => r.Items)
                .HasForeignKey(c => c.CostControlReportId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.WBSElement)
                .WithMany()
                .HasForeignKey(c => c.WBSElementId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.WorkPackage)
                .WithMany()
                .HasForeignKey(c => c.WorkPackageId)
                .OnDelete(DeleteBehavior.Restrict);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_CostControlReportItems_Amounts", 
                    "[BudgetedCost] >= 0 AND [ActualCost] >= 0 AND [EstimateAtCompletion] >= 0");
                t.HasCheckConstraint("CK_CostControlReportItems_Progress", 
                    "[PhysicalProgressPercentage] >= 0 AND [PhysicalProgressPercentage] <= 100");
                t.HasCheckConstraint("CK_CostControlReportItems_CPI", 
                    "[CostPerformanceIndex] >= 0");
                t.HasCheckConstraint("CK_CostControlReportItems_SequenceNumber", 
                    "[SequenceNumber] > 0");
                t.HasCheckConstraint("CK_CostControlReportItems_OneLink", 
                    "([WBSElementId] IS NULL AND [WorkPackageId] IS NOT NULL) OR " +
                    "([WBSElementId] IS NOT NULL AND [WorkPackageId] IS NULL) OR " +
                    "([WBSElementId] IS NULL AND [WorkPackageId] IS NULL)");
            });
        }
    }
}