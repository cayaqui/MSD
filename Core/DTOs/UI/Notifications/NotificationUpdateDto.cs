namespace Core.DTOs.UI.Notifications;

/// <summary>
/// DTO for real-time notification updates
/// </summary>
public class NotificationUpdateDto
{
    public Guid Id { get; set; }
    public string UpdateType { get; set; } = string.Empty; // "New", "Updated", "Deleted"
    public NotificationDto? Notification { get; set; }
    public DateTime Timestamp { get; set; }
}
