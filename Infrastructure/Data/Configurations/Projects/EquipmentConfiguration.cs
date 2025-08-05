using Domain.Entities.Progress;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Projects
{
    public class EquipmentConfiguration : IEntityTypeConfiguration<Equipment>
    {
        public void Configure(EntityTypeBuilder<Equipment> builder)
        {
            // Table name and schema
            builder.ToTable("Equipment", "Projects");

            // Primary key
            builder.HasKey(e => e.Id);

            // Indexes
            builder.HasIndex(e => e.EquipmentCode).IsUnique();
            builder.HasIndex(e => e.Category);
            builder.HasIndex(e => e.IsActive);
            builder.HasIndex(e => e.IsAvailable);
            builder.HasIndex(e => e.CurrentProjectId);
            builder.HasIndex(e => e.ContractorId);
            builder.HasIndex(e => e.IsDeleted);
            builder.HasIndex(e => new { e.IsActive, e.IsAvailable });

            // Basic Information
            builder.Property(e => e.EquipmentCode)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.Description)
                .HasMaxLength(500);

            builder.Property(e => e.Model)
                .HasMaxLength(100);

            builder.Property(e => e.SerialNumber)
                .HasMaxLength(100);

            // Classification
            builder.Property(e => e.Category)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.Manufacturer)
                .HasMaxLength(200);

            builder.Property(e => e.YearOfManufacture);

            // Ownership
            builder.Property(e => e.IsOwned)
                .IsRequired();

            builder.Property(e => e.PurchaseValue)
                .HasPrecision(18, 2);

            builder.Property(e => e.PurchaseDate);

            // Rates
            builder.Property(e => e.DailyRate)
                .HasPrecision(18, 2);

            builder.Property(e => e.HourlyRate)
                .HasPrecision(18, 2);

            builder.Property(e => e.Currency)
                .IsRequired()
                .HasMaxLength(3)
                .HasDefaultValue("USD");

            // Status
            builder.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(e => e.IsAvailable)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(e => e.CurrentLocation)
                .HasMaxLength(200);

            // Maintenance
            builder.Property(e => e.LastMaintenanceDate);

            builder.Property(e => e.NextMaintenanceDate);

            builder.Property(e => e.MaintenanceHours)
                .HasPrecision(18, 2);

            // Soft Delete
            builder.Property(e => e.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(e => e.DeletedAt);

            builder.Property(e => e.DeletedBy)
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
            builder.HasOne(e => e.Contractor)
                .WithMany()
                .HasForeignKey(e => e.ContractorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.CurrentProject)
                .WithMany()
                .HasForeignKey(e => e.CurrentProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            // Soft delete filter
            builder.HasQueryFilter(e => !e.IsDeleted);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Equipment_PurchaseValue", 
                    "[PurchaseValue] IS NULL OR [PurchaseValue] >= 0");
                t.HasCheckConstraint("CK_Equipment_DailyRate", 
                    "[DailyRate] IS NULL OR [DailyRate] >= 0");
                t.HasCheckConstraint("CK_Equipment_HourlyRate", 
                    "[HourlyRate] IS NULL OR [HourlyRate] >= 0");
                t.HasCheckConstraint("CK_Equipment_MaintenanceHours", 
                    "[MaintenanceHours] IS NULL OR [MaintenanceHours] >= 0");
                t.HasCheckConstraint("CK_Equipment_YearOfManufacture", 
                    "[YearOfManufacture] IS NULL OR ([YearOfManufacture] >= 1900 AND [YearOfManufacture] <= 2100)");
            });
        }
    }
}