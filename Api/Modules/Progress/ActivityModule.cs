using Api.Authorization;
using Application.Interfaces.Auth;
using Application.Interfaces.Progress;
using Carter;
using Core.Constants;
using Core.DTOs.Common;
using Core.DTOs.Progress.Activities;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Progress;

/// <summary>
/// Endpoints para gestión de Actividades del Cronograma
/// </summary>
public class ActivityModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/activities")
            .WithTags("Actividades")
            .RequireAuthorization();

        // Query endpoints
        group.MapGet("/", GetActivities)
            .WithName("GetActivities")
            .WithSummary("Obtener actividades")
            .WithDescription("Retorna una lista paginada de actividades con filtros")
            .Produces<PagedResult<ActivityDto>>()
            .RequireAuthorization();

        group.MapGet("/{id:guid}", GetActivityById)
            .WithName("GetActivityById")
            .WithSummary("Obtener actividad por ID")
            .WithDescription("Retorna el detalle completo de una actividad")
            .Produces<ActivityDto>()
            .ProducesProblem(404);

        group.MapGet("/wbs-element/{wbsElementId:guid}", GetActivitiesByWBSElement)
            .WithName("GetActivitiesByWBSElement")
            .WithSummary("Obtener actividades por elemento WBS")
            .WithDescription("Retorna todas las actividades de un elemento WBS específico")
            .Produces<List<ActivityDto>>();

        group.MapGet("/project/{projectId:guid}/critical-path", GetCriticalPathActivities)
            .WithName("GetCriticalPathActivities")
            .WithSummary("Obtener actividades de ruta crítica")
            .WithDescription("Retorna las actividades que forman parte de la ruta crítica del proyecto")
            .Produces<List<ActivityDto>>()
            .RequireAuthorization();

        // Command endpoints
        group.MapPost("/", CreateActivity)
            .WithName("CreateActivity")
            .WithSummary("Crear actividad")
            .WithDescription("Crea una nueva actividad en el cronograma")
            .Produces<Guid>(201)
            .ProducesValidationProblem()
            .RequireAuthorization();

        group.MapPut("/{id:guid}", UpdateActivity)
            .WithName("UpdateActivity")
            .WithSummary("Actualizar actividad")
            .WithDescription("Actualiza la información básica de una actividad")
            .Produces(200)
            .ProducesValidationProblem()
            .ProducesProblem(404)
            .RequireAuthorization();

        group.MapDelete("/{id:guid}", DeleteActivity)
            .WithName("DeleteActivity")
            .WithSummary("Eliminar actividad")
            .WithDescription("Elimina una actividad si no tiene progreso registrado")
            .Produces(204)
            .ProducesProblem(404)
            .ProducesProblem(400)
            .RequireAuthorization();

        // Progress management
        group.MapPut("/{id:guid}/progress", UpdateActivityProgress)
            .WithName("UpdateActivityProgress")
            .WithSummary("Actualizar progreso de actividad")
            .WithDescription("Actualiza el porcentaje de avance y horas reales")
            .Produces(200)
            .ProducesValidationProblem()
            .ProducesProblem(404)
            .RequireAuthorization();

        group.MapPost("/{id:guid}/start", StartActivity)
            .WithName("StartActivity")
            .WithSummary("Iniciar actividad")
            .WithDescription("Marca una actividad como iniciada")
            .Produces(200)
            .ProducesProblem(404)
            .ProducesProblem(400)
            .RequireAuthorization();

        group.MapPost("/{id:guid}/complete", CompleteActivity)
            .WithName("CompleteActivity")
            .WithSummary("Completar actividad")
            .WithDescription("Marca una actividad como completada")
            .Produces(200)
            .ProducesProblem(404)
            .ProducesProblem(400)
            .RequireAuthorization();

        group.MapPost("/{id:guid}/suspend", SuspendActivity)
            .WithName("SuspendActivity")
            .WithSummary("Suspender actividad")
            .WithDescription("Suspende una actividad en progreso")
            .Produces(200)
            .ProducesProblem(404)
            .ProducesProblem(400)
            .RequireAuthorization();

        group.MapPost("/{id:guid}/resume", ResumeActivity)
            .WithName("ResumeActivity")
            .WithSummary("Reanudar actividad")
            .WithDescription("Reanuda una actividad suspendida")
            .Produces(200)
            .ProducesProblem(404)
            .ProducesProblem(400)
            .RequireAuthorization();

        group.MapPost("/{id:guid}/cancel", CancelActivity)
            .WithName("CancelActivity")
            .WithSummary("Cancelar actividad")
            .WithDescription("Cancela una actividad")
            .Produces(200)
            .ProducesProblem(404)
            .ProducesProblem(400)
            .RequireAuthorization();

        // Bulk operations
        group.MapPost("/bulk-update-progress", BulkUpdateProgress)
            .WithName("BulkUpdateActivityProgress")
            .WithSummary("Actualizar progreso en lote")
            .WithDescription("Actualiza el progreso de múltiples actividades")
            .Produces<BulkOperationResult>(200)
            .ProducesValidationProblem()
            .RequireAuthorization();

        group.MapPost("/bulk-create", BulkCreateActivities)
            .WithName("BulkCreateActivities")
            .WithSummary("Crear actividades en lote")
            .WithDescription("Crea múltiples actividades en una sola operación")
            .Produces<BulkOperationResult>(200)
            .ProducesValidationProblem()
            .RequireAuthorization();

        // Schedule management
        group.MapPut("/{id:guid}/schedule", UpdateActivitySchedule)
            .WithName("UpdateActivitySchedule")
            .WithSummary("Actualizar fechas de actividad")
            .WithDescription("Actualiza las fechas planificadas de una actividad")
            .Produces(200)
            .ProducesValidationProblem()
            .ProducesProblem(404)
            .RequireAuthorization();

        group.MapPut("/{id:guid}/dependencies", SetActivityDependencies)
            .WithName("SetActivityDependencies")
            .WithSummary("Establecer dependencias")
            .WithDescription("Establece las actividades predecesoras y sucesoras")
            .Produces(200)
            .ProducesValidationProblem()
            .ProducesProblem(404)
            .RequireAuthorization();

        group.MapPost("/project/{projectId:guid}/recalculate-critical-path", RecalculateCriticalPath)
            .WithName("RecalculateCriticalPath")
            .WithSummary("Recalcular ruta crítica")
            .WithDescription("Recalcula la ruta crítica del proyecto")
            .Produces(200)
            .ProducesProblem(404)
            .RequireAuthorization();

        // Resource management
        group.MapPost("/{id:guid}/resources", AssignResources)
            .WithName("AssignActivityResources")
            .WithSummary("Asignar recursos")
            .WithDescription("Asigna recursos a una actividad")
            .Produces(200)
            .ProducesValidationProblem()
            .ProducesProblem(404)
            .RequireAuthorization();

        group.MapPut("/{id:guid}/resource-rate", UpdateResourceRate)
            .WithName("UpdateActivityResourceRate")
            .WithSummary("Actualizar tarifa de recurso")
            .WithDescription("Actualiza la tarifa del recurso asignado")
            .Produces(200)
            .ProducesValidationProblem()
            .ProducesProblem(404)
            .RequireAuthorization();

        // Validation
        group.MapGet("/validate-code", ValidateActivityCode)
            .WithName("ValidateActivityCode")
            .WithSummary("Validar código de actividad")
            .WithDescription("Valida si un código de actividad es único en el proyecto")
            .Produces<bool>();

        group.MapGet("/{id:guid}/can-delete", CanDeleteActivity)
            .WithName("CanDeleteActivity")
            .WithSummary("Verificar si se puede eliminar")
            .WithDescription("Valida si una actividad puede ser eliminada")
            .Produces<bool>();

        group.MapPost("/validate-dependencies", ValidateDependencies)
            .WithName("ValidateActivityDependencies")
            .WithSummary("Validar dependencias")
            .WithDescription("Valida las dependencias para evitar referencias circulares")
            .Produces<List<string>>();
    }

    // Query handlers
    private static async Task<IResult> GetActivities(
        [AsParameters] ActivityFilterDto filter,
        IActivityService activityService,
        CancellationToken cancellationToken)
    {
        var result = await activityService.GetActivitiesAsync(filter, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetActivityById(
        [FromRoute] Guid id,
        IActivityService activityService,
        CancellationToken cancellationToken)
    {
        var result = await activityService.GetActivityByIdAsync(id, cancellationToken);
        return result != null ? Results.Ok(result) : Results.NotFound($"Actividad {id} no encontrada");
    }

    private static async Task<IResult> GetActivitiesByWBSElement(
        [FromRoute] Guid wbsElementId,
        IActivityService activityService,
        CancellationToken cancellationToken)
    {
        var result = await activityService.GetActivitiesByWBSElementAsync(wbsElementId, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetCriticalPathActivities(
        [FromRoute] Guid projectId,
        IActivityService activityService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        if (!await currentUserService.HasProjectAccessAsync(projectId))
        {
            return Results.Forbid();
        }

        var result = await activityService.GetCriticalPathActivitiesAsync(projectId, cancellationToken);
        return Results.Ok(result);
    }

    // Command handlers
    private static async Task<IResult> CreateActivity(
        [FromBody] CreateActivityDto dto,
        IActivityService activityService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");

        // Verificar acceso al proyecto a través del WBS element
        // TODO: Verificar permisos según el elemento WBS

        var result = await activityService.CreateActivityAsync(dto, userId, cancellationToken);
        
        return result.IsSuccess 
            ? Results.Created($"/api/activities/{result.Value}", result.Value)
            : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> UpdateActivity(
        [FromRoute] Guid id,
        [FromBody] UpdateActivityDto dto,
        IActivityService activityService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        
        var result = await activityService.UpdateActivityAsync(id, dto, userId, cancellationToken);
        
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> DeleteActivity(
        [FromRoute] Guid id,
        IActivityService activityService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        
        var result = await activityService.DeleteActivityAsync(id, userId, cancellationToken);
        
        return result.IsSuccess ? Results.NoContent() : Results.BadRequest(result.Error);
    }

    // Progress management handlers
    private static async Task<IResult> UpdateActivityProgress(
        [FromRoute] Guid id,
        [FromBody] UpdateActivityProgressDto dto,
        IActivityService activityService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        
        var result = await activityService.UpdateActivityProgressAsync(id, dto, userId, cancellationToken);
        
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> StartActivity(
        [FromRoute] Guid id,
        IActivityService activityService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        
        var result = await activityService.StartActivityAsync(id, userId, cancellationToken);
        
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> CompleteActivity(
        [FromRoute] Guid id,
        [FromBody] CompleteActivityRequest? request,
        IActivityService activityService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        
        var result = await activityService.CompleteActivityAsync(id, request?.ActualEndDate, userId, cancellationToken);
        
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> SuspendActivity(
        [FromRoute] Guid id,
        [FromBody] SuspendActivityRequest request,
        IActivityService activityService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        
        var result = await activityService.SuspendActivityAsync(id, request.Reason, userId, cancellationToken);
        
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> ResumeActivity(
        [FromRoute] Guid id,
        IActivityService activityService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        
        var result = await activityService.ResumeActivityAsync(id, userId, cancellationToken);
        
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> CancelActivity(
        [FromRoute] Guid id,
        [FromBody] CancelActivityRequest request,
        IActivityService activityService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        
        var result = await activityService.CancelActivityAsync(id, request.Reason, userId, cancellationToken);
        
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    // Bulk operation handlers
    private static async Task<IResult> BulkUpdateProgress(
        [FromBody] BulkUpdateActivitiesDto dto,
        IActivityService activityService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        
        var result = await activityService.BulkUpdateProgressAsync(dto, userId, cancellationToken);
        
        return result.IsSuccess 
            ? Results.Ok(new BulkOperationResult(dto.Updates.Count, dto.Updates.Count)) 
            : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> BulkCreateActivities(
        [FromBody] List<CreateActivityDto> activities,
        IActivityService activityService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        
        var result = await activityService.BulkCreateActivitiesAsync(activities, userId, cancellationToken);
        
        return result.IsSuccess 
            ? Results.Ok(new BulkOperationResult(result.Value, activities.Count))
            : Results.BadRequest(result.Error);
    }

    // Schedule management handlers
    private static async Task<IResult> UpdateActivitySchedule(
        [FromRoute] Guid id,
        [FromBody] UpdateActivityScheduleRequest request,
        IActivityService activityService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        
        var result = await activityService.UpdateActivityScheduleAsync(
            id, 
            request.PlannedStartDate, 
            request.PlannedEndDate, 
            userId, 
            cancellationToken);
        
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> SetActivityDependencies(
        [FromRoute] Guid id,
        [FromBody] SetDependenciesRequest request,
        IActivityService activityService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        
        var result = await activityService.SetActivityDependenciesAsync(
            id, 
            request.PredecessorActivities, 
            request.SuccessorActivities, 
            userId, 
            cancellationToken);
        
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> RecalculateCriticalPath(
        [FromRoute] Guid projectId,
        IActivityService activityService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        if (!await currentUserService.HasProjectAccessAsync(projectId, SimplifiedRoles.Project.ProjectManager))
        {
            return Results.Forbid();
        }
        
        var result = await activityService.RecalculateCriticalPathAsync(projectId, cancellationToken);
        
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    // Resource management handlers
    private static async Task<IResult> AssignResources(
        [FromRoute] Guid id,
        [FromBody] AssignResourcesRequest request,
        IActivityService activityService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        
        var result = await activityService.AssignResourcesAsync(id, request.ResourceIds, userId, cancellationToken);
        
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> UpdateResourceRate(
        [FromRoute] Guid id,
        [FromBody] UpdateResourceRateRequest request,
        IActivityService activityService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        
        var result = await activityService.UpdateResourceRateAsync(id, request.Rate, userId, cancellationToken);
        
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    // Validation handlers
    private static async Task<IResult> ValidateActivityCode(
        [FromQuery] string code,
        [FromQuery] Guid projectId,
        [FromQuery] Guid? excludeId,
        IActivityService activityService,
        CancellationToken cancellationToken)
    {
        var isValid = await activityService.ValidateActivityCodeAsync(code, projectId, excludeId, cancellationToken);
        return Results.Ok(isValid);
    }

    private static async Task<IResult> CanDeleteActivity(
        [FromRoute] Guid id,
        IActivityService activityService,
        CancellationToken cancellationToken)
    {
        var canDelete = await activityService.CanDeleteActivityAsync(id, cancellationToken);
        return Results.Ok(canDelete);
    }

    private static async Task<IResult> ValidateDependencies(
        [FromBody] ValidateDependenciesRequest request,
        IActivityService activityService,
        CancellationToken cancellationToken)
    {
        var errors = await activityService.ValidateDependenciesAsync(
            request.ActivityId, 
            request.PredecessorActivities, 
            request.SuccessorActivities, 
            cancellationToken);
            
        return Results.Ok(errors);
    }
}

// Request DTOs
public record CompleteActivityRequest(DateTime? ActualEndDate);
public record SuspendActivityRequest(string Reason);
public record CancelActivityRequest(string Reason);
public record UpdateActivityScheduleRequest(DateTime PlannedStartDate, DateTime PlannedEndDate);
public record SetDependenciesRequest(string[]? PredecessorActivities, string[]? SuccessorActivities);
public record AssignResourcesRequest(List<Guid> ResourceIds);
public record UpdateResourceRateRequest(decimal Rate);
public record ValidateDependenciesRequest(Guid ActivityId, string[] PredecessorActivities, string[] SuccessorActivities);