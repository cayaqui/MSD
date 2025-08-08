using Core.DTOs.Common;
using Core.DTOs.Progress.Activities;

namespace Web.Services.Interfaces.Progress;

public interface IActivityApiService
{
    // Activity Management
    Task<PagedResult<ActivityDto>> GetActivitiesAsync(ActivityFilterDto filter, CancellationToken cancellationToken = default);
    Task<ActivityDto?> GetActivityByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<ActivityDto>> GetActivitiesByWBSElementAsync(Guid wbsElementId, CancellationToken cancellationToken = default);
    Task<List<ActivityDto>> GetCriticalPathActivitiesAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<Guid> CreateActivityAsync(CreateActivityDto dto, CancellationToken cancellationToken = default);
    Task UpdateActivityAsync(Guid id, UpdateActivityDto dto, CancellationToken cancellationToken = default);
    Task DeleteActivityAsync(Guid id, CancellationToken cancellationToken = default);
    
    // Progress Management
    Task UpdateActivityProgressAsync(Guid id, UpdateActivityProgressDto dto, CancellationToken cancellationToken = default);
    Task StartActivityAsync(Guid id, CancellationToken cancellationToken = default);
    Task CompleteActivityAsync(Guid id, DateTime? actualEndDate, CancellationToken cancellationToken = default);
    Task SuspendActivityAsync(Guid id, string reason, CancellationToken cancellationToken = default);
    Task ResumeActivityAsync(Guid id, CancellationToken cancellationToken = default);
    Task CancelActivityAsync(Guid id, string reason, CancellationToken cancellationToken = default);
    
    // Bulk Operations
    Task<BulkOperationResult> BulkUpdateProgressAsync(BulkUpdateActivitiesDto dto, CancellationToken cancellationToken = default);
    Task<BulkOperationResult> BulkCreateActivitiesAsync(List<CreateActivityDto> activities, CancellationToken cancellationToken = default);
    
    // Schedule Management
    Task UpdateActivityScheduleAsync(Guid id, DateTime plannedStart, DateTime plannedEnd, CancellationToken cancellationToken = default);
    Task SetActivityDependenciesAsync(Guid id, string[]? predecessors, string[]? successors, CancellationToken cancellationToken = default);
    Task RecalculateCriticalPathAsync(Guid projectId, CancellationToken cancellationToken = default);
    
    // Resource Management
    Task AssignResourcesAsync(Guid activityId, List<Guid> resourceIds, CancellationToken cancellationToken = default);
    Task UpdateResourceRateAsync(Guid activityId, decimal rate, CancellationToken cancellationToken = default);
    
    // Validation
    Task<bool> ValidateActivityCodeAsync(string code, Guid projectId, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<bool> CanDeleteActivityAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<string>> ValidateDependenciesAsync(Guid activityId, string[] predecessors, string[] successors, CancellationToken cancellationToken = default);
}