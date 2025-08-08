using Core.Enums.Notifications;

namespace Core.DTOs.Notifications;

public class CreateNotificationDto
{
    public NotificationType Type { get; set; }
    public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Category { get; set; }
    public Guid RecipientId { get; set; }
    public Guid? SenderId { get; set; }
    
    // Related entity information
    public Guid? RelatedEntityId { get; set; }
    public string? RelatedEntityType { get; set; }
    public string? RelatedEntityName { get; set; }
    public string? ActionUrl { get; set; }
    
    // Additional metadata
    public Dictionary<string, string>? Metadata { get; set; }
    public List<CreateNotificationActionDto>? Actions { get; set; }
}

public class CreateNotificationActionDto
{
    public string Label { get; set; } = string.Empty;
    public string ActionType { get; set; } = string.Empty;
    public string ActionUrl { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
}

public class BulkCreateNotificationDto
{
    public List<Guid> RecipientIds { get; set; } = new();
    public NotificationType Type { get; set; }
    public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Category { get; set; }
    public Guid? SenderId { get; set; }
    
    // Related entity information
    public Guid? RelatedEntityId { get; set; }
    public string? RelatedEntityType { get; set; }
    public string? RelatedEntityName { get; set; }
    public string? ActionUrl { get; set; }
}