using Core.DTOs.ControlAccounts;
using Core.DTOs.Cost;
using Web.Models.Responses;
using Web.Services.Interfaces;

namespace Web.Services.Implementation
{
    public class CostService : ICostService
    {
        private readonly IApiService _apiService;
        private readonly ILogger<CostService> _logger;
        private const string BaseEndpoint = "/api/costs";
        private const string ControlAccountsEndpoint = "/api/control-accounts";

        public CostService(IApiService apiService, ILogger<CostService> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        // Resumen de costos
        public async Task<ApiResponse<CostSummaryDto>> GetCostSummaryAsync(Guid projectId)
        {
            try
            {
                return await _apiService.GetAsync<CostSummaryDto>($"{BaseEndpoint}/projects/{projectId}/summary");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el resumen de costos del proyecto {ProjectId}", projectId);
                return ApiResponse<CostSummaryDto>.ErrorResponse("Error al obtener el resumen de costos");
            }
        }

        // Control Accounts (CBS)
        public async Task<ApiResponse<List<ControlAccountDto>>> GetControlAccountsAsync(Guid projectId)
        {
            try
            {
                return await _apiService.GetAsync<List<ControlAccountDto>>($"{ControlAccountsEndpoint}/projects/{projectId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las cuentas de control del proyecto {ProjectId}", projectId);
                return ApiResponse<List<ControlAccountDto>>.ErrorResponse("Error al obtener las cuentas de control");
            }
        }

        public async Task<ApiResponse<ControlAccountDto>> GetControlAccountAsync(Guid accountId)
        {
            try
            {
                return await _apiService.GetAsync<ControlAccountDto>($"{ControlAccountsEndpoint}/{accountId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la cuenta de control {AccountId}", accountId);
                return ApiResponse<ControlAccountDto>.ErrorResponse("Error al obtener la cuenta de control");
            }
        }

        public async Task<ApiResponse<ControlAccountDto>> CreateControlAccountAsync(CreateControlAccountDto dto)
        {
            try
            {
                return await _apiService.PostAsync<ControlAccountDto>(ControlAccountsEndpoint, dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la cuenta de control");
                return ApiResponse<ControlAccountDto>.ErrorResponse("Error al crear la cuenta de control");
            }
        }

        public async Task<ApiResponse<ControlAccountDto>> UpdateControlAccountAsync(Guid accountId, UpdateControlAccountDto dto)
        {
            try
            {
                return await _apiService.PutAsync<ControlAccountDto>($"{ControlAccountsEndpoint}/{accountId}", dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la cuenta de control {AccountId}", accountId);
                return ApiResponse<ControlAccountDto>.ErrorResponse("Error al actualizar la cuenta de control");
            }
        }

        public async Task<ApiResponse<bool>> DeleteControlAccountAsync(Guid accountId)
        {
            try
            {
                return await _apiService.DeleteAsync($"{ControlAccountsEndpoint}/{accountId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la cuenta de control {AccountId}", accountId);
                return ApiResponse<bool>.ErrorResponse("Error al eliminar la cuenta de control");
            }
        }

        // Categorías de costos
        public async Task<ApiResponse<List<CostCategoryDto>>> GetCostCategoriesAsync(Guid projectId)
        {
            try
            {
                return await _apiService.GetAsync<List<CostCategoryDto>>($"{BaseEndpoint}/projects/{projectId}/categories");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las categorías de costos del proyecto {ProjectId}", projectId);
                return ApiResponse<List<CostCategoryDto>>.ErrorResponse("Error al obtener las categorías de costos");
            }
        }

        // Entradas de costos
        public async Task<ApiResponse<List<CostEntryDto>>> GetProjectCostsAsync(Guid projectId)
        {
            try
            {
                return await _apiService.GetAsync<List<CostEntryDto>>($"{BaseEndpoint}/projects/{projectId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los costos del proyecto {ProjectId}", projectId);
                return ApiResponse<List<CostEntryDto>>.ErrorResponse("Error al obtener los costos del proyecto");
            }
        }

        public async Task<ApiResponse<PagedResult<CostEntryDto>>> SearchCostsAsync(CostFilterDto filter)
        {
            try
            {
                return await _apiService.PostAsync<PagedResult<CostEntryDto>>($"{BaseEndpoint}/search", filter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar costos");
                return ApiResponse<PagedResult<CostEntryDto>>.ErrorResponse("Error al buscar costos");
            }
        }

        public async Task<ApiResponse<CostEntryDto>> GetCostEntryAsync(Guid costId)
        {
            try
            {
                return await _apiService.GetAsync<CostEntryDto>($"{BaseEndpoint}/{costId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la entrada de costo {CostId}", costId);
                return ApiResponse<CostEntryDto>.ErrorResponse("Error al obtener la entrada de costo");
            }
        }

        public async Task<ApiResponse<CostEntryDto>> CreateCostEntryAsync(CreateCostEntryDto dto)
        {
            try
            {
                return await _apiService.PostAsync<CostEntryDto>(BaseEndpoint, dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la entrada de costo");
                return ApiResponse<CostEntryDto>.ErrorResponse("Error al crear la entrada de costo");
            }
        }

        public async Task<ApiResponse<CostEntryDto>> UpdateCostEntryAsync(Guid costId, UpdateCostEntryDto dto)
        {
            try
            {
                return await _apiService.PutAsync<CostEntryDto>($"{BaseEndpoint}/{costId}", dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la entrada de costo {CostId}", costId);
                return ApiResponse<CostEntryDto>.ErrorResponse("Error al actualizar la entrada de costo");
            }
        }

        public async Task<ApiResponse<bool>> DeleteCostEntryAsync(Guid costId)
        {
            try
            {
                return await _apiService.DeleteAsync($"{BaseEndpoint}/{costId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la entrada de costo {CostId}", costId);
                return ApiResponse<bool>.ErrorResponse("Error al eliminar la entrada de costo");
            }
        }

        // Análisis y reportes
        public async Task<ApiResponse<CostAnalysisDto>> GetCostAnalysisAsync(Guid projectId)
        {
            try
            {
                return await _apiService.GetAsync<CostAnalysisDto>($"{BaseEndpoint}/projects/{projectId}/analysis");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el análisis de costos del proyecto {ProjectId}", projectId);
                return ApiResponse<CostAnalysisDto>.ErrorResponse("Error al obtener el análisis de costos");
            }
        }

        public async Task<ApiResponse<List<CashFlowDto>>> GetCashFlowAsync(Guid projectId, string period)
        {
            try
            {
                var endpoint = $"{BaseEndpoint}/projects/{projectId}/cash-flow?period={period}";
                return await _apiService.GetAsync<List<CashFlowDto>>(endpoint);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el flujo de caja del proyecto {ProjectId}", projectId);
                return ApiResponse<List<CashFlowDto>>.ErrorResponse("Error al obtener el flujo de caja");
            }
        }

        public async Task<ApiResponse<byte[]>> ExportCostReportAsync(Guid projectId, string format)
        {
            try
            {
                var endpoint = $"{BaseEndpoint}/projects/{projectId}/export?format={format}";
                var response = await _apiService.GetAsync<byte[]>(endpoint);

                if (!response.Success)
                {
                    _logger.LogError("Error al exportar el reporte de costos: {Message}", response.Message);
                    return ApiResponse<byte[]>.ErrorResponse(response.Message);
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al exportar el reporte de costos del proyecto {ProjectId}", projectId);
                return ApiResponse<byte[]>.ErrorResponse("Error al exportar el reporte de costos");
            }
        }
    }
}