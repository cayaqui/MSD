using Core.DTOs.Common;
using Core.DTOs.Progress.Activities;
using Web.Services.Interfaces;
using Web.Services.Interfaces.Progress;

namespace Web.Services.Implementation.Progress;

public class ActivityApiService : IActivityApiService
{
    private readonly IApiService _apiService;
    private const string BaseUrl = "api/activities";

    public ActivityApiService(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<PagedResult<ActivityDto>> GetActivitiesAsync(ActivityFilterDto filter, CancellationToken cancellationToken = default)
    {
        var queryParams = new Dictionary<string, string?>
        {
            ["projectId"] = filter.ProjectId?.ToString(),
            ["wbsElementId"] = filter.WBSElementId?.ToString(),
            ["status"] = filter.Status?.ToString(),
            ["activityType"] = filter.ActivityType?.ToString(),
            ["isCritical"] = filter.IsCritical?.ToString(),
            ["hasProgress"] = filter.HasProgress?.ToString(),
            ["searchTerm"] = filter.SearchTerm,
            ["sortBy"] = filter.SortBy,
            ["sortDescending"] = filter.SortDescending.ToString(),
            ["pageNumber"] = filter.PageNumber.ToString(),
            ["pageSize"] = filter.PageSize.ToString()
        };

        return await _apiService.GetAsync<PagedResult<ActivityDto>>(BaseUrl, queryParams, cancellationToken);
    }

    public async Task<ActivityDto?> GetActivityByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _apiService.GetAsync<ActivityDto>($"{BaseUrl}/{id}", cancellationToken);
    }

    public async Task<List<ActivityDto>> GetActivitiesByWBSElementAsync(Guid wbsElementId, CancellationToken cancellationToken = default)
    {
        return await _apiService.GetAsync<List<ActivityDto>>($"{BaseUrl}/wbs-element/{wbsElementId}", cancellationToken);
    }

    public async Task<List<ActivityDto>> GetCriticalPathActivitiesAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        return await _apiService.GetAsync<List<ActivityDto>>($"{BaseUrl}/project/{projectId}/critical-path", cancellationToken);
    }

    public async Task<Guid> CreateActivityAsync(CreateActivityDto dto, CancellationToken cancellationToken = default)
    {
        return await _apiService.PostAsync<CreateActivityDto, Guid>(BaseUrl, dto, cancellationToken);
    }

    public async Task UpdateActivityAsync(Guid id, UpdateActivityDto dto, CancellationToken cancellationToken = default)
    {
        await _apiService.PutAsync($"{BaseUrl}/{id}", dto, cancellationToken);
    }

    public async Task DeleteActivityAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _apiService.DeleteAsync($"{BaseUrl}/{id}", cancellationToken);
    }

    public async Task UpdateActivityProgressAsync(Guid id, UpdateActivityProgressDto dto, CancellationToken cancellationToken = default)
    {
        await _apiService.PutAsync($"{BaseUrl}/{id}/progress", dto, cancellationToken);
    }

    public async Task StartActivityAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _apiService.PostAsync($"{BaseUrl}/{id}/start", new { }, cancellationToken);
    }

    public async Task CompleteActivityAsync(Guid id, DateTime? actualEndDate, CancellationToken cancellationToken = default)
    {
        await _apiService.PostAsync($"{BaseUrl}/{id}/complete", new { actualEndDate }, cancellationToken);
    }

    public async Task SuspendActivityAsync(Guid id, string reason, CancellationToken cancellationToken = default)
    {
        await _apiService.PostAsync($"{BaseUrl}/{id}/suspend", new { reason }, cancellationToken);
    }

    public async Task ResumeActivityAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _apiService.PostAsync($"{BaseUrl}/{id}/resume", new { }, cancellationToken);
    }

    public async Task CancelActivityAsync(Guid id, string reason, CancellationToken cancellationToken = default)
    {
        await _apiService.PostAsync($"{BaseUrl}/{id}/cancel", new { reason }, cancellationToken);
    }

    public async Task<BulkOperationResult> BulkUpdateProgressAsync(BulkUpdateActivitiesDto dto, CancellationToken cancellationToken = default)
    {
        return await _apiService.PostAsync<BulkUpdateActivitiesDto, BulkOperationResult>($"{BaseUrl}/bulk-update-progress", dto, cancellationToken);
    }

    public async Task<BulkOperationResult> BulkCreateActivitiesAsync(List<CreateActivityDto> activities, CancellationToken cancellationToken = default)
    {
        return await _apiService.PostAsync<List<CreateActivityDto>, BulkOperationResult>($"{BaseUrl}/bulk-create", activities, cancellationToken);
    }

    public async Task UpdateActivityScheduleAsync(Guid id, DateTime plannedStart, DateTime plannedEnd, CancellationToken cancellationToken = default)
    {
        await _apiService.PutAsync($"{BaseUrl}/{id}/schedule", new { plannedStartDate = plannedStart, plannedEndDate = plannedEnd }, cancellationToken);
    }

    public async Task SetActivityDependenciesAsync(Guid id, string[]? predecessors, string[]? successors, CancellationToken cancellationToken = default)
    {
        await _apiService.PutAsync($"{BaseUrl}/{id}/dependencies", new { predecessorActivities = predecessors, successorActivities = successors }, cancellationToken);
    }

    public async Task RecalculateCriticalPathAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        await _apiService.PostAsync($"{BaseUrl}/project/{projectId}/recalculate-critical-path", new { }, cancellationToken);
    }

    public async Task AssignResourcesAsync(Guid activityId, List<Guid> resourceIds, CancellationToken cancellationToken = default)
    {
        await _apiService.PostAsync($"{BaseUrl}/{activityId}/resources", new { resourceIds }, cancellationToken);
    }

    public async Task UpdateResourceRateAsync(Guid activityId, decimal rate, CancellationToken cancellationToken = default)
    {
        await _apiService.PutAsync($"{BaseUrl}/{activityId}/resource-rate", new { rate }, cancellationToken);
    }

    public async Task<bool> ValidateActivityCodeAsync(string code, Guid projectId, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var queryParams = new Dictionary<string, string?>
        {
            ["code"] = code,
            ["projectId"] = projectId.ToString(),
            ["excludeId"] = excludeId?.ToString()
        };
        return await _apiService.GetAsync<bool>($"{BaseUrl}/validate-code", queryParams, cancellationToken);
    }

    public async Task<bool> CanDeleteActivityAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _apiService.GetAsync<bool>($"{BaseUrl}/{id}/can-delete", cancellationToken);
    }

    public async Task<List<string>> ValidateDependenciesAsync(Guid activityId, string[] predecessors, string[] successors, CancellationToken cancellationToken = default)
    {
        return await _apiService.PostAsync<object, List<string>>($"{BaseUrl}/validate-dependencies", 
            new { activityId, predecessorActivities = predecessors, successorActivities = successors }, cancellationToken);
    }
}