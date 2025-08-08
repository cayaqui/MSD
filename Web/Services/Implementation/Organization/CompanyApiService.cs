using Core.DTOs.Common;
using Core.DTOs.Organization.Company;
using Web.Services.Interfaces;
using Web.Services.Interfaces.Organization;

namespace Web.Services.Implementation.Organization;

/// <summary>
/// Service implementation for company API operations
/// </summary>
public class CompanyApiService : ICompanyApiService
{
    private readonly IApiService _apiService;
    private readonly ILoggingService _logger;
    private const string BaseEndpoint = "api/companies";

    public CompanyApiService(IApiService apiService, ILoggingService logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    public async Task<PagedResult<CompanyDto>?> GetCompaniesAsync(int pageNumber = 1, int pageSize = 10, string? sortBy = null, bool isAscending = true, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting companies - Page: {pageNumber}, Size: {pageSize}");
            
            var queryParams = new Dictionary<string, string>
            {
                ["pageNumber"] = pageNumber.ToString(),
                ["pageSize"] = pageSize.ToString(),
                ["sortDirection"] = isAscending ? "asc" : "desc"
            };

            if (!string.IsNullOrEmpty(sortBy))
                queryParams["sortBy"] = sortBy;

            return await _apiService.GetAsync<PagedResult<CompanyDto>>($"{BaseEndpoint}?{BuildQueryString(queryParams)}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting companies");
            return null;
        }
    }
    
    public async Task<PagedResult<CompanyDto>?> GetCompaniesAsync(CompanyFilterDto filter, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting companies - Page: {filter.PageNumber}, Size: {filter.PageSize}");
            
            var queryParams = new Dictionary<string, string>
            {
                ["pageNumber"] = filter.PageNumber.ToString(),
                ["pageSize"] = filter.PageSize.ToString(),
                ["sortDirection"] = filter.SortDirection.ToString()
            };

            if (!string.IsNullOrEmpty(filter.SortBy))
                queryParams["sortBy"] = filter.SortBy;
            if (!string.IsNullOrEmpty(filter.SearchTerm))
                queryParams["searchTerm"] = filter.SearchTerm;

            return await _apiService.GetAsync<PagedResult<CompanyDto>>($"{BaseEndpoint}?{BuildQueryString(queryParams)}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting companies");
            return null;
        }
    }

    public async Task<CompanyDto?> GetCompanyByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting company by ID: {id}");
            return await _apiService.GetAsync<CompanyDto>($"{BaseEndpoint}/{id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting company {id}");
            return null;
        }
    }

    public async Task<CompanyWithOperationsDto?> GetCompanyOperationsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting company operations for ID: {id}");
            return await _apiService.GetAsync<CompanyWithOperationsDto>($"{BaseEndpoint}/{id}/operations");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting company operations for {id}");
            return null;
        }
    }

    public async Task<List<CompanyDto>?> GetActiveCompaniesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting active companies");
            return await _apiService.GetAsync<List<CompanyDto>>($"{BaseEndpoint}/active");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active companies");
            return null;
        }
    }

    public async Task<CompanyDto?> CreateCompanyAsync(CreateCompanyDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Creating new company: {dto.Name}");
            return await _apiService.PostAsync<CreateCompanyDto, CompanyDto>(BaseEndpoint, dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error creating company: {dto.Name}");
            return null;
        }
    }

    public async Task<CompanyDto?> UpdateCompanyAsync(Guid id, UpdateCompanyDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Updating company: {id}");
            return await _apiService.PutAsync<UpdateCompanyDto, CompanyDto>($"{BaseEndpoint}/{id}", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating company: {id}");
            return null;
        }
    }

    public async Task<bool> ActivateCompanyAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Activating company: {id}");
            var result = await _apiService.PostAsync<object, object>($"{BaseEndpoint}/{id}/activate", new { });
            return result != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error activating company: {id}");
            return false;
        }
    }

    public async Task<bool> DeactivateCompanyAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Deactivating company: {id}");
            var result = await _apiService.PostAsync<object, object>($"{BaseEndpoint}/{id}/deactivate", new { });
            return result != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deactivating company: {id}");
            return false;
        }
    }

    public async Task<bool> DeleteCompanyAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Deleting company: {id}");
            return await _apiService.DeleteAsync($"{BaseEndpoint}/{id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting company: {id}");
            return false;
        }
    }

    private static string BuildQueryString(Dictionary<string, string> parameters)
    {
        return string.Join("&", parameters.Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value)}"));
    }
}