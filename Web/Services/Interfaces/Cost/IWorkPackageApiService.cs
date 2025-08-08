using Core.DTOs.Common;
using Core.DTOs.Cost.WorkPackages;
using Core.DTOs.Projects.WorkPackageDetails;

namespace Web.Services.Interfaces.Cost;

public interface IWorkPackageApiService
{
    Task<PagedResult<WorkPackageDto>> GetWorkPackagesAsync(WorkPackageFilterDto filter);
    Task<WorkPackageDetailsDto> GetWorkPackageByIdAsync(Guid id);
    Task<Guid> CreateWorkPackageAsync(CreateWorkPackageDto dto);
    Task UpdateWorkPackageAsync(Guid id, UpdateWorkPackageDto dto);
    Task DeleteWorkPackageAsync(Guid id);
    Task UpdateProgressAsync(Guid id, UpdateWorkPackageProgressDto dto);
    Task<List<WorkPackageActivityDto>> GetActivitiesAsync(Guid workPackageId);
    Task<Guid> AddActivityAsync(Guid workPackageId, CreateActivityDto dto);
    Task UpdateActivityAsync(Guid workPackageId, Guid activityId, UpdateActivityDto dto);
    Task DeleteActivityAsync(Guid workPackageId, Guid activityId);
    Task RecalculateProgressAsync(Guid workPackageId);
}