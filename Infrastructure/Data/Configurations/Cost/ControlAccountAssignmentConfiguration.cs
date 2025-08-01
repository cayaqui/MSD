using Domain.Entities.Cost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Cost;

/// <summary>
/// Entity configuration for ControlAccountAssignment
/// </summary>
public class ControlAccountAssignmentConfiguration : IEntityTypeConfiguration<ControlAccountAssignment>
{
    public void Configure(EntityTypeBuilder<ControlAccountAssignment> builder)
    {
        // Table name and schema
        builder.ToTable("ControlAccountAssignments", "Cost");

        // Primary key
        builder.HasKey(caa => caa.Id);

        // Indexes
        builder.HasIndex(caa => new { caa.ControlAccountId, caa.UserId, caa.Role }).IsUnique();
        builder.HasIndex(caa => caa.ControlAccountId);
        builder.HasIndex(caa => caa.UserId);
        builder.HasIndex(caa => caa.Role);
        builder.HasIndex(caa => new { caa.IsActive, caa.AssignedDate, caa.EndDate });

        // Properties
        builder.Property(caa => caa.Role)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(caa => caa.AssignedDate)
            .IsRequired();

        builder.Property(caa => caa.EndDate);

        builder.Property(caa => caa.Notes)
            .HasMaxLength(500);

        // Audit properties
        builder.Property(caa => caa.CreatedAt)
            .IsRequired();

        builder.Property(caa => caa.CreatedBy)
            .HasMaxLength(256);

        builder.Property(caa => caa.UpdatedAt);

        builder.Property(caa => caa.UpdatedBy)
            .HasMaxLength(256);

        // Foreign key relationships
        builder.HasOne(caa => caa.ControlAccount)
            .WithMany(ca => ca.Assignments)
            .HasForeignKey(caa => caa.ControlAccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(caa => caa.User)
            .WithMany()
            .HasForeignKey(caa => caa.UserId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}