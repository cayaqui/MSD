using Carter;

namespace Api.Modules;

public class TestModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var test = app.MapGroup("/api/debug")
            .WithTags("Depuración y Pruebas");

        // Simple test - no dependencies
        test.MapGet("/ping", () => 
        {
            return Results.Ok(new { message = "pong", timestamp = DateTime.UtcNow });
        })
        .AllowAnonymous()
        .WithName("Ping")
        .WithSummary("Verificar conectividad del API");

        // Test with logger
        test.MapGet("/log", (ILogger<TestModule> logger) => 
        {
            logger.LogInformation("Endpoint de prueba de log llamado a las {Time}", DateTime.UtcNow);
            return Results.Ok(new { message = "logged", logged = true });
        })
        .AllowAnonymous()
        .WithName("TestLog")
        .WithSummary("Probar funcionalidad de logging");

        // Test with DI
        test.MapGet("/services", (IServiceProvider services) => 
        {
            var registeredServices = new
            {
                hasUserService = services.GetService(typeof(Application.Interfaces.Auth.IUserService)) != null,
                hasAuthService = services.GetService(typeof(Application.Interfaces.Auth.IAuthService)) != null,
                hasCurrentUserService = services.GetService(typeof(Application.Interfaces.Auth.ICurrentUserService)) != null,
                hasUnitOfWork = services.GetService(typeof(Domain.Interfaces.IUnitOfWork)) != null
            };
            return Results.Ok(registeredServices);
        })
        .AllowAnonymous()
        .WithName("TestServices")
        .WithSummary("Verificar servicios registrados en DI");

        // Test middleware order
        test.MapGet("/middleware", (HttpContext context) => 
        {
            var info = new
            {
                isAuthenticated = context.User.Identity?.IsAuthenticated,
                userName = context.User.Identity?.Name,
                hasUserId = context.Items.ContainsKey("UserId"),
                userId = context.Items.ContainsKey("UserId") ? context.Items["UserId"] : null,
                path = context.Request.Path.Value,
                method = context.Request.Method
            };
            return Results.Ok(info);
        })
        .RequireAuthorization()
        .WithName("TestMiddleware")
        .WithSummary("Verificar orden y funcionamiento del middleware");
    }
}