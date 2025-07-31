using Core.Dtos.Files;
using Core.DTOs.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Security.Claims;
using Web.Services.Interfaces;

namespace Web.Services.Implementation
{
    /// <summary>
    /// Implementación del servicio de autenticación
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly IAccessTokenProvider _tokenProvider;
        private readonly IApiService _apiService;
        private UserDto? _currentUser;

        public AuthService(
            AuthenticationStateProvider authenticationStateProvider,
            IAccessTokenProvider tokenProvider,
            IApiService apiService)
        {
            _authenticationStateProvider = authenticationStateProvider;
            _tokenProvider = tokenProvider;
            _apiService = apiService;
        }

        public async Task InitializeAsync()
        {
            // Cargar datos del usuario actual si está autenticado
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            if (authState.User.Identity?.IsAuthenticated == true)
            {
                await RefreshUserDataAsync();
            }
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            return authState.User.Identity?.IsAuthenticated ?? false;
        }

        public async Task<string?> GetAccessTokenAsync()
        {
            var result = await _tokenProvider.RequestAccessToken();
            if (result.TryGetToken(out var token))
            {
                return token.Value;
            }
            return null;
        }

        public async Task<ClaimsPrincipal> GetCurrentUserPrincipalAsync()
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            return authState.User;
        }

        public async Task<UserDto?> GetCurrentUserAsync()
        {
            if (_currentUser != null)
                return _currentUser;

            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            if (!authState.User.Identity?.IsAuthenticated ?? true)
                return null;

            // Intentar obtener datos del usuario desde el API
            try
            {
                var response = await _apiService.GetAsync<UserDto>("api/users/me");
                if (response.IsSuccess && response.Data != null)
                {
                    _currentUser = response.Data;
                    return _currentUser;
                }
            }
            catch
            {
                // Si falla, crear un DTO básico desde los claims
                _currentUser = CreateUserFromClaims(authState.User);
            }

            return _currentUser;
        }

        public string GetUserName()
        {
            var authState = _authenticationStateProvider.GetAuthenticationStateAsync().Result;
            return authState.User.Identity?.Name ?? "Usuario";
        }

        public string GetUserEmail()
        {
            var authState = _authenticationStateProvider.GetAuthenticationStateAsync().Result;
            return authState.User.FindFirst(ClaimTypes.Email)?.Value
                ?? authState.User.FindFirst("email")?.Value
                ?? authState.User.FindFirst("preferred_username")?.Value
                ?? "";
        }

        public string GetUserInitials()
        {
            var name = GetUserName();
            if (string.IsNullOrWhiteSpace(name))
                return "??";

            var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
                return $"{parts[0][0]}{parts[parts.Length - 1][0]}".ToUpper();

            return name.Substring(0, Math.Min(2, name.Length)).ToUpper();
        }

        public async Task<bool> IsInRoleAsync(string role)
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            return authState.User.IsInRole(role);
        }

        public async Task<bool> HasPermissionAsync(Guid? projectId, string permission)
        {
            // Verificar si el usuario está autenticado
            if (!await IsAuthenticatedAsync())
                return false;

            var user = await GetCurrentUserAsync();
            if (user == null)
                return false;

            // Si el usuario es administrador del sistema, tiene todos los permisos
            if (await IsInRoleAsync("System.Admin") || await IsInRoleAsync("Admin"))
                return true;

            // Si no se especifica un proyecto, verificar permisos globales
            if (!projectId.HasValue)
            {
                return await CheckGlobalPermissionAsync(user, permission);
            }

            // Si se especifica un proyecto, verificar permisos del proyecto
            return await CheckProjectPermissionAsync(user, projectId.Value, permission);
        }

        private async Task<bool> CheckGlobalPermissionAsync(UserDto user, string permission)
        {
            // Verificar permisos basados en roles globales
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var userRoles = authState.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

            // Mapear roles a permisos (simplificado - en producción esto vendría del backend)
            var rolePermissions = new Dictionary<string, List<string>>
            {
                ["ProjectManager"] = new List<string>
                {
                    "project.view", "project.create", "project.edit",
                    "project.wbs.view", "project.wbs.create", "project.wbs.edit",
                    "project.schedule.view", "project.schedule.edit",
                    "project.cost.view", "project.cost.edit",
                    "project.contract.view", "project.contract.create",
                    "project.document.view", "project.document.upload",
                    "project.quality.view", "project.risk.view",
                    "report.project.view", "report.kpis.view"
                },
                ["TeamMember"] = new List<string>
                {
                    "project.view",
                    "project.wbs.view",
                    "project.schedule.view",
                    "project.cost.view",
                    "project.document.view",
                    "project.quality.view",
                    "project.risk.view"
                },
                ["CompanyManager"] = new List<string>
                {
                    "company.view", "company.create", "company.edit",
                    "company.operations.view", "company.operations.manage"
                }
            };

            // Verificar si alguno de los roles del usuario tiene el permiso
            foreach (var role in userRoles)
            {
                if (rolePermissions.ContainsKey(role) && rolePermissions[role].Contains(permission))
                {
                    return true;
                }
            }

            // Verificar permisos específicos del usuario desde claims
            var permissionClaim = authState.User.FindFirst($"permission:{permission}");
            return permissionClaim != null;
        }

        private async Task<bool> CheckProjectPermissionAsync(UserDto user, Guid projectId, string permission)
        {
            try
            {
                // Llamar al API para verificar permisos específicos del proyecto
                var response = await _apiService.GetAsync<bool>($"api/projects/{projectId}/permissions/check?permission={permission}");
                return response.IsSuccess && response.Data;
            }
            catch
            {
                // Si falla la verificación remota, recurrir a permisos globales
                return await CheckGlobalPermissionAsync(user, permission);
            }
        }

        public async Task RefreshUserDataAsync()
        {
            _currentUser = null;
            await GetCurrentUserAsync();
        }

        private UserDto CreateUserFromClaims(ClaimsPrincipal principal)
        {
            return new UserDto
            {
                Id = Guid.Parse(principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString()),
                Email = GetUserEmail(),
                GivenName = principal.FindFirst(ClaimTypes.GivenName)?.Value ?? GetUserName().Split(' ').FirstOrDefault() ?? "",
                Surname = principal.FindFirst(ClaimTypes.Surname)?.Value ?? GetUserName().Split(' ').LastOrDefault() ?? "",
                IsActive = true,
            };
        }
    }
}