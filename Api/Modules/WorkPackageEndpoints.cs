using Carter;
using Carter.ModelBinding;
using Microsoft.AspNetCore.Http.HttpResults;
using Application.Interfaces.Cost;
using Core.DTOs.WorkPackages;
using Core.DTOs.Common;
using Core.Enums.Progress;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;

namespace API.Endpoints.Cost;

/// <summary>
/// Work Package management endpoints
/// </summary>
public class WorkPackageEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/work-packages")
            .WithTags("Work Packages")
            .RequireAuthorization()
            .WithOpenApi();

        // GET endpoints
        group.MapGet("", GetWorkPackages)
            .WithName("GetWorkPackages")
            .WithSummary("Get work packages with pagination and filtering")
            .Produces<PagedResult<WorkPackageDto>>();

        group.MapGet("{id:guid}", GetWorkPackageById)
            .WithName("GetWorkPackage")
            .WithSummary("Get work package by ID")
            .Produces<WorkPackageDetailDto>()
            .Produces(404);

        group.MapGet("by-control-account/{controlAccountId:guid}", GetWorkPackagesByControlAccount)
            .WithName("GetWorkPackagesByControlAccount")
            .WithSummary("Get work packages by control account")
            .Produces<List<WorkPackageDto>>();

        group.MapGet("{id:guid}/progress-history", GetWorkPackageProgressHistory)
            .WithName("GetWorkPackageProgressHistory")
            .WithSummary("Get work package progress history")
            .Produces<List<WorkPackageProgressDto>>();

        // POST endpoints
        group.MapPost("", CreateWorkPackage)
            .WithName("CreateWorkPackage")
            .WithSummary("Create a new work package")
            .Produces<Guid>(201)
            .ProducesValidationProblem();

        group.MapPost("{id:guid}/activities", AddActivityToWorkPackage)
            .WithName("AddActivityToWorkPackage")
            .WithSummary("Add activity to work package")
            .Produces<Guid>(201)
            .ProducesValidationProblem();

        // PUT endpoints
        group.MapPut("{id:guid}", UpdateWorkPackage)
            .WithName("UpdateWorkPackage")
            .WithSummary("Update work package")
            .Produces(204)
            .ProducesValidationProblem()
            .Produces(404);

        group.MapPut("{id:guid}/progress", UpdateWorkPackageProgress)
            .WithName("UpdateWorkPackageProgress")
            .WithSummary("Update work package progress")
            .Produces(204)
            .ProducesValidationProblem()
            .Produces(404);

        // PATCH endpoints
        group.MapPatch("activities/{activityId:guid}/progress", UpdateActivityProgress)
            .WithName("UpdateActivityProgress")
            .WithSummary("Update activity progress")
            .Produces(204)
            .Produces(404);

        // Action endpoints
        group.MapPost("{id:guid}/start", StartWorkPackage)
            .WithName("StartWorkPackage")
            .WithSummary("Start work package")
            .Produces(204)
            .Produces(404);

        group.MapPost("{id:guid}/complete", CompleteWorkPackage)
            .WithName("CompleteWorkPackage")
            .WithSummary("Complete work package")
            .Produces(204)
            .Produces(404);

        group.MapPost("{id:guid}/baseline", BaselineWorkPackage)
            .WithName("BaselineWorkPackage")
            .WithSummary("Baseline work package")
            .Produces(204)
            .Produces(404);

        // DELETE endpoints
        group.MapDelete("{id:guid}", DeleteWorkPackage)
            .WithName("DeleteWorkPackage")
            .WithSummary("Delete work package (soft delete)")
            .Produces(204)
            .Produces(404);
    }

    // GET endpoints implementation
    private static async Task<Ok<PagedResult<WorkPackageDto>>> GetWorkPackages(
        [FromQuery] Guid projectId,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        [FromQuery] string? searchTerm,
        [FromQuery] string? sortBy,
        [FromQuery] string? sortDirection,
        [FromQuery] WorkPackageStatus? status,
        [FromQuery] string? responsibleUserId,
        IWorkPackageService service,
        CancellationToken cancellationToken)
    {
        var parameters = new QueryParameters
        {
            PageNumber = pageNumber > 0 ? pageNumber : 1,
            PageSize = pageSize > 0 ? pageSize : 20,
            SearchTerm = searchTerm,
            SortBy = sortBy,
            SortDirection = sortDirection ?? "asc"
        };

        if (status.HasValue)
            parameters.Filters.Add("status", status.Value.ToString());

        if (!string.IsNullOrEmpty(responsibleUserId))
            parameters.Filters.Add("responsibleUserId", responsibleUserId);

        var result = await service.GetWorkPackagesAsync(projectId, parameters, cancellationToken);
        return TypedResults.Ok(result);
    }

    private static async Task<Results<Ok<WorkPackageDetailDto>, NotFound>> GetWorkPackageById(
        Guid id,
        IWorkPackageService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetWorkPackageByIdAsync(id, cancellationToken);
        return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }

    private static async Task<Ok<List<WorkPackageDto>>> GetWorkPackagesByControlAccount(
        Guid controlAccountId,
        IWorkPackageService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetWorkPackagesByControlAccountAsync(controlAccountId, cancellationToken);
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<List<WorkPackageProgressDto>>> GetWorkPackageProgressHistory(
        Guid id,
        IWorkPackageService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetWorkPackageProgressHistoryAsync(id, cancellationToken);
        return TypedResults.Ok(result);
    }

    // POST endpoints implementation
    private static async Task<Results<Created<Guid>, ValidationProblem>> CreateWorkPackage(
        CreateWorkPackageDto dto,
        IWorkPackageService service,
        IValidator<CreateWorkPackageDto> validator,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var userId = httpContext.User.Identity?.Name ?? "system";
        var result = await service.CreateWorkPackageAsync(dto, userId, cancellationToken);

        if (result.IsFailure)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { "error", new[] { result.Error ?? "Failed to create work package" } }
            });
        }

        return TypedResults.Created($"/api/work-packages/{result.Value}", result.Value);
    }

    private static async Task<Results<Created<Guid>, ValidationProblem>> AddActivityToWorkPackage(
        Guid id,
        CreateActivityDto dto,
        IWorkPackageService service,
        IValidator<CreateActivityDto> validator,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var userId = httpContext.User.Identity?.Name ?? "system";
        var result = await service.AddActivityToWorkPackageAsync(id, dto, userId, cancellationToken);

        if (result.IsFailure)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { "error", new[] { result.Error ?? "Failed to add activity" } }
            });
        }

        return TypedResults.Created($"/api/work-packages/{id}/activities/{result.Value}", result.Value);
    }

    // PUT endpoints implementation
    private static async Task<Results<NoContent, NotFound, ValidationProblem>> UpdateWorkPackage(
        Guid id,
        UpdateWorkPackageDto dto,
        IWorkPackageService service,
        IValidator<UpdateWorkPackageDto> validator,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var userId = httpContext.User.Identity?.Name ?? "system";
        var result = await service.UpdateWorkPackageAsync(id, dto, userId, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error?.Contains("not found", StringComparison.OrdinalIgnoreCase) ?? false)
                return TypedResults.NotFound();

            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { "error", new[] { result.Error ?? "Failed to update work package" } }
            });
        }

        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound, ValidationProblem>> UpdateWorkPackageProgress(
        Guid id,
        UpdateWorkPackageProgressDto dto,
        IWorkPackageService service,
        IValidator<UpdateWorkPackageProgressDto> validator,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var userId = httpContext.User.Identity?.Name ?? "system";
        var result = await service.UpdateWorkPackageProgressAsync(id, dto, userId, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error?.Contains("not found", StringComparison.OrdinalIgnoreCase) ?? false)
                return TypedResults.NotFound();

            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { "error", new[] { result.Error ?? "Failed to update progress" } }
            });
        }

        return TypedResults.NoContent();
    }

    // PATCH endpoints implementation
    private static async Task<Results<NoContent, NotFound>> UpdateActivityProgress(
        Guid activityId,
        [FromQuery] decimal percentComplete,
        [FromQuery] decimal actualHours,
        IWorkPackageService service,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (percentComplete < 0 || percentComplete > 100)
        {
            return TypedResults.NotFound(); // Should be BadRequest
        }

        var userId = httpContext.User.Identity?.Name ?? "system";
        var result = await service.UpdateActivityProgressAsync(activityId, percentComplete, actualHours, userId, cancellationToken);

        return result.IsFailure ? TypedResults.NotFound() : TypedResults.NoContent();
    }

    // Action endpoints implementation
    private static async Task<Results<NoContent, NotFound>> StartWorkPackage(
        Guid id,
        IWorkPackageService service,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = httpContext.User.Identity?.Name ?? "system";
        var result = await service.StartWorkPackageAsync(id, userId, cancellationToken);

        return result.IsFailure ? TypedResults.NotFound() : TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound>> CompleteWorkPackage(
        Guid id,
        IWorkPackageService service,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = httpContext.User.Identity?.Name ?? "system";
        var result = await service.CompleteWorkPackageAsync(id, userId, cancellationToken);

        return result.IsFailure ? TypedResults.NotFound() : TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound>> BaselineWorkPackage(
        Guid id,
        IWorkPackageService service,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = httpContext.User.Identity?.Name ?? "system";
        var result = await service.BaselineWorkPackageAsync(id, userId, cancellationToken);

        return result.IsFailure ? TypedResults.NotFound() : TypedResults.NoContent();
    }

    // DELETE endpoints implementation
    private static async Task<Results<NoContent, NotFound>> DeleteWorkPackage(
        Guid id,
        IWorkPackageService service,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = httpContext.User.Identity?.Name ?? "system";
        var result = await service.DeleteWorkPackageAsync(id, userId, cancellationToken);

        return result.IsFailure ? TypedResults.NotFound() : TypedResults.NoContent();
    }
}