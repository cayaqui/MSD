using Application.Interfaces.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Infrastructure.Middleware
{
    public class UserValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<UserValidationMiddleware> _logger;

        public UserValidationMiddleware(RequestDelegate next, ILogger<UserValidationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IUserService userService)
        {
            var path = context.Request.Path.Value?.ToLower() ?? "";
            _logger.LogDebug("UserValidationMiddleware processing request to {Path}", path);

            // Skip validation for non-authenticated requests
            if (!context.User.Identity?.IsAuthenticated ?? true)
            {
                _logger.LogDebug("Skipping validation - user not authenticated");
                await _next(context);
                return;
            }

            // Skip validation for specific endpoints (e.g., health checks, swagger, test)
            if (path.Contains("/health") || path.Contains("/swagger") || path.Contains("/_framework") || path.Contains("/api/test"))
            {
                _logger.LogDebug("Skipping validation for path: {Path}", path);
                await _next(context);
                return;
            }

            try
            {
                // Get the user's Azure AD object ID (EntraId)
                var entraId = context.User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value
                    ?? context.User.FindFirst("oid")?.Value
                    ?? context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(entraId))
                {
                    _logger.LogWarning("Authenticated user has no EntraId/ObjectId claim");
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Invalid user identity");
                    return;
                }

                // Check if user exists in database
                _logger.LogDebug("Looking up user with EntraId: {EntraId}", entraId);
                var user = await userService.GetByEntraIdAsync(entraId);
                
                if (user == null)
                {
                    _logger.LogWarning("User with EntraId {EntraId} not found in database", entraId);
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new 
                    {
                        error = "User not registered",
                        message = "User not registered in the system. Please contact your administrator.",
                        entraId = entraId
                    }));
                    return;
                }

                // Check if user is active
                if (!user.IsActive)
                {
                    _logger.LogWarning("Inactive user {UserId} attempted to access the system", user.Id);
                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsync("User account is inactive. Please contact your administrator.");
                    return;
                }

                // Add user ID to HttpContext for easy access
                context.Items["UserId"] = user.Id;
                context.Items["User"] = user;

                _logger.LogDebug("User validation successful for {UserId}, proceeding to next middleware", user.Id);
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating user");
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new 
                {
                    error = "Validation error",
                    message = "An error occurred during user validation",
                    detail = ex.Message
                }));
            }
        }
    }
}