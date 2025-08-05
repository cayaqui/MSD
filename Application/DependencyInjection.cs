using Application.Services.Auth;
using FluentValidation;
using Application.Services.Common;
using Application.Interfaces.Auth;
using Application.Interfaces.Configuration;
using Application.Services.Configuration;
using Application.Interfaces.Organization;
using Application.Services.Organization;
using Application.Interfaces.Cost;
using Application.Services.Cost;
using Application.Interfaces.Contracts;
using Application.Services.Contracts;


namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Add AutoMapper
        services.AddAutoMapper(cfg => { cfg.AddMaps(assembly); }, assembly);

        //Add FluentValidation
        services.AddValidatorsWithLifetime(assembly, ServiceLifetime.Scoped);
        // Add Current User Context (Scoped per request)
        services.AddScoped<ICurrentUserContext, CurrentUserContext>();

        // Add CurrentUserService - Must be before other services that depend on it
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // Add Services - Scoped lifetime (one instance per request)
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICompanyService, CompanyService>();
        services.AddScoped<IOperationService, OperationService>();
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<ICurrencyService, CurrencyService>();
        services.AddScoped<IContractorService, ContractorService>();
        services.AddScoped<IPackageService, PackageService>();
        services.AddScoped<IPhaseService, PhaseService>();
        services.AddScoped<IOBSNodeService, OBSNodeService>();
        services.AddScoped<IDisciplineService, DisciplineService>();
        services.AddScoped<IPackageDisciplineService, PackageDisciplineService>();
        services.AddScoped<IRAMService, RAMService>();
        
        // Cost Services
        services.AddScoped<IBudgetService, BudgetService>();
        services.AddScoped<IControlAccountService, ControlAccountService>();
        services.AddScoped<ICommitmentService, CommitmentService>();
        services.AddScoped<ICostService, CostService>();
        services.AddScoped<IEVMService, EVMService>();
        
        //services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IProjectTeamMemberService, ProjectTeamMemberService>();
        //services.AddScoped<ICommitmentService, CommitmentService>();
        //services.AddScoped<IRiskService, RiskService>();
        
        // Report Services
        //services.AddScoped<IReportService, ReportService>();
        //services.AddScoped<IReportTemplateService, ReportTemplateService>();
        //services.AddScoped<IReportScheduleService, ReportScheduleService>();
        // Note: IReportExportService is registered in Infrastructure layer
        
        // Document Services
        //services.AddScoped<IDocumentService, DocumentService>();
        //services.AddScoped<ITransmittalService, TransmittalService>();
        //services.AddScoped<IDocumentDistributionService, DocumentDistributionService>();
        
        // Configuration Services
        services.AddScoped<IProjectTypeService, ProjectTypeService>();
        //services.AddScoped<ISystemParameterService, SystemParameterService>();
        //services.AddScoped<IOBSService, OBSService>();
        services.AddScoped<IPermissionService, PermissionService>();
        //services.AddScoped<IWBSTemplateService, WBSTemplateService>();
        //services.AddScoped<ICBSTemplateService, CBSTemplateService>();
        
        // Organization Services
        // TODO: Fix these services to match actual domain entity properties
        // services.AddScoped<IContractorService, ContractorService>();
        // services.AddScoped<IDisciplineService, DisciplineService>();
        // services.AddScoped<ICurrencyService, CurrencyService>();
        // services.AddScoped<IPhaseService, PhaseService>();
        // services.AddScoped<IPackageService, PackageService>();
        // services.AddScoped<IPackageDisciplineService, PackageDisciplineService>();
        // services.AddScoped<IOBSNodeService, OBSNodeService>();
        // services.AddScoped<IRAMService, RAMService>();
        
        // Contract Services
        services.AddScoped<IContractService, ContractService>();
        //services.AddScoped<IChangeOrderService, ChangeOrderService>();
        //services.AddScoped<IContractMilestoneService, ContractMilestoneService>();
        //services.AddScoped<IValuationService, ValuationService>();
        //services.AddScoped<IClaimService, ClaimService>();

        // Add DateTime service - Transient (new instance each time)
        services.AddTransient<IDateTime, DateTimeService>();

        // Add Hosted Services (if needed)
        // services.AddHostedService<NotificationCleanupService>();

        // Add HttpContextAccessor (if needed in services)
        services.AddHttpContextAccessor();

        // Configure Service Options
        services.Configure<ApplicationOptions>(options =>
        {
            options.DefaultPageSize = 20;
            options.MaxPageSize = 100;
            options.EnableDetailedErrors = false;
        });

        return services;
    }
}
/// <summary>
/// Application configuration options
/// </summary>
public class ApplicationOptions
{
    public int DefaultPageSize { get; set; } = 20;
    public int MaxPageSize { get; set; } = 100;
    public bool EnableDetailedErrors { get; set; } = false;
    public int CacheExpirationMinutes { get; set; } = 60;
    public bool EnableNotificationEmail { get; set; } = true;
}

/// <summary>
/// Extension methods for IServiceCollection
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add a scoped service with its interface
    /// </summary>
    public static IServiceCollection AddScopedService<TInterface, TImplementation>(
        this IServiceCollection services)
        where TInterface : class
        where TImplementation : class, TInterface
    {
        return services.AddScoped<TInterface, TImplementation>();
    }

    /// <summary>
    /// Add multiple scoped services
    /// </summary>
    public static IServiceCollection AddScopedServices(
        this IServiceCollection services,
        params (Type serviceType, Type implementationType)[] serviceTypes)
    {
        foreach (var (serviceType, implementationType) in serviceTypes)
        {
            services.AddScoped(serviceType, implementationType);
        }
        return services;
    }
}

/// <summary>
/// Validation extensions
/// </summary>
public static class ValidationExtensions
{
    /// <summary>
    /// Add validators with custom lifetime
    /// </summary>
    public static IServiceCollection AddValidatorsWithLifetime(
        this IServiceCollection services,
        Assembly assembly,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        var validatorTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericTypeDefinition)
            .Where(t => t.GetInterfaces().Any(i =>
                i.IsGenericType &&
                i.GetGenericTypeDefinition() == typeof(IValidator<>)))
            .ToList();

        foreach (var validatorType in validatorTypes)
        {
            var validatorInterface = validatorType.GetInterfaces()
                .First(i => i.IsGenericType &&
                           i.GetGenericTypeDefinition() == typeof(IValidator<>));

            services.Add(new ServiceDescriptor(validatorInterface, validatorType, lifetime));
        }

        return services;
    }
}

