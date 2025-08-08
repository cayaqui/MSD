using Application.Interfaces.Configuration;
using Carter;
using Core.DTOs.Common;
using Core.DTOs.Configuration.WBSTemplates;
using Microsoft.AspNetCore.Authorization;

namespace Api.Modules.Configuration;

[Authorize]
public class WBSTemplateModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/configuration/wbs-templates")
            .WithTags("WBS Templates")
            .WithOpenApi();

        // Get all templates with filtering
        group.MapGet("/", async (
            IWBSTemplateService service,
            int pageNumber = 1,
            int pageSize = 20,
            string? searchTerm = null,
            string? industryType = null,
            string? projectType = null) =>
        {
            var filter = new WBSCBSTemplateFilterDto
            {
                Type = "WBS",
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchTerm = searchTerm,
                IndustryType = industryType,
                ProjectType = projectType,
                IsActive = true
            };

            var templates = await service.SearchAsync(filter);
            var totalCount = templates.Count(); // This would need proper pagination implementation

            return Results.Ok(new PagedResult<WBSTemplateDto>
            {
                Items = templates.ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            });
        })
        .WithName("GetWBSTemplates")
        .WithSummary("Get all WBS templates with pagination and filtering");

        // Get template by ID
        group.MapGet("/{id:guid}", async (Guid id, IWBSTemplateService service) =>
        {
            var template = await service.GetByIdAsync(id);
            return template != null ? Results.Ok(template) : Results.NotFound();
        })
        .WithName("GetWBSTemplateById")
        .WithSummary("Get a specific WBS template by ID");

        // Get template by code
        group.MapGet("/by-code/{code}", async (string code, IWBSTemplateService service) =>
        {
            var template = await service.GetByCodeAsync(code);
            return template != null ? Results.Ok(template) : Results.NotFound();
        })
        .WithName("GetWBSTemplateByCode")
        .WithSummary("Get a specific WBS template by code");

        // Get active templates
        group.MapGet("/active", async (IWBSTemplateService service) =>
        {
            var templates = await service.GetActiveAsync();
            return Results.Ok(templates);
        })
        .WithName("GetActiveWBSTemplates")
        .WithSummary("Get all active WBS templates");

        // Get templates by industry type
        group.MapGet("/by-industry/{industryType}", async (string industryType, IWBSTemplateService service) =>
        {
            var templates = await service.GetByIndustryTypeAsync(industryType);
            return Results.Ok(templates);
        })
        .WithName("GetWBSTemplatesByIndustry")
        .WithSummary("Get WBS templates by industry type");

        // Get templates by project type
        group.MapGet("/by-project-type/{projectType}", async (string projectType, IWBSTemplateService service) =>
        {
            var templates = await service.GetByProjectTypeAsync(projectType);
            return Results.Ok(templates);
        })
        .WithName("GetWBSTemplatesByProjectType")
        .WithSummary("Get WBS templates by project type");

        // Create template
        group.MapPost("/", async (CreateWBSTemplateDto dto, IWBSTemplateService service) =>
        {
            var template = await service.CreateAsync(dto);
            return template != null ? Results.Ok(template) : Results.BadRequest("Failed to create template");
        })
        .WithName("CreateWBSTemplate")
        .WithSummary("Create a new WBS template");

        // Update template
        group.MapPut("/{id:guid}", async (Guid id, UpdateWBSTemplateDto dto, IWBSTemplateService service) =>
        {
            var template = await service.UpdateAsync(id, dto);
            return template != null ? Results.Ok(template) : Results.NotFound();
        })
        .WithName("UpdateWBSTemplate")
        .WithSummary("Update an existing WBS template");

        // Delete template
        group.MapDelete("/{id:guid}", async (Guid id, IWBSTemplateService service) =>
        {
            var result = await service.DeleteAsync(id);
            return result ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteWBSTemplate")
        .WithSummary("Delete a WBS template");

        // Activate template
        group.MapPost("/{id:guid}/activate", async (Guid id, IWBSTemplateService service) =>
        {
            var result = await service.ActivateAsync(id);
            return result ? Results.Ok() : Results.NotFound();
        })
        .WithName("ActivateWBSTemplate")
        .WithSummary("Activate a WBS template");

        // Deactivate template
        group.MapPost("/{id:guid}/deactivate", async (Guid id, IWBSTemplateService service) =>
        {
            var result = await service.DeactivateAsync(id);
            return result ? Results.Ok() : Results.NotFound();
        })
        .WithName("DeactivateWBSTemplate")
        .WithSummary("Deactivate a WBS template");

        // Get template elements
        group.MapGet("/{templateId:guid}/elements", async (Guid templateId, IWBSTemplateService service) =>
        {
            var elements = await service.GetElementsAsync(templateId);
            return Results.Ok(elements);
        })
        .WithName("GetWBSTemplateElements")
        .WithSummary("Get all elements of a WBS template");

        // Add element
        group.MapPost("/{templateId:guid}/elements", async (Guid templateId, WBSTemplateElementDto element, IWBSTemplateService service) =>
        {
            var newElement = await service.AddElementAsync(templateId, element);
            return newElement != null ? Results.Ok(newElement) : Results.BadRequest("Failed to add element");
        })
        .WithName("AddWBSTemplateElement")
        .WithSummary("Add a new element to a WBS template");

        // Update element
        group.MapPut("/elements/{elementId:guid}", async (Guid elementId, WBSTemplateElementDto element, IWBSTemplateService service) =>
        {
            var result = await service.UpdateElementAsync(elementId, element);
            return result ? Results.Ok() : Results.NotFound();
        })
        .WithName("UpdateWBSTemplateElement")
        .WithSummary("Update a WBS template element");

        // Remove element
        group.MapDelete("/{templateId:guid}/elements/{elementId:guid}", async (Guid templateId, Guid elementId, IWBSTemplateService service) =>
        {
            var result = await service.RemoveElementAsync(templateId, elementId);
            return result ? Results.NoContent() : Results.NotFound();
        })
        .WithName("RemoveWBSTemplateElement")
        .WithSummary("Remove an element from a WBS template");

        // Reorder elements
        group.MapPost("/{templateId:guid}/elements/reorder", async (Guid templateId, List<Guid> elementIds, IWBSTemplateService service) =>
        {
            var result = await service.ReorderElementsAsync(templateId, elementIds);
            return result ? Results.Ok() : Results.BadRequest("Failed to reorder elements");
        })
        .WithName("ReorderWBSTemplateElements")
        .WithSummary("Reorder elements in a WBS template");

        // Clone template
        group.MapPost("/{sourceId:guid}/clone", async (Guid sourceId, CloneTemplateRequest request, IWBSTemplateService service) =>
        {
            var cloned = await service.CloneAsync(sourceId, request.NewCode, request.NewName);
            return cloned != null ? Results.Ok(cloned) : Results.BadRequest("Failed to clone template");
        })
        .WithName("CloneWBSTemplate")
        .WithSummary("Clone an existing WBS template");

        // Apply template to project
        group.MapPost("/{templateId:guid}/apply", async (Guid templateId, ApplyTemplateRequest request, IWBSTemplateService service) =>
        {
            var count = await service.ApplyToProjectAsync(templateId, request.ProjectId, request.IncludeOptional);
            return Results.Ok(count);
        })
        .WithName("ApplyWBSTemplate")
        .WithSummary("Apply a WBS template to a project");

        // Validate template
        group.MapGet("/{templateId:guid}/validate", async (Guid templateId, IWBSTemplateService service) =>
        {
            var result = await service.ValidateStructureAsync(templateId);
            return Results.Ok(result);
        })
        .WithName("ValidateWBSTemplate")
        .WithSummary("Validate a WBS template structure");

        // Get usage statistics
        group.MapGet("/{templateId:guid}/statistics", async (Guid templateId, IWBSTemplateService service) =>
        {
            var stats = await service.GetUsageStatisticsAsync(templateId);
            return Results.Ok(stats);
        })
        .WithName("GetWBSTemplateStatistics")
        .WithSummary("Get usage statistics for a WBS template");

        // Import template
        group.MapPost("/import", async (ImportWBSTemplateDto dto, IWBSTemplateService service) =>
        {
            var template = await service.ImportAsync(dto);
            return template != null ? Results.Ok(template) : Results.BadRequest("Failed to import template");
        })
        .WithName("ImportWBSTemplate")
        .WithSummary("Import a WBS template from file");

        // Export template
        group.MapGet("/{templateId:guid}/export", async (Guid templateId, string format, IWBSTemplateService service) =>
        {
            var data = await service.ExportAsync(templateId, format);
            return Results.File(data, "application/octet-stream", $"wbs-template-{templateId}.{format.ToLower()}");
        })
        .WithName("ExportWBSTemplate")
        .WithSummary("Export a WBS template to file");

        // Check code uniqueness
        group.MapGet("/check-code/{code}", async (string code, Guid? excludeId, IWBSTemplateService service) =>
        {
            var isUnique = await service.IsCodeUniqueAsync(code, excludeId);
            return Results.Ok(isUnique);
        })
        .WithName("CheckWBSTemplateCode")
        .WithSummary("Check if a WBS template code is unique");
    }
}

// Request DTOs
public record CloneTemplateRequest(string NewCode, string NewName);
public record ApplyTemplateRequest(Guid ProjectId, bool IncludeOptional = false);