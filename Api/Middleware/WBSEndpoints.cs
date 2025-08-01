using Application.Interfaces.Projects;
using Carter;
using Core.DTOs.Common;
using Core.DTOs.WBS;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Middleware;

/// <summary>
/// WBS (Work Breakdown Structure) endpoints
/// </summary>
public class WBSEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/wbs")
            .WithTags("WBS")
            .RequireAuthorization();

        // GET endpoints
        group.MapGet("", GetWBSElements)
            .WithName("GetWBSElements")
            .WithSummary("Get WBS elements for a project")
            .Produces<PagedResult<WBSElementDto>>();

        group.MapGet("{id:guid}", GetWBSElementById)
            .WithName("GetWBSElementById")
            .WithSummary("Get WBS element by ID")
            .Produces<WBSElementDetailDto>()
            .Produces(404);

        group.MapGet("project/{projectId:guid}/hierarchy", GetWBSHierarchy)
            .WithName("GetWBSHierarchy")
            .WithSummary("Get complete WBS hierarchy for a project")
            .Produces<List<WBSElementDto>>();

        group.MapGet("{parentId:guid}/children", GetWBSChildren)
            .WithName("GetWBSChildren")
            .WithSummary("Get children of a WBS element")
            .Produces<List<WBSElementDto>>();

        group.MapGet("control-account/{controlAccountId:guid}/work-packages", GetWorkPackagesByControlAccount)
            .WithName("GetWorkPackagesByControlAccount")
            .WithSummary("Get work packages for a control account")
            .Produces<List<WBSElementDto>>();

        group.MapGet("{id:guid}/dictionary", GetWBSDictionary)
            .WithName("GetWBSDictionary")
            .WithSummary("Get WBS dictionary information")
            .Produces<WBSDictionaryDto>()
            .Produces(404);

        // POST endpoints
        group.MapPost("", CreateWBSElement)
            .WithName("CreateWBSElement")
            .WithSummary("Create a new WBS element")
            .Produces<Created<Guid>>()
            .ProducesValidationProblem();

        group.MapPost("{id:guid}/convert-to-work-package", ConvertToWorkPackage)
            .WithName("ConvertToWorkPackage")
            .WithSummary("Convert WBS element to work package")
            .Produces(204)
            .Produces(404)
            .ProducesValidationProblem();

        group.MapPost("{id:guid}/convert-to-planning-package", ConvertToPlanningPackage)
            .WithName("ConvertToPlanningPackage")
            .WithSummary("Convert WBS element to planning package")
            .Produces(204)
            .Produces(404);

        group.MapPost("{id:guid}/convert-planning-to-work", ConvertPlanningToWorkPackage)
            .WithName("ConvertPlanningToWorkPackage")
            .WithSummary("Convert planning package to work package")
            .Produces(204)
            .Produces(404)
            .ProducesValidationProblem();

        group.MapPost("reorder", ReorderWBSElements)
            .WithName("ReorderWBSElements")
            .WithSummary("Reorder WBS elements")
            .Produces(204);

        // PUT endpoints
        group.MapPut("{id:guid}", UpdateWBSElement)
            .WithName("UpdateWBSElement")
            .WithSummary("Update WBS element basic info")
            .Produces(204)
            .Produces(404)
            .ProducesValidationProblem();

        group.MapPut("{id:guid}/dictionary", UpdateWBSDictionary)
            .WithName("UpdateWBSDictionary")
            .WithSummary("Update WBS dictionary")
            .Produces(204)
            .Produces(404);

        // DELETE endpoints
        group.MapDelete("{id:guid}", DeleteWBSElement)
            .WithName("DeleteWBSElement")
            .WithSummary("Delete WBS element (soft delete)")
            .Produces(204)
            .Produces(404);

        // Validation endpoints
        group.MapPost("validate-code", ValidateWBSCode)
            .WithName("ValidateWBSCode")
            .WithSummary("Validate WBS code")
            .Produces<ValidationResult>();
    }

    // GET endpoints implementation
    private static async Task<Ok<PagedResult<WBSElementDto>>> GetWBSElements(
        [FromQuery] Guid projectId,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        [FromQuery] string? searchTerm,
        [FromQuery] string? sortBy,
        [FromQuery] string? sortDirection,
        IWBSService service,
        CancellationToken cancellationToken)
    {
        var parameters = new QueryParameters
        {
            PageNumber = pageNumber > 0 ? pageNumber : 1,
            PageSize = pageSize > 0 ? pageSize : 20,
            SearchTerm = searchTerm,
            SortBy = sortBy,
            SortDirection = sortDirection ?? "asc"
        };

        var result = await service.GetWBSElementsAsync(projectId, parameters, cancellationToken);
        return TypedResults.Ok(result);
    }

    private static async Task<Results<Ok<WBSElementDetailDto>, NotFound>> GetWBSElementById(
        Guid id,
        IWBSService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetWBSElementByIdAsync(id, cancellationToken);
        return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }

    private static async Task<Ok<List<WBSElementDto>>> GetWBSHierarchy(
        Guid projectId,
        IWBSService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetWBSHierarchyAsync(projectId, cancellationToken);
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<List<WBSElementDto>>> GetWBSChildren(
        Guid parentId,
        IWBSService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetChildrenAsync(parentId, cancellationToken);
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<List<WBSElementDto>>> GetWorkPackagesByControlAccount(
        Guid controlAccountId,
        IWBSService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetWorkPackagesByControlAccountAsync(controlAccountId, cancellationToken);
        return TypedResults.Ok(result);
    }

    private static async Task<Results<Ok<WBSDictionaryDto>, NotFound>> GetWBSDictionary(
        Guid id,
        IWBSService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetWBSDictionaryAsync(id, cancellationToken);
        return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }

    // POST endpoints implementation
    private static async Task<Results<Created<Guid>, ValidationProblem>> CreateWBSElement(
        CreateWBSElementDto dto,
        IWBSService service,
        IValidator<CreateWBSElementDto> validator,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var userId = httpContext.User.Identity?.Name ?? "system";
        var result = await service.CreateWBSElementAsync(dto, userId, cancellationToken);

        if (result.IsFailure)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { "error", new[] { result.Error ?? "Failed to create WBS element" } }
            });
        }

        return TypedResults.Created($"/api/wbs/{result.Value}", result.Value);
    }

    private static async Task<Results<NoContent, NotFound, ValidationProblem>> ConvertToWorkPackage(
        Guid id,
        ConvertToWorkPackageDto dto,
        IWBSService service,
        IValidator<ConvertToWorkPackageDto> validator,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var userId = httpContext.User.Identity?.Name ?? "system";
        var result = await service.ConvertToWorkPackageAsync(id, dto, userId, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error?.Contains("not found", StringComparison.OrdinalIgnoreCase) ?? false)
                return TypedResults.NotFound();

            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { "error", new[] { result.Error ?? "Failed to convert to work package" } }
            });
        }

        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound>> ConvertToPlanningPackage(
        Guid id,
        [FromQuery] Guid controlAccountId,
        IWBSService service,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = httpContext.User.Identity?.Name ?? "system";
        var result = await service.ConvertToPlanningPackageAsync(id, controlAccountId, userId, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error?.Contains("not found", StringComparison.OrdinalIgnoreCase) ?? false)
                return TypedResults.NotFound();

            return TypedResults.NotFound(); // Should be ValidationProblem
        }

        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound, ValidationProblem>> ConvertPlanningToWorkPackage(
        Guid id,
        ConvertPlanningToWorkPackageDto dto,
        IWBSService service,
        IValidator<ConvertPlanningToWorkPackageDto> validator,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var userId = httpContext.User.Identity?.Name ?? "system";
        var result = await service.ConvertPlanningPackageToWorkPackageAsync(id, dto, userId, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error?.Contains("not found", StringComparison.OrdinalIgnoreCase) ?? false)
                return TypedResults.NotFound();

            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { "error", new[] { result.Error ?? "Failed to convert planning package" } }
            });
        }

        return TypedResults.NoContent();
    }

    private static async Task<NoContent> ReorderWBSElements(
        ReorderWBSElementsDto dto,
        IWBSService service,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = httpContext.User.Identity?.Name ?? "system";
        await service.ReorderWBSElementsAsync(dto, userId, cancellationToken);
        return TypedResults.NoContent();
    }

    // PUT endpoints implementation
    private static async Task<Results<NoContent, NotFound, ValidationProblem>> UpdateWBSElement(
        Guid id,
        UpdateWBSElementDto dto,
        IWBSService service,
        IValidator<UpdateWBSElementDto> validator,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var userId = httpContext.User.Identity?.Name ?? "system";
        var result = await service.UpdateWBSElementAsync(id, dto, userId, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error?.Contains("not found", StringComparison.OrdinalIgnoreCase) ?? false)
                return TypedResults.NotFound();

            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { "error", new[] { result.Error ?? "Failed to update WBS element" } }
            });
        }

        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound>> UpdateWBSDictionary(
        Guid id,
        UpdateWBSDictionaryDto dto,
        IWBSService service,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = httpContext.User.Identity?.Name ?? "system";
        var result = await service.UpdateWBSDictionaryAsync(id, dto, userId, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error?.Contains("not found", StringComparison.OrdinalIgnoreCase) ?? false)
                return TypedResults.NotFound();

            return TypedResults.NotFound(); // Should be ValidationProblem
        }

        return TypedResults.NoContent();
    }

    // DELETE endpoints implementation
    private static async Task<Results<NoContent, NotFound>> DeleteWBSElement(
        Guid id,
        IWBSService service,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = httpContext.User.Identity?.Name ?? "system";
        var result = await service.DeleteWBSElementAsync(id, userId, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error?.Contains("not found", StringComparison.OrdinalIgnoreCase) ?? false)
                return TypedResults.NotFound();

            return TypedResults.NotFound(); // Should be ValidationProblem
        }

        return TypedResults.NoContent();
    }

    // Validation endpoints implementation
    private static async Task<Ok<ValidationResult>> ValidateWBSCode(
        [FromQuery] string code,
        [FromQuery] Guid projectId,
        [FromQuery] Guid? excludeId,
        IWBSService service,
        CancellationToken cancellationToken)
    {
        var result = await service.ValidateWBSCodeAsync(code, projectId, excludeId, cancellationToken);
        return TypedResults.Ok(result);
    }
}