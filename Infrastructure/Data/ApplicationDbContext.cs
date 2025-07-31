using Application.Interfaces.Auth;
using Application.Interfaces.Common;
using Domain.Entities.Projects;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Graph.Models;
using System.Data;
using System.Reflection;
using Operation = Domain.Entities.Setup.Operation;
using User = Domain.Entities.Security.User;

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
    //public DbSet<ProjectPhase> ProjectPhases => Set<ProjectPhase>();
    public DbSet<WorkPackage> WorkPackages => Set<WorkPackage>();
    public DbSet<ProjectTeamMember> ProjectTeamMembers => Set<ProjectTeamMember>();
    public DbSet<Discipline> Disciplines => Set<Discipline>();
    //public DbSet<DocumentType> DocumentTypes => Set<DocumentType>();
    //public DbSet<Currency> Currencies => Set<Currency>();

    #endregion

    #region DbSets - Cost Module

    public DbSet<Budget> Budgets => Set<Budget>();
    public DbSet<BudgetItem> BudgetItems => Set<BudgetItem>();
    //public DbSet<CostAccount> CostAccounts => Set<CostAccount>();
    public DbSet<Commitment> Commitments => Set<Commitment>();
    //public DbSet<CommitmentItem> CommitmentItems => Set<CommitmentItem>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();

    #endregion

    #region DbSets - Schedule Module

    public DbSet<Schedule> Schedules => Set<Schedule>();
    public DbSet<Activity> Activities => Set<Activity>();
    //public DbSet<ActivityDependency> ActivityDependencies => Set<ActivityDependency>();
    public DbSet<Milestone> Milestones => Set<Milestone>();

    #endregion

    #region DbSets - Document Module

    //public DbSet<Document> Documents => Set<Document>();
    //public DbSet<DocumentRevision> DocumentRevisions => Set<DocumentRevision>();
    //public DbSet<Transmittal> Transmittals => Set<Transmittal>();

    #endregion

    #region UIUX
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

