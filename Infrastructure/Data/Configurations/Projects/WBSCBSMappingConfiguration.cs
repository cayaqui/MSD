namespace Infrastructure.Data.Configurations.Projects;

/// <summary>
/// Entity Framework configuration for WBSCBSMapping
/// </summary>
public class WBSCBSMappingConfiguration : IEntityTypeConfiguration<WBSCBSMapping>
{
    public void Configure(EntityTypeBuilder<WBSCBSMapping> builder)
    {
        // Table name and schema
        builder.ToTable("WBSCBSMappings", "Projects");

        // Primary Key
        builder.HasKey(e => e.Id);

        // Properties
        builder.Property(e => e.AllocationPercentage)
            .IsRequired()
            .HasPrecision(5, 2);

        builder.Property(e => e.IsPrimary)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.AllocationBasis)
            .HasMaxLength(200);

        builder.Property(e => e.EffectiveDate)
            .IsRequired();

        // Audit fields
        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.CreatedBy)
            .HasMaxLength(100);

        builder.Property(e => e.UpdatedBy)
            .HasMaxLength(100);

        // Indexes
        builder.HasIndex(e => new { e.WBSElementId, e.CBSId, e.EffectiveDate })
            .IsUnique()
            .HasFilter("[EndDate] IS NULL");

        builder.HasIndex(e => e.CBSId);

        // Relationships
        builder.HasOne(e => e.WBSElement)
            .WithMany(w => w.CBSMappings)
            .HasForeignKey(e => e.WBSElementId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.CBS)
            .WithMany()
            .HasForeignKey(e => e.CBSId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}