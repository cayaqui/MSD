using Core.Enums.Notifications;

namespace Core.DTOs.Notifications;

public class NotificationPreferenceDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    
    // Global preferences
    public bool EnableEmailNotifications { get; set; } = true;
    public bool EnableInAppNotifications { get; set; } = true;
    public bool EnablePushNotifications { get; set; } = false;
    
    // Quiet hours
    public bool EnableQuietHours { get; set; }
    public TimeSpan? QuietHoursStart { get; set; }
    public TimeSpan? QuietHoursEnd { get; set; }
    public string? TimeZone { get; set; }
    
    // Category preferences
    public List<CategoryPreferenceDto> CategoryPreferences { get; set; } = new();
    
    // Type preferences
    public List<TypePreferenceDto> TypePreferences { get; set; } = new();
    
    public DateTime LastUpdated { get; set; }
}

public class CategoryPreferenceDto
{
    public string Category { get; set; } = string.Empty;
    public bool EnableEmail { get; set; } = true;
    public bool EnableInApp { get; set; } = true;
    public bool EnablePush { get; set; } = false;
    public NotificationFrequency? EmailFrequency { get; set; }
}

public class TypePreferenceDto
{
    public NotificationType Type { get; set; }
    public bool EnableEmail { get; set; } = true;
    public bool EnableInApp { get; set; } = true;
    public bool EnablePush { get; set; } = false;
}

public class UpdateNotificationPreferenceDto
{
    // Global preferences
    public bool? EnableEmailNotifications { get; set; }
    public bool? EnableInAppNotifications { get; set; }
    public bool? EnablePushNotifications { get; set; }
    
    // Quiet hours
    public bool? EnableQuietHours { get; set; }
    public TimeSpan? QuietHoursStart { get; set; }
    public TimeSpan? QuietHoursEnd { get; set; }
    public string? TimeZone { get; set; }
    
    // Category preferences
    public List<CategoryPreferenceDto>? CategoryPreferences { get; set; }
    
    // Type preferences
    public List<TypePreferenceDto>? TypePreferences { get; set; }
}

public class NotificationSubscriptionDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string SubscriptionType { get; set; } = string.Empty; // "Project", "Contract", "Risk", etc.
    public Guid EntityId { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime SubscribedDate { get; set; }
    
    // What events to subscribe to
    public List<string> SubscribedEvents { get; set; } = new();
}

public class CreateNotificationSubscriptionDto
{
    public string SubscriptionType { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public List<string> SubscribedEvents { get; set; } = new();
}