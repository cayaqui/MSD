namespace Application.Interfaces.Auth;

/// <summary>
/// Interface for checking project access and roles
/// </summary>
public interface IProjectAccessChecker
{
    /// <summary>
    /// Check if the current user can access a project
    /// </summary>
    Task<bool> CanAccessProjectAsync(Guid projectId);
    
    /// <summary>
    /// Check if the current user is a project manager for the specified project
    /// </summary>
    Task<bool> IsProjectManagerAsync(Guid projectId);
    
    /// <summary>
    /// Check if the current user is a project controller for the specified project
    /// </summary>
    Task<bool> IsProjectControllerAsync(Guid projectId);
    
    /// <summary>
    /// Check if the current user is a team lead for the specified project
    /// </summary>
    Task<bool> IsTeamLeadAsync(Guid projectId);
    
    /// <summary>
    /// Check if the current user has a specific role in the project
    /// </summary>
    Task<bool> HasProjectRoleAsync(Guid projectId, string role);
    
    /// <summary>
    /// Check if the current user has any of the specified roles in the project
    /// </summary>
    Task<bool> HasAnyProjectRoleAsync(Guid projectId, params string[] roles);
    
    /// <summary>
    /// Get the current user's role in a specific project
    /// </summary>
    Task<string?> GetUserProjectRoleAsync(Guid projectId);
    
    /// <summary>
    /// Get all projects the current user has access to
    /// </summary>
    Task<IEnumerable<Guid>> GetAccessibleProjectIdsAsync();
}