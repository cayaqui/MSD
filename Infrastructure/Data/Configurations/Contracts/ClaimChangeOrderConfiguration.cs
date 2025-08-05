using Domain.Entities.Contracts.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Contracts
{
    public class ClaimChangeOrderConfiguration : IEntityTypeConfiguration<ClaimChangeOrder>
    {
        public void Configure(EntityTypeBuilder<ClaimChangeOrder> builder)
        {
            // Table name and schema
            builder.ToTable("ClaimChangeOrders", "Contracts");

            // Primary key
            builder.HasKey(cco => cco.Id);

            // Indexes
            builder.HasIndex(cco => cco.ClaimId);
            builder.HasIndex(cco => cco.ChangeOrderId);
            builder.HasIndex(cco => new { cco.ClaimId, cco.ChangeOrderId }).IsUnique();
            builder.HasIndex(cco => cco.RelationType);

            // Properties
            builder.Property(cco => cco.RelationType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(cco => cco.Notes)
                .HasMaxLength(4000);

            // Relationships
            builder.HasOne(cco => cco.Claim)
                .WithMany(c => c.RelatedChangeOrders)
                .HasForeignKey(cco => cco.ClaimId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(cco => cco.ChangeOrder)
                .WithMany(co => co.RelatedClaims)
                .HasForeignKey(cco => cco.ChangeOrderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}