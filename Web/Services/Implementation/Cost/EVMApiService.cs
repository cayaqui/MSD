using Core.DTOs.Common;
using Core.DTOs.EVM;
using Core.DTOs.Reports;
using Web.Services.Interfaces;
using Web.Services.Interfaces.Cost;

namespace Web.Services.Implementation.Cost;

public class EVMApiService : IEVMApiService
{
    private readonly IApiService _apiService;
    private readonly ILoggingService _logger;
    private const string BaseEndpoint = "api/evm";

    public EVMApiService(IApiService apiService, ILoggingService logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    // Query operations
    public async Task<PagedResult<EVMRecordDto>> GetEVMRecordsAsync(Guid controlAccountId, EVMQueryParameters parameters)
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
            if (parameters.StartDate.HasValue)
                queryParams["startDate"] = parameters.StartDate.Value.ToString("yyyy-MM-dd");
            if (parameters.EndDate.HasValue)
                queryParams["endDate"] = parameters.EndDate.Value.ToString("yyyy-MM-dd");
            if (!string.IsNullOrWhiteSpace(parameters.Status))
                queryParams["status"] = parameters.Status;
            
            queryParams["includeForecasts"] = parameters.IncludeForecasts.ToString();

            var queryString = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));
            var result = await _apiService.GetAsync<PagedResult<EVMRecordDto>>($"{BaseEndpoint}/control-account/{controlAccountId}/records?{queryString}");
            return result ?? new PagedResult<EVMRecordDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting EVM records for control account {controlAccountId}");
            return new PagedResult<EVMRecordDto>();
        }
    }

    public async Task<EVMRecordDetailDto?> GetEVMRecordByIdAsync(Guid id)
    {
        try
        {
            return await _apiService.GetAsync<EVMRecordDetailDto>($"{BaseEndpoint}/records/{id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting EVM record {id}");
            return null;
        }
    }

    public async Task<EVMPerformanceReportDto?> GetProjectEVMReportAsync(Guid projectId, DateTime? asOfDate = null)
    {
        try
        {
            var url = $"{BaseEndpoint}/project/{projectId}/report";
            if (asOfDate.HasValue)
                url += $"?asOfDate={asOfDate.Value:yyyy-MM-dd}";
            
            return await _apiService.GetAsync<EVMPerformanceReportDto>(url);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting project EVM report for project {projectId}");
            return null;
        }
    }

    public async Task<List<EVMTrendDto>> GetEVMTrendsAsync(Guid controlAccountId, DateTime startDate, DateTime endDate)
    {
        try
        {
            var url = $"{BaseEndpoint}/control-account/{controlAccountId}/trends?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";
            var result = await _apiService.GetAsync<List<EVMTrendDto>>(url);
            return result ?? new List<EVMTrendDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting EVM trends for control account {controlAccountId}");
            return new List<EVMTrendDto>();
        }
    }

    // Command operations
    public async Task<Guid?> CreateEVMRecordAsync(CreateEVMRecordDto dto)
    {
        try
        {
            return await _apiService.PostAsync<CreateEVMRecordDto, Guid>($"{BaseEndpoint}/records", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating EVM record");
            return null;
        }
    }

    public async Task<bool> UpdateEVMActualsAsync(Guid id, UpdateEVMActualsDto dto)
    {
        try
        {
            await _apiService.PutAsync($"{BaseEndpoint}/records/{id}/actuals", dto);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating EVM actuals for record {id}");
            return false;
        }
    }

    public async Task<bool> CalculateProjectEVMAsync(Guid projectId, DateTime dataDate)
    {
        try
        {
            var dto = new { DataDate = dataDate };
            await _apiService.PostAsync($"{BaseEndpoint}/project/{projectId}/calculate", dto);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error calculating project EVM for project {projectId}");
            return false;
        }
    }

    public async Task<bool> GenerateMonthlyEVMAsync(Guid projectId, int year, int month)
    {
        try
        {
            var dto = new { Year = year, Month = month };
            await _apiService.PostAsync($"{BaseEndpoint}/project/{projectId}/generate-monthly", dto);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error generating monthly EVM for project {projectId}");
            return false;
        }
    }

    // Forecast operations
    public async Task<bool> UpdateEACAsync(Guid id, decimal newEAC, string justification)
    {
        try
        {
            var dto = new { NewEAC = newEAC, Justification = justification };
            await _apiService.PutAsync($"{BaseEndpoint}/records/{id}/eac", dto);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating EAC for record {id}");
            return false;
        }
    }

    // Nine Column Report operations
    public async Task<NineColumnReportDto?> GetNineColumnReportAsync(Guid projectId, DateTime? asOfDate = null)
    {
        try
        {
            var url = $"{BaseEndpoint}/project/{projectId}/nine-column-report";
            if (asOfDate.HasValue)
                url += $"?asOfDate={asOfDate.Value:yyyy-MM-dd}";
            
            return await _apiService.GetAsync<NineColumnReportDto>(url);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting nine column report for project {projectId}");
            return null;
        }
    }

    public async Task<NineColumnReportDto?> GetFilteredNineColumnReportAsync(NineColumnReportFilterDto filter)
    {
        try
        {
            return await _apiService.PostAsync<NineColumnReportFilterDto, NineColumnReportDto>($"{BaseEndpoint}/nine-column-report/filtered", filter);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting filtered nine column report");
            return null;
        }
    }

    public async Task<byte[]> ExportNineColumnReportToExcelAsync(Guid projectId, DateTime? asOfDate = null)
    {
        try
        {
            var url = $"{BaseEndpoint}/project/{projectId}/nine-column-report/excel";
            if (asOfDate.HasValue)
                url += $"?asOfDate={asOfDate.Value:yyyy-MM-dd}";
            
            return await _apiService.GetBytesAsync(url);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error exporting nine column report to Excel for project {projectId}");
            return Array.Empty<byte>();
        }
    }

    public async Task<byte[]> ExportNineColumnReportToPdfAsync(Guid projectId, DateTime? asOfDate = null)
    {
        try
        {
            var url = $"{BaseEndpoint}/project/{projectId}/nine-column-report/pdf";
            if (asOfDate.HasValue)
                url += $"?asOfDate={asOfDate.Value:yyyy-MM-dd}";
            
            return await _apiService.GetBytesAsync(url);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error exporting nine column report to PDF for project {projectId}");
            return Array.Empty<byte>();
        }
    }

    public async Task<NineColumnReportDto?> GetNineColumnReportByControlAccountAsync(Guid controlAccountId, DateTime? asOfDate = null, bool includeChildren = false)
    {
        try
        {
            var url = $"{BaseEndpoint}/control-account/{controlAccountId}/nine-column-report?includeChildren={includeChildren}";
            if (asOfDate.HasValue)
                url += $"&asOfDate={asOfDate.Value:yyyy-MM-dd}";
            
            return await _apiService.GetAsync<NineColumnReportDto>(url);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting nine column report for control account {controlAccountId}");
            return null;
        }
    }

    public async Task<List<NineColumnReportDto>> GetNineColumnReportTrendAsync(Guid projectId, DateTime startDate, DateTime endDate, string periodType = "monthly")
    {
        try
        {
            var url = $"{BaseEndpoint}/project/{projectId}/nine-column-report/trend?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}&periodType={periodType}";
            var result = await _apiService.GetAsync<List<NineColumnReportDto>>(url);
            return result ?? new List<NineColumnReportDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting nine column report trend for project {projectId}");
            return new List<NineColumnReportDto>();
        }
    }

    public async Task<Core.DTOs.EVM.NineColumnReportValidationResult?> ValidateNineColumnReportDataAsync(Guid projectId, DateTime? asOfDate = null)
    {
        try
        {
            var url = $"{BaseEndpoint}/project/{projectId}/nine-column-report/validate";
            if (asOfDate.HasValue)
                url += $"?asOfDate={asOfDate.Value:yyyy-MM-dd}";
            
            return await _apiService.GetAsync<Core.DTOs.EVM.NineColumnReportValidationResult>(url);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error validating nine column report data for project {projectId}");
            return null;
        }
    }
}