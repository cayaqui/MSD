using Core.DTOs.Auth.ProjectTeamMembers;
using Core.DTOs.Auth.Users;
using Core.DTOs.Auth.Permissions;
using Core.DTOs.Common;
using Web.Services.Interfaces.Auth;
using Microsoft.AspNetCore.Components.Authorization;

namespace Web.Services.Implementation.Auth
{
    public class UserApiService : IUserApiService
    {
        private readonly IApiService _apiService;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly ILoggingService _logger;
        private const string BaseUrl = "api/users";

        public UserApiService(
            IApiService apiService,
            AuthenticationStateProvider authStateProvider,
            ILoggingService logger)
        {
            _apiService = apiService;
            _authStateProvider = authStateProvider;
            _logger = logger;
        }

        public async Task<UserDto?> GetCurrentUserAsync()
        {
            try
            {
                var response = await _apiService.GetAsync<UserDto>("api/auth/me");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario actual");
                return null;
            }
        }

        public async Task<UserPermissionsDto?> GetCurrentUserPermissionsAsync()
        {
            try
            {
                var response = await _apiService.GetAsync<UserPermissionsDto>("api/auth/me/permissions");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener permisos del usuario actual");
                return null;
            }
        }

        public async Task<UserDto?> GetUserByIdAsync(Guid id)
        {
            try
            {
                var response = await _apiService.GetAsync<UserDto>($"{BaseUrl}/{id}");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener usuario {id}");
                return null;
            }
        }

        public async Task<UserDto?> GetUserByEmailAsync(string email)
        {
            try
            {
                var response = await _apiService.GetAsync<UserDto>($"{BaseUrl}/by-email/{email}");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener usuario por email {email}");
                return null;
            }
        }

        public async Task<UserDto?> GetUserByEntraIdAsync(string entraId)
        {
            try
            {
                var response = await _apiService.GetAsync<UserDto>($"{BaseUrl}/by-entra-id/{entraId}");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener usuario por EntraId {entraId}");
                return null;
            }
        }

        public async Task<UserDto?> SyncUserWithAzureAsync(Guid userId)
        {
            try
            {
                _logger.LogInfo($"Sincronizando usuario {userId} con Azure AD...");
                var response = await _apiService.PostAsync<object?, UserDto>($"{BaseUrl}/{userId}/sync", null);
                
                if (response != null)
                {
                    _logger.LogInfo($"Usuario {userId} sincronizado exitosamente");
                    return response;
                }

                _logger.LogWarning($"No se pudo sincronizar usuario {userId}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al sincronizar usuario {userId} con Azure AD");
                return null;
            }
        }


        public async Task<PagedResult<UserDto>> SearchUsersAsync(UserFilterDto filter)
        {
            try
            {
                var response = await _apiService.PostAsync<UserFilterDto, PagedResult<UserDto>>($"{BaseUrl}/search", filter);
                return response ?? new PagedResult<UserDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar usuarios");
                return new PagedResult<UserDto>();
            }
        }

        public async Task<UserDto?> CreateUserAsync(CreateUserDto dto)
        {
            try
            {
                var response = await _apiService.PostAsync<CreateUserDto, UserDto>(BaseUrl, dto);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear usuario");
                return null;
            }
        }

        public async Task<UserDto?> UpdateUserAsync(Guid id, UpdateUserDto dto)
        {
            try
            {
                var response = await _apiService.PutAsync<UpdateUserDto, UserDto>($"{BaseUrl}/{id}", dto);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar usuario {id}");
                return null;
            }
        }

        public async Task<bool> ActivateUserAsync(Guid id)
        {
            try
            {
                var response = await _apiService.PostAsync<object?, UserDto>($"{BaseUrl}/{id}/activate", null);
                return response != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al activar usuario {id}");
                return false;
            }
        }

        public async Task<bool> DeactivateUserAsync(Guid id)
        {
            try
            {
                var response = await _apiService.PostAsync<object?, UserDto>($"{BaseUrl}/{id}/deactivate", null);
                return response != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al desactivar usuario {id}");
                return false;
            }
        }

        public async Task<bool> CheckEmailExistsAsync(string email)
        {
            try
            {
                var response = await _apiService.GetAsync<bool>($"{BaseUrl}/check-email/{email}");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al verificar email {email}");
                return false;
            }
        }
        
        public async Task<PagedResult<UserDto>> GetUsersAsync(int pageNumber = 1, int pageSize = 20)
        {
            try
            {
                var queryParams = $"?pageNumber={pageNumber}&pageSize={pageSize}";
                var response = await _apiService.GetAsync<PagedResult<UserDto>>($"{BaseUrl}{queryParams}");
                return response ?? new PagedResult<UserDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener lista de usuarios");
                return new PagedResult<UserDto>();
            }
        }
        
        public async Task<bool> DeleteUserAsync(Guid id)
        {
            try
            {
                await _apiService.DeleteAsync($"{BaseUrl}/{id}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar usuario {id}");
                return false;
            }
        }
        
        public async Task<BulkOperationResult> BulkActivateUsersAsync(List<Guid> userIds)
        {
            try
            {
                var request = new { UserIds = userIds };
                var response = await _apiService.PostAsync<object, BulkOperationResult>($"{BaseUrl}/bulk/activate", request);
                return response ?? new BulkOperationResult(0, userIds.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al activar usuarios en lote");
                return new BulkOperationResult(0, userIds.Count);
            }
        }
        
        public async Task<BulkOperationResult> BulkDeactivateUsersAsync(List<Guid> userIds)
        {
            try
            {
                var request = new { UserIds = userIds };
                var response = await _apiService.PostAsync<object, BulkOperationResult>($"{BaseUrl}/bulk/deactivate", request);
                return response ?? new BulkOperationResult(0, userIds.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al desactivar usuarios en lote");
                return new BulkOperationResult(0, userIds.Count);
            }
        }
        
        public async Task<List<ProjectTeamMemberDto>> GetUserProjectsAsync(Guid userId)
        {
            try
            {
                var response = await _apiService.GetAsync<List<ProjectTeamMemberDto>>($"{BaseUrl}/{userId}/projects");
                return response ?? new List<ProjectTeamMemberDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener proyectos del usuario {userId}");
                return new List<ProjectTeamMemberDto>();
            }
        }
        
        public async Task<UserPhotoResponse?> GetUserPhotoAsync(Guid userId)
        {
            try
            {
                _logger.LogInfo($"Obteniendo foto del usuario {userId}...");
                var response = await _apiService.GetAsync<dynamic>($"{BaseUrl}/{userId}/photo");
                
                if (response != null)
                {
                    _logger.LogInfo("Foto del usuario obtenida exitosamente");
                    return new UserPhotoResponse(
                        response.ContentType?.ToString() ?? "image/png",
                        response.Data ?? Array.Empty<byte>()
                    );
                }
                
                _logger.LogWarning("No se pudo obtener la foto del usuario");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener foto del usuario");
                return null;
            }
        }
        
        public async Task<CanDeleteResult> CanDeleteUserAsync(Guid userId)
        {
            try
            {
                var response = await _apiService.GetAsync<CanDeleteResult>($"{BaseUrl}/{userId}/can-delete");
                return response ?? new CanDeleteResult(false, "Error al verificar");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al verificar si se puede eliminar usuario {userId}");
                return new CanDeleteResult(false, ex.Message);
            }
        }
    }
}