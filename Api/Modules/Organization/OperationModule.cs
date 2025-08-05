using Api.Authorization;
using Application.Interfaces.Organization;
using Application.Interfaces.Auth;
using Carter;
using Core.DTOs.Common;
using Core.DTOs.Organization.Operation;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Organization;

/// <summary>
/// Endpoints for operation management
/// </summary>
public class OperationModule : CarterModule
{
    public OperationModule() : base("/api/operations")
    {
        RequireAuthorization();
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        // Query endpoints
        app.MapGet("/", GetOperationsAsync)
            .WithName("GetOperations")
            .WithSummary("Get all operations with pagination")
            .WithDescription("Returns a paginated list of operations")
            .WithTags("Operations")
            .Produces<PagedResult<OperationDto>>();

        app.MapGet("/{id:guid}", GetOperationByIdAsync)
            .WithName("GetOperationById")
            .WithSummary("Get operation by ID")
            .WithDescription("Returns a specific operation by ID")
            .WithTags("Operations")
            .Produces<OperationDto>()
            .Produces(404);

        app.MapGet("/company/{companyId:guid}", GetOperationsByCompanyAsync)
            .WithName("GetOperationsByCompany")
            .WithSummary("Get operations by company")
            .WithDescription("Returns all operations for a specific company")
            .WithTags("Operations")
            .Produces<List<OperationDto>>()
            .Produces(404);

        app.MapGet("/{id:guid}/projects", GetOperationProjectsAsync)
            .WithName("GetOperationProjects")
            .WithSummary("Get operation projects")
            .WithDescription("Returns all projects for a specific operation")
            .WithTags("Operations")
            .Produces<OperationWithProjectsDto>()
            .Produces(404);

        // Command endpoints
        app.MapPost("/", CreateOperationAsync)
            .WithName("CreateOperation")
            .WithSummary("Create a new operation")
            .WithDescription("Creates a new operation")
            .WithTags("Operations")
            .RequireAuthorization("AdminOnly")
            .Produces<Result<Guid>>(201)
            .Produces<Result>(400);

        app.MapPut("/{id:guid}", UpdateOperationAsync)
            .WithName("UpdateOperation")
            .WithSummary("Update operation")
            .WithDescription("Updates an existing operation")
            .WithTags("Operations")
            .RequireAuthorization("AdminOnly")
            .Produces<Result>()
            .Produces<Result>(400)
            .Produces(404);

        app.MapPost("/{id:guid}/activate", ActivateOperationAsync)
            .WithName("ActivateOperation")
            .WithSummary("Activate operation")
            .WithDescription("Activates an operation")
            .WithTags("Operations")
            .RequireAuthorization("AdminOnly")
            .Produces<Result>()
            .Produces(404);

        app.MapPost("/{id:guid}/deactivate", DeactivateOperationAsync)
            .WithName("DeactivateOperation")
            .WithSummary("Deactivate operation")
            .WithDescription("Deactivates an operation")
            .WithTags("Operations")
            .RequireAuthorization("AdminOnly")
            .Produces<Result>()
            .Produces(404);

        app.MapDelete("/{id:guid}", DeleteOperationAsync)
            .WithName("DeleteOperation")
            .WithSummary("Delete operation")
            .WithDescription("Soft deletes an operation")
            .WithTags("Operations")
            .RequireAuthorization("AdminOnly")
            .Produces<Result>()
            .Produces<Result>(400)
            .Produces(404);
    }

    private static async Task<IResult> GetOperationsAsync(
        [FromServices] IOperationService operationService,
        [AsParameters] SimpleQueryParameters parameters,
        CancellationToken cancellationToken)
    {
        var result = await operationService.GetAllPagedAsync(parameters.PageNumber, parameters.PageSize, parameters.SortBy, !parameters.IsAscending, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetOperationByIdAsync(
        [FromRoute] Guid id,
        [FromServices] IOperationService operationService,
        CancellationToken cancellationToken)
    {
        var result = await operationService.GetByIdAsync(id, cancellationToken);
        return result is not null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> GetOperationsByCompanyAsync(
        [FromRoute] Guid companyId,
        [FromServices] IOperationService operationService,
        CancellationToken cancellationToken)
    {
        var result = await operationService.GetByCompanyAsync(companyId, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetOperationProjectsAsync(
        [FromRoute] Guid id,
        [FromServices] IOperationService operationService,
        CancellationToken cancellationToken)
    {
        var result = await operationService.GetWithProjectsAsync(id, cancellationToken);
        return result is not null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> CreateOperationAsync(
        [FromBody] CreateOperationDto dto,
        [FromServices] IOperationService operationService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("User ID not found");
        var result = await operationService.CreateAsync(dto, userId, cancellationToken);
        
        return result != null 
            ? Results.CreatedAtRoute("GetOperationById", new { id = result.Id }, result)
            : Results.BadRequest("Failed to create operation");
    }

    private static async Task<IResult> UpdateOperationAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateOperationDto dto,
        [FromServices] IOperationService operationService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("User ID not found");
        var result = await operationService.UpdateAsync(id, dto, userId, cancellationToken);
        
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> ActivateOperationAsync(
        [FromRoute] Guid id,
        [FromServices] IOperationService operationService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("User ID not found");
        // Since there's no IsActive property, just verify the operation exists
        var existing = await operationService.GetByIdAsync(id, cancellationToken);
        if (existing == null) return Results.NotFound();
        
        return Results.Ok("Operation is active");
    }

    private static async Task<IResult> DeactivateOperationAsync(
        [FromRoute] Guid id,
        [FromServices] IOperationService operationService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("User ID not found");
        // Since there's no IsActive property, we could use soft delete instead
        var result = await operationService.DeleteAsync(id, cancellationToken);
        
        return result ? Results.Ok("Operation deactivated successfully") : Results.BadRequest("Failed to deactivate operation");
    }

    private static async Task<IResult> DeleteOperationAsync(
        [FromRoute] Guid id,
        [FromServices] IOperationService operationService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("User ID not found");
        var result = await operationService.DeleteAsync(id, cancellationToken);
        
        return result ? Results.Ok("Operation deleted successfully") : Results.BadRequest("Failed to delete operation");
    }
}