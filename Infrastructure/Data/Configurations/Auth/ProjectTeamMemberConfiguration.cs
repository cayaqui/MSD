namespace Infrastructure.Data.Configurations.Auth;

/// <summary>
/// Entity configuration for ProjectTeamMember
/// </summary>
public class ProjectTeamMemberConfiguration : IEntityTypeConfiguration<ProjectTeamMember>
{
    public void Configure(EntityTypeBuilder<ProjectTeamMember> builder)
    {
        // Table name
        builder.ToTable("ProjectTeamMembers", "Security");

        // Primary key
        builder.HasKey(ptm => ptm.Id);

        // Indexes
        builder.HasIndex(ptm => new { ptm.ProjectId, ptm.UserId, ptm.Role });
        builder.HasIndex(ptm => new { ptm.UserId, ptm.IsActive });

        // Properties
        builder.Property(ptm => ptm.Role)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(ptm => ptm.AllocationPercentage)
            .HasPrecision(5, 2);

        // Relationships
        builder.HasOne(ptm => ptm.Project)
            .WithMany(p => p.ProjectTeamMembers)
            .HasForeignKey(ptm => ptm.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ptm => ptm.User)
            .WithMany()
            .HasForeignKey(ptm => ptm.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}