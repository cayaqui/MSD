using Core.DTOs.Common;
using Core.DTOs.Cost.ControlAccounts;
using Web.Services.Interfaces;
using Web.Services.Interfaces.Cost;

namespace Web.Services.Implementation.Cost;

/// <summary>
/// Service implementation for control account API operations
/// </summary>
public class ControlAccountApiService : IControlAccountApiService
{
    private readonly IApiService _apiService;
    private readonly ILoggingService _logger;
    private const string BaseEndpoint = "api/control-accounts";

    public ControlAccountApiService(IApiService apiService, ILoggingService logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    // Query Operations
    public async Task<PagedResult<ControlAccountDto>> GetControlAccountsAsync(ControlAccountFilterDto? filter = null, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting control accounts - Page: {filter?.PageNumber}, Size: {filter?.PageSize}");
            filter ??= new ControlAccountFilterDto();
            
            var queryParams = new Dictionary<string, string>
            {
                ["pageNumber"] = filter.PageNumber.ToString(),
                ["pageSize"] = filter.PageSize.ToString(),
                ["sortDirection"] = filter.SortDirection.ToString()
            };

            if (!string.IsNullOrEmpty(filter.SortBy))
                queryParams["sortBy"] = filter.SortBy;
                
            if (filter.ProjectId.HasValue)
                queryParams["projectId"] = filter.ProjectId.Value.ToString();
                
            if (!string.IsNullOrEmpty(filter.SearchTerm))
                queryParams["searchTerm"] = filter.SearchTerm;
                
            if (filter.PhaseId.HasValue)
                queryParams["phaseId"] = filter.PhaseId.Value.ToString();
                
            if (!string.IsNullOrEmpty(filter.Status))
                queryParams["status"] = filter.Status;
                
            if (filter.AssignedToUserId.HasValue)
                queryParams["assignedToUserId"] = filter.AssignedToUserId.Value.ToString();

            return await _apiService.GetAsync<PagedResult<ControlAccountDto>>($"{BaseEndpoint}?{BuildQueryString(queryParams)}") 
                ?? new PagedResult<ControlAccountDto>(new List<ControlAccountDto>(), filter.PageNumber, filter.PageSize);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting control accounts");
            return new PagedResult<ControlAccountDto>(new List<ControlAccountDto>(), filter?.PageNumber ?? 1, filter?.PageSize ?? 10);
        }
    }

    public async Task<ControlAccountDetailDto?> GetControlAccountByIdAsync(Guid controlAccountId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting control account by ID: {controlAccountId}");
            return await _apiService.GetAsync<ControlAccountDetailDto>($"{BaseEndpoint}/{controlAccountId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting control account {controlAccountId}");
            return null;
        }
    }

    public async Task<List<ControlAccountDto>?> GetControlAccountsByPhaseAsync(Guid phaseId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting control accounts for phase: {phaseId}");
            return await _apiService.GetAsync<List<ControlAccountDto>>($"{BaseEndpoint}/phase/{phaseId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting control accounts for phase {phaseId}");
            return null;
        }
    }

    public async Task<List<ControlAccountAssignmentDto>?> GetControlAccountAssignmentsAsync(Guid controlAccountId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting control account assignments for ID: {controlAccountId}");
            return await _apiService.GetAsync<List<ControlAccountAssignmentDto>>($"{BaseEndpoint}/{controlAccountId}/assignments");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting control account assignments for {controlAccountId}");
            return null;
        }
    }

    public async Task<EVMSummaryDto?> GetLatestEVMSummaryAsync(Guid controlAccountId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting latest EVM summary for control account: {controlAccountId}");
            return await _apiService.GetAsync<EVMSummaryDto>($"{BaseEndpoint}/{controlAccountId}/evm-summary");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting EVM summary for control account {controlAccountId}");
            return null;
        }
    }

    // Command Operations
    public async Task<Guid?> CreateControlAccountAsync(CreateControlAccountDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Creating new control account: {dto.Code}");
            return await _apiService.PostAsync<CreateControlAccountDto, Guid>(BaseEndpoint, dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error creating control account: {dto.Code}");
            return null;
        }
    }

    public async Task<bool> UpdateControlAccountAsync(Guid controlAccountId, UpdateControlAccountDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Updating control account: {controlAccountId}");
            await _apiService.PutAsync<UpdateControlAccountDto, object>($"{BaseEndpoint}/{controlAccountId}", dto);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating control account: {controlAccountId}");
            return false;
        }
    }

    public async Task<bool> UpdateControlAccountStatusAsync(Guid controlAccountId, UpdateControlAccountStatusDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Updating control account status: {controlAccountId} to {dto.NewStatus}");
            await _apiService.PutAsync<UpdateControlAccountStatusDto, object>($"{BaseEndpoint}/{controlAccountId}/status", dto);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating control account status: {controlAccountId}");
            return false;
        }
    }

    public async Task<bool> DeleteControlAccountAsync(Guid controlAccountId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Deleting control account: {controlAccountId}");
            return await _apiService.DeleteAsync($"{BaseEndpoint}/{controlAccountId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting control account: {controlAccountId}");
            return false;
        }
    }

    // Assignment Operations
    public async Task<bool> AssignUserToControlAccountAsync(Guid controlAccountId, CreateControlAccountAssignmentDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Assigning user {dto.UserId} to control account: {controlAccountId}");
            await _apiService.PostAsync<CreateControlAccountAssignmentDto, object>($"{BaseEndpoint}/{controlAccountId}/assignments", dto);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error assigning user to control account: {controlAccountId}");
            return false;
        }
    }

    public async Task<bool> RemoveUserFromControlAccountAsync(Guid controlAccountId, string userToRemove, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Removing user {userToRemove} from control account: {controlAccountId}");
            return await _apiService.DeleteAsync($"{BaseEndpoint}/{controlAccountId}/assignments/{Uri.EscapeDataString(userToRemove)}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error removing user from control account: {controlAccountId}");
            return false;
        }
    }

    // Progress Operations
    public async Task<bool> UpdateControlAccountProgressAsync(Guid controlAccountId, decimal percentComplete, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Updating control account progress: {controlAccountId} to {percentComplete}%");
            await _apiService.PutAsync<object, object>($"{BaseEndpoint}/{controlAccountId}/progress", new { PercentComplete = percentComplete });
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating control account progress: {controlAccountId}");
            return false;
        }
    }

    // Workflow Operations
    public async Task<bool> BaselineControlAccountAsync(Guid controlAccountId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Baselining control account: {controlAccountId}");
            await _apiService.PostAsync<object, object>($"{BaseEndpoint}/{controlAccountId}/baseline", new { });
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error baselining control account: {controlAccountId}");
            return false;
        }
    }

    public async Task<bool> CloseControlAccountAsync(Guid controlAccountId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Closing control account: {controlAccountId}");
            await _apiService.PostAsync<object, object>($"{BaseEndpoint}/{controlAccountId}/close", new { });
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error closing control account: {controlAccountId}");
            return false;
        }
    }

    private static string BuildQueryString(Dictionary<string, string> parameters)
    {
        return string.Join("&", parameters.Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value)}"));
    }
}