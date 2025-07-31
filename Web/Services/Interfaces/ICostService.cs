using Core.DTOs.ControlAccounts;
using Core.DTOs.Cost;
using Web.Models.Responses;

namespace Web.Services.Interfaces
{
    public interface ICostService
    {
        // Resumen de costos
        Task<ApiResponse<CostSummaryDto>> GetCostSummaryAsync(Guid projectId);

        // Control Accounts (CBS)
        Task<ApiResponse<List<ControlAccountDto>>> GetControlAccountsAsync(Guid projectId);
        Task<ApiResponse<ControlAccountDto>> GetControlAccountAsync(Guid accountId);
        Task<ApiResponse<ControlAccountDto>> CreateControlAccountAsync(CreateControlAccountDto dto);
        Task<ApiResponse<ControlAccountDto>> UpdateControlAccountAsync(Guid accountId, UpdateControlAccountDto dto);
        Task<ApiResponse<bool>> DeleteControlAccountAsync(Guid accountId);

        // Categorías de costos
        Task<ApiResponse<List<CostCategoryDto>>> GetCostCategoriesAsync(Guid projectId);

        // Entradas de costos
        Task<ApiResponse<List<CostEntryDto>>> GetProjectCostsAsync(Guid projectId);
        Task<ApiResponse<PagedResult<CostEntryDto>>> SearchCostsAsync(CostFilterDto filter);
        Task<ApiResponse<CostEntryDto>> GetCostEntryAsync(Guid costId);
        Task<ApiResponse<CostEntryDto>> CreateCostEntryAsync(CreateCostEntryDto dto);
        Task<ApiResponse<CostEntryDto>> UpdateCostEntryAsync(Guid costId, UpdateCostEntryDto dto);
        Task<ApiResponse<bool>> DeleteCostEntryAsync(Guid costId);

        // Análisis y reportes
        Task<ApiResponse<CostAnalysisDto>> GetCostAnalysisAsync(Guid projectId);
        Task<ApiResponse<List<CashFlowDto>>> GetCashFlowAsync(Guid projectId, string period);
        Task<ApiResponse<byte[]>> ExportCostReportAsync(Guid projectId, string format);
    }
}