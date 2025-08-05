using Domain.Entities.Cost.Core;

namespace Infrastructure.Data.Configurations.Cost
{
    public class WBSCBSMappingConfiguration : IEntityTypeConfiguration<WBSCBSMapping>
    {
        public void Configure(EntityTypeBuilder<WBSCBSMapping> builder)
        {
            // Table name and schema
            builder.ToTable("WBSCBSMappings", "Cost");

            // Primary key
            builder.HasKey(w => w.Id);

            // Indexes
            builder.HasIndex(w => w.WBSElementId);
            builder.HasIndex(w => w.CBSId);
            builder.HasIndex(w => w.AllocationPercentage);
            builder.HasIndex(w => new { w.WBSElementId, w.CBSId }).IsUnique();

            // Allocation Information
            builder.Property(w => w.AllocationPercentage)
                .IsRequired()
                .HasPrecision(5, 2);

            builder.Property(w => w.AllocationBasis)
                .HasMaxLength(50);

            builder.Property(w => w.IsPrimary)
                .IsRequired()
                .HasDefaultValue(false);

            // Period
            builder.Property(w => w.EffectiveDate)
                .IsRequired();

            builder.Property(w => w.EndDate);

            // Audit properties
            builder.Property(w => w.CreatedAt)
                .IsRequired();

            builder.Property(w => w.CreatedBy)
                .HasMaxLength(256);

            builder.Property(w => w.UpdatedAt);

            builder.Property(w => w.UpdatedBy)
                .HasMaxLength(256);

            // Relationships
            builder.HasOne(w => w.WBSElement)
                .WithMany(wbs => wbs.CBSMappings)
                .HasForeignKey(w => w.WBSElementId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(w => w.CBS)
                .WithMany()
                .HasForeignKey(w => w.CBSId)
                .OnDelete(DeleteBehavior.Restrict);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_WBSCBSMappings_AllocationPercentage", 
                    "[AllocationPercentage] > 0 AND [AllocationPercentage] <= 100");
                t.HasCheckConstraint("CK_WBSCBSMappings_EndDate", 
                    "[EndDate] IS NULL OR [EndDate] >= [EffectiveDate]");
            });
        }
    }
}