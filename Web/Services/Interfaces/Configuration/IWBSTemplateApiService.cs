using Core.DTOs.Common;
using Core.DTOs.Configuration.WBSTemplates;

namespace Web.Services.Interfaces.Configuration;

public interface IWBSTemplateApiService
{
    Task<PagedResult<WBSTemplateDto>> GetTemplatesAsync(int pageNumber = 1, int pageSize = 20, string? searchTerm = null, string? industryType = null, string? projectType = null);
    Task<WBSTemplateDto?> GetTemplateByIdAsync(Guid id);
    Task<WBSTemplateDto?> GetTemplateByCodeAsync(string code);
    Task<IEnumerable<WBSTemplateDto>> GetActiveTemplatesAsync();
    Task<IEnumerable<WBSTemplateDto>> GetTemplatesByIndustryTypeAsync(string industryType);
    Task<IEnumerable<WBSTemplateDto>> GetTemplatesByProjectTypeAsync(string projectType);
    Task<WBSTemplateDto?> CreateTemplateAsync(CreateWBSTemplateDto dto);
    Task<WBSTemplateDto?> UpdateTemplateAsync(Guid id, UpdateWBSTemplateDto dto);
    Task<bool> DeleteTemplateAsync(Guid id);
    Task<bool> ActivateTemplateAsync(Guid id);
    Task<bool> DeactivateTemplateAsync(Guid id);
    
    // Element management
    Task<IEnumerable<WBSTemplateElementDto>> GetTemplateElementsAsync(Guid templateId);
    Task<WBSTemplateElementDto?> AddElementAsync(Guid templateId, WBSTemplateElementDto element);
    Task<bool> UpdateElementAsync(Guid elementId, WBSTemplateElementDto element);
    Task<bool> RemoveElementAsync(Guid templateId, Guid elementId);
    Task<bool> ReorderElementsAsync(Guid templateId, List<Guid> elementIds);
    
    // Template operations
    Task<WBSTemplateDto?> CloneTemplateAsync(Guid sourceId, string newCode, string newName);
    Task<int> ApplyTemplateToProjectAsync(Guid templateId, Guid projectId, bool includeOptional = false);
    Task<TemplateValidationResult> ValidateTemplateAsync(Guid templateId);
    Task<TemplateUsageStatisticsDto> GetUsageStatisticsAsync(Guid templateId);
    
    // Import/Export
    Task<WBSTemplateDto?> ImportTemplateAsync(ImportWBSTemplateDto dto);
    Task<byte[]> ExportTemplateAsync(Guid templateId, string format = "Excel");
    
    // Validation
    Task<bool> IsCodeUniqueAsync(string code, Guid? excludeId = null);
}