namespace Application.Interfaces.Setup;

public interface IProjectService
{
    Task<PagedResult<ProjectListDto>> GetPagedAsync(int pageNumber=1, int pageSize=10, ProjectFilterDto? filter = null);
    Task<IEnumerable<ProjectListDto>> GetUserProjectsAsync();
    Task<ProjectDto?> GetByIdAsync(Guid id);
    Task<ProjectDto> CreateAsync(CreateProjectDto dto);
    Task<ProjectDto> UpdateAsync(Guid id, UpdateProjectDto dto);
    Task DeleteAsync(Guid id);
    Task<bool> IsCodeUniqueAsync(string code, Guid? excludeId = null);
    Task AddTeamMemberAsync(Guid projectId, Guid userId, string role);
    Task RemoveTeamMemberAsync(Guid projectId, Guid userId);
    Task UpdateProgressAsync(Guid projectId, decimal percentage);
   
}