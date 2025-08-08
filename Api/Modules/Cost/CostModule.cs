using Application.Interfaces.Cost;
using Carter;
using Core.DTOs.Common;
using Core.DTOs.Cost;
using Core.DTOs.Cost.CostControlReports;
using Core.DTOs.Cost.CostItems;
using Core.DTOs.Cost.PlanningPackages;
using Core.DTOs.Cost.ControlAccounts;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Cost;

/// <summary>
/// Endpoints de gestión de costos
/// </summary>
public class CostModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/costs")
            .WithTags("Costos")
            .RequireAuthorization();

        // Cost Items endpoints
        group.MapGet("/items/project/{projectId:guid}", GetCostItems)
            .WithName("GetCostItems")
            .WithSummary("Obtener elementos de costo para un proyecto")
            .Produces<PagedResult<CostItemDto>>();

        group.MapGet("/items/{id:guid}", GetCostItemById)
            .WithName("GetCostItemById")
            .WithSummary("Obtener elemento de costo por ID")
            .Produces<CostItemDetailDto>()
            .ProducesProblem(404);

        group.MapPost("/items", CreateCostItem)
            .WithName("CreateCostItem")
            .WithSummary("Crear un nuevo elemento de costo")
            .Produces<Guid>(201)
            .ProducesValidationProblem();

        group.MapPut("/items/{id:guid}", UpdateCostItem)
            .WithName("UpdateCostItem")
            .WithSummary("Actualizar un elemento de costo")
            .Produces(200)
            .ProducesValidationProblem()
            .ProducesProblem(404);

        group.MapDelete("/items/{id:guid}", DeleteCostItem)
            .WithName("DeleteCostItem")
            .WithSummary("Eliminar un elemento de costo")
            .Produces(204)
            .ProducesProblem(404);

        // Cost recording endpoints
        group.MapPost("/items/{id:guid}/actual", RecordActualCost)
            .WithName("RecordActualCost")
            .WithSummary("Registrar costo real")
            .Produces(200)
            .ProducesValidationProblem()
            .ProducesProblem(404);

        group.MapPost("/items/{id:guid}/commitment", RecordCommitment)
            .WithName("RecordCommitment")
            .WithSummary("Registrar compromiso")
            .Produces(200)
            .ProducesValidationProblem()
            .ProducesProblem(404);

        group.MapPost("/items/{id:guid}/approve", ApproveCostItem)
            .WithName("ApproveCostItem")
            .WithSummary("Aprobar un elemento de costo")
            .Produces(200)
            .ProducesProblem(404);

        // Reporting endpoints
        group.MapGet("/reports/project/{projectId:guid}", GetProjectCostReport)
            .WithName("GetProjectCostReport")
            .WithSummary("Obtener reporte de costos del proyecto")
            .Produces<ProjectCostReportDto>();

        group.MapGet("/reports/project/{projectId:guid}/by-category", GetCostSummaryByCategory)
            .WithName("GetCostSummaryByCategory")
            .WithSummary("Obtener resumen de costos por categoría")
            .Produces<List<CostSummaryByCategoryDto>>();

        group.MapGet("/reports/project/{projectId:guid}/by-control-account", GetCostSummaryByControlAccount)
            .WithName("GetCostSummaryByControlAccount")
            .WithSummary("Obtener resumen de costos por cuenta de control")
            .Produces<List<CostSummaryByControlAccountDto>>();

        // Planning Package endpoints
        group.MapGet("/planning-packages/{controlAccountId:guid}", GetPlanningPackages)
            .WithName("GetCostPlanningPackages")
            .WithSummary("Obtener paquetes de planificación para cuenta de control")
            .Produces<PagedResult<PlanningPackageDto>>();

        group.MapPost("/planning-packages", CreatePlanningPackage)
            .WithName("CreateCostPlanningPackage")
            .WithSummary("Crear un paquete de planificación")
            .Produces<Guid>(201)
            .ProducesValidationProblem();

        group.MapPut("/planning-packages/{id:guid}", UpdatePlanningPackage)
            .WithName("UpdateCostPlanningPackage")
            .WithSummary("Actualizar un paquete de planificación")
            .Produces(200)
            .ProducesValidationProblem()
            .ProducesProblem(404);

        group.MapPost("/planning-packages/{id:guid}/convert", ConvertPlanningPackageToWorkPackages)
            .WithName("ConvertPlanningPackageToWorkPackages")
            .WithSummary("Convertir paquete de planificación a paquetes de trabajo")
            .Produces(200)
            .ProducesValidationProblem()
            .ProducesProblem(404);
    }

    // Cost Items handlers
    private static async Task<IResult> GetCostItems(
        [FromRoute] Guid projectId,
        [AsParameters] CostQueryParameters parameters,
        ICostService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetCostItemsAsync(projectId, parameters, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetCostItemById(
        [FromRoute] Guid id,
        ICostService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetCostItemByIdAsync(id, cancellationToken);
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> CreateCostItem(
        [FromBody] CreateCostItemDto dto,
        ICostService service,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
        var result = await service.CreateCostItemAsync(dto, userId, cancellationToken);
        return result.IsSuccess 
            ? Results.Created($"/api/costs/items/{result.Value}", result.Value)
            : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> UpdateCostItem(
        [FromRoute] Guid id,
        [FromBody] UpdateCostItemDto dto,
        ICostService service,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
        var result = await service.UpdateCostItemAsync(id, dto, userId, cancellationToken);
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> DeleteCostItem(
        [FromRoute] Guid id,
        ICostService service,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
        var result = await service.DeleteCostItemAsync(id, userId, cancellationToken);
        return result.IsSuccess ? Results.NoContent() : Results.BadRequest(result.Error);
    }

    // Cost recording handlers
    private static async Task<IResult> RecordActualCost(
        [FromRoute] Guid id,
        [FromBody] RecordActualCostDto dto,
        ICostService service,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
        var result = await service.RecordActualCostAsync(id, dto, userId, cancellationToken);
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> RecordCommitment(
        [FromRoute] Guid id,
        [FromBody] RecordCommitmentDto dto,
        ICostService service,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
        var result = await service.RecordCommitmentAsync(id, dto, userId, cancellationToken);
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> ApproveCostItem(
        [FromRoute] Guid id,
        ICostService service,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
        var result = await service.ApproveCostItemAsync(id, userId, cancellationToken);
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    // Reporting handlers
    private static async Task<IResult> GetProjectCostReport(
        [FromRoute] Guid projectId,
        [FromQuery] DateTime? asOfDate,
        ICostService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetProjectCostReportAsync(projectId, asOfDate, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetCostSummaryByCategory(
        [FromRoute] Guid projectId,
        ICostService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetCostSummaryByCategoryAsync(projectId, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetCostSummaryByControlAccount(
        [FromRoute] Guid projectId,
        ICostService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetCostSummaryByControlAccountAsync(projectId, cancellationToken);
        return Results.Ok(result);
    }

    // Planning Package handlers
    private static async Task<IResult> GetPlanningPackages(
        [FromRoute] Guid controlAccountId,
        [AsParameters] PlanningPackageQueryParameters parameters,
        ICostService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetPlanningPackagesAsync(controlAccountId, parameters, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> CreatePlanningPackage(
        [FromBody] CreatePlanningPackageDto dto,
        ICostService service,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
        var result = await service.CreatePlanningPackageAsync(dto, userId, cancellationToken);
        return result.IsSuccess 
            ? Results.Created($"/api/costs/planning-packages/{result.Value}", result.Value)
            : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> UpdatePlanningPackage(
        [FromRoute] Guid id,
        [FromBody] UpdatePlanningPackageDto dto,
        ICostService service,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
        var result = await service.UpdatePlanningPackageAsync(id, dto, userId, cancellationToken);
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> ConvertPlanningPackageToWorkPackages(
        [FromRoute] Guid id,
        [FromBody] ConvertPlanningPackageDto dto,
        ICostService service,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
        var result = await service.ConvertPlanningPackageToWorkPackagesAsync(id, dto, userId, cancellationToken);
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }
}