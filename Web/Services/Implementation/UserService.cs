using Core.DTOs.Auth.Users;
using Core.DTOs.Common;
using Web.Services.Interfaces;

namespace Web.Services.Implementation;

public class UserService : IUserService
{
    private readonly IApiService _apiService;
    
    public UserService(IApiService apiService)
    {
        _apiService = apiService;
    }
    
    public async Task<PagedResult<UserDto>> GetUsersAsync(int page = 1, int pageSize = 10)
    {
        return await _apiService.GetAsync<PagedResult<UserDto>>($"api/users?page={page}&pageSize={pageSize}") 
            ?? new PagedResult<UserDto>();
    }
    
    public async Task<UserDto?> GetUserByIdAsync(string userId)
    {
        return await _apiService.GetAsync<UserDto>($"api/users/{userId}");
    }
    
    public async Task<UserDto?> CreateUserAsync(CreateUserDto dto)
    {
        return await _apiService.PostAsync<CreateUserDto, UserDto>("api/users", dto);
    }
    
    public async Task<UserDto?> UpdateUserAsync(string userId, UpdateUserDto dto)
    {
        return await _apiService.PutAsync<UpdateUserDto, UserDto>($"api/users/{userId}", dto);
    }
    
    public async Task<bool> DeleteUserAsync(string userId)
    {
        return await _apiService.DeleteAsync($"api/users/{userId}");
    }
}