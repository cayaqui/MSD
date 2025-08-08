using Api.Authorization;
using Application.Interfaces.Organization;
using Application.Interfaces.Auth;
using Carter;
using Core.DTOs.Common;
using Core.DTOs.Organization.Phase;
using Core.Enums.Projects;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Organization;

/// <summary>
/// Endpoints para la gestión de fases de proyecto
/// </summary>
public class PhaseModule : CarterModule
{
    public PhaseModule() : base("/api/phases")
    {
        RequireAuthorization();
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        // Query endpoints
        app.MapGet("/", GetPhasesAsync)
            .WithName("GetPhases")
            .WithSummary("Obtener todas las fases con paginación")
            .WithDescription("Devuelve una lista paginada de fases")
            .WithTags("Fases")
            .Produces<PagedResult<PhaseDto>>();

        app.MapGet("/{id:guid}", GetPhaseByIdAsync)
            .WithName("GetPhaseById")
            .WithSummary("Obtener fase por ID")
            .WithDescription("Devuelve una fase específica por ID")
            .WithTags("Fases")
            .Produces<PhaseDto>()
            .Produces(404);

        app.MapGet("/project/{projectId:guid}", GetPhasesByProjectAsync)
            .WithName("GetPhasesByProject")
            .WithSummary("Obtener fases por proyecto")
            .WithDescription("Devuelve todas las fases para un proyecto específico")
            .WithTags("Fases")
            .Produces<List<PhaseDto>>()
            .Produces(404);

        app.MapGet("/{id:guid}/milestones", GetPhaseMilestonesAsync)
            .WithName("GetPhaseMilestones")
            .WithSummary("Obtener hitos de la fase")
            .WithDescription("Devuelve todos los hitos para una fase específica")
            .WithTags("Fases")
            .Produces<PhaseDto>()
            .Produces(404);

        app.MapGet("/{id:guid}/deliverables", GetPhaseDeliverablesAsync)
            .WithName("GetPhaseDeliverables")
            .WithSummary("Obtener entregables de la fase")
            .WithDescription("Devuelve todos los entregables para una fase específica")
            .WithTags("Fases")
            .Produces<PhaseDto>()
            .Produces(404);

        // Command endpoints
        app.MapPost("/", CreatePhaseAsync)
            .WithName("CreatePhase")
            .WithSummary("Crear una nueva fase")
            .WithDescription("Crea una nueva fase de proyecto")
            .WithTags("Fases")
            .RequireAuthorization("ProjectEdit")
            .Produces<Result<Guid>>(201)
            .Produces<Result>(400);

        app.MapPut("/{id:guid}", UpdatePhaseAsync)
            .WithName("UpdatePhase")
            .WithSummary("Actualizar fase")
            .WithDescription("Actualiza una fase existente")
            .WithTags("Fases")
            .RequireAuthorization("ProjectEdit")
            .Produces<Result>()
            .Produces<Result>(400)
            .Produces(404);

        app.MapPut("/{id:guid}/schedule", UpdatePhaseScheduleAsync)
            .WithName("UpdatePhaseSchedule")
            .WithSummary("Actualizar cronograma de la fase")
            .WithDescription("Actualiza las fechas del cronograma de una fase")
            .WithTags("Fases")
            .RequireAuthorization("ProjectEdit")
            .Produces<Result>()
            .Produces<Result>(400)
            .Produces(404);

        app.MapPut("/{id:guid}/budget", UpdatePhaseBudgetAsync)
            .WithName("UpdatePhaseBudget")
            .WithSummary("Actualizar presupuesto de la fase")
            .WithDescription("Actualiza el presupuesto de una fase")
            .WithTags("Fases")
            .RequireAuthorization("ProjectEdit")
            .Produces<Result>()
            .Produces<Result>(400)
            .Produces(404);

        app.MapPost("/{id:guid}/start", StartPhaseAsync)
            .WithName("StartPhase")
            .WithSummary("Iniciar fase")
            .WithDescription("Marca una fase como iniciada")
            .WithTags("Fases")
            .RequireAuthorization("ProjectEdit")
            .Produces<Result>()
            .Produces<Result>(400)
            .Produces(404);

        app.MapPost("/{id:guid}/complete", CompletePhaseAsync)
            .WithName("CompletePhase")
            .WithSummary("Completar fase")
            .WithDescription("Marca una fase como completada")
            .WithTags("Fases")
            .RequireAuthorization("ProjectEdit")
            .Produces<Result>()
            .Produces<Result>(400)
            .Produces(404);

        app.MapPost("/{id:guid}/approve-gate", ApprovePhaseGateAsync)
            .WithName("ApprovePhaseGate")
            .WithSummary("Aprobar puerta de fase")
            .WithDescription("Aprueba la revisión de puerta para una fase")
            .WithTags("Fases")
            .RequireAuthorization("ProjectApprove")
            .Produces<Result>()
            .Produces<Result>(400)
            .Produces(404);

        app.MapDelete("/{id:guid}", DeletePhaseAsync)
            .WithName("DeletePhase")
            .WithSummary("Eliminar fase")
            .WithDescription("Elimina de forma lógica una fase")
            .WithTags("Fases")
            .RequireAuthorization("ProjectEdit")
            .Produces<Result>()
            .Produces<Result>(400)
            .Produces(404);
    }

