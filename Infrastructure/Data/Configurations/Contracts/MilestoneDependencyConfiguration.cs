using Domain.Entities.Contracts.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Contracts
{
    public class MilestoneDependencyConfiguration : IEntityTypeConfiguration<MilestoneDependency>
    {
        public void Configure(EntityTypeBuilder<MilestoneDependency> builder)
        {
            // Table name and schema
            builder.ToTable("MilestoneDependencies", "Contracts");

            // Primary key
            builder.HasKey(md => md.Id);

            // Indexes
            builder.HasIndex(md => md.PredecessorId);
            builder.HasIndex(md => md.SuccessorId);
            builder.HasIndex(md => new { md.PredecessorId, md.SuccessorId }).IsUnique();
            builder.HasIndex(md => md.DependencyType);

            // Properties
            builder.Property(md => md.DependencyType)
                .IsRequired()
                .HasMaxLength(2)
                .HasDefaultValue("FS");

            builder.Property(md => md.LagDays)
                .IsRequired()
                .HasDefaultValue(0);

            // Relationships
            builder.HasOne(md => md.Predecessor)
                .WithMany(m => m.Successors)
                .HasForeignKey(md => md.PredecessorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(md => md.Successor)
                .WithMany(m => m.Predecessors)
                .HasForeignKey(md => md.SuccessorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_MilestoneDependencies_DependencyType", 
                    "[DependencyType] IN ('FS', 'SS', 'FF', 'SF')");
                t.HasCheckConstraint("CK_MilestoneDependencies_NotSelfReferencing",
                    "[PredecessorId] <> [SuccessorId]");
            });
        }
    }
}