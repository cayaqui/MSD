using Api.Authorization;
using Application.Interfaces.Auth;
using Carter;
using Core.Constants;
using Core.DTOs.Auth.Permissions;
using Core.DTOs.Auth.Users;
using Core.DTOs.Common;
using Microsoft.AspNetCore.Authorization;

namespace Api.Modules;

/// <summary>
/// Permission management endpoints
/// </summary>
public class PermissionsModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var permissions = app.MapGroup("/api/permissions")
            .WithTags("Permissions")
            .RequireAuthorization(policy => policy.RequireRoleOrAdmin(ProjectRoles.ProjectManager));

        // CRUD operations
        permissions.MapGet("/", GetPermissions)
            .WithName("GetPermissions")
            .WithSummary("Get paginated list of permissions")
            .Produces<PagedResult<PermissionDto>>(200);

        permissions.MapPost("/search", SearchPermissions)
            .WithName("SearchPermissions")
            .WithSummary("Search permissions with filters")
            .Produces<IEnumerable<PermissionDto>>(200);

        permissions.MapGet("/{id:guid}", GetPermissionById)
            .WithName("GetPermissionById")
            .WithSummary("Get permission by ID")
            .Produces<PermissionDto>(200)
            .Produces(404);

        permissions.MapGet("/code/{code}", GetPermissionByCode)
            .WithName("GetPermissionByCode")
            .WithSummary("Get permission by code")
            .Produces<PermissionDto>(200)
            .Produces(404);

        permissions.MapGet("/module/{module}", GetPermissionsByModule)
            .WithName("GetPermissionsByModule")
            .WithSummary("Get permissions by module")
            .Produces<IEnumerable<PermissionDto>>(200);

        permissions.MapGet("/module/{module}/resource/{resource}", GetPermissionsByResource)
            .WithName("GetPermissionsByResource")
            .WithSummary("Get permissions by module and resource")
            .Produces<IEnumerable<PermissionDto>>(200);

        permissions.MapPost("/", CreatePermission)
            .WithName("CreatePermission")
            .WithSummary("Create a new permission")
            .Produces<PermissionDto>(201)
            .Produces(400);

        permissions.MapPut("/{id:guid}", UpdatePermission)
            .WithName("UpdatePermission")
            .WithSummary("Update permission details")
            .Produces<PermissionDto>(200)
            .Produces(404)
            .Produces(400);

        permissions.MapDelete("/{id:guid}", DeletePermission)
            .WithName("DeletePermission")
            .WithSummary("Delete a permission")
            .Produces(204)
            .Produces(404)
            .Produces(400);

        // Permission matrix
        permissions.MapGet("/matrix", GetPermissionMatrix)
            .WithName("GetPermissionMatrix")
            .WithSummary("Get permission matrix")
            .Produces<PermissionMatrixDto>(200);

        // Activation/Deactivation
        permissions.MapPost("/{id:guid}/activate", ActivatePermission)
            .WithName("ActivatePermission")
            .WithSummary("Activate a permission")
            .Produces(204)
            .Produces(404);

        permissions.MapPost("/{id:guid}/deactivate", DeactivatePermission)
            .WithName("DeactivatePermission")
            .WithSummary("Deactivate a permission")
            .Produces(204)
            .Produces(404);

        // Bulk operations
        permissions.MapPost("/bulk-assign", BulkAssignToRoles)
            .WithName("BulkAssignToRoles")
            .WithSummary("Bulk assign permissions to roles")
            .Produces<BulkAssignmentResult>(200);

        // System operations
        permissions.MapPost("/initialize", InitializePermissions)
            .WithName("InitializePermissions")
            .WithSummary("Initialize default permissions")
            .Produces<InitializeResult>(200);

        permissions.MapPost("/validate", ValidatePermissions)
            .WithName("ValidatePermissions")
            .WithSummary("Validate permission structure")
            .Produces<PermissionValidationResult>(200);

        // Import/Export
        permissions.MapPost("/export", ExportPermissions)
            .WithName("ExportPermissions")
            .WithSummary("Export permissions to file")
            .Produces<FileResult>(200);

        permissions.MapPost("/import", ImportPermissions)
            .WithName("ImportPermissions")
            .WithSummary("Import permissions from file")
            .Accepts<IFormFile>("multipart/form-data")
            .Produces<ImportResult>(200)
            .Produces(400);

        // Validation
        permissions.MapGet("/check-code/{code}", CheckCodeUnique)
            .WithName("CheckCodeUnique")
            .WithSummary("Check if permission code is unique")
            .Produces<bool>(200);
    }

    private static async Task<IResult> GetPermissions(
        [AsParameters] PaginationQuery query,
        IPermissionService permissionService)
    {
        var result = await permissionService.GetAllPagedAsync(query.PageNumber, query.PageSize);
        return Results.Ok(result);
    }

    private static async Task<IResult> SearchPermissions(
        PermissionFilterDto filter,
        IPermissionService permissionService)
    {
        var result = await permissionService.SearchAsync(filter);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetPermissionById(
        Guid id,
        IPermissionService permissionService)
    {
        var permission = await permissionService.GetByIdAsync(id);
        return permission != null ? Results.Ok(permission) : Results.NotFound($"Permission {id} not found");
    }

    private static async Task<IResult> GetPermissionByCode(
        string code,
        IPermissionService permissionService)
    {
        var permission = await permissionService.GetByCodeAsync(code);
        return permission != null ? Results.Ok(permission) : Results.NotFound($"Permission with code '{code}' not found");
    }

    private static async Task<IResult> GetPermissionsByModule(
        string module,
        IPermissionService permissionService)
    {
        var permissions = await permissionService.GetByModuleAsync(module);
        return Results.Ok(permissions);
    }

    private static async Task<IResult> GetPermissionsByResource(
        string module,
        string resource,
        IPermissionService permissionService)
    {
        var permissions = await permissionService.GetByResourceAsync(module, resource);
        return Results.Ok(permissions);
    }

    private static async Task<IResult> CreatePermission(
        CreatePermissionDto dto,
        IPermissionService permissionService)
    {
        try
        {
            // Check if code is unique
            if (!await permissionService.IsCodeUniqueAsync(dto.Code))
            {
                return Results.BadRequest($"Permission code '{dto.Code}' already exists");
            }

            var permission = await permissionService.CreateAsync(dto);
            return Results.Created($"/api/permissions/{permission.Id}", permission);
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "Permission creation failed",
                detail: ex.Message,
                statusCode: 400);
        }
    }

    private static async Task<IResult> UpdatePermission(
        Guid id,
        UpdatePermissionDto dto,
        IPermissionService permissionService)
    {
        try
        {
            // Check if code is unique (excluding current permission)
            if (!string.IsNullOrEmpty(dto.Code))
            {
                if (!await permissionService.IsCodeUniqueAsync(dto.Code, id))
                {
                    return Results.BadRequest($"Permission code '{dto.Code}' already exists");
                }
            }

            var permission = await permissionService.UpdateAsync(id, dto);
            return permission != null ? Results.Ok(permission) : Results.NotFound($"Permission {id} not found");
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "Permission update failed",
                detail: ex.Message,
                statusCode: 400);
        }
    }

    private static async Task<IResult> DeletePermission(
        Guid id,
        IPermissionService permissionService)
    {
        try
        {
            await permissionService.DeleteAsync(id);
            return Results.NoContent();
        }
        catch (KeyNotFoundException)
        {
            return Results.NotFound($"Permission {id} not found");
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "Permission deletion failed",
                detail: ex.Message,
                statusCode: 400);
        }
    }

    private static async Task<IResult> GetPermissionMatrix(
        IPermissionService permissionService)
    {
        var matrix = await permissionService.GetMatrixAsync();
        return Results.Ok(matrix);
    }

    private static async Task<IResult> ActivatePermission(
        Guid id,
        IPermissionService permissionService)
    {
        var success = await permissionService.ActivateAsync(id);
        return success ? Results.NoContent() : Results.NotFound($"Permission {id} not found");
    }

    private static async Task<IResult> DeactivatePermission(
        Guid id,
        IPermissionService permissionService)
    {
        var success = await permissionService.DeactivateAsync(id);
        return success ? Results.NoContent() : Results.NotFound($"Permission {id} not found");
    }

    private static async Task<IResult> BulkAssignToRoles(
        BulkPermissionAssignmentDto dto,
        IPermissionService permissionService)
    {
        try
        {
            var count = await permissionService.BulkAssignToRolesAsync(dto);
            return Results.Ok(new BulkAssignmentResult(count, dto.PermissionIds.Count * dto.RoleIds.Count));
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "Bulk assignment failed",
                detail: ex.Message,
                statusCode: 400);
        }
    }

    private static async Task<IResult> InitializePermissions(
        IPermissionService permissionService)
    {
        try
        {
            var count = await permissionService.InitializeDefaultPermissionsAsync();
            return Results.Ok(new InitializeResult(count, "Default permissions initialized successfully"));
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "Initialization failed",
                detail: ex.Message,
                statusCode: 500);
        }
    }

    private static async Task<IResult> ValidatePermissions(
        IPermissionService permissionService)
    {
        var result = await permissionService.ValidateStructureAsync();
        return Results.Ok(result);
    }

    private static async Task<IResult> ExportPermissions(
        PermissionFilterDto? filter,
        IPermissionService permissionService)
    {
        try
        {
            var data = await permissionService.ExportAsync(filter);
            return Results.File(data, "application/json", $"permissions_{DateTime.Now:yyyy-MM-dd}.json");
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "Export failed",
                detail: ex.Message,
                statusCode: 500);
        }
    }
    
    private static async Task<IResult> ImportPermissions(
        HttpRequest request,
        IPermissionService permissionService)
    {
        try
        {
            if (!request.HasFormContentType)
            {
                return Results.BadRequest("Invalid content type");
            }

            var form = await request.ReadFormAsync();
            var file = form.Files["file"];
            
            if (file == null || file.Length == 0)
            {
                return Results.BadRequest("No file uploaded");
            }

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            var data = ms.ToArray();

            var overwriteExisting = form.TryGetValue("overwriteExisting", out var overwrite) 
                && bool.Parse(overwrite.ToString());

            var count = await permissionService.ImportAsync(data, overwriteExisting);
            return Results.Ok(new ImportResult(count, "Permissions imported successfully"));
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "Import failed",
                detail: ex.Message,
                statusCode: 400);
        }
    }

    private static async Task<IResult> CheckCodeUnique(
        string code,
        [AsParameters] UniqueCheckQuery query,
        IPermissionService permissionService)
    {
        var isUnique = await permissionService.IsCodeUniqueAsync(code, query.ExcludeId);
        return Results.Ok(isUnique);
    }
}




public record FileResult(byte[] Content, string ContentType, string FileName) : IResult
{
    public Task ExecuteAsync(HttpContext httpContext)
    {
        httpContext.Response.ContentType = ContentType;
        httpContext.Response.Headers.Add("Content-Disposition", $"attachment; filename={FileName}");
        return httpContext.Response.Body.WriteAsync(Content, 0, Content.Length);
    }
}
// Request/Response DTOs
