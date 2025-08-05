using Domain.Entities.Cost.Control;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Cost
{
    public class ControlAccountAssignmentConfiguration : IEntityTypeConfiguration<ControlAccountAssignment>
    {
        public void Configure(EntityTypeBuilder<ControlAccountAssignment> builder)
        {
            // Table name and schema
            builder.ToTable("ControlAccountAssignments", "Cost");

            // Primary key
            builder.HasKey(c => c.Id);

            // Indexes
            builder.HasIndex(c => c.ControlAccountId);
            builder.HasIndex(c => c.UserId);
            builder.HasIndex(c => c.Role);
            builder.HasIndex(c => c.AssignedDate);
            builder.HasIndex(c => c.IsActive);
            builder.HasIndex(c => new { c.ControlAccountId, c.UserId, c.Role }).IsUnique();
            builder.HasIndex(c => new { c.UserId, c.IsActive });

            // Assignment Information
            builder.Property(c => c.Role)
                .IsRequired()
                .HasMaxLength(50);

            // Period
            builder.Property(c => c.AssignedDate)
                .IsRequired();

            builder.Property(c => c.EndDate);

            // Allocation
            builder.Property(c => c.AllocationPercentage)
                .HasPrecision(5, 2);

            // Status
            builder.Property(c => c.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Notes
            builder.Property(c => c.Notes)
                .HasMaxLength(1000);

            // Audit properties
            builder.Property(c => c.CreatedAt)
                .IsRequired();

            builder.Property(c => c.CreatedBy)
                .HasMaxLength(256);

            builder.Property(c => c.UpdatedAt);

            builder.Property(c => c.UpdatedBy)
                .HasMaxLength(256);

            // Relationships
            builder.HasOne(c => c.ControlAccount)
                .WithMany(ca => ca.Assignments)
                .HasForeignKey(c => c.ControlAccountId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_ControlAccountAssignments_AllocationPercentage", 
                    "[AllocationPercentage] IS NULL OR ([AllocationPercentage] >= 0 AND [AllocationPercentage] <= 100)");
                t.HasCheckConstraint("CK_ControlAccountAssignments_EndDate", 
                    "[EndDate] IS NULL OR [EndDate] >= [AssignedDate]");
            });
        }
    }
}