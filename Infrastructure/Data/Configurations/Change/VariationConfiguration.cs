using Domain.Entities.Change.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Change
{
    public class VariationConfiguration : IEntityTypeConfiguration<Variation>
    {
        public void Configure(EntityTypeBuilder<Variation> builder)
        {
            // Table name and schema
            builder.ToTable("Variations", "Change");

            // Primary key
            builder.HasKey(v => v.Id);

            // Indexes
            builder.HasIndex(v => v.Code).IsUnique();
            builder.HasIndex(v => v.ProjectId);
            builder.HasIndex(v => v.ContractorId);
            builder.HasIndex(v => v.ChangeOrderId);
            builder.HasIndex(v => v.TrendId);
            builder.HasIndex(v => v.Status);
            builder.HasIndex(v => v.Type);
            builder.HasIndex(v => v.Category);
            builder.HasIndex(v => v.IssuedDate);
            builder.HasIndex(v => v.IsDeleted);
            builder.HasIndex(v => new { v.ProjectId, v.Status });

            // Basic Information
            builder.Property(v => v.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(v => v.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(v => v.Description)
                .IsRequired()
                .HasMaxLength(4000);

            // Classification
            builder.Property(v => v.Type)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(v => v.Status)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(v => v.Category)
                .IsRequired()
                .HasMaxLength(50);

            // Contract Information
            builder.Property(v => v.ContractReference)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(v => v.ClientReferenceNumber)
                .HasMaxLength(100);

            builder.Property(v => v.ContractorReferenceNumber)
                .HasMaxLength(100);

            builder.Property(v => v.IsContractual)
                .IsRequired()
                .HasDefaultValue(true);

            // Dates
            builder.Property(v => v.IssuedDate)
                .IsRequired();

            // Financial Impact
            builder.Property(v => v.OriginalValue)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(v => v.NegotiatedValue)
                .HasPrecision(18, 2);

            builder.Property(v => v.ApprovedValue)
                .IsRequired()
                .HasPrecision(18, 2)
                .HasDefaultValue(0);

            builder.Property(v => v.Currency)
                .IsRequired()
                .HasMaxLength(3)
                .HasDefaultValue("USD");

            // Cost Breakdown
            builder.Property(v => v.LaborCost)
                .HasPrecision(18, 2);

            builder.Property(v => v.MaterialCost)
                .HasPrecision(18, 2);

            builder.Property(v => v.EquipmentCost)
                .HasPrecision(18, 2);

            builder.Property(v => v.SubcontractorCost)
                .HasPrecision(18, 2);

            builder.Property(v => v.IndirectCost)
                .HasPrecision(18, 2);

            builder.Property(v => v.OverheadPercentage)
                .HasPrecision(5, 2);

            builder.Property(v => v.ProfitPercentage)
                .HasPrecision(5, 2);

            // Schedule Impact
            builder.Property(v => v.TimeExtensionDays);

            builder.Property(v => v.RevisedCompletionDate);

            builder.Property(v => v.IsCriticalPathImpacted)
                .IsRequired()
                .HasDefaultValue(false);

            // Quantity Changes
            builder.Property(v => v.QuantityChanges)
                .HasMaxLength(4000)
                .HasComment("JSON structure");

            builder.Property(v => v.RateAdjustments)
                .HasMaxLength(4000)
                .HasComment("JSON structure");

            // Approval Workflow
            builder.Property(v => v.ApprovalComments)
                .HasMaxLength(2000);

            builder.Property(v => v.ApprovalLevel)
                .IsRequired()
                .HasDefaultValue(1);

            // Client Approval
            builder.Property(v => v.RequiresClientApproval)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(v => v.ClientApproved);

            builder.Property(v => v.ClientApprovalDate);

            builder.Property(v => v.ClientApprovalReference)
                .HasMaxLength(100);

            builder.Property(v => v.ClientComments)
                .HasMaxLength(2000);

            // Documentation
            builder.Property(v => v.Justification)
                .HasMaxLength(4000);

            builder.Property(v => v.ScopeOfWork)
                .HasMaxLength(4000);

            builder.Property(v => v.ImpactAnalysis)
                .HasMaxLength(4000);

            builder.Property(v => v.ContractualBasis)
                .HasMaxLength(2000);

            builder.Property(v => v.DrawingReferences)
                .HasMaxLength(4000);

            builder.Property(v => v.SpecificationReferences)
                .HasMaxLength(4000);

            // Payment Terms
            builder.Property(v => v.PaymentMethod)
                .HasMaxLength(50);

            builder.Property(v => v.PaymentTerms)
                .HasMaxLength(2000);

            builder.Property(v => v.AdvancePaymentPercentage)
                .HasPrecision(5, 2);

            builder.Property(v => v.RetentionPercentage)
                .HasPrecision(5, 2);

            // Status Tracking
            builder.Property(v => v.IsDisputed)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(v => v.DisputeReason)
                .HasMaxLength(2000);

            builder.Property(v => v.DisputeDate);

            builder.Property(v => v.IsResolved)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(v => v.ResolutionDate);

            // Implementation
            builder.Property(v => v.IsImplemented)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(v => v.ImplementedValue)
                .HasPrecision(18, 2);

            builder.Property(v => v.CertifiedValue)
                .HasPrecision(18, 2);

            builder.Property(v => v.PaidValue)
                .HasPrecision(18, 2);

            // Calculated properties - ignore
            builder.Ignore(v => v.PendingValue);
            builder.Ignore(v => v.OutstandingValue);
            builder.Ignore(v => v.IsFullyPaid);

            // Soft Delete
            builder.Property(v => v.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(v => v.DeletedAt);

            builder.Property(v => v.DeletedBy)
                .HasMaxLength(256);

            // Audit properties
            builder.Property(v => v.CreatedAt)
                .IsRequired();

            builder.Property(v => v.CreatedBy)
                .HasMaxLength(256);

            builder.Property(v => v.UpdatedAt);

            builder.Property(v => v.UpdatedBy)
                .HasMaxLength(256);

            // Relationships
            builder.HasOne(v => v.Project)
                .WithMany()
                .HasForeignKey(v => v.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(v => v.Contractor)
                .WithMany()
                .HasForeignKey(v => v.ContractorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(v => v.ChangeOrder)
                .WithMany()
                .HasForeignKey(v => v.ChangeOrderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(v => v.Trend)
                .WithMany()
                .HasForeignKey(v => v.TrendId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(v => v.RequestedByUser)
                .WithMany()
                .HasForeignKey(v => v.RequestedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(v => v.ReviewedByUser)
                .WithMany()
                .HasForeignKey(v => v.ReviewedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(v => v.ApprovedByUser)
                .WithMany()
                .HasForeignKey(v => v.ApprovedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Soft delete filter
            builder.HasQueryFilter(v => !v.IsDeleted);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Variations_OriginalValue",
                    "[OriginalValue] >= 0");
                t.HasCheckConstraint("CK_Variations_ApprovedValue",
                    "[ApprovedValue] >= 0");
                t.HasCheckConstraint("CK_Variations_CostComponents",
                    "([LaborCost] IS NULL OR [LaborCost] >= 0) AND " +
                    "([MaterialCost] IS NULL OR [MaterialCost] >= 0) AND " +
                    "([EquipmentCost] IS NULL OR [EquipmentCost] >= 0) AND " +
                    "([SubcontractorCost] IS NULL OR [SubcontractorCost] >= 0) AND " +
                    "([IndirectCost] IS NULL OR [IndirectCost] >= 0)");
                t.HasCheckConstraint("CK_Variations_Percentages",
                    "([OverheadPercentage] IS NULL OR ([OverheadPercentage] >= 0 AND [OverheadPercentage] <= 100)) AND " +
                    "([ProfitPercentage] IS NULL OR ([ProfitPercentage] >= 0 AND [ProfitPercentage] <= 100)) AND " +
                    "([AdvancePaymentPercentage] IS NULL OR ([AdvancePaymentPercentage] >= 0 AND [AdvancePaymentPercentage] <= 100)) AND " +
                    "([RetentionPercentage] IS NULL OR ([RetentionPercentage] >= 0 AND [RetentionPercentage] <= 100))");
                t.HasCheckConstraint("CK_Variations_Implementation",
                    "([ImplementedValue] IS NULL OR [ImplementedValue] >= 0) AND " +
                    "([CertifiedValue] IS NULL OR [CertifiedValue] >= 0) AND " +
                    "([PaidValue] IS NULL OR [PaidValue] >= 0)");
                t.HasCheckConstraint("CK_Variations_ApprovalLevel",
                    "[ApprovalLevel] >= 1 AND [ApprovalLevel] <= 5");
            });
        }
    }
}