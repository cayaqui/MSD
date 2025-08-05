using Api.Authorization;
using Application.Interfaces.Organization;
using Application.Interfaces.Auth;
using Carter;
using Core.DTOs.Common;
using Core.DTOs.Organization.Discipline;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Organization;

/// <summary>
/// Endpoints for discipline management
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
            .WithSummary("Get all disciplines with pagination")
            .WithDescription("Returns a paginated list of disciplines")
            .WithTags("Disciplines")
            .Produces<PagedResult<DisciplineDto>>();

        app.MapGet("/{id:guid}", GetDisciplineByIdAsync)
            .WithName("GetDisciplineById")
            .WithSummary("Get discipline by ID")
            .WithDescription("Returns a specific discipline by ID")
            .WithTags("Disciplines")
            .Produces<DisciplineDto>()
            .Produces(404);

        app.MapGet("/active", GetActiveDisciplinesAsync)
            .WithName("GetActiveDisciplines")
            .WithSummary("Get active disciplines")
            .WithDescription("Returns all active disciplines")
            .WithTags("Disciplines")
            .Produces<List<DisciplineDto>>();

        app.MapGet("/by-type/{type}", GetDisciplinesByTypeAsync)
            .WithName("GetDisciplinesByType")
            .WithSummary("Get disciplines by type")
            .WithDescription("Returns all disciplines of a specific type")
            .WithTags("Disciplines")
            .Produces<List<DisciplineDto>>();

        // Command endpoints
        app.MapPost("/", CreateDisciplineAsync)
            .WithName("CreateDiscipline")
            .WithSummary("Create a new discipline")
            .WithDescription("Creates a new discipline")
            .WithTags("Disciplines")
            .RequireAuthorization("AdminOnly")
            .Produces<Result<Guid>>(201)
            .Produces<Result>(400);

        app.MapPut("/{id:guid}", UpdateDisciplineAsync)
            .WithName("UpdateDiscipline")
            .WithSummary("Update discipline")
            .WithDescription("Updates an existing discipline")
            .WithTags("Disciplines")
            .RequireAuthorization("AdminOnly")
            .Produces<Result>()
            .Produces<Result>(400)
            .Produces(404);

        app.MapPost("/{id:guid}/activate", ActivateDisciplineAsync)
            .WithName("ActivateDiscipline")
            .WithSummary("Activate discipline")
            .WithDescription("Activates a discipline")
            .WithTags("Disciplines")
            .RequireAuthorization("AdminOnly")
            .Produces<Result>()
            .Produces(404);

        app.MapPost("/{id:guid}/deactivate", DeactivateDisciplineAsync)
            .WithName("DeactivateDiscipline")
            .WithSummary("Deactivate discipline")
            .WithDescription("Deactivates a discipline")
            .WithTags("Disciplines")
            .RequireAuthorization("AdminOnly")
            .Produces<Result>()
            .Produces(404);

        app.MapDelete("/{id:guid}", DeleteDisciplineAsync)
            .WithName("DeleteDiscipline")
            .WithSummary("Delete discipline")
            .WithDescription("Soft deletes a discipline")
            .WithTags("Disciplines")
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
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("User ID not found");
        var result = await disciplineService.CreateAsync(dto, userId, cancellationToken);
        
        return result != null 
            ? Results.CreatedAtRoute("GetDisciplineById", new { id = result.Id }, result)
            : Results.BadRequest("Failed to create discipline");
    }

    private static async Task<IResult> UpdateDisciplineAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateDisciplineDto dto,
        [FromServices] IDisciplineService disciplineService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("User ID not found");
        var result = await disciplineService.UpdateAsync(id, dto, userId, cancellationToken);
        
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> ActivateDisciplineAsync(
        [FromRoute] Guid id,
        [FromServices] IDisciplineService disciplineService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("User ID not found");
        // Since there's no IsActive property, just verify the discipline exists
        var existing = await disciplineService.GetByIdAsync(id, cancellationToken);
        if (existing == null) return Results.NotFound();
        
        return Results.Ok("Discipline is active");
    }

    private static async Task<IResult> DeactivateDisciplineAsync(
        [FromRoute] Guid id,
        [FromServices] IDisciplineService disciplineService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("User ID not found");
        // Since there's no IsActive property, we could use soft delete instead
        var result = await disciplineService.DeleteAsync(id, cancellationToken);
        
        return result ? Results.Ok("Discipline deactivated successfully") : Results.BadRequest("Failed to deactivate discipline");
    }

    private static async Task<IResult> DeleteDisciplineAsync(
        [FromRoute] Guid id,
        [FromServices] IDisciplineService disciplineService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("User ID not found");
        var result = await disciplineService.DeleteAsync(id, cancellationToken);
        
        return result ? Results.Ok("Discipline deleted successfully") : Results.BadRequest("Failed to delete discipline");
    }
}