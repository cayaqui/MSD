using Core.DTOs.Common;
using Core.DTOs.Organization.Contractor;
using Core.Enums.Projects;
using Web.Services.Interfaces;
using Web.Services.Interfaces.Organization;

namespace Web.Services.Implementation.Organization;

/// <summary>
/// Service implementation for contractor API operations
/// </summary>
public class ContractorApiService : IContractorApiService
{
    private readonly IApiService _apiService;
    private readonly ILoggingService _logger;
    private const string BaseEndpoint = "api/contractors";

    public ContractorApiService(IApiService apiService, ILoggingService logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    public async Task<PagedResult<ContractorDto>?> GetContractorsAsync(int pageNumber = 1, int pageSize = 10, string? sortBy = null, bool isAscending = true, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting contractors - Page: {pageNumber}, Size: {pageSize}");
            
            var queryParams = new Dictionary<string, string>
            {
                ["pageNumber"] = pageNumber.ToString(),
                ["pageSize"] = pageSize.ToString(),
                ["isAscending"] = isAscending.ToString()
            };

            if (!string.IsNullOrEmpty(sortBy))
                queryParams["sortBy"] = sortBy;

            return await _apiService.GetAsync<PagedResult<ContractorDto>>($"{BaseEndpoint}?{BuildQueryString(queryParams)}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting contractors");
            return null;
        }
    }

    public async Task<ContractorDto?> GetContractorByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting contractor by ID: {id}");
            return await _apiService.GetAsync<ContractorDto>($"{BaseEndpoint}/{id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting contractor {id}");
            return null;
        }
    }

    public async Task<List<ContractorDto>?> GetActiveContractorsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting active contractors");
            return await _apiService.GetAsync<List<ContractorDto>>($"{BaseEndpoint}/active");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active contractors");
            return null;
        }
    }

    public async Task<List<ContractorDto>?> GetContractorsByTypeAsync(ContractorType type, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting contractors by type: {type}");
            return await _apiService.GetAsync<List<ContractorDto>>($"{BaseEndpoint}/by-type/{type}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting contractors by type {type}");
            return null;
        }
    }

    public async Task<ContractorWithProjectsDto?> GetContractorProjectsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting contractor projects for ID: {id}");
            return await _apiService.GetAsync<ContractorWithProjectsDto>($"{BaseEndpoint}/{id}/projects");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting contractor projects for {id}");
            return null;
        }
    }

    public async Task<ContractorPerformanceDto?> GetContractorPerformanceAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting contractor performance for ID: {id}");
            return await _apiService.GetAsync<ContractorPerformanceDto>($"{BaseEndpoint}/{id}/performance");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting contractor performance for {id}");
            return null;
        }
    }

    public async Task<ContractorDto?> CreateContractorAsync(CreateContractorDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Creating new contractor: {dto.Name}");
            return await _apiService.PostAsync<CreateContractorDto, ContractorDto>(BaseEndpoint, dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error creating contractor: {dto.Name}");
            return null;
        }
    }

    public async Task<ContractorDto?> UpdateContractorAsync(Guid id, UpdateContractorDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Updating contractor: {id}");
            return await _apiService.PutAsync<UpdateContractorDto, ContractorDto>($"{BaseEndpoint}/{id}", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating contractor: {id}");
            return null;
        }
    }

    public async Task<bool> PrequalifyContractorAsync(Guid id, PrequalifyContractorDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Prequalifying contractor: {id}");
            var result = await _apiService.PostAsync<PrequalifyContractorDto, object>($"{BaseEndpoint}/{id}/prequalify", dto);
            return result != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error prequalifying contractor: {id}");
            return false;
        }
    }

    public async Task<bool> BlacklistContractorAsync(Guid id, BlacklistContractorDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Blacklisting contractor: {id}");
            var result = await _apiService.PostAsync<BlacklistContractorDto, object>($"{BaseEndpoint}/{id}/blacklist", dto);
            return result != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error blacklisting contractor: {id}");
            return false;
        }
    }

    public async Task<bool> DeleteContractorAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Deleting contractor: {id}");
            return await _apiService.DeleteAsync($"{BaseEndpoint}/{id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting contractor: {id}");
            return false;
        }
    }

    private static string BuildQueryString(Dictionary<string, string> parameters)
    {
        return string.Join("&", parameters.Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value)}"));
    }
}