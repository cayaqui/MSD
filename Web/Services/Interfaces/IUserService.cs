using Core.DTOs.Auth.Users;
using Core.DTOs.Common;

namespace Web.Services.Interfaces;

public interface IUserService
{
    Task<PagedResult<UserDto>> GetUsersAsync(int page = 1, int pageSize = 10);
    Task<UserDto?> GetUserByIdAsync(string userId);
    Task<UserDto?> CreateUserAsync(CreateUserDto dto);
    Task<UserDto?> UpdateUserAsync(string userId, UpdateUserDto dto);
    Task<bool> DeleteUserAsync(string userId);
}