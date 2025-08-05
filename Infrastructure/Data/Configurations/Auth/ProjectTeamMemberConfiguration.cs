using Domain.Entities.Auth.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Auth;

/// <summary>
/// Entity configuration for ProjectTeamMember
/// </summary>
public class ProjectTeamMemberConfiguration : IEntityTypeConfiguration<ProjectTeamMember>
{
    public void Configure(EntityTypeBuilder<ProjectTeamMember> builder)
    {
        // Table name and schema
        builder.ToTable("ProjectTeamMembers", "Security", t =>
        {
            t.HasCheckConstraint("CK_ProjectTeamMembers_Dates",
            "[EndDate] IS NULL OR [EndDate] > [StartDate]");
            t.HasCheckConstraint("CK_ProjectTeamMembers_Role",
            "[Role] IN ('PROJECT_MANAGER', 'PROJECT_ENGINEER', 'COST_CONTROLLER', 'PLANNER', 'QA_QC', 'DOCUMENT_CONTROLLER', 'TEAM_MEMBER', 'OBSERVER')");
        });

        // Primary key
        builder.HasKey(ptm => ptm.Id);

        // Indexes
        builder.HasIndex(ptm => new { ptm.UserId, ptm.ProjectId }).IsUnique();
        builder.HasIndex(ptm => ptm.UserId);
        builder.HasIndex(ptm => ptm.ProjectId);
        builder.HasIndex(ptm => ptm.Role);
        builder.HasIndex(ptm => new { ptm.IsActive, ptm.StartDate, ptm.EndDate });

        // Properties
        builder.Property(ptm => ptm.Role)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(ptm => ptm.StartDate)
            .IsRequired();

        builder.Property(ptm => ptm.EndDate);

        builder.Property(ptm => ptm.AllocationPercentage)
            .HasPrecision(5, 2);
        // Audit properties
        builder.Property(ptm => ptm.CreatedAt)
            .IsRequired();

        builder.Property(ptm => ptm.CreatedBy)
            .HasMaxLength(256);

        builder.Property(ptm => ptm.UpdatedAt);

        builder.Property(ptm => ptm.UpdatedBy)
            .HasMaxLength(256);

        // Foreign key relationships
        builder.HasOne(ptm => ptm.User)
            .WithMany(u => u.ProjectTeamMembers)
            .HasForeignKey(ptm => ptm.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ptm => ptm.Project)
            .WithMany(p => p.ProjectTeamMembers)
            .HasForeignKey(ptm => ptm.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}