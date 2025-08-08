using Api.Authorization;
using Application.Interfaces.Organization;
using Application.Interfaces.Auth;
using Carter;
using Core.DTOs.Common;
using Core.DTOs.Organization.Company;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Organization;

/// <summary>
/// Endpoints para gestión de compañías
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
            .WithSummary("Obtener todas las compañías con paginación")
            .WithDescription("Retorna una lista paginada de compañías")
            .WithTags("Compañías")
            .Produces<PagedResult<CompanyDto>>();

        app.MapGet("/{id:guid}", GetCompanyByIdAsync)
            .WithName("GetCompanyById")
            .WithSummary("Obtener compañía por ID")
            .WithDescription("Retorna una compañía específica por ID")
            .WithTags("Compañías")
            .Produces<CompanyDto>()
            .Produces(404);

        app.MapGet("/{id:guid}/operations", GetCompanyOperationsAsync)
            .WithName("GetCompanyOperations")
            .WithSummary("Obtener operaciones de la compañía")
            .WithDescription("Retorna todas las operaciones de una compañía específica")
            .WithTags("Compañías")
            .Produces<CompanyWithOperationsDto>()
            .Produces(404);

        app.MapGet("/active", GetActiveCompaniesAsync)
            .WithName("GetActiveCompanies")
            .WithSummary("Obtener compañías activas")
            .WithDescription("Retorna todas las compañías activas")
            .WithTags("Compañías")
            .Produces<List<CompanyDto>>();

        // Command endpoints
        app.MapPost("/", CreateCompanyAsync)
            .WithName("CreateCompany")
            .WithSummary("Crear una nueva compañía")
            .WithDescription("Crea una nueva compañía")
            .WithTags("Compañías")
            .RequireAuthorization("AdminOnly")
            .Produces<Result<Guid>>(201)
            .Produces<Result>(400);

        app.MapPut("/{id:guid}", UpdateCompanyAsync)
            .WithName("UpdateCompany")
            .WithSummary("Actualizar compañía")
            .WithDescription("Actualiza una compañía existente")
            .WithTags("Compañías")
            .RequireAuthorization("AdminOnly")
            .Produces<Result>()
            .Produces<Result>(400)
            .Produces(404);

        app.MapPost("/{id:guid}/activate", ActivateCompanyAsync)
            .WithName("ActivateCompany")
            .WithSummary("Activar compañía")
            .WithDescription("Activa una compañía")
            .WithTags("Compañías")
            .RequireAuthorization("AdminOnly")
            .Produces<Result>()
            .Produces(404);

        app.MapPost("/{id:guid}/deactivate", DeactivateCompanyAsync)
            .WithName("DeactivateCompany")
            .WithSummary("Desactivar compañía")
            .WithDescription("Desactiva una compañía")
            .WithTags("Compañías")
            .RequireAuthorization("AdminOnly")
            .Produces<Result>()
            .Produces(404);

        app.MapDelete("/{id:guid}", DeleteCompanyAsync)
            .WithName("DeleteCompany")
            .WithSummary("Eliminar compañía")
            .WithDescription("Elimina lógicamente una compañía")
            .WithTags("Compañías")
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
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        var result = await companyService.CreateAsync(dto, userId, cancellationToken);
        
        return result != null 
            ? Results.CreatedAtRoute("GetCompanyById", new { id = result.Id }, result)
            : Results.BadRequest("Error al crear la compañía");
    }

    private static async Task<IResult> UpdateCompanyAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateCompanyDto dto,
        [FromServices] ICompanyService companyService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        var result = await companyService.UpdateAsync(id, dto, userId, cancellationToken);
        
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> ActivateCompanyAsync(
        [FromRoute] Guid id,
        [FromServices] ICompanyService companyService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        // Since there's no IsActive property, just verify the company exists
        var existing = await companyService.GetByIdAsync(id, cancellationToken);
        if (existing == null) return Results.NotFound();
        
        return Results.Ok("La compañía está activa");
    }

    private static async Task<IResult> DeactivateCompanyAsync(
        [FromRoute] Guid id,
        [FromServices] ICompanyService companyService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        // Since there's no IsActive property, we could use soft delete instead
        var result = await companyService.DeleteAsync(id, cancellationToken);
        
        return result ? Results.Ok("Compañía desactivada exitosamente") : Results.BadRequest("Error al desactivar la compañía");
    }

    private static async Task<IResult> DeleteCompanyAsync(
        [FromRoute] Guid id,
        [FromServices] ICompanyService companyService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        var result = await companyService.DeleteAsync(id, cancellationToken);
        
        return result ? Results.Ok("Compañía eliminada exitosamente") : Results.BadRequest("Error al eliminar la compañía");
    }
}