using Application.Common.Exceptions;
using Application.Interfaces.Cost;
using Carter;
using Core.DTOs.Reports;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules;

/// <summary>
/// Extension to EVMEndpoints for Nine Column Report
/// </summary>
public partial class EVMEndpoints : ICarterModule
{
    public void AddNineColumnReportRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/evm")
            .WithTags("Earned Value Management - Nine Column Report")
            .RequireAuthorization()
            .WithOpenApi();

        // Nine Column Report endpoints
        group.MapGet("project/{projectId:guid}/nine-column-report", GetNineColumnReport)
            .WithName("GetNineColumnReport")
            .WithSummary("Get Nine Column Report (Chilean standard)")
            .WithDescription("Obtiene el reporte de 9 columnas según estándar chileno de EVM")
            .Produces<NineColumnReportDto>()
            .Produces(404);

        group.MapPost("project/{projectId:guid}/nine-column-report/filtered", GetFilteredNineColumnReport)
            .WithName("GetFilteredNineColumnReport")
            .WithSummary("Get filtered Nine Column Report")
            .WithDescription("Obtiene el reporte de 9 columnas con filtros aplicados")
            .Produces<NineColumnReportDto>()
            .ProducesValidationProblem();

        group.MapGet("project/{projectId:guid}/nine-column-report/export/excel", ExportNineColumnReportToExcel)
            .WithName("ExportNineColumnReportToExcel")
            .WithSummary("Export Nine Column Report to Excel")
            .WithDescription("Exporta el reporte de 9 columnas a formato Excel")
            .Produces<FileResult>(200, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            .Produces(404);

        group.MapGet("project/{projectId:guid}/nine-column-report/export/pdf", ExportNineColumnReportToPdf)
            .WithName("ExportNineColumnReportToPdf")
            .WithSummary("Export Nine Column Report to PDF")
            .WithDescription("Exporta el reporte de 9 columnas a formato PDF")
            .Produces<FileResult>(200, "application/pdf")
            .Produces(404);

        group.MapGet("control-account/{controlAccountId:guid}/nine-column-report", GetNineColumnReportByControlAccount)
            .WithName("GetNineColumnReportByControlAccount")
            .WithSummary("Get Nine Column Report for specific Control Account")
            .WithDescription("Obtiene el reporte de 9 columnas para un Control Account específico")
            .Produces<NineColumnReportDto>()
            .Produces(404);

        group.MapGet("project/{projectId:guid}/nine-column-report/trend", GetNineColumnReportTrend)
            .WithName("GetNineColumnReportTrend")
            .WithSummary("Get Nine Column Report trend data")
            .WithDescription("Obtiene datos históricos del reporte para análisis de tendencias")
            .Produces<List<NineColumnReportDto>>()
            .Produces(404);

        group.MapGet("project/{projectId:guid}/nine-column-report/validate", ValidateNineColumnReport)
            .WithName("ValidateNineColumnReport")
            .WithSummary("Validate Nine Column Report data")
            .WithDescription("Valida la integridad de los datos del reporte")
            .Produces<NineColumnReportValidationResult>()
            .Produces(404);
    }

    // Handler methods

    private static async Task<Results<Ok<NineColumnReportDto>, NotFound>> GetNineColumnReport(
        [FromRoute] Guid projectId,
        [FromQuery] DateTime? asOfDate,
        [FromServices] IEVMService service,
        CancellationToken cancellationToken)
    {
        try
        {
            var report = await service.GetNineColumnReportAsync(projectId, asOfDate, cancellationToken);
            return TypedResults.Ok(report);
        }
        catch (NotFoundException)
        {
            return TypedResults.NotFound();
        }
    }

    private static async Task<Results<Ok<NineColumnReportDto>, ValidationProblem>> GetFilteredNineColumnReport(
        [FromRoute] Guid projectId,
        [FromBody] NineColumnReportFilterDto filter,
        [FromServices] IEVMService service,
        CancellationToken cancellationToken)
    {
        if (filter.ProjectId != projectId)
            filter.ProjectId = projectId;

        var report = await service.GetFilteredNineColumnReportAsync(filter, cancellationToken);
        return TypedResults.Ok(report);
    }

    private static async Task<Results<FileContentHttpResult, NotFound>> ExportNineColumnReportToExcel(
        [FromRoute] Guid projectId,
        [FromQuery] DateTime? asOfDate,
        [FromServices] IEVMService service,
        CancellationToken cancellationToken)
    {
        try
        {
            var fileContent = await service.ExportNineColumnReportToExcelAsync(projectId, asOfDate, cancellationToken);
            var fileName = $"NineColumnReport_{projectId}_{DateTime.Now:yyyyMMdd}.xlsx";

            return TypedResults.File(
                fileContent,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }
        catch (NotFoundException)
        {
            return TypedResults.NotFound();
        }
    }

    private static async Task<Results<FileContentHttpResult, NotFound>> ExportNineColumnReportToPdf(
        [FromRoute] Guid projectId,
        [FromQuery] DateTime? asOfDate,
        [FromServices] IEVMService service,
        CancellationToken cancellationToken)
    {
        try
        {
            var fileContent = await service.ExportNineColumnReportToPdfAsync(projectId, asOfDate, cancellationToken);
            var fileName = $"NineColumnReport_{projectId}_{DateTime.Now:yyyyMMdd}.pdf";

            return TypedResults.File(
                fileContent,
                "application/pdf",
                fileName);
        }
        catch (NotFoundException)
        {
            return TypedResults.NotFound();
        }
    }

    private static async Task<Results<Ok<NineColumnReportDto>, NotFound>> GetNineColumnReportByControlAccount(
        [FromRoute] Guid controlAccountId,
        [FromQuery] DateTime? asOfDate,
        [FromQuery] bool includeChildren = true,
        [FromServices] IEVMService service,
        CancellationToken cancellationToken)
    {
        try
        {
            var report = await service.GetNineColumnReportByControlAccountAsync(
                controlAccountId,
                asOfDate,
                includeChildren,
                cancellationToken);
            return TypedResults.Ok(report);
        }
        catch (NotFoundException)
        {
            return TypedResults.NotFound();
        }
    }

    private static async Task<Results<Ok<List<NineColumnReportDto>>, NotFound>> GetNineColumnReportTrend(
        [FromRoute] Guid projectId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] string periodType = "Monthly",
        [FromServices] IEVMService service,
        CancellationToken cancellationToken)
    {
        try
        {
            var reports = await service.GetNineColumnReportTrendAsync(
                projectId,
                startDate,
                endDate,
                periodType,
                cancellationToken);
            return TypedResults.Ok(reports);
        }
        catch (NotFoundException)
        {
            return TypedResults.NotFound();
        }
    }

    private static async Task<Results<Ok<NineColumnReportValidationResult>, NotFound>> ValidateNineColumnReport(
        [FromRoute] Guid projectId,
        [FromQuery] DateTime? asOfDate,
        [FromServices] IEVMService service,
        CancellationToken cancellationToken)
    {
        var result = await service.ValidateNineColumnReportDataAsync(projectId, asOfDate, cancellationToken);

        if (result.IsFailure)
            return TypedResults.NotFound();

        return TypedResults.Ok(result.Value);
    }
}   