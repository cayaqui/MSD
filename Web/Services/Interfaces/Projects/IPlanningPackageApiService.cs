using Core.DTOs.Common;
using Core.DTOs.Cost.PlanningPackages;
using Core.DTOs.Projects.WorkPackageDetails;

namespace Web.Services.Interfaces.Projects;

public interface IPlanningPackageApiService
{
    // Query operations
    Task<PagedResult<PlanningPackageDto>> GetPlanningPackagesAsync(Guid projectId, QueryParameters parameters);
    Task<PlanningPackageDto?> GetPlanningPackageByIdAsync(Guid id);
    Task<List<PlanningPackageDto>> GetPlanningPackagesByControlAccountAsync(Guid controlAccountId);
    Task<List<PlanningPackageDto>> GetUnconvertedPlanningPackagesAsync(Guid projectId);
    
    // Command operations
    Task<Guid?> CreatePlanningPackageAsync(CreatePlanningPackageDto dto);
    Task<bool> UpdatePlanningPackageAsync(Guid id, UpdatePlanningPackageDto dto);
    Task<bool> ConvertToWorkPackageAsync(Guid id, ConvertPlanningToWorkPackageDto dto);
    Task<bool> DeletePlanningPackageAsync(Guid id);
    
    // Bulk operations
    Task<PlanningPackageBulkOperationResult> BulkCreatePlanningPackagesAsync(List<CreatePlanningPackageDto> planningPackages);
    Task<PlanningPackageBulkOperationResult> BulkConvertToWorkPackagesAsync(List<Guid> planningPackageIds, ConvertPlanningToWorkPackageDto conversionDetails);
    
    // Budget operations
    Task<PlanningPackageBudgetSummaryDto> GetPlanningPackageBudgetSummaryAsync(Guid controlAccountId);
    Task<bool> RedistributeBudgetAsync(Guid controlAccountId, List<BudgetAllocation> allocations);
    
    // Export
    Task<byte[]> ExportPlanningPackagesAsync(Guid projectId);
}

// DTOs to match API module
public record PlanningPackageBulkOperationResult(int SuccessCount, int TotalCount);

public record PlanningPackageBudgetSummaryDto
{
    public Guid ControlAccountId { get; set; }
    public decimal TotalBudget { get; set; }
    public decimal AllocatedBudget { get; set; }
    public decimal UnallocatedBudget { get; set; }
    public int PlanningPackageCount { get; set; }
}

public record BudgetAllocation(Guid PlanningPackageId, decimal NewBudget);