namespace Application.Interfaces.Auth;
public interface ICurrentUserService
{
    string? UserId { get; }
    string? UserName { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
    
    // Simplified system role properties
    bool IsAdmin { get; }
    bool IsSupport { get; }
    string? SystemRole { get; }
    
    // Original methods for backward compatibility
    Task<bool> IsInRoleAsync(string role);
    Task<bool> HasPermissionAsync(string permission);
    Task<bool> HasProjectAccessAsync(Guid projectId, string? requiredRole = null);
    Task<string?> GetProjectRoleAsync(Guid projectId);
    Task<List<Guid>> GetUserProjectIdsAsync();
    
    // New simplified methods
    Task<bool> HasSystemAccessAsync();
    Task<bool> IsSystemRoleAsync();
    Task<bool> CanEditProjectAsync(Guid projectId);
    Task<bool> CanViewProjectAsync(Guid projectId);
}
