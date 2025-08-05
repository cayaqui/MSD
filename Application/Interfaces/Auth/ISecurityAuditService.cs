using Core.DTOs.Auth;

namespace Application.Interfaces.Auth;

/// <summary>
/// Interface for security auditing
/// </summary>
public interface ISecurityAuditService
{
    /// <summary>
    /// Log a login attempt
    /// </summary>
    Task LogLoginAttemptAsync(string userId, bool success, string? ipAddress = null, string? userAgent = null, string? reason = null);
    
    /// <summary>
    /// Log a logout event
    /// </summary>
    Task LogLogoutAsync(string userId, string? ipAddress = null);
    
    /// <summary>
    /// Log a permission check
    /// </summary>
    Task LogPermissionCheckAsync(string userId, string permission, bool granted, Guid? projectId = null);
    
    /// <summary>
    /// Log project access
    /// </summary>
    Task LogProjectAccessAsync(string userId, Guid projectId, string action, bool granted);
    
    /// <summary>
    /// Log a security event
    /// </summary>
    Task LogSecurityEventAsync(string userId, string eventType, string details, string? ipAddress = null);
    
    /// <summary>
    /// Log a failed authentication attempt
    /// </summary>
    Task LogFailedAuthenticationAsync(string? identifier, string reason, string? ipAddress = null);
    
    /// <summary>
    /// Log password change
    /// </summary>
    Task LogPasswordChangeAsync(string userId, bool success, string? ipAddress = null);
    
    /// <summary>
    /// Log role change
    /// </summary>
    Task LogRoleChangeAsync(string userId, string oldRole, string newRole, Guid? projectId = null, string? changedBy = null);
    
    /// <summary>
    /// Get recent security events for a user
    /// </summary>
    Task<IEnumerable<SecurityAuditEntry>> GetUserSecurityEventsAsync(string userId, int count = 10);
}