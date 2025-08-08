using Application.Interfaces.Reports;
using Application.Interfaces.Auth;
using Application.Interfaces.Storage;
using Application.Interfaces.Common;
using Application.Interfaces.Notifications;
using Infrastructure.Data;
using Infrastructure.Services.Auth;
using Infrastructure.Services.Storage;
using Infrastructure.Services.Excel;
using Infrastructure.Services.Notifications;
using Microsoft.Extensions.Logging;
using SendGrid;
namespace Infrastructure;

/// <summary>
/// Dependency injection configuration for Infrastructure layer
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds Infrastructure services to the dependency injection container
    /// </summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add DbContext with SQL Server
        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorNumbersToAdd: null);
                });


            // Enable detailed errors in development
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (environment == "Development")
            {
                options.EnableDetailedErrors();
                options.EnableSensitiveDataLogging();
            }
        });


        // Register User Context for audit
        services.AddScoped<IUserContext, UserContext>();

        // Register Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register generic repository
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        // Add health checks
        services.AddHealthChecks().AddDbContextCheck<ApplicationDbContext>("Database");
        // Add logging
        services.AddLogging(config =>
        {
            config.AddConsole();
            config.AddDebug();
        });

        // Register Auth services
        services.AddScoped<IGraphApiService, GraphApiService>();
        
        // Register Report services
        //services.AddScoped<IReportExportService, ReportExportService>();
        
        // Register Storage services
        services.AddScoped<IBlobStorageService, AzureBlobStorageService>();
        
        // Register Excel services
        services.AddScoped<IExcelExportService, ExcelExportService>();
        services.AddScoped<IExcelImportService, ExcelImportService>();
        
        // Register Notification services
        services.AddScoped<IEmailService, EmailService>();
        
        // Register SendGrid client
        services.AddSingleton<ISendGridClient>(provider =>
        {
            var apiKey = configuration["Email:SendGridApiKey"];
            return new SendGridClient(apiKey);
        });

        return services;
    }

    
}