using Domain.Entities.Cost.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Cost
{
    public class AccountCodeConfiguration : IEntityTypeConfiguration<AccountCode>
    {
        public void Configure(EntityTypeBuilder<AccountCode> builder)
        {
            // Table name and schema
            builder.ToTable("AccountCodes", "Cost");

            // Primary key
            builder.HasKey(a => a.Id);

            // Indexes
            builder.HasIndex(a => a.ProjectId);
            builder.HasIndex(a => a.PhaseId);
            builder.HasIndex(a => a.AccountType);
            builder.HasIndex(a => a.Category);
            builder.HasIndex(a => a.CostType);
            builder.HasIndex(a => a.IsActive);
            builder.HasIndex(a => new { a.ProjectId, a.PhaseId });

            // Code components
            builder.Property(a => a.AccountType)
                .IsRequired()
                .HasMaxLength(1);

            builder.Property(a => a.ProjectCode)
                .IsRequired()
                .HasMaxLength(3);

            builder.Property(a => a.PhaseCode)
                .IsRequired()
                .HasMaxLength(2);

            builder.Property(a => a.PackageCode)
                .IsRequired()
                .HasMaxLength(3);

            builder.Property(a => a.SpecificCode)
                .IsRequired()
                .HasMaxLength(2);

            // Descriptive information
            builder.Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(a => a.Description)
                .HasMaxLength(1000);

            builder.Property(a => a.Category)
                .IsRequired()
                .HasMaxLength(100);

            // Chilean accounting mapping
            builder.Property(a => a.SIICode)
                .HasMaxLength(50);

            builder.Property(a => a.SIIDescription)
                .HasMaxLength(500);

            builder.Property(a => a.RequiresIVA)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(a => a.EligibleForAcceleratedDepreciation)
                .IsRequired()
                .HasDefaultValue(false);

            // Cost type classification
            builder.Property(a => a.CostType)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("Direct");

            builder.Property(a => a.CostSubType)
                .HasMaxLength(50);

            // Status
            builder.Property(a => a.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Audit properties
            builder.Property(a => a.CreatedAt)
                .IsRequired();

            builder.Property(a => a.CreatedBy)
                .HasMaxLength(256);

            builder.Property(a => a.UpdatedAt);

            builder.Property(a => a.UpdatedBy)
                .HasMaxLength(256);

            // Relationships
            builder.HasOne(a => a.Project)
                .WithMany()
                .HasForeignKey(a => a.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Phase)
                .WithMany()
                .HasForeignKey(a => a.PhaseId)
                .OnDelete(DeleteBehavior.Restrict);

            // Calculated property (ignored)
            builder.Ignore(a => a.FullCode);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_AccountCodes_AccountType", 
                    "[AccountType] IN ('P', 'C', 'G', 'E')");
                t.HasCheckConstraint("CK_AccountCodes_CostType", 
                    "[CostType] IN ('Direct', 'Indirect', 'Contingency')");
            });
        }
    }
}