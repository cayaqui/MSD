using Application.Interfaces.Auth;
using Carter;
using Core.DTOs.Auth.Permissions;
using Core.DTOs.Auth.Users;
using Microsoft.AspNetCore.Authorization;

namespace Api.Modules;

/// <summary>
/// Authentication and authorization endpoints
/// </summary>
public class AuthModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var auth = app.MapGroup("/api/auth")
            .WithTags("Authentication")
            .RequireAuthorization();

        // Current user endpoints
        auth.MapGet("/me", GetCurrentUser)
            .WithName("GetCurrentUser")
            .WithSummary("Get current authenticated user")
            .Produces<UserDto>(200)
            .Produces(401);

        auth.MapGet("/me/permissions", GetMyPermissions)
            .WithName("GetMyPermissions")
            .WithSummary("Get current user permissions")
            .Produces<UserPermissionsDto>(200)
            .Produces(401);

        auth.MapGet("/me/projects", GetMyProjects)
            .WithName("GetMyProjects")
            .WithSummary("Get projects accessible to current user")
            .Produces<List<Guid>>(200)
            .Produces(401);

        auth.MapGet("/me/projects/{projectId}/permissions", GetMyProjectPermissions)
            .WithName("GetMyProjectPermissions")
            .WithSummary("Get current user permissions for a specific project")
            .Produces<ProjectPermissionDto>(200)
            .Produces(401)
            .Produces(404);

        // Azure AD sync endpoints
        auth.MapPost("/sync", SyncCurrentUserWithAzure)
            .WithName("SyncCurrentUserWithAzure")
            .WithSummary("Sync current user data with Azure AD")
            .Produces<UserDto>(200)
            .Produces(401);

        // Permission check endpoints
        auth.MapPost("/check-permission", CheckPermission)
            .WithName("CheckPermission")
            .WithSummary("Check if current user has a specific permission")
            .Produces<bool>(200)
            .Produces(401);

        auth.MapPost("/check-project-access", CheckProjectAccess)
            .WithName("CheckProjectAccess")
            .WithSummary("Check if current user can access a project")
            .Produces<bool>(200)
            .Produces(401);

        // Public endpoints (no auth required)
        var publicAuth = app.MapGroup("/api/auth/public")
            .WithTags("Authentication - Public")
            .AllowAnonymous();

        publicAuth.MapGet("/login-info", GetLoginInfo)
            .WithName("GetLoginInfo")
            .WithSummary("Get login configuration info")
            .Produces<LoginInfoDto>(200);
    }

    private static async Task<IResult> GetCurrentUser(
        IAuthService authService,
        IHttpContextAccessor httpContextAccessor)
    {
        var user = await authService.GetCurrentUserAsync();
        
        if (user == null)
        {
            return Results.Problem(
                title: "User not found",
                detail: "Authenticated user does not exist in the system. Please contact an administrator.",
                statusCode: 401);
        }

        return Results.Ok(user);
    }

    private static async Task<IResult> GetMyPermissions(
        IAuthService authService)
    {
        var permissions = await authService.GetUserPermissionsAsync();
        
        if (permissions == null)
        {
            return Results.Problem(
                title: "Permissions not found",
                detail: "Could not retrieve user permissions",
                statusCode: 401);
        }

        return Results.Ok(permissions);
    }

    private static async Task<IResult> GetMyProjects(
        ICurrentUserService currentUserService)
    {
        var projectIds = await currentUserService.GetUserProjectIdsAsync();
        return Results.Ok(projectIds);
    }

    private static async Task<IResult> GetMyProjectPermissions(
        Guid projectId,
        IAuthService authService,
        ICurrentUserService currentUserService)
    {
        // First check if user has access to the project
        var hasAccess = await currentUserService.HasProjectAccessAsync(projectId);
        if (!hasAccess)
        {
            return Results.NotFound($"Project {projectId} not found or access denied");
        }

        var permissions = await authService.GetUserProjectPermissionsAsync(projectId);
        if (permissions == null)
        {
            return Results.NotFound($"No permissions found for project {projectId}");
        }

        return Results.Ok(permissions);
    }

    private static async Task<IResult> SyncCurrentUserWithAzure(
        IAuthService authService)
    {
        try
        {
            var user = await authService.SyncCurrentUserWithAzureAsync();
            
            if (user == null)
            {
                return Results.Problem(
                    title: "Sync failed",
                    detail: "Could not sync user with Azure AD. User may not exist in the system.",
                    statusCode: 400);
            }

            return Results.Ok(user);
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "Sync error",
                detail: ex.Message,
                statusCode: 500);
        }
    }

    private static async Task<IResult> CheckPermission(
        CheckPermissionRequest request,
        ICurrentUserService currentUserService)
    {
        if (string.IsNullOrWhiteSpace(request.Permission))
        {
            return Results.BadRequest("Permission is required");
        }

        var hasPermission = await currentUserService.HasPermissionAsync(request.Permission);
        return Results.Ok(new { hasPermission });
    }

    private static async Task<IResult> CheckProjectAccess(
        CheckProjectAccessRequest request,
        ICurrentUserService currentUserService)
    {
        var hasAccess = await currentUserService.HasProjectAccessAsync(
            request.ProjectId, 
            request.RequiredRole);
            
        return Results.Ok(new { hasAccess });
    }

    private static IResult GetLoginInfo(IConfiguration configuration)
    {
        var loginInfo = new LoginInfoDto(
            configuration["AzureAd:Authority"] ?? "", 
            configuration["AzureAd:ClientId"] ?? "",
            configuration["AzureAd:TenantId"] ?? "",
            configuration.GetSection("AzureAd:Scopes").Get<string[]>() ?? Array.Empty<string>()
        );

        return Results.Ok(loginInfo);
    }
}

// Request DTOs
public record CheckPermissionRequest(string Permission, Guid? ProjectId = null);
public record CheckProjectAccessRequest(Guid ProjectId, string? RequiredRole = null);

// Response DTOs
public record LoginInfoDto(
    string Authority,
    string ClientId,
    string TenantId,
    string[] Scopes);