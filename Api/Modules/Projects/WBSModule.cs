using Api.Authorization;
using Application.Interfaces.Auth;
using Application.Interfaces.Projects;
using Carter;
using Core.Constants;
using Core.DTOs.Common;
using Core.DTOs.Projects.WBSElement;
using Core.DTOs.Projects.WorkPackageDetails;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Projects;

/// <summary>
/// Endpoints para gestión de la Estructura de Desglose del Trabajo (WBS)
/// </summary>
public class WBSModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/wbs")
            .WithTags("Estructura de Desglose del Trabajo (WBS)")
            .RequireAuthorization();

        // Query endpoints
        group.MapGet("/project/{projectId:guid}", GetWBSElements)
            .WithName("GetWBSElements")
            .WithSummary("Obtener elementos WBS de un proyecto")
            .WithDescription("Retorna una lista paginada de elementos WBS para un proyecto específico")
            .Produces<PagedResult<WBSElementDto>>()
            .RequireAuthorization();

        group.MapGet("/{id:guid}", GetWBSElementById)
            .WithName("GetWBSElementById")
            .WithSummary("Obtener elemento WBS por ID")
            .WithDescription("Retorna el detalle completo de un elemento WBS incluyendo hijos y mapeos CBS")
            .Produces<WBSElementDetailDto>()
            .ProducesProblem(404);

        group.MapGet("/project/{projectId:guid}/hierarchy", GetWBSHierarchy)
            .WithName("GetWBSHierarchy")
            .WithSummary("Obtener jerarquía WBS completa")
            .WithDescription("Retorna la estructura jerárquica completa del WBS del proyecto")
            .Produces<List<WBSElementDto>>()
            .RequireAuthorization();

        group.MapGet("/{parentId:guid}/children", GetWBSChildren)
            .WithName("GetWBSChildren")
            .WithSummary("Obtener elementos hijos")
            .WithDescription("Retorna todos los elementos hijos de un elemento WBS específico")
            .Produces<List<WBSElementDto>>();

        group.MapGet("/control-account/{controlAccountId:guid}/work-packages", GetWorkPackagesByControlAccount)
            .WithName("GetWorkPackagesByControlAccount")
            .WithSummary("Obtener paquetes de trabajo por cuenta de control")
            .WithDescription("Retorna todos los paquetes de trabajo asignados a una cuenta de control")
            .Produces<List<WBSElementDto>>();

        group.MapGet("/{id:guid}/dictionary", GetWBSDictionary)
            .WithName("GetWBSDictionary")
            .WithSummary("Obtener diccionario WBS")
            .WithDescription("Retorna la información del diccionario WBS para un elemento")
            .Produces<WBSDictionaryDto>()
            .ProducesProblem(404);

        // Command endpoints
        group.MapPost("/", CreateWBSElement)
            .WithName("CreateWBSElement")
            .WithSummary("Crear elemento WBS")
            .WithDescription("Crea un nuevo elemento en la estructura WBS")
            .Produces<Guid>(201)
            .ProducesValidationProblem()
            .RequireAuthorization();

        group.MapPut("/{id:guid}", UpdateWBSElement)
            .WithName("UpdateWBSElement")
            .WithSummary("Actualizar elemento WBS")
            .WithDescription("Actualiza la información básica de un elemento WBS")
            .Produces(200)
            .ProducesValidationProblem()
            .ProducesProblem(404)
            .RequireAuthorization();

        group.MapPut("/{id:guid}/dictionary", UpdateWBSDictionary)
            .WithName("UpdateWBSDictionary")
            .WithSummary("Actualizar diccionario WBS")
            .WithDescription("Actualiza la información del diccionario WBS de un elemento")
            .Produces(200)
            .ProducesValidationProblem()
            .ProducesProblem(404)
            .RequireAuthorization();

        group.MapPost("/{id:guid}/convert-to-work-package", ConvertToWorkPackage)
            .WithName("ConvertToWorkPackage")
            .WithSummary("Convertir a paquete de trabajo")
            .WithDescription("Convierte un elemento WBS a paquete de trabajo")
            .Produces(200)
            .ProducesValidationProblem()
            .ProducesProblem(404)
            .RequireAuthorization();

        group.MapPost("/{id:guid}/convert-to-planning-package", ConvertToPlanningPackage)
            .WithName("ConvertToPlanningPackage")
            .WithSummary("Convertir a paquete de planificación")
            .WithDescription("Convierte un elemento WBS a paquete de planificación")
            .Produces(200)
            .ProducesValidationProblem()
            .ProducesProblem(404)
            .RequireAuthorization();

        group.MapPost("/planning-package/{id:guid}/convert-to-work-package", ConvertPlanningPackageToWorkPackage)
            .WithName("ConvertPlanningPackageToWorkPackage")
            .WithSummary("Convertir paquete de planificación a paquete de trabajo")
            .WithDescription("Convierte un paquete de planificación existente a paquete de trabajo")
            .Produces(200)
            .ProducesValidationProblem()
            .ProducesProblem(404)
            .RequireAuthorization();

        group.MapPost("/reorder", ReorderWBSElements)
            .WithName("ReorderWBSElements")
            .WithSummary("Reordenar elementos WBS")
            .WithDescription("Cambia el orden de visualización de los elementos WBS")
            .Produces(200)
            .ProducesValidationProblem()
            .RequireAuthorization();

        group.MapDelete("/{id:guid}", DeleteWBSElement)
            .WithName("DeleteWBSElement")
            .WithSummary("Eliminar elemento WBS")
            .WithDescription("Elimina un elemento WBS si no tiene hijos ni datos asociados")
            .Produces(204)
            .ProducesProblem(404)
            .ProducesProblem(400)
            .RequireAuthorization();

        // Validation endpoints
        group.MapGet("/{id:guid}/can-convert-to-work-package", CanConvertToWorkPackage)
            .WithName("CanConvertToWorkPackage")
            .WithSummary("Verificar si se puede convertir a paquete de trabajo")
            .WithDescription("Valida si un elemento WBS puede ser convertido a paquete de trabajo")
            .Produces<bool>();

        group.MapGet("/{id:guid}/can-delete", CanDeleteWBSElement)
            .WithName("CanDeleteWBSElement")
            .WithSummary("Verificar si se puede eliminar")
            .WithDescription("Valida si un elemento WBS puede ser eliminado")
            .Produces<bool>();

        group.MapPost("/validate-code", ValidateWBSCode)
            .WithName("ValidateWBSCode")
            .WithSummary("Validar código WBS")
            .WithDescription("Valida si un código WBS es único en el proyecto")
            .Produces<ValidationResult>();

        // Bulk operations
        group.MapPost("/bulk-create", BulkCreateWBSElements)
            .WithName("BulkCreateWBSElements")
            .WithSummary("Crear múltiples elementos WBS")
            .WithDescription("Crea múltiples elementos WBS en una sola operación")
            .Produces<BulkOperationResult>(200)
            .ProducesValidationProblem()
            .RequireAuthorization();

        // Import/Export
        group.MapPost("/import", ImportWBS)
            .WithName("ImportWBS")
            .WithSummary("Importar estructura WBS")
            .WithDescription("Importa una estructura WBS desde un archivo Excel o CSV")
            .Produces<ImportResult>(200)
            .ProducesValidationProblem()
            .RequireAuthorization();

        group.MapGet("/project/{projectId:guid}/export", ExportWBS)
            .WithName("ExportWBS")
            .WithSummary("Exportar estructura WBS")
            .WithDescription("Exporta la estructura WBS del proyecto a Excel")
            .Produces(200)
            .RequireAuthorization();
    }

    // Query handlers
    private static async Task<IResult> GetWBSElements(
        [FromRoute] Guid projectId,
        [AsParameters] QueryParameters parameters,
        IWBSService wbsService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        if (!await currentUserService.HasProjectAccessAsync(projectId))
        {
            return Results.Forbid();
        }

        var result = await wbsService.GetWBSElementsAsync(projectId, parameters, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetWBSElementById(
        [FromRoute] Guid id,
        IWBSService wbsService,
        CancellationToken cancellationToken)
    {
        var result = await wbsService.GetWBSElementByIdAsync(id, cancellationToken);
        return result != null ? Results.Ok(result) : Results.NotFound($"Elemento WBS {id} no encontrado");
    }

    private static async Task<IResult> GetWBSHierarchy(
        [FromRoute] Guid projectId,
        IWBSService wbsService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        if (!await currentUserService.HasProjectAccessAsync(projectId))
        {
            return Results.Forbid();
        }

        var result = await wbsService.GetWBSHierarchyAsync(projectId, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetWBSChildren(
        [FromRoute] Guid parentId,
        IWBSService wbsService,
        CancellationToken cancellationToken)
    {
        var result = await wbsService.GetChildrenAsync(parentId, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetWorkPackagesByControlAccount(
        [FromRoute] Guid controlAccountId,
        IWBSService wbsService,
        CancellationToken cancellationToken)
    {
        var result = await wbsService.GetWorkPackagesByControlAccountAsync(controlAccountId, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetWBSDictionary(
        [FromRoute] Guid id,
        IWBSService wbsService,
        CancellationToken cancellationToken)
    {
        var result = await wbsService.GetWBSDictionaryAsync(id, cancellationToken);
        return result != null ? Results.Ok(result) : Results.NotFound($"Diccionario WBS para elemento {id} no encontrado");
    }

    // Command handlers
    private static async Task<IResult> CreateWBSElement(
        [FromBody] CreateWBSElementDto dto,
        IWBSService wbsService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");

        if (!await currentUserService.HasProjectAccessAsync(dto.ProjectId, SimplifiedRoles.Project.ProjectManager))
        {
            return Results.Forbid();
        }

        var result = await wbsService.CreateWBSElementAsync(dto, userId, cancellationToken);
        
        return result.IsSuccess 
            ? Results.Created($"/api/wbs/{result.Value}", result.Value)
            : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> UpdateWBSElement(
        [FromRoute] Guid id,
        [FromBody] UpdateWBSElementDto dto,
        IWBSService wbsService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        
        // Verificar que el usuario tenga al menos rol de TeamLead para editar
        var wbsElement = await wbsService.GetWBSElementByIdAsync(id, cancellationToken);
        if (wbsElement == null)
            return Results.NotFound($"Elemento WBS {id} no encontrado");
            
        if (!await currentUserService.HasProjectAccessAsync(wbsElement.ProjectId, SimplifiedRoles.Project.TeamLead))
        {
            return Results.Forbid();
        }
        
        var result = await wbsService.UpdateWBSElementAsync(id, dto, userId, cancellationToken);
        
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> UpdateWBSDictionary(
        [FromRoute] Guid id,
        [FromBody] UpdateWBSDictionaryDto dto,
        IWBSService wbsService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        
        var result = await wbsService.UpdateWBSDictionaryAsync(id, dto, userId, cancellationToken);
        
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> ConvertToWorkPackage(
        [FromRoute] Guid id,
        [FromBody] ConvertToWorkPackageDto dto,
        IWBSService wbsService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        
        var result = await wbsService.ConvertToWorkPackageAsync(id, dto, userId, cancellationToken);
        
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> ConvertToPlanningPackage(
        [FromRoute] Guid id,
        [FromBody] ConvertToPlanningPackageRequest request,
        IWBSService wbsService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        
        var result = await wbsService.ConvertToPlanningPackageAsync(id, request.ControlAccountId, userId, cancellationToken);
        
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> ConvertPlanningPackageToWorkPackage(
        [FromRoute] Guid id,
        [FromBody] ConvertPlanningToWorkPackageDto dto,
        IWBSService wbsService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        
        var result = await wbsService.ConvertPlanningPackageToWorkPackageAsync(id, dto, userId, cancellationToken);
        
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> ReorderWBSElements(
        [FromBody] ReorderWBSElementsDto dto,
        IWBSService wbsService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        
        var result = await wbsService.ReorderWBSElementsAsync(dto, userId, cancellationToken);
        
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> DeleteWBSElement(
        [FromRoute] Guid id,
        IWBSService wbsService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        
        // Solo ProjectManager puede eliminar elementos WBS
        var wbsElement = await wbsService.GetWBSElementByIdAsync(id, cancellationToken);
        if (wbsElement == null)
            return Results.NotFound($"Elemento WBS {id} no encontrado");
            
        if (!await currentUserService.HasProjectAccessAsync(wbsElement.ProjectId, SimplifiedRoles.Project.ProjectManager))
        {
            return Results.Forbid();
        }
        
        var result = await wbsService.DeleteWBSElementAsync(id, userId, cancellationToken);
        
        return result.IsSuccess ? Results.NoContent() : Results.BadRequest(result.Error);
    }

    // Validation handlers
    private static async Task<IResult> CanConvertToWorkPackage(
        [FromRoute] Guid id,
        IWBSService wbsService,
        CancellationToken cancellationToken)
    {
        var canConvert = await wbsService.CanConvertToWorkPackageAsync(id, cancellationToken);
        return Results.Ok(canConvert);
    }

    private static async Task<IResult> CanDeleteWBSElement(
        [FromRoute] Guid id,
        IWBSService wbsService,
        CancellationToken cancellationToken)
    {
        var canDelete = await wbsService.CanDeleteWBSElementAsync(id, cancellationToken);
        return Results.Ok(canDelete);
    }

    private static async Task<IResult> ValidateWBSCode(
        [FromBody] ValidateWBSCodeRequest request,
        IWBSService wbsService,
        CancellationToken cancellationToken)
    {
        var result = await wbsService.ValidateWBSCodeAsync(
            request.Code, 
            request.ProjectId, 
            request.ExcludeId, 
            cancellationToken);
            
        return Results.Ok(result);
    }

    // Bulk operations handlers
    private static async Task<IResult> BulkCreateWBSElements(
        [FromBody] BulkCreateWBSElementsDto dto,
        IWBSService wbsService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");

        // Check project access for the first element
        if (dto.Elements.Any() && !await currentUserService.HasProjectAccessAsync(dto.Elements.First().ProjectId, SimplifiedRoles.Project.ProjectManager))
        {
            return Results.Forbid();
        }

        var result = await wbsService.BulkCreateWBSElementsAsync(dto, userId, cancellationToken);
        return Results.Ok(result);
    }

    // Import/Export handlers
    private static async Task<IResult> ImportWBS(
        IFormFile file,
        [FromForm] Guid projectId,
        IWBSService wbsService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");

        if (!await currentUserService.HasProjectAccessAsync(projectId, SimplifiedRoles.Project.ProjectManager))
        {
            return Results.Forbid();
        }

        using var stream = file.OpenReadStream();
        var result = await wbsService.ImportWBSAsync(stream, file.FileName, projectId, userId, cancellationToken);
        
        return result.FailedRecords > 0 
            ? Results.BadRequest(result) 
            : Results.Ok(result);
    }

    private static async Task<IResult> ExportWBS(
        [FromRoute] Guid projectId,
        IWBSService wbsService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        if (!await currentUserService.HasProjectAccessAsync(projectId))
        {
            return Results.Forbid();
        }

        var bytes = await wbsService.ExportWBSAsync(projectId, cancellationToken);
        return Results.File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"WBS_{projectId}_{DateTime.Now:yyyyMMdd}.xlsx");
    }
}

// Request DTOs
public record ConvertToPlanningPackageRequest(Guid ControlAccountId);
public record ValidateWBSCodeRequest(string Code, Guid ProjectId, Guid? ExcludeId = null);