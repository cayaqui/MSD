using Application.Interfaces.Documents;
using Carter;
using Core.DTOs.Common;
using Core.DTOs.Documents.Document;
using Core.DTOs.Documents.DocumentComment;
using Core.DTOs.Documents.DocumentVersion;
using Core.Enums.Documents;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Documents;

/// <summary>
/// Endpoints for document management
/// </summary>
public class DocumentModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/documents")
            .WithTags("Documents")
            .RequireAuthorization();

        // Query endpoints
        group.MapGet("/", SearchDocuments)
            .WithName("SearchDocuments")
            .WithSummary("Search documents with filters")
            .Produces<DocumentSearchResultDto>();

        group.MapGet("/{id:guid}", GetDocumentById)
            .WithName("GetDocumentById")
            .WithSummary("Get document by ID")
            .Produces<DocumentDto>()
            .ProducesProblem(404);

        group.MapGet("/number/{documentNumber}", GetDocumentByNumber)
            .WithName("GetDocumentByNumber")
            .WithSummary("Get document by document number")
            .Produces<DocumentDto>()
            .ProducesProblem(404);

        group.MapGet("/project/{projectId:guid}", GetProjectDocuments)
            .WithName("GetProjectDocuments")
            .WithSummary("Get all documents for a project")
            .Produces<IEnumerable<DocumentDto>>();

        group.MapGet("/project/{projectId:guid}/recent", GetRecentDocuments)
            .WithName("GetRecentDocuments")
            .WithSummary("Get recent documents for a project")
            .Produces<IEnumerable<DocumentDto>>();

        group.MapGet("/my-documents", GetMyDocuments)
            .WithName("GetMyDocuments")
            .WithSummary("Get documents created by current user")
            .Produces<IEnumerable<DocumentDto>>();

        group.MapGet("/shared-with-me", GetSharedWithMe)
            .WithName("GetSharedWithMe")
            .WithSummary("Get documents shared with current user")
            .Produces<IEnumerable<DocumentDto>>();

        // Command endpoints
        group.MapPost("/", CreateDocument)
            .WithName("CreateDocument")
            .WithSummary("Create a new document")
            .Produces<DocumentDto>(201)
            .ProducesValidationProblem();

        group.MapPut("/{id:guid}", UpdateDocument)
            .WithName("UpdateDocument")
            .WithSummary("Update a document")
            .Produces<DocumentDto>()
            .ProducesValidationProblem()
            .ProducesProblem(404);

        group.MapDelete("/{id:guid}", DeleteDocument)
            .WithName("DeleteDocument")
            .WithSummary("Delete a document")
            .Produces(204)
            .ProducesProblem(404);

        // File operations
        group.MapPost("/upload", UploadDocument)
            .WithName("UploadDocument")
            .WithSummary("Upload a document file")
            .DisableAntiforgery()
            .Produces<DocumentDto>(201)
            .ProducesValidationProblem();

        group.MapGet("/{id:guid}/download", DownloadDocument)
            .WithName("DownloadDocument")
            .WithSummary("Download a document")
            .Produces(200)
            .ProducesProblem(404);

        group.MapGet("/{id:guid}/download-url", GetDownloadUrl)
            .WithName("GetDownloadUrl")
            .WithSummary("Get temporary download URL")
            .Produces<string>()
            .ProducesProblem(404);

        // Version control
        group.MapPost("/{id:guid}/versions", CreateVersion)
            .WithName("CreateDocumentVersion")
            .WithSummary("Create a new version of a document")
            .Produces<DocumentVersionDto>(201)
            .ProducesValidationProblem();

        group.MapGet("/{id:guid}/versions", GetVersions)
            .WithName("GetDocumentVersions")
            .WithSummary("Get all versions of a document")
            .Produces<IEnumerable<DocumentVersionDto>>();

        group.MapGet("/{id:guid}/versions/{version}", GetVersion)
            .WithName("GetDocumentVersion")
            .WithSummary("Get specific version of a document")
            .Produces<DocumentVersionDto>()
            .ProducesProblem(404);

        group.MapPost("/{id:guid}/versions/{versionId:guid}/set-current", SetCurrentVersion)
            .WithName("SetCurrentDocumentVersion")
            .WithSummary("Set a version as current")
            .Produces(200)
            .ProducesProblem(404);

        group.MapGet("/versions/{versionId:guid}/download", DownloadVersion)
            .WithName("DownloadDocumentVersion")
            .WithSummary("Download specific version")
            .Produces(200)
            .ProducesProblem(404);

        // Document management
        group.MapPost("/{id:guid}/lock", LockDocument)
            .WithName("LockDocument")
            .WithSummary("Lock a document for editing")
            .Produces(200)
            .ProducesProblem(404);

        group.MapPost("/{id:guid}/unlock", UnlockDocument)
            .WithName("UnlockDocument")
            .WithSummary("Unlock a document")
            .Produces(200)
            .ProducesProblem(404);

        group.MapPost("/{id:guid}/checkout", CheckOutDocument)
            .WithName("CheckOutDocument")
            .WithSummary("Check out a document")
            .Produces(200)
            .ProducesProblem(404);

        group.MapPost("/{id:guid}/checkin", CheckInDocument)
            .WithName("CheckInDocument")
            .WithSummary("Check in a document")
            .Produces(200)
            .ProducesProblem(404);

        group.MapPatch("/{id:guid}/metadata", UpdateMetadata)
            .WithName("UpdateDocumentMetadata")
            .WithSummary("Update document metadata")
            .Produces(200)
            .ProducesProblem(404);

        // Access control
        group.MapPost("/{id:guid}/permissions", GrantAccess)
            .WithName("GrantDocumentAccess")
            .WithSummary("Grant access to a document")
            .Produces(200)
            .ProducesProblem(404);

        group.MapDelete("/{id:guid}/permissions/{userId:guid}", RevokeAccess)
            .WithName("RevokeDocumentAccess")
            .WithSummary("Revoke access to a document")
            .Produces(204)
            .ProducesProblem(404);

        group.MapGet("/{id:guid}/permissions", GetPermissions)
            .WithName("GetDocumentPermissions")
            .WithSummary("Get document permissions")
            .Produces<IEnumerable<DocumentPermissionDto>>();

        // Review and approval
        group.MapPost("/{id:guid}/submit-review", SubmitForReview)
            .WithName("SubmitDocumentForReview")
            .WithSummary("Submit document for review")
            .Produces(200)
            .ProducesProblem(404);

        group.MapPost("/{id:guid}/approve", ApproveDocument)
            .WithName("ApproveDocument")
            .WithSummary("Approve a document")
            .Produces(200)
            .ProducesProblem(404);

        group.MapPost("/{id:guid}/reject", RejectDocument)
            .WithName("RejectDocument")
            .WithSummary("Reject a document")
            .Produces(200)
            .ProducesProblem(404);

        group.MapGet("/pending-reviews", GetPendingReviews)
            .WithName("GetPendingReviews")
            .WithSummary("Get documents pending review")
            .Produces<IEnumerable<DocumentDto>>();

        // Comments
        group.MapPost("/{id:guid}/comments", AddComment)
            .WithName("AddDocumentComment")
            .WithSummary("Add comment to document")
            .Produces<DocumentCommentDto>(201)
            .ProducesValidationProblem();

        group.MapGet("/{id:guid}/comments", GetComments)
            .WithName("GetDocumentComments")
            .WithSummary("Get document comments")
            .Produces<IEnumerable<DocumentCommentDto>>();

        group.MapPost("/comments/{commentId:guid}/resolve", ResolveComment)
            .WithName("ResolveDocumentComment")
            .WithSummary("Resolve a comment")
            .Produces(200)
            .ProducesProblem(404);

        // Relationships
        group.MapPost("/{id:guid}/relationships", LinkDocuments)
            .WithName("LinkDocuments")
            .WithSummary("Link documents")
            .Produces(200)
            .ProducesProblem(404);

        group.MapDelete("/{id:guid}/relationships/{relatedId:guid}", UnlinkDocuments)
            .WithName("UnlinkDocuments")
            .WithSummary("Unlink documents")
            .Produces(204)
            .ProducesProblem(404);

        group.MapGet("/{id:guid}/relationships", GetRelatedDocuments)
            .WithName("GetRelatedDocuments")
            .WithSummary("Get related documents")
            .Produces<IEnumerable<DocumentDto>>();

        // Statistics
        group.MapPost("/{id:guid}/view", RecordView)
            .WithName("RecordDocumentView")
            .WithSummary("Record document view")
            .Produces(200);

        group.MapGet("/{id:guid}/statistics", GetStatistics)
            .WithName("GetDocumentStatistics")
            .WithSummary("Get document statistics")
            .Produces<DocumentStatisticsDto>()
            .ProducesProblem(404);

        // Bulk operations
        group.MapPost("/bulk", CreateBulk)
            .WithName("CreateBulkDocuments")
            .WithSummary("Create multiple documents")
            .Produces<IEnumerable<DocumentDto>>(201)
            .ProducesValidationProblem();

        group.MapPatch("/bulk/status", UpdateBulkStatus)
            .WithName("UpdateBulkDocumentStatus")
            .WithSummary("Update status for multiple documents")
            .Produces(200)
            .ProducesValidationProblem();

        group.MapDelete("/bulk", DeleteBulk)
            .WithName("DeleteBulkDocuments")
            .WithSummary("Delete multiple documents")
            .Produces(204)
            .ProducesValidationProblem();

        // Export/Import
        group.MapPost("/export", ExportDocuments)
            .WithName("ExportDocuments")
            .WithSummary("Export document list")
            .Produces(200);

        group.MapPost("/import", ImportDocuments)
            .WithName("ImportDocuments")
            .WithSummary("Import documents from file")
            .DisableAntiforgery()
            .Produces<IEnumerable<DocumentDto>>(201)
            .ProducesValidationProblem();
    }

    // Query handlers
    private static async Task<IResult> SearchDocuments(
        [AsParameters] DocumentFilterDto filter,
        IDocumentService service)
    {
        var result = await service.SearchDocumentsAsync(filter);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetDocumentById(
        [FromRoute] Guid id,
        IDocumentService service)
    {
        var result = await service.GetByIdAsync(id);
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> GetDocumentByNumber(
        [FromRoute] string documentNumber,
        IDocumentService service)
    {
        var result = await service.GetByDocumentNumberAsync(documentNumber);
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> GetProjectDocuments(
        [FromRoute] Guid projectId,
        IDocumentService service)
    {
        var result = await service.GetByProjectAsync(projectId);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetRecentDocuments(
        [FromRoute] Guid projectId,
        [FromQuery] int count,
        IDocumentService service)
    {
        var result = await service.GetRecentDocumentsAsync(projectId, count > 0 ? count : 10);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetMyDocuments(
        IDocumentService service,
        IHttpContextAccessor httpContextAccessor)
    {
        var userId = GetUserId(httpContextAccessor);
        var result = await service.GetMyDocumentsAsync(userId);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetSharedWithMe(
        IDocumentService service,
        IHttpContextAccessor httpContextAccessor)
    {
        var userId = GetUserId(httpContextAccessor);
        var result = await service.GetSharedWithMeAsync(userId);
        return Results.Ok(result);
    }

    // Command handlers
    private static async Task<IResult> CreateDocument(
        [FromBody] CreateDocumentDto dto,
        IDocumentService service)
    {
        var result = await service.CreateAsync(dto);
        return result != null 
            ? Results.Created($"/api/documents/{result.Id}", result)
            : Results.BadRequest();
    }

    private static async Task<IResult> UpdateDocument(
        [FromRoute] Guid id,
        [FromBody] UpdateDocumentDto dto,
        IDocumentService service)
    {
        var result = await service.UpdateAsync(id, dto);
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> DeleteDocument(
        [FromRoute] Guid id,
        IDocumentService service)
    {
        var deleted = await service.DeleteAsync(id);
        return deleted ? Results.NoContent() : Results.NotFound();
    }

    // File operations
    private static async Task<IResult> UploadDocument(
        HttpRequest request,
        IDocumentService service)
    {
        if (!request.HasFormContentType)
            return Results.BadRequest("Request must have form content type");

        var form = await request.ReadFormAsync();
        var file = form.Files.FirstOrDefault();
        if (file == null || file.Length == 0)
            return Results.BadRequest("No file uploaded");

        // Parse document metadata from form
        var dto = new CreateDocumentDto
        {
            DocumentNumber = form["documentNumber"],
            Title = form["title"],
            Description = form["description"],
            ProjectId = Guid.Parse(form["projectId"]),
            DisciplineId = string.IsNullOrEmpty(form["disciplineId"]) ? null : Guid.Parse(form["disciplineId"]),
            PhaseId = string.IsNullOrEmpty(form["phaseId"]) ? null : Guid.Parse(form["phaseId"]),
            Type = Enum.Parse<DocumentType>(form["type"]),
            Category = string.IsNullOrEmpty(form["category"]) ? null : Enum.Parse<DocumentCategory>(form["category"])
        };

        using var stream = file.OpenReadStream();
        var result = await service.UploadDocumentAsync(dto, stream, file.FileName, file.ContentType);
        
        return result != null 
            ? Results.Created($"/api/documents/{result.Id}", result)
            : Results.BadRequest();
    }

    private static async Task<IResult> DownloadDocument(
        [FromRoute] Guid id,
        IDocumentService service)
    {
        var stream = await service.DownloadDocumentAsync(id);
        if (stream == null)
            return Results.NotFound();

        var document = await service.GetByIdAsync(id);
        return Results.File(stream, document!.ContentType ?? "application/octet-stream", document.FileName);
    }

    private static async Task<IResult> GetDownloadUrl(
        [FromRoute] Guid id,
        [FromQuery] int expiryMinutes,
        IDocumentService service)
    {
        var url = await service.GetDownloadUrlAsync(id, expiryMinutes > 0 ? expiryMinutes : 60);
        return url != null ? Results.Ok(new { url }) : Results.NotFound();
    }

    // Version control handlers
    private static async Task<IResult> CreateVersion(
        [FromRoute] Guid id,
        HttpRequest request,
        IDocumentService service)
    {
        if (!request.HasFormContentType)
            return Results.BadRequest("Request must have form content type");

        var form = await request.ReadFormAsync();
        var file = form.Files.FirstOrDefault();
        if (file == null || file.Length == 0)
            return Results.BadRequest("No file uploaded");

        var dto = new CreateDocumentVersionDto
        {
            DocumentId = id,
            Version = form["version"],
            Description = form["description"],
            ChangeReasons = form["changeReasons"],
            Tags = form["tags"]
        };

        // TODO: Handle file upload for version
        var result = await service.CreateVersionAsync(dto);
        
        return result != null 
            ? Results.Created($"/api/documents/{id}/versions/{result.Id}", result)
            : Results.BadRequest();
    }

    private static async Task<IResult> GetVersions(
        [FromRoute] Guid id,
        IDocumentService service)
    {
        var result = await service.GetVersionsAsync(id);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetVersion(
        [FromRoute] Guid id,
        [FromRoute] string version,
        IDocumentService service)
    {
        var result = await service.GetVersionAsync(id, version);
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> SetCurrentVersion(
        [FromRoute] Guid id,
        [FromRoute] Guid versionId,
        IDocumentService service)
    {
        var result = await service.SetCurrentVersionAsync(id, versionId);
        return result ? Results.Ok() : Results.NotFound();
    }

    private static async Task<IResult> DownloadVersion(
        [FromRoute] Guid versionId,
        IDocumentService service)
    {
        var stream = await service.DownloadVersionAsync(versionId);
        return stream != null 
            ? Results.File(stream, "application/octet-stream")
            : Results.NotFound();
    }

    // Document management handlers
    private static async Task<IResult> LockDocument(
        [FromRoute] Guid id,
        IDocumentService service,
        IHttpContextAccessor httpContextAccessor)
    {
        var userId = GetUserId(httpContextAccessor);
        var result = await service.LockDocumentAsync(id, userId);
        return result ? Results.Ok() : Results.BadRequest();
    }

    private static async Task<IResult> UnlockDocument(
        [FromRoute] Guid id,
        IDocumentService service,
        IHttpContextAccessor httpContextAccessor)
    {
        var userId = GetUserId(httpContextAccessor);
        var result = await service.UnlockDocumentAsync(id, userId);
        return result ? Results.Ok() : Results.BadRequest();
    }

    private static async Task<IResult> CheckOutDocument(
        [FromRoute] Guid id,
        IDocumentService service,
        IHttpContextAccessor httpContextAccessor)
    {
        var userId = GetUserId(httpContextAccessor);
        var result = await service.CheckOutAsync(id, userId);
        return result ? Results.Ok() : Results.BadRequest();
    }

    private static async Task<IResult> CheckInDocument(
        [FromRoute] Guid id,
        [FromBody] CheckInRequest request,
        IDocumentService service,
        IHttpContextAccessor httpContextAccessor)
    {
        var userId = GetUserId(httpContextAccessor);
        var result = await service.CheckInAsync(id, userId, request.Comments);
        return result ? Results.Ok() : Results.BadRequest();
    }

    private static async Task<IResult> UpdateMetadata(
        [FromRoute] Guid id,
        [FromBody] Dictionary<string, string> metadata,
        IDocumentService service)
    {
        var result = await service.UpdateMetadataAsync(id, metadata);
        return result ? Results.Ok() : Results.NotFound();
    }

    // Access control handlers
    private static async Task<IResult> GrantAccess(
        [FromRoute] Guid id,
        [FromBody] GrantAccessRequest request,
        IDocumentService service)
    {
        var result = await service.GrantAccessAsync(id, request.UserId, request.Permissions);
        return result ? Results.Ok() : Results.NotFound();
    }

    private static async Task<IResult> RevokeAccess(
        [FromRoute] Guid id,
        [FromRoute] Guid userId,
        IDocumentService service)
    {
        var result = await service.RevokeAccessAsync(id, userId);
        return result ? Results.NoContent() : Results.NotFound();
    }

    private static async Task<IResult> GetPermissions(
        [FromRoute] Guid id,
        IDocumentService service)
    {
        var result = await service.GetPermissionsAsync(id);
        return Results.Ok(result);
    }

    // Review and approval handlers
    private static async Task<IResult> SubmitForReview(
        [FromRoute] Guid id,
        [FromBody] SubmitForReviewRequest request,
        IDocumentService service)
    {
        var result = await service.SubmitForReviewAsync(id, request.ReviewerIds);
        return result ? Results.Ok() : Results.NotFound();
    }

    private static async Task<IResult> ApproveDocument(
        [FromRoute] Guid id,
        [FromBody] ApprovalRequest request,
        IDocumentService service,
        IHttpContextAccessor httpContextAccessor)
    {
        var userId = GetUserId(httpContextAccessor);
        var result = await service.ApproveDocumentAsync(id, userId, request.Comments);
        return result ? Results.Ok() : Results.BadRequest();
    }

    private static async Task<IResult> RejectDocument(
        [FromRoute] Guid id,
        [FromBody] RejectionRequest request,
        IDocumentService service,
        IHttpContextAccessor httpContextAccessor)
    {
        var userId = GetUserId(httpContextAccessor);
        var result = await service.RejectDocumentAsync(id, userId, request.Comments);
        return result ? Results.Ok() : Results.BadRequest();
    }

    private static async Task<IResult> GetPendingReviews(
        IDocumentService service,
        IHttpContextAccessor httpContextAccessor)
    {
        var userId = GetUserId(httpContextAccessor);
        var result = await service.GetPendingReviewsAsync(userId);
        return Results.Ok(result);
    }

    // Comments handlers
    private static async Task<IResult> AddComment(
        [FromRoute] Guid id,
        [FromBody] CreateDocumentCommentDto dto,
        IDocumentService service)
    {
        dto.DocumentId = id;
        var result = await service.AddCommentAsync(dto);
        return result != null 
            ? Results.Created($"/api/documents/{id}/comments/{result.Id}", result)
            : Results.BadRequest();
    }

    private static async Task<IResult> GetComments(
        [FromRoute] Guid id,
        IDocumentService service)
    {
        var result = await service.GetCommentsAsync(id);
        return Results.Ok(result);
    }

    private static async Task<IResult> ResolveComment(
        [FromRoute] Guid commentId,
        [FromBody] ResolveCommentRequest request,
        IDocumentService service,
        IHttpContextAccessor httpContextAccessor)
    {
        var userId = GetUserId(httpContextAccessor);
        var result = await service.ResolveCommentAsync(commentId, userId, request.Resolution);
        return result ? Results.Ok() : Results.NotFound();
    }

    // Relationships handlers
    private static async Task<IResult> LinkDocuments(
        [FromRoute] Guid id,
        [FromBody] LinkDocumentsRequest request,
        IDocumentService service)
    {
        var result = await service.LinkDocumentsAsync(id, request.RelatedDocumentId, request.RelationshipType);
        return result ? Results.Ok() : Results.NotFound();
    }

    private static async Task<IResult> UnlinkDocuments(
        [FromRoute] Guid id,
        [FromRoute] Guid relatedId,
        IDocumentService service)
    {
        var result = await service.UnlinkDocumentsAsync(id, relatedId);
        return result ? Results.NoContent() : Results.NotFound();
    }

    private static async Task<IResult> GetRelatedDocuments(
        [FromRoute] Guid id,
        IDocumentService service)
    {
        var result = await service.GetRelatedDocumentsAsync(id);
        return Results.Ok(result);
    }

    // Statistics handlers
    private static async Task<IResult> RecordView(
        [FromRoute] Guid id,
        IDocumentService service,
        IHttpContextAccessor httpContextAccessor)
    {
        var userId = GetUserId(httpContextAccessor);
        await service.RecordViewAsync(id, userId);
        return Results.Ok();
    }

    private static async Task<IResult> GetStatistics(
        [FromRoute] Guid id,
        IDocumentService service)
    {
        var result = await service.GetStatisticsAsync(id);
        return Results.Ok(result);
    }

    // Bulk operations handlers
    private static async Task<IResult> CreateBulk(
        [FromBody] List<CreateDocumentDto> documents,
        IDocumentService service)
    {
        var result = await service.CreateBulkAsync(documents);
        return Results.Created("/api/documents", result);
    }

    private static async Task<IResult> UpdateBulkStatus(
        [FromBody] BulkStatusUpdateRequest request,
        IDocumentService service)
    {
        var result = await service.UpdateBulkStatusAsync(request.DocumentIds, request.Status);
        return result ? Results.Ok() : Results.BadRequest();
    }

    private static async Task<IResult> DeleteBulk(
        [FromBody] List<Guid> documentIds,
        IDocumentService service)
    {
        var result = await service.DeleteBulkAsync(documentIds);
        return result ? Results.NoContent() : Results.BadRequest();
    }

    // Export/Import handlers
    private static async Task<IResult> ExportDocuments(
        [FromBody] ExportRequest request,
        IDocumentService service)
    {
        var data = await service.ExportDocumentListAsync(request.Filter, request.Format);
        var contentType = request.Format == ExportFormat.Excel 
            ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            : "text/csv";
        var fileName = $"documents_{DateTime.UtcNow:yyyyMMddHHmmss}.{request.Format.ToString().ToLower()}";
        
        return Results.File(data, contentType, fileName);
    }

    private static async Task<IResult> ImportDocuments(
        HttpRequest request,
        IDocumentService service)
    {
        if (!request.HasFormContentType)
            return Results.BadRequest("Request must have form content type");

        var form = await request.ReadFormAsync();
        var file = form.Files.FirstOrDefault();
        if (file == null || file.Length == 0)
            return Results.BadRequest("No file uploaded");

        using var stream = file.OpenReadStream();
        var result = await service.ImportDocumentsAsync(stream, file.ContentType);
        
        return Results.Created("/api/documents", result);
    }

    // Helper methods
    private static Guid GetUserId(IHttpContextAccessor httpContextAccessor)
    {
        var userIdClaim = httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value
            ?? httpContextAccessor.HttpContext?.User?.FindFirst("userId")?.Value;
        
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }
}

// Request DTOs
public record CheckInRequest(string? Comments);
public record GrantAccessRequest(Guid UserId, Core.DTOs.Documents.Document.DocumentPermissionDto Permissions);
public record SubmitForReviewRequest(List<Guid> ReviewerIds);
public record ApprovalRequest(string? Comments);
public record RejectionRequest(string Comments);
public record ResolveCommentRequest(string? Resolution);
public record LinkDocumentsRequest(Guid RelatedDocumentId, DocumentRelationshipType RelationshipType);
public record BulkStatusUpdateRequest(List<Guid> DocumentIds, DocumentStatus Status);
public record ExportRequest(DocumentFilterDto Filter, ExportFormat Format);