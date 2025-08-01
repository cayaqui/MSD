using Domain.Entities.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core.Constants;

namespace Infrastructure.Data.Configurations.Security
{
    public class ProjectTeamMemberConfiguration : IEntityTypeConfiguration<ProjectTeamMember>
    {
        public void Configure(EntityTypeBuilder<ProjectTeamMember> builder)
        {
            builder.ToTable("ProjectTeamMembers", "Security", p =>
            {
                p.HasCheckConstraint("CK_ProjectTeamMembers_EndDate", "[EndDate] IS NULL OR [EndDate] > [StartDate]");
                p.HasCheckConstraint("CK_ProjectTeamMembers_ValidRole", "[Role] IN ('" + string.Join("','", ProjectRoles.AllRoles) + "')");
                p.HasCheckConstraint("CK_ProjectTeamMembers_AllocationPercentage", "[AllocationPercentage] IS NULL OR ([AllocationPercentage] >= 0 AND [AllocationPercentage] <= 100)");
            });

            // Primary Key
            builder.HasKey(ptm => ptm.Id);

            // Properties
            builder.Property(ptm => ptm.Role)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(ptm => ptm.AllocationPercentage)
                .HasPrecision(5, 2)
                .HasDefaultValue(100m);

            builder.Property(ptm => ptm.StartDate)
                .IsRequired();

            builder.Property(ptm => ptm.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Audit properties
            builder.Property(ptm => ptm.CreatedBy)
                .HasMaxLength(450);

            builder.Property(ptm => ptm.UpdatedBy)
                .HasMaxLength(450);

            // Relationships
            builder.HasOne(ptm => ptm.User)
                .WithMany(u => u.ProjectTeamMembers)
                .HasForeignKey(ptm => ptm.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ptm => ptm.Project)
                .WithMany(p => p.ProjectTeamMembers)
                .HasForeignKey(ptm => ptm.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(ptm => new { ptm.ProjectId, ptm.UserId, ptm.IsActive })
                .HasDatabaseName("IX_ProjectTeamMembers_Project_User_Active")
                .IsUnique()
                .HasFilter("[IsActive] = 1");

            builder.HasIndex(ptm => ptm.UserId)
                .HasDatabaseName("IX_ProjectTeamMembers_UserId");

            builder.HasIndex(ptm => ptm.ProjectId)
                .HasDatabaseName("IX_ProjectTeamMembers_ProjectId");

            builder.HasIndex(ptm => ptm.Role)
                .HasDatabaseName("IX_ProjectTeamMembers_Role");

            builder.HasIndex(ptm => ptm.IsActive)
                .HasDatabaseName("IX_ProjectTeamMembers_IsActive");

            builder.HasIndex(ptm => new { ptm.StartDate, ptm.EndDate })
                .HasDatabaseName("IX_ProjectTeamMembers_StartDate_EndDate");

            // Query Filters
            builder.HasQueryFilter(ptm => ptm.IsActive);
        }
    }
}