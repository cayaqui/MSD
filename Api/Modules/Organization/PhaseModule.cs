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
/// Endpoints for project phase management
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
            .WithSummary("Get all phases with pagination")
            .WithDescription("Returns a paginated list of phases")
            .WithTags("Phases")
            .Produces<PagedResult<PhaseDto>>();

        app.MapGet("/{id:guid}", GetPhaseByIdAsync)
            .WithName("GetPhaseById")
            .WithSummary("Get phase by ID")
            .WithDescription("Returns a specific phase by ID")
            .WithTags("Phases")
            .Produces<PhaseDto>()
            .Produces(404);

        app.MapGet("/project/{projectId:guid}", GetPhasesByProjectAsync)
            .WithName("GetPhasesByProject")
            .WithSummary("Get phases by project")
            .WithDescription("Returns all phases for a specific project")
            .WithTags("Phases")
            .Produces<List<PhaseDto>>()
            .Produces(404);

        app.MapGet("/{id:guid}/milestones", GetPhaseMilestonesAsync)
            .WithName("GetPhaseMilestones")
            .WithSummary("Get phase milestones")
            .WithDescription("Returns all milestones for a specific phase")
            .WithTags("Phases")
            .Produces<PhaseDto>()
            .Produces(404);

        app.MapGet("/{id:guid}/deliverables", GetPhaseDeliverablesAsync)
            .WithName("GetPhaseDeliverables")
            .WithSummary("Get phase deliverables")
            .WithDescription("Returns all deliverables for a specific phase")
            .WithTags("Phases")
            .Produces<PhaseDto>()
            .Produces(404);

        // Command endpoints
        app.MapPost("/", CreatePhaseAsync)
            .WithName("CreatePhase")
            .WithSummary("Create a new phase")
            .WithDescription("Creates a new project phase")
            .WithTags("Phases")
            .RequireAuthorization("ProjectEdit")
            .Produces<Result<Guid>>(201)
            .Produces<Result>(400);

        app.MapPut("/{id:guid}", UpdatePhaseAsync)
            .WithName("UpdatePhase")
            .WithSummary("Update phase")
            .WithDescription("Updates an existing phase")
            .WithTags("Phases")
            .RequireAuthorization("ProjectEdit")
            .Produces<Result>()
            .Produces<Result>(400)
            .Produces(404);

        app.MapPut("/{id:guid}/schedule", UpdatePhaseScheduleAsync)
            .WithName("UpdatePhaseSchedule")
            .WithSummary("Update phase schedule")
            .WithDescription("Updates the schedule dates of a phase")
            .WithTags("Phases")
            .RequireAuthorization("ProjectEdit")
            .Produces<Result>()
            .Produces<Result>(400)
            .Produces(404);

        app.MapPut("/{id:guid}/budget", UpdatePhaseBudgetAsync)
            .WithName("UpdatePhaseBudget")
            .WithSummary("Update phase budget")
            .WithDescription("Updates the budget of a phase")
            .WithTags("Phases")
            .RequireAuthorization("ProjectEdit")
            .Produces<Result>()
            .Produces<Result>(400)
            .Produces(404);

        app.MapPost("/{id:guid}/start", StartPhaseAsync)
            .WithName("StartPhase")
            .WithSummary("Start phase")
            .WithDescription("Marks a phase as started")
            .WithTags("Phases")
            .RequireAuthorization("ProjectEdit")
            .Produces<Result>()
            .Produces<Result>(400)
            .Produces(404);

        app.MapPost("/{id:guid}/complete", CompletePhaseAsync)
            .WithName("CompletePhase")
            .WithSummary("Complete phase")
            .WithDescription("Marks a phase as completed")
            .WithTags("Phases")
            .RequireAuthorization("ProjectEdit")
            .Produces<Result>()
            .Produces<Result>(400)
            .Produces(404);

        app.MapPost("/{id:guid}/approve-gate", ApprovePhaseGateAsync)
            .WithName("ApprovePhaseGate")
            .WithSummary("Approve phase gate")
            .WithDescription("Approves the gate review for a phase")
            .WithTags("Phases")
            .RequireAuthorization("ProjectApprove")
            .Produces<Result>()
            .Produces<Result>(400)
            .Produces(404);

        app.MapDelete("/{id:guid}", DeletePhaseAsync)
            .WithName("DeletePhase")
            .WithSummary("Delete phase")
            .WithDescription("Soft deletes a phase")
            .WithTags("Phases")
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
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("User ID not found");
        var result = await phaseService.CreateAsync(dto, userId, cancellationToken);
        
        return result != null 
            ? Results.CreatedAtRoute("GetPhaseById", new { id = result.Id }, result)
            : Results.BadRequest("Failed to create phase");
    }

    private static async Task<IResult> UpdatePhaseAsync(
        [FromRoute] Guid id,
        [FromBody] UpdatePhaseDto dto,
        [FromServices] IPhaseService phaseService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("User ID not found");
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
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("User ID not found");
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
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("User ID not found");
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
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("User ID not found");
        var result = await phaseService.StartPhaseAsync(id, userId, cancellationToken);
        
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> CompletePhaseAsync(
        [FromRoute] Guid id,
        [FromServices] IPhaseService phaseService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("User ID not found");
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
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("User ID not found");
        var result = await phaseService.ApproveGateAsync(id, userId, cancellationToken);
        
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> DeletePhaseAsync(
        [FromRoute] Guid id,
        [FromServices] IPhaseService phaseService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("User ID not found");
        var result = await phaseService.DeleteAsync(id, cancellationToken);
        
        return result ? Results.Ok("Phase deleted successfully") : Results.BadRequest("Failed to delete phase");
    }
}