using Carter;
using Microsoft.AspNetCore.Http.HttpResults;
using Application.Interfaces.Cost;
using Core.DTOs.Cost;
using Core.DTOs.Common;
using Core.Enums.Cost;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;

namespace API.Endpoints.Cost;

/// <summary>
/// Cost management endpoints
/// </summary>
public class CostEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/costs")
            .WithTags("Cost Management")
            .RequireAuthorization()
            .WithOpenApi();

        // GET endpoints
        group.MapGet("items", GetCostItems)
            .WithName("GetCostItems")
            .WithSummary("Get cost items with pagination")
            .Produces<PagedResult<CostItemDto>>();

        group.MapGet("items/{id:guid}", GetCostItemById)
            .WithName("GetCostItem")
            .WithSummary("Get cost item by ID")
            .Produces<CostItemDetailDto>()
            .Produces(404);

        group.MapGet("project/{projectId:guid}/report", GetProjectCostReport)
            .WithName("GetProjectCostReport")
            .WithSummary("Get project cost report")
            .Produces<ProjectCostReportDto>();

        group.MapGet("project/{projectId:guid}/summary-by-category", GetCostSummaryByCategory)
            .WithName("GetCostSummaryByCategory")
            .WithSummary("Get cost summary by category")
            .Produces<List<CostSummaryByCategoryDto>>();

        group.MapGet("project/{projectId:guid}/summary-by-control-account", GetCostSummaryByControlAccount)
            .WithName("GetCostSummaryByControlAccount")
            .WithSummary("Get cost summary by control account")
            .Produces<List<CostSummaryByControlAccountDto>>();

        // Planning Package endpoints
        group.MapGet("planning-packages", GetPlanningPackages)
            .WithName("GetPlanningPackages")
            .WithSummary("Get planning packages")
            .Produces<PagedResult<PlanningPackageDto>>();

        group.MapPost("planning-packages", CreatePlanningPackage)
            .WithName("CreatePlanningPackage")
            .WithSummary("Create planning package")
            .Produces<Guid>(201)
            .ProducesValidationProblem();

        group.MapPut("planning-packages/{id:guid}", UpdatePlanningPackage)
            .WithName("UpdatePlanningPackage")
            .WithSummary("Update planning package")
            .Produces(204)
            .ProducesValidationProblem()
            .Produces(404);

        group.MapPost("planning-packages/{id:guid}/convert", ConvertPlanningPackageToWorkPackages)
            .WithName("ConvertPlanningPackage")
            .WithSummary("Convert planning package to work packages")
            .Produces(204)
            .ProducesValidationProblem()
            .Produces(404);

        // Cost Item endpoints
        group.MapPost("items", CreateCostItem)
            .WithName("CreateCostItem")
            .WithSummary("Create cost item")
            .Produces<Guid>(201)
            .ProducesValidationProblem();

        group.MapPut("items/{id:guid}", UpdateCostItem)
            .WithName("UpdateCostItem")
            .WithSummary("Update cost item")
            .Produces(204)
            .ProducesValidationProblem()
            .Produces(404);

        group.MapPost("items/{id:guid}/actual-cost", RecordActualCost)
            .WithName("RecordActualCost")
            .WithSummary("Record actual cost")
            .Produces(204)
            .ProducesValidationProblem()
            .Produces(404);

        group.MapPost("items/{id:guid}/commitment", RecordCommitment)
            .WithName("RecordCommitment")
            .WithSummary("Record commitment")
            .Produces(204)
            .ProducesValidationProblem()
            .Produces(404);

        group.MapPost("items/{id:guid}/approve", ApproveCostItem)
            .WithName("ApproveCostItem")
            .WithSummary("Approve cost item")
            .Produces(204)
            .Produces(404);

        group.MapDelete("items/{id:guid}", DeleteCostItem)
            .WithName("DeleteCostItem")
            .WithSummary("Delete cost item")
            .Produces(204)
            .Produces(404);
    }

    // GET endpoints implementation
    private static async Task<Ok<PagedResult<CostItemDto>>> GetCostItems(
        [FromQuery] Guid projectId,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        [FromQuery] CostType? type,
        [FromQuery] CostCategory? category,
        [FromQuery] CostItemStatus? status,
        [FromQuery] string? searchTerm,
        ICostService service,
        CancellationToken cancellationToken)
    {
        var parameters = new QueryParameters
        {
            PageNumber = pageNumber > 0 ? pageNumber : 1,
            PageSize = pageSize > 0 ? pageSize : 20,
            SearchTerm = searchTerm
        };

        if (type.HasValue)
            parameters.Filters.Add("type", type.Value.ToString());

        if (category.HasValue)
            parameters.Filters.Add("category", category.Value.ToString());

        if (status.HasValue)
            parameters.Filters.Add("status", status.Value.ToString());

        var result = await service.GetCostItemsAsync(projectId, parameters, cancellationToken);
        return TypedResults.Ok(result);
    }

    private static async Task<Results<Ok<CostItemDetailDto>, NotFound>> GetCostItemById(
        Guid id,
        ICostService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetCostItemByIdAsync(id, cancellationToken);
        return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }

    private static async Task<Ok<ProjectCostReportDto>> GetProjectCostReport(
        Guid projectId,
        [FromQuery] DateTime? asOfDate,
        ICostService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetProjectCostReportAsync(projectId, asOfDate, cancellationToken);
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<List<CostSummaryByCategoryDto>>> GetCostSummaryByCategory(
        Guid projectId,
        ICostService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetCostSummaryByCategoryAsync(projectId, cancellationToken);
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<List<CostSummaryByControlAccountDto>>> GetCostSummaryByControlAccount(
        Guid projectId,
        ICostService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetCostSummaryByControlAccountAsync(projectId, cancellationToken);
        return TypedResults.Ok(result);
    }

    // Planning Package endpoints implementation
    private static async Task<Ok<PagedResult<PlanningPackageDto>>> GetPlanningPackages(
        [FromQuery] Guid controlAccountId,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        [FromQuery] bool? includeConverted,
        ICostService service,
        CancellationToken cancellationToken)
    {
        var parameters = new QueryParameters
        {
            PageNumber = pageNumber > 0 ? pageNumber : 1,
            PageSize = pageSize > 0 ? pageSize : 20
        };

        if (includeConverted.HasValue)
            parameters.Filters.Add("includeConverted", includeConverted.Value.ToString());

        var result = await service.GetPlanningPackagesAsync(controlAccountId, parameters, cancellationToken);
        return TypedResults.Ok(result);
    }

    private static async Task<Results<Created<Guid>, ValidationProblem>> CreatePlanningPackage(
        CreatePlanningPackageDto dto,
        ICostService service,
        IValidator<CreatePlanningPackageDto> validator,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var userId = httpContext.User.Identity?.Name ?? "system";
        var result = await service.CreatePlanningPackageAsync(dto, userId, cancellationToken);

        if (result.IsFailure)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { "error", new[] { result.Error ?? "Failed to create planning package" } }
            });
        }

        return TypedResults.Created($"/api/costs/planning-packages/{result.Value}", result.Value);
    }

    private static async Task<Results<NoContent, NotFound, ValidationProblem>> UpdatePlanningPackage(
        Guid id,
        UpdatePlanningPackageDto dto,
        ICostService service,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = httpContext.User.Identity?.Name ?? "system";
        var result = await service.UpdatePlanningPackageAsync(id, dto, userId, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error?.Contains("not found", StringComparison.OrdinalIgnoreCase) ?? false)
                return TypedResults.NotFound();

            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { "error", new[] { result.Error ?? "Failed to update planning package" } }
            });
        }

        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound, ValidationProblem>> ConvertPlanningPackageToWorkPackages(
        Guid id,
        ConvertPlanningPackageDto dto,
        ICostService service,
        IValidator<ConvertPlanningPackageDto> validator,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var userId = httpContext.User.Identity?.Name ?? "system";
        var result = await service.ConvertPlanningPackageToWorkPackagesAsync(id, dto, userId, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error?.Contains("not found", StringComparison.OrdinalIgnoreCase) ?? false)
                return TypedResults.NotFound();

            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { "error", new[] { result.Error ?? "Failed to convert planning package" } }
            });
        }

        return TypedResults.NoContent();
    }

    // Cost Item endpoints implementation
    private static async Task<Results<Created<Guid>, ValidationProblem>> CreateCostItem(
        CreateCostItemDto dto,
        ICostService service,
        IValidator<CreateCostItemDto> validator,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var userId = httpContext.User.Identity?.Name ?? "system";
        var result = await service.CreateCostItemAsync(dto, userId, cancellationToken);

        if (result.IsFailure)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { "error", new[] { result.Error ?? "Failed to create cost item" } }
            });
        }

        return TypedResults.Created($"/api/costs/items/{result.Value}", result.Value);
    }

    private static async Task<Results<NoContent, NotFound, ValidationProblem>> UpdateCostItem(
        Guid id,
        UpdateCostItemDto dto,
        ICostService service,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = httpContext.User.Identity?.Name ?? "system";
        var result = await service.UpdateCostItemAsync(id, dto, userId, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error?.Contains("not found", StringComparison.OrdinalIgnoreCase) ?? false)
                return TypedResults.NotFound();

            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { "error", new[] { result.Error ?? "Failed to update cost item" } }
            });
        }

        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound, ValidationProblem>> RecordActualCost(
        Guid id,
        RecordActualCostDto dto,
        ICostService service,
        IValidator<RecordActualCostDto> validator,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var userId = httpContext.User.Identity?.Name ?? "system";
        var result = await service.RecordActualCostAsync(id, dto, userId, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error?.Contains("not found", StringComparison.OrdinalIgnoreCase) ?? false)
                return TypedResults.NotFound();

            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { "error", new[] { result.Error ?? "Failed to record actual cost" } }
            });
        }

        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound, ValidationProblem>> RecordCommitment(
        Guid id,
        RecordCommitmentDto dto,
        ICostService service,
        IValidator<RecordCommitmentDto> validator,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var userId = httpContext.User.Identity?.Name ?? "system";
        var result = await service.RecordCommitmentAsync(id, dto, userId, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error?.Contains("not found", StringComparison.OrdinalIgnoreCase) ?? false)
                return TypedResults.NotFound();

            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { "error", new[] { result.Error ?? "Failed to record commitment" } }
            });
        }

        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound>> ApproveCostItem(
        Guid id,
        ICostService service,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = httpContext.User.Identity?.Name ?? "system";
        var result = await service.ApproveCostItemAsync(id, userId, cancellationToken);

        return result.IsFailure ? TypedResults.NotFound() : TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound>> DeleteCostItem(
        Guid id,
        ICostService service,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = httpContext.User.Identity?.Name ?? "system";
        var result = await service.DeleteCostItemAsync(id, userId, cancellationToken);

        return result.IsFailure ? TypedResults.NotFound() : TypedResults.NoContent();
    }
}