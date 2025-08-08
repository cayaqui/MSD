using Core.DTOs.Auth.Users;
using Core.DTOs.Auth.Permissions;

namespace Web.Services.Interfaces.Auth;

public interface IAuthService
{
    Task<UserDto?> GetCurrentUserAsync();
    Task<bool> IsAuthenticatedAsync();
    Task<bool> IsInRoleAsync(string role);
    Task LogoutAsync();
    
    // Additional auth endpoints
    Task<UserPermissionsDto?> GetMyPermissionsAsync();
    Task<List<Guid>> GetMyProjectsAsync();
    Task<ProjectPermissionDto?> GetMyProjectPermissionsAsync(Guid projectId);
    Task<UserDto?> SyncCurrentUserWithAzureAsync();
    Task<bool> CheckPermissionAsync(string permission, Guid? projectId = null);
    Task<bool> CheckProjectAccessAsync(Guid projectId, string? requiredRole = null);
    Task<LoginInfoDto?> GetLoginInfoAsync();
}

// DTOs
public record LoginInfoDto(
    string Authority,
    string ClientId,
    string TenantId,
    string[] Scopes);