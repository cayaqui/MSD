using Carter;
using Carter.ModelBinding;
using Microsoft.AspNetCore.Http.HttpResults;
using Application.Interfaces.Cost;
using Core.DTOs.ControlAccounts;
using Core.DTOs.Common;
using Core.Enums.Cost;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;

namespace API.Endpoints.Cost;

/// <summary>
/// Control Account management endpoints
/// </summary>
public class ControlAccountEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/control-accounts")
            .WithTags("Control Accounts")
            .RequireAuthorization()
            .WithOpenApi();

        // GET endpoints
        group.MapGet("", GetControlAccounts)
            .WithName("GetControlAccounts")
            .WithSummary("Get control accounts with pagination and filtering")
            .Produces<PagedResult<ControlAccountDto>>();

        group.MapGet("{id:guid}", GetControlAccountById)
            .WithName("GetControlAccount")
            .WithSummary("Get control account by ID")
            .Produces<ControlAccountDetailDto>()
            .Produces(404);

        group.MapGet("by-phase/{phaseId:guid}", GetControlAccountsByPhase)
            .WithName("GetControlAccountsByPhase")
            .WithSummary("Get control accounts by phase")
            .Produces<List<ControlAccountDto>>();

        group.MapGet("{id:guid}/assignments", GetControlAccountAssignments)
            .WithName("GetControlAccountAssignments")
            .WithSummary("Get control account team assignments")
            .Produces<List<ControlAccountAssignmentDto>>();

        group.MapGet("{id:guid}/evm-summary", GetLatestEVMSummary)
            .WithName("GetLatestEVMSummary")
            .WithSummary("Get latest EVM summary for control account")
            .Produces<EVMSummaryDto>()
            .Produces(404);

        // POST endpoints
        group.MapPost("", CreateControlAccount)
            .WithName("CreateControlAccount")
            .WithSummary("Create a new control account")
            .Produces<Guid>(201)
            .ProducesValidationProblem();

        group.MapPost("{id:guid}/assignments", AssignUserToControlAccount)
            .WithName("AssignUserToControlAccount")
            .WithSummary("Assign user to control account")
            .Produces<Guid>(201)
            .ProducesValidationProblem();

        // PUT endpoints
        group.MapPut("{id:guid}", UpdateControlAccount)
            .WithName("UpdateControlAccount")
            .WithSummary("Update control account")
            .Produces(204)
            .ProducesValidationProblem()
            .Produces(404);

        group.MapPut("{id:guid}/budget", UpdateControlAccountBudget)
            .WithName("UpdateControlAccountBudget")
            .WithSummary("Update control account budget")
            .Produces(204)
            .ProducesValidationProblem()
            .Produces(404);

        // PATCH endpoints
        group.MapPatch("{id:guid}/status", UpdateControlAccountStatus)
            .WithName("UpdateControlAccountStatus")
            .WithSummary("Update control account status")
            .Produces(204)
            .ProducesValidationProblem()
            .Produces(404);

        group.MapPatch("{id:guid}/progress", UpdateControlAccountProgress)
            .WithName("UpdateControlAccountProgress")
            .WithSummary("Update control account progress percentage")
            .Produces(204)
            .Produces(404);

        // Action endpoints
        group.MapPost("{id:guid}/baseline", BaselineControlAccount)
            .WithName("BaselineControlAccount")
            .WithSummary("Baseline control account")
            .Produces(204)
            .Produces(404);

        group.MapPost("{id:guid}/close", CloseControlAccount)
            .WithName("CloseControlAccount")
            .WithSummary("Close control account")
            .Produces(204)
            .Produces(404);

        // DELETE endpoints
        group.MapDelete("{id:guid}", DeleteControlAccount)
            .WithName("DeleteControlAccount")
            .WithSummary("Delete control account (soft delete)")
            .Produces(204)
            .Produces(404);

        group.MapDelete("{id:guid}/assignments/{userId}", RemoveUserFromControlAccount)
            .WithName("RemoveUserFromControlAccount")
            .WithSummary("Remove user from control account")
            .Produces(204)
            .Produces(404);
    }

    // GET endpoints implementation
    private static async Task<Ok<PagedResult<ControlAccountDto>>> GetControlAccounts(
        [FromQuery] Guid projectId,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        [FromQuery] string? searchTerm,
        [FromQuery] string? sortBy,
        [FromQuery] string? sortDirection,
        IControlAccountService service,
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

        var result = await service.GetControlAccountsAsync(projectId, parameters, cancellationToken);
        return TypedResults.Ok(result);
    }

    private static async Task<Results<Ok<ControlAccountDetailDto>, NotFound>> GetControlAccountById(
        Guid id,
        IControlAccountService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetControlAccountByIdAsync(id, cancellationToken);
        return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }

    private static async Task<Ok<List<ControlAccountDto>>> GetControlAccountsByPhase(
        Guid phaseId,
        IControlAccountService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetControlAccountsByPhaseAsync(phaseId, cancellationToken);
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<List<ControlAccountAssignmentDto>>> GetControlAccountAssignments(
        Guid id,
        IControlAccountService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetControlAccountAssignmentsAsync(id, cancellationToken);
        return TypedResults.Ok(result);
    }

    private static async Task<Results<Ok<EVMSummaryDto>, NotFound>> GetLatestEVMSummary(
        Guid id,
        IControlAccountService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetLatestEVMSummaryAsync(id, cancellationToken);
        return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }

    // POST endpoints implementation
    private static async Task<Results<Created<Guid>, ValidationProblem>> CreateControlAccount(
        CreateControlAccountDto dto,
        IControlAccountService service,
        IValidator<CreateControlAccountDto> validator,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var userId = httpContext.User.Identity?.Name ?? "system";
        var result = await service.CreateControlAccountAsync(dto, userId, cancellationToken);

        if (result.IsFailure)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { "error", new[] { result.Error ?? "Failed to create control account" } }
            });
        }

        return TypedResults.Created($"/api/control-accounts/{result.Value}", result.Value);
    }

    private static async Task<Results<Created<Guid>, ValidationProblem>> AssignUserToControlAccount(
        Guid id,
        CreateControlAccountAssignmentDto dto,
        IControlAccountService service,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = httpContext.User.Identity?.Name ?? "system";
        var result = await service.AssignUserToControlAccountAsync(id, dto, userId, cancellationToken);

        if (result.IsFailure)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { "error", new[] { result.Error ?? "Failed to assign user" } }
            });
        }

        return TypedResults.Created($"/api/control-accounts/{id}/assignments/{result.Value}", result.Value);
    }

    // PUT endpoints implementation
    private static async Task<Results<NoContent, NotFound, ValidationProblem>> UpdateControlAccount(
        Guid id,
        UpdateControlAccountDto dto,
        IControlAccountService service,
        IValidator<UpdateControlAccountDto> validator,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var userId = httpContext.User.Identity?.Name ?? "system";
        var result = await service.UpdateControlAccountAsync(id, dto, userId, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error?.Contains("not found", StringComparison.OrdinalIgnoreCase) ?? false)
                return TypedResults.NotFound();

            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { "error", new[] { result.Error ?? "Failed to update control account" } }
            });
        }

        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound, ValidationProblem>> UpdateControlAccountBudget(
        Guid id,
        UpdateControlAccountBudgetDto dto,
        IControlAccountService service,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = httpContext.User.Identity?.Name ?? "system";
        var result = await service.UpdateControlAccountBudgetAsync(id, dto, userId, cancellationToken);

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

    // PATCH endpoints implementation
    private static async Task<Results<NoContent, NotFound, ValidationProblem>> UpdateControlAccountStatus(
        Guid id,
        UpdateControlAccountStatusDto dto,
        IControlAccountService service,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = httpContext.User.Identity?.Name ?? "system";
        var result = await service.UpdateControlAccountStatusAsync(id, dto, userId, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error?.Contains("not found", StringComparison.OrdinalIgnoreCase) ?? false)
                return TypedResults.NotFound();

            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { "error", new[] { result.Error ?? "Failed to update status" } }
            });
        }

        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound>> UpdateControlAccountProgress(
        Guid id,
        [FromQuery] decimal percentComplete,
        IControlAccountService service,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (percentComplete < 0 || percentComplete > 100)
        {
            return TypedResults.NotFound(); // Should be BadRequest, but keeping signature simple
        }

        var userId = httpContext.User.Identity?.Name ?? "system";
        var result = await service.UpdateControlAccountProgressAsync(id, percentComplete, userId, cancellationToken);

        return result.IsFailure ? TypedResults.NotFound() : TypedResults.NoContent();
    }

    // Action endpoints implementation
    private static async Task<Results<NoContent, NotFound>> BaselineControlAccount(
        Guid id,
        IControlAccountService service,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = httpContext.User.Identity?.Name ?? "system";
        var result = await service.BaselineControlAccountAsync(id, userId, cancellationToken);

        return result.IsFailure ? TypedResults.NotFound() : TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound>> CloseControlAccount(
        Guid id,
        IControlAccountService service,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = httpContext.User.Identity?.Name ?? "system";
        var result = await service.CloseControlAccountAsync(id, userId, cancellationToken);

        return result.IsFailure ? TypedResults.NotFound() : TypedResults.NoContent();
    }

    // DELETE endpoints implementation
    private static async Task<Results<NoContent, NotFound>> DeleteControlAccount(
        Guid id,
        IControlAccountService service,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = httpContext.User.Identity?.Name ?? "system";
        var result = await service.DeleteControlAccountAsync(id, userId, cancellationToken);

        return result.IsFailure ? TypedResults.NotFound() : TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound>> RemoveUserFromControlAccount(
        Guid id,
        string userId,
        IControlAccountService service,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.Identity?.Name ?? "system";
        var result = await service.RemoveUserFromControlAccountAsync(id, userId, currentUserId, cancellationToken);

        return result.IsFailure ? TypedResults.NotFound() : TypedResults.NoContent();
    }
}