using Api.Authorization;
using Application.Interfaces.Auth;
using Application.Interfaces.Cost;
using Carter;
using Core.Constants;
using Core.DTOs.Common;
using Core.DTOs.WorkPackages;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Projects;

/// <summary>
/// Endpoints para gestión de Paquetes de Trabajo
/// </summary>
public class WorkPackageModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/work-packages")
            .WithTags("Paquetes de Trabajo")
            .RequireAuthorization();

        // Endpoints de consulta
        group.MapGet("/project/{projectId:guid}", GetWorkPackages)
            .WithName("GetWorkPackages")
            .WithSummary("Obtener paquetes de trabajo de un proyecto")
            .WithDescription("Retorna una lista paginada de paquetes de trabajo para un proyecto específico")
            .Produces<PagedResult<WorkPackageDto>>()
            .RequireAuthorization();

        group.MapGet("/{id:guid}", GetWorkPackageById)
            .WithName("GetWorkPackageById")
            .WithSummary("Obtener paquete de trabajo por ID")
            .WithDescription("Retorna el detalle completo de un paquete de trabajo incluyendo progreso y actividades")
            .Produces<WorkPackageDetailDto>()
            .ProducesProblem(404);

        group.MapGet("/control-account/{controlAccountId:guid}", GetWorkPackagesByControlAccount)
            .WithName("GetWorkPackagesByCA")
            .WithSummary("Obtener paquetes de trabajo por cuenta de control")
            .WithDescription("Retorna todos los paquetes de trabajo asignados a una cuenta de control específica")
            .Produces<List<WorkPackageDto>>();

        group.MapGet("/{id:guid}/progress-history", GetWorkPackageProgressHistory)
            .WithName("GetWorkPackageProgressHistory")
            .WithSummary("Obtener historial de progreso")
            .WithDescription("Retorna el historial completo de progreso de un paquete de trabajo")
            .Produces<List<WorkPackageProgressDto>>()
            .ProducesProblem(404);

        // Endpoints de comandos
        group.MapPost("/", CreateWorkPackage)
            .WithName("CreateWorkPackage")
            .WithSummary("Crear paquete de trabajo")
            .WithDescription("Crea un nuevo paquete de trabajo directamente (sin pasar por WBS)")
            .Produces<Guid>(201)
            .ProducesValidationProblem()
            .RequireAuthorization();

        group.MapPut("/{id:guid}", UpdateWorkPackage)
            .WithName("UpdateWorkPackage")
            .WithSummary("Actualizar paquete de trabajo")
            .WithDescription("Actualiza la información básica de un paquete de trabajo")
            .Produces(200)
            .ProducesValidationProblem()
            .ProducesProblem(404)
            .RequireAuthorization();

        group.MapPut("/{id:guid}/progress", UpdateWorkPackageProgress)
            .WithName("UpdateWorkPackageProgress")
            .WithSummary("Actualizar progreso del paquete de trabajo")
            .WithDescription("Actualiza el porcentaje de avance y métricas de progreso")
            .Produces(200)
            .ProducesValidationProblem()
            .ProducesProblem(404)
            .RequireAuthorization();

        // Endpoints de gestión de estado
        group.MapPost("/{id:guid}/start", StartWorkPackage)
            .WithName("StartWorkPackage")
            .WithSummary("Iniciar paquete de trabajo")
            .WithDescription("Marca el paquete de trabajo como iniciado y registra la fecha de inicio real")
            .Produces(200)
            .ProducesProblem(404)
            .ProducesProblem(400)
            .RequireAuthorization();

        group.MapPost("/{id:guid}/complete", CompleteWorkPackage)
            .WithName("CompleteWorkPackage")
            .WithSummary("Completar paquete de trabajo")
            .WithDescription("Marca el paquete de trabajo como completado y registra la fecha de finalización")
            .Produces(200)
            .ProducesProblem(404)
            .ProducesProblem(400)
            .RequireAuthorization();

        group.MapPost("/{id:guid}/baseline", BaselineWorkPackage)
            .WithName("BaselineWorkPackage")
            .WithSummary("Establecer línea base")
            .WithDescription("Establece la línea base del paquete de trabajo para medición de desempeño")
            .Produces(200)
            .ProducesProblem(404)
            .ProducesProblem(400)
            .RequireAuthorization();

        group.MapDelete("/{id:guid}", DeleteWorkPackage)
            .WithName("DeleteWorkPackage")
            .WithSummary("Eliminar paquete de trabajo")
            .WithDescription("Elimina un paquete de trabajo si no tiene actividades ni progreso registrado")
            .Produces(204)
            .ProducesProblem(404)
            .ProducesProblem(400)
            .RequireAuthorization();

        // Endpoints de gestión de actividades
        group.MapPost("/{workPackageId:guid}/activities", AddActivityToWorkPackage)
            .WithName("AddActivityToWorkPackage")
            .WithSummary("Agregar actividad al paquete de trabajo")
            .WithDescription("Agrega una nueva actividad o tarea al paquete de trabajo")
            .Produces<Guid>(201)
            .ProducesValidationProblem()
            .ProducesProblem(404)
            .RequireAuthorization();

        group.MapPut("/activities/{activityId:guid}/progress", UpdateActivityProgress)
            .WithName("UpdateActivityProgress")
            .WithSummary("Actualizar progreso de actividad")
            .WithDescription("Actualiza el porcentaje de avance y horas reales de una actividad")
            .Produces(200)
            .ProducesValidationProblem()
            .ProducesProblem(404)
            .RequireAuthorization();

        // Operaciones masivas
        group.MapPost("/bulk-update-progress", BulkUpdateProgress)
            .WithName("BulkUpdateProgress")
            .WithSummary("Actualizar progreso en lote")
            .WithDescription("Actualiza el progreso de múltiples paquetes de trabajo en una sola operación")
            .Produces<WorkPackageBulkOperationResult>(200)
            .ProducesValidationProblem()
            .RequireAuthorization();

        // Métricas de desempeño
        group.MapGet("/project/{projectId:guid}/performance", GetProjectWorkPackagePerformance)
            .WithName("GetProjectWorkPackagePerformance")
            .WithSummary("Obtener métricas de desempeño")
            .WithDescription("Retorna métricas agregadas de desempeño de todos los paquetes de trabajo del proyecto")
            .Produces<WorkPackagePerformanceDto>()
            .RequireAuthorization();

        // Exportar
        group.MapGet("/project/{projectId:guid}/export", ExportWorkPackages)
            .WithName("ExportWorkPackages")
            .WithSummary("Exportar paquetes de trabajo")
            .WithDescription("Exporta los paquetes de trabajo del proyecto a Excel")
            .Produces(200)
            .RequireAuthorization();
    }

    // Manejadores de consultas
    private static async Task<IResult> GetWorkPackages(
        [FromRoute] Guid projectId,
        [AsParameters] QueryParameters parameters,
        IWorkPackageService workPackageService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        if (!await currentUserService.HasProjectAccessAsync(projectId))
        {
            return Results.Forbid();
        }

        var result = await workPackageService.GetWorkPackagesAsync(projectId, parameters, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetWorkPackageById(
        [FromRoute] Guid id,
        IWorkPackageService workPackageService,
        CancellationToken cancellationToken)
    {
        var result = await workPackageService.GetWorkPackageByIdAsync(id, cancellationToken);
        return result != null ? Results.Ok(result) : Results.NotFound($"Paquete de trabajo {id} no encontrado");
    }

    private static async Task<IResult> GetWorkPackagesByControlAccount(
        [FromRoute] Guid controlAccountId,
        IWorkPackageService workPackageService,
        CancellationToken cancellationToken)
    {
        var result = await workPackageService.GetWorkPackagesByControlAccountAsync(controlAccountId, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetWorkPackageProgressHistory(
        [FromRoute] Guid id,
        IWorkPackageService workPackageService,
        CancellationToken cancellationToken)
    {
        var result = await workPackageService.GetWorkPackageProgressHistoryAsync(id, cancellationToken);
        return Results.Ok(result);
    }

    // Manejadores de comandos
    private static async Task<IResult> CreateWorkPackage(
        [FromBody] CreateWorkPackageDto dto,
        IWorkPackageService workPackageService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");

        if (!await currentUserService.HasProjectAccessAsync(dto.ProjectId, SimplifiedRoles.Project.ProjectManager))
        {
            return Results.Forbid();
        }

        var result = await workPackageService.CreateWorkPackageAsync(dto, userId, cancellationToken);
        
        return result.IsSuccess 
            ? Results.Created($"/api/work-packages/{result.Value}", result.Value)
            : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> UpdateWorkPackage(
        [FromRoute] Guid id,
        [FromBody] UpdateWorkPackageDto dto,
        IWorkPackageService workPackageService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        
        var result = await workPackageService.UpdateWorkPackageAsync(id, dto, userId, cancellationToken);
        
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> UpdateWorkPackageProgress(
        [FromRoute] Guid id,
        [FromBody] UpdateWorkPackageProgressDto dto,
        IWorkPackageService workPackageService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        
        var result = await workPackageService.UpdateWorkPackageProgressAsync(id, dto, userId, cancellationToken);
        
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> StartWorkPackage(
        [FromRoute] Guid id,
        IWorkPackageService workPackageService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        
        var result = await workPackageService.StartWorkPackageAsync(id, userId, cancellationToken);
        
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> CompleteWorkPackage(
        [FromRoute] Guid id,
        IWorkPackageService workPackageService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        
        var result = await workPackageService.CompleteWorkPackageAsync(id, userId, cancellationToken);
        
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> BaselineWorkPackage(
        [FromRoute] Guid id,
        IWorkPackageService workPackageService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        
        var result = await workPackageService.BaselineWorkPackageAsync(id, userId, cancellationToken);
        
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> DeleteWorkPackage(
        [FromRoute] Guid id,
        IWorkPackageService workPackageService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        
        var result = await workPackageService.DeleteWorkPackageAsync(id, userId, cancellationToken);
        
        return result.IsSuccess ? Results.NoContent() : Results.BadRequest(result.Error);
    }

    // Manejadores de actividades
    private static async Task<IResult> AddActivityToWorkPackage(
        [FromRoute] Guid workPackageId,
        [FromBody] CreateActivityDto dto,
        IWorkPackageService workPackageService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");

        var result = await workPackageService.AddActivityToWorkPackageAsync(workPackageId, dto, userId, cancellationToken);
        
        return result.IsSuccess 
            ? Results.Created($"/api/work-packages/{workPackageId}/activities/{result.Value}", result.Value)
            : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> UpdateActivityProgress(
        [FromRoute] Guid activityId,
        [FromBody] UpdateActivityProgressDto dto,
        IWorkPackageService workPackageService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        
        var result = await workPackageService.UpdateActivityProgressAsync(
            activityId, 
            dto.PercentComplete, 
            dto.ActualHours, 
            userId, 
            cancellationToken);
        
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    // Manejadores de operaciones masivas
    private static async Task<IResult> BulkUpdateProgress(
        [FromBody] BulkUpdateProgressDto dto,
        IWorkPackageService workPackageService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");

        // TODO: Implementar actualización masiva en el servicio
        return Results.Ok(new WorkPackageBulkOperationResult(0, dto.Updates.Count));
    }

    // Manejador de métricas de desempeño
    private static async Task<IResult> GetProjectWorkPackagePerformance(
        [FromRoute] Guid projectId,
        IWorkPackageService workPackageService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        if (!await currentUserService.HasProjectAccessAsync(projectId))
        {
            return Results.Forbid();
        }

        // TODO: Implementar métricas de desempeño en el servicio
        return Results.Ok(new WorkPackagePerformanceDto
        {
            TotalWorkPackages = 0,
            CompletedWorkPackages = 0,
            InProgressWorkPackages = 0,
            NotStartedWorkPackages = 0,
            OverallProgress = 0,
            AverageCPI = 1.0m,
            AverageSPI = 1.0m
        });
    }

    // Manejador de exportación
    private static async Task<IResult> ExportWorkPackages(
        [FromRoute] Guid projectId,
        IWorkPackageService workPackageService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        if (!await currentUserService.HasProjectAccessAsync(projectId))
        {
            return Results.Forbid();
        }

        // TODO: Implementar funcionalidad de exportación
        var bytes = Array.Empty<byte>();
        return Results.File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"WorkPackages_{projectId}.xlsx");
    }
}

// DTOs de solicitud
public record UpdateActivityProgressDto(decimal PercentComplete, decimal ActualHours);
public record BulkUpdateProgressDto(List<WorkPackageProgressUpdate> Updates);
public record WorkPackageProgressUpdate(Guid WorkPackageId, decimal ProgressPercentage, string? Notes);

// DTOs de respuesta
public record WorkPackageBulkOperationResult(int SuccessCount, int TotalCount);
public record WorkPackagePerformanceDto
{
    public int TotalWorkPackages { get; set; }
    public int CompletedWorkPackages { get; set; }
    public int InProgressWorkPackages { get; set; }
    public int NotStartedWorkPackages { get; set; }
    public decimal OverallProgress { get; set; }
    public decimal AverageCPI { get; set; }
    public decimal AverageSPI { get; set; }
}