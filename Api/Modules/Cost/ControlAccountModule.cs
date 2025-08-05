using Application.Interfaces.Cost;
using Carter;
using Core.DTOs.Common;
using Core.DTOs.Cost.ControlAccounts;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Cost;

/// <summary>
/// Control Account management endpoints
/// </summary>
public class ControlAccountModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/control-accounts")
            .WithTags("Control Accounts")
            .RequireAuthorization();

        // Query endpoints
        group.MapGet("/", GetControlAccounts)
            .WithName("GetControlAccounts")
            .WithSummary("Get control accounts with optional filtering")
            .Produces<PagedResult<ControlAccountDto>>();

        group.MapGet("/{id:guid}", GetControlAccountById)
            .WithName("GetControlAccountById")
            .WithSummary("Get control account by ID")
            .Produces<ControlAccountDetailDto>()
            .ProducesProblem(404);

        group.MapGet("/phase/{phaseId:guid}", GetControlAccountsByPhase)
            .WithName("GetControlAccountsByPhase")
            .WithSummary("Get control accounts by phase")
            .Produces<List<ControlAccountDto>>();

        group.MapGet("/{id:guid}/assignments", GetControlAccountAssignments)
            .WithName("GetControlAccountAssignments")
            .WithSummary("Get control account assignments")
            .Produces<List<ControlAccountAssignmentDto>>();

        group.MapGet("/{id:guid}/evm-summary", GetLatestEVMSummary)
            .WithName("GetLatestEVMSummary")
            .WithSummary("Get latest EVM summary for control account")
            .Produces<EVMSummaryDto>()
            .ProducesProblem(404);

        // Command endpoints
        group.MapPost("/", CreateControlAccount)
            .WithName("CreateControlAccount")
            .WithSummary("Create a new control account")
            .Produces<Guid>(201)
            .ProducesValidationProblem();

        group.MapPut("/{id:guid}", UpdateControlAccount)
            .WithName("UpdateControlAccount")
            .WithSummary("Update a control account")
            .Produces(200)
            .ProducesValidationProblem()
            .ProducesProblem(404);

        group.MapPut("/{id:guid}/status", UpdateControlAccountStatus)
            .WithName("UpdateControlAccountStatus")
            .WithSummary("Update control account status")
            .Produces(200)
            .ProducesValidationProblem()
            .ProducesProblem(404);

        group.MapDelete("/{id:guid}", DeleteControlAccount)
            .WithName("DeleteControlAccount")
            .WithSummary("Delete a control account")
            .Produces(204)
            .ProducesProblem(404);

        // Assignment endpoints
        group.MapPost("/{id:guid}/assignments", AssignUserToControlAccount)
            .WithName("AssignUserToControlAccount")
            .WithSummary("Assign user to control account")
            .Produces(200)
            .ProducesValidationProblem()
            .ProducesProblem(404);

        group.MapDelete("/{id:guid}/assignments/{userToRemove}", RemoveUserFromControlAccount)
            .WithName("RemoveUserFromControlAccount")
            .WithSummary("Remove user from control account")
            .Produces(204)
            .ProducesProblem(404);

        // Progress endpoints
        group.MapPut("/{id:guid}/progress", UpdateControlAccountProgress)
            .WithName("UpdateControlAccountProgress")
            .WithSummary("Update control account progress")
            .Produces(200)
            .ProducesValidationProblem()
            .ProducesProblem(404);

        // Workflow endpoints
        group.MapPost("/{id:guid}/baseline", BaselineControlAccount)
            .WithName("BaselineControlAccount")
            .WithSummary("Baseline a control account")
            .Produces(200)
            .ProducesProblem(404);

        group.MapPost("/{id:guid}/close", CloseControlAccount)
            .WithName("CloseControlAccount")
            .WithSummary("Close a control account")
            .Produces(200)
            .ProducesProblem(404);
    }

    // Query handlers
    private static async Task<IResult> GetControlAccounts(
        [FromQuery] Guid? projectId,
        [AsParameters] QueryParameters parameters,
        IControlAccountService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetControlAccountsAsync(projectId, parameters, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetControlAccountById(
        [FromRoute] Guid id,
        IControlAccountService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetControlAccountByIdAsync(id, cancellationToken);
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> GetControlAccountsByPhase(
        [FromRoute] Guid phaseId,
        IControlAccountService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetControlAccountsByPhaseAsync(phaseId, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetControlAccountAssignments(
        [FromRoute] Guid id,
        IControlAccountService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetControlAccountAssignmentsAsync(id, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetLatestEVMSummary(
        [FromRoute] Guid id,
        IControlAccountService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetLatestEVMSummaryAsync(id, cancellationToken);
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    // Command handlers
    private static async Task<IResult> CreateControlAccount(
        [FromBody] CreateControlAccountDto dto,
        IControlAccountService service,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
        var result = await service.CreateControlAccountAsync(dto, userId, cancellationToken);
        return result.IsSuccess 
            ? Results.Created($"/api/control-accounts/{result.Value}", result.Value)
            : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> UpdateControlAccount(
        [FromRoute] Guid id,
        [FromBody] UpdateControlAccountDto dto,
        IControlAccountService service,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
        var result = await service.UpdateControlAccountAsync(id, dto, userId, cancellationToken);
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> UpdateControlAccountStatus(
        [FromRoute] Guid id,
        [FromBody] UpdateControlAccountStatusDto dto,
        IControlAccountService service,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
        var result = await service.UpdateControlAccountStatusAsync(id, dto, userId, cancellationToken);
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> DeleteControlAccount(
        [FromRoute] Guid id,
        IControlAccountService service,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
        var result = await service.DeleteControlAccountAsync(id, userId, cancellationToken);
        return result.IsSuccess ? Results.NoContent() : Results.BadRequest(result.Error);
    }

    // Assignment handlers
    private static async Task<IResult> AssignUserToControlAccount(
        [FromRoute] Guid id,
        [FromBody] CreateControlAccountAssignmentDto dto,
        IControlAccountService service,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
        var result = await service.AssignUserToControlAccountAsync(id, dto, userId, cancellationToken);
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> RemoveUserFromControlAccount(
        [FromRoute] Guid id,
        [FromRoute] string userToRemove,
        IControlAccountService service,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
        var result = await service.RemoveUserFromControlAccountAsync(id, userToRemove, userId, cancellationToken);
        return result.IsSuccess ? Results.NoContent() : Results.BadRequest(result.Error);
    }

    // Progress handlers
    private static async Task<IResult> UpdateControlAccountProgress(
        [FromRoute] Guid id,
        [FromBody] UpdateControlAccountProgressDto dto,
        IControlAccountService service,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
        var result = await service.UpdateControlAccountProgressAsync(id, dto.PercentComplete, userId, cancellationToken);
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    // Workflow handlers
    private static async Task<IResult> BaselineControlAccount(
        [FromRoute] Guid id,
        IControlAccountService service,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
        var result = await service.BaselineControlAccountAsync(id, userId, cancellationToken);
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> CloseControlAccount(
        [FromRoute] Guid id,
        IControlAccountService service,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
        var result = await service.CloseControlAccountAsync(id, userId, cancellationToken);
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }
}

// Request DTOs
public record UpdateControlAccountProgressDto(decimal PercentComplete);