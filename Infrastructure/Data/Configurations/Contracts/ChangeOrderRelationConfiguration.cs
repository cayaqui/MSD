using Domain.Entities.Contracts.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Contracts
{
    public class ChangeOrderRelationConfiguration : IEntityTypeConfiguration<ChangeOrderRelation>
    {
        public void Configure(EntityTypeBuilder<ChangeOrderRelation> builder)
        {
            // Table name and schema
            builder.ToTable("ChangeOrderRelations", "Contracts");

            // Primary key
            builder.HasKey(cor => cor.Id);

            // Indexes
            builder.HasIndex(cor => cor.ChangeOrderId);
            builder.HasIndex(cor => cor.RelatedChangeOrderId);
            builder.HasIndex(cor => new { cor.ChangeOrderId, cor.RelatedChangeOrderId }).IsUnique();
            builder.HasIndex(cor => cor.RelationType);

            // Properties
            builder.Property(cor => cor.RelationType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(cor => cor.Notes)
                .HasMaxLength(4000);

            // Relationships
            builder.HasOne(cor => cor.ChangeOrder)
                .WithMany(co => co.RelatedChangeOrders)
                .HasForeignKey(cor => cor.ChangeOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(cor => cor.RelatedChangeOrder)
                .WithMany()
                .HasForeignKey(cor => cor.RelatedChangeOrderId)
                .OnDelete(DeleteBehavior.Restrict);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_ChangeOrderRelations_NotSelfReferencing",
                    "[ChangeOrderId] <> [RelatedChangeOrderId]");
            });
        }
    }
}