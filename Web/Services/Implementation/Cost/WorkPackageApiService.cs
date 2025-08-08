using Core.DTOs.Common;
using Core.DTOs.Cost.WorkPackages;
using Core.DTOs.Projects.WorkPackageDetails;
using System.Text;
using System.Text.Json;
using Web.Services.Interfaces;
using Web.Services.Interfaces.Cost;

namespace Web.Services.Implementation.Cost;

public class WorkPackageApiService : IWorkPackageApiService
{
    private readonly IApiService _apiService;
    private const string BaseUrl = "api/work-packages";

    public WorkPackageApiService(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<PagedResult<WorkPackageDto>> GetWorkPackagesAsync(WorkPackageFilterDto filter)
    {
        var queryParams = BuildQueryString(filter);
        return await _apiService.GetAsync<PagedResult<WorkPackageDto>>($"{BaseUrl}?{queryParams}");
    }

    public async Task<WorkPackageDetailsDto> GetWorkPackageByIdAsync(Guid id)
    {
        return await _apiService.GetAsync<WorkPackageDetailsDto>($"{BaseUrl}/{id}");
    }

    public async Task<Guid> CreateWorkPackageAsync(CreateWorkPackageDto dto)
    {
        var response = await _apiService.PostAsync<CreateWorkPackageDto, Guid>(BaseUrl, dto);
        return response;
    }

    public async Task UpdateWorkPackageAsync(Guid id, UpdateWorkPackageDto dto)
    {
        await _apiService.PutAsync($"{BaseUrl}/{id}", dto);
    }

    public async Task DeleteWorkPackageAsync(Guid id)
    {
        await _apiService.DeleteAsync($"{BaseUrl}/{id}");
    }

    public async Task UpdateProgressAsync(Guid id, UpdateWorkPackageProgressDto dto)
    {
        await _apiService.PutAsync($"{BaseUrl}/{id}/progress", dto);
    }

    public async Task<List<WorkPackageActivityDto>> GetActivitiesAsync(Guid workPackageId)
    {
        return await _apiService.GetAsync<List<WorkPackageActivityDto>>($"{BaseUrl}/{workPackageId}/activities");
    }

    public async Task<Guid> AddActivityAsync(Guid workPackageId, CreateActivityDto dto)
    {
        var response = await _apiService.PostAsync<CreateActivityDto, Guid>($"{BaseUrl}/{workPackageId}/activities", dto);
        return response;
    }

    public async Task UpdateActivityAsync(Guid workPackageId, Guid activityId, UpdateActivityDto dto)
    {
        await _apiService.PutAsync($"{BaseUrl}/{workPackageId}/activities/{activityId}", dto);
    }

    public async Task DeleteActivityAsync(Guid workPackageId, Guid activityId)
    {
        await _apiService.DeleteAsync($"{BaseUrl}/{workPackageId}/activities/{activityId}");
    }

    public async Task RecalculateProgressAsync(Guid workPackageId)
    {
        await _apiService.PostAsync($"{BaseUrl}/{workPackageId}/recalculate-progress", new { });
    }

    private string BuildQueryString(WorkPackageFilterDto filter)
    {
        var parameters = new List<string>();

        if (filter.ProjectId.HasValue)
            parameters.Add($"ProjectId={filter.ProjectId}");
        if (!string.IsNullOrEmpty(filter.SearchTerm))
            parameters.Add($"SearchTerm={Uri.EscapeDataString(filter.SearchTerm)}");
        if (filter.ControlAccountId.HasValue)
            parameters.Add($"ControlAccountId={filter.ControlAccountId}");
        if (filter.ResponsibleUserId.HasValue)
            parameters.Add($"ResponsibleUserId={filter.ResponsibleUserId}");
        if (!string.IsNullOrEmpty(filter.Status))
            parameters.Add($"Status={filter.Status}");
        if (filter.OnlyActive.HasValue)
            parameters.Add($"OnlyActive={filter.OnlyActive}");
        
        parameters.Add($"PageNumber={filter.PageNumber}");
        parameters.Add($"PageSize={filter.PageSize}");
        
        if (!string.IsNullOrEmpty(filter.SortBy))
            parameters.Add($"SortBy={Uri.EscapeDataString(filter.SortBy)}");
        
        parameters.Add($"SortDirection={Uri.EscapeDataString(filter.SortDirection)}");

        return string.Join("&", parameters);
    }
}