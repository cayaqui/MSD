using Api.Authorization;
using Application.Interfaces.Organization;
using Application.Interfaces.Auth;
using Carter;
using Core.DTOs.Common;
using Core.DTOs.Organization.Contractor;
using Core.DTOs.Organization.Project;
using Core.Enums.Projects;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Organization;

/// <summary>
/// Endpoints para la gestión de contratistas
/// </summary>
public class ContractorModule : CarterModule
{
    public ContractorModule() : base("/api/contractors")
    {
        RequireAuthorization();
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        // Query endpoints
        app.MapGet("/", GetContractorsAsync)
            .WithName("GetContractors")
            .WithSummary("Obtener todos los contratistas con paginación")
            .WithDescription("Devuelve una lista paginada de contratistas")
            .WithTags("Contratistas")
            .Produces<PagedResult<ContractorDto>>();

        app.MapGet("/{id:guid}", GetContractorByIdAsync)
            .WithName("GetContractorById")
            .WithSummary("Obtener contratista por ID")
            .WithDescription("Devuelve un contratista específico por ID")
            .WithTags("Contratistas")
            .Produces<ContractorDto>()
            .Produces(404);

        app.MapGet("/active", GetActiveContractorsAsync)
            .WithName("GetActiveContractors")
            .WithSummary("Obtener contratistas activos")
            .WithDescription("Devuelve todos los contratistas activos")
            .WithTags("Contratistas")
            .Produces<List<ContractorDto>>();

        app.MapGet("/by-type/{type}", GetContractorsByTypeAsync)
            .WithName("GetContractorsByType")
            .WithSummary("Obtener contratistas por tipo")
            .WithDescription("Devuelve todos los contratistas de un tipo específico")
            .WithTags("Contratistas")
            .Produces<List<ContractorDto>>();

        app.MapGet("/by-classification/{classification}", GetContractorsByClassificationAsync)
            .WithName("GetContractorsByClassification")
            .WithSummary("Obtener contratistas por clasificación")
            .WithDescription("Devuelve todos los contratistas de una clasificación específica")
            .WithTags("Contratistas")
            .Produces<List<ContractorDto>>();

        app.MapGet("/{id:guid}/projects", GetContractorProjectsAsync)
            .WithName("GetContractorProjects")
            .WithSummary("Obtener proyectos del contratista")
            .WithDescription("Devuelve todos los proyectos asociados con un contratista")
            .WithTags("Contratistas")
            .Produces<ContractorWithProjectsDto>()
            .Produces(404);

        app.MapGet("/{id:guid}/performance", GetContractorPerformanceAsync)
            .WithName("GetContractorPerformance")
            .WithSummary("Obtener rendimiento del contratista")
            .WithDescription("Devuelve las métricas de rendimiento de un contratista")
            .WithTags("Contratistas")
            .Produces<ContractorPerformanceDto>()
            .Produces(404);

        // Command endpoints
        app.MapPost("/", CreateContractorAsync)
            .WithName("CreateContractor")
            .WithSummary("Crear un nuevo contratista")
            .WithDescription("Crea un nuevo contratista")
            .WithTags("Contratistas")
            .RequireAuthorization("AdminOnly")
            .Produces<Result<Guid>>(201)
            .Produces<Result>(400);

        app.MapPut("/{id:guid}", UpdateContractorAsync)
            .WithName("UpdateContractor")
            .WithSummary("Actualizar contratista")
            .WithDescription("Actualiza un contratista existente")
            .WithTags("Contratistas")
            .RequireAuthorization("AdminOnly")
            .Produces<Result>()
            .Produces<Result>(400)
            .Produces(404);

        app.MapPut("/{id:guid}/qualifications", UpdateContractorQualificationsAsync)
            .WithName("UpdateContractorQualifications")
            .WithSummary("Actualizar calificaciones del contratista")
            .WithDescription("Actualiza las calificaciones de un contratista")
            .WithTags("Contratistas")
            .RequireAuthorization("AdminOnly")
            .Produces<Result>()
            .Produces<Result>(400)
            .Produces(404);

        app.MapPost("/{id:guid}/evaluate", EvaluateContractorAsync)
            .WithName("EvaluateContractor")
            .WithSummary("Evaluar contratista")
            .WithDescription("Añade una evaluación para un contratista")
            .WithTags("Contratistas")
            .RequireAuthorization("ContractorEvaluate")
            .Produces<Result>()
            .Produces<Result>(400)
            .Produces(404);

        app.MapPost("/{id:guid}/activate", ActivateContractorAsync)
            .WithName("ActivateContractor")
            .WithSummary("Activar contratista")
            .WithDescription("Activa un contratista")
            .WithTags("Contratistas")
            .RequireAuthorization("AdminOnly")
            .Produces<Result>()
            .Produces(404);

        app.MapPost("/{id:guid}/deactivate", DeactivateContractorAsync)
            .WithName("DeactivateContractor")
            .WithSummary("Desactivar contratista")
            .WithDescription("Desactiva un contratista")
            .WithTags("Contratistas")
            .RequireAuthorization("AdminOnly")
            .Produces<Result>()
            .Produces(404);

        app.MapPost("/{id:guid}/blacklist", BlacklistContractorAsync)
            .WithName("BlacklistContractor")
            .WithSummary("Incluir contratista en lista negra")
            .WithDescription("Añade un contratista a la lista negra")
            .WithTags("Contratistas")
            .RequireAuthorization("AdminOnly")
            .Produces<Result>()
            .Produces<Result>(400)
            .Produces(404);

        app.MapDelete("/{id:guid}", DeleteContractorAsync)
            .WithName("DeleteContractor")
            .WithSummary("Eliminar contratista")
            .WithDescription("Elimina de forma lógica un contratista")
            .WithTags("Contratistas")
            .RequireAuthorization("AdminOnly")
            .Produces<Result>()
            .Produces<Result>(400)
            .Produces(404);
    }

