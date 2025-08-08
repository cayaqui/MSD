using Domain.Common;

namespace Domain.Entities.Notifications;

public class NotificationDelivery : BaseEntity
{
    public Guid NotificationId { get; private set; }
    public Notification Notification { get; private set; } = null!;
    
    public Core.Enums.Notifications.NotificationChannel Channel { get; private set; }
    public Core.Enums.Notifications.NotificationStatus Status { get; private set; }
    
    public DateTime? AttemptedAt { get; private set; }
    public DateTime? DeliveredAt { get; private set; }
    public DateTime? ReadAt { get; private set; }
    
    public int AttemptCount { get; private set; }
    public string? LastError { get; private set; }
    public string? DeliveryDetails { get; private set; } // JSON with channel-specific data
    
    // Channel-specific identifiers
    public string? ExternalId { get; private set; } // e.g., Email message ID, Push notification ID
    public string? RecipientAddress { get; private set; } // e.g., email address, phone number
    
    private NotificationDelivery() { } // For EF Core
    
    public NotificationDelivery(
        Guid notificationId,
        Core.Enums.Notifications.NotificationChannel channel,
        string? deliveryDetails = null)
    {
        NotificationId = notificationId;
        Channel = channel;
        Status = Core.Enums.Notifications.NotificationStatus.Pending;
        DeliveryDetails = deliveryDetails;
        AttemptCount = 0;
        CreatedAt = DateTime.UtcNow;
    }
    
    public void RecordAttempt(bool success, string? error = null, string? externalId = null)
    {
        AttemptCount++;
        AttemptedAt = DateTime.UtcNow;
        
        if (success)
        {
            Status = Core.Enums.Notifications.NotificationStatus.Delivered;
            DeliveredAt = DateTime.UtcNow;
            ExternalId = externalId;
            LastError = null;
        }
        else
        {
            Status = Core.Enums.Notifications.NotificationStatus.Failed;
            LastError = error;
        }
    }
    
    public void MarkAsRead()
    {
        if (Status == Core.Enums.Notifications.NotificationStatus.Delivered)
        {
            Status = Core.Enums.Notifications.NotificationStatus.Read;
            ReadAt = DateTime.UtcNow;
        }
    }
    
    public void SetRecipientAddress(string address)
    {
        RecipientAddress = address;
    }
}