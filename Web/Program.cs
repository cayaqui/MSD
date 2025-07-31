using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Web;
using Web.Extensions;
using Web.Services;
using Web.Services.Http;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configuración de autenticación con Azure AD / Entra ID
builder.Services.AddMsalAuthentication(options =>
{
    builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
    options.ProviderOptions.DefaultAccessTokenScopes.Add("api://25c8d8d0-8323-40fe-8863-e4b22711f572/access_as_user");
    options.ProviderOptions.LoginMode = "redirect";

    // Configurar opciones adicionales
    options.ProviderOptions.Cache.CacheLocation = "localStorage";
    options.ProviderOptions.Cache.StoreAuthStateInCookie = true;
});

// Agregar todos los servicios de EzPro
builder.Services.AddEzProServices(builder.Configuration);

// Agregar Blazored LocalStorage
builder.Services.AddBlazoredLocalStorage();

// Configurar autorización
builder.Services.AddAuthorizationCore(options =>
{
    // Políticas de autorización basadas en roles
    options.AddPolicy("RequireAdminRole", policy =>
        policy.RequireRole("Admin", "System.Admin"));

    options.AddPolicy("RequireProjectManagerRole", policy =>
        policy.RequireRole("ProjectManager", "Admin", "System.Admin"));

    options.AddPolicy("RequireTeamMemberRole", policy =>
        policy.RequireRole("TeamMember", "ProjectManager", "Admin", "System.Admin"));

    // Políticas basadas en claims
    options.AddPolicy("RequireAuthenticatedUser", policy =>
        policy.RequireAuthenticatedUser());
});

// Reemplazar el proveedor de políticas por defecto con uno que soporte políticas dinámicas
builder.Services.AddSingleton<IAuthorizationPolicyProvider, DynamicAuthorizationPolicyProvider>();

// Registrar el handler de autorización para permisos
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

// Configurar logging
builder.Logging.SetMinimumLevel(LogLevel.Information);
builder.Services.AddLogging();

await builder.Build().RunAsync();