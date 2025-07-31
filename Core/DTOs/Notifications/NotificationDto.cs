using Core.Enums.UI;
namespace Core.DTOs.Notifications;

/// <summary>
/// DTO for displaying notification information
/// </summary>
public class NotificationDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; } = NotificationType.Info;
    public NotificationPriority Priority { get; set; }
    public NotificationStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReadAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsImportant { get; set; }
    public bool IsExpired => ExpiresAt.HasValue && ExpiresAt.Value < DateTime.UtcNow;

    public bool IsRead => ReadAt.HasValue;

    // Optional context
    public Guid? ProjectId { get; set; }
    public string? ProjectName { get; set; }
    public string? ProjectCode { get; set; }

    public Guid? CompanyId { get; set; }
    public string? CompanyName { get; set; }

    public string TimeAgo
    {
        get
        {
            var timeSpan = DateTime.UtcNow - CreatedAt;
            if (timeSpan.TotalMinutes < 1)
                return "Just now";
            if (timeSpan.TotalHours < 1)
                return $"{(int)timeSpan.TotalMinutes} minutes ago";
            if (timeSpan.TotalDays < 1)
                return $"{(int)timeSpan.TotalHours} hours ago";
            return $"{(int)timeSpan.TotalDays} days ago";

        }
    }

    // Additional data
    public string? ActionUrl { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }

    public string? IconClass
    {
        get
        {
            return Type switch
            {
                NotificationType.Success => "fa-check-circle text-success",
                NotificationType.Warning => "fa-exclamation-triangle text-warning",
                NotificationType.Error => "fa-times-circle text-danger",
                NotificationType.Info => "fa-info-circle text-info",
                NotificationType.Project => "fa-project-diagram text-primary",
                NotificationType.Task => "fa-tasks text-secondary",
                NotificationType.User => "fa-user text-primary",
                NotificationType.System => "fa-cog text-muted",
                _ => "fa-bell text-primary"
            };
        }
    }
    public string? Description
    {
        get
        {
            return Type switch
            {
                NotificationType.Success => "Operation completed successfully.",
                NotificationType.Warning => "Please check the details.",
                NotificationType.Error => "An error occurred, please try again.",
                NotificationType.Info => "Here is some information for you.",
                NotificationType.Project => "Project related notification.",
                NotificationType.Task => "Task related notification.",
                NotificationType.User => "User related notification.",
                NotificationType.System => "System notification.",
                _ => "General notification."
            };
        }
    }
}
