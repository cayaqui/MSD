// Application/Services/Auth/AuthService.cs

using Core.DTOs.Auth.Permissions;
using Core.DTOs.Auth.Users;

namespace Application.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<UserDto?> GetCurrentUserAsync();
        Task<UserPermissionsDto?> GetUserPermissionsAsync();
        Task<ProjectPermissionDto?> GetUserProjectPermissionsAsync(Guid projectId);
        Task<UserDto?> SyncCurrentUserWithAzureAsync();
        Task<UserDto?> SyncUserWithAzureAsync(string entraId, string email, string displayName);
    }
}