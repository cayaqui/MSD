using Core.DTOs.Common;
using Core.DTOs.Cost;
using Core.DTOs.Cost.CostControlReports;
using Core.DTOs.Cost.CostItems;
using Core.DTOs.Cost.PlanningPackages;
using Core.DTOs.Projects.WorkPackageDetails;
using Web.Services.Interfaces;
using Web.Services.Interfaces.Cost;

namespace Web.Services.Implementation.Cost;

public class CostApiService : ICostApiService
{
    private readonly IApiService _apiService;
    private readonly ILoggingService _logger;
    private const string BaseEndpoint = "api/costs";

    public CostApiService(IApiService apiService, ILoggingService logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    // Cost Items
    public async Task<PagedResult<CostItemDto>> GetCostItemsAsync(Guid projectId, Core.DTOs.Cost.CostQueryParameters parameters)
    {
        try
        {
            var queryParams = new Dictionary<string, string>
            {
                ["pageNumber"] = parameters.PageNumber.ToString(),
                ["pageSize"] = parameters.PageSize.ToString()
            };

            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
                queryParams["searchTerm"] = parameters.SearchTerm;
            if (parameters.Category.HasValue)
                queryParams["category"] = parameters.Category.Value.ToString();
            if (parameters.ControlAccountId.HasValue)
                queryParams["controlAccountId"] = parameters.ControlAccountId.Value.ToString();
            // StartDate and EndDate not available in CostQueryParameters
            
            // IncludeCommitments and IncludeActuals not available in CostQueryParameters

            var queryString = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));
            var result = await _apiService.GetAsync<PagedResult<CostItemDto>>($"{BaseEndpoint}/items/project/{projectId}?{queryString}");
            return result ?? new PagedResult<CostItemDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting cost items for project {projectId}");
            return new PagedResult<CostItemDto>();
        }
    }

    public async Task<CostItemDetailDto?> GetCostItemByIdAsync(Guid id)
    {
        try
        {
            return await _apiService.GetAsync<CostItemDetailDto>($"{BaseEndpoint}/items/{id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting cost item {id}");
            return null;
        }
    }

    public async Task<Guid?> CreateCostItemAsync(CreateCostItemDto dto)
    {
        try
        {
            return await _apiService.PostAsync<CreateCostItemDto, Guid>($"{BaseEndpoint}/items", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating cost item");
            return null;
        }
    }

    public async Task<bool> UpdateCostItemAsync(Guid id, UpdateCostItemDto dto)
    {
        try
        {
            await _apiService.PutAsync($"{BaseEndpoint}/items/{id}", dto);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating cost item {id}");
            return false;
        }
    }

    public async Task<bool> DeleteCostItemAsync(Guid id)
    {
        try
        {
            await _apiService.DeleteAsync($"{BaseEndpoint}/items/{id}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting cost item {id}");
            return false;
        }
    }

    // Cost Recording
    public async Task<bool> RecordActualCostAsync(Guid id, RecordActualCostDto dto)
    {
        try
        {
            await _apiService.PostAsync($"{BaseEndpoint}/items/{id}/actual", dto);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error recording actual cost for item {id}");
            return false;
        }
    }

    public async Task<bool> RecordCommitmentAsync(Guid id, RecordCommitmentDto dto)
    {
        try
        {
            await _apiService.PostAsync($"{BaseEndpoint}/items/{id}/commitment", dto);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error recording commitment for item {id}");
            return false;
        }
    }

    public async Task<bool> ApproveCostItemAsync(Guid id)
    {
        try
        {
            await _apiService.PostAsync<object>($"{BaseEndpoint}/items/{id}/approve", new { });
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error approving cost item {id}");
            return false;
        }
    }

    // Reporting
    public async Task<ProjectCostReportDto?> GetProjectCostReportAsync(Guid projectId)
    {
        try
        {
            return await _apiService.GetAsync<ProjectCostReportDto>($"{BaseEndpoint}/reports/project/{projectId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting project cost report for project {projectId}");
            return null;
        }
    }

    public async Task<List<CostSummaryByCategoryDto>> GetCostSummaryByCategoryAsync(Guid projectId)
    {
        try
        {
            var result = await _apiService.GetAsync<List<CostSummaryByCategoryDto>>($"{BaseEndpoint}/reports/project/{projectId}/by-category");
            return result ?? new List<CostSummaryByCategoryDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting cost summary by category for project {projectId}");
            return new List<CostSummaryByCategoryDto>();
        }
    }

    public async Task<List<CostSummaryByControlAccountDto>> GetCostSummaryByControlAccountAsync(Guid projectId)
    {
        try
        {
            var result = await _apiService.GetAsync<List<CostSummaryByControlAccountDto>>($"{BaseEndpoint}/reports/project/{projectId}/by-control-account");
            return result ?? new List<CostSummaryByControlAccountDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting cost summary by control account for project {projectId}");
            return new List<CostSummaryByControlAccountDto>();
        }
    }

    // Planning Packages (legacy endpoints)
    public async Task<PagedResult<PlanningPackageDto>> GetPlanningPackagesAsync(Guid controlAccountId, QueryParameters parameters)
    {
        try
        {
            var queryParams = new Dictionary<string, string>
            {
                ["pageNumber"] = parameters.PageNumber.ToString(),
                ["pageSize"] = parameters.PageSize.ToString()
            };

            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
                queryParams["searchTerm"] = parameters.SearchTerm;

            var queryString = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));
            var result = await _apiService.GetAsync<PagedResult<PlanningPackageDto>>($"{BaseEndpoint}/planning-packages/{controlAccountId}?{queryString}");
            return result ?? new PagedResult<PlanningPackageDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting planning packages for control account {controlAccountId}");
            return new PagedResult<PlanningPackageDto>();
        }
    }

    public async Task<Guid?> CreatePlanningPackageAsync(CreatePlanningPackageDto dto)
    {
        try
        {
            return await _apiService.PostAsync<CreatePlanningPackageDto, Guid>($"{BaseEndpoint}/planning-packages", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating planning package");
            return null;
        }
    }

    public async Task<bool> UpdatePlanningPackageAsync(Guid id, UpdatePlanningPackageDto dto)
    {
        try
        {
            await _apiService.PutAsync($"{BaseEndpoint}/planning-packages/{id}", dto);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating planning package {id}");
            return false;
        }
    }

    public async Task<bool> ConvertPlanningPackageToWorkPackagesAsync(Guid id, ConvertPlanningToWorkPackageDto dto)
    {
        try
        {
            await _apiService.PostAsync($"{BaseEndpoint}/planning-packages/{id}/convert", dto);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error converting planning package {id} to work packages");
            return false;
        }
    }
}