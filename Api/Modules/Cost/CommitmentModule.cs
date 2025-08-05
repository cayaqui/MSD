using Application.Interfaces.Cost;
using Carter;
using Core.DTOs.Common;
using Core.DTOs.Cost.CommitmentItems;
using Core.DTOs.Cost.Commitments;
using Core.DTOs.Cost.CommitmentWorkPackage;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Cost;

/// <summary>
/// Commitment management endpoints
/// </summary>
public class CommitmentModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/commitments")
            .WithTags("Commitments")
            .RequireAuthorization();

        // Query endpoints
        group.MapPost("/search", SearchCommitments)
            .WithName("SearchCommitments")
            .WithSummary("Search commitments with filters")
            .Produces<PagedResult<CommitmentListDto>>();

        group.MapGet("/{id:guid}", GetCommitmentById)
            .WithName("GetCommitmentById")
            .WithSummary("Get commitment by ID")
            .Produces<CommitmentDto>()
            .ProducesProblem(404);

        group.MapGet("/{id:guid}/detail", GetCommitmentDetail)
            .WithName("GetCommitmentDetail")
            .WithSummary("Get commitment detailed information")
            .Produces<CommitmentDetailDto>()
            .ProducesProblem(404);

        group.MapGet("/project/{projectId:guid}", GetProjectCommitments)
            .WithName("GetProjectCommitments")
            .WithSummary("Get commitments for a project")
            .Produces<List<CommitmentListDto>>();

        group.MapGet("/project/{projectId:guid}/summary", GetProjectCommitmentSummary)
            .WithName("GetProjectCommitmentSummary")
            .WithSummary("Get commitment summary for a project")
            .Produces<CommitmentSummaryDto>();

        // Command endpoints
        group.MapPost("/", CreateCommitment)
            .WithName("CreateCommitment")
            .WithSummary("Create a new commitment")
            .Produces<CommitmentDto>(201)
            .ProducesValidationProblem();

        group.MapPut("/{id:guid}", UpdateCommitment)
            .WithName("UpdateCommitment")
            .WithSummary("Update a commitment")
            .Produces<CommitmentDto>()
            .ProducesValidationProblem()
            .ProducesProblem(404);

        group.MapDelete("/{id:guid}", DeleteCommitment)
            .WithName("DeleteCommitment")
            .WithSummary("Delete a commitment")
            .Produces(204)
            .ProducesProblem(404);

        // Workflow endpoints
        group.MapPost("/{id:guid}/submit", SubmitForApproval)
            .WithName("SubmitCommitmentForApproval")
            .WithSummary("Submit commitment for approval")
            .Produces<CommitmentDto>()
            .ProducesProblem(404);

        group.MapPost("/{id:guid}/approve", ApproveCommitment)
            .WithName("ApproveCommitment")
            .WithSummary("Approve a commitment")
            .Produces<CommitmentDto>()
            .ProducesValidationProblem()
            .ProducesProblem(404);

        group.MapPost("/{id:guid}/reject", RejectCommitment)
            .WithName("RejectCommitment")
            .WithSummary("Reject a commitment")
            .Produces<CommitmentDto>()
            .ProducesValidationProblem()
            .ProducesProblem(404);

        group.MapPost("/{id:guid}/activate", ActivateCommitment)
            .WithName("ActivateCommitment")
            .WithSummary("Activate a commitment")
            .Produces<CommitmentDto>()
            .ProducesProblem(404);

        group.MapPost("/{id:guid}/revise", ReviseCommitment)
            .WithName("ReviseCommitment")
            .WithSummary("Revise a commitment")
            .Produces<CommitmentDto>()
            .ProducesValidationProblem()
            .ProducesProblem(404);

        group.MapPost("/{id:guid}/close", CloseCommitment)
            .WithName("CloseCommitment")
            .WithSummary("Close a commitment")
            .Produces<CommitmentDto>()
            .ProducesProblem(404);

        group.MapPost("/{id:guid}/cancel", CancelCommitment)
            .WithName("CancelCommitment")
            .WithSummary("Cancel a commitment")
            .Produces<CommitmentDto>()
            .ProducesValidationProblem()
            .ProducesProblem(404);

        // Work Package allocation endpoints
        group.MapPost("/{id:guid}/allocations", AddWorkPackageAllocation)
            .WithName("AddWorkPackageAllocation")
            .WithSummary("Add work package allocation")
            .Produces<CommitmentDto>()
            .ProducesValidationProblem()
            .ProducesProblem(404);

        group.MapPut("/{id:guid}/allocations/{allocationId:guid}", UpdateWorkPackageAllocation)
            .WithName("UpdateWorkPackageAllocation")
            .WithSummary("Update work package allocation")
            .Produces<CommitmentDto>()
            .ProducesProblem(404);

        group.MapDelete("/{id:guid}/allocations/{allocationId:guid}", RemoveWorkPackageAllocation)
            .WithName("RemoveWorkPackageAllocation")
            .WithSummary("Remove work package allocation")
            .Produces(204)
            .ProducesProblem(404);

        group.MapGet("/{id:guid}/allocations", GetWorkPackageAllocations)
            .WithName("GetWorkPackageAllocations")
            .WithSummary("Get work package allocations")
            .Produces<List<CommitmentWorkPackageDto>>();

        // Commitment Item endpoints
        group.MapPost("/{id:guid}/items", AddCommitmentItem)
            .WithName("AddCommitmentItem")
            .WithSummary("Add commitment item")
            .Produces<CommitmentDto>()
            .ProducesValidationProblem()
            .ProducesProblem(404);

        group.MapPut("/{id:guid}/items/{itemId:guid}", UpdateCommitmentItem)
            .WithName("UpdateCommitmentItem")
            .WithSummary("Update commitment item")
            .Produces<CommitmentDto>()
            .ProducesValidationProblem()
            .ProducesProblem(404);

        group.MapDelete("/{id:guid}/items/{itemId:guid}", RemoveCommitmentItem)
            .WithName("RemoveCommitmentItem")
            .WithSummary("Remove commitment item")
            .Produces(204)
            .ProducesProblem(404);

        group.MapGet("/{id:guid}/items", GetCommitmentItems)
            .WithName("GetCommitmentItems")
            .WithSummary("Get commitment items")
            .Produces<List<CommitmentItemDto>>();

        // Financial endpoints
        group.MapPost("/{id:guid}/invoice", RecordInvoice)
            .WithName("RecordCommitmentInvoice")
            .WithSummary("Record invoice for commitment")
            .Produces<CommitmentDto>()
            .ProducesValidationProblem()
            .ProducesProblem(404);

        group.MapPost("/{id:guid}/performance", UpdatePerformance)
            .WithName("UpdateCommitmentPerformance")
            .WithSummary("Update commitment performance")
            .Produces<CommitmentDto>()
            .ProducesValidationProblem()
            .ProducesProblem(404);

        // Reporting endpoints
        group.MapGet("/{id:guid}/revisions", GetCommitmentRevisions)
            .WithName("GetCommitmentRevisions")
            .WithSummary("Get commitment revisions")
            .Produces<List<CommitmentRevisionDto>>();

        group.MapGet("/{id:guid}/financial-summary", GetFinancialSummary)
            .WithName("GetCommitmentFinancialSummary")
            .WithSummary("Get commitment financial summary")
            .Produces<CommitmentFinancialSummary>()
            .ProducesProblem(404);

        group.MapGet("/{id:guid}/performance-metrics", GetPerformanceMetrics)
            .WithName("GetCommitmentPerformanceMetrics")
            .WithSummary("Get commitment performance metrics")
            .Produces<CommitmentPerformanceMetrics>()
            .ProducesProblem(404);

        group.MapPost("/export", ExportCommitments)
            .WithName("ExportCommitments")
            .WithSummary("Export commitments")
            .Produces(200);

        // Validation endpoints
        group.MapGet("/validate/number/{number}", ValidateCommitmentNumber)
            .WithName("ValidateCommitmentNumber")
            .WithSummary("Validate commitment number uniqueness")
            .Produces<bool>();
    }

    // Query handlers
    private static async Task<IResult> SearchCommitments(
        [FromBody] CommitmentFilterDto filter,
        ICommitmentService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetCommitmentsAsync(filter);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetCommitmentById(
        [FromRoute] Guid id,
        ICommitmentService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetCommitmentAsync(id);
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> GetCommitmentDetail(
        [FromRoute] Guid id,
        ICommitmentService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetCommitmentDetailAsync(id);
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> GetProjectCommitments(
        [FromRoute] Guid projectId,
        ICommitmentService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetProjectCommitmentsAsync(projectId);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetProjectCommitmentSummary(
        [FromRoute] Guid projectId,
        ICommitmentService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetProjectCommitmentSummaryAsync(projectId);
        return Results.Ok(result);
    }

    // Command handlers
    private static async Task<IResult> CreateCommitment(
        [FromBody] CreateCommitmentDto dto,
        ICommitmentService service,
        CancellationToken cancellationToken)
    {
        var result = await service.CreateCommitmentAsync(dto);
        return Results.Created($"/api/commitments/{result.Id}", result);
    }

    private static async Task<IResult> UpdateCommitment(
        [FromRoute] Guid id,
        [FromBody] UpdateCommitmentDto dto,
        ICommitmentService service,
        CancellationToken cancellationToken)
    {
        var result = await service.UpdateCommitmentAsync(id, dto);
        return Results.Ok(result);
    }

    private static async Task<IResult> DeleteCommitment(
        [FromRoute] Guid id,
        ICommitmentService service,
        CancellationToken cancellationToken)
    {
        var deleted = await service.DeleteCommitmentAsync(id);
        return deleted ? Results.NoContent() : Results.NotFound();
    }

    // Workflow handlers
    private static async Task<IResult> SubmitForApproval(
        [FromRoute] Guid id,
        ICommitmentService service,
        CancellationToken cancellationToken)
    {
        var result = await service.SubmitForApprovalAsync(id);
        return Results.Ok(result);
    }

    private static async Task<IResult> ApproveCommitment(
        [FromRoute] Guid id,
        [FromBody] ApproveCommitmentDto dto,
        ICommitmentService service,
        CancellationToken cancellationToken)
    {
        var result = await service.ApproveCommitmentAsync(id, dto);
        return Results.Ok(result);
    }

    private static async Task<IResult> RejectCommitment(
        [FromRoute] Guid id,
        [FromBody] CancelCommitmentDto dto,
        ICommitmentService service,
        CancellationToken cancellationToken)
    {
        var result = await service.RejectCommitmentAsync(id, dto);
        return Results.Ok(result);
    }

    private static async Task<IResult> ActivateCommitment(
        [FromRoute] Guid id,
        ICommitmentService service,
        CancellationToken cancellationToken)
    {
        var result = await service.ActivateCommitmentAsync(id);
        return Results.Ok(result);
    }

    private static async Task<IResult> ReviseCommitment(
        [FromRoute] Guid id,
        [FromBody] ReviseCommitmentDto dto,
        ICommitmentService service,
        CancellationToken cancellationToken)
    {
        var result = await service.ReviseCommitmentAsync(id, dto);
        return Results.Ok(result);
    }

    private static async Task<IResult> CloseCommitment(
        [FromRoute] Guid id,
        ICommitmentService service,
        CancellationToken cancellationToken)
    {
        var result = await service.CloseCommitmentAsync(id);
        return Results.Ok(result);
    }

    private static async Task<IResult> CancelCommitment(
        [FromRoute] Guid id,
        [FromBody] CancelCommitmentDto dto,
        ICommitmentService service,
        CancellationToken cancellationToken)
    {
        var result = await service.CancelCommitmentAsync(id, dto);
        return Results.Ok(result);
    }

    // Work Package allocation handlers
    private static async Task<IResult> AddWorkPackageAllocation(
        [FromRoute] Guid id,
        [FromBody] CommitmentWorkPackageAllocationDto dto,
        ICommitmentService service,
        CancellationToken cancellationToken)
    {
        var result = await service.AddWorkPackageAllocationAsync(id, dto);
        return Results.Ok(result);
    }

    private static async Task<IResult> UpdateWorkPackageAllocation(
        [FromRoute] Guid id,
        [FromRoute] Guid allocationId,
        [FromBody] UpdateAllocationAmountDto dto,
        ICommitmentService service,
        CancellationToken cancellationToken)
    {
        var result = await service.UpdateWorkPackageAllocationAsync(id, allocationId, dto.Amount);
        return Results.Ok(result);
    }

    private static async Task<IResult> RemoveWorkPackageAllocation(
        [FromRoute] Guid id,
        [FromRoute] Guid allocationId,
        ICommitmentService service,
        CancellationToken cancellationToken)
    {
        var deleted = await service.RemoveWorkPackageAllocationAsync(id, allocationId);
        return deleted ? Results.NoContent() : Results.NotFound();
    }

    private static async Task<IResult> GetWorkPackageAllocations(
        [FromRoute] Guid id,
        ICommitmentService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetWorkPackageAllocationsAsync(id);
        return Results.Ok(result);
    }

    // Commitment Item handlers
    private static async Task<IResult> AddCommitmentItem(
        [FromRoute] Guid id,
        [FromBody] CreateCommitmentItemDto dto,
        ICommitmentService service,
        CancellationToken cancellationToken)
    {
        var result = await service.AddCommitmentItemAsync(id, dto);
        return Results.Ok(result);
    }

    private static async Task<IResult> UpdateCommitmentItem(
        [FromRoute] Guid id,
        [FromRoute] Guid itemId,
        [FromBody] UpdateCommitmentItemDto dto,
        ICommitmentService service,
        CancellationToken cancellationToken)
    {
        var result = await service.UpdateCommitmentItemAsync(id, itemId, dto);
        return Results.Ok(result);
    }

    private static async Task<IResult> RemoveCommitmentItem(
        [FromRoute] Guid id,
        [FromRoute] Guid itemId,
        ICommitmentService service,
        CancellationToken cancellationToken)
    {
        var deleted = await service.RemoveCommitmentItemAsync(id, itemId);
        return deleted ? Results.NoContent() : Results.NotFound();
    }

    private static async Task<IResult> GetCommitmentItems(
        [FromRoute] Guid id,
        ICommitmentService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetCommitmentItemsAsync(id);
        return Results.Ok(result);
    }

    // Financial handlers
    private static async Task<IResult> RecordInvoice(
        [FromRoute] Guid id,
        [FromBody] RecordCommitmentInvoiceDto dto,
        ICommitmentService service,
        CancellationToken cancellationToken)
    {
        var result = await service.RecordInvoiceAsync(id, dto);
        return Results.Ok(result);
    }

    private static async Task<IResult> UpdatePerformance(
        [FromRoute] Guid id,
        [FromBody] UpdateCommitmentPerformanceDto dto,
        ICommitmentService service,
        CancellationToken cancellationToken)
    {
        var result = await service.UpdatePerformanceAsync(id, dto);
        return Results.Ok(result);
    }

    // Reporting handlers
    private static async Task<IResult> GetCommitmentRevisions(
        [FromRoute] Guid id,
        ICommitmentService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetCommitmentRevisionsAsync(id);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetFinancialSummary(
        [FromRoute] Guid id,
        ICommitmentService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetFinancialSummaryAsync(id);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetPerformanceMetrics(
        [FromRoute] Guid id,
        ICommitmentService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetPerformanceMetricsAsync(id);
        return Results.Ok(result);
    }

    private static async Task<IResult> ExportCommitments(
        [FromBody] ExportCommitmentsRequestDto request,
        ICommitmentService service,
        CancellationToken cancellationToken)
    {
        var result = await service.ExportCommitmentsAsync(request.Filter, request.Format);
        return Results.File(result, $"application/{request.Format}", $"commitments.{request.Format}");
    }

    // Validation handlers
    private static async Task<IResult> ValidateCommitmentNumber(
        [FromRoute] string number,
        [FromQuery] Guid? excludeId,
        ICommitmentService service,
        CancellationToken cancellationToken)
    {
        var isUnique = await service.IsCommitmentNumberUniqueAsync(number, excludeId);
        return Results.Ok(isUnique);
    }
}

// Request DTOs
public record UpdateAllocationAmountDto(decimal Amount);
public record ExportCommitmentsRequestDto(CommitmentFilterDto Filter, string Format = "xlsx");