using Application.Interfaces.Cost;
using Carter;
using Core.DTOs.Common;
using Core.DTOs.EVM;
using Core.DTOs.Cost;
using Core.DTOs.Reports;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Cost;

/// <summary>
/// Earned Value Management (EVM) endpoints
/// </summary>
public class EVMModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/evm")
            .WithTags("EVM")
            .RequireAuthorization();

        // Query endpoints
        group.MapGet("/control-account/{controlAccountId:guid}/records", GetEVMRecords)
            .WithName("GetEVMRecords")
            .WithSummary("Get EVM records for control account")
            .Produces<PagedResult<EVMRecordDto>>();

        group.MapGet("/records/{id:guid}", GetEVMRecordById)
            .WithName("GetEVMRecordById")
            .WithSummary("Get EVM record by ID")
            .Produces<EVMRecordDetailDto>()
            .ProducesProblem(404);

        group.MapGet("/project/{projectId:guid}/report", GetProjectEVMReport)
            .WithName("GetProjectEVMReport")
            .WithSummary("Get project EVM report")
            .Produces<EVMPerformanceReportDto>();

        group.MapGet("/control-account/{controlAccountId:guid}/trends", GetEVMTrends)
            .WithName("GetEVMTrends")
            .WithSummary("Get EVM trends for control account")
            .Produces<List<EVMTrendDto>>();

        // Command endpoints
        group.MapPost("/records", CreateEVMRecord)
            .WithName("CreateEVMRecord")
            .WithSummary("Create EVM record")
            .Produces<Guid>(201)
            .ProducesValidationProblem();

        group.MapPut("/records/{id:guid}/actuals", UpdateEVMActuals)
            .WithName("UpdateEVMActuals")
            .WithSummary("Update EVM actuals")
            .Produces(200)
            .ProducesValidationProblem()
            .ProducesProblem(404);

        group.MapPost("/project/{projectId:guid}/calculate", CalculateProjectEVM)
            .WithName("CalculateProjectEVM")
            .WithSummary("Calculate project EVM")
            .Produces(200)
            .ProducesProblem(404);

        group.MapPost("/project/{projectId:guid}/generate-monthly", GenerateMonthlyEVM)
            .WithName("GenerateMonthlyEVM")
            .WithSummary("Generate monthly EVM")
            .Produces(200)
            .ProducesProblem(404);

        // Forecast endpoints
        group.MapPut("/records/{id:guid}/eac", UpdateEAC)
            .WithName("UpdateEAC")
            .WithSummary("Update Estimate at Completion")
            .Produces(200)
            .ProducesValidationProblem()
            .ProducesProblem(404);

        // Nine Column Report endpoints
        group.MapGet("/project/{projectId:guid}/nine-column-report", GetNineColumnReport)
            .WithName("GetNineColumnReport")
            .WithSummary("Get Nine Column Report for project")
            .Produces<NineColumnReportDto>();

        group.MapPost("/nine-column-report/filtered", GetFilteredNineColumnReport)
            .WithName("GetFilteredNineColumnReport")
            .WithSummary("Get filtered Nine Column Report")
            .Produces<NineColumnReportDto>();

        group.MapGet("/project/{projectId:guid}/nine-column-report/excel", ExportNineColumnReportToExcel)
            .WithName("ExportNineColumnReportToExcel")
            .WithSummary("Export Nine Column Report to Excel")
            .Produces(200);

        group.MapGet("/project/{projectId:guid}/nine-column-report/pdf", ExportNineColumnReportToPdf)
            .WithName("ExportNineColumnReportToPdf")
            .WithSummary("Export Nine Column Report to PDF")
            .Produces(200);

        group.MapGet("/control-account/{controlAccountId:guid}/nine-column-report", GetNineColumnReportByControlAccount)
            .WithName("GetNineColumnReportByControlAccount")
            .WithSummary("Get Nine Column Report by Control Account")
            .Produces<NineColumnReportDto>();

        group.MapGet("/project/{projectId:guid}/nine-column-report/trend", GetNineColumnReportTrend)
            .WithName("GetNineColumnReportTrend")
            .WithSummary("Get Nine Column Report trend data")
            .Produces<List<NineColumnReportDto>>();

        group.MapGet("/project/{projectId:guid}/nine-column-report/validate", ValidateNineColumnReportData)
            .WithName("ValidateNineColumnReportData")
            .WithSummary("Validate Nine Column Report data")
            .Produces<NineColumnReportValidationResult>();
    }

    // Query handlers
    private static async Task<IResult> GetEVMRecords(
        [FromRoute] Guid controlAccountId,
        [AsParameters] EVMQueryParameters parameters,
        IEVMService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetEVMRecordsAsync(controlAccountId, parameters, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetEVMRecordById(
        [FromRoute] Guid id,
        IEVMService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetEVMRecordByIdAsync(id, cancellationToken);
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> GetProjectEVMReport(
        [FromRoute] Guid projectId,
        [FromQuery] DateTime? asOfDate,
        IEVMService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetProjectEVMReportAsync(projectId, asOfDate, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetEVMTrends(
        [FromRoute] Guid controlAccountId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        IEVMService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetEVMTrendsAsync(controlAccountId, startDate, endDate, cancellationToken);
        return Results.Ok(result);
    }

    // Command handlers
    private static async Task<IResult> CreateEVMRecord(
        [FromBody] CreateEVMRecordDto dto,
        IEVMService service,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
        var result = await service.CreateEVMRecordAsync(dto, userId, cancellationToken);
        return result.IsSuccess 
            ? Results.Created($"/api/evm/records/{result.Value}", result.Value)
            : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> UpdateEVMActuals(
        [FromRoute] Guid id,
        [FromBody] UpdateEVMActualsDto dto,
        IEVMService service,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
        var result = await service.UpdateEVMActualsAsync(id, dto, userId, cancellationToken);
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> CalculateProjectEVM(
        [FromRoute] Guid projectId,
        [FromBody] CalculateEVMDto dto,
        IEVMService service,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
        var result = await service.CalculateProjectEVMAsync(projectId, dto.DataDate, userId, cancellationToken);
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> GenerateMonthlyEVM(
        [FromRoute] Guid projectId,
        [FromBody] GenerateMonthlyEVMDto dto,
        IEVMService service,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
        var result = await service.GenerateMonthlyEVMAsync(projectId, dto.Year, dto.Month, userId, cancellationToken);
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    // Forecast handlers
    private static async Task<IResult> UpdateEAC(
        [FromRoute] Guid id,
        [FromBody] UpdateEACDto dto,
        IEVMService service,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
        var result = await service.UpdateEACAsync(id, dto.NewEAC, dto.Justification, userId, cancellationToken);
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    // Nine Column Report handlers
    private static async Task<IResult> GetNineColumnReport(
        [FromRoute] Guid projectId,
        [FromQuery] DateTime? asOfDate,
        IEVMService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetNineColumnReportAsync(projectId, asOfDate, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetFilteredNineColumnReport(
        [FromBody] NineColumnReportFilterDto filter,
        IEVMService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetFilteredNineColumnReportAsync(filter, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> ExportNineColumnReportToExcel(
        [FromRoute] Guid projectId,
        [FromQuery] DateTime? asOfDate,
        IEVMService service,
        CancellationToken cancellationToken)
    {
        var result = await service.ExportNineColumnReportToExcelAsync(projectId, asOfDate, cancellationToken);
        return Results.File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "nine-column-report.xlsx");
    }

    private static async Task<IResult> ExportNineColumnReportToPdf(
        [FromRoute] Guid projectId,
        [FromQuery] DateTime? asOfDate,
        IEVMService service,
        CancellationToken cancellationToken)
    {
        var result = await service.ExportNineColumnReportToPdfAsync(projectId, asOfDate, cancellationToken);
        return Results.File(result, "application/pdf", "nine-column-report.pdf");
    }

    private static async Task<IResult> GetNineColumnReportByControlAccount(
        [FromRoute] Guid controlAccountId,
        [FromQuery] DateTime? asOfDate,
        [FromQuery] bool includeChildren,
        IEVMService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetNineColumnReportByControlAccountAsync(controlAccountId, asOfDate, includeChildren, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetNineColumnReportTrend(
        [FromRoute] Guid projectId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] string periodType,
        IEVMService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetNineColumnReportTrendAsync(projectId, startDate, endDate, periodType, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> ValidateNineColumnReportData(
        [FromRoute] Guid projectId,
        [FromQuery] DateTime? asOfDate,
        IEVMService service,
        CancellationToken cancellationToken)
    {
        var result = await service.ValidateNineColumnReportDataAsync(projectId, asOfDate, cancellationToken);
        return result.IsSuccess 
            ? Results.Ok(result.Value) 
            : Results.BadRequest(result.Error);
    }
}

// Request DTOs
public record CalculateEVMDto(DateTime DataDate);
public record GenerateMonthlyEVMDto(int Year, int Month);
public record UpdateEACDto(decimal NewEAC, string Justification);