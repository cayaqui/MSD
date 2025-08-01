// Web/Services/Implementation/AuthService.cs

namespace Web.Services.Interfaces
{
    public interface IAuthService
    {
        void ClearCache();
        Task<string?> GetAccessTokenAsync();
        Task<UserDto?> GetCurrentUserAsync();
        Task<ClaimsPrincipal> GetCurrentUserPrincipalAsync();
        string GetUserEmail();
        string GetUserInitials();
        string GetUserName();
        Task<UserPermissionsDto?> GetUserPermissionsAsync();
        Task<bool> HasPermissionAsync(Guid? projectId, string permission);
        Task<bool> HasProjectAccessAsync(Guid projectId, string? requiredRole = null);
        Task InitializeAsync();
        Task<bool> IsAuthenticatedAsync();
        Task<bool> IsInRoleAsync(string role);
        Task RefreshUserDataAsync();
    }
}