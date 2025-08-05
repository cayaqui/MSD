using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Headers;
using System.Security.Claims;
using Web.Services.Interfaces;

namespace Web.Services.Implementation
{
    public class UserSyncService : IUserSyncService
    {
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILoggingService _logger;
        private readonly IConfiguration _configuration;
        private readonly IJSRuntime _jsRuntime;
        
        public UserSyncService(
            AuthenticationStateProvider authStateProvider,
            IHttpClientFactory httpClientFactory,
            ILoggingService logger,
            IConfiguration configuration,
            IJSRuntime jsRuntime)
        {
            _authStateProvider = authStateProvider;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _configuration = configuration;
            _jsRuntime = jsRuntime;
        }

        public async Task<UserSyncResult> SyncUserAsync()
        {
            try
            {
                _logger.LogInfo("Iniciando sincronización de usuario con Entra ID...");
                
                var authState = await _authStateProvider.GetAuthenticationStateAsync();
                var user = authState.User;
                
                if (!user.Identity?.IsAuthenticated ?? true)
                {
                    _logger.LogWarning("Usuario no autenticado - no se puede sincronizar");
                    return new UserSyncResult { Success = false, Message = "Usuario no autenticado" };
                }

                // Extraer información del usuario de los claims
                var userInfo = ExtractUserInfo(user);
                
                // Intentar obtener la foto del perfil
                userInfo.PhotoBase64 = await GetUserPhotoAsync(user);
                
                // Si no se pudo obtener con el método estándar, intentar con JavaScript
                if (string.IsNullOrEmpty(userInfo.PhotoBase64))
                {
                    try
                    {
                        _logger.LogInfo("Intentando obtener foto mediante JavaScript interop...");
                        userInfo.PhotoBase64 = await _jsRuntime.InvokeAsync<string?>("GraphHelper.getUserPhoto");
                        if (!string.IsNullOrEmpty(userInfo.PhotoBase64))
                        {
                            _logger.LogInfo("Foto obtenida exitosamente mediante JavaScript");
                        }
                    }
                    catch (Exception jsEx)
                    {
                        _logger.LogError(jsEx, "Error al obtener foto mediante JavaScript");
                    }
                }
                
                _logger.LogInfo($"Sincronización completada para usuario: {userInfo.DisplayName}");
                
                return new UserSyncResult
                {
                    Success = true,
                    UserInfo = userInfo,
                    Message = "Sincronización exitosa"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante la sincronización del usuario");
                return new UserSyncResult 
                { 
                    Success = false, 
                    Message = $"Error: {ex.Message}" 
                };
            }
        }

        private UserSyncInfo ExtractUserInfo(ClaimsPrincipal user)
        {
            return new UserSyncInfo
            {
                ObjectId = user.FindFirst("oid")?.Value ?? user.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value ?? "",
                DisplayName = user.FindFirst("name")?.Value ?? user.Identity?.Name ?? "Usuario",
                Email = user.FindFirst("email")?.Value 
                    ?? user.FindFirst("preferred_username")?.Value
                    ?? user.FindFirst("upn")?.Value 
                    ?? "",
                GivenName = user.FindFirst("given_name")?.Value ?? user.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname")?.Value ?? "",
                Surname = user.FindFirst("family_name")?.Value ?? user.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname")?.Value ?? "",
                JobTitle = user.FindFirst("jobTitle")?.Value ?? "",
                Department = user.FindFirst("department")?.Value ?? "",
                OfficeLocation = user.FindFirst("officeLocation")?.Value ?? "",
                MobilePhone = user.FindFirst("mobilePhone")?.Value ?? "",
                BusinessPhones = user.FindFirst("businessPhones")?.Value?.Split(',').ToList() ?? new List<string>(),
                Roles = user.FindAll("roles").Select(c => c.Value).ToList()
            };
        }

        private async Task<string?> GetUserPhotoAsync(ClaimsPrincipal user)
        {
            try
            {
                _logger.LogInfo("Intentando obtener foto del usuario desde Microsoft Graph...");
                
                // Obtener el token de acceso
                var accessToken = await GetAccessTokenAsync();
                if (string.IsNullOrEmpty(accessToken))
                {
                    _logger.LogWarning("No se pudo obtener token de acceso para Graph API");
                    return null;
                }

                // Crear cliente HTTP
                var httpClient = _httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                
                // Llamar a Graph API para obtener la foto
                var response = await httpClient.GetAsync("https://graph.microsoft.com/v1.0/me/photo/$value");
                
                if (response.IsSuccessStatusCode)
                {
                    var photoBytes = await response.Content.ReadAsByteArrayAsync();
                    var base64Photo = Convert.ToBase64String(photoBytes);
                    
                    _logger.LogInfo("Foto del usuario obtenida exitosamente");
                    return $"data:image/jpeg;base64,{base64Photo}";
                }
                else
                {
                    _logger.LogWarning($"No se pudo obtener la foto del usuario: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la foto del usuario");
                return null;
            }
        }

        private async Task<string?> GetAccessTokenAsync()
        {
            try
            {
                _logger.LogDebug("Intentando obtener token de acceso mediante JavaScript interop...");
                
                // Usar JavaScript para obtener el token de acceso
                var token = await _jsRuntime.InvokeAsync<string?>("GraphHelper.getAccessToken");
                
                if (!string.IsNullOrEmpty(token))
                {
                    _logger.LogInfo("Token de acceso obtenido exitosamente");
                    return token;
                }
                else
                {
                    _logger.LogWarning("No se pudo obtener el token de acceso");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener token de acceso");
                return null;
            }
        }
    }

    public interface IUserSyncService
    {
        Task<UserSyncResult> SyncUserAsync();
    }

    public class UserSyncResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public UserSyncInfo? UserInfo { get; set; }
    }

    public class UserSyncInfo
    {
        public string ObjectId { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string Email { get; set; } = "";
        public string GivenName { get; set; } = "";
        public string Surname { get; set; } = "";
        public string JobTitle { get; set; } = "";
        public string Department { get; set; } = "";
        public string OfficeLocation { get; set; } = "";
        public string MobilePhone { get; set; } = "";
        public List<string> BusinessPhones { get; set; } = new();
        public List<string> Roles { get; set; } = new();
        public string? PhotoBase64 { get; set; }
    }
}