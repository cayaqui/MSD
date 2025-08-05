using Domain.Entities.Organization.Core;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;

public static class DatabaseExtensions
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

        try
        {
            logger.LogInformation("Starting database initialization...");

            // Apply migrations
            if ((await dbContext.Database.GetPendingMigrationsAsync()).Any())
            {
                logger.LogInformation("Applying pending migrations...");
                await dbContext.Database.MigrateAsync();
                logger.LogInformation("Migrations applied successfully");
            }

            // Seed data if needed
            await SeedDataAsync(dbContext, logger);

            logger.LogInformation("Database initialization completed");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing the database");
            throw;
        }
    }

    private static async Task SeedDataAsync(ApplicationDbContext context, ILogger logger)
    {
        // Check if we already have data
        if (await context.Companies.AnyAsync())
        {
            logger.LogInformation("Database already contains data, skipping seed");
            return;
        }

        logger.LogInformation("Seeding initial data...");

        // Add default company
        var defaultCompany = new Company("DEFAULT", "Default Company", "00000000-0")
        {
            Description = "Default company for initial setup",
        };
        defaultCompany.SetCurrency("USD");
        context.Companies.Add(defaultCompany);

        // Add default disciplines based on the Cuentas Control document
        var disciplines = new[]
        {
            new { Code = "ENG", Name = "Engineering", Color = "#0066CC", IsEngineering = true, Order = 1 },
            new { Code = "PRO", Name = "Procurement", Color = "#FF9900", IsEngineering = false, Order = 2 },
            new { Code = "CON", Name = "Construction", Color = "#FF3333", IsEngineering = false, Order = 3 },
            new { Code = "COM", Name = "Commissioning", Color = "#009900", IsEngineering = false, Order = 4 },
            new { Code = "PM", Name = "Project Management", Color = "#663399", IsEngineering = false, Order = 5 },
            new { Code = "QC", Name = "Quality Control", Color = "#FF6666", IsEngineering = false, Order = 6 },
            new { Code = "HSE", Name = "Health, Safety & Environment", Color = "#FFCC00", IsEngineering = false, Order = 7 }
        };

        foreach (var disc in disciplines)
        {
            var discipline = new Discipline(disc.Code, disc.Name, disc.Color, disc.Order, disc.IsEngineering)
            {
                Description = $"{disc.Name} discipline",
            };
           // context.Disciplines.Add(discipline);
        }

        await context.SaveChangesAsync();
        logger.LogInformation("Initial data seeded successfully");
    }
}