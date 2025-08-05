using Domain.Entities.Contracts.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Contracts
{
    public class ClaimRelationConfiguration : IEntityTypeConfiguration<ClaimRelation>
    {
        public void Configure(EntityTypeBuilder<ClaimRelation> builder)
        {
            // Table name and schema
            builder.ToTable("ClaimRelations", "Contracts");

            // Primary key
            builder.HasKey(cr => cr.Id);

            // Indexes
            builder.HasIndex(cr => cr.ClaimId);
            builder.HasIndex(cr => cr.RelatedClaimId);
            builder.HasIndex(cr => new { cr.ClaimId, cr.RelatedClaimId }).IsUnique();
            builder.HasIndex(cr => cr.RelationType);

            // Properties
            builder.Property(cr => cr.RelationType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(cr => cr.Notes)
                .HasMaxLength(4000);

            // Relationships
            builder.HasOne(cr => cr.Claim)
                .WithMany(c => c.RelatedClaims)
                .HasForeignKey(cr => cr.ClaimId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(cr => cr.RelatedClaim)
                .WithMany()
                .HasForeignKey(cr => cr.RelatedClaimId)
                .OnDelete(DeleteBehavior.Restrict);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_ClaimRelations_NotSelfReferencing",
                    "[ClaimId] <> [RelatedClaimId]");
            });
        }
    }
}