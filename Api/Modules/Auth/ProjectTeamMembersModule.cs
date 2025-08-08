using Api.Authorization;
using Application.Interfaces.Auth;
using Carter;
using Core.Constants;
using Core.DTOs.Auth.ProjectTeamMembers;
using Core.DTOs.Common;
using Core.DTOs.Organization.Project;
using Microsoft.AspNetCore.Authorization;

namespace Api.Modules;

/// <summary>
/// Endpoints de gestión de miembros del equipo de proyecto
/// </summary>
public class ProjectTeamMembersModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var teamMembers = app.MapGroup("/api/project-team-members")
            .WithTags("Miembros del Equipo de Proyecto")
            .RequireAuthorization();

        // Read operations
        teamMembers.MapPost("/search", SearchTeamMembers)
            .WithName("SearchTeamMembers")
            .WithSummary("Buscar miembros del equipo con filtros")
            .Produces<PagedResult<ProjectTeamMemberDetailDto>>(200);

        teamMembers.MapGet("/{id:guid}", GetTeamMemberById)
            .WithName("GetTeamMemberById")
            .WithSummary("Obtener miembro del equipo por ID")
            .Produces<ProjectTeamMemberDetailDto>(200)
            .Produces(404);

        teamMembers.MapGet("/project/{projectId:guid}", GetProjectTeamMembers)
            .WithName("GetProjectTeamMembers")
            .WithSummary("Obtener todos los miembros del equipo para un proyecto")
            .Produces<IEnumerable<ProjectTeamMemberDetailDto>>(200);

        teamMembers.MapGet("/user/{userId:guid}", GetUserAssignments)
            .WithName("GetUserAssignments")
            .WithSummary("Obtener todas las asignaciones de proyecto para un usuario")
            .Produces<IEnumerable<ProjectTeamMemberDetailDto>>(200);

        // Assignment management
        teamMembers.MapPost("/project/{projectId:guid}/assign", AssignTeamMember)
            .WithName("AssignTeamMember")
            .WithSummary("Asignar un miembro del equipo a un proyecto")
            .Produces<ProjectTeamMemberDetailDto>(201)
            .Produces(400)
            .RequireAuthorization(policy => policy.RequireRoleOrAdmin(SimplifiedRoles.Project.ProjectManager));

        teamMembers.MapPut("/{id:guid}", UpdateTeamMember)
            .WithName("UpdateTeamMember")
            .WithSummary("Actualizar detalles del miembro del equipo")
            .Produces<ProjectTeamMemberDetailDto>(200)
            .Produces(404)
            .Produces(400)
            .RequireAuthorization(policy => policy.RequireRoleOrAdmin(SimplifiedRoles.Project.ProjectManager));

        teamMembers.MapDelete("/{id:guid}", RemoveTeamMember)
            .WithName("RemoveTeamMember")
            .WithSummary("Eliminar miembro del equipo del proyecto")
            .Produces(204)
            .Produces(404)
            .RequireAuthorization(policy => policy.RequireRoleOrAdmin(SimplifiedRoles.Project.ProjectManager));

        // Bulk operations
        teamMembers.MapPost("/bulk-assign", BulkAssignTeamMembers)
            .WithName("BulkAssignTeamMembers")
            .WithSummary("Asignar múltiples miembros del equipo a proyectos")
            .Produces<BulkAssignResult>(200)
            .RequireAuthorization(policy => policy.RequireRoleOrAdmin(SimplifiedRoles.Project.ProjectManager));

        teamMembers.MapDelete("/project/{projectId:guid}/all", RemoveAllFromProject)
            .WithName("RemoveAllFromProject")
            .WithSummary("Eliminar todos los miembros del equipo de un proyecto")
            .Produces<int>(200)
            .RequireAuthorization(policy => policy.RequireRoleOrAdmin(SimplifiedRoles.Project.ProjectManager));

        // Allocation management
        teamMembers.MapGet("/user/{userId:guid}/availability", GetUserAvailability)
            .WithName("GetUserAvailability")
            .WithSummary("Obtener disponibilidad y asignación del usuario")
            .Produces<UserAvailabilityDto>(200)
            .Produces(404);

        teamMembers.MapGet("/allocation-report", GetAllocationReport)
            .WithName("GetAllocationReport")
            .WithSummary("Obtener reporte de asignación del equipo")
            .Produces<IEnumerable<TeamAllocationReportDto>>(200);

        teamMembers.MapPut("/{id:guid}/allocation", UpdateAllocation)
            .WithName("UpdateAllocation")
            .WithSummary("Actualizar porcentaje de asignación del miembro del equipo")
            .Produces<ProjectTeamMemberDetailDto>(200)
            .Produces(404)
            .RequireAuthorization(policy => policy.RequireRoleOrAdmin(SimplifiedRoles.Project.ProjectManager));

        // Transfer and extension
        teamMembers.MapPost("/{id:guid}/transfer", TransferTeamMember)
            .WithName("TransferTeamMember")
            .WithSummary("Transferir miembro del equipo a otro proyecto")
            .Produces<ProjectTeamMemberDetailDto>(200)
            .Produces(404)
            .RequireAuthorization(policy => policy.RequireRoleOrAdmin(SimplifiedRoles.Project.ProjectManager));

        teamMembers.MapPost("/{id:guid}/extend", ExtendAssignment)
            .WithName("ExtendAssignment")
            .WithSummary("Extender fecha de fin de asignación del miembro del equipo")
            .Produces<ProjectTeamMemberDetailDto>(200)
            .Produces(404)
            .RequireAuthorization(policy => policy.RequireRoleOrAdmin(SimplifiedRoles.Project.ProjectManager));

        // Validation endpoints
        teamMembers.MapGet("/check-assignment/{userId:guid}/{projectId:guid}", CheckUserAssignment)
            .WithName("CheckUserAssignment")
            .WithSummary("Verificar si el usuario está asignado al proyecto")
            .Produces<bool>(200);

        teamMembers.MapPost("/can-assign", CanAssignUser)
            .WithName("CanAssignUser")
            .WithSummary("Verificar si el usuario puede ser asignado al proyecto")
            .Produces<AssignmentValidationResult>(200);

        teamMembers.MapGet("/check-role/{userId:guid}/{projectId:guid}/{role}", CheckUserRole)
            .WithName("CheckUserRole")
            .WithSummary("Verificar si el usuario tiene un rol específico en el proyecto")
            .Produces<bool>(200);
    }

    private static async Task<IResult> SearchTeamMembers(
        ProjectTeamMemberFilterDto filter,
        IProjectTeamMemberService teamMemberService)
    {
        var result = await teamMemberService.GetPagedAsync(filter);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetTeamMemberById(
        Guid id,
        IProjectTeamMemberService teamMemberService)
    {
        var member = await teamMemberService.GetByIdAsync(id);
        return member != null ? Results.Ok(member) : Results.NotFound($"Miembro del equipo {id} no encontrado");
    }

    private static async Task<IResult> GetProjectTeamMembers(
        Guid projectId,
        IProjectTeamMemberService teamMemberService,
        ICurrentUserService currentUserService)
    {
        // Check if user has access to view project team
        if (!await currentUserService.HasProjectAccessAsync(projectId))
        {
            return Results.Forbid();
        }

        var members = await teamMemberService.GetByProjectAsync(projectId);
        return Results.Ok(members);
    }

    private static async Task<IResult> GetUserAssignments(
        Guid userId,
        IProjectTeamMemberService teamMemberService,
        ICurrentUserService currentUserService)
    {
        // Users can view their own assignments, managers can view any
        if (currentUserService.UserId != userId.ToString() && 
            !await currentUserService.HasPermissionAsync(PermissionConstants.Projects.ViewTeam))
        {
            return Results.Forbid();
        }

        var assignments = await teamMemberService.GetByUserAsync(userId);
        return Results.Ok(assignments);
    }

    private static async Task<IResult> AssignTeamMember(
        Guid projectId,
        AssignProjectTeamMemberDto dto,
        IProjectTeamMemberService teamMemberService,
        ICurrentUserService currentUserService)
    {
        try
        {
            // Check if user has permission to manage team for this project
            if (!await currentUserService.HasProjectAccessAsync(projectId, SimplifiedRoles.Project.ProjectManager))
            {
                return Results.Forbid();
            }

            // Validate assignment is possible
            if (!await teamMemberService.CanUserBeAssignedAsync(dto.UserId, projectId, dto.AllocationPercentage))
            {
                return Results.BadRequest("El usuario no puede ser asignado a este proyecto. Verifique la asignación y las asignaciones existentes.");
            }

            var member = await teamMemberService.CreateAsync(projectId, dto);
            return Results.Created($"/api/project-team-members/{member.Id}", member);
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "Asignación fallida",
                detail: ex.Message,
                statusCode: 400);
        }
    }

    private static async Task<IResult> UpdateTeamMember(
        Guid id,
        UpdateProjectTeamMemberDto dto,
        IProjectTeamMemberService teamMemberService,
        ICurrentUserService currentUserService)
    {
        try
        {
            var existing = await teamMemberService.GetByIdAsync(id);
            if (existing == null)
            {
                return Results.NotFound($"Miembro del equipo {id} no encontrado");
            }

            // Check permission for the project
            if (!await currentUserService.HasProjectAccessAsync(existing.ProjectId, SimplifiedRoles.Project.ProjectManager))
            {
                return Results.Forbid();
            }

            var updated = await teamMemberService.UpdateAsync(id, dto);
            return updated != null ? Results.Ok(updated) : Results.NotFound();
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "Actualización fallida",
                detail: ex.Message,
                statusCode: 400);
        }
    }

    private static async Task<IResult> RemoveTeamMember(
        Guid id,
        IProjectTeamMemberService teamMemberService,
        ICurrentUserService currentUserService)
    {
        var existing = await teamMemberService.GetByIdAsync(id);
        if (existing == null)
        {
            return Results.NotFound($"Miembro del equipo {id} no encontrado");
        }

        // Check permission for the project
        if (!await currentUserService.HasProjectAccessAsync(existing.ProjectId, SimplifiedRoles.Project.ProjectManager))
        {
            return Results.Forbid();
        }

        await teamMemberService.RemoveAsync(id);
        return Results.NoContent();
    }

    private static async Task<IResult> BulkAssignTeamMembers(
        BulkAssignProjectTeamDto dto,
        IProjectTeamMemberService teamMemberService,
        ICurrentUserService currentUserService)
    {
        try
        {
            // Check permission for all projects
            foreach (var assignment in dto.Assignments)
            {
                if (!await currentUserService.HasProjectAccessAsync(assignment.ProjectId, SimplifiedRoles.Project.ProjectManager))
                {
                    return Results.Problem($"Sin permiso para gestionar el equipo del proyecto {assignment.ProjectId.ToString()}");
                }
            }

            var count = await teamMemberService.BulkAssignAsync(dto);
            return Results.Ok(new BulkAssignResult(count, dto.Assignments.Count));
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "Asignación masiva fallida",
                detail: ex.Message,
                statusCode: 400);
        }
    }

    private static async Task<IResult> RemoveAllFromProject(
        Guid projectId,
        IProjectTeamMemberService teamMemberService,
        ICurrentUserService currentUserService)
    {
        // Check permission
        if (!await currentUserService.HasProjectAccessAsync(projectId, SimplifiedRoles.Project.ProjectManager))
        {
            return Results.Forbid();
        }

        var count = await teamMemberService.RemoveAllFromProjectAsync(projectId);
        return Results.Ok(count);
    }

    private static async Task<IResult> GetUserAvailability(
        Guid userId,
        [AsParameters] AvailabilityQuery query,
        IProjectTeamMemberService teamMemberService)
    {
        var availability = await teamMemberService.GetUserAvailabilityAsync(
            userId, 
            query.StartDate, 
            query.EndDate);
            
        return availability != null ? Results.Ok(availability) : Results.NotFound($"Usuario {userId} no encontrado");
    }

    private static async Task<IResult> GetAllocationReport(
        [AsParameters] AllocationReportQuery query,
        IProjectTeamMemberService teamMemberService,
        ICurrentUserService currentUserService)
    {
        // If filtering by project, check access
        if (query.ProjectId.HasValue && !await currentUserService.HasProjectAccessAsync(query.ProjectId.Value))
        {
            return Results.Forbid();
        }

        var report = await teamMemberService.GetAllocationReportAsync(
            query.Date, 
            query.ProjectId, 
            query.UserId);
            
        return Results.Ok(report);
    }

    private static async Task<IResult> UpdateAllocation(
        Guid id,
        UpdateAllocationRequest request,
        IProjectTeamMemberService teamMemberService,
        ICurrentUserService currentUserService)
    {
        var existing = await teamMemberService.GetByIdAsync(id);
        if (existing == null)
        {
            return Results.NotFound($"Miembro del equipo {id} no encontrado");
        }

        // Check permission
        if (!await currentUserService.HasProjectAccessAsync(existing.ProjectId, SimplifiedRoles.Project.ProjectManager))
        {
            return Results.Forbid();
        }

        if (request.AllocationPercentage < 0 || request.AllocationPercentage > 100)
        {
            return Results.BadRequest("El porcentaje de asignación debe estar entre 0 y 100");
        }

        var updated = await teamMemberService.UpdateAllocationAsync(id, request.AllocationPercentage);
        return updated != null ? Results.Ok(updated) : Results.NotFound();
    }

    private static async Task<IResult> TransferTeamMember(
        Guid id,
        TransferTeamMemberDto dto,
        IProjectTeamMemberService teamMemberService,
        ICurrentUserService currentUserService)
    {
        try
        {
            var existing = await teamMemberService.GetByIdAsync(id);
            if (existing == null)
            {
                return Results.NotFound($"Miembro del equipo {id} no encontrado");
            }

            // Check permission for both source and target projects
            if (!await currentUserService.HasProjectAccessAsync(existing.ProjectId, SimplifiedRoles.Project.ProjectManager) ||
                !await currentUserService.HasProjectAccessAsync(dto.NewProjectId, SimplifiedRoles.Project.ProjectManager))
            {
                return Results.Forbid();
            }

            var transferred = await teamMemberService.TransferAsync(id, dto);
            return transferred != null ? Results.Ok(transferred) : Results.NotFound();
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "Transferencia fallida",
                detail: ex.Message,
                statusCode: 400);
        }
    }

    private static async Task<IResult> ExtendAssignment(
        Guid id,
        ExtendAssignmentRequest request,
        IProjectTeamMemberService teamMemberService,
        ICurrentUserService currentUserService)
    {
        var existing = await teamMemberService.GetByIdAsync(id);
        if (existing == null)
        {
            return Results.NotFound($"Miembro del equipo {id} no encontrado");
        }

        // Check permission
        if (!await currentUserService.HasProjectAccessAsync(existing.ProjectId, SimplifiedRoles.Project.ProjectManager))
        {
            return Results.Forbid();
        }

        if (request.NewEndDate <= existing.StartDate)
        {
            return Results.BadRequest("La nueva fecha de fin debe ser posterior a la fecha de inicio");
        }

        var extended = await teamMemberService.ExtendAssignmentAsync(id, request.NewEndDate);
        return extended != null ? Results.Ok(extended) : Results.NotFound();
    }

    private static async Task<IResult> CheckUserAssignment(
        Guid userId,
        Guid projectId,
        IProjectTeamMemberService teamMemberService)
    {
        var isAssigned = await teamMemberService.IsUserAssignedToProjectAsync(userId, projectId);
        return Results.Ok(isAssigned);
    }

    private static async Task<IResult> CanAssignUser(
        CanAssignUserRequest request,
        IProjectTeamMemberService teamMemberService)
    {
        var canAssign = await teamMemberService.CanUserBeAssignedAsync(
            request.UserId, 
            request.ProjectId, 
            request.AllocationPercentage);
            
        return Results.Ok(new AssignmentValidationResult(
            canAssign,
            canAssign ? null : "El usuario no puede ser asignado debido a restricciones de asignación o asignaciones existentes"));
    }

    private static async Task<IResult> CheckUserRole(
        Guid userId,
        Guid projectId,
        string role,
        IProjectTeamMemberService teamMemberService)
    {
        var hasRole = await teamMemberService.HasUserRoleInProjectAsync(userId, projectId, role);
        return Results.Ok(hasRole);
    }
}

// Request/Response DTOs
public record AvailabilityQuery(DateTime? StartDate, DateTime? EndDate);
public record AllocationReportQuery(DateTime? Date, Guid? ProjectId, Guid? UserId);
public record UpdateAllocationRequest(decimal AllocationPercentage);
public record ExtendAssignmentRequest(DateTime NewEndDate);
public record CanAssignUserRequest(Guid UserId, Guid ProjectId, decimal? AllocationPercentage);
public record BulkAssignResult(int SuccessCount, int TotalCount);
public record AssignmentValidationResult(bool CanAssign, string? Reason);