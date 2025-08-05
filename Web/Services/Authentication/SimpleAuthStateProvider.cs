using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Web.Services.Authentication;

public class SimpleAuthStateProvider : AuthenticationStateProvider
{
    private readonly ILoggingService _logger;

    public SimpleAuthStateProvider(ILoggingService logger)
    {
        _logger = logger;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            _logger.LogInfo("SimpleAuthStateProvider: Getting authentication state");
            
            // Create an anonymous user for now
            var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
            return Task.FromResult(new AuthenticationState(anonymous));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting authentication state");
            var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
            return Task.FromResult(new AuthenticationState(anonymous));
        }
    }

    public void NotifyUserAuthentication(string userName)
    {
        var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, userName),
            new Claim(ClaimTypes.NameIdentifier, userName)
        }, "simple"));

        var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
        NotifyAuthenticationStateChanged(authState);
    }

    public void NotifyUserLogout()
    {
        var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
        var authState = Task.FromResult(new AuthenticationState(anonymous));
        NotifyAuthenticationStateChanged(authState);
    }
}