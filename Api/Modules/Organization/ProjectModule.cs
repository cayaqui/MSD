using Api.Authorization;
using Application.Interfaces.Organization;
using Application.Interfaces.Auth;
using Carter;
using Core.DTOs.Common;
using Core.DTOs.Organization.Project;
using Core.DTOs.Auth.ProjectTeamMembers;
using Core.Enums.Projects;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Organization;

/// <summary>
/// Endpoints para gestión de proyectos
/// </summary>
public class ProjectModule : CarterModule
{
    public ProjectModule() : base("/api/projects")
    {
        RequireAuthorization();
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        // Query endpoints
        app.MapGet("/", GetProjectsAsync)
            .WithName("GetProjects")
            .WithSummary("Obtener todos los proyectos con paginación")
            .WithDescription("Retorna una lista paginada de proyectos basada en permisos del usuario")
            .WithTags("Proyectos")
            .Produces<PagedResult<ProjectListDto>>();

        app.MapGet("/{id:guid}", GetProjectByIdAsync)
            .WithName("GetProjectById")
            .WithSummary("Obtener proyecto por ID")
            .WithDescription("Retorna un proyecto específico por ID con detalles completos")
            .WithTags("Proyectos")
            .Produces<ProjectDto>()
            .Produces(404);

        app.MapGet("/summary", GetProjectsSummaryAsync)
            .WithName("GetProjectsSummary")
            .WithSummary("Obtener resumen de proyectos")
            .WithDescription("Retorna un resumen de todos los proyectos accesibles para el usuario")
            .WithTags("Proyectos")
            .Produces<List<ProjectSummaryDto>>();

        app.MapGet("/{id:guid}/team", GetProjectTeamAsync)
            .WithName("GetProjectTeam")
            .WithSummary("Obtener miembros del equipo del proyecto")
            .WithDescription("Retorna todos los miembros del equipo asignados a un proyecto")
            .WithTags("Proyectos")
            .Produces<List<ProjectTeamMemberDetailDto>>()
            .Produces(404);

        app.MapGet("/{id:guid}/status-history", GetProjectStatusHistoryAsync)
            .WithName("GetProjectStatusHistory")
            .WithSummary("Obtener historial de estado del proyecto")
            .WithDescription("Retorna el historial de cambios de estado de un proyecto")
            .WithTags("Proyectos")
            .Produces<List<ProjectStatusHistoryDto>>()
            .Produces(404);

        // Command endpoints
        app.MapPost("/", CreateProjectAsync)
            .WithName("CreateProject")
            .WithSummary("Crear un nuevo proyecto")
            .WithDescription("Crea un nuevo proyecto")
            .WithTags("Proyectos")
            .RequireAuthorization("ProjectCreate")
            .Produces<Result<Guid>>(201)
            .Produces<Result>(400);

        app.MapPut("/{id:guid}", UpdateProjectAsync)
            .WithName("UpdateProject")
            .WithSummary("Actualizar proyecto")
            .WithDescription("Actualiza un proyecto existente")
            .WithTags("Proyectos")
            .RequireAuthorization("ProjectEdit")
            .Produces<Result>()
            .Produces<Result>(400)
            .Produces(404);

        app.MapPut("/{id:guid}/status", ChangeProjectStatusAsync)
            .WithName("ChangeProjectStatus")
            .WithSummary("Cambiar estado del proyecto")
            .WithDescription("Cambia el estado de un proyecto")
            .WithTags("Proyectos")
            .RequireAuthorization("ProjectStatusChange")
            .Produces<Result>()
            .Produces<Result>(400)
            .Produces(404);

        app.MapPost("/{id:guid}/hold", HoldProjectAsync)
            .WithName("HoldProject")
            .WithSummary("Poner proyecto en espera")
            .WithDescription("Pone un proyecto en espera con una razón")
            .WithTags("Proyectos")
            .RequireAuthorization("ProjectStatusChange")
            .Produces<Result>()
            .Produces<Result>(400)
            .Produces(404);

        app.MapPost("/{id:guid}/cancel", CancelProjectAsync)
            .WithName("CancelProject")
            .WithSummary("Cancelar proyecto")
            .WithDescription("Cancela un proyecto con una razón")
            .WithTags("Proyectos")
            .RequireAuthorization("ProjectCancel")
            .Produces<Result>()
            .Produces<Result>(400)
            .Produces(404);

        app.MapPost("/{id:guid}/complete", CompleteProjectAsync)
            .WithName("CompleteProject")
            .WithSummary("Completar proyecto")
            .WithDescription("Marca un proyecto como completado")
            .WithTags("Proyectos")
            .RequireAuthorization("ProjectComplete")
            .Produces<Result>()
            .Produces<Result>(400)
            .Produces(404);

        app.MapPut("/{id:guid}/progress", UpdateProjectProgressAsync)
            .WithName("UpdateProjectProgress")
            .WithSummary("Actualizar progreso del proyecto")
            .WithDescription("Actualiza el progreso general de un proyecto")
            .WithTags("Proyectos")
            .RequireAuthorization("ProjectEdit")
            .Produces<Result>()
            .Produces<Result>(400)
            .Produces(404);

        app.MapDelete("/{id:guid}", DeleteProjectAsync)
            .WithName("DeleteProject")
            .WithSummary("Eliminar proyecto")
            .WithDescription("Elimina lógicamente un proyecto")
            .WithTags("Proyectos")
            .RequireAuthorization("ProjectDelete")
            .Produces<Result>()
            .Produces<Result>(400)
            .Produces(404);
    }

