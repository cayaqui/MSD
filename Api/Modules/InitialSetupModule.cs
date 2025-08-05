using Application.Interfaces.Auth;
using Carter;
using Core.DTOs.Auth.Users;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Api.Modules;

/// <summary>
/// Módulo temporal para configuración inicial
/// IMPORTANTE: Eliminar o comentar después de crear el primer usuario administrador
/// </summary>
public class InitialSetupModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/setup")
            .WithTags("Initial Setup");

        // Endpoint para auto-registrar el usuario actual como administrador
        // SOLO funciona si no hay usuarios en el sistema
        group.MapPost("/initialize-admin", InitializeAdmin)
            .WithName("InitializeAdmin")
            .WithSummary("Auto-registra el usuario autenticado como administrador del sistema")
            .WithDescription("SOLO funciona si no hay usuarios registrados en el sistema")
            .Produces<UserDto>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status409Conflict)
            .RequireAuthorization();
    }

    private static async Task<IResult> InitializeAdmin(
        ClaimsPrincipal user,
        IUserService userService,
        ILogger<InitialSetupModule> logger)
    {
        try
        {
            // Obtener información del usuario autenticado
            var entraId = user.FindFirst("oid")?.Value ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = user.FindFirst("preferred_username")?.Value ?? user.FindFirst(ClaimTypes.Email)?.Value;
            var name = user.FindFirst("name")?.Value ?? user.Identity?.Name;

            if (string.IsNullOrEmpty(entraId) || string.IsNullOrEmpty(email))
            {
                return Results.BadRequest(new { error = "No se pudo obtener la información del usuario de Azure AD" });
            }

            // Crear el usuario administrador
            var createUserDto = new CreateUserDto
            {
                EntraId = entraId,
                Email = email!,
                Name = name ?? email!
            };

            var newUser = await userService.CreateAsync(createUserDto);

            logger.LogInformation("Usuario administrador inicial creado: {Email} con ID: {UserId}", email, newUser.Id);

            return Results.Created($"/api/users/{newUser.Id}", new 
            { 
                user = newUser,
                message = "Usuario creado exitosamente. IMPORTANTE: Para asignar permisos de administrador, ejecuta el siguiente script SQL con el ID del usuario.",
                userId = newUser.Id,
                sqlScript = $@"
-- Asignar permisos de administrador al usuario
DECLARE @UserId UNIQUEIDENTIFIER = '{newUser.Id}';
DECLARE @Now DATETIME2 = GETUTCDATE();

-- Insertar permisos globales (sin ProjectId)
INSERT INTO [dbo].[UserProjectPermissions] 
    ([Id], [UserId], [ProjectId], [PermissionCode], [IsGranted], [GrantedAt], [GrantedBy], [IsActive], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy])
VALUES 
    (NEWID(), @UserId, '00000000-0000-0000-0000-000000000000', 'system.admin', 1, @Now, 'System', 1, @Now, 'System', @Now, 'System'),
    (NEWID(), @UserId, '00000000-0000-0000-0000-000000000000', 'users.view', 1, @Now, 'System', 1, @Now, 'System', @Now, 'System'),
    (NEWID(), @UserId, '00000000-0000-0000-0000-000000000000', 'users.manage', 1, @Now, 'System', 1, @Now, 'System', @Now, 'System'),
    (NEWID(), @UserId, '00000000-0000-0000-0000-000000000000', 'projects.view', 1, @Now, 'System', 1, @Now, 'System', @Now, 'System'),
    (NEWID(), @UserId, '00000000-0000-0000-0000-000000000000', 'projects.create', 1, @Now, 'System', 1, @Now, 'System', @Now, 'System'),
    (NEWID(), @UserId, '00000000-0000-0000-0000-000000000000', 'projects.edit', 1, @Now, 'System', 1, @Now, 'System', @Now, 'System'),
    (NEWID(), @UserId, '00000000-0000-0000-0000-000000000000', 'projects.manage', 1, @Now, 'System', 1, @Now, 'System', @Now, 'System');
"
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al crear el usuario administrador inicial");
            return Results.Problem($"Error al crear el usuario administrador inicial: {ex.Message}");
        }
    }
}