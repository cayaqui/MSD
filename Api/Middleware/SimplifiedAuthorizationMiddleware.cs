using Application.Interfaces.Auth;
using Core.Constants;

namespace API.Middleware;

/// <summary>
/// Middleware for simplified authorization checks
/// Adds authorization context for Admin and Support users
/// </summary>
public class SimplifiedAuthorizationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SimplifiedAuthorizationMiddleware> _logger;

    public SimplifiedAuthorizationMiddleware(
        RequestDelegate next,
        ILogger<SimplifiedAuthorizationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ICurrentUserService currentUserService)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            // Check if user has system role
            if (await currentUserService.HasSystemAccessAsync())
            {
                // Add system access flag to context for use in authorization handlers
                context.Items["HasSystemAccess"] = true;
                context.Items["SystemRole"] = currentUserService.SystemRole;
                
                _logger.LogDebug("User {UserId} has system access with role {SystemRole}", 
                    currentUserService.UserId, currentUserService.SystemRole);
            }
            
            // For debugging - log user's authentication status
            _logger.LogDebug("User {UserId} authenticated: IsAdmin={IsAdmin}, IsSupport={IsSupport}", 
                currentUserService.UserId, 
                currentUserService.IsAdmin, 
                currentUserService.IsSupport);
        }

        await _next(context);
    }
}