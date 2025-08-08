using Core.DTOs.Auth.Users;
using Core.DTOs.Common;
using Web.Services.Interfaces;
using Web.Services.Interfaces.Auth;

namespace Web.Services.Implementation;

public class UserService : IUserService
{
    private readonly IUserApiService _userApiService;

    public UserService(IUserApiService userApiService)
    {
        _userApiService = userApiService;
    }

    public async Task<List<UserDto>> SearchUsersAsync(string searchTerm)
    {
        var filter = new UserFilterDto
        {
            SearchTerm = searchTerm,
            PageSize = 10,
            IsActive = true
        };

        var result = await _userApiService.SearchUsersAsync(filter);
        return result?.Items?.ToList() ?? new List<UserDto>();
    }
}