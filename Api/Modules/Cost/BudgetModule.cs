using Application.Interfaces.Cost;
using Carter;
using Core.DTOs.Common;
using Core.DTOs.Cost.BudgetItems;
using Core.DTOs.Cost.BudgetRevisions;
using Core.DTOs.Cost.Budgets;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Cost;

/// <summary>
/// Budget management endpoints
/// </summary>
public class BudgetModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/budgets")
            .WithTags("Budgets")
            .RequireAuthorization();

        // Query endpoints
        group.MapGet("/project/{projectId:guid}", GetProjectBudgets)
            .WithName("GetProjectBudgets")
            .WithSummary("Get budgets for a project")
            .Produces<PagedResult<BudgetDto>>();

        group.MapGet("/{id:guid}", GetBudgetById)
            .WithName("GetBudgetById")
            .WithSummary("Get budget by ID")
            .Produces<BudgetDto>()
            .ProducesProblem(404);

        group.MapGet("/project/{projectId:guid}/baseline", GetCurrentBaselineBudget)
            .WithName("GetCurrentBaselineBudget")
            .WithSummary("Get current baseline budget for a project")
            .Produces<BudgetDetailDto>()
            .ProducesProblem(404);

        group.MapGet("/{budgetId:guid}/items", GetBudgetItems)
            .WithName("GetBudgetItems")
            .WithSummary("Get items for a budget")
            .Produces<List<BudgetItemDto>>();

        // Command endpoints
        group.MapPost("/", CreateBudget)
            .WithName("CreateBudget")
            .WithSummary("Create a new budget")
            .Produces<BudgetDto>(201)
            .ProducesValidationProblem();

        group.MapPut("/{id:guid}", UpdateBudget)
            .WithName("UpdateBudget")
            .WithSummary("Update a budget")
            .Produces<BudgetDto>()
            .ProducesValidationProblem()
            .ProducesProblem(404);

        group.MapDelete("/{id:guid}", DeleteBudget)
            .WithName("DeleteBudget")
            .WithSummary("Delete a budget")
            .Produces(204)
            .ProducesProblem(404);

        // Workflow endpoints
        group.MapPost("/{id:guid}/submit", SubmitBudgetForApproval)
            .WithName("SubmitBudgetForApproval")
            .WithSummary("Submit budget for approval")
            .Produces(200)
            .ProducesProblem(404);

        group.MapPost("/{id:guid}/approve", ApproveBudget)
            .WithName("ApproveBudget")
            .WithSummary("Approve a budget")
            .Produces(200)
            .ProducesValidationProblem()
            .ProducesProblem(404);

        group.MapPost("/{id:guid}/reject", RejectBudget)
            .WithName("RejectBudget")
            .WithSummary("Reject a budget")
            .Produces(200)
            .ProducesValidationProblem()
            .ProducesProblem(404);

        group.MapPost("/{id:guid}/baseline", SetBudgetAsBaseline)
            .WithName("SetBudgetAsBaseline")
            .WithSummary("Set budget as baseline")
            .Produces(200)
            .ProducesProblem(404);

        group.MapPost("/{id:guid}/lock", LockBudget)
            .WithName("LockBudget")
            .WithSummary("Lock a budget")
            .Produces(200)
            .ProducesProblem(404);

        // Budget Item endpoints
        group.MapPost("/items", AddBudgetItem)
            .WithName("AddBudgetItem")
            .WithSummary("Add item to budget")
            .Produces<Guid>(201)
            .ProducesValidationProblem();

        group.MapPut("/items/{itemId:guid}", UpdateBudgetItem)
            .WithName("UpdateBudgetItem")
            .WithSummary("Update budget item")
            .Produces(200)
            .ProducesValidationProblem()
            .ProducesProblem(404);

        group.MapDelete("/items/{itemId:guid}", RemoveBudgetItem)
            .WithName("RemoveBudgetItem")
            .WithSummary("Remove budget item")
            .Produces(204)
            .ProducesProblem(404);

        // Revision endpoints
        group.MapPost("/{budgetId:guid}/revisions", CreateBudgetRevision)
            .WithName("CreateBudgetRevision")
            .WithSummary("Create budget revision")
            .Produces<Guid>(201)
            .ProducesValidationProblem();

        group.MapPost("/revisions/{revisionId:guid}/approve", ApproveBudgetRevision)
            .WithName("ApproveBudgetRevision")
            .WithSummary("Approve budget revision")
            .Produces(200)
            .ProducesProblem(404);
    }

    // Query handlers
    private static async Task<IResult> GetProjectBudgets(
        [FromRoute] Guid projectId,
        [AsParameters] QueryParameters parameters,
        IBudgetService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetBudgetsAsync(projectId, parameters, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetBudgetById(
        [FromRoute] Guid id,
        IBudgetService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetByIdAsync(id, cancellationToken);
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> GetCurrentBaselineBudget(
        [FromRoute] Guid projectId,
        IBudgetService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetCurrentBaselineBudgetAsync(projectId, cancellationToken);
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> GetBudgetItems(
        [FromRoute] Guid budgetId,
        IBudgetService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetBudgetItemsAsync(budgetId, cancellationToken);
        return Results.Ok(result);
    }

    // Command handlers
    private static async Task<IResult> CreateBudget(
        [FromBody] CreateBudgetDto dto,
        IBudgetService service,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
        var result = await service.CreateAsync(dto, userId, cancellationToken);
        return Results.Created($"/api/budgets/{result.Id}", result);
    }

    private static async Task<IResult> UpdateBudget(
        [FromRoute] Guid id,
        [FromBody] UpdateBudgetDto dto,
        IBudgetService service,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
        var result = await service.UpdateAsync(id, dto, userId, cancellationToken);
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> DeleteBudget(
        [FromRoute] Guid id,
        IBudgetService service,
        CancellationToken cancellationToken)
    {
        var deleted = await service.DeleteAsync(id, cancellationToken);
        return deleted ? Results.NoContent() : Results.NotFound();
    }

    // Workflow handlers
    private static async Task<IResult> SubmitBudgetForApproval(
        [FromRoute] Guid id,
        IBudgetService service,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
        var result = await service.SubmitBudgetForApprovalAsync(id, userId, cancellationToken);
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> ApproveBudget(
        [FromRoute] Guid id,
        [FromBody] ApproveBudgetDto dto,
        IBudgetService service,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
        var result = await service.ApproveBudgetAsync(id, dto, userId, cancellationToken);
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> RejectBudget(
        [FromRoute] Guid id,
        [FromBody] RejectBudgetDto dto,
        IBudgetService service,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
        var result = await service.RejectBudgetAsync(id, dto, userId, cancellationToken);
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> SetBudgetAsBaseline(
        [FromRoute] Guid id,
        IBudgetService service,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
        var result = await service.SetBudgetAsBaselineAsync(id, userId, cancellationToken);
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> LockBudget(
        [FromRoute] Guid id,
        IBudgetService service,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
        var result = await service.LockBudgetAsync(id, userId, cancellationToken);
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    // Budget Item handlers
    private static async Task<IResult> AddBudgetItem(
        [FromBody] CreateBudgetItemDto dto,
        IBudgetService service,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
        var result = await service.AddBudgetItemAsync(dto, userId, cancellationToken);
        return result.IsSuccess 
            ? Results.Created($"/api/budgets/items/{result.Value}", result.Value)
            : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> UpdateBudgetItem(
        [FromRoute] Guid itemId,
        [FromBody] UpdateBudgetItemRequestDto dto,
        IBudgetService service,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
        var result = await service.UpdateBudgetItemAsync(itemId, dto.Quantity, dto.UnitRate, userId, cancellationToken);
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> RemoveBudgetItem(
        [FromRoute] Guid itemId,
        IBudgetService service,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
        var result = await service.RemoveBudgetItemAsync(itemId, userId, cancellationToken);
        return result.IsSuccess ? Results.NoContent() : Results.BadRequest(result.Error);
    }

    // Revision handlers
    private static async Task<IResult> CreateBudgetRevision(
        [FromRoute] Guid budgetId,
        [FromBody] CreateBudgetRevisionDto dto,
        IBudgetService service,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
        var result = await service.CreateBudgetRevisionAsync(budgetId, dto, userId, cancellationToken);
        return result.IsSuccess 
            ? Results.Created($"/api/budgets/revisions/{result.Value}", result.Value)
            : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> ApproveBudgetRevision(
        [FromRoute] Guid revisionId,
        IBudgetService service,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
        var result = await service.ApproveBudgetRevisionAsync(revisionId, userId, cancellationToken);
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }
}

// Request DTOs for endpoints that need custom parameters
public record UpdateBudgetItemRequestDto(decimal Quantity, decimal UnitRate);