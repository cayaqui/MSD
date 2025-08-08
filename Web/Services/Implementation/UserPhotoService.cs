using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Core.DTOs.Auth.Users;
using Web.Services.Interfaces;

namespace Web.Services.Implementation;

public class UserPhotoService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IAccessTokenProvider _tokenProvider;
    private readonly ILoggingService _logger;

    public UserPhotoService(IHttpClientFactory httpClientFactory, IAccessTokenProvider tokenProvider, ILoggingService logger)
    {
        _httpClientFactory = httpClientFactory;
        _tokenProvider = tokenProvider;
        _logger = logger;
    }

    public async Task<(bool Success, UserDto? User)> SyncUserWithPhotoAsync()
    {
        try
        {
            _logger.LogInfo("Iniciando sincronización de usuario con foto...");
            
            // Obtener el HttpClient configurado
            var httpClient = _httpClientFactory.CreateClient("EzProAPI");
            
            // Obtener token de acceso
            var tokenResult = await _tokenProvider.RequestAccessToken();
            if (!tokenResult.TryGetToken(out var token))
            {
                _logger.LogWarning("No se pudo obtener el token de acceso");
                return (false, null);
            }
            _logger.LogInfo("Iniciando sincronización de usuario con foto...");
            // Configurar el header de autorización
            httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", token.Value);

            // Llamar al endpoint de sincronización con timeout
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            var response = await httpClient.PostAsync("api/auth/sync", null, cts.Token);
            
            if (response.IsSuccessStatusCode)
            {
                var user = await response.Content.ReadFromJsonAsync<UserDto>();
                
                if (user != null)
                {
                    _logger.LogInfo($"Usuario sincronizado exitosamente: {user.DisplayName}");
                    if (!string.IsNullOrEmpty(user.PhotoUrl))
                    {
                        _logger.LogInfo("Foto de usuario disponible en: " + user.PhotoUrl);
                    }
                    else
                    {
                        _logger.LogInfo("El usuario no tiene foto disponible");
                    }
                }
                
                return (true, user);
            }
            else
            {
                _logger.LogWarning($"Error al sincronizar usuario: {response.StatusCode}");
                return (false, null);
            }
        }
        catch (TaskCanceledException)
        {
            _logger.LogWarning("Timeout al sincronizar usuario con Azure AD");
            return (false, null);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning($"Error de conexión al sincronizar usuario: {ex.Message}");
            return (false, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante la sincronización del usuario");
            return (false, null);
        }
    }

    public async Task<UserDto?> GetCurrentUserAsync()
    {
        try
        {
            // Obtener el HttpClient configurado
            var httpClient = _httpClientFactory.CreateClient("EzProAPI");
            
            // Obtener token de acceso
            var tokenResult = await _tokenProvider.RequestAccessToken();
            if (!tokenResult.TryGetToken(out var token))
            {
                _logger.LogWarning("No se pudo obtener el token de acceso");
                return null;
            }

            // Configurar el header de autorización
            httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", token.Value);

            // Llamar al endpoint para obtener el usuario actual con timeout
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            var response = await httpClient.GetAsync("api/auth/me", cts.Token);
            
            if (response.IsSuccessStatusCode)
            {
                var user = await response.Content.ReadFromJsonAsync<UserDto>();
                return user;
            }
            else
            {
                _logger.LogWarning($"Error al obtener usuario actual: {response.StatusCode}");
                return null;
            }
        }
        catch (TaskCanceledException)
        {
            _logger.LogWarning("Timeout al obtener usuario actual");
            return null;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning($"Error de conexión al obtener usuario: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el usuario actual");
            return null;
        }
    }
}