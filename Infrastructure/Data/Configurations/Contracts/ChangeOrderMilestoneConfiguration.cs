using Domain.Entities.Contracts.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Contracts
{
    public class ChangeOrderMilestoneConfiguration : IEntityTypeConfiguration<ChangeOrderMilestone>
    {
        public void Configure(EntityTypeBuilder<ChangeOrderMilestone> builder)
        {
            // Table name and schema
            builder.ToTable("ChangeOrderMilestones", "Contracts");

            // Primary key
            builder.HasKey(com => com.Id);

            // Indexes
            builder.HasIndex(com => com.ChangeOrderId);
            builder.HasIndex(com => com.MilestoneId);
            builder.HasIndex(com => new { com.ChangeOrderId, com.MilestoneId }).IsUnique();
            builder.HasIndex(com => com.ImpactType);

            // Properties
            builder.Property(com => com.ImpactType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(com => com.ImpactDescription)
                .IsRequired()
                .HasMaxLength(4000);

            // Relationships
            builder.HasOne(com => com.ChangeOrder)
                .WithMany(co => co.AffectedMilestones)
                .HasForeignKey(com => com.ChangeOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(com => com.Milestone)
                .WithMany(m => m.ChangeOrders)
                .HasForeignKey(com => com.MilestoneId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}