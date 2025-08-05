using Domain.Entities.Cost.EVM;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Cost
{
    public class EVMRecordConfiguration : IEntityTypeConfiguration<EVMRecord>
    {
        public void Configure(EntityTypeBuilder<EVMRecord> builder)
        {
            // Table name and schema
            builder.ToTable("EVMRecords", "Cost");

            // Primary key
            builder.HasKey(e => e.Id);

            // Indexes
            builder.HasIndex(e => e.ControlAccountId);
            builder.HasIndex(e => e.DataDate);
            builder.HasIndex(e => e.PeriodType);
            builder.HasIndex(e => e.Year);
            builder.HasIndex(e => e.Status);
            builder.HasIndex(e => e.IsBaseline);
            builder.HasIndex(e => e.IsApproved);
            builder.HasIndex(e => new { e.ControlAccountId, e.DataDate }).IsUnique();
            builder.HasIndex(e => new { e.ControlAccountId, e.Year, e.PeriodNumber });
            builder.HasIndex(e => new { e.ControlAccountId, e.IsBaseline });

            // Period Information
            builder.Property(e => e.DataDate)
                .IsRequired();

            builder.Property(e => e.PeriodType)
                .IsRequired();

            builder.Property(e => e.PeriodNumber)
                .IsRequired();

            builder.Property(e => e.Year)
                .IsRequired();

            // Core EVM Values
            builder.Property(e => e.PV)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(e => e.EV)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(e => e.AC)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(e => e.BAC)
                .IsRequired()
                .HasPrecision(18, 2);

            // Forecasts
            builder.Property(e => e.EAC)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(e => e.ETC)
                .IsRequired()
                .HasPrecision(18, 2);

            // Additional Metrics
            builder.Property(e => e.PlannedPercentComplete)
                .HasPrecision(5, 2);

            builder.Property(e => e.ActualPercentComplete)
                .HasPrecision(5, 2);

            builder.Property(e => e.Status)
                .IsRequired();

            // Analysis
            builder.Property(e => e.Comments)
                .HasMaxLength(2000);

            builder.Property(e => e.IsBaseline)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(e => e.IsApproved)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(e => e.ApprovedBy)
                .HasMaxLength(256);

            // Audit properties
            builder.Property(e => e.CreatedAt)
                .IsRequired();

            builder.Property(e => e.CreatedBy)
                .HasMaxLength(256);

            builder.Property(e => e.UpdatedAt);

            builder.Property(e => e.UpdatedBy)
                .HasMaxLength(256);

            // Relationships
            builder.HasOne(e => e.ControlAccount)
                .WithMany(ca => ca.EVMRecords)
                .HasForeignKey(e => e.ControlAccountId)
                .OnDelete(DeleteBehavior.Cascade);

            // Calculated properties (ignored)
            builder.Ignore(e => e.CV);
            builder.Ignore(e => e.SV);
            builder.Ignore(e => e.VAC);
            builder.Ignore(e => e.CPI);
            builder.Ignore(e => e.SPI);
            builder.Ignore(e => e.TCPI);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_EVMRecords_Values", 
                    "[PV] >= 0 AND [EV] >= 0 AND [AC] >= 0 AND [BAC] >= 0 AND [EAC] >= 0 AND [ETC] >= 0");
                t.HasCheckConstraint("CK_EVMRecords_Percentages", 
                    "([PlannedPercentComplete] IS NULL OR ([PlannedPercentComplete] >= 0 AND [PlannedPercentComplete] <= 100)) AND " +
                    "([ActualPercentComplete] IS NULL OR ([ActualPercentComplete] >= 0 AND [ActualPercentComplete] <= 100))");
                t.HasCheckConstraint("CK_EVMRecords_PeriodNumber", 
                    "[PeriodNumber] > 0");
                t.HasCheckConstraint("CK_EVMRecords_Year", 
                    "[Year] >= 2000 AND [Year] <= 2100");
            });
        }
    }
}