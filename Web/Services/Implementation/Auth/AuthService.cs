using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components;
using Core.DTOs.Auth.Users;
using Core.DTOs.Auth.Permissions;
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
    
    public async Task<UserPermissionsDto?> GetMyPermissionsAsync()
    {
        try
        {
            _logger.LogDebug("Getting user permissions");
            return await _apiService.GetAsync<UserPermissionsDto>("api/auth/me/permissions");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get user permissions");
            return null;
        }
    }
    
    public async Task<List<Guid>> GetMyProjectsAsync()
    {
        try
        {
            _logger.LogDebug("Getting user projects");
            var projects = await _apiService.GetAsync<List<Guid>>("api/auth/me/projects");
            return projects ?? new List<Guid>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get user projects");
            return new List<Guid>();
        }
    }
    
    public async Task<ProjectPermissionDto?> GetMyProjectPermissionsAsync(Guid projectId)
    {
        try
        {
            _logger.LogDebug("Getting project permissions for project: {0}", projectId);
            return await _apiService.GetAsync<ProjectPermissionDto>($"api/auth/me/projects/{projectId}/permissions");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get project permissions for project: {0}", projectId);
            return null;
        }
    }
    
    public async Task<UserDto?> SyncCurrentUserWithAzureAsync()
    {
        try
        {
            _logger.LogDebug("Syncing current user with Azure AD");
            _cachedUser = null; // Clear cache
            var user = await _apiService.PostAsync<object, UserDto>("api/auth/sync", new { });
            if (user != null)
            {
                _cachedUser = user;
                _logger.LogInfo("Successfully synced user with Azure AD: {0}", user.Email);
            }
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to sync user with Azure AD");
            return null;
        }
    }
    
    public async Task<bool> CheckPermissionAsync(string permission, Guid? projectId = null)
    {
        try
        {
            _logger.LogDebug("Checking permission: {0} for project: {1}", permission, projectId);
            var request = new { Permission = permission, ProjectId = projectId };
            var response = await _apiService.PostAsync<object, dynamic>("api/auth/check-permission", request);
            return response?.hasPermission ?? false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check permission: {0}", permission);
            return false;
        }
    }
    
    public async Task<bool> CheckProjectAccessAsync(Guid projectId, string? requiredRole = null)
    {
        try
        {
            _logger.LogDebug("Checking project access for project: {0}, role: {1}", projectId, requiredRole);
            var request = new { ProjectId = projectId, RequiredRole = requiredRole };
            var response = await _apiService.PostAsync<object, dynamic>("api/auth/check-project-access", request);
            return response?.hasAccess ?? false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check project access for project: {0}", projectId);
            return false;
        }
    }
    
    public async Task<LoginInfoDto?> GetLoginInfoAsync()
    {
        try
        {
            _logger.LogDebug("Getting login info");
            return await _apiService.GetAsync<LoginInfoDto>("api/auth/public/login-info");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get login info");
            return null;
        }
    }
}