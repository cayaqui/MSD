using Application.Services.Interfaces;
using Carter;
using Carter.ModelBinding;
using Core.DTOs.Cost;
using Core.DTOs.Common;
using Core.Constants;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Core.DTOs.Cost.Commitments;
using ProblemDetails = Core.DTOs.Common.ProblemDetails;

namespace Api.Modules;

/// <summary>
/// Commitment management endpoints using Carter minimal APIs
/// </summary>
public class CommitmentEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/commitments")
            .WithTags("Commitments")
            .RequireAuthorization()
            .WithOpenApi();

        // Query endpoints
        group.MapPost("search", SearchCommitments)
            .WithName("SearchCommitments")
            .WithSummary("Search commitments with filters and pagination")
            .RequireAuthorization(SystemPermissions.Cost.ViewCommitment)
            .Produces<PagedResult<CommitmentListDto>>();

        group.MapGet("{id:guid}", GetCommitment)
            .WithName("GetCommitment")
            .WithSummary("Get commitment by ID")
            .RequireAuthorization(SystemPermissions.Cost.ViewCommitment)
            .Produces<CommitmentDto>()
            .Produces(404);

        group.MapGet("{id:guid}/detail", GetCommitmentDetail)
            .WithName("GetCommitmentDetail")
            .WithSummary("Get detailed commitment information")
            .RequireAuthorization(SystemPermissions.Cost.ViewCommitment)
            .Produces<CommitmentDetailDto>()
            .Produces(404);

        group.MapGet("project/{projectId:guid}", GetProjectCommitments)
            .WithName("GetProjectCommitments")
            .WithSummary("Get commitments for a project")
            .RequireAuthorization(SystemPermissions.Cost.ViewCommitment)
            .Produces<List<CommitmentListDto>>();

        group.MapGet("project/{projectId:guid}/summary", GetProjectCommitmentSummary)
            .WithName("GetProjectCommitmentSummary")
            .WithSummary("Get commitment summary for a project")
            .RequireAuthorization(SystemPermissions.Cost.ViewCommitment)
            .Produces<CommitmentSummaryDto>();

        // CRUD endpoints
        group.MapPost("", CreateCommitment)
            .WithName("CreateCommitment")
            .WithSummary("Create new commitment")
            .RequireAuthorization(SystemPermissions.Cost.CreateCommitment)
            .Produces<CommitmentDto>(201)
            .ProducesValidationProblem();

        group.MapPut("{id:guid}", UpdateCommitment)
            .WithName("UpdateCommitment")
            .WithSummary("Update commitment")
            .RequireAuthorization(SystemPermissions.Cost.EditCommitment)
            .Produces<CommitmentDto>()
            .ProducesValidationProblem()
            .Produces(404);

        group.MapDelete("{id:guid}", DeleteCommitment)
            .WithName("DeleteCommitment")
            .WithSummary("Delete commitment")
            .RequireAuthorization(SystemPermissions.Cost.DeleteCommitment)
            .Produces(204)
            .Produces(404)
            .Produces<ProblemDetails>(400);

        // Workflow endpoints
        group.MapPost("{id:guid}/submit-for-approval", SubmitForApproval)
            .WithName("SubmitCommitmentForApproval")
            .WithSummary("Submit commitment for approval")
            .RequireAuthorization(SystemPermissions.Cost.EditCommitment)
            .Produces<CommitmentDto>()
            .Produces(404)
            .Produces<ProblemDetails>(400);

        group.MapPost("{id:guid}/approve", ApproveCommitment)
            .WithName("ApproveCommitment")
            .WithSummary("Approve commitment")
            .RequireAuthorization(SystemPermissions.Cost.ApproveCommitment)
            .Produces<CommitmentDto>()
            .Produces(404)
            .Produces<ProblemDetails>(400);

        group.MapPost("{id:guid}/reject", RejectCommitment)
            .WithName("RejectCommitment")
            .WithSummary("Reject commitment")
            .RequireAuthorization(SystemPermissions.Cost.ApproveCommitment)
            .Produces<CommitmentDto>()
            .Produces(404)
            .Produces<ProblemDetails>(400);

        group.MapPost("{id:guid}/activate", ActivateCommitment)
            .WithName("ActivateCommitment")
            .WithSummary("Activate commitment")
            .RequireAuthorization(SystemPermissions.Cost.EditCommitment)
            .Produces<CommitmentDto>()
            .Produces(404)
            .Produces<ProblemDetails>(400);

        group.MapPost("{id:guid}/revise", ReviseCommitment)
            .WithName("ReviseCommitment")
            .WithSummary("Revise commitment amount")
            .RequireAuthorization(SystemPermissions.Cost.EditCommitment)
            .Produces<CommitmentDto>()
            .Produces(404)
            .Produces<ProblemDetails>(400);

        group.MapPost("{id:guid}/close", CloseCommitment)
            .WithName("CloseCommitment")
            .WithSummary("Close commitment")
            .RequireAuthorization(SystemPermissions.Cost.EditCommitment)
            .Produces<CommitmentDto>()
            .Produces(404)
            .Produces<Core.DTOs.Common.ProblemDetails>(400);

        group.MapPost("{id:guid}/cancel", CancelCommitment)
            .WithName("CancelCommitment")
            .WithSummary("Cancel commitment")
            .RequireAuthorization(SystemPermissions.Cost.EditCommitment)
            .Produces<CommitmentDto>()
            .Produces(404)
            .Produces<ProblemDetails>(400);

        // Work Package allocations
        group.MapGet("{id:guid}/work-packages", GetWorkPackageAllocations)
            .WithName("GetCommitmentWorkPackages")
            .WithSummary("Get work package allocations")
            .RequireAuthorization(SystemPermissions.Cost.ViewCommitment)
            .Produces<List<CommitmentWorkPackageDto>>()
            .Produces(404);

        group.MapPost("{id:guid}/work-packages", AddWorkPackageAllocation)
            .WithName("AddCommitmentWorkPackage")
            .WithSummary("Add work package allocation")
            .RequireAuthorization(SystemPermissions.Cost.EditCommitment)
            .Produces<CommitmentDto>()
            .Produces(404)
            .Produces<ProblemDetails>(400);

        // Commitment Items
        group.MapGet("{id:guid}/items", GetCommitmentItems)
            .WithName("GetCommitmentItems")
            .WithSummary("Get commitment items")
            .RequireAuthorization(SystemPermissions.Cost.ViewCommitment)
            .Produces<List<CommitmentItemDto>>()
            .Produces(404);

        group.MapPost("{id:guid}/items", AddCommitmentItem)
            .WithName("AddCommitmentItem")
            .WithSummary("Add commitment item")
            .RequireAuthorization(SystemPermissions.Cost.EditCommitment)
            .Produces<CommitmentDto>()
            .Produces(404)
            .Produces<ProblemDetails>(400);

        // Financial operations
        group.MapPost("{id:guid}/record-invoice", RecordInvoice)
            .WithName("RecordCommitmentInvoice")
            .WithSummary("Record invoice against commitment")
            .RequireAuthorization(SystemPermissions.Cost.CreateInvoice)
            .Produces<CommitmentDto>()
            .Produces(404)
            .Produces<ProblemDetails>(400);

        // Reporting
        group.MapGet("{id:guid}/revisions", GetCommitmentRevisions)
            .WithName("GetCommitmentRevisions")
            .WithSummary("Get commitment revisions")
            .RequireAuthorization(SystemPermissions.Cost.ViewCommitment)
            .Produces<List<CommitmentRevisionDto>>()
            .Produces(404);

        group.MapGet("{id:guid}/financial-summary", GetFinancialSummary)
            .WithName("GetCommitmentFinancialSummary")
            .WithSummary("Get financial summary")
            .RequireAuthorization(SystemPermissions.Cost.ViewCommitment)
            .Produces<CommitmentFinancialSummary>()
            .Produces(404);

        group.MapGet("{id:guid}/performance-metrics", GetPerformanceMetrics)
            .WithName("GetCommitmentPerformanceMetrics")
            .WithSummary("Get performance metrics")
            .RequireAuthorization(SystemPermissions.Cost.ViewCommitment)
            .Produces<CommitmentPerformanceMetrics>()
            .Produces(404);

        // Export
        group.MapPost("export", ExportCommitments)
            .WithName("ExportCommitments")
            .WithSummary("Export commitments to file")
            .RequireAuthorization(SystemPermissions.Cost.ViewCommitment)
            .Produces(200, contentType: "application/octet-stream")
            .Produces<ProblemDetails>(400);

        // Utilities
        group.MapGet("check-unique-number", CheckUniqueNumber)
            .WithName("CheckCommitmentNumberUnique")
            .WithSummary("Check if commitment number is unique")
            .RequireAuthorization(SystemPermissions.Cost.ViewCommitment)
            .Produces<bool>();
    }

    // Query endpoints implementation
    private static async Task<Results<Ok<PagedResult<CommitmentListDto>>, ValidationProblem>> SearchCommitments(
        [FromBody] CommitmentFilterDto filter,
        ICommitmentService commitmentService,
        IValidator<CommitmentFilterDto>? validator = null)
    {
        if (validator != null)
        {
            var validationResult = await validator.ValidateAsync(filter);
            if (!validationResult.IsValid)
                return TypedResults.ValidationProblem(validationResult.GetValidationProblems());
        }

        var result = await commitmentService.GetCommitmentsAsync(filter);
        return TypedResults.Ok(result);
    }

    private static async Task<Results<Ok<CommitmentDto>, NotFound>> GetCommitment(
        Guid id,
        ICommitmentService commitmentService)
    {
        var commitment = await commitmentService.GetCommitmentAsync(id);
        return commitment == null
            ? TypedResults.NotFound()
            : TypedResults.Ok(commitment);
    }

    private static async Task<Results<Ok<CommitmentDetailDto>, NotFound>> GetCommitmentDetail(
        Guid id,
        ICommitmentService commitmentService)
    {
        var commitment = await commitmentService.GetCommitmentDetailAsync(id);
        return commitment == null
            ? TypedResults.NotFound()
            : TypedResults.Ok(commitment);
    }

    private static async Task<Ok<List<CommitmentListDto>>> GetProjectCommitments(
        Guid projectId,
        ICommitmentService commitmentService)
    {
        var commitments = await commitmentService.GetProjectCommitmentsAsync(projectId);
        return TypedResults.Ok(commitments);
    }

    private static async Task<Ok<CommitmentSummaryDto>> GetProjectCommitmentSummary(
        Guid projectId,
        ICommitmentService commitmentService)
    {
        var summary = await commitmentService.GetProjectCommitmentSummaryAsync(projectId);
        return TypedResults.Ok(summary);
    }

    // CRUD endpoints implementation
    private static async Task<Results<Created<CommitmentDto>, ValidationProblem, ProblemHttpResult>> CreateCommitment(
        [FromBody] CreateCommitmentDto dto,
        ICommitmentService commitmentService,
        IValidator<CreateCommitmentDto> validator,
        HttpContext context)
    {
        var validationResult = await validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            return TypedResults.ValidationProblem(validationResult.GetValidationProblems());

        try
        {
            var commitment = await commitmentService.CreateCommitmentAsync(dto);
            var location = $"{context.Request.Path}/{commitment.Id}";
            return TypedResults.Created(location, commitment);
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.Problem(ex.Message, statusCode: 400);
        }
    }

    private static async Task<Results<Ok<CommitmentDto>, NotFound, ValidationProblem, ProblemHttpResult>> UpdateCommitment(
        Guid id,
        [FromBody] UpdateCommitmentDto dto,
        ICommitmentService commitmentService,
        IValidator<UpdateCommitmentDto>? validator = null)
    {
        if (validator != null)
        {
            var validationResult = await validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                return TypedResults.ValidationProblem(validationResult.GetValidationProblems());
        }

        try
        {
            var commitment = await commitmentService.UpdateCommitmentAsync(id, dto);
            return TypedResults.Ok(commitment);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
        {
            return TypedResults.NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.Problem(ex.Message, statusCode: 400);
        }
    }

    private static async Task<Results<NoContent, NotFound, ProblemHttpResult>> DeleteCommitment(
        Guid id,
        ICommitmentService commitmentService)
    {
        try
        {
            var canDelete = await commitmentService.CanDeleteCommitmentAsync(id);
            if (!canDelete)
                return TypedResults.Problem("Cannot delete commitment with invoices or in active status", statusCode: 400);

            var result = await commitmentService.DeleteCommitmentAsync(id);
            return result ? TypedResults.NoContent() : TypedResults.NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.Problem(ex.Message, statusCode: 400);
        }
    }

    // Workflow endpoints implementation
    private static async Task<Results<Ok<CommitmentDto>, NotFound, ProblemHttpResult>> SubmitForApproval(
        Guid id,
        ICommitmentService commitmentService)
    {
        try
        {
            var commitment = await commitmentService.SubmitForApprovalAsync(id);
            return TypedResults.Ok(commitment);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
        {
            return TypedResults.NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.Problem(ex.Message, statusCode: 400);
        }
    }

    private static async Task<Results<Ok<CommitmentDto>, NotFound, ProblemHttpResult>> ApproveCommitment(
        Guid id,
        [FromBody] ApproveCommitmentDto dto,
        ICommitmentService commitmentService)
    {
        try
        {
            var commitment = await commitmentService.ApproveCommitmentAsync(id, dto);
            return TypedResults.Ok(commitment);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
        {
            return TypedResults.NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.Problem(ex.Message, statusCode: 400);
        }
    }

    private static async Task<Results<Ok<CommitmentDto>, NotFound, ProblemHttpResult>> RejectCommitment(
        Guid id,
        [FromBody] CancelCommitmentDto dto,
        ICommitmentService commitmentService)
    {
        try
        {
            var commitment = await commitmentService.RejectCommitmentAsync(id, dto);
            return TypedResults.Ok(commitment);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
        {
            return TypedResults.NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.Problem(ex.Message, statusCode: 400);
        }
    }

    private static async Task<Results<Ok<CommitmentDto>, NotFound, ProblemHttpResult>> ActivateCommitment(
        Guid id,
        ICommitmentService commitmentService)
    {
        try
        {
            var commitment = await commitmentService.ActivateCommitmentAsync(id);
            return TypedResults.Ok(commitment);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
        {
            return TypedResults.NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.Problem(ex.Message, statusCode: 400);
        }
    }

    private static async Task<Results<Ok<CommitmentDto>, NotFound, ValidationProblem, ProblemHttpResult>> ReviseCommitment(
        Guid id,
        [FromBody] ReviseCommitmentDto dto,
        ICommitmentService commitmentService,
        IValidator<ReviseCommitmentDto> validator)
    {
        var validationResult = await validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            return TypedResults.ValidationProblem(validationResult.GetValidationProblems());

        try
        {
            var canRevise = await commitmentService.CanReviseCommitmentAsync(id, dto.RevisedAmount);
            if (!canRevise)
                return TypedResults.Problem("Cannot revise commitment to amount less than invoiced amount", statusCode: 400);

            var commitment = await commitmentService.ReviseCommitmentAsync(id, dto);
            return TypedResults.Ok(commitment);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
        {
            return TypedResults.NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.Problem(ex.Message, statusCode: 400);
        }
    }

    private static async Task<Results<Ok<CommitmentDto>, NotFound, ProblemHttpResult>> CloseCommitment(
        Guid id,
        ICommitmentService commitmentService)
    {
        try
        {
            var commitment = await commitmentService.CloseCommitmentAsync(id);
            return TypedResults.Ok(commitment);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
        {
            return TypedResults.NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.Problem(ex.Message, statusCode: 400);
        }
    }

    private static async Task<Results<Ok<CommitmentDto>, NotFound, ProblemHttpResult>> CancelCommitment(
        Guid id,
        [FromBody] CancelCommitmentDto dto,
        ICommitmentService commitmentService)
    {
        try
        {
            var commitment = await commitmentService.CancelCommitmentAsync(id, dto);
            return TypedResults.Ok(commitment);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
        {
            return TypedResults.NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.Problem(ex.Message, statusCode: 400);
        }
    }

    // Additional endpoints implementation
    private static async Task<Results<Ok<List<CommitmentWorkPackageDto>>, NotFound>> GetWorkPackageAllocations(
        Guid id,
        ICommitmentService commitmentService)
    {
        var allocations = await commitmentService.GetWorkPackageAllocationsAsync(id);
        return TypedResults.Ok(allocations);
    }

    private static async Task<Results<Ok<CommitmentDto>, NotFound, ValidationProblem, ProblemHttpResult>> AddWorkPackageAllocation(
        Guid id,
        [FromBody] CommitmentWorkPackageAllocationDto dto,
        ICommitmentService commitmentService,
        IValidator<CommitmentWorkPackageAllocationDto>? validator = null)
    {
        if (validator != null)
        {
            var validationResult = await validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                return TypedResults.ValidationProblem(validationResult.GetValidationProblems());
        }

        try
        {
            var commitment = await commitmentService.AddWorkPackageAllocationAsync(id, dto);
            return TypedResults.Ok(commitment);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
        {
            return TypedResults.NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.Problem(ex.Message, statusCode: 400);
        }
    }

    private static async Task<Results<Ok<List<CommitmentItemDto>>, NotFound>> GetCommitmentItems(
        Guid id,
        ICommitmentService commitmentService)
    {
        var items = await commitmentService.GetCommitmentItemsAsync(id);
        return TypedResults.Ok(items);
    }

    private static async Task<Results<Ok<CommitmentDto>, NotFound, ValidationProblem, ProblemHttpResult>> AddCommitmentItem(
        Guid id,
        [FromBody] CreateCommitmentItemDto dto,
        ICommitmentService commitmentService,
        IValidator<CreateCommitmentItemDto>? validator = null)
    {
        if (validator != null)
        {
            var validationResult = await validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                return TypedResults.ValidationProblem(validationResult.GetValidationProblems());
        }

        try
        {
            var commitment = await commitmentService.AddCommitmentItemAsync(id, dto);
            return TypedResults.Ok(commitment);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
        {
            return TypedResults.NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.Problem(ex.Message, statusCode: 400);
        }
    }

    private static async Task<Results<Ok<CommitmentDto>, NotFound, ValidationProblem, ProblemHttpResult>> RecordInvoice(
        Guid id,
        [FromBody] RecordCommitmentInvoiceDto dto,
        ICommitmentService commitmentService,
        IValidator<RecordCommitmentInvoiceDto>? validator = null)
    {
        if (validator != null)
        {
            var validationResult = await validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                return TypedResults.ValidationProblem(validationResult.GetValidationProblems());
        }

        try
        {
            var commitment = await commitmentService.RecordInvoiceAsync(id, dto);
            return TypedResults.Ok(commitment);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
        {
            return TypedResults.NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.Problem(ex.Message, statusCode: 400);
        }
    }

    private static async Task<Results<Ok<List<CommitmentRevisionDto>>, NotFound>> GetCommitmentRevisions(
        Guid id,
        ICommitmentService commitmentService)
    {
        var revisions = await commitmentService.GetCommitmentRevisionsAsync(id);
        return TypedResults.Ok(revisions);
    }

    private static async Task<Results<Ok<CommitmentFinancialSummary>, NotFound>> GetFinancialSummary(
        Guid id,
        ICommitmentService commitmentService)
    {
        var summary = await commitmentService.GetFinancialSummaryAsync(id);
        return TypedResults.Ok(summary);
    }

    private static async Task<Results<Ok<CommitmentPerformanceMetrics>, NotFound>> GetPerformanceMetrics(
        Guid id,
        ICommitmentService commitmentService)
    {
        var metrics = await commitmentService.GetPerformanceMetricsAsync(id);
        return TypedResults.Ok(metrics);
    }

    private static async Task<Results<FileContentHttpResult, ProblemHttpResult>> ExportCommitments(
        [FromBody] CommitmentFilterDto filter,
        [FromQuery] string format,
        ICommitmentService commitmentService)
    {
        var validFormats = new[] { "xlsx", "csv", "pdf" };
        if (!validFormats.Contains(format?.ToLower() ?? "xlsx"))
            return TypedResults.Problem($"Invalid format. Supported formats: {string.Join(", ", validFormats)}", statusCode: 400);

        try
        {
            var fileContent = await commitmentService.ExportCommitmentsAsync(filter, format);

            var contentType = format.ToLower() switch
            {
                "xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "csv" => "text/csv",
                "pdf" => "application/pdf",
                _ => "application/octet-stream"
            };

            var fileName = $"commitments_{DateTime.UtcNow:yyyyMMddHHmmss}.{format}";

            return TypedResults.File(fileContent, contentType, fileName);
        }
        catch (Exception ex)
        {
            return TypedResults.Problem($"Error exporting commitments: {ex.Message}", statusCode: 500);
        }
    }

    private static async Task<Ok<bool>> CheckUniqueNumber(
        [FromQuery] string commitmentNumber,
        [FromQuery] Guid? excludeId,
        ICommitmentService commitmentService)
    {
        var isUnique = await commitmentService.IsCommitmentNumberUniqueAsync(commitmentNumber, excludeId);
        return TypedResults.Ok(isUnique);
    }
}

// Extension methods for FluentValidation integration
public static class ValidationExtensions
{
    public static IDictionary<string, string[]> GetValidationProblems(this FluentValidation.Results.ValidationResult validationResult)
    {
        return validationResult.Errors
            .GroupBy(x => x.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.ErrorMessage).ToArray()
            );
    }
}