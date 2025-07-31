using Web.Models.Responses;

namespace Web.Services.Interfaces
{
    public interface IProjectService
    {
        // Consultas básicas
        Task<ApiResponse<List<ProjectListDto>>> GetProjectsAsync();
        Task<ApiResponse<List<ProjectListDto>>> GetActiveProjectsAsync();
        Task<ApiResponse<List<ProjectListDto>>> GetMyProjectsAsync();
        Task<ApiResponse<ProjectDto>> GetProjectByIdAsync(Guid id);
        Task<ApiResponse<ProjectSummaryDto>> GetProjectSummaryAsync(Guid id);

        // Búsqueda y filtrado
        Task<ApiResponse<PagedResult<ProjectListDto>>> SearchProjectsAsync(ProjectFilterDto filter, int pageNumber, int pageSize);

        // CRUD
        Task<ApiResponse<ProjectDto>> CreateProjectAsync(CreateProjectDto dto);
        Task<ApiResponse<ProjectDto>> UpdateProjectAsync(Guid id, UpdateProjectDto dto);
        Task<ApiResponse<bool>> DeleteProjectAsync(Guid id);

        // Gestión de estado
        Task<ApiResponse<ProjectDto>> StartProjectAsync(Guid id);
        Task<ApiResponse<ProjectDto>> CompleteProjectAsync(Guid id);
        Task<ApiResponse<ProjectDto>> PutProjectOnHoldAsync(Guid id, HoldProjectDto dto);
        Task<ApiResponse<ProjectDto>> CancelProjectAsync(Guid id, CancelProjectDto dto);

        // Progreso
        Task<ApiResponse<ProjectDto>> UpdateProjectProgressAsync(Guid id, UpdateProjectProgressDto dto);

        // Gestión de equipo
        Task<ApiResponse<List<ProjectTeamMemberDto>>> GetProjectTeamMembersAsync(Guid projectId);
        Task<ApiResponse<bool>> AssignProjectTeamMemberAsync(Guid projectId, AssignProjectTeamMemberDto dto);
        Task<ApiResponse<bool>> RemoveProjectTeamMemberAsync(Guid projectId, Guid userId);

        // Validaciones
        Task<ApiResponse<bool>> CheckProjectCodeAsync(string code, Guid? excludeId = null);
    }
}