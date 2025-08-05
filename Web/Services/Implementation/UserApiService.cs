using System.Net.Http.Json;
using Web.Services.Interfaces;
using Core.DTOs.Auth.Users;
using Core.DTOs.Common;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Web.Services.Implementation
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
                var authState = await _authStateProvider.GetAuthenticationStateAsync();
                var user = authState.User;
                
                if (!user.Identity?.IsAuthenticated ?? true)
                {
                    _logger.LogWarning("Usuario no autenticado");
                    return null;
                }

                // Obtener EntraId del usuario actual
                var entraId = user.FindFirst("oid")?.Value 
                    ?? user.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
                    
                if (string.IsNullOrEmpty(entraId))
                {
                    _logger.LogWarning("No se pudo obtener EntraId del usuario");
                    return null;
                }

                // Buscar usuario por EntraId
                var response = await _apiService.GetAsync<UserDto>($"{BaseUrl}/by-entra-id/{entraId}");
                if (response != null)
                {
                    return response;
                }

                _logger.LogWarning($"Usuario con EntraId {entraId} no encontrado en la base de datos");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario actual");
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

        public async Task<string?> GetUserPhotoAsync(Guid userId)
        {
            try
            {
                _logger.LogInfo($"Obteniendo foto del usuario {userId}...");
                
                // Llamar al endpoint de foto del API
                var response = await _apiService.GetAsync<UserPhotoResponse>($"{BaseUrl}/{userId}/photo");
                
                if (response != null)
                {
                    _logger.LogInfo("Foto del usuario obtenida exitosamente");
                    return response.PhotoDataUrl;
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
    }

    public interface IUserApiService
    {
        Task<UserDto?> GetCurrentUserAsync();
        Task<UserDto?> GetUserByIdAsync(Guid id);
        Task<UserDto?> GetUserByEmailAsync(string email);
        Task<UserDto?> GetUserByEntraIdAsync(string entraId);
        Task<UserDto?> SyncUserWithAzureAsync(Guid userId);
        Task<string?> GetUserPhotoAsync(Guid userId);
        Task<PagedResult<UserDto>> SearchUsersAsync(UserFilterDto filter);
        Task<UserDto?> CreateUserAsync(CreateUserDto dto);
        Task<UserDto?> UpdateUserAsync(Guid id, UpdateUserDto dto);
        Task<bool> ActivateUserAsync(Guid id);
        Task<bool> DeactivateUserAsync(Guid id);
        Task<bool> CheckEmailExistsAsync(string email);
    }

    public class UserPhotoResponse
    {
        public string? PhotoDataUrl { get; set; }
        public string? ContentType { get; set; }
    }
}