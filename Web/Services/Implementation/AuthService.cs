// Web/Services/Implementation/AuthService.cs
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using Web.Services.Interfaces;
using Core.DTOs.Auth;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace Web.Services.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ILogger<AuthService> _logger;

        private UserDto? _currentUser;
        private UserPermissionsDto? _userPermissions;
        private DateTime _lastPermissionsFetch = DateTime.MinValue;
        private readonly TimeSpan _permissionsCacheDuration = TimeSpan.FromMinutes(5);

        public AuthService(
            HttpClient httpClient,
            AuthenticationStateProvider authenticationStateProvider,
            ILogger<AuthService> logger)
        {
            _httpClient = httpClient;
            _authenticationStateProvider = authenticationStateProvider;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            try
            {
                var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
                if (authState.User?.Identity?.IsAuthenticated == true)
                {
                    await RefreshUserDataAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing auth service");
            }
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            return authState.User?.Identity?.IsAuthenticated ?? false;
        }

        public async Task<UserDto?> GetCurrentUserAsync()
        {
            if (_currentUser == null)
            {
                await RefreshUserDataAsync();
            }
            return _currentUser;
        }

        public async Task<ClaimsPrincipal> GetCurrentUserPrincipalAsync()
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            return authState.User;
        }

        public string GetUserEmail()
        {
            return _currentUser?.Email ?? string.Empty;
        }

        public string GetUserName()
        {
            return _currentUser?.DisplayName ?? string.Empty;
        }

        public string GetUserInitials()
        {
            if (string.IsNullOrEmpty(_currentUser?.DisplayName))
                return "U";

            var parts = _currentUser.DisplayName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
                return $"{parts[0][0]}{parts[1][0]}".ToUpper();

            return parts[0][0].ToString().ToUpper();
        }

        public async Task RefreshUserDataAsync()
        {
            try
            {
                // Get current user
                var userResponse = await _httpClient.GetAsync("/api/auth/me");
                if (userResponse.IsSuccessStatusCode)
                {
                    _currentUser = await userResponse.Content.ReadFromJsonAsync<UserDto>();
                }

                // Get user permissions
                await RefreshPermissionsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing user data");
            }
        }

        public async Task<bool> HasPermissionAsync(Guid? projectId, string permission)
        {
            try
            {
                // Ensure permissions are loaded and not stale
                if (_userPermissions == null || DateTime.UtcNow - _lastPermissionsFetch > _permissionsCacheDuration)
                {
                    await RefreshPermissionsAsync();
                }

                if (_userPermissions == null)
                    return false;

                // Check global permissions first
                if (_userPermissions.GlobalPermissions.Contains(permission))
                    return true;

                // If a project ID is specified, check project-specific permissions
                if (projectId.HasValue)
                {
                    var projectPermissions = _userPermissions.ProjectPermissions
                        .FirstOrDefault(p => p.ProjectId == projectId.Value);

                    if (projectPermissions != null)
                    {
                        return projectPermissions.Permissions.Contains(permission);
                    }
                }

                // If no project ID specified, check if user has this permission in ANY project
                return _userPermissions.ProjectPermissions
                    .Any(p => p.Permissions.Contains(permission));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking permission {Permission} for project {ProjectId}",
                    permission, projectId);
                return false;
            }
        }

        public async Task<bool> IsInRoleAsync(string role)
        {
            try
            {
                if (_userPermissions == null)
                {
                    await RefreshPermissionsAsync();
                }

                if (_userPermissions == null)
                    return false;

                // Check if user has this role in any project
                return _userPermissions.ProjectPermissions
                    .Any(p => p.Role.Equals(role, StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking role {Role}", role);
                return false;
            }
        }

        public async Task<string?> GetAccessTokenAsync()
        {
            // This would be implemented based on your authentication method
            // For example, if using MSAL, you would get the token from the token cache
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();

            // Extract token from claims or authentication context
            var tokenClaim = authState.User?.Claims?.FirstOrDefault(c => c.Type == "access_token");
            return tokenClaim?.Value;
        }

        public async Task<UserPermissionsDto?> GetUserPermissionsAsync()
        {
            if (_userPermissions == null || DateTime.UtcNow - _lastPermissionsFetch > _permissionsCacheDuration)
            {
                await RefreshPermissionsAsync();
            }
            return _userPermissions;
        }

        public async Task<bool> HasProjectAccessAsync(Guid projectId, string? requiredRole = null)
        {
            try
            {
                if (_userPermissions == null)
                {
                    await RefreshPermissionsAsync();
                }

                if (_userPermissions == null)
                    return false;

                var projectPermissions = _userPermissions.ProjectPermissions
                    .FirstOrDefault(p => p.ProjectId == projectId);

                if (projectPermissions == null || !projectPermissions.IsActive)
                    return false;

                // If no specific role required, just check if user has access
                if (string.IsNullOrEmpty(requiredRole))
                    return true;

                // Check if user has the required role or higher
                return IsRoleEqualOrHigher(projectPermissions.Role, requiredRole);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking project access for {ProjectId}", projectId);
                return false;
            }
        }

        public void ClearCache()
        {
            _currentUser = null;
            _userPermissions = null;
            _lastPermissionsFetch = DateTime.MinValue;
        }

        private async Task RefreshPermissionsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/auth/permissions");
                if (response.IsSuccessStatusCode)
                {
                    _userPermissions = await response.Content.ReadFromJsonAsync<UserPermissionsDto>();
                    _lastPermissionsFetch = DateTime.UtcNow;

                    _logger.LogInformation("Refreshed permissions for user {UserId}: {GlobalCount} global, {ProjectCount} projects",
                        _currentUser?.Id,
                        _userPermissions?.GlobalPermissions.Count ?? 0,
                        _userPermissions?.ProjectPermissions.Count ?? 0);
                }
                else
                {
                    _logger.LogWarning("Failed to fetch permissions: {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing permissions");
            }
        }

        private bool IsRoleEqualOrHigher(string userRole, string requiredRole)
        {
            var roleHierarchy = new Dictionary<string, int>
            {
                { "Viewer", 1 },
                { "TeamMember", 2 },
                { "TeamLead", 3 },
                { "SchedController", 4 },
                { "CostController", 4 },
                { "ProjectController", 5 },
                { "ProjectManager", 6 }
            };

            if (!roleHierarchy.TryGetValue(userRole, out var userLevel))
                return false;

            if (!roleHierarchy.TryGetValue(requiredRole, out var requiredLevel))
                return false;

            return userLevel >= requiredLevel;
        }
    }
}