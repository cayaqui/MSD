using Domain.Entities.Contracts.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Contracts
{
    public class ContractConfiguration : IEntityTypeConfiguration<Contract>
    {
        public void Configure(EntityTypeBuilder<Contract> builder)
        {
            // Table name and schema
            builder.ToTable("Contracts", "Contracts");

            // Primary key
            builder.HasKey(c => c.Id);

            // Indexes
            builder.HasIndex(c => c.ContractNumber).IsUnique();
            builder.HasIndex(c => c.ProjectId);
            builder.HasIndex(c => c.ContractorId);
            builder.HasIndex(c => c.Status);
            builder.HasIndex(c => c.Type);
            builder.HasIndex(c => c.Category);
            builder.HasIndex(c => c.IsActive);
            builder.HasIndex(c => new { c.ProjectId, c.Status });
            builder.HasIndex(c => new { c.ContractorId, c.Status });
            builder.HasIndex(c => new { c.Status, c.CurrentEndDate });

            // Properties
            builder.Property(c => c.ContractNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.Title)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(c => c.Description)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(c => c.Type)
                .IsRequired();

            builder.Property(c => c.Status)
                .IsRequired();

            builder.Property(c => c.Category)
                .IsRequired();

            builder.Property(c => c.SubCategory)
                .HasMaxLength(100);

            builder.Property(c => c.ContractorReference)
                .HasMaxLength(100);

            // Value properties
            builder.Property(c => c.OriginalValue)
                .HasPrecision(18, 2);

            builder.Property(c => c.CurrentValue)
                .HasPrecision(18, 2);

            builder.Property(c => c.Currency)
                .IsRequired()
                .HasMaxLength(3)
                .HasDefaultValue("USD");

            builder.Property(c => c.ExchangeRate)
                .HasPrecision(10, 6)
                .HasDefaultValue(1.0m);

            // Performance properties
            builder.Property(c => c.PercentageComplete)
                .HasPrecision(5, 2);

            builder.Property(c => c.AmountInvoiced)
                .HasPrecision(18, 2);

            builder.Property(c => c.AmountPaid)
                .HasPrecision(18, 2);

            builder.Property(c => c.RetentionAmount)
                .HasPrecision(18, 2);

            builder.Property(c => c.RetentionPercentage)
                .HasPrecision(5, 2);

            // Payment terms
            builder.Property(c => c.PaymentTerms)
                .IsRequired();

            builder.Property(c => c.PaymentDays)
                .IsRequired();

            builder.Property(c => c.PaymentSchedule)
                .HasMaxLength(500);

            // Bonds
            builder.Property(c => c.PerformanceBondAmount)
                .HasPrecision(18, 2);

            builder.Property(c => c.PaymentBondAmount)
                .HasPrecision(18, 2);

            builder.Property(c => c.InsuranceRequirements)
                .HasMaxLength(1000);

            // Risk
            builder.Property(c => c.RiskLevel)
                .IsRequired();

            // Documents
            builder.Property(c => c.ContractDocumentUrl)
                .HasMaxLength(500);

            // Approval
            builder.Property(c => c.ApprovedBy)
                .HasMaxLength(256);

            builder.Property(c => c.ApprovalComments)
                .HasMaxLength(1000);

            // Additional Information
            builder.Property(c => c.Scope)
                .HasMaxLength(4000);

            builder.Property(c => c.Exclusions)
                .HasMaxLength(2000);

            builder.Property(c => c.SpecialConditions)
                .HasMaxLength(2000);

            builder.Property(c => c.PenaltyClauses)
                .HasMaxLength(2000);

            builder.Property(c => c.TerminationClauses)
                .HasMaxLength(2000);

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
            builder.HasOne(c => c.Project)
                .WithMany()
                .HasForeignKey(c => c.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Contractor)
                .WithMany()
                .HasForeignKey(c => c.ContractorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.ChangeOrders)
                .WithOne(co => co.Contract)
                .HasForeignKey(co => co.ContractId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Milestones)
                .WithOne(m => m.Contract)
                .HasForeignKey(m => m.ContractId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Valuations)
                .WithOne(v => v.Contract)
                .HasForeignKey(v => v.ContractId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Claims)
                .WithOne(cl => cl.Contract)
                .HasForeignKey(cl => cl.ContractId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Documents)
                .WithOne(d => d.Contract)
                .HasForeignKey(d => d.ContractId)
                .OnDelete(DeleteBehavior.Restrict);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Contracts_CurrentValue", "[CurrentValue] >= 0");
                t.HasCheckConstraint("CK_Contracts_PercentageComplete", "[PercentageComplete] >= 0 AND [PercentageComplete] <= 100");
                t.HasCheckConstraint("CK_Contracts_RetentionPercentage", "[RetentionPercentage] >= 0 AND [RetentionPercentage] <= 100");
                t.HasCheckConstraint("CK_Contracts_ExchangeRate", "[ExchangeRate] > 0");
                t.HasCheckConstraint("CK_Contracts_PaymentDays", "[PaymentDays] >= 0");
                t.HasCheckConstraint("CK_Contracts_Dates", "[CurrentEndDate] >= [StartDate]");
            });

            // Ignore calculated properties
            builder.Ignore(c => c.ChangeOrderValue);
            builder.Ignore(c => c.ChangeOrderCount);
            builder.Ignore(c => c.PendingChangeOrderValue);
            builder.Ignore(c => c.TotalMilestones);
            builder.Ignore(c => c.CompletedMilestones);
            builder.Ignore(c => c.NextMilestoneDate);
            builder.Ignore(c => c.NextMilestoneName);
            builder.Ignore(c => c.OpenIssuesCount);
            builder.Ignore(c => c.OpenClaimsCount);
            builder.Ignore(c => c.TotalClaimsValue);
            builder.Ignore(c => c.AttachmentCount);
        }
    }
}