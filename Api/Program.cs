using API.Middleware;
using Application;
using Application.Interfaces.Auth;
using Application.Services.Auth;
using Carter;
using Domain.Interfaces;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Serilog;

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

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorApp", policy =>
    {
        policy.WithOrigins(builder.Configuration["ClientUrl"] ?? "https://localhost:5001")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Add services
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

// Add Carter
builder.Services.AddCarter();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "EzPro API",
        Version = "v1",
        Description = "Project Control System API for Engineering and Construction"
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
                    { $"api://{builder.Configuration["AzureAd:ClientId"]}/access_as_user", "Access API as user" }
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

// Configure CurrentUserService
builder.Services.AddScoped<ICurrentUserService>(provider =>
{
    var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
    var unitOfWork = provider.GetRequiredService<IUnitOfWork>();

    var userService = new CurrentUserService(unitOfWork);

    // This will be populated by the middleware
    return userService;
});

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

app.UseHttpsRedirection();
app.UseSerilogRequestLogging();
app.UseCors("BlazorApp");

app.UseAuthentication();
// Add custom middleware to populate CurrentUserService
app.UseMiddleware<CurrentUserMiddleware>();
app.UseAuthorization();
// Global exception handling
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Map Carter modules
app.MapCarter();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
   .AllowAnonymous();



Log.Information("Starting EzPro API");

app.Run();