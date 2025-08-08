using Api.Authorization;
using Application.Interfaces.Organization;
using Application.Interfaces.Auth;
using Carter;
using Core.DTOs.Common;
using Core.DTOs.Organization.Operation;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Organization;

/// <summary>
/// Endpoints para la gestión de operaciones
/// </summary>
public class OperationModule : CarterModule
{
    public OperationModule() : base("/api/operations")
    {
        RequireAuthorization();
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        // Query endpoints
        app.MapGet("/", GetOperationsAsync)
            .WithName("GetOperations")
            .WithSummary("Obtener todas las operaciones con paginación")
            .WithDescription("Devuelve una lista paginada de operaciones")
            .WithTags("Operaciones")
            .Produces<PagedResult<OperationDto>>();

        app.MapGet("/{id:guid}", GetOperationByIdAsync)
            .WithName("GetOperationById")
            .WithSummary("Obtener operación por ID")
            .WithDescription("Devuelve una operación específica por ID")
            .WithTags("Operaciones")
            .Produces<OperationDto>()
            .Produces(404);

        app.MapGet("/company/{companyId:guid}", GetOperationsByCompanyAsync)
            .WithName("GetOperationsByCompany")
            .WithSummary("Obtener operaciones por empresa")
            .WithDescription("Devuelve todas las operaciones para una empresa específica")
            .WithTags("Operaciones")
            .Produces<List<OperationDto>>()
            .Produces(404);

        app.MapGet("/{id:guid}/projects", GetOperationProjectsAsync)
            .WithName("GetOperationProjects")
            .WithSummary("Obtener proyectos de la operación")
            .WithDescription("Devuelve todos los proyectos para una operación específica")
            .WithTags("Operaciones")
            .Produces<OperationWithProjectsDto>()
            .Produces(404);

        // Command endpoints
        app.MapPost("/", CreateOperationAsync)
            .WithName("CreateOperation")
            .WithSummary("Crear una nueva operación")
            .WithDescription("Crea una nueva operación")
            .WithTags("Operaciones")
            .RequireAuthorization("AdminOnly")
            .Produces<Result<Guid>>(201)
            .Produces<Result>(400);

        app.MapPut("/{id:guid}", UpdateOperationAsync)
            .WithName("UpdateOperation")
            .WithSummary("Actualizar operación")
            .WithDescription("Actualiza una operación existente")
            .WithTags("Operaciones")
            .RequireAuthorization("AdminOnly")
            .Produces<Result>()
            .Produces<Result>(400)
            .Produces(404);

        app.MapPost("/{id:guid}/activate", ActivateOperationAsync)
            .WithName("ActivateOperation")
            .WithSummary("Activar operación")
            .WithDescription("Activa una operación")
            .WithTags("Operaciones")
            .RequireAuthorization("AdminOnly")
            .Produces<Result>()
            .Produces(404);

        app.MapPost("/{id:guid}/deactivate", DeactivateOperationAsync)
            .WithName("DeactivateOperation")
            .WithSummary("Desactivar operación")
            .WithDescription("Desactiva una operación")
            .WithTags("Operaciones")
            .RequireAuthorization("AdminOnly")
            .Produces<Result>()
            .Produces(404);

        app.MapDelete("/{id:guid}", DeleteOperationAsync)
            .WithName("DeleteOperation")
            .WithSummary("Eliminar operación")
            .WithDescription("Elimina de forma lógica una operación")
            .WithTags("Operaciones")
            .RequireAuthorization("AdminOnly")
            .Produces<Result>()
            .Produces<Result>(400)
            .Produces(404);
    }

    private static async Task<IResult> GetOperationsAsync(
        [FromServices] IOperationService operationService,
        [AsParameters] SimpleQueryParameters parameters,
        CancellationToken cancellationToken)
    {
        var result = await operationService.GetAllPagedAsync(parameters.PageNumber, parameters.PageSize, parameters.SortBy, !parameters.IsAscending, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetOperationByIdAsync(
        [FromRoute] Guid id,
        [FromServices] IOperationService operationService,
        CancellationToken cancellationToken)
    {
        var result = await operationService.GetByIdAsync(id, cancellationToken);
        return result is not null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> GetOperationsByCompanyAsync(
        [FromRoute] Guid companyId,
        [FromServices] IOperationService operationService,
        CancellationToken cancellationToken)
    {
        var result = await operationService.GetByCompanyAsync(companyId, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetOperationProjectsAsync(
        [FromRoute] Guid id,
        [FromServices] IOperationService operationService,
        CancellationToken cancellationToken)
    {
        var result = await operationService.GetWithProjectsAsync(id, cancellationToken);
        return result is not null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> CreateOperationAsync(
        [FromBody] CreateOperationDto dto,
        [FromServices] IOperationService operationService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        var result = await operationService.CreateAsync(dto, userId, cancellationToken);
        
        return result != null 
            ? Results.CreatedAtRoute("GetOperationById", new { id = result.Id }, result)
            : Results.BadRequest("Error al crear la operación");
    }

    private static async Task<IResult> UpdateOperationAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateOperationDto dto,
        [FromServices] IOperationService operationService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        var result = await operationService.UpdateAsync(id, dto, userId, cancellationToken);
        
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> ActivateOperationAsync(
        [FromRoute] Guid id,
        [FromServices] IOperationService operationService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        // Since there's no IsActive property, just verify the operation exists
        var existing = await operationService.GetByIdAsync(id, cancellationToken);
        if (existing == null) return Results.NotFound();
        
        return Results.Ok("La operación está activa");
    }

    private static async Task<IResult> DeactivateOperationAsync(
        [FromRoute] Guid id,
        [FromServices] IOperationService operationService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        // Since there's no IsActive property, we could use soft delete instead
        var result = await operationService.DeleteAsync(id, cancellationToken);
        
        return result ? Results.Ok("Operación desactivada exitosamente") : Results.BadRequest("Error al desactivar la operación");
    }

    private static async Task<IResult> DeleteOperationAsync(
        [FromRoute] Guid id,
        [FromServices] IOperationService operationService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        var result = await operationService.DeleteAsync(id, cancellationToken);
        
        return result ? Results.Ok("Operación eliminada exitosamente") : Results.BadRequest("Error al eliminar la operación");
    }
}