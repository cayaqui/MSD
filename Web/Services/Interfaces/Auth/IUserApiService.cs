using Core.DTOs.Auth.Users;
using Core.DTOs.Auth.ProjectTeamMembers;
using Core.DTOs.Auth.Permissions;
using Core.DTOs.Common;

namespace Web.Services.Interfaces.Auth;

public interface IUserApiService
{
    // User retrieval
    Task<UserDto?> GetCurrentUserAsync();
    Task<UserPermissionsDto?> GetCurrentUserPermissionsAsync();
    Task<PagedResult<UserDto>> GetUsersAsync(int pageNumber = 1, int pageSize = 20);
    Task<UserDto?> GetUserByIdAsync(Guid id);
    Task<UserDto?> GetUserByEmailAsync(string email);
    Task<UserDto?> GetUserByEntraIdAsync(string entraId);
    
    // User management
    Task<PagedResult<UserDto>> SearchUsersAsync(UserFilterDto filter);
    Task<UserDto?> CreateUserAsync(CreateUserDto dto);
    Task<UserDto?> UpdateUserAsync(Guid id, UpdateUserDto dto);
    Task<bool> DeleteUserAsync(Guid id);
    
    // User activation
    Task<bool> ActivateUserAsync(Guid id);
    Task<bool> DeactivateUserAsync(Guid id);
    Task<BulkOperationResult> BulkActivateUsersAsync(List<Guid> userIds);
    Task<BulkOperationResult> BulkDeactivateUsersAsync(List<Guid> userIds);
    
    // User projects and sync
    Task<List<ProjectTeamMemberDto>> GetUserProjectsAsync(Guid userId);
    Task<UserDto?> SyncUserWithAzureAsync(Guid userId);
    Task<UserPhotoResponse?> GetUserPhotoAsync(Guid userId);
    
    // Validation
    Task<bool> CheckEmailExistsAsync(string email);
    Task<CanDeleteResult> CanDeleteUserAsync(Guid userId);
}

// DTOs
public record UserPhotoResponse(string ContentType, byte[] Data);
