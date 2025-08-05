using Core.DTOs.Common;
using Core.DTOs.Organization.OBSNode;
using Core.DTOs.Auth.ProjectTeamMembers;
using Web.Services.Interfaces;
using Web.Services.Interfaces.Organization;

namespace Web.Services.Implementation.Organization;

/// <summary>
/// Service implementation for OBS (Organizational Breakdown Structure) node API operations
/// </summary>
public class OBSNodeApiService : IOBSNodeApiService
{
    private readonly IApiService _apiService;
    private readonly ILoggingService _logger;
    private const string BaseEndpoint = "api/obs-nodes";

    public OBSNodeApiService(IApiService apiService, ILoggingService logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    // Query Operations

    public async Task<PagedResult<OBSNodeDto>?> GetOBSNodesAsync(int pageNumber = 1, int pageSize = 10, string? sortBy = null, bool isAscending = true, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting OBS nodes - Page: {pageNumber}, Size: {pageSize}");
            
            var queryParams = new Dictionary<string, string>
            {
                ["pageNumber"] = pageNumber.ToString(),
                ["pageSize"] = pageSize.ToString(),
                ["isAscending"] = isAscending.ToString()
            };

            if (!string.IsNullOrEmpty(sortBy))
                queryParams["sortBy"] = sortBy;

            return await _apiService.GetAsync<PagedResult<OBSNodeDto>>($"{BaseEndpoint}?{BuildQueryString(queryParams)}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting OBS nodes");
            return null;
        }
    }

    public async Task<OBSNodeDto?> GetOBSNodeByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting OBS node by ID: {id}");
            return await _apiService.GetAsync<OBSNodeDto>($"{BaseEndpoint}/{id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting OBS node {id}");
            return null;
        }
    }

    public async Task<List<OBSNodeDto>?> GetOBSNodesByProjectAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting OBS nodes for project: {projectId}");
            return await _apiService.GetAsync<List<OBSNodeDto>>($"{BaseEndpoint}/project/{projectId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting OBS nodes for project {projectId}");
            return null;
        }
    }

    public async Task<OBSNodeHierarchyDto?> GetOBSHierarchyAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting OBS hierarchy for project: {projectId}");
            return await _apiService.GetAsync<OBSNodeHierarchyDto>($"{BaseEndpoint}/project/{projectId}/hierarchy");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting OBS hierarchy for project {projectId}");
            return null;
        }
    }

    public async Task<List<OBSNodeDto>?> GetOBSNodeChildrenAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting OBS node children for ID: {id}");
            return await _apiService.GetAsync<List<OBSNodeDto>>($"{BaseEndpoint}/{id}/children");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting OBS node children for {id}");
            return null;
        }
    }

    public async Task<OBSNodeTeamDto?> GetOBSNodeTeamAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting OBS node team for ID: {id}");
            return await _apiService.GetAsync<OBSNodeTeamDto>($"{BaseEndpoint}/{id}/team");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting OBS node team for {id}");
            return null;
        }
    }

    // Command Operations

    public async Task<OBSNodeDto?> CreateOBSNodeAsync(CreateOBSNodeDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Creating new OBS node: {dto.Name}");
            return await _apiService.PostAsync<CreateOBSNodeDto, OBSNodeDto>(BaseEndpoint, dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error creating OBS node: {dto.Name}");
            return null;
        }
    }

    public async Task<OBSNodeDto?> UpdateOBSNodeAsync(Guid id, UpdateOBSNodeDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Updating OBS node: {id}");
            return await _apiService.PutAsync<UpdateOBSNodeDto, OBSNodeDto>($"{BaseEndpoint}/{id}", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating OBS node: {id}");
            return null;
        }
    }

    public async Task<OBSNodeDto?> UpdateOBSNodeManagerAsync(Guid id, UpdateOBSNodeManagerDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Updating OBS node manager: {id}");
            return await _apiService.PutAsync<UpdateOBSNodeManagerDto, OBSNodeDto>($"{BaseEndpoint}/{id}/manager", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating OBS node manager: {id}");
            return null;
        }
    }

    public async Task<OBSNodeDto?> UpdateOBSNodeCostCenterAsync(Guid id, UpdateOBSNodeCostCenterDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Updating OBS node cost center: {id}");
            return await _apiService.PutAsync<UpdateOBSNodeCostCenterDto, OBSNodeDto>($"{BaseEndpoint}/{id}/cost-center", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating OBS node cost center: {id}");
            return null;
        }
    }

    public async Task<OBSNodeDto?> MoveOBSNodeAsync(Guid id, MoveOBSNodeDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Moving OBS node: {id} to parent: {dto.NewParentId}");
            return await _apiService.PostAsync<MoveOBSNodeDto, OBSNodeDto>($"{BaseEndpoint}/{id}/move", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error moving OBS node: {id}");
            return null;
        }
    }

    public async Task<List<OBSNodeDto>?> BulkUpdateOBSNodesAsync(List<UpdateOBSNodeDto> dtos, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Bulk updating {dtos.Count} OBS nodes");
            return await _apiService.PostAsync<List<UpdateOBSNodeDto>, List<OBSNodeDto>>($"{BaseEndpoint}/bulk-update", dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bulk updating OBS nodes");
            return null;
        }
    }

    public async Task<bool> DeleteOBSNodeAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Deleting OBS node: {id}");
            return await _apiService.DeleteAsync($"{BaseEndpoint}/{id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting OBS node: {id}");
            return false;
        }
    }

    private static string BuildQueryString(Dictionary<string, string> parameters)
    {
        return string.Join("&", parameters.Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value)}"));
    }
}