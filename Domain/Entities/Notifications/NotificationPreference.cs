using Core.Enums.Notifications;
using Domain.Common;

namespace Domain.Entities.Notifications;

public class NotificationPreference : BaseEntity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;
    
    // Global preferences
    public bool EnableEmailNotifications { get; private set; }
    public bool EnableInAppNotifications { get; private set; }
    public bool EnablePushNotifications { get; private set; }
    
    // Quiet hours
    public bool EnableQuietHours { get; private set; }
    public TimeSpan? QuietHoursStart { get; private set; }
    public TimeSpan? QuietHoursEnd { get; private set; }
    public string? TimeZone { get; private set; }
    
    // Preferences stored as JSON
    public string? CategoryPreferencesJson { get; private set; }
    public string? TypePreferencesJson { get; private set; }
    
    // Subscriptions
    public List<NotificationSubscription> Subscriptions { get; private set; } = new();
    
    private NotificationPreference() { } // For EF Core
    
    public NotificationPreference(Guid userId)
    {
        UserId = userId;
        EnableEmailNotifications = true;
        EnableInAppNotifications = true;
        EnablePushNotifications = false;
        EnableQuietHours = false;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void UpdateGlobalPreferences(
        bool? enableEmail,
        bool? enableInApp,
        bool? enablePush)
    {
        if (enableEmail.HasValue)
            EnableEmailNotifications = enableEmail.Value;
        if (enableInApp.HasValue)
            EnableInAppNotifications = enableInApp.Value;
        if (enablePush.HasValue)
            EnablePushNotifications = enablePush.Value;
        
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void ConfigureQuietHours(
        bool enable,
        TimeSpan? start,
        TimeSpan? end,
        string? timeZone)
    {
        EnableQuietHours = enable;
        QuietHoursStart = start;
        QuietHoursEnd = end;
        TimeZone = timeZone;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void SetCategoryPreferences(string categoryPreferencesJson)
    {
        CategoryPreferencesJson = categoryPreferencesJson;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void SetTypePreferences(string typePreferencesJson)
    {
        TypePreferencesJson = typePreferencesJson;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public bool IsWithinQuietHours(DateTime checkTime)
    {
        if (!EnableQuietHours || !QuietHoursStart.HasValue || !QuietHoursEnd.HasValue)
            return false;
        
        // Convert to user's timezone if specified
        var userTime = checkTime;
        if (!string.IsNullOrEmpty(TimeZone))
        {
            try
            {
                var tz = TimeZoneInfo.FindSystemTimeZoneById(TimeZone);
                userTime = TimeZoneInfo.ConvertTimeFromUtc(checkTime, tz);
            }
            catch
            {
                // If timezone is invalid, use UTC
            }
        }
        
        var timeOfDay = userTime.TimeOfDay;
        
        // Handle case where quiet hours span midnight
        if (QuietHoursEnd.Value < QuietHoursStart.Value)
        {
            return timeOfDay >= QuietHoursStart.Value || timeOfDay <= QuietHoursEnd.Value;
        }
        else
        {
            return timeOfDay >= QuietHoursStart.Value && timeOfDay <= QuietHoursEnd.Value;
        }
    }
}