namespace Application.Interfaces.Auth;

/// <summary>
/// Interface for checking user permissions
/// </summary>
public interface IPermissionChecker
{
    /// <summary>
    /// Check if the current user has a specific permission
    /// </summary>
    Task<bool> HasPermissionAsync(string permission, Guid? projectId = null);
    
    /// <summary>
    /// Check if the current user has any of the specified permissions
    /// </summary>
    Task<bool> HasAnyPermissionAsync(params string[] permissions);
    
    /// <summary>
    /// Check if the current user has all of the specified permissions
    /// </summary>
    Task<bool> HasAllPermissionsAsync(params string[] permissions);
    
    /// <summary>
    /// Check if the current user has a specific permission in a project context
    /// </summary>
    Task<bool> HasProjectPermissionAsync(Guid projectId, string permission);
    
    /// <summary>
    /// Get all permissions for the current user
    /// </summary>
    Task<IEnumerable<string>> GetUserPermissionsAsync();
    
    /// <summary>
    /// Get all permissions for the current user in a specific project
    /// </summary>
    Task<IEnumerable<string>> GetProjectPermissionsAsync(Guid projectId);
}