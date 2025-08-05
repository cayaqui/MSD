using Core.DTOs.Common;
using Core.DTOs.Organization.Operation;
using Web.Services.Interfaces;
using Web.Services.Interfaces.Organization;

namespace Web.Services.Implementation.Organization;

/// <summary>
/// Service implementation for operation API operations
/// </summary>
public class OperationApiService : IOperationApiService
{
    private readonly IApiService _apiService;
    private readonly ILoggingService _logger;
    private const string BaseEndpoint = "api/operations";

    public OperationApiService(IApiService apiService, ILoggingService logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    public async Task<PagedResult<OperationDto>?> GetOperationsAsync(int pageNumber = 1, int pageSize = 10, string? sortBy = null, bool isAscending = true, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting operations - Page: {pageNumber}, Size: {pageSize}");
            
            var queryParams = new Dictionary<string, string>
            {
                ["pageNumber"] = pageNumber.ToString(),
                ["pageSize"] = pageSize.ToString(),
                ["isAscending"] = isAscending.ToString()
            };

            if (!string.IsNullOrEmpty(sortBy))
                queryParams["sortBy"] = sortBy;

            return await _apiService.GetAsync<PagedResult<OperationDto>>($"{BaseEndpoint}?{BuildQueryString(queryParams)}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting operations");
            return null;
        }
    }

    public async Task<OperationDto?> GetOperationByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting operation by ID: {id}");
            return await _apiService.GetAsync<OperationDto>($"{BaseEndpoint}/{id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting operation {id}");
            return null;
        }
    }

    public async Task<List<OperationDto>?> GetOperationsByCompanyAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting operations for company: {companyId}");
            return await _apiService.GetAsync<List<OperationDto>>($"{BaseEndpoint}/company/{companyId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting operations for company {companyId}");
            return null;
        }
    }

    public async Task<OperationWithProjectsDto?> GetOperationProjectsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting operation projects for ID: {id}");
            return await _apiService.GetAsync<OperationWithProjectsDto>($"{BaseEndpoint}/{id}/projects");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting operation projects for {id}");
            return null;
        }
    }

    public async Task<OperationDto?> CreateOperationAsync(CreateOperationDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Creating new operation: {dto.Name}");
            return await _apiService.PostAsync<CreateOperationDto, OperationDto>(BaseEndpoint, dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error creating operation: {dto.Name}");
            return null;
        }
    }

    public async Task<OperationDto?> UpdateOperationAsync(Guid id, UpdateOperationDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Updating operation: {id}");
            return await _apiService.PutAsync<UpdateOperationDto, OperationDto>($"{BaseEndpoint}/{id}", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating operation: {id}");
            return null;
        }
    }

    public async Task<bool> ActivateOperationAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Activating operation: {id}");
            var result = await _apiService.PostAsync<object, object>($"{BaseEndpoint}/{id}/activate", new { });
            return result != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error activating operation: {id}");
            return false;
        }
    }

    public async Task<bool> DeactivateOperationAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Deactivating operation: {id}");
            var result = await _apiService.PostAsync<object, object>($"{BaseEndpoint}/{id}/deactivate", new { });
            return result != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deactivating operation: {id}");
            return false;
        }
    }

    public async Task<bool> DeleteOperationAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Deleting operation: {id}");
            return await _apiService.DeleteAsync($"{BaseEndpoint}/{id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting operation: {id}");
            return false;
        }
    }

    private static string BuildQueryString(Dictionary<string, string> parameters)
    {
        return string.Join("&", parameters.Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value)}"));
    }
}