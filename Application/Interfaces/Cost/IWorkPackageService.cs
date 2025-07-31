
using Core.DTOs.WorkPackages;

namespace Application.Interfaces.Cost;

/// <summary>
/// Interface for Work Package management service
/// </summary>
public interface IWorkPackageService
{
    // Query Operations
    Task<PagedResult<WorkPackageDto>> GetWorkPackagesAsync(
        Guid projectId,
        QueryParameters parameters,
        CancellationToken cancellationToken = default);

    Task<WorkPackageDetailDto?> GetWorkPackageByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<List<WorkPackageDto>> GetWorkPackagesByControlAccountAsync(
        Guid controlAccountId,
        CancellationToken cancellationToken = default);

    Task<List<WorkPackageProgressDto>> GetWorkPackageProgressHistoryAsync(
        Guid workPackageId,
        CancellationToken cancellationToken = default);

    // Command Operations
    Task<Result<Guid>> CreateWorkPackageAsync(
        CreateWorkPackageDto dto,
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result> UpdateWorkPackageAsync(
        Guid id,
        UpdateWorkPackageDto dto,
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result> UpdateWorkPackageProgressAsync(
        Guid id,
        UpdateWorkPackageProgressDto dto,
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result> StartWorkPackageAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result> CompleteWorkPackageAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result> BaselineWorkPackageAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteWorkPackageAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default);

    // Activity Operations
    Task<Result<Guid>> AddActivityToWorkPackageAsync(
        Guid workPackageId,
        CreateActivityDto dto,
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result> UpdateActivityProgressAsync(
        Guid activityId,
        decimal percentComplete,
        decimal actualHours,
        string userId,
        CancellationToken cancellationToken = default);
}
