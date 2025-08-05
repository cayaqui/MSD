namespace Core.DTOs.Auth;

/// <summary>
/// Represents a security audit log entry
/// </summary>
public class SecurityAuditEntry
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public Guid? ProjectId { get; set; }
    public DateTime Timestamp { get; set; }
    public string? AdditionalData { get; set; }
}