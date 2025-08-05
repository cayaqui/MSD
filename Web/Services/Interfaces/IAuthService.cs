using Core.DTOs.Auth.Users;

namespace Web.Services.Interfaces;

public interface IAuthService
{
    Task<UserDto?> GetCurrentUserAsync();
    Task<bool> IsAuthenticatedAsync();
    Task<bool> IsInRoleAsync(string role);
    Task LogoutAsync();
}