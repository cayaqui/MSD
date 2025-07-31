using Microsoft.Extensions.Logging;
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

        return services;
    }

    
}