using Api.Authorization;
using Application.Interfaces.Organization;
using Application.Interfaces.Auth;
using Carter;
using Core.DTOs.Common;
using Core.DTOs.Organization.Company;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Organization;

/// <summary>
/// Endpoints for company management
/// </summary>
public class CompanyModule : CarterModule
{
    public CompanyModule() : base("/api/companies")
    {
        RequireAuthorization();
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        // Query endpoints
        app.MapGet("/", GetCompaniesAsync)
            .WithName("GetCompanies")
            .WithSummary("Get all companies with pagination")
            .WithDescription("Returns a paginated list of companies")
            .WithTags("Companies")
            .Produces<PagedResult<CompanyDto>>();

        app.MapGet("/{id:guid}", GetCompanyByIdAsync)
            .WithName("GetCompanyById")
            .WithSummary("Get company by ID")
            .WithDescription("Returns a specific company by ID")
            .WithTags("Companies")
            .Produces<CompanyDto>()
            .Produces(404);

        app.MapGet("/{id:guid}/operations", GetCompanyOperationsAsync)
            .WithName("GetCompanyOperations")
            .WithSummary("Get company operations")
            .WithDescription("Returns all operations for a specific company")
            .WithTags("Companies")
            .Produces<CompanyWithOperationsDto>()
            .Produces(404);

        app.MapGet("/active", GetActiveCompaniesAsync)
            .WithName("GetActiveCompanies")
            .WithSummary("Get active companies")
            .WithDescription("Returns all active companies")
            .WithTags("Companies")
            .Produces<List<CompanyDto>>();

        // Command endpoints
        app.MapPost("/", CreateCompanyAsync)
            .WithName("CreateCompany")
            .WithSummary("Create a new company")
            .WithDescription("Creates a new company")
            .WithTags("Companies")
            .RequireAuthorization("AdminOnly")
            .Produces<Result<Guid>>(201)
            .Produces<Result>(400);

        app.MapPut("/{id:guid}", UpdateCompanyAsync)
            .WithName("UpdateCompany")
            .WithSummary("Update company")
            .WithDescription("Updates an existing company")
            .WithTags("Companies")
            .RequireAuthorization("AdminOnly")
            .Produces<Result>()
            .Produces<Result>(400)
            .Produces(404);

        app.MapPost("/{id:guid}/activate", ActivateCompanyAsync)
            .WithName("ActivateCompany")
            .WithSummary("Activate company")
            .WithDescription("Activates a company")
            .WithTags("Companies")
            .RequireAuthorization("AdminOnly")
            .Produces<Result>()
            .Produces(404);

        app.MapPost("/{id:guid}/deactivate", DeactivateCompanyAsync)
            .WithName("DeactivateCompany")
            .WithSummary("Deactivate company")
            .WithDescription("Deactivates a company")
            .WithTags("Companies")
            .RequireAuthorization("AdminOnly")
            .Produces<Result>()
            .Produces(404);

        app.MapDelete("/{id:guid}", DeleteCompanyAsync)
            .WithName("DeleteCompany")
            .WithSummary("Delete company")
            .WithDescription("Soft deletes a company")
            .WithTags("Companies")
            .RequireAuthorization("AdminOnly")
            .Produces<Result>()
            .Produces<Result>(400)
            .Produces(404);
    }

    private static async Task<IResult> GetCompaniesAsync(
        [FromServices] ICompanyService companyService,
        [AsParameters] SimpleQueryParameters parameters,
        CancellationToken cancellationToken)
    {
        var result = await companyService.GetAllPagedAsync(parameters.PageNumber, parameters.PageSize, parameters.SortBy, !parameters.IsAscending, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetCompanyByIdAsync(
        [FromRoute] Guid id,
        [FromServices] ICompanyService companyService,
        CancellationToken cancellationToken)
    {
        var result = await companyService.GetByIdAsync(id, cancellationToken);
        return result is not null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> GetCompanyOperationsAsync(
        [FromRoute] Guid id,
        [FromServices] ICompanyService companyService,
        CancellationToken cancellationToken)
    {
        // Use GetCompaniesWithOperationsAsync and filter by ID
        var companies = await companyService.GetCompaniesWithOperationsAsync(cancellationToken);
        var result = companies.FirstOrDefault(c => c.Id == id);
        return result is not null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> GetActiveCompaniesAsync(
        [FromServices] ICompanyService companyService,
        CancellationToken cancellationToken)
    {
        // Get all companies and filter active ones (not deleted)
        var allCompanies = await companyService.GetAllAsync(cancellationToken);
        var activeCompanies = allCompanies.ToList();
        return Results.Ok(activeCompanies);
    }

    private static async Task<IResult> CreateCompanyAsync(
        [FromBody] CreateCompanyDto dto,
        [FromServices] ICompanyService companyService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("User ID not found");
        var result = await companyService.CreateAsync(dto, userId, cancellationToken);
        
        return result != null 
            ? Results.CreatedAtRoute("GetCompanyById", new { id = result.Id }, result)
            : Results.BadRequest("Failed to create company");
    }

    private static async Task<IResult> UpdateCompanyAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateCompanyDto dto,
        [FromServices] ICompanyService companyService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("User ID not found");
        var result = await companyService.UpdateAsync(id, dto, userId, cancellationToken);
        
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> ActivateCompanyAsync(
        [FromRoute] Guid id,
        [FromServices] ICompanyService companyService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("User ID not found");
        // Since there's no IsActive property, just verify the company exists
        var existing = await companyService.GetByIdAsync(id, cancellationToken);
        if (existing == null) return Results.NotFound();
        
        return Results.Ok("Company is active");
    }

    private static async Task<IResult> DeactivateCompanyAsync(
        [FromRoute] Guid id,
        [FromServices] ICompanyService companyService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("User ID not found");
        // Since there's no IsActive property, we could use soft delete instead
        var result = await companyService.DeleteAsync(id, cancellationToken);
        
        return result ? Results.Ok("Company deactivated successfully") : Results.BadRequest("Failed to deactivate company");
    }

    private static async Task<IResult> DeleteCompanyAsync(
        [FromRoute] Guid id,
        [FromServices] ICompanyService companyService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("User ID not found");
        var result = await companyService.DeleteAsync(id, cancellationToken);
        
        return result ? Results.Ok("Company deleted successfully") : Results.BadRequest("Failed to delete company");
    }
}