using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Web;
using Web.Services;
using Web.Services.Implementation;
using Web.Services.Interfaces;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using MudBlazor.Services;

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
    
    var defaultScope = builder.Configuration["AzureAd:DefaultScopes:0"] ?? "api://25c8d8d0-8323-40fe-8863-e4b22711f572/access_as_user";
    options.ProviderOptions.DefaultAccessTokenScopes.Add(defaultScope);
    
    options.ProviderOptions.LoginMode = "redirect";
    options.ProviderOptions.Cache.CacheLocation = "localStorage";
    
    // Log MSAL configuration for debugging
    Console.WriteLine($"[MSAL Config] Authority: {options.ProviderOptions.Authentication.Authority}");
    Console.WriteLine($"[MSAL Config] ClientId: {options.ProviderOptions.Authentication.ClientId}");
    Console.WriteLine($"[MSAL Config] Default Scope: {defaultScope}");
});

// Registrar el servicio de autenticación simple
builder.Services.AddScoped<Web.Services.Authentication.ISimpleAuthService, Web.Services.Authentication.SimpleAuthService>();

// Registrar el servicio de sincronización de usuario
builder.Services.AddScoped<Web.Services.Implementation.IUserSyncService, Web.Services.Implementation.UserSyncService>();

// Configure Named HttpClient for API
builder.Services.AddHttpClient("EzProAPI", (sp, client) =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    client.BaseAddress = new Uri(configuration["ApiSettings:BaseUrl"] ?? "https://ez-capst-dev-api-eastus.azurewebsites.net/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
})
.AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

// Configure Authorization Message Handler
builder.Services.AddScoped(sp =>
{
    var handler = sp.GetRequiredService<BaseAddressAuthorizationMessageHandler>();
    var configuration = sp.GetRequiredService<IConfiguration>();
    var apiUrl = configuration["ApiSettings:BaseUrl"] ?? "https://ez-capst-dev-api-eastus.azurewebsites.net/";
    
    handler.ConfigureHandler(
        authorizedUrls: new[] { apiUrl },
        scopes: new[] { configuration["AzureAd:DefaultScopes:0"] ?? "api://25c8d8d0-8323-40fe-8863-e4b22711f572/access_as_user" }
    );
    
    return handler;
});

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
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<ICostService, CostService>();
builder.Services.AddScoped<IEVMService, EVMService>();

// Add Organization API Services
builder.Services.AddScoped<Web.Services.Interfaces.Organization.ICompanyApiService, Web.Services.Implementation.Organization.CompanyApiService>();
builder.Services.AddScoped<Web.Services.Interfaces.Organization.IOperationApiService, Web.Services.Implementation.Organization.OperationApiService>();
builder.Services.AddScoped<Web.Services.Interfaces.Organization.IContractorApiService, Web.Services.Implementation.Organization.ContractorApiService>();
builder.Services.AddScoped<Web.Services.Interfaces.Organization.IDisciplineApiService, Web.Services.Implementation.Organization.DisciplineApiService>();
builder.Services.AddScoped<Web.Services.Interfaces.Organization.IPhaseApiService, Web.Services.Implementation.Organization.PhaseApiService>();

// Configure Logging
builder.Logging.SetMinimumLevel(LogLevel.Information);

var app = builder.Build();

// Get logging service to log startup
var loggingService = app.Services.GetRequiredService<ILoggingService>();
loggingService.LogInfo("=== EzPro MSD Web Application Starting ===");
loggingService.LogInfo("API Base URL: {0}", builder.Configuration["ApiSettings:BaseUrl"] ?? "api://25c8d8d0-8323-40fe-8863-e4b22711f572/access_as_user");
loggingService.LogInfo("Azure AD Authority: {0}", builder.Configuration["AzureAd:Authority"] ?? "Not configured");

try
{
    loggingService.LogInfo("Starting Blazor WebAssembly app...");
    await app.RunAsync();
}
catch (Exception ex)
{
    loggingService.LogError(ex, "Failed to start application");
    throw;
}