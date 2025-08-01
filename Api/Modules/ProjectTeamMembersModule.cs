
using Application.Common.Exceptions;
using Carter;
using Core.DTOs.Auth;
using Core.DTOs.Common;
using Core.DTOs.Projects;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ValidationProblemDetails = Core.DTOs.Common.ValidationProblemDetails;
using Application.Interfaces.Auth;

namespace API.Modules;

public class ProjectTeamMembersModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/project-team-members")
            .WithTags("Project Team Members")
            .RequireAuthorization();

        // Get all team members across all projects
        group.MapGet("", GetAllTeamMembers)
            .WithName("GetAllTeamMembers")
            .WithSummary("Get all project team members with pagination")
            .Produces<PagedResult<ProjectTeamMemberDetailDto>>(200);

        // Get team member by ID
        group.MapGet("{id:guid}", GetTeamMemberById)
            .WithName("GetTeamMemberById")
            .WithSummary("Get team member by ID")
            .Produces<ProjectTeamMemberDetailDto>(200)
            .Produces(404);

        // Update team member
        group.MapPut("{id:guid}", UpdateTeamMember)
            .WithName("UpdateTeamMember")
            .WithSummary("Update team member assignment")
            .Produces<ProjectTeamMemberDetailDto>(200)
            .Produces<ValidationProblemDetails>(400)
            .Produces(403)
            .Produces(404);

        // Bulk assign users to project
        group.MapPost("bulk-assign", BulkAssignToProject)
            .WithName("BulkAssignToProject")
            .WithSummary("Assign multiple users to a project")
            .Produces<int>(200)
            .Produces<ValidationProblemDetails>(400)
            .Produces(403);

        // Get user availability
        group.MapGet("users/{userId:guid}/availability", GetUserAvailability)
            .WithName("GetUserAvailability")
            .WithSummary("Get user availability across projects")
            .Produces<UserAvailabilityDto>(200)
            .Produces(404);

        // Transfer team member to another project
        group.MapPost("{id:guid}/transfer", TransferTeamMember)
            .WithName("TransferTeamMember")
            .WithSummary("Transfer team member to another project")
            .Produces<ProjectTeamMemberDetailDto>(200)
            .Produces(400)
            .Produces(403)
            .Produces(404);

        // Get team allocation report
        group.MapGet("allocation-report", GetAllocationReport)
            .WithName("GetAllocationReport")
            .WithSummary("Get team allocation report")
            .Produces<IEnumerable<TeamAllocationReportDto>>(200);

        // Update allocation percentage
        group.MapPatch("{id:guid}/allocation", UpdateAllocation)
            .WithName("UpdateAllocation")
            .WithSummary("Update team member allocation percentage")
            .Produces<ProjectTeamMemberDetailDto>(200)
            .Produces(403)
            .Produces(404);

        // Extend assignment
        group.MapPatch("{id:guid}/extend", ExtendAssignment)
            .WithName("ExtendAssignment")
            .WithSummary("Extend team member assignment end date")
            .Produces<ProjectTeamMemberDetailDto>(200)
            .Produces(403)
            .Produces(404);
    }

    private static async Task<Ok<PagedResult<ProjectTeamMemberDetailDto>>> GetAllTeamMembers(
        [AsParameters] ProjectTeamMemberFilterDto filter,
        [FromServices] IProjectTeamMemberService service)
    {
        var result = await service.GetPagedAsync(filter);
        return TypedResults.Ok(result);
    }

    private static async Task<Results<Ok<ProjectTeamMemberDetailDto>, NotFound>> GetTeamMemberById(
        Guid id,
        IProjectTeamMemberService service)
    {
        var member = await service.GetByIdAsync(id);
        return member != null
            ? TypedResults.Ok(member)
            : TypedResults.NotFound();
    }

    private static async Task<Results<Ok<ProjectTeamMemberDetailDto>, NotFound, ValidationProblem, ForbidHttpResult>> UpdateTeamMember(
        Guid id,
        [FromBody] UpdateProjectTeamMemberDto dto,
        [FromServices] IProjectTeamMemberService service,
        [FromServices] IValidator<UpdateProjectTeamMemberDto> validator,
        [FromServices] ICurrentUserService currentUserService)
    {
        var validationResult = await validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        try
        {
            var member = await service.UpdateAsync(id, dto);
            return member != null
                ? TypedResults.Ok(member)
                : TypedResults.NotFound();
        }
        catch (ForbiddenAccessException)
        {
            return TypedResults.Forbid();
        }
    }

    private static async Task<Results<Ok<int>, ValidationProblem, ForbidHttpResult>> BulkAssignToProject(
        [FromBody] BulkAssignProjectTeamDto dto,
        [FromServices] IProjectTeamMemberService service,
        [FromServices] IValidator<BulkAssignProjectTeamDto> validator)
    {
        var validationResult = await validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        try
        {
            var count = await service.BulkAssignAsync(dto);
            return TypedResults.Ok(count);
        }
        catch (ForbiddenAccessException)
        {
            return TypedResults.Forbid();
        }
    }

    private static async Task<Results<Ok<UserAvailabilityDto>, NotFound>> GetUserAvailability(
        Guid userId,
        DateTime? startDate,
        DateTime? endDate,
        IProjectTeamMemberService service)
    {
        var availability = await service.GetUserAvailabilityAsync(userId, startDate, endDate);
        return availability != null
            ? TypedResults.Ok(availability)
            : TypedResults.NotFound();
    }

    private static async Task<Results<Ok<ProjectTeamMemberDetailDto>, BadRequest<string>, NotFound, ForbidHttpResult>> TransferTeamMember(
        Guid id,
        [FromBody] TransferTeamMemberDto dto,
        [FromServices] IProjectTeamMemberService service)
    {
        try
        {
            var member = await service.TransferAsync(id, dto);
            return member != null
                ? TypedResults.Ok(member)
                : TypedResults.NotFound();
        }
        catch (BadRequestException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
        catch (ForbiddenAccessException)
        {
            return TypedResults.Forbid();
        }
    }

    private static async Task<Ok<IEnumerable<TeamAllocationReportDto>>> GetAllocationReport(
        DateTime? date,
        Guid? projectId,
        Guid? userId,
        IProjectTeamMemberService service)
    {
        var report = await service.GetAllocationReportAsync(date, projectId, userId);
        return TypedResults.Ok(report);
    }

    private static async Task<Results<Ok<ProjectTeamMemberDetailDto>, NotFound, ForbidHttpResult>> UpdateAllocation(
        Guid id,
        [FromBody] UpdateAllocationDto dto,
        [FromServices] IProjectTeamMemberService service)
    {
        try
        {
            var member = await service.UpdateAllocationAsync(id, dto.AllocationPercentage);
            return member != null
                ? TypedResults.Ok(member)
                : TypedResults.NotFound();
        }
        catch (ForbiddenAccessException)
        {
            return TypedResults.Forbid();
        }
    }

    private static async Task<Results<Ok<ProjectTeamMemberDetailDto>, NotFound, ForbidHttpResult>> ExtendAssignment(
        Guid id,
        [FromBody] ExtendAssignmentDto dto,
        [FromServices] IProjectTeamMemberService service)
    {
        try
        {
            var member = await service.ExtendAssignmentAsync(id, dto.NewEndDate);
            return member != null
                ? TypedResults.Ok(member)
                : TypedResults.NotFound();
        }
        catch (ForbiddenAccessException)
        {
            return TypedResults.Forbid();
        }
    }
}
