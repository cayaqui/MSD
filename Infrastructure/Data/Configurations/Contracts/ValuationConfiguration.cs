using Domain.Entities.Contracts.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Contracts
{
    public class ValuationConfiguration : IEntityTypeConfiguration<Valuation>
    {
        public void Configure(EntityTypeBuilder<Valuation> builder)
        {
            // Table name and schema
            builder.ToTable("Valuations", "Contracts");

            // Primary key
            builder.HasKey(v => v.Id);

            // Indexes
            builder.HasIndex(v => v.ValuationNumber).IsUnique();
            builder.HasIndex(v => new { v.ContractId, v.ValuationPeriod }).IsUnique();
            builder.HasIndex(v => v.ContractId);
            builder.HasIndex(v => v.Status);
            builder.HasIndex(v => v.IsActive);
            builder.HasIndex(v => v.ValuationDate);
            builder.HasIndex(v => v.SubmissionDate);
            builder.HasIndex(v => new { v.ContractId, v.Status });

            // Properties
            builder.Property(v => v.ValuationNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(v => v.ValuationPeriod)
                .IsRequired();

            builder.Property(v => v.Title)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(v => v.Description)
                .HasMaxLength(4000);

            builder.Property(v => v.Status)
                .IsRequired()
                .HasMaxLength(50);

            // Work Done
            builder.Property(v => v.PreviouslyCompletedWork)
                .HasPrecision(18, 2);

            builder.Property(v => v.CurrentPeriodWork)
                .HasPrecision(18, 2);

            builder.Property(v => v.TotalCompletedWork)
                .HasPrecision(18, 2);

            builder.Property(v => v.PercentageComplete)
                .HasPrecision(5, 2);

            // Materials
            builder.Property(v => v.MaterialsOnSite)
                .HasPrecision(18, 2);

            builder.Property(v => v.MaterialsOffSite)
                .HasPrecision(18, 2);

            builder.Property(v => v.TotalMaterials)
                .HasPrecision(18, 2);

            // Variations
            builder.Property(v => v.ApprovedVariations)
                .HasPrecision(18, 2);

            builder.Property(v => v.PendingVariations)
                .HasPrecision(18, 2);

            // Amounts
            builder.Property(v => v.GrossValuation)
                .HasPrecision(18, 2);

            builder.Property(v => v.LessRetention)
                .HasPrecision(18, 2);

            builder.Property(v => v.RetentionAmount)
                .HasPrecision(18, 2);

            builder.Property(v => v.LessPreviousCertificates)
                .HasPrecision(18, 2);

            builder.Property(v => v.NetValuation)
                .HasPrecision(18, 2);

            builder.Property(v => v.Currency)
                .IsRequired()
                .HasMaxLength(3)
                .HasDefaultValue("USD");

            // Deductions
            builder.Property(v => v.AdvancePaymentRecovery)
                .HasPrecision(18, 2);

            builder.Property(v => v.Penalties)
                .HasPrecision(18, 2);

            builder.Property(v => v.OtherDeductions)
                .HasPrecision(18, 2);

            builder.Property(v => v.TotalDeductions)
                .HasPrecision(18, 2);

            builder.Property(v => v.AmountDue)
                .HasPrecision(18, 2);

            // Payment
            builder.Property(v => v.InvoiceNumber)
                .HasMaxLength(50);

            builder.Property(v => v.PaymentAmount)
                .HasPrecision(18, 2);

            // Approval
            builder.Property(v => v.ApprovedBy)
                .HasMaxLength(256);

            builder.Property(v => v.ApprovalComments)
                .HasMaxLength(2000);

            builder.Property(v => v.RejectedBy)
                .HasMaxLength(256);

            builder.Property(v => v.RejectionReason)
                .HasMaxLength(2000);

            // Audit
            builder.Property(v => v.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(v => v.Notes)
                .HasMaxLength(4000);

            builder.Property(v => v.Metadata)
                .HasColumnType("nvarchar(max)");

            // Base audit properties
            builder.Property(v => v.CreatedAt)
                .IsRequired();

            builder.Property(v => v.CreatedBy)
                .HasMaxLength(256);

            builder.Property(v => v.UpdatedAt);

            builder.Property(v => v.UpdatedBy)
                .HasMaxLength(256);

            // Relationships
            builder.HasOne(v => v.Contract)
                .WithMany(c => c.Valuations)
                .HasForeignKey(v => v.ContractId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(v => v.Items)
                .WithOne(i => i.Valuation)
                .HasForeignKey(i => i.ValuationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(v => v.Documents)
                .WithOne(d => d.Valuation)
                .HasForeignKey(d => d.ValuationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Valuations_Amounts",
                    "[GrossValuation] >= 0 AND [RetentionAmount] >= 0 AND [NetValuation] >= 0 AND [AmountDue] >= 0");
                t.HasCheckConstraint("CK_Valuations_PercentageComplete",
                    "[PercentageComplete] >= 0 AND [PercentageComplete] <= 100");
                t.HasCheckConstraint("CK_Valuations_Dates",
                    "[PeriodEndDate] >= [PeriodStartDate]");
                t.HasCheckConstraint("CK_Valuations_ValuationPeriod",
                    "[ValuationPeriod] > 0");
            });

            // Ignore calculated properties
            builder.Ignore(v => v.AttachmentCount);
        }
    }
}