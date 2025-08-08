using Core.DTOs.Common;
using Core.DTOs.Configuration.WBSTemplates;
using Web.Services.Interfaces;
using Web.Services.Interfaces.Configuration;

namespace Web.Services.Implementation.Configuration;

public class WBSTemplateApiService : IWBSTemplateApiService
{
    private readonly IApiService _apiService;
    private readonly ILoggingService _logger;
    private const string BaseEndpoint = "api/configuration/wbs-templates";

    public WBSTemplateApiService(IApiService apiService, ILoggingService logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    public async Task<PagedResult<WBSTemplateDto>> GetTemplatesAsync(int pageNumber = 1, int pageSize = 20, string? searchTerm = null, string? industryType = null, string? projectType = null)
    {
        try
        {
            var queryParams = new Dictionary<string, string>
            {
                ["pageNumber"] = pageNumber.ToString(),
                ["pageSize"] = pageSize.ToString()
            };

            if (!string.IsNullOrWhiteSpace(searchTerm))
                queryParams["searchTerm"] = searchTerm;
            if (!string.IsNullOrWhiteSpace(industryType))
                queryParams["industryType"] = industryType;
            if (!string.IsNullOrWhiteSpace(projectType))
                queryParams["projectType"] = projectType;

            var queryString = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));
            var result = await _apiService.GetAsync<PagedResult<WBSTemplateDto>>($"{BaseEndpoint}?{queryString}");
            return result ?? new PagedResult<WBSTemplateDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting WBS templates");
            return new PagedResult<WBSTemplateDto>();
        }
    }

    public async Task<WBSTemplateDto?> GetTemplateByIdAsync(Guid id)
    {
        try
        {
            return await _apiService.GetAsync<WBSTemplateDto>($"{BaseEndpoint}/{id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting WBS template {id}");
            return null;
        }
    }

    public async Task<WBSTemplateDto?> GetTemplateByCodeAsync(string code)
    {
        try
        {
            return await _apiService.GetAsync<WBSTemplateDto>($"{BaseEndpoint}/by-code/{code}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting WBS template by code {code}");
            return null;
        }
    }

    public async Task<IEnumerable<WBSTemplateDto>> GetActiveTemplatesAsync()
    {
        try
        {
            var result = await _apiService.GetAsync<IEnumerable<WBSTemplateDto>>($"{BaseEndpoint}/active");
            return result ?? Enumerable.Empty<WBSTemplateDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active WBS templates");
            return Enumerable.Empty<WBSTemplateDto>();
        }
    }

    public async Task<IEnumerable<WBSTemplateDto>> GetTemplatesByIndustryTypeAsync(string industryType)
    {
        try
        {
            var result = await _apiService.GetAsync<IEnumerable<WBSTemplateDto>>($"{BaseEndpoint}/by-industry/{industryType}");
            return result ?? Enumerable.Empty<WBSTemplateDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting WBS templates for industry {industryType}");
            return Enumerable.Empty<WBSTemplateDto>();
        }
    }

    public async Task<IEnumerable<WBSTemplateDto>> GetTemplatesByProjectTypeAsync(string projectType)
    {
        try
        {
            var result = await _apiService.GetAsync<IEnumerable<WBSTemplateDto>>($"{BaseEndpoint}/by-project-type/{projectType}");
            return result ?? Enumerable.Empty<WBSTemplateDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting WBS templates for project type {projectType}");
            return Enumerable.Empty<WBSTemplateDto>();
        }
    }

    public async Task<WBSTemplateDto?> CreateTemplateAsync(CreateWBSTemplateDto dto)
    {
        try
        {
            return await _apiService.PostAsync<CreateWBSTemplateDto, WBSTemplateDto>(BaseEndpoint, dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating WBS template");
            return null;
        }
    }

    public async Task<WBSTemplateDto?> UpdateTemplateAsync(Guid id, UpdateWBSTemplateDto dto)
    {
        try
        {
            return await _apiService.PutAsync<UpdateWBSTemplateDto, WBSTemplateDto>($"{BaseEndpoint}/{id}", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating WBS template {id}");
            return null;
        }
    }

    public async Task<bool> DeleteTemplateAsync(Guid id)
    {
        try
        {
            await _apiService.DeleteAsync($"{BaseEndpoint}/{id}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting WBS template {id}");
            return false;
        }
    }

    public async Task<bool> ActivateTemplateAsync(Guid id)
    {
        try
        {
            await _apiService.PostAsync<object>($"{BaseEndpoint}/{id}/activate", new { });
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error activating WBS template {id}");
            return false;
        }
    }

    public async Task<bool> DeactivateTemplateAsync(Guid id)
    {
        try
        {
            await _apiService.PostAsync<object>($"{BaseEndpoint}/{id}/deactivate", new { });
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deactivating WBS template {id}");
            return false;
        }
    }

    public async Task<IEnumerable<WBSTemplateElementDto>> GetTemplateElementsAsync(Guid templateId)
    {
        try
        {
            var result = await _apiService.GetAsync<IEnumerable<WBSTemplateElementDto>>($"{BaseEndpoint}/{templateId}/elements");
            return result ?? Enumerable.Empty<WBSTemplateElementDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting elements for template {templateId}");
            return Enumerable.Empty<WBSTemplateElementDto>();
        }
    }

    public async Task<WBSTemplateElementDto?> AddElementAsync(Guid templateId, WBSTemplateElementDto element)
    {
        try
        {
            return await _apiService.PostAsync<WBSTemplateElementDto, WBSTemplateElementDto>($"{BaseEndpoint}/{templateId}/elements", element);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error adding element to template {templateId}");
            return null;
        }
    }

    public async Task<bool> UpdateElementAsync(Guid elementId, WBSTemplateElementDto element)
    {
        try
        {
            await _apiService.PutAsync<WBSTemplateElementDto>($"{BaseEndpoint}/elements/{elementId}", element);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating element {elementId}");
            return false;
        }
    }

    public async Task<bool> RemoveElementAsync(Guid templateId, Guid elementId)
    {
        try
        {
            await _apiService.DeleteAsync($"{BaseEndpoint}/{templateId}/elements/{elementId}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error removing element {elementId} from template {templateId}");
            return false;
        }
    }

    public async Task<bool> ReorderElementsAsync(Guid templateId, List<Guid> elementIds)
    {
        try
        {
            await _apiService.PostAsync<List<Guid>>($"{BaseEndpoint}/{templateId}/elements/reorder", elementIds);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error reordering elements in template {templateId}");
            return false;
        }
    }

    public async Task<WBSTemplateDto?> CloneTemplateAsync(Guid sourceId, string newCode, string newName)
    {
        try
        {
            var dto = new { NewCode = newCode, NewName = newName };
            return await _apiService.PostAsync<object, WBSTemplateDto>($"{BaseEndpoint}/{sourceId}/clone", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error cloning template {sourceId}");
            return null;
        }
    }

    public async Task<int> ApplyTemplateToProjectAsync(Guid templateId, Guid projectId, bool includeOptional = false)
    {
        try
        {
            var dto = new { ProjectId = projectId, IncludeOptional = includeOptional };
            // The API returns the count directly in Results.Ok(count)
            var result = await _apiService.PostAsync<object, int>($"{BaseEndpoint}/{templateId}/apply", dto);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error applying template {templateId} to project {projectId}");
            return 0;
        }
    }

    public async Task<TemplateValidationResult> ValidateTemplateAsync(Guid templateId)
    {
        try
        {
            var result = await _apiService.GetAsync<TemplateValidationResult>($"{BaseEndpoint}/{templateId}/validate");
            return result ?? new TemplateValidationResult { IsValid = false, Errors = new List<string> { "Validation failed" } };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error validating template {templateId}");
            return new TemplateValidationResult { IsValid = false, Errors = new List<string> { ex.Message } };
        }
    }

    public async Task<TemplateUsageStatisticsDto> GetUsageStatisticsAsync(Guid templateId)
    {
        try
        {
            var result = await _apiService.GetAsync<TemplateUsageStatisticsDto>($"{BaseEndpoint}/{templateId}/statistics");
            return result ?? new TemplateUsageStatisticsDto();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting usage statistics for template {templateId}");
            return new TemplateUsageStatisticsDto();
        }
    }

    public async Task<WBSTemplateDto?> ImportTemplateAsync(ImportWBSTemplateDto dto)
    {
        try
        {
            return await _apiService.PostAsync<ImportWBSTemplateDto, WBSTemplateDto>($"{BaseEndpoint}/import", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing WBS template");
            return null;
        }
    }

    public async Task<byte[]> ExportTemplateAsync(Guid templateId, string format = "Excel")
    {
        try
        {
            // Use GetBytesAsync for file downloads
            return await _apiService.GetBytesAsync($"{BaseEndpoint}/{templateId}/export?format={format}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error exporting template {templateId}");
            return Array.Empty<byte>();
        }
    }

    public async Task<bool> IsCodeUniqueAsync(string code, Guid? excludeId = null)
    {
        try
        {
            var queryString = excludeId.HasValue ? $"?excludeId={excludeId}" : "";
            var result = await _apiService.GetAsync<bool>($"{BaseEndpoint}/check-code/{code}{queryString}");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error checking code uniqueness for {code}");
            return false;
        }
    }
}