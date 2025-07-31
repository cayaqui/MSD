using Application.Common.Exceptions;
using Application.Interfaces.Setup;
using Carter;
using Carter.ModelBinding;
using Core.DTOs.Auth;
using Core.DTOs.Common;
using Core.DTOs.Projects;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ValidationProblemDetails = Core.DTOs.Common.ValidationProblemDetails;

namespace API.Modules;

public class ProjectsModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/projects")
            .WithTags("Projects")
            .RequireAuthorization();

        // Get all projects (simplified) - Returns ProjectListDto
        group.MapGet("", GetProjects)
            .WithName("GetProjects")
            .WithSummary("Get all projects")
            .Produces<IEnumerable<ProjectListDto>>(200);

        // Search projects with pagination
        group.MapPost("search", SearchProjects)
            .WithName("SearchProjects")
            .WithSummary("Search projects with filters and pagination")
            .Produces<PagedResult<ProjectListDto>>(200);

        // Get active projects - Returns ProjectListDto
        group.MapGet("active", GetActiveProjects)
            .WithName("GetActiveProjects")
            .WithSummary("Get active projects only")
            .Produces<IEnumerable<ProjectListDto>>(200);

        // Get my projects
        group.MapGet("my-projects", GetMyProjects)
            .WithName("GetMyProjects")
            .WithSummary("Get current user's projects")
            .Produces<IEnumerable<ProjectListDto>>(200);

        // Get project by ID - Returns full ProjectDto
        group.MapGet("{id:guid}", GetProjectById)
            .WithName("GetProjectById")
            .WithSummary("Get project by ID")
            .Produces<ProjectDto>(200)
            .Produces(403)
            .Produces(404);

        // Get project summary
        group.MapGet("{id:guid}/summary", GetProjectSummary)
            .WithName("GetProjectSummary")
            .WithSummary("Get project summary with statistics")
            .Produces<ProjectSummaryDto>(200)
            .Produces(403)
            .Produces(404);

        // Create project
        group.MapPost("", CreateProject)
            .WithName("CreateProject")
            .WithSummary("Create a new project")
            .Produces<ProjectDto>(201)
            .Produces<ValidationProblemDetails>(400);

        // Update project
        group.MapPut("{id:guid}", UpdateProject)
            .WithName("UpdateProject")
            .WithSummary("Update project")
            .Produces<ProjectDto>(200)
            .Produces<ValidationProblemDetails>(400)
            .Produces(403)
            .Produces(404);

        // Delete project
        group.MapDelete("{id:guid}", DeleteProject)
            .WithName("DeleteProject")
            .WithSummary("Delete project")
            .Produces(204)
            .Produces(403)
            .Produces(404);

        // Project status operations
        group.MapPost("{id:guid}/start", StartProject)
            .WithName("StartProject")
            .WithSummary("Start a project")
            .Produces<ProjectDto>(200)
            .Produces(400)
            .Produces(403)
            .Produces(404);

        group.MapPost("{id:guid}/complete", CompleteProject)
            .WithName("CompleteProject")
            .WithSummary("Complete a project")
            .Produces<ProjectDto>(200)
            .Produces(400)
            .Produces(403)
            .Produces(404);

        group.MapPost("{id:guid}/hold", PutProjectOnHold)
            .WithName("PutProjectOnHold")
            .WithSummary("Put project on hold")
            .Produces<ProjectDto>(200)
            .Produces(400)
            .Produces(403)
            .Produces(404);

        group.MapPost("{id:guid}/cancel", CancelProject)
            .WithName("CancelProject")
            .WithSummary("Cancel a project")
            .Produces<ProjectDto>(200)
            .Produces(400)
            .Produces(403)
            .Produces(404);

        // Progress update
        group.MapPatch("{id:guid}/progress", UpdateProjectProgress)
            .WithName("UpdateProjectProgress")
            .WithSummary("Update project progress")
            .Produces<ProjectDto>(200)
            .Produces(403)
            .Produces(404);

        // Team management
        group.MapGet("{id:guid}/team", GetProjectTeamMembers)
            .WithName("GetProjectTeamMembers")
            .WithSummary("Get project team members")
            .Produces<IEnumerable<ProjectTeamMemberDto>>(200);

        group.MapPost("{id:guid}/team", AssignProjectTeamMember)
            .WithName("AssignProjectTeamMember")
            .WithSummary("Assign team member to project")
            .Produces(200)
            .Produces(400)
            .Produces(403);

        group.MapDelete("{id:guid}/team/{userId:guid}", RemoveProjectTeamMember)
            .WithName("RemoveProjectTeamMember")
            .WithSummary("Remove team member from project")
            .Produces(204)
            .Produces(403)
            .Produces(404);

        // Check code uniqueness
        group.MapGet("check-code/{code}", CheckProjectCode)
            .WithName("CheckProjectCode")
            .WithSummary("Check if project code is unique")
            .Produces<bool>(200);
    }

    // Cambia la declaración del método para que devuelva Ok<IReadOnlyList<ProjectListDto>> en vez de Ok<IEnumerable<ProjectListDto>>
    private static async Task<Ok<IReadOnlyList<ProjectListDto>>> GetProjects(
        IProjectService projectService)
    {
        var result = await projectService.GetPagedAsync(1, 100);
        return TypedResults.Ok(result.Items);
    }

    private static async Task<Ok<PagedResult<ProjectListDto>>> SearchProjects(
        [FromBody] ProjectFilterDto filter,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        [FromServices] IProjectService projectService)
    {
        pageNumber = pageNumber > 0 ? pageNumber : 1;
        pageSize = pageSize > 0 ? pageSize : 20;
        pageSize = Math.Min(pageSize, 100); // Max 100 items per page

        var result = await projectService.GetPagedAsync(pageNumber, pageSize, filter);
        return TypedResults.Ok(result);
    }

    // Cambia la declaración del método para que devuelva Ok<IReadOnlyList<ProjectListDto>> en vez de Ok<IEnumerable<ProjectListDto>>
    private static async Task<Ok<IReadOnlyList<ProjectListDto>>> GetActiveProjects(
        IProjectService projectService)
    {
        var filter = new ProjectFilterDto { IsActive = true };
        var result = await projectService.GetPagedAsync(1, 100, filter);
        return TypedResults.Ok(result.Items);
    }

    private static async Task<Ok<IReadOnlyList<ProjectListDto>>> GetMyProjects(
        IProjectService projectService)
    {
        var projects = await projectService.GetUserProjectsAsync();
        // Asegura la conversión a IReadOnlyList<ProjectListDto>
        var list = projects is IReadOnlyList<ProjectListDto> readOnlyList
            ? readOnlyList
            : projects.ToList();
        return TypedResults.Ok(list);
    }

    private static async Task<Results<Ok<ProjectDto>, NotFound, ForbidHttpResult>> GetProjectById(
        Guid id,
        IProjectService projectService)
    {
        try
        {
            var project = await projectService.GetByIdAsync(id);
            return project != null
                ? TypedResults.Ok(project)
                : TypedResults.NotFound();
        }
        catch (ForbiddenAccessException)
        {
            return TypedResults.Forbid();
        }
    }

    private static async Task<Results<Ok<ProjectSummaryDto>, NotFound, ForbidHttpResult>> GetProjectSummary(
        Guid id,
        IProjectService projectService)
    {
        try
        {
            // This would be implemented in ProjectService
            var project = await projectService.GetByIdAsync(id);
            if (project == null)
                return TypedResults.NotFound();

            // Map to summary DTO with calculated fields
            var summary = new ProjectSummaryDto
            {
                Id = project.Id,
                Code = project.Code,
                Name = project.Name,
                Description = project.Description,
                Status = project.Status,
                PlannedStartDate = project.PlannedStartDate,
                PlannedEndDate = project.PlannedEndDate,
                TotalBudget = project.TotalBudget,
                Currency = project.Currency,
                ProgressPercentage = project.ProgressPercentage,
                // Add calculated fields
                DaysRemaining = (project.PlannedEndDate - DateTime.UtcNow).Days,
                IsOverdue = DateTime.UtcNow > project.PlannedEndDate && project.Status != "Completed"
            };

            return TypedResults.Ok(summary);
        }
        catch (ForbiddenAccessException)
        {
            return TypedResults.Forbid();
        }
    }

    private static async Task<Results<Created<ProjectDto>, ValidationProblem>> CreateProject(
        CreateProjectDto dto,
        IProjectService projectService,
        IValidator<CreateProjectDto> validator)
    {
        var validationResult = await validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var project = await projectService.CreateAsync(dto);
        return TypedResults.Created($"/api/projects/{project.Id}", project);
    }

    private static async Task<Results<Ok<ProjectDto>, NotFound, ValidationProblem, ForbidHttpResult>> UpdateProject(
        Guid id,
        UpdateProjectDto dto,
        IProjectService projectService,
        IValidator<UpdateProjectDto> validator)
    {
        var validationResult = await validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        try
        {
            var project = await projectService.UpdateAsync(id, dto);
            return project != null
                ? TypedResults.Ok(project)
                : TypedResults.NotFound();
        }
        catch (ForbiddenAccessException)
        {
            return TypedResults.Forbid();
        }
    }

    private static async Task<Results<NoContent, NotFound, ForbidHttpResult>> DeleteProject(
        Guid id,
        IProjectService projectService)
    {
        try
        {
            await projectService.DeleteAsync(id);
            return TypedResults.NoContent();
        }
        catch (NotFoundException)
        {
            return TypedResults.NotFound();
        }
        catch (ForbiddenAccessException)
        {
            return TypedResults.Forbid();
        }
    }

    private static async Task<Results<Ok<ProjectDto>, BadRequest<string>, NotFound, ForbidHttpResult>> StartProject(
        Guid id,
        IProjectService projectService)
    {
        try
        {
            // Implementation would be in ProjectService
            var dto = new UpdateProjectDto { Status = "Active" };
            var project = await projectService.UpdateAsync(id, dto);
            return project != null
                ? TypedResults.Ok(project)
                : TypedResults.NotFound();
        }
        catch (ForbiddenAccessException)
        {
            return TypedResults.Forbid();
        }
        catch (BadRequestException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }

    private static async Task<Results<Ok<ProjectDto>, BadRequest<string>, NotFound, ForbidHttpResult>> CompleteProject(
        Guid id,
        IProjectService projectService)
    {
        try
        {
            var dto = new UpdateProjectDto { Status = "Completed" };
            var project = await projectService.UpdateAsync(id, dto);
            return project != null
                ? TypedResults.Ok(project)
                : TypedResults.NotFound();
        }
        catch (ForbiddenAccessException)
        {
            return TypedResults.Forbid();
        }
        catch (BadRequestException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }

    private static async Task<Results<Ok<ProjectDto>, BadRequest<string>, NotFound, ForbidHttpResult>> PutProjectOnHold(
        Guid id,
        HoldProjectDto request,
        IProjectService projectService)
    {
        try
        {
            var dto = new UpdateProjectDto
            {
                Status = "OnHold",
                Description = $"On Hold: {request.Reason}"
            };
            var project = await projectService.UpdateAsync(id, dto);
            return project != null
                ? TypedResults.Ok(project)
                : TypedResults.NotFound();
        }
        catch (ForbiddenAccessException)
        {
            return TypedResults.Forbid();
        }
        catch (BadRequestException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }

    private static async Task<Results<Ok<ProjectDto>, BadRequest<string>, NotFound, ForbidHttpResult>> CancelProject(
        Guid id,
        CancelProjectDto request,
        IProjectService projectService)
    {
        try
        {
            var dto = new UpdateProjectDto
            {
                Status = "Cancelled",
                Description = $"Cancelled: {request.Reason}"
            };
            var project = await projectService.UpdateAsync(id, dto);
            return project != null
                ? TypedResults.Ok(project)
                : TypedResults.NotFound();
        }
        catch (ForbiddenAccessException)
        {
            return TypedResults.Forbid();
        }
        catch (BadRequestException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }

    private static async Task<Results<Ok<ProjectDto>, NotFound, ForbidHttpResult>> UpdateProjectProgress(
        Guid id,
        UpdateProjectProgressDto request,
        IProjectService projectService)
    {
        try
        {
            await projectService.UpdateProgressAsync(id, request.ProgressPercentage);
            var project = await projectService.GetByIdAsync(id);
            return project != null
                ? TypedResults.Ok(project)
                : TypedResults.NotFound();
        }
        catch (ForbiddenAccessException)
        {
            return TypedResults.Forbid();
        }
    }

    private static async Task<Ok<IEnumerable<ProjectTeamMemberDto>>> GetProjectTeamMembers(
        Guid id,
        IProjectService projectService)
    {
        // This would be implemented in ProjectService
        return TypedResults.Ok(Enumerable.Empty<ProjectTeamMemberDto>());
    }

    private static async Task<Results<Ok, BadRequest<string>, ForbidHttpResult>> AssignProjectTeamMember(
        Guid id,
        AssignProjectTeamMemberDto request,
        IProjectService projectService)
    {
        try
        {
            await projectService.AddTeamMemberAsync(id, request.UserId, request.Role);
            return TypedResults.Ok();
        }
        catch (ForbiddenAccessException)
        {
            return TypedResults.Forbid();
        }
        catch (BadRequestException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }

    private static async Task<Results<NoContent, NotFound, ForbidHttpResult>> RemoveProjectTeamMember(
        Guid id,
        Guid userId,
        IProjectService projectService)
    {
        try
        {
            await projectService.RemoveTeamMemberAsync(id, userId);
            return TypedResults.NoContent();
        }
        catch (NotFoundException)
        {
            return TypedResults.NotFound();
        }
        catch (ForbiddenAccessException)
        {
            return TypedResults.Forbid();
        }
    }

    private static async Task<Ok<bool>> CheckProjectCode(
        string code,
        Guid? excludeId,
        IProjectService projectService)
    {
        var isUnique = await projectService.IsCodeUniqueAsync(code, excludeId);
        return TypedResults.Ok(isUnique);
    }
}