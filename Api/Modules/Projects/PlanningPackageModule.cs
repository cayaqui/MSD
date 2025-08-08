using Api.Authorization;
using Application.Interfaces.Auth;
using Application.Interfaces.Projects;
using Carter;
using Core.Constants;
using Core.DTOs.Common;
using Core.DTOs.Cost.PlanningPackages;
using Core.DTOs.Projects.WorkPackageDetails;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Projects;

/// <summary>
/// Endpoints para gestión de Paquetes de Planificación
/// </summary>
public class PlanningPackageModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/planning-packages")
            .WithTags("Paquetes de Planificación")
            .RequireAuthorization();

        // Endpoints de consulta
        group.MapGet("/project/{projectId:guid}", GetPlanningPackages)
            .WithName("GetPlanningPackages")
            .WithSummary("Obtener paquetes de planificación de un proyecto")
            .WithDescription("Retorna una lista paginada de paquetes de planificación para un proyecto específico")
            .Produces<PagedResult<PlanningPackageDto>>()
            .RequireAuthorization();

        group.MapGet("/{id:guid}", GetPlanningPackageById)
            .WithName("GetPlanningPackageById")
            .WithSummary("Obtener paquete de planificación por ID")
            .WithDescription("Retorna el detalle completo de un paquete de planificación")
            .Produces<PlanningPackageDto>()
            .ProducesProblem(404);

        group.MapGet("/control-account/{controlAccountId:guid}", GetPlanningPackagesByControlAccount)
            .WithName("GetPlanningPackagesByControlAccount")
            .WithSummary("Obtener paquetes de planificación por cuenta de control")
            .WithDescription("Retorna todos los paquetes de planificación asignados a una cuenta de control")
            .Produces<List<PlanningPackageDto>>();

        group.MapGet("/project/{projectId:guid}/unconverted", GetUnconvertedPlanningPackages)
            .WithName("GetUnconvertedPlanningPackages")
            .WithSummary("Obtener paquetes de planificación no convertidos")
            .WithDescription("Retorna solo los paquetes de planificación que aún no han sido convertidos a paquetes de trabajo")
            .Produces<List<PlanningPackageDto>>()
            .RequireAuthorization();

        // Endpoints de comandos
        group.MapPost("/", CreatePlanningPackage)
            .WithName("CreatePlanningPackage")
            .WithSummary("Crear paquete de planificación")
            .WithDescription("Crea un nuevo paquete de planificación directamente (sin pasar por WBS)")
            .Produces<Guid>(201)
            .ProducesValidationProblem()
            .RequireAuthorization();

        group.MapPut("/{id:guid}", UpdatePlanningPackage)
            .WithName("UpdatePlanningPackage")
            .WithSummary("Actualizar paquete de planificación")
            .WithDescription("Actualiza la información de un paquete de planificación")
            .Produces(200)
            .ProducesValidationProblem()
            .ProducesProblem(404)
            .RequireAuthorization();

        group.MapPost("/{id:guid}/convert-to-work-package", ConvertToWorkPackage)
            .WithName("ConvertPlanningToWorkPackage")
            .WithSummary("Convertir a paquete de trabajo")
            .WithDescription("Convierte un paquete de planificación a paquete de trabajo")
            .Produces(200)
            .ProducesValidationProblem()
            .ProducesProblem(404)
            .RequireAuthorization();

        group.MapDelete("/{id:guid}", DeletePlanningPackage)
            .WithName("DeletePlanningPackage")
            .WithSummary("Eliminar paquete de planificación")
            .WithDescription("Elimina un paquete de planificación no convertido")
            .Produces(204)
            .ProducesProblem(404)
            .ProducesProblem(400)
            .RequireAuthorization();

        // Operaciones masivas
        group.MapPost("/bulk-create", BulkCreatePlanningPackages)
            .WithName("BulkCreatePlanningPackages")
            .WithSummary("Crear múltiples paquetes de planificación")
            .WithDescription("Crea múltiples paquetes de planificación en una sola operación")
            .Produces<PlanningPackageBulkOperationResult>(200)
            .ProducesValidationProblem()
            .RequireAuthorization();

        group.MapPost("/bulk-convert", BulkConvertToWorkPackages)
            .WithName("BulkConvertToWorkPackages")
            .WithSummary("Convertir múltiples paquetes a trabajo")
            .WithDescription("Convierte múltiples paquetes de planificación a paquetes de trabajo")
            .Produces<PlanningPackageBulkOperationResult>(200)
            .ProducesValidationProblem()
            .RequireAuthorization();

        // Operaciones de presupuesto
        group.MapGet("/control-account/{controlAccountId:guid}/budget-summary", GetPlanningPackageBudgetSummary)
            .WithName("GetPlanningPackageBudgetSummary")
            .WithSummary("Obtener resumen de presupuesto")
            .WithDescription("Retorna el resumen de presupuesto de paquetes de planificación por cuenta de control")
            .Produces<PlanningPackageBudgetSummaryDto>();

        group.MapPost("/redistribute-budget", RedistributeBudget)
            .WithName("RedistributePlanningPackageBudget")
            .WithSummary("Redistribuir presupuesto")
            .WithDescription("Redistribuye el presupuesto entre paquetes de planificación de una cuenta de control")
            .Produces(200)
            .ProducesValidationProblem()
            .RequireAuthorization();

        // Exportar
        group.MapGet("/project/{projectId:guid}/export", ExportPlanningPackages)
            .WithName("ExportPlanningPackages")
            .WithSummary("Exportar paquetes de planificación")
            .WithDescription("Exporta los paquetes de planificación del proyecto a Excel")
            .Produces(200)
            .RequireAuthorization();
    }

    // Manejadores de consultas
    private static async Task<IResult> GetPlanningPackages(
        [FromRoute] Guid projectId,
        [AsParameters] QueryParameters parameters,
        IWBSService wbsService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        if (!await currentUserService.HasProjectAccessAsync(projectId))
        {
            return Results.Forbid();
        }

        // TODO: Implementar consulta específica de paquetes de planificación
        var emptyResult = new PagedResult<PlanningPackageDto>
        {
            Items = new List<PlanningPackageDto>(),
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize,
            TotalPages = 0
        };
        
        return Results.Ok(emptyResult);
    }

    private static async Task<IResult> GetPlanningPackageById(
        [FromRoute] Guid id,
        IWBSService wbsService,
        CancellationToken cancellationToken)
    {
        // TODO: Implementar consulta específica de paquetes de planificación
        return Results.NotFound($"Paquete de planificación {id} no encontrado");
    }

    private static async Task<IResult> GetPlanningPackagesByControlAccount(
        [FromRoute] Guid controlAccountId,
        IWBSService wbsService,
        CancellationToken cancellationToken)
    {
        // TODO: Implementar consulta específica de paquetes de planificación
        return Results.Ok(new List<PlanningPackageDto>());
    }

    private static async Task<IResult> GetUnconvertedPlanningPackages(
        [FromRoute] Guid projectId,
        IWBSService wbsService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        if (!await currentUserService.HasProjectAccessAsync(projectId))
        {
            return Results.Forbid();
        }

        // TODO: Implementar consulta específica de paquetes de planificación
        return Results.Ok(new List<PlanningPackageDto>());
    }

    // Manejadores de comandos
    private static async Task<IResult> CreatePlanningPackage(
        [FromBody] CreatePlanningPackageDto dto,
        IWBSService wbsService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");

        // TODO: Verificar acceso al proyecto a través de cuenta de control
        // if (!await currentUserService.HasProjectAccessAsync(dto.ProjectId, SimplifiedRoles.Project.ProjectManager))
        // {
        //     return Results.Forbid();
        // }

        // TODO: Implementar creación de paquetes de planificación
        return Results.BadRequest("Creación de paquetes de planificación no implementada");
    }

    private static async Task<IResult> UpdatePlanningPackage(
        [FromRoute] Guid id,
        [FromBody] UpdatePlanningPackageDto dto,
        IWBSService wbsService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        
        // TODO: Implementar actualización de paquetes de planificación
        return Results.BadRequest("Actualización de paquetes de planificación no implementada");
    }

    private static async Task<IResult> ConvertToWorkPackage(
        [FromRoute] Guid id,
        [FromBody] ConvertPlanningToWorkPackageDto dto,
        IWBSService wbsService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        
        // Usar el método existente del servicio WBS para conversión
        var result = await wbsService.ConvertPlanningPackageToWorkPackageAsync(id, dto, userId, cancellationToken);
        
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> DeletePlanningPackage(
        [FromRoute] Guid id,
        IWBSService wbsService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        
        // TODO: Implementar eliminación de paquetes de planificación
        return Results.BadRequest("Eliminación de paquetes de planificación no implementada");
    }

    // Manejadores de operaciones masivas
    private static async Task<IResult> BulkCreatePlanningPackages(
        [FromBody] BulkCreatePlanningPackagesDto dto,
        IWBSService wbsService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");

        // TODO: Implementar creación masiva
        return Results.Ok(new PlanningPackageBulkOperationResult(0, dto.PlanningPackages.Count));
    }

    private static async Task<IResult> BulkConvertToWorkPackages(
        [FromBody] BulkConvertPlanningPackagesDto dto,
        IWBSService wbsService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");

        // TODO: Implementar conversión masiva
        return Results.Ok(new PlanningPackageBulkOperationResult(0, dto.PlanningPackageIds.Count));
    }

    // Manejadores de operaciones de presupuesto
    private static async Task<IResult> GetPlanningPackageBudgetSummary(
        [FromRoute] Guid controlAccountId,
        IWBSService wbsService,
        CancellationToken cancellationToken)
    {
        // TODO: Implementar resumen de presupuesto
        return Results.Ok(new PlanningPackageBudgetSummaryDto
        {
            ControlAccountId = controlAccountId,
            TotalBudget = 0,
            AllocatedBudget = 0,
            UnallocatedBudget = 0,
            PlanningPackageCount = 0
        });
    }

    private static async Task<IResult> RedistributeBudget(
        [FromBody] RedistributeBudgetDto dto,
        IWBSService wbsService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");

        // TODO: Implementar redistribución de presupuesto
        return Results.BadRequest("Redistribución de presupuesto no implementada");
    }

    // Manejador de exportación
    private static async Task<IResult> ExportPlanningPackages(
        [FromRoute] Guid projectId,
        IWBSService wbsService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        if (!await currentUserService.HasProjectAccessAsync(projectId))
        {
            return Results.Forbid();
        }

        // TODO: Implementar funcionalidad de exportación
        var bytes = Array.Empty<byte>();
        return Results.File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"PlanningPackages_{projectId}.xlsx");
    }
}

// DTOs de solicitud
public record BulkCreatePlanningPackagesDto(List<CreatePlanningPackageDto> PlanningPackages);
public record BulkConvertPlanningPackagesDto(List<Guid> PlanningPackageIds, ConvertPlanningToWorkPackageDto ConversionDetails);
public record RedistributeBudgetDto(Guid ControlAccountId, List<BudgetAllocation> Allocations);
public record BudgetAllocation(Guid PlanningPackageId, decimal NewBudget);

// DTOs de respuesta
public record PlanningPackageBulkOperationResult(int SuccessCount, int TotalCount);
public record PlanningPackageBudgetSummaryDto
{
    public Guid ControlAccountId { get; set; }
    public decimal TotalBudget { get; set; }
    public decimal AllocatedBudget { get; set; }
    public decimal UnallocatedBudget { get; set; }
    public int PlanningPackageCount { get; set; }
}