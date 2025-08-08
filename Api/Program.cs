using API.Middleware;
using Application;
using Application.Services.Auth;
using Carter;
using Domain.Interfaces;
using Infrastructure;
using Infrastructure.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;
using Serilog;
using Application.Interfaces.Auth;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/ezpro-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add Authentication with Azure AD
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(options =>
    {
        builder.Configuration.Bind("AzureAd", options);
        options.TokenValidationParameters.NameClaimType = "name";
    }, options =>
    {
        builder.Configuration.Bind("AzureAd", options);
    });

// Add Authorization
builder.Services.AddAuthorization();

// Add custom authorization handler for system admins
builder.Services.AddScoped<IAuthorizationHandler, Api.Authorization.SystemAdminOrRoleHandler>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorApp", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            // En desarrollo, permitir cualquier origen
            policy.SetIsOriginAllowed(origin => true)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        }
        else
        {
            // En producción, usar los orígenes configurados
            var allowedOrigins = builder.Configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>() 
                                ?? new[] { "https://localhost:5001" };
            
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        }
    });
});

// Add services
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

// Add Carter - after Application to ensure our validator registrations take precedence
builder.Services.AddCarter(configurator: c =>
{
    // Disable automatic validator registration from Carter
    c.WithEmptyValidators();
});

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "API de EzPro",
        Version = "v1",
        Description = "API del Sistema de Control de Proyectos para Ingeniería y Construcción"
    });
    c.AddSecurityDefinition("oauth2", new()
    {
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.OAuth2,
        Flows = new()
        {
            Implicit = new()
            {
                AuthorizationUrl = new Uri($"https://login.microsoftonline.com/{builder.Configuration["AzureAd:TenantId"]}/oauth2/v2.0/authorize"),
                TokenUrl = new Uri($"https://login.microsoftonline.com/{builder.Configuration["AzureAd:TenantId"]}/oauth2/v2.0/token"),
                Scopes = new Dictionary<string, string>
                {
                    { $"api://{builder.Configuration["AzureAd:ClientId"]}/access_as_user", "Acceder a la API como usuario" }
                }
            }
        }
    });
    c.AddSecurityRequirement(new()
    {
        {
            new()
            {
                Reference = new()
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "oauth2"
                }
            },
            new[] { $"api://{builder.Configuration["AzureAd:ClientId"]}/access_as_user" }
        }
    });
});

// Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "EzPro API v1");
        c.OAuthClientId(builder.Configuration["AzureAd:ClientId"]);
        c.OAuthUsePkce();
    });
}

// Global exception handling - MUST be first to catch all errors
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

// CORS must be early in the pipeline, before authentication
app.UseCors("BlazorApp");

app.UseSerilogRequestLogging();

app.UseAuthentication();
// Add custom middleware to populate CurrentUserService
app.UseMiddleware<CurrentUserMiddleware>();
// Validate that authenticated users exist in the database
app.UseUserValidation();
app.UseAuthorization();

// Map Carter modules
app.MapCarter();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
   .AllowAnonymous();

// Diagnostic endpoint
app.MapGet("/api/diagnostics", (IServiceProvider services, IConfiguration config) => 
{
    Log.Information("Diagnostics endpoint called");
    
    var diagnostics = new
    {
        timestamp = DateTime.UtcNow,
        environment = app.Environment.EnvironmentName,
        authentication = new
        {
            scheme = config["AzureAd:Instance"] != null ? "AzureAD" : "None",
            tenantId = config["AzureAd:TenantId"],
            clientId = config["AzureAd:ClientId"],
            hasClientSecret = !string.IsNullOrEmpty(config["AzureAd:ClientSecret"])
        },
        services = new
        {
            hasDbContext = services.GetService<Infrastructure.Data.ApplicationDbContext>() != null,
            hasUnitOfWork = services.GetService<IUnitOfWork>() != null,
            hasUserService = services.GetService<IUserService>() != null,
            hasAuthService = services.GetService<IAuthService>() != null,
            hasCurrentUserService = services.GetService<ICurrentUserService>() != null,
            hasHttpContextAccessor = services.GetService<IHttpContextAccessor>() != null
        },
        cors = new
        {
            policy = "BlazorApp",
            allowedOrigins = config.GetSection("CorsSettings:AllowedOrigins").Get<string[]>()
        }
    };
    
    return Results.Ok(diagnostics);
})
.AllowAnonymous()
.WithName("Diagnostics");

// Test endpoint to verify API is responding
app.MapGet("/api/test", () => 
{
    Log.Information("Test endpoint called");
    return Results.Ok(new { 
        message = "API is working", 
        timestamp = DateTime.UtcNow,
        environment = app.Environment.EnvironmentName 
    });
})
.AllowAnonymous()
.WithName("TestEndpoint");

// Handle OPTIONS for test endpoint
app.MapMethods("/api/test", new[] { "OPTIONS" }, () => Results.Ok())
   .AllowAnonymous();

// Test authenticated endpoint
app.MapGet("/api/test/auth", (HttpContext context) => 
{
    Log.Information("Authenticated test endpoint called by {User}", context.User.Identity?.Name ?? "Anonymous");
    return Results.Ok(new { 
        message = "Authenticated endpoint working",
        user = context.User.Identity?.Name,
        isAuthenticated = context.User.Identity?.IsAuthenticated,
        claims = context.User.Claims.Select(c => new { c.Type, c.Value })
    });
})
.RequireAuthorization()
.WithName("TestAuthEndpoint");

Log.Information("Starting EzPro API");

app.Run();