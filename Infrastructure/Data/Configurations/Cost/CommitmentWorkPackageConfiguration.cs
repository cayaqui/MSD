using Domain.Entities.Cost.Commitments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Cost
{
    public class CommitmentWorkPackageConfiguration : IEntityTypeConfiguration<CommitmentWorkPackage>
    {
        public void Configure(EntityTypeBuilder<CommitmentWorkPackage> builder)
        {
            // Table name and schema
            builder.ToTable("CommitmentWorkPackages", "Cost");

            // Primary key
            builder.HasKey(c => c.Id);

            // Indexes
            builder.HasIndex(c => c.CommitmentId);
            builder.HasIndex(c => c.WBSElementId);
            builder.HasIndex(c => new { c.CommitmentId, c.WBSElementId }).IsUnique();
            builder.HasIndex(c => c.ProgressPercentage);

            // Foreign Keys
            builder.Property(c => c.CommitmentId)
                .IsRequired();

            builder.Property(c => c.WBSElementId)
                .IsRequired();

            // Financial amounts
            builder.Property(c => c.AllocatedAmount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(c => c.InvoicedAmount)
                .IsRequired()
                .HasPrecision(18, 2)
                .HasDefaultValue(0);

            builder.Property(c => c.RetainedAmount)
                .IsRequired()
                .HasPrecision(18, 2)
                .HasDefaultValue(0);

            builder.Property(c => c.PaidAmount)
                .IsRequired()
                .HasPrecision(18, 2)
                .HasDefaultValue(0);

            // Progress tracking
            builder.Property(c => c.ProgressPercentage)
                .IsRequired()
                .HasPrecision(5, 2)
                .HasDefaultValue(0);

            builder.Property(c => c.LastProgressUpdate);

            // Audit properties
            builder.Property(c => c.CreatedAt)
                .IsRequired();

            builder.Property(c => c.CreatedBy)
                .HasMaxLength(256);

            builder.Property(c => c.UpdatedAt);

            builder.Property(c => c.UpdatedBy)
                .HasMaxLength(256);

            // Relationships
            builder.HasOne(c => c.Commitment)
                .WithMany(com => com.WorkPackageAllocations)
                .HasForeignKey(c => c.CommitmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.WBSElement)
                .WithMany()
                .HasForeignKey(c => c.WBSElementId)
                .OnDelete(DeleteBehavior.Restrict);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_CommitmentWorkPackages_Amounts", 
                    "[AllocatedAmount] >= 0 AND [InvoicedAmount] >= 0 AND [RetainedAmount] >= 0 AND [PaidAmount] >= 0");
                t.HasCheckConstraint("CK_CommitmentWorkPackages_Progress", 
                    "[ProgressPercentage] >= 0 AND [ProgressPercentage] <= 100");
                t.HasCheckConstraint("CK_CommitmentWorkPackages_InvoicedVsAllocated", 
                    "[InvoicedAmount] <= [AllocatedAmount]");
                t.HasCheckConstraint("CK_CommitmentWorkPackages_PaidVsInvoiced", 
                    "[PaidAmount] <= [InvoicedAmount]");
            });
        }
    }
}