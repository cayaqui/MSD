using Api.Authorization;
using Application.Interfaces.Organization;
using Application.Interfaces.Auth;
using Carter;
using Core.DTOs.Common;
using Core.DTOs.Organization.RAM;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Organization;

/// <summary>
/// Endpoints para la gestión de RAM (Matriz de Asignación de Responsabilidades)
/// </summary>
public class RAMModule : CarterModule
{
    public RAMModule() : base("/api/ram")
    {
        RequireAuthorization();
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        // Query endpoints
        app.MapGet("/", GetRAMsAsync)
            .WithName("GetRAMs")
            .WithSummary("Obtener todas las asignaciones RAM con paginación")
            .WithDescription("Devuelve una lista paginada de asignaciones RAM")
            .WithTags("RAM")
            .Produces<PagedResult<RAMDto>>();

        app.MapGet("/{id:guid}", GetRAMByIdAsync)
            .WithName("GetRAMById")
            .WithSummary("Obtener asignación RAM por ID")
            .WithDescription("Devuelve una asignación RAM específica por ID")
            .WithTags("RAM")
            .Produces<RAMDto>()
            .Produces(404);

        app.MapGet("/project/{projectId:guid}", GetRAMByProjectAsync)
            .WithName("GetRAMByProject")
            .WithSummary("Obtener asignaciones RAM por proyecto")
            .WithDescription("Devuelve todas las asignaciones RAM para un proyecto específico")
            .WithTags("RAM")
            .Produces<List<RAMDto>>()
            .Produces(404);

        app.MapGet("/project/{projectId:guid}/matrix", GetProjectRAMMatrixAsync)
            .WithName("GetProjectRAMMatrix")
            .WithSummary("Obtener matriz RAM del proyecto")
            .WithDescription("Devuelve la matriz RAM completa para un proyecto")
            .WithTags("RAM")
            .Produces<RAMMatrixDto>()
            .Produces(404);

        app.MapGet("/user/{userId:guid}/assignments", GetUserRAMAssignmentsAsync)
            .WithName("GetUserRAMAssignments")
            .WithSummary("Obtener asignaciones RAM del usuario")
            .WithDescription("Devuelve todas las asignaciones RAM para un usuario específico")
            .WithTags("RAM")
            .Produces<List<UserRAMAssignmentDto>>()
            .Produces(404);

        app.MapGet("/obs-node/{obsNodeId:guid}", GetRAMByOBSNodeAsync)
            .WithName("GetRAMByOBSNode")
            .WithSummary("Obtener asignaciones RAM por nodo OBS")
            .WithDescription("Devuelve todas las asignaciones RAM para un nodo OBS específico")
            .WithTags("RAM")
            .Produces<List<RAMDto>>()
            .Produces(404);

        // Command endpoints
        app.MapPost("/", CreateRAMAsync)
            .WithName("CreateRAM")
            .WithSummary("Crear una nueva asignación RAM")
            .WithDescription("Crea una nueva asignación RAM")
            .WithTags("RAM")
            .RequireAuthorization("ProjectEdit")
            .Produces<Result<Guid>>(201)
            .Produces<Result>(400);

        app.MapPut("/{id:guid}", UpdateRAMAsync)
            .WithName("UpdateRAM")
            .WithSummary("Actualizar asignación RAM")
            .WithDescription("Actualiza una asignación RAM existente")
            .WithTags("RAM")
            .RequireAuthorization("ProjectEdit")
            .Produces<Result>()
            .Produces<Result>(400)
            .Produces(404);

        app.MapPost("/bulk-assign", BulkAssignRAMAsync)
            .WithName("BulkAssignRAM")
            .WithSummary("Asignar RAM en lote")
            .WithDescription("Crea múltiples asignaciones RAM en una sola operación")
            .WithTags("RAM")
            .RequireAuthorization("ProjectEdit")
            .Produces<BulkOperationResult>()
            .Produces<Result>(400);

        app.MapPost("/import", ImportRAMMatrixAsync)
            .WithName("ImportRAMMatrix")
            .WithSummary("Importar matriz RAM")
            .WithDescription("Importa una matriz RAM completa desde Excel")
            .WithTags("RAM")
            .RequireAuthorization("ProjectEdit")
            .Produces<ImportResultDto>()
            .Produces<Result>(400);

        app.MapGet("/export/{projectId:guid}", ExportRAMMatrixAsync)
            .WithName("ExportRAMMatrix")
            .WithSummary("Exportar matriz RAM")
            .WithDescription("Exporta la matriz RAM a Excel")
            .WithTags("RAM")
            .Produces(200, contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            .Produces(404);

        app.MapPost("/{id:guid}/add-notes", AddRAMNotesAsync)
            .WithName("AddRAMNotes")
            .WithSummary("Añadir notas a asignación RAM")
            .WithDescription("Añade notas a una asignación RAM")
            .WithTags("RAM")
            .RequireAuthorization("ProjectEdit")
            .Produces<Result>()
            .Produces<Result>(400)
            .Produces(404);

        app.MapDelete("/{id:guid}", DeleteRAMAsync)
            .WithName("DeleteRAM")
            .WithSummary("Eliminar asignación RAM")
            .WithDescription("Elimina de forma lógica una asignación RAM")
            .WithTags("RAM")
            .RequireAuthorization("ProjectEdit")
            .Produces<Result>()
            .Produces<Result>(400)
            .Produces(404);
    }

    private static async Task<IResult> GetRAMsAsync(
        [FromServices] IRAMService ramService,
        [AsParameters] SimpleQueryParameters parameters,
        CancellationToken cancellationToken)
    {
        var result = await ramService.GetAllPagedAsync(parameters.PageNumber, parameters.PageSize, parameters.SortBy, !parameters.IsAscending, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetRAMByIdAsync(
        [FromRoute] Guid id,
        [FromServices] IRAMService ramService,
        CancellationToken cancellationToken)
    {
        var result = await ramService.GetByIdAsync(id, cancellationToken);
        return result is not null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> GetRAMByProjectAsync(
        [FromRoute] Guid projectId,
        [FromServices] IRAMService ramService,
        CancellationToken cancellationToken)
    {
        var result = await ramService.GetByProjectAsync(projectId, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetProjectRAMMatrixAsync(
        [FromRoute] Guid projectId,
        [FromServices] IRAMService ramService,
        CancellationToken cancellationToken)
    {
        var result = await ramService.GetRAMMatrixAsync(projectId, cancellationToken);
        return result is not null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> GetUserRAMAssignmentsAsync(
        [FromRoute] Guid userId,
        [FromServices] IRAMService ramService,
        CancellationToken cancellationToken)
    {
        // This would need to be implemented in the service
        return Results.Ok(new List<UserRAMAssignmentDto>());
    }

    private static async Task<IResult> GetRAMByOBSNodeAsync(
        [FromRoute] Guid obsNodeId,
        [FromServices] IRAMService ramService,
        CancellationToken cancellationToken)
    {
        var result = await ramService.GetByOBSNodeAsync(obsNodeId, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> CreateRAMAsync(
        [FromBody] CreateRAMDto dto,
        [FromServices] IRAMService ramService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        var result = await ramService.CreateAsync(dto, userId, cancellationToken);
        
        return result != null 
            ? Results.CreatedAtRoute("GetRAMById", new { id = result.Id }, result)
            : Results.BadRequest("Error al crear la asignación RAM");
    }

    private static async Task<IResult> UpdateRAMAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateRAMDto dto,
        [FromServices] IRAMService ramService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        var result = await ramService.UpdateAsync(id, dto, userId, cancellationToken);
        
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> BulkAssignRAMAsync(
        [FromBody] List<CreateRAMDto> dto,
        [FromServices] IRAMService ramService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        var result = await ramService.BulkCreateAsync(dto, userId, cancellationToken);
        
        return Results.Ok(result);
    }

    private static async Task<IResult> ImportRAMMatrixAsync(
        IFormFile file,
        [FromQuery] Guid projectId,
        [FromServices] IRAMService ramService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
            return Results.BadRequest(Result.Failure("No se cargó ningún archivo"));

        using var stream = new MemoryStream();
        await file.CopyToAsync(stream, cancellationToken);
        stream.Position = 0;

        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        // This would need to be implemented in the service
        return Results.Ok(new ImportResultDto { SuccessfulRecords = 0, FailedRecords = 0 });
    }

    private static async Task<IResult> ExportRAMMatrixAsync(
        [FromRoute] Guid projectId,
        [FromServices] IRAMService ramService,
        CancellationToken cancellationToken)
    {
        // This would need to be implemented in the service
        var result = await ramService.GetRAMMatrixAsync(projectId, cancellationToken);
        if (result == null) return Results.NotFound();
        
        // Return empty file for now
        var emptyFile = new byte[0];
        return Results.File(
            emptyFile,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"RAM_Matrix_{projectId}_{DateTime.UtcNow:yyyyMMdd}.xlsx");
    }

    private static async Task<IResult> AddRAMNotesAsync(
        [FromRoute] Guid id,
        [FromBody] AddRAMNotesDto dto,
        [FromServices] IRAMService ramService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        // This would need to be implemented using Update method
        var existing = await ramService.GetByIdAsync(id, cancellationToken);
        if (existing == null) return Results.NotFound();
        
        var updateDto = new UpdateRAMDto
        {
            ResponsibilityType = existing.ResponsibilityType,
            AllocationPercentage = existing.AllocationPercentage,
            PlannedManHours = existing.PlannedManHours,
            PlannedCost = existing.PlannedCost,
            StartDate = existing.StartDate,
            EndDate = existing.EndDate,
            ControlAccountId = existing.ControlAccountId,
            Notes = dto.Notes,
            IsActive = existing.IsActive
        };
        
        var result = await ramService.UpdateAsync(id, updateDto, userId, cancellationToken);
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> DeleteRAMAsync(
        [FromRoute] Guid id,
        [FromServices] IRAMService ramService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        var result = await ramService.DeleteAsync(id, cancellationToken);
        
        return result ? Results.Ok("Asignación RAM eliminada exitosamente") : Results.BadRequest("Error al eliminar la asignación RAM");
    }
}