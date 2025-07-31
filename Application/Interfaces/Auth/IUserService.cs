using Core.DTOs.Auth;
using Core.DTOs.Common;

namespace Application.Interfaces.Auth;

public interface IUserService
{
    // Read operations
    Task<PagedResult<UserDto>> GetPagedAsync(int pageNumber, int pageSize);
    Task<PagedResult<UserDto>> SearchAsync(UserFilterDto filter);
    Task<UserDto?> GetByIdAsync(Guid id);
    Task<UserDto?> GetByEmailAsync(string email);
    Task<UserDto?> GetByEntraIdAsync(string entraId);
    Task<IEnumerable<ProjectTeamMemberDto>> GetUserProjectsAsync(Guid userId);

    // Write operations
    Task<UserDto> CreateAsync(CreateUserDto dto);
    Task<UserDto?> UpdateAsync(Guid id, UpdateUserDto dto);
    Task<UserDto?> ActivateAsync(Guid id);
    Task<UserDto?> DeactivateAsync(Guid id);
    Task DeleteAsync(Guid id, string? deletedBy = null);

    // Azure AD operations
    Task<UserDto?> SyncWithAzureADAsync(Guid id);
    Task<UserDto> CreateOrUpdateFromAzureADAsync(AzureAdUser azureUser);

    // Bulk operations
    Task<int> BulkActivateAsync(List<Guid> userIds);
    Task<int> BulkDeactivateAsync(List<Guid> userIds);

    // Validation
    Task<bool> EmailExistsAsync(string email, Guid? excludeId = null);
    Task<bool> CanUserBeDeletedAsync(Guid id);
}