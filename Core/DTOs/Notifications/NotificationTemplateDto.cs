using Core.Enums.Notifications;

namespace Core.DTOs.Notifications;

public class NotificationTemplateDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public string Category { get; set; } = string.Empty;
    public string SubjectTemplate { get; set; } = string.Empty;
    public string BodyTemplate { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    
    // Email specific settings
    public bool SendEmail { get; set; }
    public string? EmailSubjectTemplate { get; set; }
    public string? EmailBodyTemplate { get; set; }
    public bool IsHtmlEmail { get; set; }
    
    // In-app notification settings
    public bool SendInApp { get; set; }
    public NotificationPriority DefaultPriority { get; set; }
    public int? ExpirationDays { get; set; }
    
    // Push notification settings (future)
    public bool SendPush { get; set; }
    public string? PushTitleTemplate { get; set; }
    public string? PushBodyTemplate { get; set; }
    
    // Placeholders available for this template
    public List<string> AvailablePlaceholders { get; set; } = new();
    
    // Default actions for this template
    public List<NotificationTemplateActionDto> DefaultActions { get; set; } = new();
}

public class NotificationTemplateActionDto
{
    public string Label { get; set; } = string.Empty;
    public string ActionType { get; set; } = string.Empty;
    public string ActionUrlTemplate { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
}

public class CreateNotificationTemplateDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public string Category { get; set; } = string.Empty;
    public string SubjectTemplate { get; set; } = string.Empty;
    public string BodyTemplate { get; set; } = string.Empty;
    
    // Email specific settings
    public bool SendEmail { get; set; }
    public string? EmailSubjectTemplate { get; set; }
    public string? EmailBodyTemplate { get; set; }
    public bool IsHtmlEmail { get; set; }
    
    // In-app notification settings
    public bool SendInApp { get; set; } = true;
    public NotificationPriority DefaultPriority { get; set; } = NotificationPriority.Normal;
    public int? ExpirationDays { get; set; }
    
    // Push notification settings (future)
    public bool SendPush { get; set; }
    public string? PushTitleTemplate { get; set; }
    public string? PushBodyTemplate { get; set; }
}

public class UpdateNotificationTemplateDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? SubjectTemplate { get; set; }
    public string? BodyTemplate { get; set; }
    public bool? IsActive { get; set; }
    
    // Email specific settings
    public bool? SendEmail { get; set; }
    public string? EmailSubjectTemplate { get; set; }
    public string? EmailBodyTemplate { get; set; }
    public bool? IsHtmlEmail { get; set; }
    
    // In-app notification settings
    public bool? SendInApp { get; set; }
    public NotificationPriority? DefaultPriority { get; set; }
    public int? ExpirationDays { get; set; }
    
    // Push notification settings (future)
    public bool? SendPush { get; set; }
    public string? PushTitleTemplate { get; set; }
    public string? PushBodyTemplate { get; set; }
}