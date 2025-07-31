namespace Web.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string?> GetAccessTokenAsync();
        Task<UserDto?> GetCurrentUserAsync();
        Task<ClaimsPrincipal> GetCurrentUserPrincipalAsync();
        string GetUserEmail();
        string GetUserInitials();
        string GetUserName();
        Task<bool> HasPermissionAsync(Guid? projectId, string permission);
        Task InitializeAsync();
        Task<bool> IsAuthenticatedAsync();
        Task<bool> IsInRoleAsync(string role);
        Task RefreshUserDataAsync();
    }
}