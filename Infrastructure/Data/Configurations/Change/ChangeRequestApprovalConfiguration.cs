using Domain.Entities.Change.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Change
{
    public class ChangeRequestApprovalConfiguration : IEntityTypeConfiguration<ChangeRequestApproval>
    {
        public void Configure(EntityTypeBuilder<ChangeRequestApproval> builder)
        {
            // Table name and schema
            builder.ToTable("ChangeRequestApprovals", "Change");

            // Primary key
            builder.HasKey(c => c.Id);

            // Indexes
            builder.HasIndex(c => c.ChangeRequestId);
            builder.HasIndex(c => c.ApproverId);
            builder.HasIndex(c => c.ApprovalLevel);
            builder.HasIndex(c => c.ApprovalDate);
            builder.HasIndex(c => new { c.ChangeRequestId, c.ApprovalLevel });

            // Foreign Keys
            builder.Property(c => c.ChangeRequestId)
                .IsRequired();

            builder.Property(c => c.ApproverId)
                .IsRequired();

            // Properties
            builder.Property(c => c.ApprovalLevel)
                .IsRequired();

            builder.Property(c => c.IsApproved)
                .IsRequired();

            builder.Property(c => c.ApprovalDate)
                .IsRequired();

            builder.Property(c => c.Comments)
                .HasMaxLength(2000);

            // Audit properties
            builder.Property(c => c.CreatedAt)
                .IsRequired();

            builder.Property(c => c.CreatedBy)
                .HasMaxLength(256);

            builder.Property(c => c.UpdatedAt);

            builder.Property(c => c.UpdatedBy)
                .HasMaxLength(256);

            // Relationships
            builder.HasOne(c => c.ChangeRequest)
                .WithMany(cr => cr.Approvals)
                .HasForeignKey(c => c.ChangeRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.Approver)
                .WithMany()
                .HasForeignKey(c => c.ApproverId)
                .OnDelete(DeleteBehavior.Restrict);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_ChangeRequestApprovals_ApprovalLevel", 
                    "[ApprovalLevel] >= 1 AND [ApprovalLevel] <= 10");
            });
        }
    }
}