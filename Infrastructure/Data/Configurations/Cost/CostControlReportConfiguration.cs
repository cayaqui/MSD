using Domain.Entities.Cost.Core;

namespace Infrastructure.Data.Configurations.Cost
{
    public class CostControlReportConfiguration : IEntityTypeConfiguration<CostControlReport>
    {
        public void Configure(EntityTypeBuilder<CostControlReport> builder)
        {
            // Table name and schema
            builder.ToTable("CostControlReports", "Cost");

            // Primary key
            builder.HasKey(c => c.Id);

            // Indexes
            builder.HasIndex(c => c.ProjectId);
            builder.HasIndex(c => c.ControlAccountId);
            builder.HasIndex(c => c.ReportDate);
            builder.HasIndex(c => c.PeriodType);
            builder.HasIndex(c => new { c.ProjectId, c.ReportDate });
            builder.HasIndex(c => new { c.ControlAccountId, c.ReportDate });

            // Report Information
            builder.Property(c => c.ReportDate)
                .IsRequired();

            builder.Property(c => c.PeriodType)
                .IsRequired()
                .HasMaxLength(50);

            // 9-Column Report Metrics
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

            // Additional Chilean context metrics
            builder.Property(c => c.ExchangeRateUSD)
                .HasPrecision(12, 6);

            builder.Property(c => c.UFValue)
                .HasPrecision(12, 6);

            builder.Property(c => c.ImportedMaterialsPercentage)
                .HasPrecision(5, 2);

            // Additional EVM metrics
            builder.Property(c => c.VarianceAtCompletion)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(c => c.ToCompletePerformanceIndex)
                .HasPrecision(5, 3);

            builder.Property(c => c.SchedulePerformanceIndex)
                .IsRequired()
                .HasPrecision(5, 3);

            builder.Property(c => c.EstimateToComplete)
                .IsRequired()
                .HasPrecision(18, 2);

            // Report metadata
            builder.Property(c => c.Notes)
                .HasMaxLength(4000);

            builder.Property(c => c.ApprovedBy)
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
                .WithMany(p => p.CostControlReports)
                .HasForeignKey(c => c.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.ControlAccount)
                .WithMany(ca => ca.CostControlReports)
                .HasForeignKey(c => c.ControlAccountId)
                .OnDelete(DeleteBehavior.Restrict);

          
            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_CostControlReports_Amounts", 
                    "[BudgetedCost] >= 0 AND [ActualCost] >= 0 AND [EstimateAtCompletion] >= 0 AND [EstimateToComplete] >= 0");
                t.HasCheckConstraint("CK_CostControlReports_Progress", 
                    "[PhysicalProgressPercentage] >= 0 AND [PhysicalProgressPercentage] <= 100");
                t.HasCheckConstraint("CK_CostControlReports_Indices", 
                    "[CostPerformanceIndex] >= 0 AND [SchedulePerformanceIndex] >= 0");
                t.HasCheckConstraint("CK_CostControlReports_TCPI", 
                    "[ToCompletePerformanceIndex] IS NULL OR [ToCompletePerformanceIndex] >= 0");
                t.HasCheckConstraint("CK_CostControlReports_ImportedMaterials", 
                    "[ImportedMaterialsPercentage] IS NULL OR ([ImportedMaterialsPercentage] >= 0 AND [ImportedMaterialsPercentage] <= 100)");
            });
        }
    }
}