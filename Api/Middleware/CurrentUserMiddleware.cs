using Application.Interfaces.Auth;
using Application.Services.Auth;
using System.Security.Claims;

namespace API.Middleware;

public class CurrentUserMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CurrentUserMiddleware> _logger;

    public CurrentUserMiddleware(RequestDelegate next, ILogger<CurrentUserMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ICurrentUserService currentUserService)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userService = currentUserService as CurrentUserService;
            if (userService != null)
            {
                // Extract user information from claims
                userService.UserId = context.User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value
                    ?? context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                userService.UserName = context.User.FindFirst("name")?.Value
                    ?? context.User.FindFirst(ClaimTypes.Name)?.Value;

                userService.Email = context.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value
                    ?? context.User.FindFirst(ClaimTypes.Email)?.Value;

                userService.IsAuthenticated = true;
                userService.Principal = context.User;

                _logger.LogDebug("User context set: {UserId} - {UserName}",
                    userService.UserId, userService.UserName);
            }
        }

        await _next(context);
    }
}