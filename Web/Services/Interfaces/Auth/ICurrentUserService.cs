namespace Web.Services.Interfaces.Auth;

/// <summary>
/// Service for accessing current user information in the Blazor application
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Gets the current user's ID
    /// </summary>
    Task<string?> GetUserIdAsync();
    
    /// <summary>
    /// Gets the current user's email
    /// </summary>
    Task<string?> GetUserEmailAsync();
    
    /// <summary>
    /// Gets the current user's display name
    /// </summary>
    Task<string?> GetUserNameAsync();
    
    /// <summary>
    /// Checks if the current user is authenticated
    /// </summary>
    Task<bool> IsAuthenticatedAsync();
}