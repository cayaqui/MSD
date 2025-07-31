using Web.Services.Implementation;
using Web.Services.Interfaces;
using Web.State;

namespace Web.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registra todos los servicios de infraestructura y comunicación
        /// </summary>
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            // Servicios HTTP y API
            services.AddScoped<IApiService, ApiService>();
            services.AddScoped<IHttpInterceptor, HttpInterceptor>();
            services.AddScoped<HttpInterceptorHandler>();
            services.AddScoped<AuthorizationMessageHandler>();

            // Servicios de autenticación y autorización
            services.AddScoped<IAuthService, AuthService>();

            // Servicios de manejo de errores
            services.AddScoped<IEzErrorHandlingService, EzErrorHandlingService>();

            // Cache
            services.AddSingleton<ICacheService, CacheService>();

            return services;
        }
        /// <summary>
        /// Registra servicios de UI y navegación
        /// </summary>
        public static IServiceCollection AddUIServices(this IServiceCollection services)
        {
            // Navegación
            services.AddScoped<INavigationService, NavigationService>();
            services.AddScoped<IBreadcrumbService, BreadcrumbService>();

            // Notificaciones y feedback
            services.AddScoped<IToastService, ToastService>();

            // Estado de carga
            services.AddScoped<ILoadingService, LoadingService>();
            services.AddScoped<ISearchService, SearchService>();
            return services;
        }

        /// <summary>
        /// Registra servicios de estado de la aplicación
        /// </summary>
        public static IServiceCollection AddApplicationState(this IServiceCollection services)
        {
            // Estado global
            services.AddScoped<IAppStateService, AppStateService>();

            // Estados específicos
            services.AddScoped<AppState>();
            services.AddScoped<ProjectState>();
            services.AddScoped<UserState>();
            services.AddScoped<NotificationState>();

            return services;
        }

        /// <summary>
        /// Configura HttpClient y servicios HTTP
        /// </summary>
        public static IServiceCollection AddHttpServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Configurar HttpClient para el API
            var apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7001";

            services.AddHttpClient("EzProAPI", client =>
            {
                client.BaseAddress = new Uri(apiBaseUrl);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.Timeout = TimeSpan.FromSeconds(configuration.GetValue<int>("ApiSettings:Timeout", 30));
            })
            .AddHttpMessageHandler<HttpInterceptorHandler>();

            return services;
        }

        /// <summary>
        /// Registra todos los servicios de negocio
        /// </summary>
        public static IServiceCollection AddBusinessServices(this IServiceCollection services)
        {
            // Configuración y administración
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IUserService, UserService>();
            //services.AddScoped<IOperationService, OperationService>();
            //services.AddScoped<IPermissionService, PermissionService>();
            //services.AddScoped<ISystemConfigService, SystemConfigService>();

            // Proyectos y estructura
            services.AddScoped<IProjectService, ProjectService>();
            //services.AddScoped<IProjectTypeService, ProjectTypeService>();
            //services.AddScoped<IWBSService, WBSService>();

            // Gestión de alcance
            //services.AddScoped<IScopeService, ScopeService>();
            //services.AddScoped<IDeliverableService, DeliverableService>();
            //services.AddScoped<IChangeRequestService, ChangeRequestService>();

            // Gestión de cronograma
            //services.AddScoped<IScheduleService, ScheduleService>();
            //services.AddScoped<IActivityService, ActivityService>();
            //services.AddScoped<IMilestoneService, MilestoneService>();
            //services.AddScoped<ICriticalPathService, CriticalPathService>();

            // Gestión de costos y EVM
            services.AddScoped<ICostService, CostService>();
            //services.AddScoped<IBudgetService, BudgetService>();
            //services.AddScoped<IEVMService, EVMService>();
            //services.AddScoped<ICashFlowService, CashFlowService>();
            //services.AddScoped<IControlAccountService, ControlAccountService>();

            // Gestión de contratos
            //services.AddScoped<IContractService, ContractService>();
            //services.AddScoped<IContractorService, ContractorService>();
            //services.AddScoped<IValuationService, ValuationService>();
            //services.AddScoped<IChangeOrderService, ChangeOrderService>();

            // Gestión de riesgos
            //services.AddScoped<IRiskService, RiskService>();
            //services.AddScoped<IRiskCategoryService, RiskCategoryService>();
            //services.AddScoped<IRiskResponseService, RiskResponseService>();

            // Gestión de calidad
            //services.AddScoped<IQualityService, QualityService>();
            //services.AddScoped<IChecklistService, ChecklistService>();
            //services.AddScoped<INonConformityService, NonConformityService>();

            // Gestión documental
            services.AddScoped<IFileService, FileService>();
            //services.AddScoped<IDocumentService, DocumentService>();
            //services.AddScoped<ITransmittalService, TransmittalService>();
            //services.AddScoped<ITemplateService, TemplateService>();
            //services.AddScoped<IVersionControlService, VersionControlService>();

            // Reportes y análisis
            //services.AddScoped<IReportService, ReportService>();
            //services.AddScoped<IKPIService, KPIService>();
            //services.AddScoped<IDashboardService, DashboardService>();
            //services.AddScoped<IAnalyticsService, AnalyticsService>();

            return services;
        }

        /// <summary>
        /// Registra servicios de utilidades
        /// </summary>
        public static IServiceCollection AddUtilityServices(this IServiceCollection services)
        {
            // Fecha y hora
            services.AddScoped<IDateTimeService, DateTimeService>();

            // Exportación e importación
            //services.AddScoped<IExportService, ExportService>();
            //services.AddScoped<IImportService, ImportService>();

            // Localización y formato
            //services.AddScoped<ICurrencyService, CurrencyService>();
            //services.AddScoped<ILocalizationService, LocalizationService>();

            return services;
        }

        /// <summary>
        /// Método de conveniencia para registrar todos los servicios
        /// </summary>
        public static IServiceCollection AddEzProServices(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddHttpServices(configuration)
                .AddInfrastructureServices()
                .AddUIServices()
                .AddApplicationState()
                .AddBusinessServices()
                .AddUtilityServices();

            return services;
        }
    
        
    }
}