using Core.DTOs.Common;
using Core.DTOs.Documents.Document;
using Core.DTOs.Documents.DocumentComment;
using Core.DTOs.Documents.DocumentVersion;
using Core.Enums.Documents;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Headers;
using Web.Services.Interfaces;
using Web.Services.Interfaces.Documents;

namespace Web.Services.Implementation.Documents;

/// <summary>
/// Service implementation for document API operations
/// </summary>
public class DocumentApiService : IDocumentApiService
{
    private readonly IApiService _apiService;
    private readonly ILoggingService _logger;
    private readonly HttpClient _httpClient;
    private const string BaseEndpoint = "api/documents";

    public DocumentApiService(IApiService apiService, ILoggingService logger, IHttpClientFactory httpClientFactory)
    {
        _apiService = apiService;
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient("EzProAPI");
    }

    // Query Operations

    public async Task<DocumentSearchResultDto?> SearchDocumentsAsync(DocumentFilterDto filter, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Searching documents - Page: {filter.PageNumber}, Size: {filter.PageSize}");

            var queryParams = new Dictionary<string, string>
            {
                ["pageNumber"] = filter.PageNumber.ToString(),
                ["pageSize"] = filter.PageSize.ToString()
            };

            if (!string.IsNullOrEmpty(filter.SearchTerm))
                queryParams["searchTerm"] = filter.SearchTerm;

            if (filter.ProjectId.HasValue)
                queryParams["projectId"] = filter.ProjectId.Value.ToString();

            if (filter.Type.HasValue)
                queryParams["type"] = filter.Type.Value.ToString();

            if (filter.Category.HasValue)
                queryParams["category"] = filter.Category.Value.ToString();

            if (filter.Status.HasValue)
                queryParams["status"] = filter.Status.Value.ToString();

            if (filter.DisciplineId.HasValue)
                queryParams["disciplineId"] = filter.DisciplineId.Value.ToString();

            if (filter.PhaseId.HasValue)
                queryParams["phaseId"] = filter.PhaseId.Value.ToString();

            if (filter.DateFrom.HasValue)
                queryParams["dateFrom"] = filter.DateFrom.Value.ToString("O");

            if (filter.DateTo.HasValue)
                queryParams["dateTo"] = filter.DateTo.Value.ToString("O");

            if (!string.IsNullOrEmpty(filter.SortBy))
                queryParams["sortBy"] = filter.SortBy;

            if (filter.SortDescending)
                queryParams["sortDescending"] = "true";

            return await _apiService.GetAsync<DocumentSearchResultDto>($"{BaseEndpoint}?{BuildQueryString(queryParams)}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching documents");
            return null;
        }
    }

    public async Task<DocumentDto?> GetDocumentByIdAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting document by ID: {documentId}");
            return await _apiService.GetAsync<DocumentDto>($"{BaseEndpoint}/{documentId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting document {documentId}");
            return null;
        }
    }

    public async Task<DocumentDto?> GetDocumentByNumberAsync(string documentNumber, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting document by number: {documentNumber}");
            return await _apiService.GetAsync<DocumentDto>($"{BaseEndpoint}/number/{Uri.EscapeDataString(documentNumber)}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting document {documentNumber}");
            return null;
        }
    }

    public async Task<IEnumerable<DocumentDto>?> GetProjectDocumentsAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting documents for project: {projectId}");
            return await _apiService.GetAsync<IEnumerable<DocumentDto>>($"{BaseEndpoint}/project/{projectId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting documents for project {projectId}");
            return null;
        }
    }

    public async Task<IEnumerable<DocumentDto>?> GetRecentDocumentsAsync(Guid projectId, int count = 10, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting recent documents for project: {projectId}");
            return await _apiService.GetAsync<IEnumerable<DocumentDto>>($"{BaseEndpoint}/project/{projectId}/recent?count={count}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting recent documents for project {projectId}");
            return null;
        }
    }

    public async Task<IEnumerable<DocumentDto>?> GetMyDocumentsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting my documents");
            return await _apiService.GetAsync<IEnumerable<DocumentDto>>($"{BaseEndpoint}/my-documents");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting my documents");
            return null;
        }
    }

    public async Task<IEnumerable<DocumentDto>?> GetSharedWithMeAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting shared documents");
            return await _apiService.GetAsync<IEnumerable<DocumentDto>>($"{BaseEndpoint}/shared-with-me");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting shared documents");
            return null;
        }
    }

    // Command Operations

    public async Task<DocumentDto?> CreateDocumentAsync(CreateDocumentDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Creating new document: {dto.Title}");
            return await _apiService.PostAsync<CreateDocumentDto, DocumentDto>(BaseEndpoint, dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error creating document: {dto.Title}");
            return null;
        }
    }

    public async Task<DocumentDto?> UpdateDocumentAsync(Guid documentId, UpdateDocumentDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Updating document: {documentId}");
            return await _apiService.PutAsync<UpdateDocumentDto, DocumentDto>($"{BaseEndpoint}/{documentId}", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating document: {documentId}");
            return null;
        }
    }

    public async Task<bool> DeleteDocumentAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Deleting document: {documentId}");
            return await _apiService.DeleteAsync($"{BaseEndpoint}/{documentId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting document: {documentId}");
            return false;
        }
    }

    // File Operations

    public async Task<DocumentDto?> UploadDocumentAsync(CreateDocumentDto dto, IBrowserFile file, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Uploading document: {dto.Title}");

            using var content = new MultipartFormDataContent();
            
            // Add file
            var fileContent = new StreamContent(file.OpenReadStream(maxAllowedSize: 100 * 1024 * 1024)); // 100MB max
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
            content.Add(fileContent, "file", file.Name);

            // Add metadata
            content.Add(new StringContent(dto.DocumentNumber), "documentNumber");
            content.Add(new StringContent(dto.Title), "title");
            content.Add(new StringContent(dto.Description ?? ""), "description");
            content.Add(new StringContent(dto.ProjectId.ToString()), "projectId");
            content.Add(new StringContent(dto.Type.ToString()), "type");

            if (dto.DisciplineId.HasValue)
                content.Add(new StringContent(dto.DisciplineId.Value.ToString()), "disciplineId");

            if (dto.PhaseId.HasValue)
                content.Add(new StringContent(dto.PhaseId.Value.ToString()), "phaseId");

            if (dto.Category.HasValue)
                content.Add(new StringContent(dto.Category.Value.ToString()), "category");

            var response = await _httpClient.PostAsync($"{BaseEndpoint}/upload", content, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                return System.Text.Json.JsonSerializer.Deserialize<DocumentDto>(json, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            _logger.LogError($"Upload failed with status: {response.StatusCode}");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error uploading document: {dto.Title}");
            return null;
        }
    }

    public async Task<byte[]?> DownloadDocumentAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Downloading document: {documentId}");
            var response = await _httpClient.GetAsync($"{BaseEndpoint}/{documentId}/download", cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync(cancellationToken);
            }

            _logger.LogError($"Download failed with status: {response.StatusCode}");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error downloading document: {documentId}");
            return null;
        }
    }

    public async Task<string?> GetDownloadUrlAsync(Guid documentId, int expiryMinutes = 60, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting download URL for document: {documentId}");
            var result = await _apiService.GetAsync<dynamic>($"{BaseEndpoint}/{documentId}/download-url?expiryMinutes={expiryMinutes}");
            return result?.url?.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting download URL for document: {documentId}");
            return null;
        }
    }

    // Version Control

    public async Task<DocumentVersionDto?> CreateVersionAsync(Guid documentId, CreateDocumentVersionDto dto, IBrowserFile file, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Creating new version for document: {documentId}");

            using var content = new MultipartFormDataContent();
            
            // Add file
            var fileContent = new StreamContent(file.OpenReadStream(maxAllowedSize: 100 * 1024 * 1024)); // 100MB max
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
            content.Add(fileContent, "file", file.Name);

            // Add metadata
            content.Add(new StringContent(dto.Version), "version");
            content.Add(new StringContent(dto.Description ?? ""), "description");
            content.Add(new StringContent(dto.ChangeReasons ?? ""), "changeReasons");
            content.Add(new StringContent(dto.Tags ?? ""), "tags");

            var response = await _httpClient.PostAsync($"{BaseEndpoint}/{documentId}/versions", content, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                return System.Text.Json.JsonSerializer.Deserialize<DocumentVersionDto>(json, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            _logger.LogError($"Version creation failed with status: {response.StatusCode}");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error creating version for document: {documentId}");
            return null;
        }
    }

    public async Task<IEnumerable<DocumentVersionDto>?> GetVersionsAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting versions for document: {documentId}");
            return await _apiService.GetAsync<IEnumerable<DocumentVersionDto>>($"{BaseEndpoint}/{documentId}/versions");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting versions for document: {documentId}");
            return null;
        }
    }

    public async Task<DocumentVersionDto?> GetVersionAsync(Guid documentId, string version, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting version {version} for document: {documentId}");
            return await _apiService.GetAsync<DocumentVersionDto>($"{BaseEndpoint}/{documentId}/versions/{Uri.EscapeDataString(version)}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting version {version} for document: {documentId}");
            return null;
        }
    }

    public async Task<bool> SetCurrentVersionAsync(Guid documentId, Guid versionId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Setting current version for document: {documentId}");
            return await _apiService.PostAsync($"{BaseEndpoint}/{documentId}/versions/{versionId}/set-current");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error setting current version for document: {documentId}");
            return false;
        }
    }

    public async Task<byte[]?> DownloadVersionAsync(Guid versionId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Downloading version: {versionId}");
            var response = await _httpClient.GetAsync($"{BaseEndpoint}/versions/{versionId}/download", cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync(cancellationToken);
            }

            _logger.LogError($"Version download failed with status: {response.StatusCode}");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error downloading version: {versionId}");
            return null;
        }
    }

    // Document Management

    public async Task<bool> LockDocumentAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Locking document: {documentId}");
            return await _apiService.PostAsync($"{BaseEndpoint}/{documentId}/lock");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error locking document: {documentId}");
            return false;
        }
    }

    public async Task<bool> UnlockDocumentAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Unlocking document: {documentId}");
            return await _apiService.PostAsync($"{BaseEndpoint}/{documentId}/unlock");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error unlocking document: {documentId}");
            return false;
        }
    }

    public async Task<bool> CheckOutDocumentAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Checking out document: {documentId}");
            return await _apiService.PostAsync($"{BaseEndpoint}/{documentId}/checkout");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error checking out document: {documentId}");
            return false;
        }
    }

    public async Task<bool> CheckInDocumentAsync(Guid documentId, string? comments = null, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Checking in document: {documentId}");
            return await _apiService.PostAsync($"{BaseEndpoint}/{documentId}/checkin", new { comments });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error checking in document: {documentId}");
            return false;
        }
    }

    public async Task<bool> UpdateMetadataAsync(Guid documentId, Dictionary<string, string> metadata, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Updating metadata for document: {documentId}");
            return await _apiService.PatchAsync($"{BaseEndpoint}/{documentId}/metadata", metadata);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating metadata for document: {documentId}");
            return false;
        }
    }

    // Access Control

    public async Task<bool> GrantAccessAsync(Guid documentId, Guid userId, DocumentPermissionDto permissions, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Granting access to document: {documentId} for user: {userId}");
            return await _apiService.PostAsync($"{BaseEndpoint}/{documentId}/permissions", new { userId, permissions });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error granting access to document: {documentId}");
            return false;
        }
    }

    public async Task<bool> RevokeAccessAsync(Guid documentId, Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Revoking access to document: {documentId} for user: {userId}");
            return await _apiService.DeleteAsync($"{BaseEndpoint}/{documentId}/permissions/{userId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error revoking access to document: {documentId}");
            return false;
        }
    }

    public async Task<IEnumerable<DocumentPermissionDto>?> GetPermissionsAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting permissions for document: {documentId}");
            return await _apiService.GetAsync<IEnumerable<DocumentPermissionDto>>($"{BaseEndpoint}/{documentId}/permissions");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting permissions for document: {documentId}");
            return null;
        }
    }

    // Review and Approval

    public async Task<bool> SubmitForReviewAsync(Guid documentId, List<Guid> reviewerIds, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Submitting document for review: {documentId}");
            return await _apiService.PostAsync($"{BaseEndpoint}/{documentId}/submit-review", new { reviewerIds });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error submitting document for review: {documentId}");
            return false;
        }
    }

    public async Task<bool> ApproveDocumentAsync(Guid documentId, string? comments = null, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Approving document: {documentId}");
            return await _apiService.PostAsync($"{BaseEndpoint}/{documentId}/approve", new { comments });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error approving document: {documentId}");
            return false;
        }
    }

    public async Task<bool> RejectDocumentAsync(Guid documentId, string comments, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Rejecting document: {documentId}");
            return await _apiService.PostAsync($"{BaseEndpoint}/{documentId}/reject", new { comments });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error rejecting document: {documentId}");
            return false;
        }
    }

    public async Task<IEnumerable<DocumentDto>?> GetPendingReviewsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting pending reviews");
            return await _apiService.GetAsync<IEnumerable<DocumentDto>>($"{BaseEndpoint}/pending-reviews");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pending reviews");
            return null;
        }
    }

    // Comments

    public async Task<DocumentCommentDto?> AddCommentAsync(Guid documentId, CreateDocumentCommentDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Adding comment to document: {documentId}");
            dto.DocumentId = documentId;
            return await _apiService.PostAsync<CreateDocumentCommentDto, DocumentCommentDto>($"{BaseEndpoint}/{documentId}/comments", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error adding comment to document: {documentId}");
            return null;
        }
    }

    public async Task<IEnumerable<DocumentCommentDto>?> GetCommentsAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting comments for document: {documentId}");
            return await _apiService.GetAsync<IEnumerable<DocumentCommentDto>>($"{BaseEndpoint}/{documentId}/comments");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting comments for document: {documentId}");
            return null;
        }
    }

    public async Task<bool> ResolveCommentAsync(Guid commentId, string? resolution = null, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Resolving comment: {commentId}");
            return await _apiService.PostAsync($"{BaseEndpoint}/comments/{commentId}/resolve", new { resolution });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error resolving comment: {commentId}");
            return false;
        }
    }

    // Relationships

    public async Task<bool> LinkDocumentsAsync(Guid documentId, Guid relatedDocumentId, DocumentRelationshipType relationshipType, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Linking documents: {documentId} and {relatedDocumentId}");
            return await _apiService.PostAsync($"{BaseEndpoint}/{documentId}/relationships", new { relatedDocumentId, relationshipType });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error linking documents: {documentId} and {relatedDocumentId}");
            return false;
        }
    }

    public async Task<bool> UnlinkDocumentsAsync(Guid documentId, Guid relatedDocumentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Unlinking documents: {documentId} and {relatedDocumentId}");
            return await _apiService.DeleteAsync($"{BaseEndpoint}/{documentId}/relationships/{relatedDocumentId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error unlinking documents: {documentId} and {relatedDocumentId}");
            return false;
        }
    }

    public async Task<IEnumerable<DocumentDto>?> GetRelatedDocumentsAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting related documents for: {documentId}");
            return await _apiService.GetAsync<IEnumerable<DocumentDto>>($"{BaseEndpoint}/{documentId}/relationships");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting related documents for: {documentId}");
            return null;
        }
    }

    // Statistics

    public async Task RecordViewAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Recording view for document: {documentId}");
            await _apiService.PostAsync($"{BaseEndpoint}/{documentId}/view");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error recording view for document: {documentId}");
        }
    }

    public async Task<DocumentStatisticsDto?> GetStatisticsAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting statistics for document: {documentId}");
            return await _apiService.GetAsync<DocumentStatisticsDto>($"{BaseEndpoint}/{documentId}/statistics");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting statistics for document: {documentId}");
            return null;
        }
    }

    // Bulk Operations

    public async Task<IEnumerable<DocumentDto>?> CreateBulkAsync(List<CreateDocumentDto> documents, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Creating {documents.Count} documents in bulk");
            return await _apiService.PostAsync<List<CreateDocumentDto>, IEnumerable<DocumentDto>>($"{BaseEndpoint}/bulk", documents);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating documents in bulk");
            return null;
        }
    }

    public async Task<bool> UpdateBulkStatusAsync(List<Guid> documentIds, DocumentStatus status, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Updating status for {documentIds.Count} documents");
            return await _apiService.PatchAsync($"{BaseEndpoint}/bulk/status", new { documentIds, status });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating documents status in bulk");
            return false;
        }
    }

    public async Task<bool> DeleteBulkAsync(List<Guid> documentIds, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Deleting {documentIds.Count} documents");
            return await _apiService.DeleteAsync($"{BaseEndpoint}/bulk", documentIds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting documents in bulk");
            return false;
        }
    }

    // Export/Import

    public async Task<byte[]?> ExportDocumentsAsync(DocumentFilterDto filter, ExportFormat format, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Exporting documents to {format}");
            var response = await _httpClient.PostAsJsonAsync($"{BaseEndpoint}/export", new { filter, format }, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync(cancellationToken);
            }

            _logger.LogError($"Export failed with status: {response.StatusCode}");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting documents");
            return null;
        }
    }

    public async Task<IEnumerable<DocumentDto>?> ImportDocumentsAsync(IBrowserFile file, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Importing documents from file: {file.Name}");

            using var content = new MultipartFormDataContent();
            var fileContent = new StreamContent(file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024)); // 10MB max
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
            content.Add(fileContent, "file", file.Name);

            var response = await _httpClient.PostAsync($"{BaseEndpoint}/import", content, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                return System.Text.Json.JsonSerializer.Deserialize<IEnumerable<DocumentDto>>(json, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            _logger.LogError($"Import failed with status: {response.StatusCode}");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error importing documents from file: {file.Name}");
            return null;
        }
    }

    private static string BuildQueryString(Dictionary<string, string> parameters)
    {
        return string.Join("&", parameters.Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value)}"));
    }
}