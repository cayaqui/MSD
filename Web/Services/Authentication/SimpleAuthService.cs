using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Security.Claims;
using Web.Services.Interfaces;

namespace Web.Services.Authentication
{
    public class SimpleAuthService : ISimpleAuthService
    {
        private readonly NavigationManager _navigation;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly IRemoteAuthenticationService<RemoteAuthenticationState> _remoteAuth;
        private readonly ILoggingService _logger;

        public SimpleAuthService(
            NavigationManager navigation,
            AuthenticationStateProvider authStateProvider,
            IRemoteAuthenticationService<RemoteAuthenticationState> remoteAuth,
            ILoggingService logger)
        {
            _navigation = navigation;
            _authStateProvider = authStateProvider;
            _remoteAuth = remoteAuth;
            _logger = logger;
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            try
            {
                var authState = await _authStateProvider.GetAuthenticationStateAsync();
                return authState.User?.Identity?.IsAuthenticated == true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar autenticación");
                return false;
            }
        }

        public async Task<ClaimsPrincipal> GetCurrentUserAsync()
        {
            try
            {
                var authState = await _authStateProvider.GetAuthenticationStateAsync();
                return authState.User;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario actual");
                return new ClaimsPrincipal(new ClaimsIdentity());
            }
        }

        public async Task<string> GetUserNameAsync()
        {
            try
            {
                var user = await GetCurrentUserAsync();
                if (user?.Identity?.IsAuthenticated == true)
                {
                    // Intenta obtener el nombre en este orden
                    return user.FindFirst("name")?.Value
                        ?? user.FindFirst(ClaimTypes.Name)?.Value
                        ?? user.FindFirst("preferred_username")?.Value
                        ?? user.Identity.Name
                        ?? "Usuario";
                }
                return "Usuario";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener nombre de usuario");
                return "Usuario";
            }
        }

        public async Task<string> GetUserEmailAsync()
        {
            try
            {
                var user = await GetCurrentUserAsync();
                if (user?.Identity?.IsAuthenticated == true)
                {
                    // Intenta obtener el email en este orden
                    return user.FindFirst("email")?.Value
                        ?? user.FindFirst(ClaimTypes.Email)?.Value
                        ?? user.FindFirst("preferred_username")?.Value
                        ?? user.FindFirst("upn")?.Value
                        ?? "";
                }
                return "";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener email de usuario");
                return "";
            }
        }

        public void Login(string? returnUrl = null)
        {
            try
            {
                _logger.LogInfo("Iniciando proceso de login...");
                var absoluteUri = _navigation.ToAbsoluteUri(_navigation.Uri);
                var loginUrl = $"authentication/login?returnUrl={Uri.EscapeDataString(returnUrl ?? absoluteUri.PathAndQuery)}";
                _navigation.NavigateTo(loginUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al iniciar login");
            }
        }

        public void Logout()
        {
            try
            {
                _logger.LogInfo("Iniciando proceso de logout...");
                _navigation.NavigateTo($"authentication/logout");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al iniciar logout");
            }
        }

        public async Task<bool> TryLoginAsync()
        {
            try
            {
                _logger.LogInfo("Intentando login silencioso...");
                var result = await _remoteAuth.SignInAsync(new RemoteAuthenticationContext<RemoteAuthenticationState>
                {
                    State = new RemoteAuthenticationState()
                });

                return result.Status == RemoteAuthenticationStatus.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en login silencioso");
                return false;
            }
        }
    }

    public interface ISimpleAuthService
    {
        Task<bool> IsAuthenticatedAsync();
        Task<ClaimsPrincipal> GetCurrentUserAsync();
        Task<string> GetUserNameAsync();
        Task<string> GetUserEmailAsync();
        void Login(string? returnUrl = null);
        void Logout();
        Task<bool> TryLoginAsync();
    }
}