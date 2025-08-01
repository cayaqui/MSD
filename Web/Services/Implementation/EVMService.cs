using Application.Interfaces.Cost;
using Core.DTOs.EVM;
using Core.DTOs.Reports;
using System.Net.Http.Json;
using Web.Models.Responses;
using Web.Services.Interfaces;

namespace Web.Services.Implementation;

/// <summary>
/// EVM Service implementation for Blazor Web project
/// </summary>
public class EVMService : IEVMService
{
    private readonly IApiService _apiService;
    private readonly ILogger<EVMService> _logger;
    private const string BaseEndpoint = "/api/evm";

    public EVMService(IApiService apiService, ILogger<EVMService> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    // Nine Column Report implementations

    public async Task<ApiResponse<NineColumnReportDto>> GetNineColumnReportAsync(
        Guid projectId,
        DateTime? asOfDate = null)
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
            _logger.LogError(ex, "Error al obtener el reporte de 9 columnas para el proyecto {ProjectId}", projectId);
            return ApiResponse<NineColumnReportDto>.ErrorResponse("Error al obtener el reporte de 9 columnas");
        }
    }

    public async Task<ApiResponse<NineColumnReportDto>> GetFilteredNineColumnReportAsync(
        NineColumnReportFilterDto filter)
    {
        try
        {
            var url = $"{BaseEndpoint}/project/{filter.ProjectId}/nine-column-report/filtered";
            return await _apiService.PostAsync<NineColumnReportDto>(url, filter);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el reporte filtrado de 9 columnas");
            return ApiResponse<NineColumnReportDto>.ErrorResponse("Error al obtener el reporte filtrado");
        }
    }

    public async Task<ApiResponse<byte[]>> ExportNineColumnReportToExcelAsync(
        Guid projectId,
        DateTime? asOfDate = null)
    {
        try
        {
            var url = $"{BaseEndpoint}/project/{projectId}/nine-column-report/export/excel";
            if (asOfDate.HasValue)
                url += $"?asOfDate={asOfDate.Value:yyyy-MM-dd}";

            return await _apiService.GetBytesAsync(url);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al exportar el reporte a Excel");
            return ApiResponse<byte[]>.ErrorResponse("Error al exportar el reporte a Excel");
        }
    }

    public async Task<ApiResponse<byte[]>> ExportNineColumnReportToPdfAsync(
        Guid projectId,
        DateTime? asOfDate = null)
    {
        try
        {
            var url = $"{BaseEndpoint}/project/{projectId}/nine-column-report/export/pdf";
            if (asOfDate.HasValue)
                url += $"?asOfDate={asOfDate.Value:yyyy-MM-dd}";

            return await _apiService.GetBytesAsync(url);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al exportar el reporte a PDF");
            return ApiResponse<byte[]>.ErrorResponse("Error al exportar el reporte a PDF");
        }
    }

    public async Task<ApiResponse<NineColumnReportDto>> GetNineColumnReportByControlAccountAsync(
        Guid controlAccountId,
        DateTime? asOfDate = null,
        bool includeChildren = true)
    {
        try
        {
            var url = $"{BaseEndpoint}/control-account/{controlAccountId}/nine-column-report";
            url += $"?includeChildren={includeChildren}";
            if (asOfDate.HasValue)
                url += $"&asOfDate={asOfDate.Value:yyyy-MM-dd}";

            return await _apiService.GetAsync<NineColumnReportDto>(url);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el reporte por Control Account {ControlAccountId}", controlAccountId);
            return ApiResponse<NineColumnReportDto>.ErrorResponse("Error al obtener el reporte por Control Account");
        }
    }

    public async Task<ApiResponse<List<NineColumnReportDto>>> GetNineColumnReportTrendAsync(
        Guid projectId,
        DateTime startDate,
        DateTime endDate,
        string periodType = "Monthly")
    {
        try
        {
            var url = $"{BaseEndpoint}/project/{projectId}/nine-column-report/trend";
            url += $"?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}&periodType={periodType}";

            return await _apiService.GetAsync<List<NineColumnReportDto>>(url);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener tendencias del reporte");
            return ApiResponse<List<NineColumnReportDto>>.ErrorResponse("Error al obtener tendencias del reporte");
        }
    }

    public async Task<ApiResponse<NineColumnReportValidationResult>> ValidateNineColumnReportAsync(
        Guid projectId,
        DateTime? asOfDate = null)
    {
        try
        {
            var url = $"{BaseEndpoint}/project/{projectId}/nine-column-report/validate";
            if (asOfDate.HasValue)
                url += $"?asOfDate={asOfDate.Value:yyyy-MM-dd}";

            return await _apiService.GetAsync<NineColumnReportValidationResult>(url);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al validar el reporte");
            return ApiResponse<NineColumnReportValidationResult>.ErrorResponse("Error al validar el reporte");
        }
    }

    // Standard EVM implementations

    public async Task<ApiResponse<List<EVMRecordDto>>> GetEVMRecordsAsync(Guid projectId)
    {
        try
        {
            return await _apiService.GetAsync<List<EVMRecordDto>>($"{BaseEndpoint}/records?projectId={projectId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener registros EVM");
            return ApiResponse<List<EVMRecordDto>>.ErrorResponse("Error al obtener registros EVM");
        }
    }

    public async Task<ApiResponse<EVMRecordDetailDto>> GetEVMRecordAsync(Guid recordId)
    {
        try
        {
            return await _apiService.GetAsync<EVMRecordDetailDto>($"{BaseEndpoint}/records/{recordId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el registro EVM");
            return ApiResponse<EVMRecordDetailDto>.ErrorResponse("Error al obtener el registro EVM");
        }
    }

    public async Task<ApiResponse<EVMPerformanceReportDto>> GetProjectEVMReportAsync(Guid projectId)
    {
        try
        {
            return await _apiService.GetAsync<EVMPerformanceReportDto>($"{BaseEndpoint}/project/{projectId}/report");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el reporte EVM del proyecto");
            return ApiResponse<EVMPerformanceReportDto>.ErrorResponse("Error al obtener el reporte EVM");
        }
    }

    public async Task<ApiResponse<List<EVMTrendDto>>> GetEVMTrendsAsync(Guid controlAccountId)
    {
        try
        {
            return await _apiService.GetAsync<List<EVMTrendDto>>($"{BaseEndpoint}/control-account/{controlAccountId}/trends");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener tendencias EVM");
            return ApiResponse<List<EVMTrendDto>>.ErrorResponse("Error al obtener tendencias EVM");
        }
    }

    public async Task<ApiResponse<Guid>> CreateEVMRecordAsync(CreateEVMRecordDto dto)
    {
        try
        {
            return await _apiService.PostAsync<Guid>($"{BaseEndpoint}/records", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear registro EVM");
            return ApiResponse<Guid>.ErrorResponse("Error al crear registro EVM");
        }
    }

    public async Task<ApiResponse<bool>> CalculateProjectEVMAsync(Guid projectId)
    {
        try
        {
            return await _apiService.PostAsync<bool>($"{BaseEndpoint}/project/{projectId}/calculate", null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al calcular EVM del proyecto");
            return ApiResponse<bool>.ErrorResponse("Error al calcular EVM");
        }
    }
}