using Domain.Entities.Cost.Budget;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Cost
{
    public class BudgetConfiguration : IEntityTypeConfiguration<Budget>
    {
        public void Configure(EntityTypeBuilder<Budget> builder)
        {
            // Table name and schema
            builder.ToTable("Budgets", "Cost");

            // Primary key
            builder.HasKey(b => b.Id);

            // Indexes
            builder.HasIndex(b => b.ProjectId);
            builder.HasIndex(b => b.Version);
            builder.HasIndex(b => b.Status);
            builder.HasIndex(b => b.Type);
            builder.HasIndex(b => b.IsBaseline);
            builder.HasIndex(b => b.IsDeleted);
            builder.HasIndex(b => b.ParentBudgetId);
            builder.HasIndex(b => new { b.ProjectId, b.Version }).IsUnique();
            builder.HasIndex(b => new { b.ProjectId, b.Status });
            builder.HasIndex(b => new { b.ProjectId, b.IsBaseline });

            // Basic Information
            builder.Property(b => b.Version)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(b => b.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(b => b.Description)
                .HasMaxLength(1000);

            // Status and Type
            builder.Property(b => b.Status)
                .IsRequired();

            builder.Property(b => b.Type)
                .IsRequired();

            builder.Property(b => b.IsBaseline)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(b => b.IsLocked)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(b => b.LockedBy)
                .HasMaxLength(256);

            // Financial Information
            builder.Property(b => b.Currency)
                .IsRequired()
                .HasMaxLength(3)
                .HasDefaultValue("USD");

            builder.Property(b => b.ExchangeRate)
                .IsRequired()
                .HasPrecision(18, 6)
                .HasDefaultValue(1.0m);

            builder.Property(b => b.TotalAmount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(b => b.ContingencyAmount)
                .IsRequired()
                .HasPrecision(18, 2)
                .HasDefaultValue(0);

            builder.Property(b => b.ContingencyPercentage)
                .IsRequired()
                .HasPrecision(5, 2)
                .HasDefaultValue(0);

            builder.Property(b => b.ManagementReserve)
                .IsRequired()
                .HasPrecision(18, 2)
                .HasDefaultValue(0);

            builder.Property(b => b.ManagementReservePercentage)
                .IsRequired()
                .HasPrecision(5, 2)
                .HasDefaultValue(0);

            // Approval Information
            builder.Property(b => b.SubmittedBy)
                .HasMaxLength(256);

            builder.Property(b => b.ApprovedBy)
                .HasMaxLength(256);

            builder.Property(b => b.ApprovalComments)
                .HasMaxLength(2000);

            // Revision Information
            builder.Property(b => b.RevisionCount)
                .IsRequired()
                .HasDefaultValue(0);

            // Soft Delete
            builder.Property(b => b.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(b => b.DeletedBy)
                .HasMaxLength(256);

            // Audit properties
            builder.Property(b => b.CreatedAt)
                .IsRequired();

            builder.Property(b => b.CreatedBy)
                .HasMaxLength(256);

            builder.Property(b => b.UpdatedAt);

            builder.Property(b => b.UpdatedBy)
                .HasMaxLength(256);

            // Relationships
            builder.HasOne(b => b.Project)
                .WithMany(p => p.Budgets)
                .HasForeignKey(b => b.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.ParentBudget)
                .WithMany(pb => pb.ChildBudgets)
                .HasForeignKey(b => b.ParentBudgetId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(b => b.BudgetItems)
                .WithOne(bi => bi.Budget)
                .HasForeignKey(bi => bi.BudgetId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(b => b.Revisions)
                .WithOne(r => r.Budget)
                .HasForeignKey(r => r.BudgetId)
                .OnDelete(DeleteBehavior.Cascade);

            // Calculated properties (ignored)
            builder.Ignore(b => b.TotalBudget);
            builder.Ignore(b => b.AllocatedAmount);
            builder.Ignore(b => b.UnallocatedAmount);
            builder.Ignore(b => b.AllocationPercentage);
            builder.Ignore(b => b.IsOverAllocated);
            builder.Ignore(b => b.CanBeModified);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Budgets_TotalAmount", "[TotalAmount] >= 0");
                t.HasCheckConstraint("CK_Budgets_ContingencyPercentage", 
                    "[ContingencyPercentage] >= 0 AND [ContingencyPercentage] <= 100");
                t.HasCheckConstraint("CK_Budgets_ManagementReservePercentage", 
                    "[ManagementReservePercentage] >= 0 AND [ManagementReservePercentage] <= 100");
                t.HasCheckConstraint("CK_Budgets_ExchangeRate", "[ExchangeRate] > 0");
            });

            // Global query filter for soft delete
            builder.HasQueryFilter(b => !b.IsDeleted);
        }
    }
}