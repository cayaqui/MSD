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
/// Project team member management endpoints
/// </summary>
public class ProjectTeamMembersModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var teamMembers = app.MapGroup("/api/project-team-members")
            .WithTags("Project Team Members")
            .RequireAuthorization();

        // Read operations
        teamMembers.MapPost("/search", SearchTeamMembers)
            .WithName("SearchTeamMembers")
            .WithSummary("Search team members with filters")
            .Produces<PagedResult<ProjectTeamMemberDetailDto>>(200);

        teamMembers.MapGet("/{id:guid}", GetTeamMemberById)
            .WithName("GetTeamMemberById")
            .WithSummary("Get team member by ID")
            .Produces<ProjectTeamMemberDetailDto>(200)
            .Produces(404);

        teamMembers.MapGet("/project/{projectId:guid}", GetProjectTeamMembers)
            .WithName("GetProjectTeamMembers")
            .WithSummary("Get all team members for a project")
            .Produces<IEnumerable<ProjectTeamMemberDetailDto>>(200);

        teamMembers.MapGet("/user/{userId:guid}", GetUserAssignments)
            .WithName("GetUserAssignments")
            .WithSummary("Get all project assignments for a user")
            .Produces<IEnumerable<ProjectTeamMemberDetailDto>>(200);

        // Assignment management
        teamMembers.MapPost("/project/{projectId:guid}/assign", AssignTeamMember)
            .WithName("AssignTeamMember")
            .WithSummary("Assign a team member to a project")
            .Produces<ProjectTeamMemberDetailDto>(201)
            .Produces(400)
            .RequireAuthorization(policy => policy.RequireRoleOrAdmin(ProjectRoles.ProjectManager));

        teamMembers.MapPut("/{id:guid}", UpdateTeamMember)
            .WithName("UpdateTeamMember")
            .WithSummary("Update team member details")
            .Produces<ProjectTeamMemberDetailDto>(200)
            .Produces(404)
            .Produces(400)
            .RequireAuthorization(policy => policy.RequireRoleOrAdmin(ProjectRoles.ProjectManager));

        teamMembers.MapDelete("/{id:guid}", RemoveTeamMember)
            .WithName("RemoveTeamMember")
            .WithSummary("Remove team member from project")
            .Produces(204)
            .Produces(404)
            .RequireAuthorization(policy => policy.RequireRoleOrAdmin(ProjectRoles.ProjectManager));

        // Bulk operations
        teamMembers.MapPost("/bulk-assign", BulkAssignTeamMembers)
            .WithName("BulkAssignTeamMembers")
            .WithSummary("Assign multiple team members to projects")
            .Produces<BulkAssignResult>(200)
            .RequireAuthorization(policy => policy.RequireRoleOrAdmin(ProjectRoles.ProjectManager));

        teamMembers.MapDelete("/project/{projectId:guid}/all", RemoveAllFromProject)
            .WithName("RemoveAllFromProject")
            .WithSummary("Remove all team members from a project")
            .Produces<int>(200)
            .RequireAuthorization(policy => policy.RequireRoleOrAdmin(ProjectRoles.ProjectManager));

        // Allocation management
        teamMembers.MapGet("/user/{userId:guid}/availability", GetUserAvailability)
            .WithName("GetUserAvailability")
            .WithSummary("Get user availability and allocation")
            .Produces<UserAvailabilityDto>(200)
            .Produces(404);

        teamMembers.MapGet("/allocation-report", GetAllocationReport)
            .WithName("GetAllocationReport")
            .WithSummary("Get team allocation report")
            .Produces<IEnumerable<TeamAllocationReportDto>>(200);

        teamMembers.MapPut("/{id:guid}/allocation", UpdateAllocation)
            .WithName("UpdateAllocation")
            .WithSummary("Update team member allocation percentage")
            .Produces<ProjectTeamMemberDetailDto>(200)
            .Produces(404)
            .RequireAuthorization(policy => policy.RequireRoleOrAdmin(ProjectRoles.ProjectManager));

        // Transfer and extension
        teamMembers.MapPost("/{id:guid}/transfer", TransferTeamMember)
            .WithName("TransferTeamMember")
            .WithSummary("Transfer team member to another project")
            .Produces<ProjectTeamMemberDetailDto>(200)
            .Produces(404)
            .RequireAuthorization(policy => policy.RequireRoleOrAdmin(ProjectRoles.ProjectManager));

        teamMembers.MapPost("/{id:guid}/extend", ExtendAssignment)
            .WithName("ExtendAssignment")
            .WithSummary("Extend team member assignment end date")
            .Produces<ProjectTeamMemberDetailDto>(200)
            .Produces(404)
            .RequireAuthorization(policy => policy.RequireRoleOrAdmin(ProjectRoles.ProjectManager));

        // Validation endpoints
        teamMembers.MapGet("/check-assignment/{userId:guid}/{projectId:guid}", CheckUserAssignment)
            .WithName("CheckUserAssignment")
            .WithSummary("Check if user is assigned to project")
            .Produces<bool>(200);

        teamMembers.MapPost("/can-assign", CanAssignUser)
            .WithName("CanAssignUser")
            .WithSummary("Check if user can be assigned to project")
            .Produces<AssignmentValidationResult>(200);

        teamMembers.MapGet("/check-role/{userId:guid}/{projectId:guid}/{role}", CheckUserRole)
            .WithName("CheckUserRole")
            .WithSummary("Check if user has specific role in project")
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
        return member != null ? Results.Ok(member) : Results.NotFound($"Team member {id} not found");
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
            if (!await currentUserService.HasProjectAccessAsync(projectId, ProjectRoles.ProjectManager))
            {
                return Results.Forbid();
            }

            // Validate assignment is possible
            if (!await teamMemberService.CanUserBeAssignedAsync(dto.UserId, projectId, dto.AllocationPercentage))
            {
                return Results.BadRequest("User cannot be assigned to this project. Check allocation and existing assignments.");
            }

            var member = await teamMemberService.CreateAsync(projectId, dto);
            return Results.Created($"/api/project-team-members/{member.Id}", member);
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "Assignment failed",
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
                return Results.NotFound($"Team member {id} not found");
            }

            // Check permission for the project
            if (!await currentUserService.HasProjectAccessAsync(existing.ProjectId, ProjectRoles.ProjectManager))
            {
                return Results.Forbid();
            }

            var updated = await teamMemberService.UpdateAsync(id, dto);
            return updated != null ? Results.Ok(updated) : Results.NotFound();
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "Update failed",
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
            return Results.NotFound($"Team member {id} not found");
        }

        // Check permission for the project
        if (!await currentUserService.HasProjectAccessAsync(existing.ProjectId, ProjectRoles.ProjectManager))
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
                if (!await currentUserService.HasProjectAccessAsync(assignment.ProjectId, ProjectRoles.ProjectManager))
                {
                    return Results.Problem($"No permission to manage team for project {assignment.ProjectId.ToString()}");
                }
            }

            var count = await teamMemberService.BulkAssignAsync(dto);
            return Results.Ok(new BulkAssignResult(count, dto.Assignments.Count));
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "Bulk assignment failed",
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
        if (!await currentUserService.HasProjectAccessAsync(projectId, ProjectRoles.ProjectManager))
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
            
        return availability != null ? Results.Ok(availability) : Results.NotFound($"User {userId} not found");
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
            return Results.NotFound($"Team member {id} not found");
        }

        // Check permission
        if (!await currentUserService.HasProjectAccessAsync(existing.ProjectId, ProjectRoles.ProjectManager))
        {
            return Results.Forbid();
        }

        if (request.AllocationPercentage < 0 || request.AllocationPercentage > 100)
        {
            return Results.BadRequest("Allocation percentage must be between 0 and 100");
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
                return Results.NotFound($"Team member {id} not found");
            }

            // Check permission for both source and target projects
            if (!await currentUserService.HasProjectAccessAsync(existing.ProjectId, ProjectRoles.ProjectManager) ||
                !await currentUserService.HasProjectAccessAsync(dto.NewProjectId, ProjectRoles.ProjectManager))
            {
                return Results.Forbid();
            }

            var transferred = await teamMemberService.TransferAsync(id, dto);
            return transferred != null ? Results.Ok(transferred) : Results.NotFound();
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "Transfer failed",
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
            return Results.NotFound($"Team member {id} not found");
        }

        // Check permission
        if (!await currentUserService.HasProjectAccessAsync(existing.ProjectId, ProjectRoles.ProjectManager))
        {
            return Results.Forbid();
        }

        if (request.NewEndDate <= existing.StartDate)
        {
            return Results.BadRequest("New end date must be after the start date");
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
            canAssign ? null : "User cannot be assigned due to allocation constraints or existing assignments"));
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