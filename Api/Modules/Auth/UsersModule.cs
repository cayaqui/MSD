using Api.Authorization;
using Application.Interfaces.Auth;
using Carter;
using Core.Constants;
using Core.DTOs.Auth.ProjectTeamMembers;
using Core.DTOs.Auth.Users;
using Core.DTOs.Common;
using Microsoft.AspNetCore.Authorization;

namespace Api.Modules;

/// <summary>
/// Endpoints de gestión de usuarios
/// </summary>
public class UsersModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var users = app.MapGroup("/api/users")
            .WithTags("Usuarios")
            .RequireAuthorization();

        // Operaciones CRUD de usuarios
        users.MapGet("/", GetUsers)
            .WithName("GetUsers")
            .WithSummary("Obtener lista paginada de usuarios")
            .Produces<PagedResult<UserDto>>(200)
            .RequireAuthorization(policy => policy.RequireRoleOrAdmin(SimplifiedRoles.Project.ProjectManager));

        users.MapPost("/search", SearchUsers)
            .WithName("SearchUsers")
            .WithSummary("Buscar usuarios con filtros")
            .Produces<PagedResult<UserDto>>(200)
            .RequireAuthorization(policy => policy.RequireRoleOrAdmin(SimplifiedRoles.Project.ProjectManager));

        users.MapGet("/{id:guid}", GetUserById)
            .WithName("GetUserById")
            .WithSummary("Obtener usuario por ID")
            .Produces<UserDto>(200)
            .Produces(404);

        users.MapGet("/by-email/{email}", GetUserByEmail)
            .WithName("GetUserByEmail")
            .WithSummary("Obtener usuario por correo electrónico")
            .Produces<UserDto>(200)
            .Produces(404);

        users.MapGet("/by-entra-id/{entraId}", GetUserByEntraId)
            .WithName("GetUserByEntraId")
            .WithSummary("Obtener usuario por ID de Entra")
            .Produces<UserDto>(200)
            .Produces(404);

        users.MapPost("/", CreateUser)
            .WithName("CreateUser")
            .WithSummary("Crear un nuevo usuario")
            .Produces<UserDto>(201)
            .Produces(400)
            .RequireAuthorization(policy => policy.RequireRoleOrAdmin(SimplifiedRoles.Project.ProjectManager));

        users.MapPut("/{id:guid}", UpdateUser)
            .WithName("UpdateUser")
            .WithSummary("Actualizar detalles del usuario")
            .Produces<UserDto>(200)
            .Produces(404)
            .Produces(400)
            .RequireAuthorization(policy => policy.RequireRoleOrAdmin(SimplifiedRoles.Project.ProjectManager));

        users.MapPost("/{id:guid}/activate", ActivateUser)
            .WithName("ActivateUser")
            .WithSummary("Activar un usuario")
            .Produces<UserDto>(200)
            .Produces(404)
            .RequireAuthorization(policy => policy.RequireRoleOrAdmin(SimplifiedRoles.Project.ProjectManager));

        users.MapPost("/{id:guid}/deactivate", DeactivateUser)
            .WithName("DeactivateUser")
            .WithSummary("Desactivar un usuario")
            .Produces<UserDto>(200)
            .Produces(404)
            .RequireAuthorization(policy => policy.RequireRoleOrAdmin(SimplifiedRoles.Project.ProjectManager));

        users.MapDelete("/{id:guid}", DeleteUser)
            .WithName("DeleteUser")
            .WithSummary("Eliminar un usuario")
            .Produces(204)
            .Produces(404)
            .Produces(400)
            .RequireAuthorization(policy => policy.RequireRoleOrAdmin(SimplifiedRoles.Project.ProjectManager));

        // Proyectos del usuario
        users.MapGet("/{id:guid}/projects", GetUserProjects)
            .WithName("GetUserProjects")
            .WithSummary("Obtener asignaciones de proyecto del usuario")
            .Produces<IEnumerable<ProjectTeamMemberDto>>(200)
            .Produces(404);

        // Sincronización con Azure AD
        users.MapPost("/{id:guid}/sync", SyncUserWithAzure)
            .WithName("SyncUserWithAzure")
            .WithSummary("Sincronizar datos del usuario con Azure AD")
            .Produces<UserDto>(200)
            .Produces(404)
            .RequireAuthorization(policy => policy.RequireRoleOrAdmin(SimplifiedRoles.Project.ProjectManager));

        // Foto del usuario
        users.MapGet("/{id:guid}/photo", GetUserPhoto)
            .WithName("GetUserPhoto")
            .WithSummary("Obtener foto del usuario desde Azure AD")
            //.Produces<UserPhotoResponse>(200)
            .Produces(404);

        // Operaciones masivas
        users.MapPost("/bulk/activate", BulkActivateUsers)
            .WithName("BulkActivateUsers")
            .WithSummary("Activar múltiples usuarios")
            .Produces<BulkOperationResult>(200)
            .RequireAuthorization(policy => policy.RequireRoleOrAdmin(SimplifiedRoles.Project.ProjectManager));

        users.MapPost("/bulk/deactivate", BulkDeactivateUsers)
            .WithName("BulkDeactivateUsers")
            .WithSummary("Desactivar múltiples usuarios")
            .Produces<BulkOperationResult>(200)
            .RequireAuthorization(policy => policy.RequireRoleOrAdmin(SimplifiedRoles.Project.ProjectManager));

        // Endpoints de validación
        users.MapGet("/check-email/{email}", CheckEmailExists)
            .WithName("CheckEmailExists")
            .WithSummary("Verificar si el correo electrónico ya existe")
            .Produces<bool>(200);

        users.MapGet("/{id:guid}/can-delete", CanDeleteUser)
            .WithName("CanDeleteUser")
            .WithSummary("Verificar si el usuario puede ser eliminado")
            .Produces<CanDeleteResult>(200)
            .Produces(404);
    }

    private static async Task<IResult> GetUsers(
        [AsParameters] PaginationQuery query,
        IUserService userService)
    {
        var result = await userService.GetPagedAsync(query.PageNumber, query.PageSize);
        return Results.Ok(result);
    }

    private static async Task<IResult> SearchUsers(
        UserFilterDto filter,
        IUserService userService)
    {
        var result = await userService.SearchAsync(filter);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetUserById(
        Guid id,
        IUserService userService)
    {
        var user = await userService.GetByIdAsync(id);
        return user != null ? Results.Ok(user) : Results.NotFound($"Usuario {id} no encontrado");
    }

    private static async Task<IResult> GetUserByEmail(
        string email,
        IUserService userService)
    {
        var user = await userService.GetByEmailAsync(email);
        return user != null ? Results.Ok(user) : Results.NotFound($"Usuario con correo {email} no encontrado");
    }

    private static async Task<IResult> GetUserByEntraId(
        string entraId,
        IUserService userService)
    {
        var user = await userService.GetByEntraIdAsync(entraId);
        return user != null ? Results.Ok(user) : Results.NotFound($"Usuario con ID de Entra {entraId} no encontrado");
    }

    private static async Task<IResult> CreateUser(
        CreateUserDto dto,
        IUserService userService,
        ICurrentUserService currentUserService)
    {
        try
        {
            // Verificar si el correo ya existe
            if (await userService.EmailExistsAsync(dto.Email))
            {
                return Results.BadRequest("Ya existe un usuario con este correo electrónico");
            }

            var user = await userService.CreateAsync(dto);
            return Results.Created($"/api/users/{user.Id}", user);
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "Creación de usuario fallida",
                detail: ex.Message,
                statusCode: 400);
        }
    }

    private static async Task<IResult> UpdateUser(
        Guid id,
        UpdateUserDto dto,
        IUserService userService)
    {
        try
        {
            // Verificar si el correo está siendo cambiado y ya existe
            if (!string.IsNullOrEmpty(dto.Email))
            {
                if (await userService.EmailExistsAsync(dto.Email, id))
                {
                    return Results.BadRequest("Ya existe un usuario con este correo electrónico");
                }
            }

            var user = await userService.UpdateAsync(id, dto);
            return user != null ? Results.Ok(user) : Results.NotFound($"Usuario {id} no encontrado");
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "Actualización de usuario fallida",
                detail: ex.Message,
                statusCode: 400);
        }
    }

    private static async Task<IResult> ActivateUser(
        Guid id,
        IUserService userService)
    {
        var user = await userService.ActivateAsync(id);
        return user != null ? Results.Ok(user) : Results.NotFound($"Usuario {id} no encontrado");
    }

    private static async Task<IResult> DeactivateUser(
        Guid id,
        IUserService userService)
    {
        var user = await userService.DeactivateAsync(id);
        return user != null ? Results.Ok(user) : Results.NotFound($"Usuario {id} no encontrado");
    }

    private static async Task<IResult> DeleteUser(
        Guid id,
        IUserService userService,
        ICurrentUserService currentUserService)
    {
        try
        {
            // Verificar si el usuario puede ser eliminado
            if (!await userService.CanUserBeDeletedAsync(id))
            {
                return Results.BadRequest("El usuario no puede ser eliminado debido a dependencias existentes");
            }

            await userService.DeleteAsync(id, currentUserService.UserId);
            return Results.NoContent();
        }
        catch (KeyNotFoundException)
        {
            return Results.NotFound($"Usuario {id} no encontrado");
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "Eliminación de usuario fallida",
                detail: ex.Message,
                statusCode: 400);
        }
    }

    private static async Task<IResult> GetUserProjects(
        Guid id,
        IUserService userService)
    {
        var projects = await userService.GetUserProjectsAsync(id);
        return Results.Ok(projects);
    }

    private static async Task<IResult> SyncUserWithAzure(
        Guid id,
        IUserService userService)
    {
        try
        {
            var user = await userService.SyncWithAzureADAsync(id);
            return user != null ? Results.Ok(user) : Results.NotFound($"Usuario {id} no encontrado");
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "Sincronización con Azure AD fallida",
                detail: ex.Message,
                statusCode: 500);
        }
    }

    private static async Task<IResult> GetUserPhoto(
        Guid id,
        IUserService userService)
    {
        // Esto necesitaría ser implementado en el servicio
        var user = await userService.GetByIdAsync(id);
        if (user == null) return Results.NotFound();
        
        // Retornar foto vacía por ahora
        return Results.Ok(new { ContentType = "image/png", Data = Array.Empty<byte>() });
    }

    private static async Task<IResult> BulkActivateUsers(
        BulkUserOperationRequest request,
        IUserService userService)
    {
        var count = await userService.BulkActivateAsync(request.UserIds);
        return Results.Ok(new BulkOperationResult(count, 0));
    }

    private static async Task<IResult> BulkDeactivateUsers(
        BulkUserOperationRequest request,
        IUserService userService)
    {
        var count = await userService.BulkDeactivateAsync(request.UserIds);
        return Results.Ok(new BulkOperationResult(count, 0));
    }

    private static async Task<IResult> CheckEmailExists(
        string email,
        IUserService userService)
    {
        var exists = await userService.EmailExistsAsync(email);
        return Results.Ok(exists);
    }

    private static async Task<IResult> CanDeleteUser(
        Guid id,
        IUserService userService)
    {
        var canDelete = await userService.CanUserBeDeletedAsync(id);
        return Results.Ok(new CanDeleteResult(canDelete, 
            canDelete ? null : "El usuario tiene asignaciones de proyecto activas u otras dependencias"));
    }
}

// DTOs de solicitud/respuesta
