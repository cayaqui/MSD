using Api.Authorization;
using Application.Interfaces.Auth;
using Application.Interfaces.Progress;
using Carter;
using Core.Constants;
using Core.DTOs.Common;
using Core.DTOs.Progress.Milestones;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Progress;

/// <summary>
/// Endpoints para gestión de Hitos del Proyecto
/// </summary>
public class MilestoneModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/milestones")
            .WithTags("Hitos")
            .RequireAuthorization();

        // Query endpoints
        group.MapGet("/", GetMilestones)
            .WithName("GetMilestones")
            .WithSummary("Obtener hitos")
            .WithDescription("Retorna una lista paginada de hitos con filtros")
            .Produces<PagedResult<MilestoneDto>>()
            .RequireAuthorization();

        group.MapGet("/{id:guid}", GetMilestoneById)
            .WithName("GetMilestoneById")
            .WithSummary("Obtener hito por ID")
            .WithDescription("Retorna el detalle completo de un hito")
            .Produces<MilestoneDto>()
            .ProducesProblem(404);

        group.MapGet("/project/{projectId:guid}", GetProjectMilestones)
            .WithName("GetProjectMilestones")
            .WithSummary("Obtener hitos del proyecto")
            .WithDescription("Retorna todos los hitos de un proyecto específico")
            .Produces<List<MilestoneDto>>();

        group.MapGet("/project/{projectId:guid}/upcoming", GetUpcomingMilestones)
            .WithName("GetUpcomingMilestones")
            .WithSummary("Obtener hitos próximos")
            .WithDescription("Retorna los hitos que vencen en los próximos días")
            .Produces<List<MilestoneDto>>();

        group.MapGet("/project/{projectId:guid}/overdue", GetOverdueMilestones)
            .WithName("GetOverdueMilestones")
            .WithSummary("Obtener hitos vencidos")
            .WithDescription("Retorna los hitos que han pasado su fecha objetivo")
            .Produces<List<MilestoneDto>>();

        group.MapGet("/project/{projectId:guid}/critical", GetCriticalMilestones)
            .WithName("GetCriticalMilestones")
            .WithSummary("Obtener hitos críticos")
            .WithDescription("Retorna los hitos marcados como críticos para el proyecto")
            .Produces<List<MilestoneDto>>();

        group.MapGet("/project/{projectId:guid}/contractual", GetContractualMilestones)
            .WithName("GetContractualMilestones")
            .WithSummary("Obtener hitos contractuales")
            .WithDescription("Retorna los hitos contractuales del proyecto")
            .Produces<List<MilestoneDto>>();

        // Command endpoints
        group.MapPost("/", CreateMilestone)
            .WithName("CreateMilestone")
            .WithSummary("Crear hito")
            .WithDescription("Crea un nuevo hito en el proyecto")
            .Produces<Guid>(201)
            .ProducesValidationProblem()
            .RequireAuthorization();

        group.MapPut("/{id:guid}", UpdateMilestone)
            .WithName("UpdateMilestone")
            .WithSummary("Actualizar hito")
            .WithDescription("Actualiza la información básica de un hito")
            .Produces(200)
            .ProducesValidationProblem()
            .ProducesProblem(404)
            .RequireAuthorization();

        group.MapDelete("/{id:guid}", DeleteMilestone)
            .WithName("DeleteMilestone")
            .WithSummary("Eliminar hito")
            .WithDescription("Elimina un hito si no tiene dependencias")
            .Produces(204)
            .ProducesProblem(404)
            .ProducesProblem(400)
            .RequireAuthorization();

        // Progress management
        group.MapPut("/{id:guid}/progress", UpdateMilestoneProgress)
            .WithName("UpdateMilestoneProgress")
            .WithSummary("Actualizar progreso del hito")
            .WithDescription("Actualiza el porcentaje de avance y criterios completados")
            .Produces(200)
            .ProducesValidationProblem()
            .ProducesProblem(404)
            .RequireAuthorization();

        group.MapPost("/{id:guid}/complete", CompleteMilestone)
            .WithName("CompleteMilestone")
            .WithSummary("Completar hito")
            .WithDescription("Marca un hito como completado")
            .Produces(200)
            .ProducesValidationProblem()
            .ProducesProblem(404)
            .ProducesProblem(400)
            .RequireAuthorization();

        group.MapPost("/{id:guid}/approve", ApproveMilestone)
            .WithName("ApproveMilestone")
            .WithSummary("Aprobar hito")
            .WithDescription("Aprueba un hito completado")
            .Produces(200)
            .ProducesValidationProblem()
            .ProducesProblem(404)
            .ProducesProblem(400)
            .RequireAuthorization();

        // Financial management
        group.MapPut("/{id:guid}/payment-terms", SetPaymentTerms)
            .WithName("SetMilestonePaymentTerms")
            .WithSummary("Establecer términos de pago")
            .WithDescription("Define el monto de pago asociado al hito")
            .Produces(200)
            .ProducesValidationProblem()
            .ProducesProblem(404)
            .RequireAuthorization();

        group.MapPost("/{id:guid}/trigger-payment", TriggerPayment)
            .WithName("TriggerMilestonePayment")
            .WithSummary("Activar pago del hito")
            .WithDescription("Activa el pago asociado a un hito aprobado")
            .Produces(200)
            .ProducesProblem(404)
            .ProducesProblem(400)
            .RequireAuthorization();

        group.MapGet("/project/{projectId:guid}/payment-summary", GetPaymentSummary)
            .WithName("GetMilestonePaymentSummary")
            .WithSummary("Resumen de pagos por hitos")
            .WithDescription("Retorna el resumen de pagos asociados a hitos del proyecto")
            .Produces<PaymentSummaryDto>();

        // Dependencies
        group.MapPut("/{id:guid}/dependencies", SetMilestoneDependencies)
            .WithName("SetMilestoneDependencies")
            .WithSummary("Establecer dependencias")
            .WithDescription("Establece los hitos predecesores y sucesores")
            .Produces(200)
            .ProducesValidationProblem()
            .ProducesProblem(404)
            .RequireAuthorization();

        group.MapPost("/validate-dependencies", ValidateDependencies)
            .WithName("ValidateMilestoneDependencies")
            .WithSummary("Validar dependencias")
            .WithDescription("Valida las dependencias para evitar referencias circulares")
            .Produces<List<string>>();

        // Dashboard
        group.MapGet("/project/{projectId:guid}/dashboard", GetMilestoneDashboard)
            .WithName("GetMilestoneDashboard")
            .WithSummary("Dashboard de hitos")
            .WithDescription("Retorna estadísticas y métricas de hitos del proyecto")
            .Produces<MilestoneDashboardDto>();

        // Validation
        group.MapGet("/validate-code", ValidateMilestoneCode)
            .WithName("ValidateMilestoneCode")
            .WithSummary("Validar código de hito")
            .WithDescription("Valida si un código de hito es único en el proyecto")
            .Produces<bool>();

        group.MapGet("/{id:guid}/can-delete", CanDeleteMilestone)
            .WithName("CanDeleteMilestone")
            .WithSummary("Verificar si se puede eliminar")
            .WithDescription("Valida si un hito puede ser eliminado")
            .Produces<bool>();

        group.MapGet("/{id:guid}/can-complete", CanCompleteMilestone)
            .WithName("CanCompleteMilestone")
            .WithSummary("Verificar si se puede completar")
            .WithDescription("Valida si un hito puede ser marcado como completado")
            .Produces<bool>();
    }

    // Query handlers
    private static async Task<IResult> GetMilestones(
        [AsParameters] MilestoneFilterDto filter,
        IMilestoneService milestoneService,
        CancellationToken cancellationToken)
    {
        var result = await milestoneService.GetMilestonesAsync(filter, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetMilestoneById(
        [FromRoute] Guid id,
        IMilestoneService milestoneService,
        CancellationToken cancellationToken)
    {
        var result = await milestoneService.GetMilestoneByIdAsync(id, cancellationToken);
        return result != null ? Results.Ok(result) : Results.NotFound($"Hito {id} no encontrado");
    }

    private static async Task<IResult> GetProjectMilestones(
        [FromRoute] Guid projectId,
        IMilestoneService milestoneService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        if (!await currentUserService.HasProjectAccessAsync(projectId))
        {
            return Results.Forbid();
        }

        var result = await milestoneService.GetProjectMilestonesAsync(projectId, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetUpcomingMilestones(
        [FromRoute] Guid projectId,
        [FromQuery] int days = 30,
        IMilestoneService milestoneService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        if (!await currentUserService.HasProjectAccessAsync(projectId))
        {
            return Results.Forbid();
        }

        var result = await milestoneService.GetUpcomingMilestonesAsync(projectId, days, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetOverdueMilestones(
        [FromRoute] Guid projectId,
        IMilestoneService milestoneService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        if (!await currentUserService.HasProjectAccessAsync(projectId))
        {
            return Results.Forbid();
        }

        var result = await milestoneService.GetOverdueMilestonesAsync(projectId, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetCriticalMilestones(
        [FromRoute] Guid projectId,
        IMilestoneService milestoneService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        if (!await currentUserService.HasProjectAccessAsync(projectId))
        {
            return Results.Forbid();
        }

        var result = await milestoneService.GetCriticalMilestonesAsync(projectId, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetContractualMilestones(
        [FromRoute] Guid projectId,
        IMilestoneService milestoneService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        if (!await currentUserService.HasProjectAccessAsync(projectId))
        {
            return Results.Forbid();
        }

        var result = await milestoneService.GetContractualMilestonesAsync(projectId, cancellationToken);
        return Results.Ok(result);
    }

    // Command handlers
    private static async Task<IResult> CreateMilestone(
        [FromBody] CreateMilestoneDto dto,
        IMilestoneService milestoneService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");

        if (!await currentUserService.HasProjectAccessAsync(dto.ProjectId, SimplifiedRoles.Project.ProjectManager))
        {
            return Results.Forbid();
        }

        var result = await milestoneService.CreateMilestoneAsync(dto, userId, cancellationToken);
        
        return result.IsSuccess 
            ? Results.Created($"/api/milestones/{result.Value}", result.Value)
            : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> UpdateMilestone(
        [FromRoute] Guid id,
        [FromBody] UpdateMilestoneDto dto,
        IMilestoneService milestoneService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        
        var result = await milestoneService.UpdateMilestoneAsync(id, dto, userId, cancellationToken);
        
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> DeleteMilestone(
        [FromRoute] Guid id,
        IMilestoneService milestoneService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        
        var result = await milestoneService.DeleteMilestoneAsync(id, userId, cancellationToken);
        
        return result.IsSuccess ? Results.NoContent() : Results.BadRequest(result.Error);
    }

    // Progress management handlers
    private static async Task<IResult> UpdateMilestoneProgress(
        [FromRoute] Guid id,
        [FromBody] UpdateMilestoneProgressDto dto,
        IMilestoneService milestoneService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        
        var result = await milestoneService.UpdateMilestoneProgressAsync(id, dto, userId, cancellationToken);
        
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> CompleteMilestone(
        [FromRoute] Guid id,
        [FromBody] CompleteMilestoneDto dto,
        IMilestoneService milestoneService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        
        var result = await milestoneService.CompleteMilestoneAsync(id, dto, userId, cancellationToken);
        
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> ApproveMilestone(
        [FromRoute] Guid id,
        [FromBody] ApproveMilestoneDto dto,
        IMilestoneService milestoneService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        
        // Solo Project Manager puede aprobar hitos
        var milestone = await milestoneService.GetMilestoneByIdAsync(id, cancellationToken);
        if (milestone == null)
            return Results.NotFound($"Hito {id} no encontrado");
            
        if (!await currentUserService.HasProjectAccessAsync(milestone.ProjectId, SimplifiedRoles.Project.ProjectManager))
        {
            return Results.Forbid();
        }
        
        var result = await milestoneService.ApproveMilestoneAsync(id, dto, userId, cancellationToken);
        
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    // Financial management handlers
    private static async Task<IResult> SetPaymentTerms(
        [FromRoute] Guid id,
        [FromBody] SetPaymentTermsRequest request,
        IMilestoneService milestoneService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        
        var result = await milestoneService.SetPaymentTermsAsync(id, request.Amount, request.Currency, userId, cancellationToken);
        
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> TriggerPayment(
        [FromRoute] Guid id,
        IMilestoneService milestoneService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        
        // Solo Project Manager puede activar pagos
        var milestone = await milestoneService.GetMilestoneByIdAsync(id, cancellationToken);
        if (milestone == null)
            return Results.NotFound($"Hito {id} no encontrado");
            
        if (!await currentUserService.HasProjectAccessAsync(milestone.ProjectId, SimplifiedRoles.Project.ProjectManager))
        {
            return Results.Forbid();
        }
        
        var result = await milestoneService.TriggerPaymentAsync(id, userId, cancellationToken);
        
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> GetPaymentSummary(
        [FromRoute] Guid projectId,
        [FromQuery] string currency = "USD",
        IMilestoneService milestoneService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        if (!await currentUserService.HasProjectAccessAsync(projectId))
        {
            return Results.Forbid();
        }

        var totalAmount = await milestoneService.GetTotalPaymentAmountAsync(projectId, currency, cancellationToken);
        var triggeredAmount = await milestoneService.GetTriggeredPaymentAmountAsync(projectId, currency, cancellationToken);
        
        var summary = new PaymentSummaryDto
        {
            ProjectId = projectId,
            Currency = currency,
            TotalPaymentAmount = totalAmount,
            TriggeredPaymentAmount = triggeredAmount,
            PendingPaymentAmount = totalAmount - triggeredAmount,
            PaymentProgress = totalAmount > 0 ? (triggeredAmount / totalAmount) * 100 : 0
        };
        
        return Results.Ok(summary);
    }

    // Dependency handlers
    private static async Task<IResult> SetMilestoneDependencies(
        [FromRoute] Guid id,
        [FromBody] SetDependenciesRequest request,
        IMilestoneService milestoneService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        
        var result = await milestoneService.SetMilestoneDependenciesAsync(
            id, 
            request.PredecessorMilestones, 
            request.SuccessorMilestones, 
            userId, 
            cancellationToken);
        
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> ValidateDependencies(
        [FromBody] ValidateDependenciesRequest request,
        IMilestoneService milestoneService,
        CancellationToken cancellationToken)
    {
        var errors = await milestoneService.ValidateMilestoneDependenciesAsync(
            request.MilestoneId, 
            request.PredecessorMilestones, 
            request.SuccessorMilestones, 
            cancellationToken);
            
        return Results.Ok(errors);
    }

    // Dashboard handler
    private static async Task<IResult> GetMilestoneDashboard(
        [FromRoute] Guid projectId,
        IMilestoneService milestoneService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        if (!await currentUserService.HasProjectAccessAsync(projectId))
        {
            return Results.Forbid();
        }

        var dashboard = await milestoneService.GetMilestoneDashboardAsync(projectId, cancellationToken);
        return Results.Ok(dashboard);
    }

    // Validation handlers
    private static async Task<IResult> ValidateMilestoneCode(
        [FromQuery] string code,
        [FromQuery] Guid projectId,
        [FromQuery] Guid? excludeId,
        IMilestoneService milestoneService,
        CancellationToken cancellationToken)
    {
        var isValid = await milestoneService.ValidateMilestoneCodeAsync(code, projectId, excludeId, cancellationToken);
        return Results.Ok(isValid);
    }

    private static async Task<IResult> CanDeleteMilestone(
        [FromRoute] Guid id,
        IMilestoneService milestoneService,
        CancellationToken cancellationToken)
    {
        var canDelete = await milestoneService.CanDeleteMilestoneAsync(id, cancellationToken);
        return Results.Ok(canDelete);
    }

    private static async Task<IResult> CanCompleteMilestone(
        [FromRoute] Guid id,
        IMilestoneService milestoneService,
        CancellationToken cancellationToken)
    {
        var canComplete = await milestoneService.CanCompleteMilestoneAsync(id, cancellationToken);
        return Results.Ok(canComplete);
    }
}

// Request DTOs
public record SetPaymentTermsRequest(decimal Amount, string Currency);
public record SetDependenciesRequest(string[]? PredecessorMilestones, string[]? SuccessorMilestones);
public record ValidateDependenciesRequest(Guid MilestoneId, string[] PredecessorMilestones, string[] SuccessorMilestones);

// Summary DTOs
public record PaymentSummaryDto
{
    public Guid ProjectId { get; init; }
    public string Currency { get; init; } = "USD";
    public decimal TotalPaymentAmount { get; init; }
    public decimal TriggeredPaymentAmount { get; init; }
    public decimal PendingPaymentAmount { get; init; }
    public decimal PaymentProgress { get; init; }
}