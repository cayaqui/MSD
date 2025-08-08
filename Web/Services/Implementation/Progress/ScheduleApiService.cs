using Core.DTOs.Common;
using Core.DTOs.Progress.Schedules;
using System.Net.Http.Headers;
using Web.Services.Interfaces;
using Web.Services.Interfaces.Progress;

namespace Web.Services.Implementation.Progress;

public class ScheduleApiService : IScheduleApiService
{
    private readonly IApiService _apiService;
    private const string BaseUrl = "api/schedules";

    public ScheduleApiService(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<PagedResult<ScheduleVersionDto>> GetScheduleVersionsAsync(ScheduleFilterDto filter, CancellationToken cancellationToken = default)
    {
        var queryParams = new Dictionary<string, string?>
        {
            ["projectId"] = filter.ProjectId?.ToString(),
            ["status"] = filter.Status?.ToString(),
            ["version"] = filter.Version,
            ["searchTerm"] = filter.SearchTerm,
            ["sortBy"] = filter.SortBy,
            ["sortDescending"] = filter.SortDescending.ToString(),
            ["pageNumber"] = filter.PageNumber.ToString(),
            ["pageSize"] = filter.PageSize.ToString()
        };

        return await _apiService.GetAsync<PagedResult<ScheduleVersionDto>>(BaseUrl, queryParams, cancellationToken);
    }

    public async Task<ScheduleVersionDto?> GetScheduleVersionByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _apiService.GetAsync<ScheduleVersionDto>($"{BaseUrl}/{id}", cancellationToken);
    }

    public async Task<ScheduleVersionDto?> GetCurrentScheduleAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        return await _apiService.GetAsync<ScheduleVersionDto>($"{BaseUrl}/project/{projectId}/current", cancellationToken);
    }

    public async Task<ScheduleVersionDto?> GetBaselineScheduleAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        return await _apiService.GetAsync<ScheduleVersionDto>($"{BaseUrl}/project/{projectId}/baseline", cancellationToken);
    }

    public async Task<Guid> CreateScheduleVersionAsync(CreateScheduleVersionDto dto, CancellationToken cancellationToken = default)
    {
        return await _apiService.PostAsync<CreateScheduleVersionDto, Guid>(BaseUrl, dto, cancellationToken);
    }

    public async Task UpdateScheduleVersionAsync(Guid id, UpdateScheduleVersionDto dto, CancellationToken cancellationToken = default)
    {
        await _apiService.PutAsync($"{BaseUrl}/{id}", dto, cancellationToken);
    }

    public async Task DeleteScheduleVersionAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _apiService.DeleteAsync($"{BaseUrl}/{id}", cancellationToken);
    }

    public async Task SubmitForApprovalAsync(Guid scheduleId, CancellationToken cancellationToken = default)
    {
        await _apiService.PostAsync($"{BaseUrl}/{scheduleId}/submit", new { }, cancellationToken);
    }

    public async Task ApproveScheduleAsync(Guid scheduleId, ApproveScheduleDto dto, CancellationToken cancellationToken = default)
    {
        await _apiService.PostAsync($"{BaseUrl}/{scheduleId}/approve", dto, cancellationToken);
    }

    public async Task SetAsBaselineAsync(Guid scheduleId, CancellationToken cancellationToken = default)
    {
        await _apiService.PostAsync($"{BaseUrl}/{scheduleId}/baseline", new { }, cancellationToken);
    }

    public async Task<Guid> ImportScheduleAsync(Guid projectId, string version, string name, string sourceSystem, Stream fileStream, string fileName, CancellationToken cancellationToken = default)
    {
        using var content = new MultipartFormDataContent();
        
        var fileContent = new StreamContent(fileStream);
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
        content.Add(fileContent, "file", fileName);
        content.Add(new StringContent(projectId.ToString()), "projectId");
        content.Add(new StringContent(version), "version");
        content.Add(new StringContent(name), "name");
        content.Add(new StringContent(sourceSystem), "sourceSystem");

        return await _apiService.PostMultipartAsync<Guid>($"{BaseUrl}/import", content, cancellationToken);
    }

    public async Task<byte[]> ExportScheduleAsync(Guid scheduleId, string format = "MSProject", CancellationToken cancellationToken = default)
    {
        var queryParams = new Dictionary<string, string?> { ["format"] = format };
        return await _apiService.GetBytesAsync($"{BaseUrl}/{scheduleId}/export", queryParams, cancellationToken);
    }

    public async Task<byte[]> DownloadScheduleTemplateAsync(string format = "MSProject", CancellationToken cancellationToken = default)
    {
        var queryParams = new Dictionary<string, string?> { ["format"] = format };
        return await _apiService.GetBytesAsync($"{BaseUrl}/template", queryParams, cancellationToken);
    }

    public async Task<ScheduleComparisonDto> CompareSchedulesAsync(Guid baselineId, Guid currentId, CancellationToken cancellationToken = default)
    {
        var queryParams = new Dictionary<string, string?>
        {
            ["baselineId"] = baselineId.ToString(),
            ["currentId"] = currentId.ToString()
        };
        return await _apiService.GetAsync<ScheduleComparisonDto>($"{BaseUrl}/compare", queryParams, cancellationToken);
    }

    public async Task<List<ScheduleVarianceDto>> GetScheduleVariancesAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        return await _apiService.GetAsync<List<ScheduleVarianceDto>>($"{BaseUrl}/project/{projectId}/variances", cancellationToken);
    }

    public async Task<bool> CanCreateNewVersionAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        return await _apiService.GetAsync<bool>($"{BaseUrl}/project/{projectId}/can-create-version", cancellationToken);
    }

    public async Task<bool> CanSetAsBaselineAsync(Guid scheduleId, CancellationToken cancellationToken = default)
    {
        return await _apiService.GetAsync<bool>($"{BaseUrl}/{scheduleId}/can-baseline", cancellationToken);
    }
}