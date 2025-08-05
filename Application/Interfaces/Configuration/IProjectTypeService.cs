using Application.Interfaces.Common;
using Core.DTOs.Configuration.ProjectTypes;

namespace Application.Interfaces.Configuration;

/// <summary>
/// Service for managing project types
/// </summary>
public interface IProjectTypeService : IBaseService<ProjectTypeDto, CreateProjectTypeDto, UpdateProjectTypeDto>
{
    /// <summary>
    /// Get project type by code
    /// </summary>
    Task<ProjectTypeDto?> GetByCodeAsync(string code);
    
    /// <summary>
    /// Get active project types
    /// </summary>
    Task<IEnumerable<ProjectTypeDto>> GetActiveAsync();
    
    /// <summary>
    /// Search project types
    /// </summary>
    Task<IEnumerable<ProjectTypeDto>> SearchAsync(ProjectTypeFilterDto filter);
    
    /// <summary>
    /// Activate a project type
    /// </summary>
    Task<bool> ActivateAsync(Guid id);
    
    /// <summary>
    /// Deactivate a project type
    /// </summary>
    Task<bool> DeactivateAsync(Guid id);
    
    /// <summary>
    /// Set templates for a project type
    /// </summary>
    Task<bool> SetTemplatesAsync(Guid id, Guid? wbsTemplateId, Guid? cbsTemplateId, Guid? obsTemplateId, Guid? riskRegisterTemplateId);
    
    /// <summary>
    /// Set workflow stages
    /// </summary>
    Task<bool> SetWorkflowStagesAsync(Guid id, List<WorkflowStageDto> stages);
    
    /// <summary>
    /// Set approval levels
    /// </summary>
    Task<bool> SetApprovalLevelsAsync(Guid id, List<string> approvalLevels);
    
    /// <summary>
    /// Clone a project type
    /// </summary>
    Task<ProjectTypeDto?> CloneAsync(Guid sourceId, string newCode, string newName);
    
    /// <summary>
    /// Check if code is unique
    /// </summary>
    Task<bool> IsCodeUniqueAsync(string code, Guid? excludeId = null);
}