using Core.DTOs.Common;
using Core.DTOs.Cost.Budgets;
using Core.DTOs.Cost.BudgetItems;
using Core.DTOs.Cost.BudgetRevisions;
using Web.Services.Interfaces;
using Web.Services.Interfaces.Cost;

namespace Web.Services.Implementation.Cost;

/// <summary>
/// Service implementation for budget API operations
/// </summary>
public class BudgetApiService : IBudgetApiService
{
    private readonly IApiService _apiService;
    private readonly ILoggingService _logger;
    private const string BaseEndpoint = "api/budgets";

    public BudgetApiService(IApiService apiService, ILoggingService logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    // Query Operations
    public async Task<PagedResult<BudgetDto>> GetBudgetsAsync(BudgetFilterDto? filter = null, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting budgets - Page: {filter?.PageNumber}, Size: {filter?.PageSize}");
            filter ??= new BudgetFilterDto();
            
            var queryParams = new Dictionary<string, string>
            {
                ["pageNumber"] = filter.PageNumber.ToString(),
                ["pageSize"] = filter.PageSize.ToString(),
                ["sortDirection"] = filter.SortDirection.ToString()
            };

            if (!string.IsNullOrEmpty(filter.SortBy))
                queryParams["sortBy"] = filter.SortBy;

            var projectEndpoint = filter.ProjectId.HasValue 
                ? $"{BaseEndpoint}/project/{filter.ProjectId.Value}?{BuildQueryString(queryParams)}"
                : $"{BaseEndpoint}?{BuildQueryString(queryParams)}";

            return await _apiService.GetAsync<PagedResult<BudgetDto>>(projectEndpoint) 
                ?? new PagedResult<BudgetDto>(new List<BudgetDto>(), filter.PageNumber, filter.PageSize);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting budgets");
            return new PagedResult<BudgetDto>(new List<BudgetDto>(), filter?.PageNumber ?? 1, filter?.PageSize ?? 10);
        }
    }

    public async Task<BudgetDto?> GetBudgetByIdAsync(Guid budgetId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting budget by ID: {budgetId}");
            return await _apiService.GetAsync<BudgetDto>($"{BaseEndpoint}/{budgetId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting budget {budgetId}");
            return null;
        }
    }

    public async Task<BudgetDetailDto?> GetCurrentBaselineBudgetAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting current baseline budget for project: {projectId}");
            return await _apiService.GetAsync<BudgetDetailDto>($"{BaseEndpoint}/project/{projectId}/baseline");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting baseline budget for project {projectId}");
            return null;
        }
    }

    public async Task<List<BudgetItemDto>?> GetBudgetItemsAsync(Guid budgetId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting budget items for budget: {budgetId}");
            return await _apiService.GetAsync<List<BudgetItemDto>>($"{BaseEndpoint}/{budgetId}/items");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting budget items for {budgetId}");
            return null;
        }
    }

    // Command Operations
    public async Task<BudgetDto?> CreateBudgetAsync(CreateBudgetDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Creating new budget: {dto.Name}");
            return await _apiService.PostAsync<CreateBudgetDto, BudgetDto>(BaseEndpoint, dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error creating budget: {dto.Name}");
            return null;
        }
    }

    public async Task<BudgetDto?> UpdateBudgetAsync(Guid budgetId, UpdateBudgetDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Updating budget: {budgetId}");
            return await _apiService.PutAsync<UpdateBudgetDto, BudgetDto>($"{BaseEndpoint}/{budgetId}", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating budget: {budgetId}");
            return null;
        }
    }

    public async Task<bool> DeleteBudgetAsync(Guid budgetId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Deleting budget: {budgetId}");
            return await _apiService.DeleteAsync($"{BaseEndpoint}/{budgetId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting budget: {budgetId}");
            return false;
        }
    }

    // Workflow Operations
    public async Task<bool> SubmitBudgetForApprovalAsync(Guid budgetId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Submitting budget for approval: {budgetId}");
            await _apiService.PostAsync<object, object>($"{BaseEndpoint}/{budgetId}/submit", new { });
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error submitting budget for approval: {budgetId}");
            return false;
        }
    }

    public async Task<bool> ApproveBudgetAsync(Guid budgetId, ApproveBudgetDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Approving budget: {budgetId}");
            await _apiService.PostAsync<ApproveBudgetDto, object>($"{BaseEndpoint}/{budgetId}/approve", dto);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error approving budget: {budgetId}");
            return false;
        }
    }

    public async Task<bool> RejectBudgetAsync(Guid budgetId, RejectBudgetDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Rejecting budget: {budgetId}");
            await _apiService.PostAsync<RejectBudgetDto, object>($"{BaseEndpoint}/{budgetId}/reject", dto);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error rejecting budget: {budgetId}");
            return false;
        }
    }

    public async Task<bool> SetBudgetAsBaselineAsync(Guid budgetId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Setting budget as baseline: {budgetId}");
            await _apiService.PostAsync<object, object>($"{BaseEndpoint}/{budgetId}/baseline", new { });
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error setting budget as baseline: {budgetId}");
            return false;
        }
    }

    public async Task<bool> LockBudgetAsync(Guid budgetId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Locking budget: {budgetId}");
            await _apiService.PostAsync<object, object>($"{BaseEndpoint}/{budgetId}/lock", new { });
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error locking budget: {budgetId}");
            return false;
        }
    }

    // Budget Item Operations
    public async Task<Guid?> AddBudgetItemAsync(CreateBudgetItemDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Adding budget item: {dto.Description}");
            return await _apiService.PostAsync<CreateBudgetItemDto, Guid>($"{BaseEndpoint}/items", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error adding budget item: {dto.Description}");
            return null;
        }
    }

    public async Task<bool> UpdateBudgetItemAsync(Guid itemId, decimal quantity, decimal unitRate, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Updating budget item: {itemId}");
            await _apiService.PutAsync<object, object>($"{BaseEndpoint}/items/{itemId}", new { Quantity = quantity, UnitRate = unitRate });
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating budget item: {itemId}");
            return false;
        }
    }

    public async Task<bool> RemoveBudgetItemAsync(Guid itemId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Removing budget item: {itemId}");
            return await _apiService.DeleteAsync($"{BaseEndpoint}/items/{itemId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error removing budget item: {itemId}");
            return false;
        }
    }

    // Revision Operations
    public async Task<Guid?> CreateBudgetRevisionAsync(Guid budgetId, CreateBudgetRevisionDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Creating budget revision for budget: {budgetId}");
            return await _apiService.PostAsync<CreateBudgetRevisionDto, Guid>($"{BaseEndpoint}/{budgetId}/revisions", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error creating budget revision for {budgetId}");
            return null;
        }
    }

    public async Task<bool> ApproveBudgetRevisionAsync(Guid revisionId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Approving budget revision: {revisionId}");
            await _apiService.PostAsync<object, object>($"{BaseEndpoint}/revisions/{revisionId}/approve", new { });
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error approving budget revision: {revisionId}");
            return false;
        }
    }

    private static string BuildQueryString(Dictionary<string, string> parameters)
    {
        return string.Join("&", parameters.Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value)}"));
    }
}