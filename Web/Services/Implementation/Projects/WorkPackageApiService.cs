using Core.DTOs.Common;
using Core.DTOs.WorkPackages;
using Core.DTOs.Projects.WorkPackageDetails;
using Web.Services.Interfaces;
using Web.Services.Interfaces.Projects;

namespace Web.Services.Implementation.Projects;

public class WorkPackageApiService : IWorkPackageApiService
{
    private readonly IApiService _apiService;
    private const string BaseUrl = "api/work-packages";

    public WorkPackageApiService(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<PagedResult<WorkPackageDto>> GetWorkPackagesByProjectAsync(Guid projectId, QueryParameters parameters)
    {
        var queryString = $"?PageNumber={parameters.PageNumber}&PageSize={parameters.PageSize}";
        if (!string.IsNullOrEmpty(parameters.SearchTerm))
            queryString += $"&SearchTerm={Uri.EscapeDataString(parameters.SearchTerm)}";
        if (!string.IsNullOrEmpty(parameters.SortBy))
            queryString += $"&SortBy={Uri.EscapeDataString(parameters.SortBy)}";
        queryString += $"&SortDirection={parameters.SortDirection}";

        return await _apiService.GetAsync<PagedResult<WorkPackageDto>>($"{BaseUrl}/project/{projectId}{queryString}");
    }

    public async Task<WorkPackageDetailDto> GetWorkPackageByIdAsync(Guid id)
    {
        return await _apiService.GetAsync<WorkPackageDetailDto>($"{BaseUrl}/{id}");
    }

    public async Task<List<WorkPackageDto>> GetWorkPackagesByControlAccountAsync(Guid controlAccountId)
    {
        return await _apiService.GetAsync<List<WorkPackageDto>>($"{BaseUrl}/control-account/{controlAccountId}");
    }

    public async Task<List<WorkPackageProgressDto>> GetWorkPackageProgressHistoryAsync(Guid id)
    {
        return await _apiService.GetAsync<List<WorkPackageProgressDto>>($"{BaseUrl}/{id}/progress-history");
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

    public async Task UpdateWorkPackageProgressAsync(Guid id, UpdateWorkPackageProgressDto dto)
    {
        await _apiService.PutAsync($"{BaseUrl}/{id}/progress", dto);
    }

    public async Task StartWorkPackageAsync(Guid id)
    {
        await _apiService.PostAsync($"{BaseUrl}/{id}/start", new { });
    }

    public async Task CompleteWorkPackageAsync(Guid id)
    {
        await _apiService.PostAsync($"{BaseUrl}/{id}/complete", new { });
    }

    public async Task BaselineWorkPackageAsync(Guid id)
    {
        await _apiService.PostAsync($"{BaseUrl}/{id}/baseline", new { });
    }

    public async Task DeleteWorkPackageAsync(Guid id)
    {
        await _apiService.DeleteAsync($"{BaseUrl}/{id}");
    }

    public async Task<Guid> AddActivityToWorkPackageAsync(Guid workPackageId, CreateActivityDto dto)
    {
        var response = await _apiService.PostAsync<CreateActivityDto, Guid>($"{BaseUrl}/{workPackageId}/activities", dto);
        return response;
    }

    public async Task UpdateActivityProgressAsync(Guid activityId, decimal percentComplete, decimal actualHours)
    {
        await _apiService.PutAsync($"{BaseUrl}/activities/{activityId}/progress", new 
        { 
            PercentComplete = percentComplete, 
            ActualHours = actualHours 
        });
    }

    public async Task<byte[]> ExportWorkPackagesAsync(Guid projectId)
    {
        return await _apiService.GetBytesAsync($"{BaseUrl}/project/{projectId}/export");
    }
}