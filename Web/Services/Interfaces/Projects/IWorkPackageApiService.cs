using Core.DTOs.Common;
using Core.DTOs.WorkPackages;
using Core.DTOs.Projects.WorkPackageDetails;

namespace Web.Services.Interfaces.Projects;

public interface IWorkPackageApiService
{
    Task<PagedResult<WorkPackageDto>> GetWorkPackagesByProjectAsync(Guid projectId, QueryParameters parameters);
    Task<WorkPackageDetailDto> GetWorkPackageByIdAsync(Guid id);
    Task<List<WorkPackageDto>> GetWorkPackagesByControlAccountAsync(Guid controlAccountId);
    Task<List<WorkPackageProgressDto>> GetWorkPackageProgressHistoryAsync(Guid id);
    Task<Guid> CreateWorkPackageAsync(CreateWorkPackageDto dto);
    Task UpdateWorkPackageAsync(Guid id, UpdateWorkPackageDto dto);
    Task UpdateWorkPackageProgressAsync(Guid id, UpdateWorkPackageProgressDto dto);
    Task StartWorkPackageAsync(Guid id);
    Task CompleteWorkPackageAsync(Guid id);
    Task BaselineWorkPackageAsync(Guid id);
    Task DeleteWorkPackageAsync(Guid id);
    Task<Guid> AddActivityToWorkPackageAsync(Guid workPackageId, CreateActivityDto dto);
    Task UpdateActivityProgressAsync(Guid activityId, decimal percentComplete, decimal actualHours);
    Task<byte[]> ExportWorkPackagesAsync(Guid projectId);
}