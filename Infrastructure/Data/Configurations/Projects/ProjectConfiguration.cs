using Domain.Entities.Projects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Setup
{
    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.ToTable("Projects", "Setup", t =>
            {
                t.HasCheckConstraint("CK_Projects_Dates", "[PlannedEndDate] > [PlannedStartDate]");
                t.HasCheckConstraint("CK_Projects_ActualDates", "[ActualEndDate] IS NULL OR [ActualStartDate] IS NULL OR [ActualEndDate] > [ActualStartDate]");
                t.HasCheckConstraint("CK_Projects_Budget", "[TotalBudget] >= 0 AND ([ApprovedBudget] IS NULL OR [ApprovedBudget] >= 0) AND ([ContingencyBudget] IS NULL OR [ContingencyBudget] >= 0)");
                t.HasCheckConstraint("CK_Projects_Costs", "([ActualCost] IS NULL OR [ActualCost] >= 0) AND ([CommittedCost] IS NULL OR [CommittedCost] >= 0)");
                t.HasCheckConstraint("CK_Projects_Progress", "[ProgressPercentage] >= 0 AND [ProgressPercentage] <= 100");
            });

            // Primary Key
            builder.HasKey(p => p.Id);

            // Properties
            builder.Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(p => p.Description)
                .HasMaxLength(2000);

            builder.Property(p => p.WBSCode)
                .IsRequired()
                .HasMaxLength(50);

            // Project Charter fields
            builder.Property(p => p.ProjectCharter)
                .HasMaxLength(4000);

            builder.Property(p => p.BusinessCase)
                .HasMaxLength(4000);

            builder.Property(p => p.Objectives)
                .HasMaxLength(4000);

            builder.Property(p => p.Scope)
                .HasMaxLength(4000);

            builder.Property(p => p.Deliverables)
                .HasMaxLength(4000);

            builder.Property(p => p.SuccessCriteria)
                .HasMaxLength(2000);

            builder.Property(p => p.Assumptions)
                .HasMaxLength(2000);

            builder.Property(p => p.Constraints)
                .HasMaxLength(2000);

            // Financial properties
            builder.Property(p => p.TotalBudget)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(p => p.ApprovedBudget)
                .HasPrecision(18, 2);

            builder.Property(p => p.ContingencyBudget)
                .HasPrecision(18, 2);

            builder.Property(p => p.ActualCost)
                .HasPrecision(18, 2);

            builder.Property(p => p.CommittedCost)
                .HasPrecision(18, 2);

            builder.Property(p => p.Currency)
                .IsRequired()
                .HasMaxLength(3)
                .HasDefaultValue("USD");

            // Progress properties
            builder.Property(p => p.ProgressPercentage)
                .HasPrecision(5, 2)
                .HasDefaultValue(0);

            builder.Property(p => p.PlannedProgress)
                .HasPrecision(5, 2);

            builder.Property(p => p.EarnedValue)
                .HasPrecision(18, 2);

            // Metadata properties
            builder.Property(p => p.ProjectManagerId)
                .HasMaxLength(128);

            builder.Property(p => p.ProjectManagerName)
                .HasMaxLength(256);

            builder.Property(p => p.Location)
                .HasMaxLength(256);

            builder.Property(p => p.Client)
                .HasMaxLength(256);

            builder.Property(p => p.ContractNumber)
                .HasMaxLength(100);

            builder.Property(p => p.PurchaseOrderNumber)
                .HasMaxLength(100);

            builder.Property(p => p.CostCenter)
                .HasMaxLength(50);

            builder.Property(p => p.LastProgressUpdateBy)
                .HasMaxLength(450);

            // Enum conversion
            builder.Property(p => p.Status)
                .HasConversion<int>()
                .IsRequired();

            // Soft delete properties
            builder.Property(p => p.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(p => p.DeletedBy)
                .HasMaxLength(450);

            // Audit properties
            builder.Property(p => p.CreatedBy)
                .HasMaxLength(450);

            builder.Property(p => p.UpdatedBy)
                .HasMaxLength(450);

            // Default values
            builder.Property(p => p.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(p => p.ChangeOrderCount)
                .HasDefaultValue(0);

            // Relationships
            builder.HasOne(p => p.Operation)
                .WithMany(o => o.Projects)
                .HasForeignKey(p => p.OperationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.ProjectTeamMembers)
                .WithOne(ptm => ptm.Project)
                .HasForeignKey(ptm => ptm.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.UserProjectPermissions)
                .WithOne(upp => upp.Project)
                .HasForeignKey(upp => upp.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(p => p.Code)
                .IsUnique()
                .HasDatabaseName("IX_Projects_Code");

            builder.HasIndex(p => p.WBSCode)
                .HasDatabaseName("IX_Projects_WBSCode");

            builder.HasIndex(p => new { p.OperationId, p.Status })
                .HasDatabaseName("IX_Projects_Operation_Status");

            builder.HasIndex(p => new { p.IsDeleted, p.IsActive })
                .HasDatabaseName("IX_Projects_IsDeleted_IsActive");

            builder.HasIndex(p => p.Status)
                .HasDatabaseName("IX_Projects_Status");

            builder.HasIndex(p => p.ProjectManagerId)
                .HasDatabaseName("IX_Projects_ProjectManagerId")
                .HasFilter("[ProjectManagerId] IS NOT NULL");

            builder.HasIndex(p => p.Client)
                .HasDatabaseName("IX_Projects_Client")
                .HasFilter("[Client] IS NOT NULL");

            builder.HasIndex(p => p.ContractNumber)
                .HasDatabaseName("IX_Projects_ContractNumber")
                .HasFilter("[ContractNumber] IS NOT NULL");

            // Query Filters
            builder.HasQueryFilter(p => !p.IsDeleted);
        }
    }
}