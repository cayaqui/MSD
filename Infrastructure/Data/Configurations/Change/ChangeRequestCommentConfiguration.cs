using Domain.Entities.Change.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Change
{
    public class ChangeRequestCommentConfiguration : IEntityTypeConfiguration<ChangeRequestComment>
    {
        public void Configure(EntityTypeBuilder<ChangeRequestComment> builder)
        {
            // Table name and schema
            builder.ToTable("ChangeRequestComments", "Change");

            // Primary key
            builder.HasKey(c => c.Id);

            // Indexes
            builder.HasIndex(c => c.ChangeRequestId);
            builder.HasIndex(c => c.UserId);
            builder.HasIndex(c => c.CommentDate);
            builder.HasIndex(c => c.IsInternal);

            // Foreign Keys
            builder.Property(c => c.ChangeRequestId)
                .IsRequired();

            builder.Property(c => c.UserId)
                .IsRequired();

            // Properties
            builder.Property(c => c.Comment)
                .IsRequired()
                .HasMaxLength(4000);

            builder.Property(c => c.CommentDate)
                .IsRequired();

            builder.Property(c => c.IsInternal)
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
            builder.HasOne(c => c.ChangeRequest)
                .WithMany(cr => cr.Comments)
                .HasForeignKey(c => c.ChangeRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}