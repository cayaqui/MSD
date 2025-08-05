using Domain.Common;
using System;

namespace Domain.Entities.Auth.Security
{
    /// <summary>
    /// Tracks user sessions for audit and security purposes
    /// Compatible with Azure AD/MSAL authentication
    /// </summary>
    public class UserSession : BaseEntity, IAuditable
    {
        public Guid UserId { get; private set; }
        public string SessionId { get; private set; } = string.Empty;
        public DateTime StartedAt { get; private set; }
        public DateTime? EndedAt { get; private set; }
        public DateTime LastActivityAt { get; private set; }
        public string? IpAddress { get; private set; }
        public string? UserAgent { get; private set; }
        public string? DeviceInfo { get; private set; }
        public bool IsActive { get; private set; }
        
        // Navigation property
        public User User { get; private set; } = null!;
        
        private UserSession() { } // EF Core
        
        public UserSession(Guid userId, string sessionId, string? ipAddress = null, string? userAgent = null)
        {
            UserId = userId;
            SessionId = sessionId ?? throw new ArgumentNullException(nameof(sessionId));
            StartedAt = DateTime.UtcNow;
            LastActivityAt = DateTime.UtcNow;
            IpAddress = ipAddress;
            UserAgent = userAgent;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }
        
        public void UpdateActivity()
        {
            LastActivityAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
        
        public void EndSession()
        {
            EndedAt = DateTime.UtcNow;
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }
        
        public TimeSpan GetSessionDuration()
        {
            var endTime = EndedAt ?? DateTime.UtcNow;
            return endTime - StartedAt;
        }
    }
}