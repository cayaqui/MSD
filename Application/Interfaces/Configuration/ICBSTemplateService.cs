using Application.Interfaces.Common;
using Core.DTOs.Configuration.WBSTemplates;

namespace Application.Interfaces.Configuration;

/// <summary>
/// Service for managing CBS templates
/// </summary>
public interface ICBSTemplateService : IBaseService<CBSTemplateDto, CreateCBSTemplateDto, UpdateCBSTemplateDto>
{
    /// <summary>
    /// Get template by code
    /// </summary>
    Task<CBSTemplateDto?> GetByCodeAsync(string code);
    
    /// <summary>
    /// Get active templates
    /// </summary>
    Task<IEnumerable<CBSTemplateDto>> GetActiveAsync();
    
    /// <summary>
    /// Get templates by industry type
    /// </summary>
    Task<IEnumerable<CBSTemplateDto>> GetByIndustryTypeAsync(string industryType);
    
    /// <summary>
    /// Get templates by cost type
    /// </summary>
    Task<IEnumerable<CBSTemplateDto>> GetByCostTypeAsync(string costType);
    
    /// <summary>
    /// Search templates
    /// </summary>
    Task<IEnumerable<CBSTemplateDto>> SearchAsync(WBSCBSTemplateFilterDto filter);
    
    /// <summary>
    /// Get template elements
    /// </summary>
    Task<IEnumerable<CBSTemplateElementDto>> GetElementsAsync(Guid templateId);
    
    /// <summary>
    /// Add element to template
    /// </summary>
    Task<CBSTemplateElementDto?> AddElementAsync(Guid templateId, CBSTemplateElementDto element);
    
    /// <summary>
    /// Update template element
    /// </summary>
    Task<bool> UpdateElementAsync(Guid elementId, CBSTemplateElementDto element);
    
    /// <summary>
    /// Remove element from template
    /// </summary>
    Task<bool> RemoveElementAsync(Guid templateId, Guid elementId);
    
    /// <summary>
    /// Get control accounts
    /// </summary>
    Task<IEnumerable<CBSTemplateElementDto>> GetControlAccountsAsync(Guid templateId);
    
    /// <summary>
    /// Apply template to project
    /// </summary>
    Task<int> ApplyToProjectAsync(Guid templateId, Guid projectId);
    
    /// <summary>
    /// Clone template
    /// </summary>
    Task<CBSTemplateDto?> CloneAsync(Guid sourceId, string newCode, string newName);
    
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
    Task<CBSTemplateDto?> ImportAsync(byte[] data, string format = "Excel");
    
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
    
    /// <summary>
    /// Map CBS template to WBS elements
    /// </summary>
    Task<int> MapToWBSAsync(Guid templateId, Guid projectId, Dictionary<Guid, List<Guid>> cbsToWbsMapping);
}