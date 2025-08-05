using Domain.Entities.Cost.Core;
using Domain.Entities.Cost.Budget;

namespace Infrastructure.Data.Configurations.Cost
{
    public class CBSConfiguration : IEntityTypeConfiguration<CBS>
    {
        public void Configure(EntityTypeBuilder<CBS> builder)
        {
            // Table name and schema
            builder.ToTable("CBS", "Cost");

            // Primary key
            builder.HasKey(c => c.Id);

            // Indexes
            builder.HasIndex(c => c.Code).IsUnique();
            builder.HasIndex(c => c.ProjectId);
            builder.HasIndex(c => c.ParentId);
            builder.HasIndex(c => c.Level);
            builder.HasIndex(c => c.Category);
            builder.HasIndex(c => c.CostType);
            builder.HasIndex(c => c.IsControlPoint);
            builder.HasIndex(c => c.IsLeafNode);
            builder.HasIndex(c => c.IsDeleted);
            builder.HasIndex(c => c.IsActive);
            builder.HasIndex(c => new { c.ProjectId, c.Code }).IsUnique();
            builder.HasIndex(c => new { c.ProjectId, c.Level });
            builder.HasIndex(c => new { c.ParentId, c.SequenceNumber });

            // Basic Information
            builder.Property(c => c.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(c => c.Description)
                .HasMaxLength(2000);

            // Hierarchy
            builder.Property(c => c.Level)
                .IsRequired();

            builder.Property(c => c.SequenceNumber)
                .IsRequired();

            builder.Property(c => c.FullPath)
                .IsRequired()
                .HasMaxLength(2048);

            // Classification
            builder.Property(c => c.Category)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.CostType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.IsControlPoint)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(c => c.IsLeafNode)
                .IsRequired()
                .HasDefaultValue(true);

            // Cost Account Mapping
            builder.Property(c => c.CostAccountCode)
                .HasMaxLength(50);

            builder.Property(c => c.AccountingCode)
                .HasMaxLength(50);

            builder.Property(c => c.CostCenter)
                .HasMaxLength(50);

            // Estimation Class
            builder.Property(c => c.EstimateClass)
                .HasConversion<int>();

            builder.Property(c => c.EstimateAccuracyLow)
                .HasPrecision(5, 2);

            builder.Property(c => c.EstimateAccuracyHigh)
                .HasPrecision(5, 2);

            // Budget Information
            builder.Property(c => c.OriginalBudget)
                .IsRequired()
                .HasPrecision(18, 2)
                .HasDefaultValue(0);

            builder.Property(c => c.ApprovedChanges)
                .IsRequired()
                .HasPrecision(18, 2)
                .HasDefaultValue(0);

            builder.Property(c => c.CommittedCost)
                .IsRequired()
                .HasPrecision(18, 2)
                .HasDefaultValue(0);

            builder.Property(c => c.ActualCost)
                .IsRequired()
                .HasPrecision(18, 2)
                .HasDefaultValue(0);

            builder.Property(c => c.ForecastCost)
                .IsRequired()
                .HasPrecision(18, 2)
                .HasDefaultValue(0);

            builder.Property(c => c.Currency)
                .IsRequired()
                .HasMaxLength(3)
                .HasDefaultValue("USD");

            // Allocation
            builder.Property(c => c.AllocationPercentage)
                .HasPrecision(5, 2);

            builder.Property(c => c.AllocationBasis)
                .HasMaxLength(200);

            // Time-phased Budget
            builder.Property(c => c.IsTimePhased)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(c => c.TimePhasedData)
                .HasMaxLength(4000);

            // Soft Delete
            builder.Property(c => c.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(c => c.DeletedBy)
                .HasMaxLength(256);

            // Activatable
            builder.Property(c => c.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

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
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Parent)
                .WithMany(p => p.Children)
                .HasForeignKey(c => c.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.CostItems)
                .WithOne(ci => ci.CBS)
                .HasForeignKey(ci => ci.CBSId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.BudgetItems)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "CBSBudgetItems",
                    j => j.HasOne<BudgetItem>().WithMany().HasForeignKey("BudgetItemId").OnDelete(DeleteBehavior.Cascade),
                    j => j.HasOne<CBS>().WithMany().HasForeignKey("CBSId").OnDelete(DeleteBehavior.Restrict));

            builder.HasMany(c => c.WBSMappings)
                .WithMany()
                .UsingEntity<WBSCBSMapping>(
                    j => j.HasOne(m => m.WBSElement).WithMany(w => w.CBSMappings).HasForeignKey(m => m.WBSElementId).OnDelete(DeleteBehavior.Cascade),
                    j => j.HasOne(m => m.CBS).WithMany().HasForeignKey(m => m.CBSId).OnDelete(DeleteBehavior.Restrict));

            // Calculated properties (ignored)
            builder.Ignore(c => c.CurrentBudget);
            builder.Ignore(c => c.CostVariance);
            builder.Ignore(c => c.CostVariancePercentage);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_CBS_Level", "[Level] >= 0");
                t.HasCheckConstraint("CK_CBS_SequenceNumber", "[SequenceNumber] > 0");
                t.HasCheckConstraint("CK_CBS_Amounts", 
                    "[OriginalBudget] >= 0 AND [ApprovedChanges] >= 0 AND [CommittedCost] >= 0 " +
                    "AND [ActualCost] >= 0 AND [ForecastCost] >= 0");
                t.HasCheckConstraint("CK_CBS_AllocationPercentage", 
                    "[AllocationPercentage] IS NULL OR ([AllocationPercentage] >= 0 AND [AllocationPercentage] <= 100)");
                t.HasCheckConstraint("CK_CBS_EstimateAccuracy", 
                    "([EstimateAccuracyLow] IS NULL AND [EstimateAccuracyHigh] IS NULL) OR " +
                    "([EstimateAccuracyLow] <= 0 AND [EstimateAccuracyHigh] >= 0)");
            });

            // Global query filter for soft delete
            builder.HasQueryFilter(c => !c.IsDeleted);
        }
    }
}