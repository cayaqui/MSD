using Carter;
using Carter.ModelBinding;
using Microsoft.AspNetCore.Http.HttpResults;
using Core.DTOs.Auth;
using Domain.Entities.Security;
using Application.Interfaces.Auth;

namespace API.Modules;

public class AuthModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Authentication")
            .RequireAuthorization();

        // Get current user info (with auto-sync from Azure AD)
        group.MapGet("/me", GetCurrentUser)
            .WithName("GetCurrentUser")
            .WithSummary("Get current authenticated user information with Azure AD sync")
            .Produces<UserDto>(200)
            .Produces(401);

        // Get all user permissions
        group.MapGet("/permissions", GetUserPermissions)
            .WithName("GetUserPermissions")
            .WithSummary("Get all permissions for the current user")
            .Produces<UserPermissionsDto>(200)
            .Produces(401);

        // Check specific permission (existing)
        group.MapPost("/check-permission", CheckPermission)
            .WithName("CheckPermission")
            .WithSummary("Check if current user has specific role in project")
            .Produces<bool>(200)
            .Produces(400)
            .Produces(401);

        // Refresh user data from Azure AD
        group.MapPost("/refresh", RefreshUserData)
            .WithName("RefreshUserData")
            .WithSummary("Force refresh user data from Azure AD")
            .Produces<UserDto>(200)
            .Produces(401);

        // Health check endpoint (anonymous)
        app.MapGet("/api/auth/health", () => Results.Ok(new { status = "healthy" }))
            .WithTags("Health")
            .AllowAnonymous();
    }

    private static async Task<Results<Ok<UserDto>, UnauthorizedHttpResult>> GetCurrentUser(
        IAuthService authService,
        ICurrentUserService currentUserService,
        ILogger<AuthModule> logger)
    {
        try
        {
            // Get current user with Azure AD sync if needed
            var user = await authService.GetCurrentUserAsync();

            if (user == null)
            {
                logger.LogWarning("Current user not found for {UserId}", currentUserService.UserId);
                return TypedResults.Unauthorized();
            }

            // Check if user data needs updating from Azure AD
            if (ShouldSyncWithAzureAd(user))
            {
                logger.LogInformation("User {UserId} data is stale, syncing with Azure AD", user.Id);
                user = await authService.SyncCurrentUserWithAzureAsync();
            }

            return TypedResults.Ok(user);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting current user");
            throw;
        }
    }

    private static async Task<Results<Ok<UserPermissionsDto>, UnauthorizedHttpResult>> GetUserPermissions(
        IAuthService authService,
        ICurrentUserService currentUserService,
        ILogger<AuthModule> logger)
    {
        try
        {
            if (!currentUserService.IsAuthenticated)
            {
                return TypedResults.Unauthorized();
            }

            var permissions = await authService.GetUserPermissionsAsync();

            if (permissions == null)
            {
                logger.LogWarning("Could not retrieve permissions for user {UserId}", currentUserService.UserId);
                return TypedResults.Unauthorized();
            }

            logger.LogInformation(
                "Retrieved permissions for user {UserId}: {GlobalCount} global, {ProjectCount} projects",
                currentUserService.UserId,
                permissions.GlobalPermissions.Count,
                permissions.ProjectPermissions.Count);

            return TypedResults.Ok(permissions);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting user permissions");
            throw;
        }
    }

    private static async Task<Results<Ok<bool>, BadRequest<string>>> CheckPermission(
        CheckPermissionRequestDto request,
        IAuthService authService,
        ICurrentUserService currentUserService,
        ILogger<AuthModule> logger)
    {
        if (!request.ProjectId.HasValue || string.IsNullOrEmpty(request.Role))
        {
            return TypedResults.BadRequest("ProjectId and Role are required");
        }

        try
        {
            var hasAccess = await currentUserService.HasProjectAccessAsync(
                request.ProjectId.Value,
                request.Role);

            logger.LogInformation(
                "Permission check for user {UserId} on project {ProjectId} with role {Role}: {Result}",
                currentUserService.UserId, request.ProjectId, request.Role, hasAccess);

            return TypedResults.Ok(hasAccess);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking permission");
            throw;
        }
    }

    private static async Task<Results<Ok<UserDto>, UnauthorizedHttpResult>> RefreshUserData(
        IAuthService authService,
        ICurrentUserService currentUserService,
        ILogger<AuthModule> logger)
    {
        try
        {
            if (!currentUserService.IsAuthenticated)
            {
                return TypedResults.Unauthorized();
            }

            logger.LogInformation("Force refreshing user data for {UserId}", currentUserService.UserId);

            var user = await authService.SyncCurrentUserWithAzureAsync();

            if (user == null)
            {
                logger.LogWarning("Could not sync user {UserId} with Azure AD", currentUserService.UserId);
                return TypedResults.Unauthorized();
            }

            return TypedResults.Ok(user);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error refreshing user data");
            throw;
        }
    }

    private static bool ShouldSyncWithAzureAd(UserDto user)
    {
        // Sync if last login was more than 24 hours ago
        if (!user.LastLoginAt.HasValue)
            return true;

        var hoursSinceLastLogin = (DateTime.UtcNow - user.LastLoginAt.Value).TotalHours;
        return hoursSinceLastLogin > 24;
    }
}