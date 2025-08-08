using Domain.Entities.Auth.Security;
using Domain.Entities.Configuration.Core;
using Domain.Entities.Configuration.Templates;
using Domain.Entities.Contracts.Core;
using Domain.Entities.Cost.Budget;
using Domain.Entities.Cost.Commitments;
using Domain.Entities.Cost.Control;
using Domain.Entities.Cost.Core;
using Domain.Entities.Cost.EVM;
using Domain.Entities.Documents.Core;
using Domain.Entities.Documents.Transmittals;
using DocumentView = Domain.Entities.Documents.DocumentView;
using DocumentDownload = Domain.Entities.Documents.DocumentDownload;
using Domain.Entities.Organization.Core;
using Domain.Entities.Progress;
using Domain.Entities.WBS;
using Infrastructure.Data.Configurations.Documents;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using ContractClaim = Domain.Entities.Contracts.Core.Claim;
using Operation = Domain.Entities.Organization.Core.Operation;
using User = Domain.Entities.Auth.Security.User;

namespace Infrastructure.Data;

/// <summary>
/// Application database context for EzPro system
/// </summary>
public class ApplicationDbContext : DbContext 
{
    private readonly IUserContext? _userContext;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IUserContext userContext)
        : base(options)
    {
        _userContext = userContext;
    }

    #region DbSets - Security Module

    public DbSet<User> Users => Set<User>();
    public DbSet<ProjectTeamMember> TeamMembers => Set<ProjectTeamMember>();
    public DbSet<UserSession> UserSessions => Set<UserSession>();

    #endregion

    #region DbSets - Organization Module

    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Contractor> Contractors => Set<Contractor>();
    public DbSet<Currency> Currencies => Set<Currency>();
    public DbSet<Discipline> Disciplines => Set<Discipline>();
    public DbSet<OBSNode> OBSNodes => Set<OBSNode>();
    public DbSet<Operation> Operations => Set<Operation>();
    public DbSet<Package> Packages => Set<Package>();
    public DbSet<PackageDiscipline> PackageDisciplines => Set<PackageDiscipline>();
    public DbSet<Phase> Phases => Set<Phase>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<RAM> RAMAssignments => Set<RAM>();
    #endregion

  
    #region DbSets - UI/UX Module

    public DbSet<Notification> Notifications => Set<Notification>();

    #endregion

    #region DbSets - Configuration Module

    public DbSet<ProjectType> ProjectTypes => Set<ProjectType>();
    public DbSet<SystemParameter> SystemParameters => Set<SystemParameter>();
    public DbSet<WBSTemplate> WBSTemplates => Set<WBSTemplate>();
    public DbSet<WBSTemplateElement> WBSTemplateElements => Set<WBSTemplateElement>();
    public DbSet<CBSTemplate> CBSTemplates => Set<CBSTemplate>();
    public DbSet<CBSTemplateElement> CBSTemplateElements => Set<CBSTemplateElement>();

    #endregion

    #region DbSets - Contracts Module

    public DbSet<Contract> Contracts => Set<Contract>();
    public DbSet<ContractChangeOrder> ContractChangeOrders => Set<ContractChangeOrder>();
    public DbSet<ContractMilestone> ContractMilestones => Set<ContractMilestone>();
    public DbSet<ContractClaim> Claims => Set<ContractClaim>();
    public DbSet<Valuation> Valuations => Set<Valuation>();
    public DbSet<ValuationItem> ValuationItems => Set<ValuationItem>();
    public DbSet<ContractDocument> ContractDocuments => Set<ContractDocument>();
    public DbSet<ChangeOrderDocument> ChangeOrderDocuments => Set<ChangeOrderDocument>();
    public DbSet<MilestoneDocument> MilestoneDocuments => Set<MilestoneDocument>();
    public DbSet<ClaimDocument> ClaimDocuments => Set<ClaimDocument>();
    public DbSet<ValuationDocument> ValuationDocuments => Set<ValuationDocument>();
    public DbSet<ChangeOrderMilestone> ChangeOrderMilestones => Set<ChangeOrderMilestone>();
    public DbSet<MilestoneDependency> MilestoneDependencies => Set<MilestoneDependency>();
    public DbSet<ClaimRelation> ClaimRelations => Set<ClaimRelation>();
    public DbSet<ClaimChangeOrder> ClaimChangeOrders => Set<ClaimChangeOrder>();
    public DbSet<ChangeOrderRelation> ChangeOrderRelations => Set<ChangeOrderRelation>();

    #endregion

    #region DbSets - Projects Module

    public DbSet<WBSElement> WBSElements => Set<WBSElement>();
    public DbSet<WorkPackageDetails> WorkPackageDetails => Set<WorkPackageDetails>();
    public DbSet<PlanningPackage> PlanningPackages => Set<PlanningPackage>();
    public DbSet<ScheduleVersion> ScheduleVersions => Set<ScheduleVersion>();
    public DbSet<Activity> Activities => Set<Activity>();
    public DbSet<Milestone> Milestones => Set<Milestone>();
    public DbSet<Resource> Resources => Set<Resource>();

    #endregion

    #region DbSets - Cost Module

    public DbSet<Budget> Budgets => Set<Budget>();
    public DbSet<BudgetItem> BudgetItems => Set<BudgetItem>();
    public DbSet<BudgetRevision> BudgetRevisions => Set<BudgetRevision>();
    public DbSet<Commitment> Commitments => Set<Commitment>();
    public DbSet<CommitmentItem> CommitmentItems => Set<CommitmentItem>();
    public DbSet<CommitmentRevision> CommitmentRevisions => Set<CommitmentRevision>();
    public DbSet<CommitmentWorkPackage> CommitmentWorkPackages => Set<CommitmentWorkPackage>();
    public DbSet<ControlAccount> ControlAccounts => Set<ControlAccount>();
    public DbSet<ControlAccountAssignment> ControlAccountAssignments => Set<ControlAccountAssignment>();
    public DbSet<ActualCost> ActualCosts => Set<ActualCost>();
    public DbSet<CostItem> CostItems => Set<CostItem>();
    public DbSet<AccountCode> AccountCodes => Set<AccountCode>();
    public DbSet<CBS> CBSElements => Set<CBS>();
    public DbSet<CostControlReport> CostControlReports => Set<CostControlReport>();
    public DbSet<CostControlReportItem> CostControlReportItems => Set<CostControlReportItem>();
    public DbSet<ExchangeRate> ExchangeRates => Set<ExchangeRate>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();
    public DbSet<TimePhasedBudget> TimePhasedBudgets => Set<TimePhasedBudget>();
    public DbSet<WBSCBSMapping> WBSCBSMappings => Set<WBSCBSMapping>();
    public DbSet<EVMRecord> EVMRecords => Set<EVMRecord>();

    #endregion

    #region DbSets - Documents Module
    
    public DbSet<CommentAttachment> CommentAttachments => Set<CommentAttachment>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<DocumentComment> DocumentComments => Set<DocumentComment>();
    public DbSet<DocumentDistribution> DocumentDistributions => Set<DocumentDistribution>();
    public DbSet<DocumentPermission> DocumentPermissions => Set<DocumentPermission>();
    public DbSet<DocumentRelationship> DocumentRelationships => Set<DocumentRelationship>();
    public DbSet<DocumentVersion> DocumentVersions => Set<DocumentVersion>();
    public DbSet<DocumentView> DocumentViews => Set<DocumentView>();
    public DbSet<DocumentDownload> DocumentDownloads => Set<DocumentDownload>();

    public DbSet<Transmittal> DocumentTypes => Set<Transmittal>(); 
    public DbSet<TransmittalAttachment> TransmittalAttachments => Set<TransmittalAttachment>();
    public DbSet<TransmittalDocument> TransmittalDocuments => Set<TransmittalDocument>();
    public DbSet<TransmittalRecipient> TransmittalRecipients => Set<TransmittalRecipient>();

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
        modelBuilder.Entity<UserSession>().ToTable("UserSessions", "Security");

        // Setup Module
        modelBuilder.Entity<Company>().ToTable("Companies", "Organization");
        modelBuilder.Entity<Contractor>().ToTable("Contractors", "Organization");
        modelBuilder.Entity<Currency>().ToTable("Currencies", "Organization");
        modelBuilder.Entity<Discipline>().ToTable("Disciplines", "Organization");
        modelBuilder.Entity<OBSNode>().ToTable("OBSNodes", "Organization");
        modelBuilder.Entity<Operation>().ToTable("Operations", "Organization");
        modelBuilder.Entity<Package>().ToTable("Packages", "Organization");
        modelBuilder.Entity<PackageDiscipline>().ToTable("PackageDisciplines", "Organization");
        modelBuilder.Entity<Phase>().ToTable("Phases", "Organization");
        modelBuilder.Entity<Project>().ToTable("Projects", "Organization");
        modelBuilder.Entity<RAM>().ToTable("RAMAssignments", "Organization");


        // UI Module
        modelBuilder.Entity<Notification>().ToTable("Notifications", "UI");
        modelBuilder.Entity<NotificationDelivery>().ToTable("NotificationDeliveries", "UI");
        modelBuilder.Entity<NotificationTemplate>().ToTable("NotificationTemplates", "UI");
        modelBuilder.Entity<NotificationPreference>().ToTable("NotificationPreferences", "UI");
        modelBuilder.Entity<NotificationSubscription>().ToTable("NotificationSubscriptions", "UI");

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
                entity.CreatedBy = _userContext?.CurrentUserId ?? "System";
            }

            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = _userContext?.CurrentUserId ?? "System";
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