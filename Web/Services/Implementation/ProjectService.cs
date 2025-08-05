using Core.DTOs.Organization.Project;
using Core.DTOs.Common;
using Web.Services.Interfaces;

namespace Web.Services.Implementation;

public class ProjectService : IProjectService
{
    private readonly IApiService _apiService;
    
    public ProjectService(IApiService apiService)
    {
        _apiService = apiService;
    }
    
    public async Task<PagedResult<ProjectListDto>> GetProjectsAsync(ProjectFilterDto? filter = null)
    {
        var queryString = filter != null ? $"?{BuildQueryString(filter)}" : "";
        return await _apiService.GetAsync<PagedResult<ProjectListDto>>($"api/projects{queryString}") 
            ?? new PagedResult<ProjectListDto>();
    }
    
    public async Task<ProjectDto?> GetProjectByIdAsync(Guid projectId)
    {
        return await _apiService.GetAsync<ProjectDto>($"api/projects/{projectId}");
    }
    
    public async Task<ProjectDto?> CreateProjectAsync(CreateProjectDto dto)
    {
        return await _apiService.PostAsync<CreateProjectDto, ProjectDto>("api/projects", dto);
    }
    
    public async Task<ProjectDto?> UpdateProjectAsync(Guid projectId, UpdateProjectDto dto)
    {
        return await _apiService.PutAsync<UpdateProjectDto, ProjectDto>($"api/projects/{projectId}", dto);
    }
    
    public async Task<bool> DeleteProjectAsync(Guid projectId)
    {
        return await _apiService.DeleteAsync($"api/projects/{projectId}");
    }
    
    private string BuildQueryString(ProjectFilterDto filter)
    {
        var parameters = new List<string>();
        
        if (!string.IsNullOrEmpty(filter.SearchTerm))
            parameters.Add($"searchTerm={Uri.EscapeDataString(filter.SearchTerm)}");
            
        if (!string.IsNullOrEmpty(filter.Status))
            parameters.Add($"status={filter.Status}");
            
        if (filter.CompanyId.HasValue)
            parameters.Add($"companyId={filter.CompanyId.Value}");
            
        if (filter.PageNumber > 0)
            parameters.Add($"pageNumber={filter.PageNumber}");
            
        if (filter.PageSize > 0)
            parameters.Add($"pageSize={filter.PageSize}");
            
        return string.Join("&", parameters);
    }
}