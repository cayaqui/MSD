using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Web;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using MudBlazor.Services;
using Web.Services.Interfaces;
using Web.Services.Interfaces.Auth;
using Web.Services.Interfaces.Cost;
using Web.Services.Interfaces.Organization;
using Web.Services.Implementation;
using Web.Services.Implementation.Auth;
using Web.Services.Implementation.Cost;
using Web.Services.Implementation.Organization;

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
        scopes: new[] { "api://25c8d8d0-8323-40fe-8863-e4b22711f572/access_as_user" }
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
// builder.Services.AddScoped<IUserService, UserService>(); // TODO: Create IUserService interface
builder.Services.AddScoped<ICostService, CostService>();
builder.Services.AddScoped<IEVMService, EVMService>();

// Add Organization API Services
builder.Services.AddScoped<ICompanyApiService, CompanyApiService>();
builder.Services.AddScoped<IOperationApiService, OperationApiService>();
builder.Services.AddScoped<IContractorApiService, ContractorApiService>();
builder.Services.AddScoped<IDisciplineApiService, DisciplineApiService>();
builder.Services.AddScoped<IProjectApiService, ProjectApiService>();
builder.Services.AddScoped<IPhaseApiService, PhaseApiService>();
builder.Services.AddScoped<IOBSNodeApiService, OBSNodeApiService>();
builder.Services.AddScoped<IRAMApiService, RAMApiService>();

// Add wrapper services
builder.Services.AddScoped<IProjectService, ProjectService>();

// Add user photo service
builder.Services.AddScoped<UserPhotoService>();

// Configure Logging
builder.Logging.SetMinimumLevel(LogLevel.Information);

var app = builder.Build();

await app.RunAsync();