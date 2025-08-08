using Core.DTOs.Common;
using Core.DTOs.Cost.PlanningPackages;
using Core.DTOs.Projects.WorkPackageDetails;
using Web.Services.Interfaces;
using Web.Services.Interfaces.Projects;

namespace Web.Services.Implementation.Projects;

public class PlanningPackageApiService : IPlanningPackageApiService
{
    private readonly IApiService _apiService;
    private readonly ILoggingService _logger;
    private const string BaseEndpoint = "api/planning-packages";

    public PlanningPackageApiService(IApiService apiService, ILoggingService logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    public async Task<PagedResult<PlanningPackageDto>> GetPlanningPackagesAsync(Guid projectId, QueryParameters parameters)
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
            if (!string.IsNullOrWhiteSpace(parameters.SortBy))
                queryParams["sortBy"] = parameters.SortBy;
            if (!string.IsNullOrWhiteSpace(parameters.SortDirection))
                queryParams["sortDirection"] = parameters.SortDirection;

            var queryString = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));
            var result = await _apiService.GetAsync<PagedResult<PlanningPackageDto>>($"{BaseEndpoint}/project/{projectId}?{queryString}");
            return result ?? new PagedResult<PlanningPackageDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting planning packages for project {projectId}");
            return new PagedResult<PlanningPackageDto>();
        }
    }

    public async Task<PlanningPackageDto?> GetPlanningPackageByIdAsync(Guid id)
    {
        try
        {
            return await _apiService.GetAsync<PlanningPackageDto>($"{BaseEndpoint}/{id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting planning package {id}");
            return null;
        }
    }

    public async Task<List<PlanningPackageDto>> GetPlanningPackagesByControlAccountAsync(Guid controlAccountId)
    {
        try
        {
            var result = await _apiService.GetAsync<List<PlanningPackageDto>>($"{BaseEndpoint}/control-account/{controlAccountId}");
            return result ?? new List<PlanningPackageDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting planning packages for control account {controlAccountId}");
            return new List<PlanningPackageDto>();
        }
    }

    public async Task<List<PlanningPackageDto>> GetUnconvertedPlanningPackagesAsync(Guid projectId)
    {
        try
        {
            var result = await _apiService.GetAsync<List<PlanningPackageDto>>($"{BaseEndpoint}/project/{projectId}/unconverted");
            return result ?? new List<PlanningPackageDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting unconverted planning packages for project {projectId}");
            return new List<PlanningPackageDto>();
        }
    }

    public async Task<Guid?> CreatePlanningPackageAsync(CreatePlanningPackageDto dto)
    {
        try
        {
            return await _apiService.PostAsync<CreatePlanningPackageDto, Guid>(BaseEndpoint, dto);
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
            await _apiService.PutAsync($"{BaseEndpoint}/{id}", dto);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating planning package {id}");
            return false;
        }
    }

    public async Task<bool> ConvertToWorkPackageAsync(Guid id, ConvertPlanningToWorkPackageDto dto)
    {
        try
        {
            await _apiService.PostAsync($"{BaseEndpoint}/{id}/convert-to-work-package", dto);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error converting planning package {id} to work package");
            return false;
        }
    }

    public async Task<bool> DeletePlanningPackageAsync(Guid id)
    {
        try
        {
            await _apiService.DeleteAsync($"{BaseEndpoint}/{id}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting planning package {id}");
            return false;
        }
    }

    public async Task<PlanningPackageBulkOperationResult> BulkCreatePlanningPackagesAsync(List<CreatePlanningPackageDto> planningPackages)
    {
        try
        {
            var dto = new { PlanningPackages = planningPackages };
            var result = await _apiService.PostAsync<object, PlanningPackageBulkOperationResult>($"{BaseEndpoint}/bulk-create", dto);
            return result ?? new PlanningPackageBulkOperationResult(0, planningPackages.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bulk creating planning packages");
            return new PlanningPackageBulkOperationResult(0, planningPackages.Count);
        }
    }

    public async Task<PlanningPackageBulkOperationResult> BulkConvertToWorkPackagesAsync(List<Guid> planningPackageIds, ConvertPlanningToWorkPackageDto conversionDetails)
    {
        try
        {
            var dto = new { PlanningPackageIds = planningPackageIds, ConversionDetails = conversionDetails };
            var result = await _apiService.PostAsync<object, PlanningPackageBulkOperationResult>($"{BaseEndpoint}/bulk-convert", dto);
            return result ?? new PlanningPackageBulkOperationResult(0, planningPackageIds.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bulk converting planning packages");
            return new PlanningPackageBulkOperationResult(0, planningPackageIds.Count);
        }
    }

    public async Task<PlanningPackageBudgetSummaryDto> GetPlanningPackageBudgetSummaryAsync(Guid controlAccountId)
    {
        try
        {
            var result = await _apiService.GetAsync<PlanningPackageBudgetSummaryDto>($"{BaseEndpoint}/control-account/{controlAccountId}/budget-summary");
            return result ?? new PlanningPackageBudgetSummaryDto
            {
                ControlAccountId = controlAccountId,
                TotalBudget = 0,
                AllocatedBudget = 0,
                UnallocatedBudget = 0,
                PlanningPackageCount = 0
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting budget summary for control account {controlAccountId}");
            return new PlanningPackageBudgetSummaryDto
            {
                ControlAccountId = controlAccountId,
                TotalBudget = 0,
                AllocatedBudget = 0,
                UnallocatedBudget = 0,
                PlanningPackageCount = 0
            };
        }
    }

    public async Task<bool> RedistributeBudgetAsync(Guid controlAccountId, List<BudgetAllocation> allocations)
    {
        try
        {
            var dto = new { ControlAccountId = controlAccountId, Allocations = allocations };
            await _apiService.PostAsync($"{BaseEndpoint}/redistribute-budget", dto);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error redistributing budget for control account {controlAccountId}");
            return false;
        }
    }

    public async Task<byte[]> ExportPlanningPackagesAsync(Guid projectId)
    {
        try
        {
            return await _apiService.GetBytesAsync($"{BaseEndpoint}/project/{projectId}/export");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error exporting planning packages for project {projectId}");
            return Array.Empty<byte>();
        }
    }
}