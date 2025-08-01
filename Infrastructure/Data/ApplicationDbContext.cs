using Application.Interfaces.Auth;
using Application.Interfaces.Common;
using Domain.Entities.Cost;
using Domain.Entities.EVM;
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
    public DbSet<Contractor> Contractors => Set<Contractor>();
    public DbSet<Currency> Currencies => Set<Currency>();
    public DbSet<Discipline> Disciplines => Set<Discipline>();

    #endregion

    #region DbSets - Projects Module

    public DbSet<Phase> Phases => Set<Phase>();
    public DbSet<WBSElement> WBSElements => Set<WBSElement>();
    public DbSet<WorkPackageDetails> WorkPackageDetails => Set<WorkPackageDetails>();
    public DbSet<PlanningPackage> PlanningPackages => Set<PlanningPackage>();
    public DbSet<ProjectTeamMember> ProjectTeamMembers => Set<ProjectTeamMember>();

    #endregion

    #region DbSets - Cost Module

    public DbSet<Budget> Budgets => Set<Budget>();
    public DbSet<BudgetItem> BudgetItems => Set<BudgetItem>();
    public DbSet<ControlAccount> ControlAccounts => Set<ControlAccount>();
    public DbSet<ControlAccountAssignment> ControlAccountAssignments => Set<ControlAccountAssignment>();
    public DbSet<Commitment> Commitments => Set<Commitment>();
    public DbSet<CommitmentItem> CommitmentItems => Set<CommitmentItem>();
    public DbSet<CommitmentWorkPackage> CommitmentWorkPackages => Set<CommitmentWorkPackage>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();

    #endregion

    #region DbSets - Schedule Module

    public DbSet<Schedule> Schedules => Set<Schedule>();
    public DbSet<Activity> Activities => Set<Activity>();
    public DbSet<Milestone> Milestones => Set<Milestone>();

    #endregion

    #region DbSets - EVM Module

    public DbSet<EVMRecord> EVMRecords => Set<EVMRecord>();

    #endregion

    #region DbSets - Document Module

    // TODO: Implement when document management is required
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

        // Apply configurations from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Configure schemas
        modelBuilder.HasDefaultSchema("dbo");

        // Security Module
        modelBuilder.Entity<User>().ToTable("Users", "Security");
        modelBuilder.Entity<ProjectTeamMember>().ToTable("ProjectTeamMembers", "Security");

        // Setup Module
        modelBuilder.Entity<Company>().ToTable("Companies", "Setup");
        modelBuilder.Entity<Operation>().ToTable("Operations", "Setup");
        modelBuilder.Entity<Project>().ToTable("Projects", "Setup");
        modelBuilder.Entity<Contractor>().ToTable("Contractors", "Setup");
        modelBuilder.Entity<Currency>().ToTable("Currencies", "Setup");
        modelBuilder.Entity<Discipline>().ToTable("Disciplines", "Setup");

        // Projects Module
        modelBuilder.Entity<Phase>().ToTable("Phases", "Projects");
        modelBuilder.Entity<WBSElement>().ToTable("WBSElements", "Projects");
        modelBuilder.Entity<WorkPackageDetails>().ToTable("WorkPackageDetails", "Projects");
        modelBuilder.Entity<PlanningPackage>().ToTable("PlanningPackages", "Projects");

        // Cost Module
        modelBuilder.Entity<Budget>().ToTable("Budgets", "Cost");
        modelBuilder.Entity<BudgetItem>().ToTable("BudgetItems", "Cost");
        modelBuilder.Entity<ControlAccount>().ToTable("ControlAccounts", "Cost");
        modelBuilder.Entity<ControlAccountAssignment>().ToTable("ControlAccountAssignments", "Cost");
        modelBuilder.Entity<Commitment>().ToTable("Commitments", "Cost");
        modelBuilder.Entity<CommitmentItem>().ToTable("CommitmentItems", "Cost");
        modelBuilder.Entity<CommitmentWorkPackage>().ToTable("CommitmentWorkPackages", "Cost");
        modelBuilder.Entity<Invoice>().ToTable("Invoices", "Cost");
        modelBuilder.Entity<InvoiceItem>().ToTable("InvoiceItems", "Cost");

        // Schedule Module
        modelBuilder.Entity<Schedule>().ToTable("Schedules", "Schedule");
        modelBuilder.Entity<Activity>().ToTable("Activities", "Schedule");
        modelBuilder.Entity<Milestone>().ToTable("Milestones", "Schedule");

        // EVM Module
        modelBuilder.Entity<EVMRecord>().ToTable("EVMRecords", "EVM");

        // UI Module
        modelBuilder.Entity<Notification>().ToTable("Notifications", "UI");

        // Apply global query filters for soft delete
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
            {
                entityType.AddSoftDeleteQueryFilter();
            }
        }

        // Set decimal precision
        foreach (var property in modelBuilder.Model.GetEntityTypes()
            .SelectMany(t => t.GetProperties())
            .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
        {
            property.SetColumnType("decimal(18,2)");
        }

        // Configure indexes
        ConfigureIndexes(modelBuilder);

        // Configure relationships
        ConfigureRelationships(modelBuilder);
    }

    private void ConfigureIndexes(ModelBuilder modelBuilder)
    {
        // Company
        modelBuilder.Entity<Company>()
            .HasIndex(c => c.Code)
            .IsUnique();
        modelBuilder.Entity<Company>()
            .HasIndex(c => c.TaxId)
            .IsUnique();

        // Project
        modelBuilder.Entity<Project>()
            .HasIndex(p => p.Code)
            .IsUnique();
        modelBuilder.Entity<Project>()
            .HasIndex(p => new { p.IsDeleted, p.IsActive });

        // Phase
        modelBuilder.Entity<Phase>()
            .HasIndex(p => new { p.ProjectId, p.SequenceNumber })
            .IsUnique();

        // WBSElement
        modelBuilder.Entity<WBSElement>()
            .HasIndex(w => new { w.ProjectId, w.Code })
            .IsUnique();
        modelBuilder.Entity<WBSElement>()
            .HasIndex(w => w.FullPath);

        // ControlAccount
        modelBuilder.Entity<ControlAccount>()
            .HasIndex(ca => ca.Code)
            .IsUnique();

        // Contractor
        modelBuilder.Entity<Contractor>()
            .HasIndex(c => c.Code)
            .IsUnique();
        modelBuilder.Entity<Contractor>()
            .HasIndex(c => c.TaxId)
            .IsUnique();

        // Currency
        modelBuilder.Entity<Currency>()
            .HasIndex(c => c.Code)
            .IsUnique();

        // Commitment
        modelBuilder.Entity<Commitment>()
            .HasIndex(c => c.ContractNumber)
            .IsUnique();
    }

    private void ConfigureRelationships(ModelBuilder modelBuilder)
    {
        // Phase -> Project
        modelBuilder.Entity<Phase>()
            .HasOne(p => p.Project)
            .WithMany(pr => pr.Phases)
            .HasForeignKey(p => p.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        // WBSElement self-referencing
        modelBuilder.Entity<WBSElement>()
            .HasOne(w => w.ParentWBSElement)
            .WithMany(w => w.ChildElements)
            .HasForeignKey(w => w.ParentWBSElementId)
            .OnDelete(DeleteBehavior.Restrict);

        // WBSElement -> ControlAccount
        modelBuilder.Entity<WBSElement>()
            .HasOne(w => w.ControlAccount)
            .WithMany(ca => ca.WorkPackages)
            .HasForeignKey(w => w.ControlAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        // ControlAccount -> Phase
        modelBuilder.Entity<ControlAccount>()
            .HasOne(ca => ca.Phase)
            .WithMany(p => p.ControlAccounts)
            .HasForeignKey(ca => ca.PhaseId)
            .OnDelete(DeleteBehavior.Restrict);

        // ControlAccount -> CAM User
        modelBuilder.Entity<ControlAccount>()
            .HasOne(ca => ca.CAMUser)
            .WithMany()
            .HasForeignKey(ca => ca.CAMUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // ControlAccountAssignment
        modelBuilder.Entity<ControlAccountAssignment>()
            .HasOne(a => a.ControlAccount)
            .WithMany(ca => ca.Assignments)
            .HasForeignKey(a => a.ControlAccountId)
            .OnDelete(DeleteBehavior.Cascade);

        // EVMRecord -> ControlAccount
        modelBuilder.Entity<EVMRecord>()
            .HasOne(e => e.ControlAccount)
            .WithMany(ca => ca.EVMRecords)
            .HasForeignKey(e => e.ControlAccountId)
            .OnDelete(DeleteBehavior.Cascade);

        // CommitmentWorkPackage
        modelBuilder.Entity<CommitmentWorkPackage>()
            .HasOne(cw => cw.Commitment)
            .WithMany(c => c.WorkPackageAllocations)
            .HasForeignKey(cw => cw.CommitmentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CommitmentWorkPackage>()
            .HasOne(cw => cw.WBSElement)
            .WithMany(w => w.CommitmentWorkPackages)
            .HasForeignKey(cw => cw.WBSElementId)
            .OnDelete(DeleteBehavior.Restrict);

        // Commitment -> Contractor
        modelBuilder.Entity<Commitment>()
            .HasOne(c => c.Contractor)
            .WithMany(ct => ct.Commitments)
            .HasForeignKey(c => c.ContractorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Invoice -> Contractor
        modelBuilder.Entity<Invoice>()
            .HasOne(i => i.Contractor)
            .WithMany(c => c.Invoices)
            .HasForeignKey(i => i.ContractorId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    public override int SaveChanges()
    {
        AddAuditInfo();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AddAuditInfo();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void AddAuditInfo()
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is IAuditable && (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var entity = (IAuditable)entry.Entity;

            if (entry.State == EntityState.Added)
            {
                entity.CreatedAt = DateTime.UtcNow;
                entity.CreatedBy = _currentUserId ?? "System";
            }

            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = _currentUserId ?? "System";
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