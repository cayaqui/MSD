using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Authentication.WebAssembly.Msal.Models;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Web.Services.Interfaces;

namespace Web.Services.Authentication
{
    public class CustomAuthStateProvider : RemoteAuthenticationService<RemoteAuthenticationState, RemoteUserAccount, MsalProviderOptions>
    {
        private readonly ILoggingService _logger;

        public CustomAuthStateProvider(
            IJSRuntime jsRuntime,
            IOptionsSnapshot<RemoteAuthenticationOptions<MsalProviderOptions>> options,
            NavigationManager navigation,
            AccountClaimsPrincipalFactory<RemoteUserAccount> accountClaimsPrincipalFactory,
            ILoggingService logger)
            : base(jsRuntime, options, navigation, accountClaimsPrincipalFactory)
        {
            _logger = logger;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                _logger.LogDebug("Getting authentication state...");
                var authState = await base.GetAuthenticationStateAsync();
                
                if (authState.User?.Identity?.IsAuthenticated == true)
                {
                    _logger.LogInfo($"Usuario autenticado: {authState.User.Identity.Name}");
                    
                    // Log claims para debugging
                    foreach (var claim in authState.User.Claims)
                    {
                        _logger.LogDebug($"Claim: {claim.Type} = {claim.Value}");
                    }
                }
                else
                {
                    _logger.LogDebug("Usuario no autenticado");
                }

                return authState;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el estado de autenticación");
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }

        public override async Task<RemoteAuthenticationResult<RemoteAuthenticationState>> CompleteSignInAsync(RemoteAuthenticationContext<RemoteAuthenticationState> context)
        {
            try
            {
                _logger.LogInfo("Completando inicio de sesión...");
                var result = await base.CompleteSignInAsync(context);
                
                if (result.Status == RemoteAuthenticationStatus.Success)
                {
                    _logger.LogInfo("Inicio de sesión exitoso");
                }
                else
                {
                    _logger.LogWarning($"Inicio de sesión falló con estado: {result.Status}");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al completar el inicio de sesión");
                throw;
            }
        }

        public override async Task<RemoteAuthenticationResult<RemoteAuthenticationState>> CompleteSignOutAsync(RemoteAuthenticationContext<RemoteAuthenticationState> context)
        {
            try
            {
                _logger.LogInfo("Completando cierre de sesión...");
                var result = await base.CompleteSignOutAsync(context);
                
                if (result.Status == RemoteAuthenticationStatus.Success)
                {
                    _logger.LogInfo("Cierre de sesión exitoso");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al completar el cierre de sesión");
                throw;
            }
        }
    }
}