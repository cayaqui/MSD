using Application.Interfaces.Auth;
using Application.Interfaces.Common;
using Domain.Entities.Projects;
using Domain.Entities.Cost;
using Domain.Entities.Progress;
using Domain.Entities.Risk;
using Domain.Entities.EVM;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using Operation = Domain.Entities.Setup.Operation;
using User = Domain.Entities.Security.User;
using Domain.Entities.Change;

namespace Infrastructure.Data;

/// <summary>
/// Application database context for EzPro system
/// </summary>
public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly string? _currentUserId;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentUserService currentUserService)
        : base(options)
    {
        _currentUserId = currentUserService?.UserId;
    }

    #region DbSets - Security Module

    public DbSet<User> Users => Set<User>();
    public DbSet<ProjectTeamMember> TeamMembers => Set<ProjectTeamMember>();

    #endregion

    #region DbSets - Setup Module

    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Operation> Operations => Set<Operation>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Phase> Phases => Set<Phase>();
    public DbSet<Discipline> Disciplines => Set<Discipline>();
    public DbSet<Contractor> Contractors => Set<Contractor>();
    public DbSet<ProjectTeamMember> ProjectTeamMembers => Set<ProjectTeamMember>();

    #endregion

    #region DbSets - WBS Module

    public DbSet<WBSElement> WBSElements => Set<WBSElement>();
    public DbSet<WorkPackageDetails> WorkPackageDetails => Set<WorkPackageDetails>();
    public DbSet<Package> Packages => Set<Package>();
    public DbSet<PackageDiscipline> PackageDisciplines => Set<PackageDiscipline>();

    #endregion

    #region DbSets - Cost Module

    public DbSet<Budget> Budgets => Set<Budget>();
    public DbSet<BudgetItem> BudgetItems => Set<BudgetItem>();
    public DbSet<ControlAccount> ControlAccounts => Set<ControlAccount>();
    public DbSet<ControlAccountAssignment> ControlAccountAssignments => Set<ControlAccountAssignment>();
    public DbSet<CBS> CBSElements => Set<CBS>();
    public DbSet<CostItem> CostItems => Set<CostItem>();
    public DbSet<PlanningPackage> PlanningPackages => Set<PlanningPackage>();
    public DbSet<Commitment> Commitments => Set<Commitment>();
    public DbSet<CommitmentWorkPackage> CommitmentWorkPackages => Set<CommitmentWorkPackage>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();

    #endregion

    #region DbSets - Schedule Module

    public DbSet<ScheduleVersion> Schedules => Set<ScheduleVersion>();
    public DbSet<Activity> Activities => Set<Activity>();
    public DbSet<Resource> Resources => Set<Resource>();
    public DbSet<Milestone> Milestones => Set<Milestone>();

    #endregion

    #region DbSets - EVM Module

    public DbSet<EVMRecord> EVMRecords => Set<EVMRecord>();

    #endregion

    #region DbSets - Risk Module

    public DbSet<Risk> Risks => Set<Risk>();
    public DbSet<RiskResponse> RiskResponses => Set<RiskResponse>();
    public DbSet<RiskReview> RiskReviews => Set<RiskReview>();

    #endregion

    #region DbSets - Change Management Module

    public DbSet<ChangeOrder> ChangeOrders => Set<ChangeOrder>();
    public DbSet<ChangeOrderApproval> ChangeOrderApprovals => Set<ChangeOrderApproval>();
    public DbSet<ChangeOrderImpact> ChangeOrderImpacts => Set<ChangeOrderImpact>();

    #endregion

    #region DbSets - Document Module (Currently Inactive)

    //public DbSet<Document> Documents => Set<Document>();
    //public DbSet<DocumentRevision> DocumentRevisions => Set<DocumentRevision>();
    //public DbSet<Transmittal> Transmittals => Set<Transmittal>();

    #endregion

    #region DbSets - UI/UX Module

    public DbSet<Notification> Notifications => Set<Notification>();

    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Apply global query filters for soft delete
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
            {
                entityType.AddSoftDeleteQueryFilter();
            }
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Update audit fields
        UpdateAuditableEntities();

        // Update soft delete fields
        UpdateSoftDeleteEntities();

        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateAuditableEntities()
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is IAuditable &&
                       (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var entity = (IAuditable)entry.Entity;
            var now = DateTime.UtcNow;
            var userId = _currentUserId ?? "System";

            if (entry.State == EntityState.Added)
            {
                entity.CreatedAt = now;
                entity.CreatedBy = userId;
            }
            else
            {
                // Prevent modification of creation audit fields
                entry.Property(nameof(IAuditable.CreatedAt)).IsModified = false;
                entry.Property(nameof(IAuditable.CreatedBy)).IsModified = false;
            }

            entity.UpdatedAt = now;
            entity.UpdatedBy = userId;
        }
    }

    private void UpdateSoftDeleteEntities()
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is ISoftDelete && e.State == EntityState.Deleted);

        foreach (var entry in entries)
        {
            var entity = (ISoftDelete)entry.Entity;

            // Change the state to modified instead of deleted
            entry.State = EntityState.Modified;

            // Set soft delete properties
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
            entity.DeletedBy = _currentUserId ?? "System";
        }
    }
}

/// <summary>
/// Extension methods for ModelBuilder
/// </summary>
public static class ModelBuilderExtensions
{
    public static void AddSoftDeleteQueryFilter(this IMutableEntityType entityType)
    {
        var method = typeof(ModelBuilderExtensions)
            .GetMethod(nameof(GetSoftDeleteFilter), BindingFlags.NonPublic | BindingFlags.Static)
            ?.MakeGenericMethod(entityType.ClrType);

        var filter = method?.Invoke(null, Array.Empty<object>());
        entityType.SetQueryFilter((LambdaExpression?)filter);
    }

    private static LambdaExpression GetSoftDeleteFilter<TEntity>() where TEntity : class, ISoftDelete
    {
        Expression<Func<TEntity, bool>> filter = x => !x.IsDeleted;
        return filter;
    }
}
