using Api.Authorization;
using Application.Interfaces.Auth;
using Application.Interfaces.Progress;
using Carter;
using Core.Constants;
using Core.DTOs.Common;
using Core.DTOs.Progress.Schedules;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Progress;

/// <summary>
/// Endpoints para gestión de Cronogramas y Versiones de Cronograma
/// </summary>
public class ScheduleModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/schedules")
            .WithTags("Cronogramas")
            .RequireAuthorization();

        // Query endpoints
        group.MapGet("/", GetScheduleVersions)
            .WithName("GetScheduleVersions")
            .WithSummary("Obtener versiones de cronograma")
            .WithDescription("Retorna una lista paginada de versiones de cronograma con filtros")
            .Produces<PagedResult<ScheduleVersionDto>>()
            .RequireAuthorization();

        group.MapGet("/{id:guid}", GetScheduleVersionById)
            .WithName("GetScheduleVersionById")
            .WithSummary("Obtener versión de cronograma por ID")
            .WithDescription("Retorna el detalle completo de una versión de cronograma")
            .Produces<ScheduleVersionDto>()
            .ProducesProblem(404);

        group.MapGet("/project/{projectId:guid}/current", GetCurrentSchedule)
            .WithName("GetCurrentSchedule")
            .WithSummary("Obtener cronograma actual del proyecto")
            .WithDescription("Retorna la versión de cronograma actualmente activa del proyecto")
            .Produces<ScheduleVersionDto>()
            .ProducesProblem(404);

        group.MapGet("/project/{projectId:guid}/baseline", GetBaselineSchedule)
            .WithName("GetBaselineSchedule")
            .WithSummary("Obtener línea base del proyecto")
            .WithDescription("Retorna la versión de cronograma establecida como línea base")
            .Produces<ScheduleVersionDto>()
            .ProducesProblem(404);

        // Command endpoints
        group.MapPost("/", CreateScheduleVersion)
            .WithName("CreateScheduleVersion")
            .WithSummary("Crear nueva versión de cronograma")
            .WithDescription("Crea una nueva versión de cronograma para un proyecto")
            .Produces<Guid>(201)
            .ProducesValidationProblem()
            .RequireAuthorization();

        group.MapPut("/{id:guid}", UpdateScheduleVersion)
            .WithName("UpdateScheduleVersion")
            .WithSummary("Actualizar versión de cronograma")
            .WithDescription("Actualiza la información de una versión de cronograma en borrador")
            .Produces(200)
            .ProducesValidationProblem()
            .ProducesProblem(404)
            .RequireAuthorization();

        group.MapDelete("/{id:guid}", DeleteScheduleVersion)
            .WithName("DeleteScheduleVersion")
            .WithSummary("Eliminar versión de cronograma")
            .WithDescription("Elimina una versión de cronograma en borrador")
            .Produces(204)
            .ProducesProblem(404)
            .ProducesProblem(400)
            .RequireAuthorization();

        // Schedule operations
        group.MapPost("/{id:guid}/submit", SubmitForApproval)
            .WithName("SubmitScheduleForApproval")
            .WithSummary("Enviar cronograma para aprobación")
            .WithDescription("Envía una versión de cronograma para su revisión y aprobación")
            .Produces(200)
            .ProducesProblem(404)
            .ProducesProblem(400)
            .RequireAuthorization();

        group.MapPost("/{id:guid}/approve", ApproveSchedule)
            .WithName("ApproveSchedule")
            .WithSummary("Aprobar cronograma")
            .WithDescription("Aprueba una versión de cronograma en revisión")
            .Produces(200)
            .ProducesValidationProblem()
            .ProducesProblem(404)
            .ProducesProblem(400)
            .RequireAuthorization();

        group.MapPost("/{id:guid}/baseline", SetAsBaseline)
            .WithName("SetScheduleAsBaseline")
            .WithSummary("Establecer como línea base")
            .WithDescription("Establece una versión aprobada como línea base del proyecto")
            .Produces(200)
            .ProducesProblem(404)
            .ProducesProblem(400)
            .RequireAuthorization();

        // Comparison
        group.MapGet("/compare", CompareSchedules)
            .WithName("CompareSchedules")
            .WithSummary("Comparar cronogramas")
            .WithDescription("Compara dos versiones de cronograma y retorna las variaciones")
            .Produces<ScheduleComparisonDto>()
            .ProducesProblem(404);

        group.MapGet("/project/{projectId:guid}/variances", GetScheduleVariances)
            .WithName("GetScheduleVariances")
            .WithSummary("Obtener variaciones del cronograma")
            .WithDescription("Retorna las variaciones entre el cronograma actual y la línea base")
            .Produces<List<ScheduleVarianceDto>>()
            .ProducesProblem(404);

        // Import/Export
        group.MapPost("/import", ImportSchedule)
            .WithName("ImportSchedule")
            .WithSummary("Importar cronograma")
            .WithDescription("Importa un cronograma desde MS Project o Primavera")
            .Produces<Guid>(201)
            .ProducesValidationProblem()
            .RequireAuthorization()
            .DisableAntiforgery();

        group.MapGet("/{id:guid}/export", ExportSchedule)
            .WithName("ExportSchedule")
            .WithSummary("Exportar cronograma")
            .WithDescription("Exporta un cronograma a MS Project o Primavera")
            .Produces(200)
            .ProducesProblem(404)
            .RequireAuthorization();

        group.MapGet("/template", DownloadScheduleTemplate)
            .WithName("DownloadScheduleTemplate")
            .WithSummary("Descargar plantilla de cronograma")
            .WithDescription("Descarga una plantilla vacía para importar cronogramas")
            .Produces(200);

        // Validation
        group.MapGet("/project/{projectId:guid}/can-create-version", CanCreateNewVersion)
            .WithName("CanCreateNewScheduleVersion")
            .WithSummary("Verificar si se puede crear nueva versión")
            .WithDescription("Valida si es posible crear una nueva versión de cronograma")
            .Produces<bool>();

        group.MapGet("/{id:guid}/can-baseline", CanSetAsBaseline)
            .WithName("CanSetScheduleAsBaseline")
            .WithSummary("Verificar si se puede establecer como línea base")
            .WithDescription("Valida si una versión puede ser establecida como línea base")
            .Produces<bool>();
    }

    // Query handlers
    private static async Task<IResult> GetScheduleVersions(
        [AsParameters] ScheduleFilterDto filter,
        IScheduleService scheduleService,
        CancellationToken cancellationToken)
    {
        var result = await scheduleService.GetScheduleVersionsAsync(filter, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetScheduleVersionById(
        [FromRoute] Guid id,
        IScheduleService scheduleService,
        CancellationToken cancellationToken)
    {
        var result = await scheduleService.GetScheduleVersionByIdAsync(id, cancellationToken);
        return result != null ? Results.Ok(result) : Results.NotFound($"Versión de cronograma {id} no encontrada");
    }

    private static async Task<IResult> GetCurrentSchedule(
        [FromRoute] Guid projectId,
        IScheduleService scheduleService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        if (!await currentUserService.HasProjectAccessAsync(projectId))
        {
            return Results.Forbid();
        }

        var result = await scheduleService.GetCurrentScheduleAsync(projectId, cancellationToken);
        return result != null ? Results.Ok(result) : Results.NotFound($"No se encontró cronograma actual para el proyecto");
    }

    private static async Task<IResult> GetBaselineSchedule(
        [FromRoute] Guid projectId,
        IScheduleService scheduleService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        if (!await currentUserService.HasProjectAccessAsync(projectId))
        {
            return Results.Forbid();
        }

        var result = await scheduleService.GetBaselineScheduleAsync(projectId, cancellationToken);
        return result != null ? Results.Ok(result) : Results.NotFound($"No se encontró línea base para el proyecto");
    }

    // Command handlers
    private static async Task<IResult> CreateScheduleVersion(
        [FromBody] CreateScheduleVersionDto dto,
        IScheduleService scheduleService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");

        if (!await currentUserService.HasProjectAccessAsync(dto.ProjectId, SimplifiedRoles.Project.ProjectManager))
        {
            return Results.Forbid();
        }

        var result = await scheduleService.CreateScheduleVersionAsync(dto, userId, cancellationToken);
        
        return result.IsSuccess 
            ? Results.Created($"/api/schedules/{result.Value}", result.Value)
            : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> UpdateScheduleVersion(
        [FromRoute] Guid id,
        [FromBody] UpdateScheduleVersionDto dto,
        IScheduleService scheduleService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        
        var result = await scheduleService.UpdateScheduleVersionAsync(id, dto, userId, cancellationToken);
        
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> DeleteScheduleVersion(
        [FromRoute] Guid id,
        IScheduleService scheduleService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        
        var result = await scheduleService.DeleteScheduleVersionAsync(id, userId, cancellationToken);
        
        return result.IsSuccess ? Results.NoContent() : Results.BadRequest(result.Error);
    }

    // Schedule operation handlers
    private static async Task<IResult> SubmitForApproval(
        [FromRoute] Guid id,
        IScheduleService scheduleService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        
        var result = await scheduleService.SubmitForApprovalAsync(id, userId, cancellationToken);
        
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> ApproveSchedule(
        [FromRoute] Guid id,
        [FromBody] ApproveScheduleDto dto,
        IScheduleService scheduleService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        
        // Solo Project Manager puede aprobar
        var schedule = await scheduleService.GetScheduleVersionByIdAsync(id, cancellationToken);
        if (schedule == null)
            return Results.NotFound($"Versión de cronograma {id} no encontrada");
            
        if (!await currentUserService.HasProjectAccessAsync(schedule.ProjectId, SimplifiedRoles.Project.ProjectManager))
        {
            return Results.Forbid();
        }
        
        var result = await scheduleService.ApproveScheduleAsync(id, dto, userId, cancellationToken);
        
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> SetAsBaseline(
        [FromRoute] Guid id,
        IScheduleService scheduleService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        
        // Solo Project Manager puede establecer línea base
        var schedule = await scheduleService.GetScheduleVersionByIdAsync(id, cancellationToken);
        if (schedule == null)
            return Results.NotFound($"Versión de cronograma {id} no encontrada");
            
        if (!await currentUserService.HasProjectAccessAsync(schedule.ProjectId, SimplifiedRoles.Project.ProjectManager))
        {
            return Results.Forbid();
        }
        
        var result = await scheduleService.SetAsBaselineAsync(id, userId, cancellationToken);
        
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }

    // Comparison handlers
    private static async Task<IResult> CompareSchedules(
        [FromQuery] Guid baselineId,
        [FromQuery] Guid currentId,
        IScheduleService scheduleService,
        CancellationToken cancellationToken)
    {
        var result = await scheduleService.CompareSchedulesAsync(baselineId, currentId, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetScheduleVariances(
        [FromRoute] Guid projectId,
        IScheduleService scheduleService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        if (!await currentUserService.HasProjectAccessAsync(projectId))
        {
            return Results.Forbid();
        }

        var result = await scheduleService.GetScheduleVariancesAsync(projectId, cancellationToken);
        return Results.Ok(result);
    }

    // Import/Export handlers
    private static async Task<IResult> ImportSchedule(
        IFormFile file,
        [FromForm] string projectId,
        [FromForm] string version,
        [FromForm] string name,
        [FromForm] string sourceSystem,
        IScheduleService scheduleService,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("ID de usuario no encontrado");
        
        if (!Guid.TryParse(projectId, out var projectGuid))
            return Results.BadRequest("ID de proyecto inválido");

        if (!await currentUserService.HasProjectAccessAsync(projectGuid, SimplifiedRoles.Project.ProjectManager))
        {
            return Results.Forbid();
        }

        using var stream = new MemoryStream();
        await file.CopyToAsync(stream, cancellationToken);
        
        var dto = new ImportScheduleDto
        {
            ProjectId = projectGuid,
            Version = version,
            Name = name,
            SourceSystem = sourceSystem,
            FileContent = stream.ToArray(),
            FileName = file.FileName
        };

        var result = await scheduleService.ImportScheduleAsync(dto, userId, cancellationToken);
        
        return result.IsSuccess 
            ? Results.Created($"/api/schedules/{result.Value}", result.Value)
            : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> ExportSchedule(
        [FromRoute] Guid id,
        [FromQuery] string format = "MSProject",
        IScheduleService scheduleService,
        CancellationToken cancellationToken)
    {
        var bytes = await scheduleService.ExportScheduleAsync(id, format, cancellationToken);
        var extension = format.ToLower() switch
        {
            "msproject" => "mpp",
            "primavera" => "xer",
            "excel" => "xlsx",
            _ => "xml"
        };
        
        return Results.File(bytes, "application/octet-stream", $"Schedule_{id}_{DateTime.Now:yyyyMMdd}.{extension}");
    }

    private static async Task<IResult> DownloadScheduleTemplate(
        [FromQuery] string format = "MSProject",
        IScheduleService scheduleService,
        CancellationToken cancellationToken)
    {
        var bytes = await scheduleService.ExportScheduleTemplateAsync(format, cancellationToken);
        var extension = format.ToLower() switch
        {
            "msproject" => "mpp",
            "primavera" => "xer",
            "excel" => "xlsx",
            _ => "xml"
        };
        
        return Results.File(bytes, "application/octet-stream", $"Schedule_Template.{extension}");
    }

    // Validation handlers
    private static async Task<IResult> CanCreateNewVersion(
        [FromRoute] Guid projectId,
        IScheduleService scheduleService,
        CancellationToken cancellationToken)
    {
        var canCreate = await scheduleService.CanCreateNewVersionAsync(projectId, cancellationToken);
        return Results.Ok(canCreate);
    }

    private static async Task<IResult> CanSetAsBaseline(
        [FromRoute] Guid id,
        IScheduleService scheduleService,
        CancellationToken cancellationToken)
    {
        var canBaseline = await scheduleService.CanSetAsBaselineAsync(id, cancellationToken);
        return Results.Ok(canBaseline);
    }
}