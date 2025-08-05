using Application.Interfaces.Common;
using Core.DTOs.Configuration.WBSTemplates;

namespace Application.Interfaces.Configuration;

/// <summary>
/// Service for managing WBS templates
/// </summary>
public interface IWBSTemplateService : IBaseService<WBSTemplateDto, CreateWBSTemplateDto, UpdateWBSTemplateDto>
{
    /// <summary>
    /// Get template by code
    /// </summary>
    Task<WBSTemplateDto?> GetByCodeAsync(string code);
    
    /// <summary>
    /// Get active templates
    /// </summary>
    Task<IEnumerable<WBSTemplateDto>> GetActiveAsync();
    
    /// <summary>
    /// Get templates by industry type
    /// </summary>
    Task<IEnumerable<WBSTemplateDto>> GetByIndustryTypeAsync(string industryType);
    
    /// <summary>
    /// Get templates by project type
    /// </summary>
    Task<IEnumerable<WBSTemplateDto>> GetByProjectTypeAsync(string projectType);
    
    /// <summary>
    /// Search templates
    /// </summary>
    Task<IEnumerable<WBSTemplateDto>> SearchAsync(WBSCBSTemplateFilterDto filter);
    
    /// <summary>
    /// Get template elements
    /// </summary>
    Task<IEnumerable<WBSTemplateElementDto>> GetElementsAsync(Guid templateId);
    
    /// <summary>
    /// Add element to template
    /// </summary>
    Task<WBSTemplateElementDto?> AddElementAsync(Guid templateId, WBSTemplateElementDto element);
    
    /// <summary>
    /// Update template element
    /// </summary>
    Task<bool> UpdateElementAsync(Guid elementId, WBSTemplateElementDto element);
    
    /// <summary>
    /// Remove element from template
    /// </summary>
    Task<bool> RemoveElementAsync(Guid templateId, Guid elementId);
    
    /// <summary>
    /// Reorder elements
    /// </summary>
    Task<bool> ReorderElementsAsync(Guid templateId, List<Guid> elementIds);
    
    /// <summary>
    /// Apply template to project
    /// </summary>
    Task<int> ApplyToProjectAsync(Guid templateId, Guid projectId, bool includeOptional = false);
    
    /// <summary>
    /// Clone template
    /// </summary>
    Task<WBSTemplateDto?> CloneAsync(Guid sourceId, string newCode, string newName);
    
    /// <summary>
    /// Activate template
    /// </summary>
    Task<bool> ActivateAsync(Guid id);
    
    /// <summary>
    /// Deactivate template
    /// </summary>
    Task<bool> DeactivateAsync(Guid id);
    
    /// <summary>
    /// Import template
    /// </summary>
    Task<WBSTemplateDto?> ImportAsync(ImportWBSTemplateDto dto);
    
    /// <summary>
    /// Export template
    /// </summary>
    Task<byte[]> ExportAsync(Guid templateId, string format = "Excel");
    
    /// <summary>
    /// Validate template structure
    /// </summary>
    Task<TemplateValidationResult> ValidateStructureAsync(Guid templateId);
    
    /// <summary>
    /// Check if code is unique
    /// </summary>
    Task<bool> IsCodeUniqueAsync(string code, Guid? excludeId = null);
    
    /// <summary>
    /// Get template usage statistics
    /// </summary>
    Task<TemplateUsageStatisticsDto> GetUsageStatisticsAsync(Guid templateId);
}