    private static async Task<IResult> GetPhasesAsync(
        [FromServices] IPhaseService phaseService,
        [AsParameters] SimpleQueryParameters parameters,
        CancellationToken cancellationToken)
    {
        var result = await phaseService.GetAllPagedAsync(parameters.PageNumber, parameters.PageSize, parameters.SortBy, !parameters.IsAscending, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetPhaseByIdAsync(
        [FromRoute] Guid id,
        [FromServices] IPhaseService phaseService,
        CancellationToken cancellationToken)
    {
        var result = await phaseService.GetByIdAsync(id, cancellationToken);
        return result is not null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> GetPhasesByProjectAsync(
        [FromRoute] Guid projectId,
        [FromServices] IPhaseService phaseService,
        CancellationToken cancellationToken)
    {
        var result = await phaseService.GetByProjectAsync(projectId, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetPhaseMilestonesAsync(
        [FromRoute] Guid id,
        [FromServices] IPhaseService phaseService,
        CancellationToken cancellationToken)
    {
        // This would need to be implemented in the service
        var phase = await phaseService.GetByIdAsync(id, cancellationToken);
        if (phase == null) return Results.NotFound();
        return Results.Ok(phase);
    }

    private static async Task<IResult> GetPhaseDeliverablesAsync(
        [FromRoute] Guid id,
        [FromServices] IPhaseService phaseService,
        CancellationToken cancellationToken)
    {
        // This would need to be implemented in the service
        var phase = await phaseService.GetByIdAsync(id, cancellationToken);
        if (phase == null) return Results.NotFound();
        return Results.Ok(phase);
    }

    private static async Task<IResult> CreatePhaseAsync(
        [FromBody] CreatePhaseDto dto,
        [FromServices] IPhaseService phaseService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        var result = await phaseService.CreateAsync(dto, userId, cancellationToken);
        
        return result != null 
            ? Results.CreatedAtRoute("GetPhaseById", new { id = result.Id }, result)
            : Results.BadRequest("Error al crear la fase");
    }

    private static async Task<IResult> UpdatePhaseAsync(
        [FromRoute] Guid id,
        [FromBody] UpdatePhaseDto dto,
        [FromServices] IPhaseService phaseService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        var result = await phaseService.UpdateAsync(id, dto, userId, cancellationToken);
        
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> UpdatePhaseScheduleAsync(
        [FromRoute] Guid id,
        [FromBody] object dto, // Generic object since we don't have the specific DTO
        [FromServices] IPhaseService phaseService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        // This would need to be implemented using Update method
        var existing = await phaseService.GetByIdAsync(id, cancellationToken);
        if (existing == null) return Results.NotFound();
        
        var updateDto = new UpdatePhaseDto
        {
            Name = existing.Name,
            Description = existing.Description,
            PlannedStartDate = existing.PlannedStartDate,
            PlannedEndDate = existing.PlannedEndDate,
            PlannedBudget = existing.PlannedBudget,
            ApprovedBudget = existing.ApprovedBudget,
            WeightFactor = existing.WeightFactor,
            RequiresGateApproval = existing.RequiresGateApproval,
            KeyDeliverables = existing.KeyDeliverables
        };
        
        var result = await phaseService.UpdateAsync(id, updateDto, userId, cancellationToken);
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> UpdatePhaseBudgetAsync(
        [FromRoute] Guid id,
        [FromBody] object dto, // Generic object since we don't have the specific DTO
        [FromServices] IPhaseService phaseService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        // This would need to be implemented using Update method
        var existing = await phaseService.GetByIdAsync(id, cancellationToken);
        if (existing == null) return Results.NotFound();
        
        var updateDto = new UpdatePhaseDto
        {
            Name = existing.Name,
            Description = existing.Description,
            PlannedStartDate = existing.PlannedStartDate,
            PlannedEndDate = existing.PlannedEndDate,
            PlannedBudget = existing.PlannedBudget,
            ApprovedBudget = existing.ApprovedBudget,
            WeightFactor = existing.WeightFactor,
            RequiresGateApproval = existing.RequiresGateApproval,
            KeyDeliverables = existing.KeyDeliverables
        };
        
        var result = await phaseService.UpdateAsync(id, updateDto, userId, cancellationToken);
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> StartPhaseAsync(
        [FromRoute] Guid id,
        [FromServices] IPhaseService phaseService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        var result = await phaseService.StartPhaseAsync(id, userId, cancellationToken);
        
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> CompletePhaseAsync(
        [FromRoute] Guid id,
        [FromServices] IPhaseService phaseService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        var result = await phaseService.CompletePhaseAsync(id, userId, cancellationToken);
        
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> ApprovePhaseGateAsync(
        [FromRoute] Guid id,
        [FromBody] object dto, // Generic object since we don't have the specific DTO
        [FromServices] IPhaseService phaseService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        var result = await phaseService.ApproveGateAsync(id, userId, cancellationToken);
        
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> DeletePhaseAsync(
        [FromRoute] Guid id,
        [FromServices] IPhaseService phaseService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        var result = await phaseService.DeleteAsync(id, cancellationToken);
        
        return result ? Results.Ok("Fase eliminada exitosamente") : Results.BadRequest("Error al eliminar la fase");
    }
}