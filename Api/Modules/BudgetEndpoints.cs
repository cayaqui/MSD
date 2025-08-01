using Carter;
using Carter.ModelBinding;
using Microsoft.AspNetCore.Http.HttpResults;
using Application.Interfaces.Cost;
using Core.DTOs.Budget;
using Core.DTOs.Common;
using Core.Enums.Cost;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;

namespace Api.Modules;

/// <summary>
/// Budget management endpoints
/// </summary>
public class BudgetEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/budgets")
            .WithTags("Budget Management")
            .RequireAuthorization()
            .WithOpenApi();

        // GET endpoints
        group.MapGet("", GetBudgets)
            .WithName("GetBudgets")
            .WithSummary("Get budgets with pagination")
            .Produces<PagedResult<BudgetDto>>();

        group.MapGet("{id:guid}", GetBudgetById)
            .WithName("GetBudget")
            .WithSummary("Get budget by ID")
            .Produces<BudgetDetailDto>()
            .Produces(404);

        group.MapGet("project/{projectId:guid}/baseline", GetCurrentBaselineBudget)
            .WithName("GetCurrentBaselineBudget")
            .WithSummary("Get current baseline budget for project")
            .Produces<BudgetDetailDto>()
            .Produces(404);

        group.MapGet("{id:guid}/items", GetBudgetItems)
            .WithName("GetBudgetItems")
            .WithSummary("Get budget items")
            .Produces<List<BudgetItemDto>>();

        // POST endpoints
        group.MapPost("", CreateBudget)
            .WithName("CreateBudget")
            .WithSummary("Create a new budget")
            .Produces<Guid>(201)
            .ProducesValidationProblem();

        group.MapPost("items", AddBudgetItem)
            .WithName("AddBudgetItem")
            .WithSummary("Add item to budget")
            .Produces<Guid>(201)
            .ProducesValidationProblem();

        group.MapPost("{id:guid}/revisions", CreateBudgetRevision)
            .WithName("CreateBudgetRevision")
            .WithSummary("Create budget revision")
            .Produces<Guid>(201)
            .ProducesValidationProblem();

        // PUT endpoints
        group.MapPut("{id:guid}", UpdateBudget)
            .WithName("UpdateBudget")
            .WithSummary("Update budget")
            .Produces(204)
            .ProducesValidationProblem()
            .Produces(404);

        group.MapPut("items/{itemId:guid}", UpdateBudgetItem)
            .WithName("UpdateBudgetItem")
            .WithSummary("Update budget item")
            .Produces(204)
            .Produces(404);

        // Action endpoints
        group.MapPost("{id:guid}/submit", SubmitBudgetForApproval)
            .WithName("SubmitBudgetForApproval")
            .WithSummary("Submit budget for approval")
            .Produces(204)
            .Produces(404);

        group.MapPost("{id:guid}/approve", ApproveBudget)
            .WithName("ApproveBudget")
            .WithSummary("Approve budget")
            .Produces(204)
            .ProducesValidationProblem()
            .Produces(404);

        group.MapPost("{id:guid}/reject", RejectBudget)
            .WithName("RejectBudget")
            .WithSummary("Reject budget")
            .Produces(204)
            .ProducesValidationProblem()
            .Produces(404);

        group.MapPost("{id:guid}/baseline", SetBudgetAsBaseline)
            .WithName("SetBudgetAsBaseline")
            .WithSummary("Set budget as baseline")
            .Produces(204)
            .Produces(404);

        group.MapPost("{id:guid}/lock", LockBudget)
            .WithName("LockBudget")
            .WithSummary("Lock budget")
            .Produces(204)
            .Produces(404);

        group.MapPost("revisions/{revisionId:guid}/approve", ApproveBudgetRevision)
            .WithName("ApproveBudgetRevision")
            .WithSummary("Approve budget revision")
            .Produces(204)
            .Produces(404);

        // DELETE endpoints
        group.MapDelete("{id:guid}", DeleteBudget)
            .WithName("DeleteBudget")
            .WithSummary("Delete budget")
            .Produces(204)
            .Produces(404);

        group.MapDelete("items/{itemId:guid}", RemoveBudgetItem)
            .WithName("RemoveBudgetItem")
            .WithSummary("Remove budget item")
            .Produces(204)
            .Produces(404);
    }

    // GET endpoints implementation
    private static async Task<Ok<PagedResult<BudgetDto>>> GetBudgets(
        [FromQuery] Guid projectId,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        [FromQuery] BudgetType? type,
        [FromQuery] BudgetStatus? status,
        [FromQuery] string? searchTerm,
        IBudgetService service,
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

        if (status.HasValue)
            parameters.Filters.Add("status", status.Value.ToString());

        var result = await service.GetBudgetsAsync(projectId, parameters, cancellationToken);
        return TypedResults.Ok(result);
    }

    private static async Task<Results<Ok<BudgetDetailDto>, NotFound>> GetBudgetById(
        Guid id,
        IBudgetService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetBudgetByIdAsync(id, cancellationToken);
        return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }

    private static async Task<Results<Ok<BudgetDetailDto>, NotFound>> GetCurrentBaselineBudget(
        Guid projectId,
        IBudgetService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetCurrentBaselineBudgetAsync(projectId, cancellationToken);
        return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }

    private static async Task<Ok<List<BudgetItemDto>>> GetBudgetItems(
        Guid id,
        IBudgetService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetBudgetItemsAsync(id, cancellationToken);
        return TypedResults.Ok(result);
    }

    // POST endpoints implementation
    private static async Task<Results<Created<Guid>, ValidationProblem>> CreateBudget(
        CreateBudgetDto dto,
        IBudgetService service,
        IValidator<CreateBudgetDto> validator,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var userId = httpContext.User.Identity?.Name ?? "system";
        var result = await service.CreateBudgetAsync(dto, userId, cancellationToken);

        if (result.IsFailure)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { "error", new[] { result.Error ?? "Failed to create budget" } }
            });
        }

        return TypedResults.Created($"/api/budgets/{result.Value}", result.Value);
    }

    private static async Task<Results<Created<Guid>, ValidationProblem>> AddBudgetItem(
        CreateBudgetItemDto dto,
        IBudgetService service,
        IValidator<CreateBudgetItemDto> validator,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var userId = httpContext.User.Identity?.Name ?? "system";
        var result = await service.AddBudgetItemAsync(dto, userId, cancellationToken);

        if (result.IsFailure)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { "error", new[] { result.Error ?? "Failed to add budget item" } }
            });
        }

        return TypedResults.Created($"/api/budgets/items/{result.Value}", result.Value);
    }

    private static async Task<Results<Created<Guid>, ValidationProblem>> CreateBudgetRevision(
        Guid id,
        CreateBudgetRevisionDto dto,
        IBudgetService service,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = httpContext.User.Identity?.Name ?? "system";
        var result = await service.CreateBudgetRevisionAsync(id, dto, userId, cancellationToken);

        if (result.IsFailure)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { "error", new[] { result.Error ?? "Failed to create budget revision" } }
            });
        }

        return TypedResults.Created($"/api/budgets/{id}/revisions/{result.Value}", result.Value);
    }

    // PUT endpoints implementation
    private static async Task<Results<NoContent, NotFound, ValidationProblem>> UpdateBudget(
        Guid id,
        UpdateBudgetDto dto,
        IBudgetService service,
        IValidator<UpdateBudgetDto> validator,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var userId = httpContext.User.Identity?.Name ?? "system";
        var result = await service.UpdateBudgetAsync(id, dto, userId, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error?.Contains("not found", StringComparison.OrdinalIgnoreCase) ?? false)
                return TypedResults.NotFound();

            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { "error", new[] { result.Error ?? "Failed to update budget" } }
            });
        }

        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound>> UpdateBudgetItem(
        Guid itemId,
        [FromQuery] decimal quantity,
        [FromQuery] decimal unitRate,
        IBudgetService service,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (quantity <= 0 || unitRate < 0)
        {
            return TypedResults.NotFound(); // Should be BadRequest
        }

        var userId = httpContext.User.Identity?.Name ?? "system";
        var result = await service.UpdateBudgetItemAsync(itemId, quantity, unitRate, userId, cancellationToken);

        return result.IsFailure ? TypedResults.NotFound() : TypedResults.NoContent();
    }

    // Action endpoints implementation
    private static async Task<Results<NoContent, NotFound>> SubmitBudgetForApproval(
        Guid id,
        IBudgetService service,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = httpContext.User.Identity?.Name ?? "system";
        var result = await service.SubmitBudgetForApprovalAsync(id, userId, cancellationToken);

        return result.IsFailure ? TypedResults.NotFound() : TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound, ValidationProblem>> ApproveBudget(
        Guid id,
        ApproveBudgetDto dto,
        IBudgetService service,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = httpContext.User.Identity?.Name ?? "system";
        var result = await service.ApproveBudgetAsync(id, dto, userId, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error?.Contains("not found", StringComparison.OrdinalIgnoreCase) ?? false)
                return TypedResults.NotFound();

            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { "error", new[] { result.Error ?? "Failed to approve budget" } }
            });
        }

        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound, ValidationProblem>> RejectBudget(
        Guid id,
        RejectBudgetDto dto,
        IBudgetService service,
        IValidator<RejectBudgetDto> validator,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var userId = httpContext.User.Identity?.Name ?? "system";
        var result = await service.RejectBudgetAsync(id, dto, userId, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error?.Contains("not found", StringComparison.OrdinalIgnoreCase) ?? false)
                return TypedResults.NotFound();

            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { "error", new[] { result.Error ?? "Failed to reject budget" } }
            });
        }

        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound>> SetBudgetAsBaseline(
        Guid id,
        IBudgetService service,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = httpContext.User.Identity?.Name ?? "system";
        var result = await service.SetBudgetAsBaselineAsync(id, userId, cancellationToken);

        return result.IsFailure ? TypedResults.NotFound() : TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound>> LockBudget(
        Guid id,
        IBudgetService service,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = httpContext.User.Identity?.Name ?? "system";
        var result = await service.LockBudgetAsync(id, userId, cancellationToken);

        return result.IsFailure ? TypedResults.NotFound() : TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound>> ApproveBudgetRevision(
        Guid revisionId,
        IBudgetService service,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = httpContext.User.Identity?.Name ?? "system";
        var result = await service.ApproveBudgetRevisionAsync(revisionId, userId, cancellationToken);

        return result.IsFailure ? TypedResults.NotFound() : TypedResults.NoContent();
    }

    // DELETE endpoints implementation
    private static async Task<Results<NoContent, NotFound>> DeleteBudget(
        Guid id,
        IBudgetService service,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = httpContext.User.Identity?.Name ?? "system";
        var result = await service.DeleteBudgetAsync(id, userId, cancellationToken);

        return result.IsFailure ? TypedResults.NotFound() : TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound>> RemoveBudgetItem(
        Guid itemId,
        IBudgetService service,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = httpContext.User.Identity?.Name ?? "system";
        var result = await service.RemoveBudgetItemAsync(itemId, userId, cancellationToken);

        return result.IsFailure ? TypedResults.NotFound() : TypedResults.NoContent();
    }
}
