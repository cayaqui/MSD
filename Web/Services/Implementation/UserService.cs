using Web.Models.Responses;
using Web.Services.Interfaces;

namespace Web.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly IApiService _apiService;
        private readonly ILogger<UserService> _logger;
        private const string BaseEndpoint = "/api/users";

        public UserService(IApiService apiService, ILogger<UserService> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<ApiResponse<PagedResult<UserDto>>> GetUsersAsync(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var endpoint = $"{BaseEndpoint}?pageNumber={pageNumber}&pageSize={pageSize}";
                return await _apiService.GetAsync<PagedResult<UserDto>>(endpoint);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuarios");
                return ApiResponse<PagedResult<UserDto>>.ErrorResponse("Error al obtener usuarios");
            }
        }

        public async Task<ApiResponse<PagedResult<UserDto>>> SearchUsersAsync(UserFilterDto filter)
        {
            try
            {
                return await _apiService.PostAsync<PagedResult<UserDto>>($"{BaseEndpoint}/search", filter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar usuarios");
                return ApiResponse<PagedResult<UserDto>>.ErrorResponse("Error al buscar usuarios");
            }
        }

        public async Task<ApiResponse<UserDto>> GetUserByIdAsync(Guid id)
        {
            try
            {
                return await _apiService.GetAsync<UserDto>($"{BaseEndpoint}/{id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el usuario {UserId}", id);
                return ApiResponse<UserDto>.ErrorResponse("Error al obtener el usuario");
            }
        }

        public async Task<ApiResponse<UserDto>> GetUserByEmailAsync(string email)
        {
            try
            {
                return await _apiService.GetAsync<UserDto>($"{BaseEndpoint}/by-email/{Uri.EscapeDataString(email)}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el usuario por email {Email}", email);
                return ApiResponse<UserDto>.ErrorResponse("Error al obtener el usuario");
            }
        }

        public async Task<ApiResponse<UserDto>> CreateUserAsync(CreateUserDto dto)
        {
            try
            {
                return await _apiService.PostAsync<UserDto>(BaseEndpoint, dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el usuario");
                return ApiResponse<UserDto>.ErrorResponse("Error al crear el usuario");
            }
        }

        public async Task<ApiResponse<UserDto>> UpdateUserAsync(Guid id, UpdateUserDto dto)
        {
            try
            {
                return await _apiService.PutAsync<UserDto>($"{BaseEndpoint}/{id}", dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el usuario {UserId}", id);
                return ApiResponse<UserDto>.ErrorResponse("Error al actualizar el usuario");
            }
        }

        public async Task<ApiResponse<bool>> DeleteUserAsync(Guid id)
        {
            try
            {
                return await _apiService.DeleteAsync($"{BaseEndpoint}/{id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el usuario {UserId}", id);
                return ApiResponse<bool>.ErrorResponse("Error al eliminar el usuario");
            }
        }

        public async Task<ApiResponse<UserDto>> ActivateUserAsync(Guid id)
        {
            try
            {
                return await _apiService.PatchAsync<UserDto>($"{BaseEndpoint}/{id}/activate", new { });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al activar el usuario {UserId}", id);
                return ApiResponse<UserDto>.ErrorResponse("Error al activar el usuario");
            }
        }

        public async Task<ApiResponse<UserDto>> DeactivateUserAsync(Guid id)
        {
            try
            {
                return await _apiService.PatchAsync<UserDto>($"{BaseEndpoint}/{id}/deactivate", new { });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al desactivar el usuario {UserId}", id);
                return ApiResponse<UserDto>.ErrorResponse("Error al desactivar el usuario");
            }
        }

        public async Task<ApiResponse<List<ProjectTeamMemberDto>>> GetUserProjectsAsync(Guid id)
        {
            try
            {
                return await _apiService.GetAsync<List<ProjectTeamMemberDto>>($"{BaseEndpoint}/{id}/projects");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los proyectos del usuario {UserId}", id);
                return ApiResponse<List<ProjectTeamMemberDto>>.ErrorResponse("Error al obtener los proyectos");
            }
        }

        public async Task<ApiResponse<UserDto>> SyncUserWithAzureADAsync(Guid id)
        {
            try
            {
                return await _apiService.PostAsync<UserDto>($"{BaseEndpoint}/{id}/sync", new { });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al sincronizar el usuario {UserId} con Azure AD", id);
                return ApiResponse<UserDto>.ErrorResponse("Error al sincronizar con Azure AD");
            }
        }

        public async Task<ApiResponse<int>> BulkActivateUsersAsync(BulkUserOperationDto dto)
        {
            try
            {
                return await _apiService.PostAsync<int>($"{BaseEndpoint}/bulk/activate", dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al activar usuarios en lote");
                return ApiResponse<int>.ErrorResponse("Error al activar usuarios");
            }
        }

        public async Task<ApiResponse<int>> BulkDeactivateUsersAsync(BulkUserOperationDto dto)
        {
            try
            {
                return await _apiService.PostAsync<int>($"{BaseEndpoint}/bulk/deactivate", dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al desactivar usuarios en lote");
                return ApiResponse<int>.ErrorResponse("Error al desactivar usuarios");
            }
        }
    }
}