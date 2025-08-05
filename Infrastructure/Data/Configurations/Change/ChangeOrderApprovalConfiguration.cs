using Domain.Entities.Change.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Change
{
    public class ChangeOrderApprovalConfiguration : IEntityTypeConfiguration<ChangeOrderApproval>
    {
        public void Configure(EntityTypeBuilder<ChangeOrderApproval> builder)
        {
            // Table name and schema
            builder.ToTable("ChangeOrderApprovals", "Change");

            // Primary key
            builder.HasKey(c => c.Id);

            // Indexes
            builder.HasIndex(c => c.ChangeOrderId);
            builder.HasIndex(c => c.ApproverId);
            builder.HasIndex(c => c.ApprovalLevel);
            builder.HasIndex(c => c.Decision);
            builder.HasIndex(c => c.DecisionDate);
            builder.HasIndex(c => new { c.ChangeOrderId, c.ApprovalLevel });

            // Foreign Keys
            builder.Property(c => c.ChangeOrderId)
                .IsRequired();

            builder.Property(c => c.ApproverId)
                .IsRequired()
                .HasMaxLength(256);

            // Basic Information
            builder.Property(c => c.ApprovalLevel)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Decision)
                .IsRequired();

            builder.Property(c => c.DecisionDate)
                .IsRequired();

            // Authority Limits
            builder.Property(c => c.AuthorityLimit)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(c => c.IsWithinAuthority)
                .IsRequired()
                .HasDefaultValue(false);

            // Decision Details
            builder.Property(c => c.Comments)
                .HasMaxLength(2000);

            builder.Property(c => c.Conditions)
                .HasMaxLength(2000);

            builder.Property(c => c.IsConditional)
                .IsRequired()
                .HasDefaultValue(false);

            // Audit properties
            builder.Property(c => c.CreatedAt)
                .IsRequired();

            builder.Property(c => c.CreatedBy)
                .HasMaxLength(256);

            builder.Property(c => c.UpdatedAt);

            builder.Property(c => c.UpdatedBy)
                .HasMaxLength(256);

            // Relationships
            builder.HasOne(c => c.ChangeOrder)
                .WithMany(co => co.Approvals)
                .HasForeignKey(c => c.ChangeOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Note: ApproverId is a string (likely Entra ID) and cannot be linked to User.Id (Guid)
            // If you need to link to User, consider adding a separate UserId property of type Guid
            builder.Ignore(c => c.Approver);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_ChangeOrderApprovals_AuthorityLimit", 
                    "[AuthorityLimit] >= 0");
            });
        }
    }
}