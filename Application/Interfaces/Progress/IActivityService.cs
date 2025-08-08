using Core.DTOs.Common;
using Core.DTOs.Progress.Activities;
using Result = Core.Results.Result;

namespace Application.Interfaces.Progress;

public interface IActivityService
{
    // Activity Management
    Task<PagedResult<ActivityDto>> GetActivitiesAsync(ActivityFilterDto filter, CancellationToken cancellationToken = default);
    Task<ActivityDto?> GetActivityByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<ActivityDto>> GetActivitiesByWBSElementAsync(Guid wbsElementId, CancellationToken cancellationToken = default);
    Task<List<ActivityDto>> GetCriticalPathActivitiesAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<Core.Results.Result<Guid>> CreateActivityAsync(CreateActivityDto dto, Guid userId, CancellationToken cancellationToken = default);
    Task<Result> UpdateActivityAsync(Guid id, UpdateActivityDto dto, Guid userId, CancellationToken cancellationToken = default);
    Task<Result> DeleteActivityAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
    
    // Progress Management
    Task<Result> UpdateActivityProgressAsync(Guid id, UpdateActivityProgressDto dto, Guid userId, CancellationToken cancellationToken = default);
    Task<Result> StartActivityAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
    Task<Result> CompleteActivityAsync(Guid id, DateTime? actualEndDate, Guid userId, CancellationToken cancellationToken = default);
    Task<Result> SuspendActivityAsync(Guid id, string reason, Guid userId, CancellationToken cancellationToken = default);
    Task<Result> ResumeActivityAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
    Task<Result> CancelActivityAsync(Guid id, string reason, Guid userId, CancellationToken cancellationToken = default);
    
    // Bulk Operations
    Task<Result> BulkUpdateProgressAsync(BulkUpdateActivitiesDto dto, Guid userId, CancellationToken cancellationToken = default);
    Task<Core.Results.Result<int>> BulkCreateActivitiesAsync(List<CreateActivityDto> activities, Guid userId, CancellationToken cancellationToken = default);
    
    // Schedule Management
    Task<Result> UpdateActivityScheduleAsync(Guid id, DateTime plannedStart, DateTime plannedEnd, Guid userId, CancellationToken cancellationToken = default);
    Task<Result> SetActivityDependenciesAsync(Guid id, string[]? predecessors, string[]? successors, Guid userId, CancellationToken cancellationToken = default);
    Task<Result> RecalculateCriticalPathAsync(Guid projectId, CancellationToken cancellationToken = default);
    
    // Resource Management
    Task<Result> AssignResourcesAsync(Guid activityId, List<Guid> resourceIds, Guid userId, CancellationToken cancellationToken = default);
    Task<Result> UpdateResourceRateAsync(Guid activityId, decimal rate, Guid userId, CancellationToken cancellationToken = default);
    
    // Validation
    Task<bool> ValidateActivityCodeAsync(string code, Guid projectId, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<bool> CanDeleteActivityAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<string>> ValidateDependenciesAsync(Guid activityId, string[] predecessors, string[] successors, CancellationToken cancellationToken = default);
}