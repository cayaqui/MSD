// Application/Services/Auth/AuthService.cs

namespace Application.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<UserDto?> GetCurrentUserAsync();
        Task<UserPermissionsDto?> GetUserPermissionsAsync();
        Task<ProjectPermissionsDto?> GetUserProjectPermissionsAsync(Guid projectId);
        Task<UserDto?> SyncCurrentUserWithAzureAsync();
        Task<UserDto?> SyncUserWithAzureAsync(string entraId, string email, string displayName);
    }
}