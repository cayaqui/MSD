using Domain.Entities.Contracts.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Claim = Domain.Entities.Contracts.Core.Claim;

namespace Infrastructure.Data.Configurations.Contracts
{
    public class ClaimConfiguration : IEntityTypeConfiguration<Claim>
    {
        public void Configure(EntityTypeBuilder<Claim> builder)
        {
            // Table name and schema
            builder.ToTable("Claims", "Contracts");

            // Primary key
            builder.HasKey(c => c.Id);

            // Indexes
            builder.HasIndex(c => c.ClaimNumber).IsUnique();
            builder.HasIndex(c => c.ContractId);
            builder.HasIndex(c => c.Type);
            builder.HasIndex(c => c.Status);
            builder.HasIndex(c => c.Priority);
            builder.HasIndex(c => c.IsActive);
            builder.HasIndex(c => c.Direction);
            builder.HasIndex(c => new { c.ContractId, c.Status });
            builder.HasIndex(c => c.EventDate);
            builder.HasIndex(c => c.SubmissionDate);
            builder.HasIndex(c => c.ResponseDueDate);

            // Properties
            builder.Property(c => c.ClaimNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.Title)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(c => c.Description)
                .IsRequired()
                .HasMaxLength(4000);

            builder.Property(c => c.Type)
                .IsRequired();

            builder.Property(c => c.Status)
                .IsRequired();

            builder.Property(c => c.Priority)
                .IsRequired();

            // Claim Details
            builder.Property(c => c.ClaimBasis)
                .IsRequired()
                .HasMaxLength(4000);

            builder.Property(c => c.ContractualReference)
                .HasMaxLength(500);

            builder.Property(c => c.FactualBasis)
                .HasMaxLength(4000);

            builder.Property(c => c.LegalBasis)
                .HasMaxLength(4000);

            // Financial
            builder.Property(c => c.ClaimedAmount)
                .HasPrecision(18, 2);

            builder.Property(c => c.AssessedAmount)
                .HasPrecision(18, 2);

            builder.Property(c => c.ApprovedAmount)
                .HasPrecision(18, 2);

            builder.Property(c => c.PaidAmount)
                .HasPrecision(18, 2);

            builder.Property(c => c.Currency)
                .IsRequired()
                .HasMaxLength(3)
                .HasDefaultValue("USD");

            // Time Impact
            builder.Property(c => c.ClaimedTimeExtension)
                .HasDefaultValue(0);

            builder.Property(c => c.AssessedTimeExtension)
                .HasDefaultValue(0);

            builder.Property(c => c.ApprovedTimeExtension)
                .HasDefaultValue(0);

            // Parties
            builder.Property(c => c.Direction)
                .IsRequired();

            builder.Property(c => c.ClaimantName)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(c => c.RespondentName)
                .IsRequired()
                .HasMaxLength(256);

            // Assessment
            builder.Property(c => c.LiabilityPercentage)
                .HasPrecision(5, 2)
                .HasDefaultValue(0);

            builder.Property(c => c.AssessmentComments)
                .HasMaxLength(4000);

            // Resolution
            builder.Property(c => c.Resolution);

            builder.Property(c => c.ResolutionDetails)
                .HasMaxLength(4000);

            builder.Property(c => c.SettlementTerms)
                .HasMaxLength(4000);

            // Audit
            builder.Property(c => c.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(c => c.Notes)
                .HasMaxLength(4000);

            builder.Property(c => c.Metadata)
                .HasColumnType("nvarchar(max)");

            // Base audit properties
            builder.Property(c => c.CreatedAt)
                .IsRequired();

            builder.Property(c => c.CreatedBy)
                .HasMaxLength(256);

            builder.Property(c => c.UpdatedAt);

            builder.Property(c => c.UpdatedBy)
                .HasMaxLength(256);

            // Relationships
            builder.HasOne(c => c.Contract)
                .WithMany(ct => ct.Claims)
                .HasForeignKey(c => c.ContractId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Documents)
                .WithOne(d => d.Claim)
                .HasForeignKey(d => d.ClaimId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.RelatedClaims)
                .WithOne(r => r.Claim)
                .HasForeignKey(r => r.ClaimId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.RelatedChangeOrders)
                .WithOne(co => co.Claim)
                .HasForeignKey(co => co.ClaimId)
                .OnDelete(DeleteBehavior.Cascade);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Claims_Amounts",
                    "[ClaimedAmount] >= 0 AND [AssessedAmount] >= 0 AND [ApprovedAmount] >= 0 AND [PaidAmount] >= 0");
                t.HasCheckConstraint("CK_Claims_TimeExtensions",
                    "[ClaimedTimeExtension] >= 0 AND [AssessedTimeExtension] >= 0 AND [ApprovedTimeExtension] >= 0");
                t.HasCheckConstraint("CK_Claims_LiabilityPercentage",
                    "[LiabilityPercentage] >= 0 AND [LiabilityPercentage] <= 100");
                t.HasCheckConstraint("CK_Claims_Dates",
                    "[NotificationDate] >= [EventDate] AND [SubmissionDate] >= [NotificationDate]");
            });

            // Ignore calculated properties
            builder.Ignore(c => c.AttachmentCount);
            builder.Ignore(c => c.IsOverdue);
        }
    }
}