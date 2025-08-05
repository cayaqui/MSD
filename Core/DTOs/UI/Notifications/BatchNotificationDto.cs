using Core.Enums.UI;

namespace Core.DTOs.UI.Notifications;

/// <summary>
/// DTO for batch notification operations
/// </summary>
public class BatchNotificationDto
{
    public List<Guid> NotificationIds { get; set; } = new();
    public NotificationBatchAction Action { get; set; }
}
