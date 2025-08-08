using Application.Interfaces.Auth;
using Carter;
using Core.DTOs.Auth.Permissions;
using Core.DTOs.Auth.Users;
using Microsoft.AspNetCore.Authorization;

namespace Api.Modules;

/// <summary>
/// Endpoints de autenticación y autorización
/// </summary>
public class AuthModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var auth = app.MapGroup("/api/auth")
            .WithTags("Autenticación")
            .RequireAuthorization();

        // Current user endpoints
        auth.MapGet("/me", GetCurrentUser)
            .WithName("GetCurrentUser")
            .WithSummary("Obtener usuario autenticado actual")
            .Produces<UserDto>(200)
            .Produces(401);

        auth.MapGet("/me/permissions", GetMyPermissions)
            .WithName("GetMyPermissions")
            .WithSummary("Obtener permisos del usuario actual")
            .Produces<UserPermissionsDto>(200)
            .Produces(401);

        auth.MapGet("/me/projects", GetMyProjects)
            .WithName("GetMyProjects")
            .WithSummary("Obtener proyectos accesibles para el usuario actual")
            .Produces<List<Guid>>(200)
            .Produces(401);

        auth.MapGet("/me/projects/{projectId}/permissions", GetMyProjectPermissions)
            .WithName("GetMyProjectPermissions")
            .WithSummary("Obtener permisos del usuario actual para un proyecto específico")
            .Produces<ProjectPermissionDto>(200)
            .Produces(401)
            .Produces(404);

        // Azure AD sync endpoints
        auth.MapPost("/sync", SyncCurrentUserWithAzure)
            .WithName("SyncCurrentUserWithAzure")
            .WithSummary("Sincronizar datos del usuario actual con Azure AD")
            .Produces<UserDto>(200)
            .Produces(401);

        // Permission check endpoints
        auth.MapPost("/check-permission", CheckPermission)
            .WithName("CheckPermission")
            .WithSummary("Verificar si el usuario actual tiene un permiso específico")
            .Produces<bool>(200)
            .Produces(401);

        auth.MapPost("/check-project-access", CheckProjectAccess)
            .WithName("CheckProjectAccess")
            .WithSummary("Verificar si el usuario actual puede acceder a un proyecto")
            .Produces<bool>(200)
            .Produces(401);

        // Public endpoints (no auth required)
        var publicAuth = app.MapGroup("/api/auth/public")
            .WithTags("Autenticación - Público")
            .AllowAnonymous();

        publicAuth.MapGet("/login-info", GetLoginInfo)
            .WithName("GetLoginInfo")
            .WithSummary("Obtener información de configuración de inicio de sesión")
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
                title: "Usuario no encontrado",
                detail: "El usuario autenticado no existe en el sistema. Por favor contacte a un administrador.",
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
                title: "Permisos no encontrados",
                detail: "No se pudieron obtener los permisos del usuario",
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
            return Results.NotFound($"Proyecto {projectId} no encontrado o acceso denegado");
        }

        var permissions = await authService.GetUserProjectPermissionsAsync(projectId);
        if (permissions == null)
        {
            return Results.NotFound($"No se encontraron permisos para el proyecto {projectId}");
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
                    title: "Sincronización fallida",
                    detail: "No se pudo sincronizar el usuario con Azure AD. El usuario puede no existir en el sistema.",
                    statusCode: 400);
            }

            return Results.Ok(user);
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "Error de sincronización",
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
            return Results.BadRequest("El permiso es requerido");
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