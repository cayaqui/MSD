using Core.DTOs.Organization.Project;
using Core.DTOs.Common;
using Core.DTOs.Auth.ProjectTeamMembers;

namespace Web.Services.Interfaces.Organization;

/// <summary>
/// Service interface for project API operations
/// </summary>
public interface IProjectApiService
{
    // Query Operations
    
    /// <summary>
    /// Get all projects with pagination
    /// </summary>
    Task<PagedResult<ProjectListDto>> GetProjectsAsync(ProjectFilterDto? filter = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get project by ID
    /// </summary>
    Task<ProjectDto?> GetProjectByIdAsync(Guid projectId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get projects summary
    /// </summary>
    Task<ProjectSummaryDto?> GetProjectsSummaryAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get project team members
    /// </summary>
    Task<List<ProjectTeamMemberDto>?> GetProjectTeamAsync(Guid projectId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get project status history
    /// </summary>
    Task<List<ProjectStatusHistoryDto>?> GetProjectStatusHistoryAsync(Guid projectId, CancellationToken cancellationToken = default);

    // Command Operations
    
    /// <summary>
    /// Create a new project
    /// </summary>
    Task<ProjectDto?> CreateProjectAsync(CreateProjectDto dto, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Update an existing project
    /// </summary>
    Task<ProjectDto?> UpdateProjectAsync(Guid projectId, UpdateProjectDto dto, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Change project status
    /// </summary>
    Task<ProjectDto?> ChangeProjectStatusAsync(Guid projectId, ChangeProjectStatusDto dto, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Put project on hold
    /// </summary>
    Task<ProjectDto?> HoldProjectAsync(Guid projectId, HoldProjectDto dto, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Cancel project
    /// </summary>
    Task<ProjectDto?> CancelProjectAsync(Guid projectId, CancelProjectDto dto, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Complete project
    /// </summary>
    Task<ProjectDto?> CompleteProjectAsync(Guid projectId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Update project progress
    /// </summary>
    Task<ProjectDto?> UpdateProjectProgressAsync(Guid projectId, UpdateProjectProgressDto dto, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Delete project (soft delete)
    /// </summary>
    Task<bool> DeleteProjectAsync(Guid projectId, CancellationToken cancellationToken = default);
}