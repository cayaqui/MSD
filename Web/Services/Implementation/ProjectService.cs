using Core.DTOs.Common;
using Core.DTOs.Organization.Project;
using Web.Services.Interfaces;
using Web.Services.Interfaces.Organization;

namespace Web.Services.Implementation;

/// <summary>
/// Wrapper service for project operations that uses IProjectApiService internally
/// </summary>
public class ProjectService : IProjectService
{
    private readonly IProjectApiService _projectApiService;

    public ProjectService(IProjectApiService projectApiService)
    {
        _projectApiService = projectApiService;
    }

    public async Task<PagedResult<ProjectListDto>> GetProjectsAsync(ProjectFilterDto? filter = null)
    {
        return await _projectApiService.GetProjectsAsync(filter);
    }

    public async Task<ProjectDto?> GetProjectByIdAsync(Guid projectId)
    {
        return await _projectApiService.GetProjectByIdAsync(projectId);
    }

    public async Task<ProjectDto?> CreateProjectAsync(CreateProjectDto dto)
    {
        return await _projectApiService.CreateProjectAsync(dto);
    }

    public async Task<ProjectDto?> UpdateProjectAsync(Guid projectId, UpdateProjectDto dto)
    {
        return await _projectApiService.UpdateProjectAsync(projectId, dto);
    }

    public async Task<bool> DeleteProjectAsync(Guid projectId)
    {
        return await _projectApiService.DeleteProjectAsync(projectId);
    }
}