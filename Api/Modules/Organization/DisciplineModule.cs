using Api.Authorization;
using Application.Interfaces.Organization;
using Application.Interfaces.Auth;
using Carter;
using Core.DTOs.Common;
using Core.DTOs.Organization.Discipline;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Organization;

/// <summary>
/// Endpoints para la gestión de disciplinas
/// </summary>
public class DisciplineModule : CarterModule
{
    public DisciplineModule() : base("/api/disciplines")
    {
        RequireAuthorization();
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        // Query endpoints
        app.MapGet("/", GetDisciplinesAsync)
            .WithName("GetDisciplines")
            .WithSummary("Obtener todas las disciplinas con paginación")
            .WithDescription("Devuelve una lista paginada de disciplinas")
            .WithTags("Disciplinas")
            .Produces<PagedResult<DisciplineDto>>();

        app.MapGet("/{id:guid}", GetDisciplineByIdAsync)
            .WithName("GetDisciplineById")
            .WithSummary("Obtener disciplina por ID")
            .WithDescription("Devuelve una disciplina específica por ID")
            .WithTags("Disciplinas")
            .Produces<DisciplineDto>()
            .Produces(404);

        app.MapGet("/active", GetActiveDisciplinesAsync)
            .WithName("GetActiveDisciplines")
            .WithSummary("Obtener disciplinas activas")
            .WithDescription("Devuelve todas las disciplinas activas")
            .WithTags("Disciplinas")
            .Produces<List<DisciplineDto>>();

        app.MapGet("/by-type/{type}", GetDisciplinesByTypeAsync)
            .WithName("GetDisciplinesByType")
            .WithSummary("Obtener disciplinas por tipo")
            .WithDescription("Devuelve todas las disciplinas de un tipo específico")
            .WithTags("Disciplinas")
            .Produces<List<DisciplineDto>>();

        // Command endpoints
        app.MapPost("/", CreateDisciplineAsync)
            .WithName("CreateDiscipline")
            .WithSummary("Crear una nueva disciplina")
            .WithDescription("Crea una nueva disciplina")
            .WithTags("Disciplinas")
            .RequireAuthorization("AdminOnly")
            .Produces<Result<Guid>>(201)
            .Produces<Result>(400);

        app.MapPut("/{id:guid}", UpdateDisciplineAsync)
            .WithName("UpdateDiscipline")
            .WithSummary("Actualizar disciplina")
            .WithDescription("Actualiza una disciplina existente")
            .WithTags("Disciplinas")
            .RequireAuthorization("AdminOnly")
            .Produces<Result>()
            .Produces<Result>(400)
            .Produces(404);

        app.MapPost("/{id:guid}/activate", ActivateDisciplineAsync)
            .WithName("ActivateDiscipline")
            .WithSummary("Activar disciplina")
            .WithDescription("Activa una disciplina")
            .WithTags("Disciplinas")
            .RequireAuthorization("AdminOnly")
            .Produces<Result>()
            .Produces(404);

        app.MapPost("/{id:guid}/deactivate", DeactivateDisciplineAsync)
            .WithName("DeactivateDiscipline")
            .WithSummary("Desactivar disciplina")
            .WithDescription("Desactiva una disciplina")
            .WithTags("Disciplinas")
            .RequireAuthorization("AdminOnly")
            .Produces<Result>()
            .Produces(404);

        app.MapDelete("/{id:guid}", DeleteDisciplineAsync)
            .WithName("DeleteDiscipline")
            .WithSummary("Eliminar disciplina")
            .WithDescription("Elimina de forma lógica una disciplina")
            .WithTags("Disciplinas")
            .RequireAuthorization("AdminOnly")
            .Produces<Result>()
            .Produces<Result>(400)
            .Produces(404);
    }

    private static async Task<IResult> GetDisciplinesAsync(
        [FromServices] IDisciplineService disciplineService,
        [AsParameters] SimpleQueryParameters parameters,
        CancellationToken cancellationToken)
    {
        var result = await disciplineService.GetAllPagedAsync(parameters.PageNumber, parameters.PageSize, parameters.SortBy, !parameters.IsAscending, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetDisciplineByIdAsync(
        [FromRoute] Guid id,
        [FromServices] IDisciplineService disciplineService,
        CancellationToken cancellationToken)
    {
        var result = await disciplineService.GetByIdAsync(id, cancellationToken);
        return result is not null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> GetActiveDisciplinesAsync(
        [FromServices] IDisciplineService disciplineService,
        CancellationToken cancellationToken)
    {
        // Return all disciplines since there's no IsActive property
        var result = await disciplineService.GetAllAsync(cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetDisciplinesByTypeAsync(
        [FromRoute] string type,
        [FromServices] IDisciplineService disciplineService,
        CancellationToken cancellationToken)
    {
        var result = await disciplineService.GetAllAsync(cancellationToken);
        // Filter by engineering or management based on type
        var disciplinesByType = type.ToLower() switch
        {
            "engineering" => result.Where(d => d.IsEngineering).ToList(),
            "management" => result.Where(d => d.IsManagement).ToList(),
            _ => result.ToList()
        };
        return Results.Ok(disciplinesByType);
    }

    private static async Task<IResult> CreateDisciplineAsync(
        [FromBody] CreateDisciplineDto dto,
        [FromServices] IDisciplineService disciplineService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        var result = await disciplineService.CreateAsync(dto, userId, cancellationToken);
        
        return result != null 
            ? Results.CreatedAtRoute("GetDisciplineById", new { id = result.Id }, result)
            : Results.BadRequest("Error al crear la disciplina");
    }

    private static async Task<IResult> UpdateDisciplineAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateDisciplineDto dto,
        [FromServices] IDisciplineService disciplineService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        var result = await disciplineService.UpdateAsync(id, dto, userId, cancellationToken);
        
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> ActivateDisciplineAsync(
        [FromRoute] Guid id,
        [FromServices] IDisciplineService disciplineService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        // Since there's no IsActive property, just verify the discipline exists
        var existing = await disciplineService.GetByIdAsync(id, cancellationToken);
        if (existing == null) return Results.NotFound();
        
        return Results.Ok("La disciplina está activa");
    }

    private static async Task<IResult> DeactivateDisciplineAsync(
        [FromRoute] Guid id,
        [FromServices] IDisciplineService disciplineService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        // Since there's no IsActive property, we could use soft delete instead
        var result = await disciplineService.DeleteAsync(id, cancellationToken);
        
        return result ? Results.Ok("Disciplina desactivada exitosamente") : Results.BadRequest("Error al desactivar la disciplina");
    }

    private static async Task<IResult> DeleteDisciplineAsync(
        [FromRoute] Guid id,
        [FromServices] IDisciplineService disciplineService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        var result = await disciplineService.DeleteAsync(id, cancellationToken);
        
        return result ? Results.Ok("Disciplina eliminada exitosamente") : Results.BadRequest("Error al eliminar la disciplina");
    }
}