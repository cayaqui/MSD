using Domain.Entities.Cost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Cost;

/// <summary>
/// Entity configuration for CBS (Cost Breakdown Structure)
/// </summary>
public class CBSConfiguration : IEntityTypeConfiguration<CBS>
{
    public void Configure(EntityTypeBuilder<CBS> builder)
    {
        // Table name and schema with check constraints
        builder.ToTable("CBS", "Cost", t =>
        {
            t.HasCheckConstraint("CK_CBS_Level",
                "[Level] >= 1 AND [Level] <= 10");
            t.HasCheckConstraint("CK_CBS_SequenceNumber",
                "[SequenceNumber] > 0");
            t.HasCheckConstraint("CK_CBS_OriginalBudget",
                "[OriginalBudget] >= 0");
            t.HasCheckConstraint("CK_CBS_CommittedCost",
                "[CommittedCost] >= 0");
            t.HasCheckConstraint("CK_CBS_ActualCost",
                "[ActualCost] >= 0");
            t.HasCheckConstraint("CK_CBS_ForecastCost",
                "[ForecastCost] >= 0");
            t.HasCheckConstraint("CK_CBS_AllocationPercentage",
                "[AllocationPercentage] IS NULL OR ([AllocationPercentage] >= 0 AND [AllocationPercentage] <= 100)");
            t.HasCheckConstraint("CK_CBS_EstimateAccuracy",
                "[EstimateAccuracyLow] IS NULL OR [EstimateAccuracyHigh] IS NULL OR " +
                "([EstimateAccuracyLow] <= 0 AND [EstimateAccuracyHigh] >= 0)");
            t.HasCheckConstraint("CK_CBS_Code_Format",
                "[Code] LIKE '[0-9][0-9]%'"); // Must start with two digits
        });

        // Primary key
        builder.HasKey(cbs => cbs.Id);

        // Indexes
        builder.HasIndex(cbs => new { cbs.ProjectId, cbs.Code }).IsUnique();
        builder.HasIndex(cbs => cbs.ProjectId);
        builder.HasIndex(cbs => cbs.ParentId);
        builder.HasIndex(cbs => new { cbs.Level, cbs.SequenceNumber });
        builder.HasIndex(cbs => cbs.Category);
        builder.HasIndex(cbs => cbs.CostType);
        builder.HasIndex(cbs => cbs.IsDeleted);
        builder.HasIndex(cbs => new { cbs.IsActive, cbs.IsControlPoint });
        builder.HasIndex(cbs => cbs.IsLeafNode);
        builder.HasIndex(cbs => cbs.CostAccountCode);
        builder.HasIndex(cbs => cbs.AccountingCode);

        // Properties
        builder.Property(cbs => cbs.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(cbs => cbs.Name)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(cbs => cbs.Description)
            .HasMaxLength(1000);

        builder.Property(cbs => cbs.Level)
            .IsRequired();

        builder.Property(cbs => cbs.SequenceNumber)
            .IsRequired();

        builder.Property(cbs => cbs.FullPath)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(cbs => cbs.Category)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(cbs => cbs.CostType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(cbs => cbs.IsControlPoint)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(cbs => cbs.IsLeafNode)
            .IsRequired()
            .HasDefaultValue(true);

        // Cost Account Mapping
        builder.Property(cbs => cbs.CostAccountCode)
            .HasMaxLength(50);

        builder.Property(cbs => cbs.AccountingCode)
            .HasMaxLength(50);

        builder.Property(cbs => cbs.CostCenter)
            .HasMaxLength(50);

        // Estimation Class (AACE)
        builder.Property(cbs => cbs.EstimateClass)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(cbs => cbs.EstimateAccuracyLow)
            .HasPrecision(5, 2);

        builder.Property(cbs => cbs.EstimateAccuracyHigh)
            .HasPrecision(5, 2);

        // Budget Information
        builder.Property(cbs => cbs.OriginalBudget)
            .HasPrecision(18, 2)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(cbs => cbs.ApprovedChanges)
            .HasPrecision(18, 2)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(cbs => cbs.CommittedCost)
            .HasPrecision(18, 2)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(cbs => cbs.ActualCost)
            .HasPrecision(18, 2)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(cbs => cbs.ForecastCost)
            .HasPrecision(18, 2)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(cbs => cbs.Currency)
            .IsRequired()
            .HasMaxLength(3)
            .HasDefaultValue("USD");

        // Computed columns for calculated fields
        builder.Property(cbs => cbs.CurrentBudget)
            .HasComputedColumnSql("[OriginalBudget] + [ApprovedChanges]", stored: false);

        builder.Property(cbs => cbs.CostVariance)
            .HasComputedColumnSql("[OriginalBudget] + [ApprovedChanges] - [ForecastCost]", stored: false);

        builder.Property(cbs => cbs.CostVariancePercentage)
            .HasComputedColumnSql(
                "CASE WHEN ([OriginalBudget] + [ApprovedChanges]) = 0 THEN 0 " +
                "ELSE (([OriginalBudget] + [ApprovedChanges] - [ForecastCost]) / ([OriginalBudget] + [ApprovedChanges])) * 100 END",
                stored: false);

        // Allocation
        builder.Property(cbs => cbs.AllocationPercentage)
            .HasPrecision(5, 2);

        builder.Property(cbs => cbs.AllocationBasis)
            .HasMaxLength(100);

        // Time-phased Budget
        builder.Property(cbs => cbs.IsTimePhased)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(cbs => cbs.TimePhasedData)
            .HasMaxLength(4000); // JSON data

        // Audit properties
        builder.Property(cbs => cbs.CreatedAt)
            .IsRequired();

        builder.Property(cbs => cbs.CreatedBy)
            .HasMaxLength(256);

        builder.Property(cbs => cbs.UpdatedAt);

        builder.Property(cbs => cbs.UpdatedBy)
            .HasMaxLength(256);

        // Soft delete properties
        builder.Property(cbs => cbs.DeletedAt);

        builder.Property(cbs => cbs.DeletedBy)
            .HasMaxLength(256);

        // Foreign key relationships
        builder.HasOne(cbs => cbs.Project)
            .WithMany(p => p.CBSElements)
            .HasForeignKey(cbs => cbs.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cbs => cbs.Parent)
            .WithMany(cbs => cbs.Children)
            .HasForeignKey(cbs => cbs.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Navigation properties
        builder.HasMany(cbs => cbs.Children)
            .WithOne(cbs => cbs.Parent)
            .HasForeignKey(cbs => cbs.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Global query filter for soft delete
        builder.HasQueryFilter(cbs => !cbs.IsDeleted);
    }
}