using Core.DTOs.Common;
using Core.DTOs.Cost;
using Core.DTOs.Cost.CostControlReports;
using Core.DTOs.Cost.CostItems;
using Core.DTOs.Cost.PlanningPackages;
using Core.DTOs.Projects.WorkPackageDetails;

namespace Web.Services.Interfaces.Cost;

public interface ICostApiService
{
    // Cost Items
    Task<PagedResult<CostItemDto>> GetCostItemsAsync(Guid projectId, Core.DTOs.Cost.CostQueryParameters parameters);
    Task<CostItemDetailDto?> GetCostItemByIdAsync(Guid id);
    Task<Guid?> CreateCostItemAsync(CreateCostItemDto dto);
    Task<bool> UpdateCostItemAsync(Guid id, UpdateCostItemDto dto);
    Task<bool> DeleteCostItemAsync(Guid id);
    
    // Cost Recording
    Task<bool> RecordActualCostAsync(Guid id, RecordActualCostDto dto);
    Task<bool> RecordCommitmentAsync(Guid id, RecordCommitmentDto dto);
    Task<bool> ApproveCostItemAsync(Guid id);
    
    // Reporting
    Task<ProjectCostReportDto?> GetProjectCostReportAsync(Guid projectId);
    Task<List<CostSummaryByCategoryDto>> GetCostSummaryByCategoryAsync(Guid projectId);
    Task<List<CostSummaryByControlAccountDto>> GetCostSummaryByControlAccountAsync(Guid projectId);
    
    // Planning Packages (legacy endpoints in CostModule)
    Task<PagedResult<PlanningPackageDto>> GetPlanningPackagesAsync(Guid controlAccountId, QueryParameters parameters);
    Task<Guid?> CreatePlanningPackageAsync(CreatePlanningPackageDto dto);
    Task<bool> UpdatePlanningPackageAsync(Guid id, UpdatePlanningPackageDto dto);
    Task<bool> ConvertPlanningPackageToWorkPackagesAsync(Guid id, ConvertPlanningToWorkPackageDto dto);
}