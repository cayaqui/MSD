using Web.Models.Responses;

namespace Web.Services.Interfaces
{
    public interface IUserService
    {
        Task<ApiResponse<PagedResult<UserDto>>> GetUsersAsync(int pageNumber = 1, int pageSize = 10);
        Task<ApiResponse<PagedResult<UserDto>>> SearchUsersAsync(UserFilterDto filter);
        Task<ApiResponse<UserDto>> GetUserByIdAsync(Guid id);
        Task<ApiResponse<UserDto>> GetUserByEmailAsync(string email);
        Task<ApiResponse<UserDto>> CreateUserAsync(CreateUserDto dto);
        Task<ApiResponse<UserDto>> UpdateUserAsync(Guid id, UpdateUserDto dto);
        Task<ApiResponse<bool>> DeleteUserAsync(Guid id);
        Task<ApiResponse<UserDto>> ActivateUserAsync(Guid id);
        Task<ApiResponse<UserDto>> DeactivateUserAsync(Guid id);
        Task<ApiResponse<List<ProjectTeamMemberDto>>> GetUserProjectsAsync(Guid id);
        Task<ApiResponse<UserDto>> SyncUserWithAzureADAsync(Guid id);
        Task<ApiResponse<int>> BulkActivateUsersAsync(BulkUserOperationDto dto);
        Task<ApiResponse<int>> BulkDeactivateUsersAsync(BulkUserOperationDto dto);
    }

    //public class PagedResult<T>
    //{
    //    public List<T> Items { get; set; } = new();
    //    public int PageNumber { get; set; }
    //    public int PageSize { get; set; }
    //    public int TotalCount { get; set; }
    //    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    //    public bool HasPreviousPage => PageNumber > 1;
    //    public bool HasNextPage => PageNumber < TotalPages;
    //}
}