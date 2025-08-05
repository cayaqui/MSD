using Api.Authorization;
using Application.Interfaces.Organization;
using Application.Interfaces.Auth;
using Carter;
using Core.DTOs.Common;
using Core.DTOs.Organization.RAM;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Organization;

/// <summary>
/// Endpoints for RAM (Responsibility Assignment Matrix) management
/// </summary>
public class RAMModule : CarterModule
{
    public RAMModule() : base("/api/ram")
    {
        RequireAuthorization();
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        // Query endpoints
        app.MapGet("/", GetRAMsAsync)
            .WithName("GetRAMs")
            .WithSummary("Get all RAM assignments with pagination")
            .WithDescription("Returns a paginated list of RAM assignments")
            .WithTags("RAM")
            .Produces<PagedResult<RAMDto>>();

        app.MapGet("/{id:guid}", GetRAMByIdAsync)
            .WithName("GetRAMById")
            .WithSummary("Get RAM assignment by ID")
            .WithDescription("Returns a specific RAM assignment by ID")
            .WithTags("RAM")
            .Produces<RAMDto>()
            .Produces(404);

        app.MapGet("/project/{projectId:guid}", GetRAMByProjectAsync)
            .WithName("GetRAMByProject")
            .WithSummary("Get RAM assignments by project")
            .WithDescription("Returns all RAM assignments for a specific project")
            .WithTags("RAM")
            .Produces<List<RAMDto>>()
            .Produces(404);

        app.MapGet("/project/{projectId:guid}/matrix", GetProjectRAMMatrixAsync)
            .WithName("GetProjectRAMMatrix")
            .WithSummary("Get project RAM matrix")
            .WithDescription("Returns the complete RAM matrix for a project")
            .WithTags("RAM")
            .Produces<RAMMatrixDto>()
            .Produces(404);

        app.MapGet("/user/{userId:guid}/assignments", GetUserRAMAssignmentsAsync)
            .WithName("GetUserRAMAssignments")
            .WithSummary("Get user RAM assignments")
            .WithDescription("Returns all RAM assignments for a specific user")
            .WithTags("RAM")
            .Produces<List<UserRAMAssignmentDto>>()
            .Produces(404);

        app.MapGet("/obs-node/{obsNodeId:guid}", GetRAMByOBSNodeAsync)
            .WithName("GetRAMByOBSNode")
            .WithSummary("Get RAM assignments by OBS node")
            .WithDescription("Returns all RAM assignments for a specific OBS node")
            .WithTags("RAM")
            .Produces<List<RAMDto>>()
            .Produces(404);

        // Command endpoints
        app.MapPost("/", CreateRAMAsync)
            .WithName("CreateRAM")
            .WithSummary("Create a new RAM assignment")
            .WithDescription("Creates a new RAM assignment")
            .WithTags("RAM")
            .RequireAuthorization("ProjectEdit")
            .Produces<Result<Guid>>(201)
            .Produces<Result>(400);

        app.MapPut("/{id:guid}", UpdateRAMAsync)
            .WithName("UpdateRAM")
            .WithSummary("Update RAM assignment")
            .WithDescription("Updates an existing RAM assignment")
            .WithTags("RAM")
            .RequireAuthorization("ProjectEdit")
            .Produces<Result>()
            .Produces<Result>(400)
            .Produces(404);

        app.MapPost("/bulk-assign", BulkAssignRAMAsync)
            .WithName("BulkAssignRAM")
            .WithSummary("Bulk assign RAM")
            .WithDescription("Creates multiple RAM assignments in a single operation")
            .WithTags("RAM")
            .RequireAuthorization("ProjectEdit")
            .Produces<BulkOperationResult>()
            .Produces<Result>(400);

        app.MapPost("/import", ImportRAMMatrixAsync)
            .WithName("ImportRAMMatrix")
            .WithSummary("Import RAM matrix")
            .WithDescription("Imports a complete RAM matrix from Excel")
            .WithTags("RAM")
            .RequireAuthorization("ProjectEdit")
            .Produces<ImportResultDto>()
            .Produces<Result>(400);

        app.MapGet("/export/{projectId:guid}", ExportRAMMatrixAsync)
            .WithName("ExportRAMMatrix")
            .WithSummary("Export RAM matrix")
            .WithDescription("Exports the RAM matrix to Excel")
            .WithTags("RAM")
            .Produces(200, contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            .Produces(404);

        app.MapPost("/{id:guid}/add-notes", AddRAMNotesAsync)
            .WithName("AddRAMNotes")
            .WithSummary("Add notes to RAM assignment")
            .WithDescription("Adds notes to a RAM assignment")
            .WithTags("RAM")
            .RequireAuthorization("ProjectEdit")
            .Produces<Result>()
            .Produces<Result>(400)
            .Produces(404);

        app.MapDelete("/{id:guid}", DeleteRAMAsync)
            .WithName("DeleteRAM")
            .WithSummary("Delete RAM assignment")
            .WithDescription("Soft deletes a RAM assignment")
            .WithTags("RAM")
            .RequireAuthorization("ProjectEdit")
            .Produces<Result>()
            .Produces<Result>(400)
            .Produces(404);
    }

    private static async Task<IResult> GetRAMsAsync(
        [FromServices] IRAMService ramService,
        [AsParameters] SimpleQueryParameters parameters,
        CancellationToken cancellationToken)
    {
        var result = await ramService.GetAllPagedAsync(parameters.PageNumber, parameters.PageSize, parameters.SortBy, !parameters.IsAscending, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetRAMByIdAsync(
        [FromRoute] Guid id,
        [FromServices] IRAMService ramService,
        CancellationToken cancellationToken)
    {
        var result = await ramService.GetByIdAsync(id, cancellationToken);
        return result is not null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> GetRAMByProjectAsync(
        [FromRoute] Guid projectId,
        [FromServices] IRAMService ramService,
        CancellationToken cancellationToken)
    {
        var result = await ramService.GetByProjectAsync(projectId, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetProjectRAMMatrixAsync(
        [FromRoute] Guid projectId,
        [FromServices] IRAMService ramService,
        CancellationToken cancellationToken)
    {
        var result = await ramService.GetRAMMatrixAsync(projectId, cancellationToken);
        return result is not null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> GetUserRAMAssignmentsAsync(
        [FromRoute] Guid userId,
        [FromServices] IRAMService ramService,
        CancellationToken cancellationToken)
    {
        // This would need to be implemented in the service
        return Results.Ok(new List<UserRAMAssignmentDto>());
    }

    private static async Task<IResult> GetRAMByOBSNodeAsync(
        [FromRoute] Guid obsNodeId,
        [FromServices] IRAMService ramService,
        CancellationToken cancellationToken)
    {
        var result = await ramService.GetByOBSNodeAsync(obsNodeId, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> CreateRAMAsync(
        [FromBody] CreateRAMDto dto,
        [FromServices] IRAMService ramService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("User ID not found");
        var result = await ramService.CreateAsync(dto, userId, cancellationToken);
        
        return result != null 
            ? Results.CreatedAtRoute("GetRAMById", new { id = result.Id }, result)
            : Results.BadRequest("Failed to create RAM assignment");
    }

    private static async Task<IResult> UpdateRAMAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateRAMDto dto,
        [FromServices] IRAMService ramService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("User ID not found");
        var result = await ramService.UpdateAsync(id, dto, userId, cancellationToken);
        
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> BulkAssignRAMAsync(
        [FromBody] List<CreateRAMDto> dto,
        [FromServices] IRAMService ramService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("User ID not found");
        var result = await ramService.BulkCreateAsync(dto, userId, cancellationToken);
        
        return Results.Ok(result);
    }

    private static async Task<IResult> ImportRAMMatrixAsync(
        IFormFile file,
        [FromQuery] Guid projectId,
        [FromServices] IRAMService ramService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
            return Results.BadRequest(Result.Failure("No file uploaded"));

        using var stream = new MemoryStream();
        await file.CopyToAsync(stream, cancellationToken);
        stream.Position = 0;

        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("User ID not found");
        // This would need to be implemented in the service
        return Results.Ok(new ImportResultDto { SuccessfulRecords = 0, FailedRecords = 0 });
    }

    private static async Task<IResult> ExportRAMMatrixAsync(
        [FromRoute] Guid projectId,
        [FromServices] IRAMService ramService,
        CancellationToken cancellationToken)
    {
        // This would need to be implemented in the service
        var result = await ramService.GetRAMMatrixAsync(projectId, cancellationToken);
        if (result == null) return Results.NotFound();
        
        // Return empty file for now
        var emptyFile = new byte[0];
        return Results.File(
            emptyFile,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"RAM_Matrix_{projectId}_{DateTime.UtcNow:yyyyMMdd}.xlsx");
    }

    private static async Task<IResult> AddRAMNotesAsync(
        [FromRoute] Guid id,
        [FromBody] AddRAMNotesDto dto,
        [FromServices] IRAMService ramService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("User ID not found");
        // This would need to be implemented using Update method
        var existing = await ramService.GetByIdAsync(id, cancellationToken);
        if (existing == null) return Results.NotFound();
        
        var updateDto = new UpdateRAMDto
        {
            ResponsibilityType = existing.ResponsibilityType,
            AllocationPercentage = existing.AllocationPercentage,
            PlannedManHours = existing.PlannedManHours,
            PlannedCost = existing.PlannedCost,
            StartDate = existing.StartDate,
            EndDate = existing.EndDate,
            ControlAccountId = existing.ControlAccountId,
            Notes = dto.Notes,
            IsActive = existing.IsActive
        };
        
        var result = await ramService.UpdateAsync(id, updateDto, userId, cancellationToken);
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> DeleteRAMAsync(
        [FromRoute] Guid id,
        [FromServices] IRAMService ramService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("User ID not found");
        var result = await ramService.DeleteAsync(id, cancellationToken);
        
        return result ? Results.Ok("RAM assignment deleted successfully") : Results.BadRequest("Failed to delete RAM assignment");
    }
}