using Web.Models.Responses;
using Web.Services.Interfaces;

namespace Web.Services.Implementation
{
    public class ProjectService : IProjectService
    {
        private readonly IApiService _apiService;
        private readonly ILogger<ProjectService> _logger;
        private const string BaseEndpoint = "/api/projects";

        public ProjectService(IApiService apiService, ILogger<ProjectService> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        // Consultas básicas
        public async Task<ApiResponse<List<ProjectListDto>>> GetProjectsAsync()
        {
            try
            {
                return await _apiService.GetAsync<List<ProjectListDto>>(BaseEndpoint);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener proyectos");
                return ApiResponse<List<ProjectListDto>>.ErrorResponse("Error al obtener proyectos");
            }
        }

        public async Task<ApiResponse<List<ProjectListDto>>> GetActiveProjectsAsync()
        {
            try
            {
                return await _apiService.GetAsync<List<ProjectListDto>>($"{BaseEndpoint}/active");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener proyectos activos");
                return ApiResponse<List<ProjectListDto>>.ErrorResponse("Error al obtener proyectos activos");
            }
        }

        public async Task<ApiResponse<List<ProjectListDto>>> GetMyProjectsAsync()
        {
            try
            {
                return await _apiService.GetAsync<List<ProjectListDto>>($"{BaseEndpoint}/my-projects");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener mis proyectos");
                return ApiResponse<List<ProjectListDto>>.ErrorResponse("Error al obtener sus proyectos");
            }
        }

        public async Task<ApiResponse<ProjectDto>> GetProjectByIdAsync(Guid id)
        {
            try
            {
                return await _apiService.GetAsync<ProjectDto>($"{BaseEndpoint}/{id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el proyecto {ProjectId}", id);
                return ApiResponse<ProjectDto>.ErrorResponse("Error al obtener el proyecto");
            }
        }

        public async Task<ApiResponse<ProjectSummaryDto>> GetProjectSummaryAsync(Guid id)
        {
            try
            {
                return await _apiService.GetAsync<ProjectSummaryDto>($"{BaseEndpoint}/{id}/summary");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el resumen del proyecto {ProjectId}", id);
                return ApiResponse<ProjectSummaryDto>.ErrorResponse("Error al obtener el resumen del proyecto");
            }
        }

        // Búsqueda y filtrado
        public async Task<ApiResponse<PagedResult<ProjectListDto>>> SearchProjectsAsync(
            ProjectFilterDto filter, int pageNumber, int pageSize)
        {
            try
            {
                var endpoint = $"{BaseEndpoint}/search?pageNumber={pageNumber}&pageSize={pageSize}";
                return await _apiService.PostAsync<PagedResult<ProjectListDto>>(endpoint, filter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar proyectos");
                return ApiResponse<PagedResult<ProjectListDto>>.ErrorResponse("Error al buscar proyectos");
            }
        }

        // CRUD
        public async Task<ApiResponse<ProjectDto>> CreateProjectAsync(CreateProjectDto dto)
        {
            try
            {
                return await _apiService.PostAsync<ProjectDto>(BaseEndpoint, dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el proyecto");
                return ApiResponse<ProjectDto>.ErrorResponse("Error al crear el proyecto");
            }
        }

        public async Task<ApiResponse<ProjectDto>> UpdateProjectAsync(Guid id, UpdateProjectDto dto)
        {
            try
            {
                return await _apiService.PutAsync<ProjectDto>($"{BaseEndpoint}/{id}", dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el proyecto {ProjectId}", id);
                return ApiResponse<ProjectDto>.ErrorResponse("Error al actualizar el proyecto");
            }
        }

        public async Task<ApiResponse<bool>> DeleteProjectAsync(Guid id)
        {
            try
            {
                return await _apiService.DeleteAsync($"{BaseEndpoint}/{id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el proyecto {ProjectId}", id);
                return ApiResponse<bool>.ErrorResponse("Error al eliminar el proyecto");
            }
        }

        // Gestión de estado
        public async Task<ApiResponse<ProjectDto>> StartProjectAsync(Guid id)
        {
            try
            {
                return await _apiService.PostAsync<ProjectDto>($"{BaseEndpoint}/{id}/start", new { });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al iniciar el proyecto {ProjectId}", id);
                return ApiResponse<ProjectDto>.ErrorResponse("Error al iniciar el proyecto");
            }
        }

        public async Task<ApiResponse<ProjectDto>> CompleteProjectAsync(Guid id)
        {
            try
            {
                return await _apiService.PostAsync<ProjectDto>($"{BaseEndpoint}/{id}/complete", new { });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al completar el proyecto {ProjectId}", id);
                return ApiResponse<ProjectDto>.ErrorResponse("Error al completar el proyecto");
            }
        }

        public async Task<ApiResponse<ProjectDto>> PutProjectOnHoldAsync(Guid id, HoldProjectDto dto)
        {
            try
            {
                return await _apiService.PostAsync<ProjectDto>($"{BaseEndpoint}/{id}/hold", dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al poner en espera el proyecto {ProjectId}", id);
                return ApiResponse<ProjectDto>.ErrorResponse("Error al poner el proyecto en espera");
            }
        }

        public async Task<ApiResponse<ProjectDto>> CancelProjectAsync(Guid id, CancelProjectDto dto)
        {
            try
            {
                return await _apiService.PostAsync<ProjectDto>($"{BaseEndpoint}/{id}/cancel", dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cancelar el proyecto {ProjectId}", id);
                return ApiResponse<ProjectDto>.ErrorResponse("Error al cancelar el proyecto");
            }
        }

        // Progreso
        public async Task<ApiResponse<ProjectDto>> UpdateProjectProgressAsync(Guid id, UpdateProjectProgressDto dto)
        {
            try
            {
                return await _apiService.PatchAsync<ProjectDto>($"{BaseEndpoint}/{id}/progress", dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el progreso del proyecto {ProjectId}", id);
                return ApiResponse<ProjectDto>.ErrorResponse("Error al actualizar el progreso");
            }
        }

        // Gestión de equipo
        public async Task<ApiResponse<List<ProjectTeamMemberDto>>> GetProjectTeamMembersAsync(Guid projectId)
        {
            try
            {
                return await _apiService.GetAsync<List<ProjectTeamMemberDto>>($"{BaseEndpoint}/{projectId}/team");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el equipo del proyecto {ProjectId}", projectId);
                return ApiResponse<List<ProjectTeamMemberDto>>.ErrorResponse("Error al obtener el equipo");
            }
        }

        public async Task<ApiResponse<bool>> AssignProjectTeamMemberAsync(Guid projectId, AssignProjectTeamMemberDto dto)
        {
            try
            {
                return await _apiService.PostAsync<bool>($"{BaseEndpoint}/{projectId}/team", dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al asignar miembro al proyecto {ProjectId}", projectId);
                return ApiResponse<bool>.ErrorResponse("Error al asignar el miembro del equipo");
            }
        }

        public async Task<ApiResponse<bool>> RemoveProjectTeamMemberAsync(Guid projectId, Guid userId)
        {
            try
            {
                return await _apiService.DeleteAsync($"{BaseEndpoint}/{projectId}/team/{userId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al remover miembro del proyecto {ProjectId}", projectId);
                return ApiResponse<bool>.ErrorResponse("Error al remover el miembro del equipo");
            }
        }

        // Validaciones
        public async Task<ApiResponse<bool>> CheckProjectCodeAsync(string code, Guid? excludeId = null)
        {
            try
            {
                var endpoint = $"{BaseEndpoint}/check-code/{Uri.EscapeDataString(code)}";
                if (excludeId.HasValue)
                {
                    endpoint += $"?excludeId={excludeId.Value}";
                }
                return await _apiService.GetAsync<bool>(endpoint);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar el código del proyecto {Code}", code);
                return ApiResponse<bool>.ErrorResponse("Error al verificar el código");
            }
        }
    }
}