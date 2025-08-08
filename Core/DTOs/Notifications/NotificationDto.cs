using Core.Enums.Notifications;

namespace Core.DTOs.Notifications;

public class NotificationDto
{
    public Guid Id { get; set; }
    public NotificationType Type { get; set; }
    public NotificationPriority Priority { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Category { get; set; }
    public Guid? RecipientId { get; set; }
    public string RecipientName { get; set; } = string.Empty;
    public string RecipientEmail { get; set; } = string.Empty;
    public Guid? SenderId { get; set; }
    public string? SenderName { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ReadDate { get; set; }
    public bool IsRead { get; set; }
    public bool IsArchived { get; set; }
    
    // Related entity information
    public Guid? RelatedEntityId { get; set; }
    public string? RelatedEntityType { get; set; }
    public string? RelatedEntityName { get; set; }
    public string? ActionUrl { get; set; }
    
    // Additional metadata
    public Dictionary<string, string> Metadata { get; set; } = new();
    public List<NotificationActionDto> Actions { get; set; } = new();
}

public class NotificationActionDto
{
    public string Label { get; set; } = string.Empty;
    public string ActionType { get; set; } = string.Empty;
    public string ActionUrl { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
}