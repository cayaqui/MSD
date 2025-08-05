using Core.DTOs.Common;
using Core.DTOs.Organization.RAM;
using Web.Services.Interfaces;
using Web.Services.Interfaces.Organization;

namespace Web.Services.Implementation.Organization;

/// <summary>
/// Service implementation for RAM (Responsibility Assignment Matrix) API operations
/// </summary>
public class RAMApiService : IRAMApiService
{
    private readonly IApiService _apiService;
    private readonly ILoggingService _logger;
    private const string BaseEndpoint = "api/ram";

    public RAMApiService(IApiService apiService, ILoggingService logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    // Query Operations

    public async Task<PagedResult<RAMDto>?> GetRAMAssignmentsAsync(int pageNumber = 1, int pageSize = 10, string? sortBy = null, bool isAscending = true, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting RAM assignments - Page: {pageNumber}, Size: {pageSize}");
            
            var queryParams = new Dictionary<string, string>
            {
                ["pageNumber"] = pageNumber.ToString(),
                ["pageSize"] = pageSize.ToString(),
                ["isAscending"] = isAscending.ToString()
            };

            if (!string.IsNullOrEmpty(sortBy))
                queryParams["sortBy"] = sortBy;

            return await _apiService.GetAsync<PagedResult<RAMDto>>($"{BaseEndpoint}?{BuildQueryString(queryParams)}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting RAM assignments");
            return null;
        }
    }

    public async Task<RAMDto?> GetRAMByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting RAM assignment by ID: {id}");
            return await _apiService.GetAsync<RAMDto>($"{BaseEndpoint}/{id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting RAM assignment {id}");
            return null;
        }
    }

    public async Task<List<RAMDto>?> GetRAMByProjectAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting RAM assignments for project: {projectId}");
            return await _apiService.GetAsync<List<RAMDto>>($"{BaseEndpoint}/project/{projectId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting RAM assignments for project {projectId}");
            return null;
        }
    }

    public async Task<RAMMatrixDto?> GetProjectRAMMatrixAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting RAM matrix for project: {projectId}");
            return await _apiService.GetAsync<RAMMatrixDto>($"{BaseEndpoint}/project/{projectId}/matrix");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting RAM matrix for project {projectId}");
            return null;
        }
    }

    public async Task<List<UserRAMAssignmentDto>?> GetUserRAMAssignmentsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting RAM assignments for user: {userId}");
            return await _apiService.GetAsync<List<UserRAMAssignmentDto>>($"{BaseEndpoint}/user/{userId}/assignments");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting RAM assignments for user {userId}");
            return null;
        }
    }

    public async Task<List<RAMDto>?> GetRAMByOBSNodeAsync(Guid obsNodeId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting RAM assignments for OBS node: {obsNodeId}");
            return await _apiService.GetAsync<List<RAMDto>>($"{BaseEndpoint}/obs-node/{obsNodeId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting RAM assignments for OBS node {obsNodeId}");
            return null;
        }
    }

    public async Task<byte[]?> ExportRAMMatrixAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Exporting RAM matrix for project: {projectId}");
            
            // Using HttpClient directly for file download
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"{BaseEndpoint}/export/{projectId}", cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }
            
            _logger.LogWarning($"Failed to export RAM matrix. Status: {response.StatusCode}");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error exporting RAM matrix for project {projectId}");
            return null;
        }
    }

    // Command Operations

    public async Task<RAMDto?> CreateRAMAsync(CreateRAMDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Creating new RAM assignment for WBS element: {dto.WBSElementId}");
            return await _apiService.PostAsync<CreateRAMDto, RAMDto>(BaseEndpoint, dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error creating RAM assignment");
            return null;
        }
    }

    public async Task<RAMDto?> UpdateRAMAsync(Guid id, UpdateRAMDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Updating RAM assignment: {id}");
            return await _apiService.PutAsync<UpdateRAMDto, RAMDto>($"{BaseEndpoint}/{id}", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating RAM assignment: {id}");
            return null;
        }
    }

    public async Task<List<RAMDto>?> BulkAssignRAMAsync(List<CreateRAMDto> dtos, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Bulk assigning {dtos.Count} RAM assignments");
            return await _apiService.PostAsync<List<CreateRAMDto>, List<RAMDto>>($"{BaseEndpoint}/bulk-assign", dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bulk assigning RAM");
            return null;
        }
    }

    public async Task<RAMMatrixDto?> ImportRAMMatrixAsync(Guid projectId, byte[] fileData, string fileName, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Importing RAM matrix for project: {projectId}");
            
            // Using MultipartFormDataContent for file upload
            using var content = new MultipartFormDataContent();
            content.Add(new ByteArrayContent(fileData), "file", fileName);
            
            // Note: This would need a special method in IApiService to handle file uploads
            // For now, returning null. In a real implementation, you'd add a method like:
            // return await _apiService.PostFileAsync<RAMMatrixDto>($"{BaseEndpoint}/import", content);
            
            _logger.LogWarning("File upload not implemented in IApiService");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error importing RAM matrix for project {projectId}");
            return null;
        }
    }

    public async Task<RAMDto?> AddRAMNotesAsync(Guid id, AddRAMNotesDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Adding notes to RAM assignment: {id}");
            return await _apiService.PostAsync<AddRAMNotesDto, RAMDto>($"{BaseEndpoint}/{id}/add-notes", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error adding notes to RAM assignment: {id}");
            return null;
        }
    }

    public async Task<bool> DeleteRAMAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Deleting RAM assignment: {id}");
            return await _apiService.DeleteAsync($"{BaseEndpoint}/{id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting RAM assignment: {id}");
            return false;
        }
    }

    private static string BuildQueryString(Dictionary<string, string> parameters)
    {
        return string.Join("&", parameters.Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value)}"));
    }
}