using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Web;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using MudBlazor.Services;
using Web.Services.Interfaces;
using Web.Services.Interfaces.Auth;
using Web.Services.Interfaces.Cost;
using Web.Services.Interfaces.Organization;
using Web.Services.Interfaces.Configuration;
using Web.Services.Interfaces.Projects;
using Web.Services.Interfaces.Progress;
using Web.Services.Interfaces.Documents;
using Web.Services.Interfaces.Visualization;
using Web.Services.Implementation;
using Web.Services.Implementation.Auth;
using Web.Services.Implementation.Cost;
using Web.Services.Implementation.Organization;
using Web.Services.Implementation.Configuration;
using Web.Services.Implementation.Projects;
using Web.Services.Implementation.Progress;
using Web.Services.Implementation.Documents;
using Web.Services.Implementation.Visualization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HttpClient
builder.Services.AddScoped(sp =>
{
    var client = new HttpClient
    {
        BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7001")
    };
    return client;
});

// Add MSAL Authentication
builder.Services.AddMsalAuthentication(options =>
{
    builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
    options.ProviderOptions.DefaultAccessTokenScopes.Add("api://25c8d8d0-8323-40fe-8863-e4b22711f572/access_as_user");
    options.ProviderOptions.LoginMode = "redirect";
});

// Registrar el servicio de autenticación simple
builder.Services.AddScoped<Web.Services.Authentication.ISimpleAuthService, Web.Services.Authentication.SimpleAuthService>();

// Registrar el servicio de sincronización de usuario
builder.Services.AddScoped<Web.Services.Implementation.IUserSyncService, Web.Services.Implementation.UserSyncService>();

// Configure HttpClient for testing (without auth)
builder.Services.AddHttpClient("TestAPI", (sp, client) =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var baseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7193/";
    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Configure Authorization Message Handler
builder.Services.AddScoped<Web.Services.CustomAuthorizationMessageHandler>();

// Configure Named HttpClient for API
builder.Services.AddHttpClient("EzProAPI", (sp, client) =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var baseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7193/";
    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(configuration.GetValue<int>("ApiSettings:Timeout", 30));
})
.AddHttpMessageHandler<Web.Services.CustomAuthorizationMessageHandler>();

// Add MudBlazor Services
builder.Services.AddMudServices();

// Add Storage Services
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredSessionStorage();

// Add Logging Service (before other services)
builder.Services.AddScoped<ILoggingService, LoggingService>();

// Add Application Services
builder.Services.AddScoped<IApiService, ApiService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<INavigationService, NavigationService>();
builder.Services.AddScoped<IToastService, ToastService>();
builder.Services.AddScoped<ILoadingService, LoadingService>();
builder.Services.AddScoped<IStateService, StateService>();

// Add API Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICostService, CostService>();
builder.Services.AddScoped<IEVMService, EVMService>();

// Add Cost API Services
builder.Services.AddScoped<IBudgetApiService, BudgetApiService>();
builder.Services.AddScoped<ICommitmentApiService, CommitmentApiService>();
builder.Services.AddScoped<IControlAccountApiService, ControlAccountApiService>();
builder.Services.AddScoped<Cost.IWorkPackageApiService, Cost.WorkPackageApiService>();
builder.Services.AddScoped<ICostApiService, CostApiService>();
builder.Services.AddScoped<IEVMApiService, EVMApiService>();

// Add Auth API Services
builder.Services.AddScoped<IProjectTeamMemberApiService, ProjectTeamMemberApiService>();
builder.Services.AddScoped<IUserApiService, UserApiService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Add Organization API Services
builder.Services.AddScoped<ICompanyApiService, CompanyApiService>();
builder.Services.AddScoped<IOperationApiService, OperationApiService>();
builder.Services.AddScoped<IContractorApiService, ContractorApiService>();
builder.Services.AddScoped<IDisciplineApiService, DisciplineApiService>();
builder.Services.AddScoped<IProjectApiService, ProjectApiService>();
builder.Services.AddScoped<IPhaseApiService, PhaseApiService>();
builder.Services.AddScoped<IOBSNodeApiService, OBSNodeApiService>();
builder.Services.AddScoped<IRAMApiService, RAMApiService>();

// Add Configuration API Services
builder.Services.AddScoped<IWBSTemplateApiService, WBSTemplateApiService>();

// Add Projects API Services
builder.Services.AddScoped<IWBSApiService, WBSApiService>();
builder.Services.AddScoped<Projects.IWorkPackageApiService, Projects.WorkPackageApiService>();
builder.Services.AddScoped<IPlanningPackageApiService, PlanningPackageApiService>();

// Add Progress API Services
builder.Services.AddScoped<IScheduleApiService, ScheduleApiService>();
builder.Services.AddScoped<IActivityApiService, ActivityApiService>();
builder.Services.AddScoped<IMilestoneApiService, MilestoneApiService>();

// Add Document API Services
builder.Services.AddScoped<IDocumentApiService, DocumentApiService>();

// Add Visualization API Services
builder.Services.AddScoped<IVisualizationApiService, VisualizationApiService>();

// Add wrapper services
builder.Services.AddScoped<IProjectService, ProjectService>();

// Add user photo service
builder.Services.AddScoped<UserPhotoService>();

// Configure Logging
builder.Logging.SetMinimumLevel(LogLevel.Information);

var app = builder.Build();

await app.RunAsync();