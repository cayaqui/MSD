using Domain.Entities.Cost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Cost;

/// <summary>
/// Entity configuration for Budget
/// </summary>
public class BudgetConfiguration : IEntityTypeConfiguration<Budget>
{
    public void Configure(EntityTypeBuilder<Budget> builder)
    {
        // Table name and schema with check constraints
        builder.ToTable("Budgets", "Cost", t =>
        {
            t.HasCheckConstraint("CK_Budgets_TotalAmount",
                "[TotalAmount] >= 0");
            t.HasCheckConstraint("CK_Budgets_ContingencyAmount",
                "[ContingencyAmount] >= 0");
            t.HasCheckConstraint("CK_Budgets_ManagementReserve",
                "[ManagementReserve] >= 0");
            t.HasCheckConstraint("CK_Budgets_ExchangeRate",
                "[ExchangeRate] > 0");
            t.HasCheckConstraint("CK_Budgets_BaselineDate",
                "([IsBaseline] = 0 AND [BaselineDate] IS NULL) OR ([IsBaseline] = 1 AND [BaselineDate] IS NOT NULL)");
            t.HasCheckConstraint("CK_Budgets_Approval",
                "([Status] != 'Approved' AND [ApprovalDate] IS NULL AND [ApprovedBy] IS NULL) OR " +
                "([Status] = 'Approved' AND [ApprovalDate] IS NOT NULL AND [ApprovedBy] IS NOT NULL)");
        });

        // Primary key
        builder.HasKey(b => b.Id);

        // Indexes
        builder.HasIndex(b => new { b.ProjectId, b.Version }).IsUnique();
        builder.HasIndex(b => b.ProjectId);
        builder.HasIndex(b => b.Status);
        builder.HasIndex(b => b.Type);
        builder.HasIndex(b => b.IsDeleted);
        builder.HasIndex(b => new { b.IsBaseline, b.BaselineDate });
        builder.HasIndex(b => b.SubmittedDate);
        builder.HasIndex(b => b.ApprovalDate);

        // Properties
        builder.Property(b => b.Version)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(b => b.Description)
            .HasMaxLength(1000);

        builder.Property(b => b.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(b => b.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(b => b.IsBaseline)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(b => b.BaselineDate);

        builder.Property(b => b.Currency)
            .IsRequired()
            .HasMaxLength(3)
            .HasDefaultValue("USD");

        builder.Property(b => b.ExchangeRate)
            .HasPrecision(10, 6)
            .IsRequired()
            .HasDefaultValue(1.0m);

        builder.Property(b => b.TotalAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(b => b.ContingencyAmount)
            .HasPrecision(18, 2)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(b => b.ManagementReserve)
            .HasPrecision(18, 2)
            .IsRequired()
            .HasDefaultValue(0);

        // Computed columns for calculated fields
        builder.Property(b => b.TotalBudget)
            .HasComputedColumnSql("[TotalAmount] + [ContingencyAmount] + [ManagementReserve]", stored: false);

        // Note: AllocatedAmount and UnallocatedAmount are calculated in code from BudgetItems
        // These cannot be computed columns as they depend on related entities

        // Submission Information
        builder.Property(b => b.SubmittedDate);

        builder.Property(b => b.SubmittedBy)
            .HasMaxLength(256);

        // Approval Information
        builder.Property(b => b.ApprovalDate);

        builder.Property(b => b.ApprovedBy)
            .HasMaxLength(256);

        builder.Property(b => b.ApprovalComments)
            .HasMaxLength(2000);

        // Audit properties
        builder.Property(b => b.CreatedAt)
            .IsRequired();

        builder.Property(b => b.CreatedBy)
            .HasMaxLength(256);

        builder.Property(b => b.UpdatedAt);

        builder.Property(b => b.UpdatedBy)
            .HasMaxLength(256);

        // Soft delete properties
        builder.Property(b => b.DeletedAt);

        builder.Property(b => b.DeletedBy)
            .HasMaxLength(256);

        // Foreign key relationships
        builder.HasOne(b => b.Project)
            .WithMany(p => p.Budgets)
            .HasForeignKey(b => b.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // Navigation properties
        builder.HasMany(b => b.BudgetItems)
            .WithOne(bi => bi.Budget)
            .HasForeignKey(bi => bi.BudgetId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(b => b.Revisions)
            .WithOne(br => br.Budget)
            .HasForeignKey(br => br.BudgetId)
            .OnDelete(DeleteBehavior.Cascade);

        // Global query filter for soft delete
        builder.HasQueryFilter(b => !b.IsDeleted);
    }
}