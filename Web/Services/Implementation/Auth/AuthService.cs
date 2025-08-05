using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components;
using Core.DTOs.Auth.Users;
using System.Linq;
using Web.Services.Interfaces.Auth;

namespace Web.Services.Implementation.Auth;

public class AuthService : IAuthService
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly IApiService _apiService;
    private readonly NavigationManager _navigation;
    private readonly ILoggingService _logger;
    private UserDto? _cachedUser;
    
    public AuthService(
        AuthenticationStateProvider authenticationStateProvider,
        IApiService apiService,
        NavigationManager navigation,
        ILoggingService logger)
    {
        _authenticationStateProvider = authenticationStateProvider;
        _apiService = apiService;
        _navigation = navigation;
        _logger = logger;
        _logger.LogDebug("AuthService initialized");
    }
    
    public async Task<UserDto?> GetCurrentUserAsync()
    {
        _logger.LogDebug("GetCurrentUserAsync called");
        
        if (_cachedUser != null)
        {
            _logger.LogDebug("Returning cached user: {0}", _cachedUser.Email);
            return _cachedUser;
        }
            
        _logger.LogDebug("Getting authentication state...");
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        
        if (authState.User.Identity?.IsAuthenticated == true)
        {
            _logger.LogInfo("User is authenticated: {0}", authState.User.Identity.Name);
            _logger.LogDebug("User claims count: {0}", authState.User.Claims.Count());
            
            try
            {
                _logger.LogDebug("Calling API to get user details from /api/auth/me");
                _cachedUser = await _apiService.GetAsync<UserDto>("api/auth/me");
                
                if (_cachedUser != null)
                {
                    _logger.LogInfo("Successfully retrieved user details: {0}", _cachedUser.Email);
                }
                else
                {
                    _logger.LogWarning("API returned null user data");
                }
                
                return _cachedUser;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get user details from API");
                return null;
            }
        }
        else
        {
            _logger.LogWarning("User is not authenticated");
        }
        
        return null;
    }
    
    public async Task<bool> IsAuthenticatedAsync()
    {
        _logger.LogDebug("IsAuthenticatedAsync called");
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var isAuthenticated = authState.User.Identity?.IsAuthenticated == true;
        _logger.LogDebug("IsAuthenticated: {0}", isAuthenticated);
        return isAuthenticated;
    }
    
    public async Task<bool> IsInRoleAsync(string role)
    {
        _logger.LogDebug("IsInRoleAsync called for role: {0}", role);
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var hasRole = authState.User.IsInRole(role);
        _logger.LogDebug("User has role '{0}': {1}", role, hasRole);
        return hasRole;
    }
    
    public async Task LogoutAsync()
    {
        _logger.LogInfo("LogoutAsync called");
        _cachedUser = null;
        _navigation.NavigateToLogout("authentication/logout");
    }
}