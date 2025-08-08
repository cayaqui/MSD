using Core.DTOs.Auth.Users;

namespace Web.Services.Interfaces;

public interface IUserService
{
    Task<List<UserDto>> SearchUsersAsync(string searchTerm);
}