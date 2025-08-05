using Core.DTOs.Common;
using Core.DTOs.Cost;
using Core.DTOs.Cost.CostControlReports;
using Core.DTOs.Cost.CostItems;
using Core.DTOs.Cost.PlanningPackages;

namespace Application.Interfaces.Cost;

/// <summary>
/// Interface for Cost management service
/// </summary>
public interface ICostService
{
    // Query Operations
    Task<PagedResult<CostItemDto>> GetCostItemsAsync(
        Guid projectId,
        CostQueryParameters parameters,
        CancellationToken cancellationToken = default
    );

    Task<CostItemDetailDto?> GetCostItemByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    );

    Task<ProjectCostReportDto> GetProjectCostReportAsync(
        Guid projectId,
        DateTime? asOfDate = null,
        CancellationToken cancellationToken = default
    );

    Task<List<CostSummaryByCategoryDto>> GetCostSummaryByCategoryAsync(
        Guid projectId,
        CancellationToken cancellationToken = default
    );

    Task<List<CostSummaryByControlAccountDto>> GetCostSummaryByControlAccountAsync(
        Guid projectId,
        CancellationToken cancellationToken = default
    );

    // Command Operations
    Task<Result<Guid>> CreateCostItemAsync(
        CreateCostItemDto dto,
        string userId,
        CancellationToken cancellationToken = default
    );

    Task<Result> UpdateCostItemAsync(
        Guid id,
        UpdateCostItemDto dto,
        string userId,
        CancellationToken cancellationToken = default
    );

    Task<Result> RecordActualCostAsync(
        Guid costItemId,
        RecordActualCostDto dto,
        string userId,
        CancellationToken cancellationToken = default
    );

    Task<Result> RecordCommitmentAsync(
        Guid costItemId,
        RecordCommitmentDto dto,
        string userId,
        CancellationToken cancellationToken = default
    );

    Task<Result> ApproveCostItemAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default
    );

    Task<Result> DeleteCostItemAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default
    );

    // Planning Package Operations
    Task<PagedResult<PlanningPackageDto>> GetPlanningPackagesAsync(
        Guid controlAccountId,
        PlanningPackageQueryParameters parameters,
        CancellationToken cancellationToken = default
    );

    Task<Result<Guid>> CreatePlanningPackageAsync(
        CreatePlanningPackageDto dto,
        string userId,
        CancellationToken cancellationToken = default
    );

    Task<Result> UpdatePlanningPackageAsync(
        Guid id,
        UpdatePlanningPackageDto dto,
        string userId,
        CancellationToken cancellationToken = default
    );

    Task<Result> ConvertPlanningPackageToWorkPackagesAsync(
        Guid id,
        ConvertPlanningPackageDto dto,
        string userId,
        CancellationToken cancellationToken = default
    );
}
