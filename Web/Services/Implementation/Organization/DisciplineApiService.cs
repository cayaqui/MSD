using Core.DTOs.Common;
using Core.DTOs.Organization.Discipline;
using Core.Enums.Projects;
using Web.Services.Interfaces;
using Web.Services.Interfaces.Organization;

namespace Web.Services.Implementation.Organization;

/// <summary>
/// Service implementation for discipline API operations
/// </summary>
public class DisciplineApiService : IDisciplineApiService
{
    private readonly IApiService _apiService;
    private readonly ILoggingService _logger;
    private const string BaseEndpoint = "api/disciplines";

    public DisciplineApiService(IApiService apiService, ILoggingService logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    public async Task<PagedResult<DisciplineDto>?> GetDisciplinesAsync(int pageNumber = 1, int pageSize = 10, string? sortBy = null, bool isAscending = true, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting disciplines - Page: {pageNumber}, Size: {pageSize}");
            
            var queryParams = new Dictionary<string, string>
            {
                ["pageNumber"] = pageNumber.ToString(),
                ["pageSize"] = pageSize.ToString(),
                ["isAscending"] = isAscending.ToString()
            };

            if (!string.IsNullOrEmpty(sortBy))
                queryParams["sortBy"] = sortBy;

            return await _apiService.GetAsync<PagedResult<DisciplineDto>>($"{BaseEndpoint}?{BuildQueryString(queryParams)}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting disciplines");
            return null;
        }
    }

    public async Task<DisciplineDto?> GetDisciplineByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting discipline by ID: {id}");
            return await _apiService.GetAsync<DisciplineDto>($"{BaseEndpoint}/{id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting discipline {id}");
            return null;
        }
    }

    public async Task<List<DisciplineDto>?> GetActiveDisciplinesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting active disciplines");
            return await _apiService.GetAsync<List<DisciplineDto>>($"{BaseEndpoint}/active");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active disciplines");
            return null;
        }
    }

    public async Task<List<DisciplineDto>?> GetDisciplinesByTypeAsync(DisciplineType type, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting disciplines by type: {type}");
            return await _apiService.GetAsync<List<DisciplineDto>>($"{BaseEndpoint}/by-type/{type}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting disciplines by type {type}");
            return null;
        }
    }

    public async Task<DisciplineDto?> CreateDisciplineAsync(CreateDisciplineDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Creating new discipline: {dto.Name}");
            return await _apiService.PostAsync<CreateDisciplineDto, DisciplineDto>(BaseEndpoint, dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error creating discipline: {dto.Name}");
            return null;
        }
    }

    public async Task<DisciplineDto?> UpdateDisciplineAsync(Guid id, UpdateDisciplineDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Updating discipline: {id}");
            return await _apiService.PutAsync<UpdateDisciplineDto, DisciplineDto>($"{BaseEndpoint}/{id}", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating discipline: {id}");
            return null;
        }
    }

    public async Task<bool> DeleteDisciplineAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Deleting discipline: {id}");
            return await _apiService.DeleteAsync($"{BaseEndpoint}/{id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting discipline: {id}");
            return false;
        }
    }

    private static string BuildQueryString(Dictionary<string, string> parameters)
    {
        return string.Join("&", parameters.Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value)}"));
    }
}