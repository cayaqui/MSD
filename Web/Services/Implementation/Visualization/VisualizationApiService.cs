using Core.DTOs.Visualization;
using Web.Services.Interfaces.Visualization;
using Web.Services.Interfaces;
using System.Net.Http.Json;

namespace Web.Services.Implementation.Visualization;

public class VisualizationApiService : IVisualizationApiService
{
    private readonly IApiService _apiService;
    private readonly ILoggingService _logger;

    public VisualizationApiService(IApiService apiService, ILoggingService logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    public async Task<GanttChartDto?> GenerateProjectGanttAsync(Guid projectId, GanttConfigDto? config = null)
    {
        try
        {
            var endpoint = $"visualization/gantt/{projectId}";
            if (config != null)
            {
                return await _apiService.PostAsync<GanttConfigDto, GanttChartDto>(endpoint, config);
            }
            else
            {
                return await _apiService.GetAsync<GanttChartDto>(endpoint);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating project Gantt chart");
            return null;
        }
    }

    public async Task<CostSCurveDto?> GenerateCostSCurveAsync(Guid projectId, DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var queryParams = new List<string>();
            if (startDate.HasValue) queryParams.Add($"startDate={startDate.Value:yyyy-MM-dd}");
            if (endDate.HasValue) queryParams.Add($"endDate={endDate.Value:yyyy-MM-dd}");
            
            var endpoint = $"visualization/cost-scurve/{projectId}";
            if (queryParams.Any()) endpoint += "?" + string.Join("&", queryParams);
            
            return await _apiService.GetAsync<CostSCurveDto>(endpoint);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating cost S-curve");
            return null;
        }
    }

    public async Task<ProgressSCurveDto?> GenerateProgressSCurveAsync(Guid projectId, DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var queryParams = new List<string>();
            if (startDate.HasValue) queryParams.Add($"startDate={startDate.Value:yyyy-MM-dd}");
            if (endDate.HasValue) queryParams.Add($"endDate={endDate.Value:yyyy-MM-dd}");
            
            var endpoint = $"visualization/progress-scurve/{projectId}";
            if (queryParams.Any()) endpoint += "?" + string.Join("&", queryParams);
            
            return await _apiService.GetAsync<ProgressSCurveDto>(endpoint);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating progress S-curve");
            return null;
        }
    }

    public async Task<EVMChartDto?> GenerateEVMChartAsync(Guid projectId, DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var queryParams = new List<string>();
            if (startDate.HasValue) queryParams.Add($"startDate={startDate.Value:yyyy-MM-dd}");
            if (endDate.HasValue) queryParams.Add($"endDate={endDate.Value:yyyy-MM-dd}");
            
            var endpoint = $"visualization/evm/{projectId}";
            if (queryParams.Any()) endpoint += "?" + string.Join("&", queryParams);
            
            return await _apiService.GetAsync<EVMChartDto>(endpoint);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating EVM chart");
            return null;
        }
    }

    public async Task<ResourceUtilizationChartDto?> GenerateResourceUtilizationAsync(Guid projectId, DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var queryParams = new List<string>();
            if (startDate.HasValue) queryParams.Add($"startDate={startDate.Value:yyyy-MM-dd}");
            if (endDate.HasValue) queryParams.Add($"endDate={endDate.Value:yyyy-MM-dd}");
            
            var endpoint = $"visualization/resource-utilization/{projectId}";
            if (queryParams.Any()) endpoint += "?" + string.Join("&", queryParams);
            
            return await _apiService.GetAsync<ResourceUtilizationChartDto>(endpoint);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating resource utilization chart");
            return null;
        }
    }

    public async Task<CashFlowChartDto?> GenerateCashFlowChartAsync(Guid projectId, DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var queryParams = new List<string>();
            if (startDate.HasValue) queryParams.Add($"startDate={startDate.Value:yyyy-MM-dd}");
            if (endDate.HasValue) queryParams.Add($"endDate={endDate.Value:yyyy-MM-dd}");
            
            var endpoint = $"visualization/cash-flow/{projectId}";
            if (queryParams.Any()) endpoint += "?" + string.Join("&", queryParams);
            
            return await _apiService.GetAsync<CashFlowChartDto>(endpoint);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating cash flow chart");
            return null;
        }
    }

    public async Task<MilestoneChartDto?> GenerateMilestoneChartAsync(Guid projectId)
    {
        try
        {
            return await _apiService.GetAsync<MilestoneChartDto>($"visualization/milestones/{projectId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating milestone chart");
            return null;
        }
    }

    public async Task<RiskMatrixDto?> GenerateRiskMatrixAsync(Guid projectId)
    {
        try
        {
            return await _apiService.GetAsync<RiskMatrixDto>($"visualization/risk-matrix/{projectId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating risk matrix");
            return null;
        }
    }

    public async Task<ContractStatusChartDto?> GenerateContractStatusChartAsync(Guid projectId)
    {
        try
        {
            return await _apiService.GetAsync<ContractStatusChartDto>($"visualization/contract-status/{projectId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating contract status chart");
            return null;
        }
    }

    public async Task<DashboardMetricsDto?> GetDashboardMetricsAsync(Guid projectId)
    {
        try
        {
            return await _apiService.GetAsync<DashboardMetricsDto>($"visualization/dashboard/{projectId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dashboard metrics");
            return null;
        }
    }
}