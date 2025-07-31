using Core.Enums.UI;
namespace Core.DTOs.Notifications;

/// <summary>
/// DTO for notification summary/count
/// </summary>
public class NotificationSummaryDto
{
    public int TotalCount { get; set; }
    public int UnreadCount { get; set; }
    public int ImportantCount { get; set; }
    public int UnreadImportantCount { get; set; }

    public Dictionary<NotificationType, int> CountByType { get; set; } = new();
    public DateTime? LastNotificationAt { get; set; }
}
