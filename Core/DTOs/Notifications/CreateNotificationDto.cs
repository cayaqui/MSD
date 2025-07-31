using Core.Enums.UI;
namespace Core.DTOs.Notifications;

/// <summary>
/// DTO for creating a new notification
/// </summary>
public class CreateNotificationDto
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; } = NotificationType.Info;
    public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;
    public bool IsImportant { get; set; }

    // Target user(s)
    public Guid? UserId { get; set; }
    public List<Guid>? UserIds { get; set; }

    // Optional context
    public Guid? ProjectId { get; set; }
    public Guid? CompanyId { get; set; }

    // Additional data
    public string? ActionUrl { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
}
