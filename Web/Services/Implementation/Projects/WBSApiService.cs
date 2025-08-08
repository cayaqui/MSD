using Core.DTOs.Common;
using Core.DTOs.Projects.WBSElement;
using Core.DTOs.Projects.WorkPackageDetails;
using Core.Enums.Projects;
using Core.Enums.Documents;
using System.Net.Http.Json;
using Web.Services.Interfaces;
using Web.Services.Interfaces.Projects;

namespace Web.Services.Implementation.Projects;

public class WBSApiService : IWBSApiService
{
    private readonly IApiService _apiService;
    private const string BaseUrl = "api/wbs";

    public WBSApiService(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<List<WBSElementDto>> GetWBSHierarchyAsync(Guid projectId)
    {
        return await _apiService.GetAsync<List<WBSElementDto>>($"{BaseUrl}/project/{projectId}/hierarchy");
    }

    public async Task<WBSElementDetailDto> GetWBSElementByIdAsync(Guid id)
    {
        return await _apiService.GetAsync<WBSElementDetailDto>($"{BaseUrl}/{id}");
    }

    public async Task<Guid> CreateWBSElementAsync(CreateWBSElementDto dto)
    {
        var response = await _apiService.PostAsync<CreateWBSElementDto, Guid>(BaseUrl, dto);
        return response;
    }

    public async Task UpdateWBSElementAsync(Guid id, UpdateWBSElementDto dto)
    {
        await _apiService.PutAsync($"{BaseUrl}/{id}", dto);
    }

    public async Task DeleteWBSElementAsync(Guid id)
    {
        await _apiService.DeleteAsync($"{BaseUrl}/{id}");
    }

    public async Task<WBSDictionaryDto> GetWBSDictionaryAsync(Guid elementId)
    {
        return await _apiService.GetAsync<WBSDictionaryDto>($"{BaseUrl}/{elementId}/dictionary");
    }

    public async Task UpdateWBSDictionaryAsync(Guid elementId, UpdateWBSDictionaryDto dto)
    {
        await _apiService.PutAsync($"{BaseUrl}/{elementId}/dictionary", dto);
    }

    public async Task ConvertToWorkPackageAsync(Guid elementId, ConvertToWorkPackageDto dto)
    {
        await _apiService.PostAsync($"{BaseUrl}/{elementId}/convert-to-work-package", dto);
    }

    public async Task ConvertToPlanningPackageAsync(Guid elementId, Guid controlAccountId)
    {
        await _apiService.PostAsync($"{BaseUrl}/{elementId}/convert-to-planning-package", new { controlAccountId });
    }

    public async Task<ImportResultDto> ImportWBSAsync(Guid projectId, Stream fileStream, string fileName)
    {
        using var content = new MultipartFormDataContent();
        using var streamContent = new StreamContent(fileStream);
        streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
        content.Add(streamContent, "file", fileName);
        content.Add(new StringContent(projectId.ToString()), "projectId");

        var response = await _apiService.PostFileAsync<ImportResultDto>($"{BaseUrl}/import", content);
        return response;
    }

    public async Task<byte[]> ExportWBSAsync(Guid projectId, ExportFormat format = ExportFormat.Excel)
    {
        var bytes = await _apiService.GetBytesAsync($"{BaseUrl}/project/{projectId}/export?format={format}");
        return bytes;
    }

    public async Task<byte[]> DownloadWBSTemplateAsync()
    {
        var bytes = await _apiService.GetBytesAsync($"{BaseUrl}/template");
        return bytes;
    }

    public async Task ReorderWBSElementsAsync(ReorderWBSElementsDto dto)
    {
        await _apiService.PostAsync($"{BaseUrl}/reorder", dto);
    }
}