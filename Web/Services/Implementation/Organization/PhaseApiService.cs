using Core.DTOs.Common;
using Web.Services.Interfaces;
using Web.Services.Interfaces.Organization;

namespace Web.Services.Implementation.Organization;

/// <summary>
/// Service implementation for phase API operations
/// </summary>
public class PhaseApiService : IPhaseApiService
{
    private readonly IApiService _apiService;
    private readonly ILoggingService _logger;
    private const string BaseEndpoint = "api/phases";

    public PhaseApiService(IApiService apiService, ILoggingService logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    public async Task<PagedResult<Core.DTOs.Organization.Phase.PhaseDto>?> GetPhasesAsync(int pageNumber = 1, int pageSize = 10, string? sortBy = null, bool isAscending = true, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting phases - Page: {pageNumber}, Size: {pageSize}");
            
            var queryParams = new Dictionary<string, string>
            {
                ["pageNumber"] = pageNumber.ToString(),
                ["pageSize"] = pageSize.ToString(),
                ["isAscending"] = isAscending.ToString()
            };

            if (!string.IsNullOrEmpty(sortBy))
                queryParams["sortBy"] = sortBy;

            return await _apiService.GetAsync<PagedResult<Core.DTOs.Organization.Phase.PhaseDto>>($"{BaseEndpoint}?{BuildQueryString(queryParams)}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting phases");
            return null;
        }
    }

    public async Task<Core.DTOs.Organization.Phase.PhaseDto?> GetPhaseByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting phase by ID: {id}");
            return await _apiService.GetAsync<Core.DTOs.Organization.Phase.PhaseDto>($"{BaseEndpoint}/{id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting phase {id}");
            return null;
        }
    }

    public async Task<List<Core.DTOs.Organization.Phase.PhaseDto>?> GetPhasesByProjectAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting phases for project: {projectId}");
            return await _apiService.GetAsync<List<Core.DTOs.Organization.Phase.PhaseDto>>($"{BaseEndpoint}/project/{projectId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting phases for project {projectId}");
            return null;
        }
    }

    public async Task<Core.DTOs.Organization.Phase.PhaseWithMilestonesDto?> GetPhaseWithMilestonesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting phase with milestones for ID: {id}");
            return await _apiService.GetAsync<Core.DTOs.Organization.Phase.PhaseWithMilestonesDto>($"{BaseEndpoint}/{id}/milestones");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting phase milestones for {id}");
            return null;
        }
    }

    public async Task<Core.DTOs.Organization.Phase.PhaseWithDeliverablesDto?> GetPhaseWithDeliverablesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting phase with deliverables for ID: {id}");
            return await _apiService.GetAsync<Core.DTOs.Organization.Phase.PhaseWithDeliverablesDto>($"{BaseEndpoint}/{id}/deliverables");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting phase deliverables for {id}");
            return null;
        }
    }

    public async Task<Core.DTOs.Organization.Phase.PhaseDto?> CreatePhaseAsync(Core.DTOs.Organization.Phase.CreatePhaseDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Creating new phase: {dto.Name}");
            return await _apiService.PostAsync<Core.DTOs.Organization.Phase.CreatePhaseDto, Core.DTOs.Organization.Phase.PhaseDto>(BaseEndpoint, dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error creating phase: {dto.Name}");
            return null;
        }
    }

    public async Task<Core.DTOs.Organization.Phase.PhaseDto?> UpdatePhaseAsync(Guid id, Core.DTOs.Organization.Phase.UpdatePhaseDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Updating phase: {id}");
            return await _apiService.PutAsync<Core.DTOs.Organization.Phase.UpdatePhaseDto, Core.DTOs.Organization.Phase.PhaseDto>($"{BaseEndpoint}/{id}", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating phase: {id}");
            return null;
        }
    }

    public async Task<bool> UpdatePhaseScheduleAsync(Guid id, Core.DTOs.Organization.Phase.UpdatePhaseScheduleDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Updating phase schedule: {id}");
            var result = await _apiService.PutAsync<Core.DTOs.Organization.Phase.UpdatePhaseScheduleDto, object>($"{BaseEndpoint}/{id}/schedule", dto);
            return result != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating phase schedule: {id}");
            return false;
        }
    }

    public async Task<bool> UpdatePhaseBudgetAsync(Guid id, Core.DTOs.Organization.Phase.UpdatePhaseBudgetDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Updating phase budget: {id}");
            var result = await _apiService.PutAsync<Core.DTOs.Organization.Phase.UpdatePhaseBudgetDto, object>($"{BaseEndpoint}/{id}/budget", dto);
            return result != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating phase budget: {id}");
            return false;
        }
    }

    public async Task<bool> ApprovePhaseGateAsync(Guid id, Core.DTOs.Organization.Phase.ApprovePhaseGateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Approving phase gate: {id}");
            var result = await _apiService.PostAsync<Core.DTOs.Organization.Phase.ApprovePhaseGateDto, object>($"{BaseEndpoint}/{id}/approve-gate", dto);
            return result != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error approving phase gate: {id}");
            return false;
        }
    }

    public async Task<bool> DeletePhaseAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Deleting phase: {id}");
            return await _apiService.DeleteAsync($"{BaseEndpoint}/{id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting phase: {id}");
            return false;
        }
    }

    private static string BuildQueryString(Dictionary<string, string> parameters)
    {
        return string.Join("&", parameters.Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value)}"));
    }
}