using Carter;
using Carter.ModelBinding;
using Microsoft.AspNetCore.Http.HttpResults;
using Application.Interfaces.Cost;
using Core.DTOs.EVM;
using Core.DTOs.Common;
using Core.Enums.Cost;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;

namespace Api.Modules;

/// <summary>
/// Earned Value Management (EVM) endpoints
/// </summary>
public class EVMEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/evm")
            .WithTags("Earned Value Management")
            .RequireAuthorization()
            .WithOpenApi();

        // GET endpoints
        group.MapGet("records", GetEVMRecords)
            .WithName("GetEVMRecords")
            .WithSummary("Get EVM records with pagination")
            .Produces<PagedResult<EVMRecordDto>>();

        group.MapGet("records/{id:guid}", GetEVMRecordById)
            .WithName("GetEVMRecord")
            .WithSummary("Get EVM record by ID")
            .Produces<EVMRecordDetailDto>()
            .Produces(404);

        group.MapGet("project/{projectId:guid}/report", GetProjectEVMReport)
            .WithName("GetProjectEVMReport")
            .WithSummary("Get project EVM performance report")
            .Produces<EVMPerformanceReportDto>();

        group.MapGet("control-account/{controlAccountId:guid}/trends", GetEVMTrends)
            .WithName("GetEVMTrends")
            .WithSummary("Get EVM trends for control account")
            .Produces<List<EVMTrendDto>>();

        // POST endpoints
        group.MapPost("records", CreateEVMRecord)
            .WithName("CreateEVMRecord")
            .WithSummary("Create a new EVM record")
            .Produces<Guid>(201)
            .ProducesValidationProblem();

        group.MapPost("project/{projectId:guid}/calculate", CalculateProjectEVM)
            .WithName("CalculateProjectEVM")
            .WithSummary("Calculate EVM for entire project")
            .Produces(204)
            .ProducesValidationProblem();

        group.MapPost("project/{projectId:guid}/generate-monthly", GenerateMonthlyEVM)
            .WithName("GenerateMonthlyEVM")
            .WithSummary("Generate monthly EVM records")
            .Produces(204)
            .ProducesValidationProblem();

        // PUT endpoints
        group.MapPut("records/{id:guid}/actuals", UpdateEVMActuals)
            .WithName("UpdateEVMActuals")
            .WithSummary("Update EVM actual values")
            .Produces(204)
            .ProducesValidationProblem()
            .Produces(404);

        group.MapPut("records/{id:guid}/eac", UpdateEAC)
            .WithName("UpdateEAC")
            .WithSummary("Update Estimate at Completion")
            .Produces(204)
            .ProducesValidationProblem()
            .Produces(404);
    }

    // GET endpoints implementation
    private static async Task<Ok<PagedResult<EVMRecordDto>>> GetEVMRecords(
        [FromQuery] Guid controlAccountId,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] EVMPeriodType? periodType,
        IEVMService service,
        CancellationToken cancellationToken)
    {
        var parameters = new QueryParameters
        {
            PageNumber = pageNumber > 0 ? pageNumber : 1,
            PageSize = pageSize > 0 ? pageSize : 20
        };

        if (startDate.HasValue)
            parameters.Filters.Add("startDate", startDate.Value.ToString("yyyy-MM-dd"));

        if (endDate.HasValue)
            parameters.Filters.Add("endDate", endDate.Value.ToString("yyyy-MM-dd"));

        if (periodType.HasValue)
            parameters.Filters.Add("periodType", periodType.Value.ToString());

        var result = await service.GetEVMRecordsAsync(controlAccountId, parameters, cancellationToken);
        return TypedResults.Ok(result);
    }

    private static async Task<Results<Ok<EVMRecordDetailDto>, NotFound>> GetEVMRecordById(
        Guid id,
        IEVMService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetEVMRecordByIdAsync(id, cancellationToken);
        return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }

    private static async Task<Ok<EVMPerformanceReportDto>> GetProjectEVMReport(
        Guid projectId,
        [FromQuery] DateTime? asOfDate,
        IEVMService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetProjectEVMReportAsync(projectId, asOfDate, cancellationToken);
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<List<EVMTrendDto>>> GetEVMTrends(
        Guid controlAccountId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        IEVMService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetEVMTrendsAsync(controlAccountId, startDate, endDate, cancellationToken);
        return TypedResults.Ok(result);
    }

    // POST endpoints implementation
    private static async Task<Results<Created<Guid>, ValidationProblem>> CreateEVMRecord(
        CreateEVMRecordDto dto,
        IEVMService service,
        IValidator<CreateEVMRecordDto> validator,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var userId = httpContext.User.Identity?.Name ?? "system";
        var result = await service.CreateEVMRecordAsync(dto, userId, cancellationToken);

        if (result.IsFailure)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { "error", new[] { result.Error ?? "Failed to create EVM record" } }
            });
        }

        return TypedResults.Created($"/api/evm/records/{result.Value}", result.Value);
    }

    private static async Task<Results<NoContent, ValidationProblem>> CalculateProjectEVM(
        Guid projectId,
        [FromQuery] DateTime dataDate,
        IEVMService service,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = httpContext.User.Identity?.Name ?? "system";
        var result = await service.CalculateProjectEVMAsync(projectId, dataDate, userId, cancellationToken);

        if (result.IsFailure)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { "error", new[] { result.Error ?? "Failed to calculate project EVM" } }
            });
        }

        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, ValidationProblem>> GenerateMonthlyEVM(
        Guid projectId,
        [FromQuery] int year,
        [FromQuery] int month,
        IEVMService service,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (year < 2000 || year > 2100 || month < 1 || month > 12)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { "error", new[] { "Invalid year or month" } }
            });
        }

        var userId = httpContext.User.Identity?.Name ?? "system";
        var result = await service.GenerateMonthlyEVMAsync(projectId, year, month, userId, cancellationToken);

        if (result.IsFailure)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { "error", new[] { result.Error ?? "Failed to generate monthly EVM" } }
            });
        }

        return TypedResults.NoContent();
    }

    // PUT endpoints implementation
    private static async Task<Results<NoContent, NotFound, ValidationProblem>> UpdateEVMActuals(
        Guid id,
        UpdateEVMActualsDto dto,
        IEVMService service,
        IValidator<UpdateEVMActualsDto> validator,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var userId = httpContext.User.Identity?.Name ?? "system";
        var result = await service.UpdateEVMActualsAsync(id, dto, userId, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error?.Contains("not found", StringComparison.OrdinalIgnoreCase) ?? false)
                return TypedResults.NotFound();

            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { "error", new[] { result.Error ?? "Failed to update EVM actuals" } }
            });
        }

        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound, ValidationProblem>> UpdateEAC(
        Guid id,
        [FromBody] UpdateEACRequest request,
        IEVMService service,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (request.NewEAC <= 0)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { "newEAC", new[] { "EAC must be greater than zero" } }
            });
        }

        if (string.IsNullOrWhiteSpace(request.Justification))
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { "justification", new[] { "Justification is required" } }
            });
        }

        var userId = httpContext.User.Identity?.Name ?? "system";
        var result = await service.UpdateEACAsync(id, request.NewEAC, request.Justification, userId, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error?.Contains("not found", StringComparison.OrdinalIgnoreCase) ?? false)
                return TypedResults.NotFound();

            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { "error", new[] { result.Error ?? "Failed to update EAC" } }
            });
        }

        return TypedResults.NoContent();
    }

    // Request models
    private record UpdateEACRequest(decimal NewEAC, string Justification);
}