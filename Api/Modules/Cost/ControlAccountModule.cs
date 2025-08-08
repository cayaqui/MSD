using Application.Interfaces.Cost;
using Carter;
using Core.DTOs.Common;
using Core.DTOs.Cost.ControlAccounts;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Cost;

/// <summary>
/// Endpoints de gestión de cuentas de control
/// </summary>
public class ControlAccountModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/control-accounts")
            .WithTags("Cuentas de Control")
            .RequireAuthorization();

        // Endpoints de consulta
        group.MapGet("/", GetControlAccounts)
            .WithName("GetControlAccounts")
            .WithSummary("Obtener cuentas de control con filtrado opcional")
            .Produces<PagedResult<ControlAccountDto>>();

        group.MapGet("/{id:guid}", GetControlAccountById)
            .WithName("GetControlAccountById")
            .WithSummary("Obtener cuenta de control por ID")
            .Produces<ControlAccountDetailDto>()
            .ProducesProblem(404);

        group.MapGet("/phase/{phaseId:guid}", GetControlAccountsByPhase)
            .WithName("GetControlAccountsByPhase")
            .WithSummary("Obtener cuentas de control por fase")
            .Produces<List<ControlAccountDto>>();

        group.MapGet("/{id:guid}/assignments", GetControlAccountAssignments)
            .WithName("GetControlAccountAssignments")
            .WithSummary("Obtener asignaciones de cuenta de control")
            .Produces<List<ControlAccountAssignmentDto>>();

        group.MapGet("/{id:guid}/evm-summary", GetLatestEVMSummary)
            .WithName("GetLatestEVMSummary")
            .WithSummary("Obtener resumen EVM más reciente para cuenta de control")
            .Produces<EVMSummaryDto>()
            .ProducesProblem(404);

        // Endpoints de comandos
        group.MapPost("/", CreateControlAccount)
            .WithName("CreateControlAccount")
            .WithSummary("Crear una nueva cuenta de control")
            .Produces<Guid>(201)
            .ProducesValidationProblem();

        group.MapPut("/{id:guid}", UpdateControlAccount)
            .WithName("UpdateControlAccount")
            .WithSummary("Actualizar una cuenta de control")
            .Produces(200)
            .ProducesValidationProblem()
            .ProducesProblem(404);

        group.MapPut("/{id:guid}/status", UpdateControlAccountStatus)
            .WithName("UpdateControlAccountStatus")
            .WithSummary("Actualizar estado de cuenta de control")
            .Produces(200)
            .ProducesValidationProblem()
            .ProducesProblem(404);

        group.MapDelete("/{id:guid}", DeleteControlAccount)
            .WithName("DeleteControlAccount")
            .WithSummary("Eliminar una cuenta de control")
            .Produces(204)
            .ProducesProblem(404);

        // Endpoints de asignación
        group.MapPost("/{id:guid}/assignments", AssignUserToControlAccount)
            .WithName("AssignUserToControlAccount")
            .WithSummary("Asignar usuario a cuenta de control")
            .Produces(200)
            .ProducesValidationProblem()
            .ProducesProblem(404);

        group.MapDelete("/{id:guid}/assignments/{userToRemove}", RemoveUserFromControlAccount)
            .WithName("RemoveUserFromControlAccount")
            .WithSummary("Eliminar usuario de cuenta de control")
            .Produces(204)
            .ProducesProblem(404);

        // Endpoints de progreso
        group.MapPut("/{id:guid}/progress", UpdateControlAccountProgress)
            .WithName("UpdateControlAccountProgress")
            .WithSummary("Actualizar progreso de cuenta de control")
            .Produces(200)
            .ProducesValidationProblem()
            .ProducesProblem(404);

        // Endpoints de flujo de trabajo
        group.MapPost("/{id:guid}/baseline", BaselineControlAccount)
            .WithName("BaselineControlAccount")
            .WithSummary("Establecer línea base de cuenta de control")
            .Produces(200)
            .ProducesProblem(404);

        group.MapPost("/{id:guid}/close", CloseControlAccount)
            .WithName("CloseControlAccount")
            .WithSummary("Cerrar una cuenta de control")
            .Produces(200)
            .ProducesProblem(404);
    }

    // Manejadores de consultas
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

    // Manejadores de comandos
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

    // Manejadores de asignación
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

    // Manejadores de progreso
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

    // Manejadores de flujo de trabajo
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

// DTOs de solicitud
public record UpdateControlAccountProgressDto(decimal PercentComplete);