    private static async Task<IResult> GetProjectsAsync(
        [FromServices] IProjectService projectService,
        [AsParameters] SimpleQueryParameters parameters,
        CancellationToken cancellationToken)
    {
        var result = await projectService.GetAllPagedAsync(parameters.PageNumber, parameters.PageSize, parameters.SortBy, !parameters.IsAscending, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetProjectByIdAsync(
        [FromRoute] Guid id,
        [FromServices] IProjectService projectService,
        CancellationToken cancellationToken)
    {
        var result = await projectService.GetByIdAsync(id, cancellationToken);
        return result is not null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> GetProjectsSummaryAsync(
        [FromServices] IProjectService projectService,
        CancellationToken cancellationToken)
    {
        // Get all projects and map to summary
        var projects = await projectService.GetAllAsync(cancellationToken);
        var summaries = projects.Select(p => new ProjectSummaryDto
        {
            Id = p.Id,
            Code = p.Code,
            Name = p.Name,
            Status = p.Status,
            ProgressPercentage = p.ProgressPercentage,
            PlannedStartDate = p.PlannedStartDate,
            PlannedEndDate = p.PlannedEndDate,
            WBSCode = p.WBSCode,
            OperationId = p.OperationId,
            OperationName = p.OperationName,
            TotalBudget = p.TotalBudget,
            Currency = p.Currency,
            IsActive = p.IsActive
        }).ToList();
        return Results.Ok(summaries);
    }

    private static async Task<IResult> GetProjectTeamAsync(
        [FromRoute] Guid id,
        [FromServices] IProjectService projectService,
        CancellationToken cancellationToken)
    {
        // Esto necesitaría ser implementado en el servicio
        return Results.Ok(new List<ProjectTeamMemberDetailDto>());
    }

    private static async Task<IResult> GetProjectStatusHistoryAsync(
        [FromRoute] Guid id,
        [FromServices] IProjectService projectService,
        CancellationToken cancellationToken)
    {
        // Esto necesitaría ser implementado en el servicio
        return Results.Ok(new List<ProjectStatusHistoryDto>());
    }

    private static async Task<IResult> CreateProjectAsync(
        [FromBody] CreateProjectDto dto,
        [FromServices] IProjectService projectService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        var result = await projectService.CreateAsync(dto, userId, cancellationToken);
        
        return result != null 
            ? Results.CreatedAtRoute("GetProjectById", new { id = result.Id }, result)
            : Results.BadRequest("Error al crear el proyecto");
    }

    private static async Task<IResult> UpdateProjectAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateProjectDto dto,
        [FromServices] IProjectService projectService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        var result = await projectService.UpdateAsync(id, dto, userId, cancellationToken);
        
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> ChangeProjectStatusAsync(
        [FromRoute] Guid id,
        [FromBody] ChangeProjectStatusDto dto,
        [FromServices] IProjectService projectService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        if (!Enum.TryParse<ProjectStatus>(dto.Status, out var status))
            return Results.BadRequest($"Estado inválido: {dto.Status}");
        
        var result = await projectService.UpdateStatusAsync(id, status, userId, cancellationToken);
        
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> HoldProjectAsync(
        [FromRoute] Guid id,
        [FromBody] HoldProjectDto dto,
        [FromServices] IProjectService projectService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        var result = await projectService.UpdateStatusAsync(id, ProjectStatus.OnHold, userId, cancellationToken);
        
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> CancelProjectAsync(
        [FromRoute] Guid id,
        [FromBody] CancelProjectDto dto,
        [FromServices] IProjectService projectService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        var result = await projectService.UpdateStatusAsync(id, ProjectStatus.Cancelled, userId, cancellationToken);
        
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> CompleteProjectAsync(
        [FromRoute] Guid id,
        [FromServices] IProjectService projectService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        var result = await projectService.UpdateStatusAsync(id, ProjectStatus.Completed, userId, cancellationToken);
        
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> UpdateProjectProgressAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateProjectProgressDto dto,
        [FromServices] IProjectService projectService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        var result = await projectService.UpdateProgressAsync(id, dto, userId, cancellationToken);
        
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> DeleteProjectAsync(
        [FromRoute] Guid id,
        [FromServices] IProjectService projectService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        var result = await projectService.DeleteAsync(id, cancellationToken);
        
        return result ? Results.Ok("Proyecto eliminado exitosamente") : Results.BadRequest("Error al eliminar el proyecto");
    }
}