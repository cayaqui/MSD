using Domain.Entities.Change.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Change
{
    public class TrendCommentConfiguration : IEntityTypeConfiguration<TrendComment>
    {
        public void Configure(EntityTypeBuilder<TrendComment> builder)
        {
            // Table name and schema
            builder.ToTable("TrendComments", "Change");

            // Primary key
            builder.HasKey(t => t.Id);

            // Indexes
            builder.HasIndex(t => t.TrendId);
            builder.HasIndex(t => t.UserId);
            builder.HasIndex(t => t.CommentDate);

            // Foreign Keys
            builder.Property(t => t.TrendId)
                .IsRequired();

            builder.Property(t => t.UserId)
                .IsRequired();

            // Properties
            builder.Property(t => t.Comment)
                .IsRequired()
                .HasMaxLength(4000);

            builder.Property(t => t.CommentDate)
                .IsRequired();

            // Audit properties
            builder.Property(t => t.CreatedAt)
                .IsRequired();

            builder.Property(t => t.CreatedBy)
                .HasMaxLength(256);

            builder.Property(t => t.UpdatedAt);

            builder.Property(t => t.UpdatedBy)
                .HasMaxLength(256);

            // Relationships
            builder.HasOne(t => t.Trend)
                .WithMany(trend => trend.Comments)
                .HasForeignKey(t => t.TrendId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}