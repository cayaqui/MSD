using Core.DTOs.Auth.Users;
using Core.DTOs.Common;

namespace Web.Services.Interfaces.Auth;

public interface IUserApiService
{
    Task<UserDto?> GetCurrentUserAsync();
    Task<UserDto?> GetUserByIdAsync(Guid id);
    Task<UserDto?> GetUserByEmailAsync(string email);
    Task<UserDto?> GetUserByEntraIdAsync(string entraId);
    Task<UserDto?> SyncUserWithAzureAsync(Guid userId);
    Task<string?> GetUserPhotoAsync(Guid userId);
    Task<PagedResult<UserDto>> SearchUsersAsync(UserFilterDto filter);
    Task<UserDto?> CreateUserAsync(CreateUserDto dto);
    Task<UserDto?> UpdateUserAsync(Guid id, UpdateUserDto dto);
    Task<bool> ActivateUserAsync(Guid id);
    Task<bool> DeactivateUserAsync(Guid id);
    Task<bool> CheckEmailExistsAsync(string email);
}
