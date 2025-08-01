using Domain.Entities.Cost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Cost;

/// <summary>
/// Entity configuration for CommitmentWorkPackage
/// </summary>
public class CommitmentWorkPackageConfiguration : IEntityTypeConfiguration<CommitmentWorkPackage>
{
    public void Configure(EntityTypeBuilder<CommitmentWorkPackage> builder)
    {
        // Table name and schema with check constraints
        builder.ToTable("CommitmentWorkPackages", "Cost", t =>
        {
            t.HasCheckConstraint("CK_CommitmentWorkPackages_AllocatedAmount",
                "[AllocatedAmount] > 0");
            t.HasCheckConstraint("CK_CommitmentWorkPackages_InvoicedAmount",
                "[InvoicedAmount] >= 0 AND [InvoicedAmount] <= [AllocatedAmount]");
            t.HasCheckConstraint("CK_CommitmentWorkPackages_RetainedAmount",
                "[RetainedAmount] >= 0");
            t.HasCheckConstraint("CK_CommitmentWorkPackages_PaidAmount",
                "[PaidAmount] >= 0 AND [PaidAmount] <= [InvoicedAmount]");
            t.HasCheckConstraint("CK_CommitmentWorkPackages_ProgressPercentage",
                "[ProgressPercentage] >= 0 AND [ProgressPercentage] <= 100");
            t.HasCheckConstraint("CK_CommitmentWorkPackages_LastProgressUpdate",
                "[LastProgressUpdate] IS NULL OR [LastProgressUpdate] <= GETUTCDATE()");
        });

        // Primary key
        builder.HasKey(cwp => cwp.Id);

        // Indexes
        builder.HasIndex(cwp => new { cwp.CommitmentId, cwp.WBSElementId }).IsUnique();
        builder.HasIndex(cwp => cwp.CommitmentId);
        builder.HasIndex(cwp => cwp.WBSElementId);
        builder.HasIndex(cwp => cwp.ProgressPercentage);
        builder.HasIndex(cwp => cwp.LastProgressUpdate);
        builder.HasIndex(cwp => new { cwp.InvoicedAmount, cwp.PaidAmount }); // For payment tracking

        // Properties
        builder.Property(cwp => cwp.AllocatedAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(cwp => cwp.InvoicedAmount)
            .HasPrecision(18, 2)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(cwp => cwp.RetainedAmount)
            .HasPrecision(18, 2)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(cwp => cwp.ProgressPercentage)
            .HasPrecision(5, 2)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(cwp => cwp.LastProgressUpdate);

        builder.Property(cwp => cwp.PaidAmount)
            .HasPrecision(18, 2)
            .IsRequired()
            .HasDefaultValue(0);

        // Computed column for PendingAmount
        builder.Property(cwp => cwp.PendingAmount)
            .HasComputedColumnSql("[AllocatedAmount] - [InvoicedAmount]", stored: false);

        // Audit properties (inherited from BaseEntity)
        builder.Property(cwp => cwp.CreatedAt)
            .IsRequired();

        // Foreign key relationships
        builder.HasOne(cwp => cwp.Commitment)
            .WithMany(c => c.WorkPackages)
            .HasForeignKey(cwp => cwp.CommitmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cwp => cwp.WBSElement)
            .WithMany()
            .HasForeignKey(cwp => cwp.WBSElementId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}