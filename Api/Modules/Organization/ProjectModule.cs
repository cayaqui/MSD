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
/// Endpoints for project management
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
            .WithSummary("Get all projects with pagination")
            .WithDescription("Returns a paginated list of projects based on user permissions")
            .WithTags("Projects")
            .Produces<PagedResult<ProjectListDto>>();

        app.MapGet("/{id:guid}", GetProjectByIdAsync)
            .WithName("GetProjectById")
            .WithSummary("Get project by ID")
            .WithDescription("Returns a specific project by ID with full details")
            .WithTags("Projects")
            .Produces<ProjectDto>()
            .Produces(404);

        app.MapGet("/summary", GetProjectsSummaryAsync)
            .WithName("GetProjectsSummary")
            .WithSummary("Get projects summary")
            .WithDescription("Returns a summary of all projects accessible to the user")
            .WithTags("Projects")
            .Produces<List<ProjectSummaryDto>>();

        app.MapGet("/{id:guid}/team", GetProjectTeamAsync)
            .WithName("GetProjectTeam")
            .WithSummary("Get project team members")
            .WithDescription("Returns all team members assigned to a project")
            .WithTags("Projects")
            .Produces<List<ProjectTeamMemberDetailDto>>()
            .Produces(404);

        app.MapGet("/{id:guid}/status-history", GetProjectStatusHistoryAsync)
            .WithName("GetProjectStatusHistory")
            .WithSummary("Get project status history")
            .WithDescription("Returns the status change history of a project")
            .WithTags("Projects")
            .Produces<List<ProjectStatusHistoryDto>>()
            .Produces(404);

        // Command endpoints
        app.MapPost("/", CreateProjectAsync)
            .WithName("CreateProject")
            .WithSummary("Create a new project")
            .WithDescription("Creates a new project")
            .WithTags("Projects")
            .RequireAuthorization("ProjectCreate")
            .Produces<Result<Guid>>(201)
            .Produces<Result>(400);

        app.MapPut("/{id:guid}", UpdateProjectAsync)
            .WithName("UpdateProject")
            .WithSummary("Update project")
            .WithDescription("Updates an existing project")
            .WithTags("Projects")
            .RequireAuthorization("ProjectEdit")
            .Produces<Result>()
            .Produces<Result>(400)
            .Produces(404);

        app.MapPut("/{id:guid}/status", ChangeProjectStatusAsync)
            .WithName("ChangeProjectStatus")
            .WithSummary("Change project status")
            .WithDescription("Changes the status of a project")
            .WithTags("Projects")
            .RequireAuthorization("ProjectStatusChange")
            .Produces<Result>()
            .Produces<Result>(400)
            .Produces(404);

        app.MapPost("/{id:guid}/hold", HoldProjectAsync)
            .WithName("HoldProject")
            .WithSummary("Put project on hold")
            .WithDescription("Puts a project on hold with a reason")
            .WithTags("Projects")
            .RequireAuthorization("ProjectStatusChange")
            .Produces<Result>()
            .Produces<Result>(400)
            .Produces(404);

        app.MapPost("/{id:guid}/cancel", CancelProjectAsync)
            .WithName("CancelProject")
            .WithSummary("Cancel project")
            .WithDescription("Cancels a project with a reason")
            .WithTags("Projects")
            .RequireAuthorization("ProjectCancel")
            .Produces<Result>()
            .Produces<Result>(400)
            .Produces(404);

        app.MapPost("/{id:guid}/complete", CompleteProjectAsync)
            .WithName("CompleteProject")
            .WithSummary("Complete project")
            .WithDescription("Marks a project as completed")
            .WithTags("Projects")
            .RequireAuthorization("ProjectComplete")
            .Produces<Result>()
            .Produces<Result>(400)
            .Produces(404);

        app.MapPut("/{id:guid}/progress", UpdateProjectProgressAsync)
            .WithName("UpdateProjectProgress")
            .WithSummary("Update project progress")
            .WithDescription("Updates the overall progress of a project")
            .WithTags("Projects")
            .RequireAuthorization("ProjectEdit")
            .Produces<Result>()
            .Produces<Result>(400)
            .Produces(404);

        app.MapDelete("/{id:guid}", DeleteProjectAsync)
            .WithName("DeleteProject")
            .WithSummary("Delete project")
            .WithDescription("Soft deletes a project")
            .WithTags("Projects")
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
        // This would need to be implemented in the service
        return Results.Ok(new List<ProjectTeamMemberDetailDto>());
    }

    private static async Task<IResult> GetProjectStatusHistoryAsync(
        [FromRoute] Guid id,
        [FromServices] IProjectService projectService,
        CancellationToken cancellationToken)
    {
        // This would need to be implemented in the service
        return Results.Ok(new List<ProjectStatusHistoryDto>());
    }

    private static async Task<IResult> CreateProjectAsync(
        [FromBody] CreateProjectDto dto,
        [FromServices] IProjectService projectService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("User ID not found");
        var result = await projectService.CreateAsync(dto, userId, cancellationToken);
        
        return result != null 
            ? Results.CreatedAtRoute("GetProjectById", new { id = result.Id }, result)
            : Results.BadRequest("Failed to create project");
    }

    private static async Task<IResult> UpdateProjectAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateProjectDto dto,
        [FromServices] IProjectService projectService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("User ID not found");
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
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("User ID not found");
        if (!Enum.TryParse<ProjectStatus>(dto.Status, out var status))
            return Results.BadRequest($"Invalid status: {dto.Status}");
        
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
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("User ID not found");
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
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("User ID not found");
        var result = await projectService.UpdateStatusAsync(id, ProjectStatus.Cancelled, userId, cancellationToken);
        
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> CompleteProjectAsync(
        [FromRoute] Guid id,
        [FromServices] IProjectService projectService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("User ID not found");
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
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("User ID not found");
        var result = await projectService.UpdateProgressAsync(id, dto, userId, cancellationToken);
        
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> DeleteProjectAsync(
        [FromRoute] Guid id,
        [FromServices] IProjectService projectService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("User ID not found");
        var result = await projectService.DeleteAsync(id, cancellationToken);
        
        return result ? Results.Ok("Project deleted successfully") : Results.BadRequest("Failed to delete project");
    }
}