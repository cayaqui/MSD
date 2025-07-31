using Core.Enums.UI;
namespace Core.DTOs.Notifications;

/// <summary>
/// DTO for notification preferences
/// </summary>
public class NotificationPreferencesDto
{
    public Guid UserId { get; set; }

    // Email notifications
    public bool EmailEnabled { get; set; }
    public bool EmailOnImportant { get; set; }
    public bool EmailOnProjectUpdates { get; set; }
    public bool EmailOnTeamChanges { get; set; }

    // In-app notifications
    public bool InAppEnabled { get; set; } = true;
    public bool ShowDesktopNotifications { get; set; }
    public bool PlaySound { get; set; }

    // Filtering
    public List<NotificationType> EnabledTypes { get; set; } = new();
    public List<Guid> MutedProjects { get; set; } = new();
}
