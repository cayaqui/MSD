// Application/Common/Interfaces/Auth/IAuthService.cs
using Core.DTOs.Auth;

namespace Application.Interfaces.Auth;

/// <summary>
/// Service for authentication and authorization operations
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Gets the current authenticated user with Azure AD sync if needed
    /// </summary>
    Task<UserDto?> GetCurrentUserAsync();

    /// <summary>
    /// Gets all permissions for the current user
    /// </summary>
    Task<UserPermissionsDto?> GetUserPermissionsAsync();

    /// <summary>
    /// Gets user permissions for a specific project
    /// </summary>
    Task<ProjectPermissionsDto?> GetUserProjectPermissionsAsync(Guid projectId);

    /// <summary>
    /// Syncs the current user data with Azure AD
    /// </summary>
    Task<UserDto?> SyncCurrentUserWithAzureAsync();

    /// <summary>
    /// Syncs any user with Azure AD (for initial login)
    /// </summary>
    Task<UserDto?> SyncUserWithAzureAsync(string entraId, string email, string displayName);
}