    private static async Task<IResult> GetContractorsAsync(
        [FromServices] IContractorService contractorService,
        [AsParameters] SimpleQueryParameters parameters,
        CancellationToken cancellationToken)
    {
        var result = await contractorService.GetAllPagedAsync(parameters.PageNumber, parameters.PageSize, parameters.SortBy, !parameters.IsAscending, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetContractorByIdAsync(
        [FromRoute] Guid id,
        [FromServices] IContractorService contractorService,
        CancellationToken cancellationToken)
    {
        var result = await contractorService.GetByIdAsync(id, cancellationToken);
        return result is not null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> GetActiveContractorsAsync(
        [FromServices] IContractorService contractorService,
        CancellationToken cancellationToken)
    {
        var result = await contractorService.GetAllAsync(cancellationToken);
        var activeContractors = result.Where(c => c.IsActive).ToList();
        return Results.Ok(activeContractors);
    }

    private static async Task<IResult> GetContractorsByTypeAsync(
        [FromRoute] string type,
        [FromServices] IContractorService contractorService,
        CancellationToken cancellationToken)
    {
        // Parse the type string to enum
        if (!Enum.TryParse<ContractorType>(type, out var contractorType))
            return Results.BadRequest("Tipo de contratista inválido");
        
        var result = await contractorService.GetByTypeAsync(contractorType, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetContractorsByClassificationAsync(
        [FromRoute] string classification,
        [FromServices] IContractorService contractorService,
        CancellationToken cancellationToken)
    {
        // Get all contractors and filter by classification
        var result = await contractorService.GetAllAsync(cancellationToken);
        var contractorsByClassification = result.Where(c => c.Classification.ToString() == classification).ToList();
        return Results.Ok(contractorsByClassification);
    }

    private static async Task<IResult> GetContractorProjectsAsync(
        [FromRoute] Guid id,
        [FromServices] IContractorService contractorService,
        CancellationToken cancellationToken)
    {
        // This would need to be implemented in the service
        var contractor = await contractorService.GetByIdAsync(id, cancellationToken);
        if (contractor == null) return Results.NotFound();
        
        var result = new ContractorWithProjectsDto
        {
            Id = contractor.Id,
            Code = contractor.Code,
            Name = contractor.Name,
            Projects = new List<ProjectSummaryDto>()
        };
        return Results.Ok(result);
    }

    private static async Task<IResult> GetContractorPerformanceAsync(
        [FromRoute] Guid id,
        [FromServices] IContractorService contractorService,
        CancellationToken cancellationToken)
    {
        // This would need to be implemented in the service
        var contractor = await contractorService.GetByIdAsync(id, cancellationToken);
        if (contractor == null) return Results.NotFound();
        
        var result = new ContractorPerformanceDto
        {
            ContractorId = contractor.Id,
            ContractorName = contractor.Name,
            OverallRating = contractor.PerformanceRating ?? 0
        };
        return Results.Ok(result);
    }

    private static async Task<IResult> CreateContractorAsync(
        [FromBody] CreateContractorDto dto,
        [FromServices] IContractorService contractorService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        var result = await contractorService.CreateAsync(dto, userId, cancellationToken);
        
        return result != null 
            ? Results.CreatedAtRoute("GetContractorById", new { id = result.Id }, result)
            : Results.BadRequest("Error al crear el contratista");
    }

    private static async Task<IResult> UpdateContractorAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateContractorDto dto,
        [FromServices] IContractorService contractorService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        var result = await contractorService.UpdateAsync(id, dto, userId, cancellationToken);
        
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> UpdateContractorQualificationsAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateContractorQualificationsDto dto,
        [FromServices] IContractorService contractorService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        // This would need to be implemented using Update method
        var existing = await contractorService.GetByIdAsync(id, cancellationToken);
        if (existing == null) return Results.NotFound();
        
        var updateDto = new UpdateContractorDto
        {
            Name = existing.Name,
            TaxId = existing.TaxId,
            Type = existing.Type,
            Classification = existing.Classification,
            IsActive = existing.IsActive,
            Status = existing.Status
        };
        
        var result = await contractorService.UpdateAsync(id, updateDto, userId, cancellationToken);
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> EvaluateContractorAsync(
        [FromRoute] Guid id,
        [FromBody] EvaluateContractorDto dto,
        [FromServices] IContractorService contractorService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        // Update performance rating
        var rating = (dto.QualityRating + dto.SafetyRating + dto.ScheduleRating + dto.CostRating + dto.CommunicationRating) / 5;
        var result = await contractorService.UpdatePerformanceRatingAsync(id, rating, userId, cancellationToken);
        
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> ActivateContractorAsync(
        [FromRoute] Guid id,
        [FromServices] IContractorService contractorService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        var result = await contractorService.UpdateStatusAsync(id, ContractorStatus.Active, userId, cancellationToken);
        
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> DeactivateContractorAsync(
        [FromRoute] Guid id,
        [FromServices] IContractorService contractorService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        var result = await contractorService.UpdateStatusAsync(id, ContractorStatus.Inactive, userId, cancellationToken);
        
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> BlacklistContractorAsync(
        [FromRoute] Guid id,
        [FromBody] BlacklistContractorDto dto,
        [FromServices] IContractorService contractorService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        var result = await contractorService.UpdateStatusAsync(id, ContractorStatus.Blacklisted, userId, cancellationToken);
        
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> DeleteContractorAsync(
        [FromRoute] Guid id,
        [FromServices] IContractorService contractorService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        var result = await contractorService.DeleteAsync(id, cancellationToken);
        
        return result ? Results.Ok("Contratista eliminado exitosamente") : Results.BadRequest("Error al eliminar el contratista");
    }
}