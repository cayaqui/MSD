using Core.DTOs.Organization.Project;
using Core.DTOs.Common;

namespace Web.Services.Interfaces;

/// <summary>
/// Service interface for project operations
/// This is a wrapper service that uses IProjectApiService internally
/// </summary>
public interface IProjectService
{
    Task<PagedResult<ProjectListDto>> GetProjectsAsync(ProjectFilterDto? filter = null);
    Task<ProjectDto?> GetProjectByIdAsync(Guid projectId);
    Task<ProjectDto?> CreateProjectAsync(CreateProjectDto dto);
    Task<ProjectDto?> UpdateProjectAsync(Guid projectId, UpdateProjectDto dto);
    Task<bool> DeleteProjectAsync(Guid projectId);
}