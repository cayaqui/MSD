using Domain.Common;

namespace Domain.Entities.Notifications;

public class Notification : BaseEntity
{
    public Core.Enums.Notifications.NotificationType Type { get; private set; }
    public Core.Enums.Notifications.NotificationPriority Priority { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Message { get; private set; } = string.Empty;
    public string? Category { get; private set; }
    
    // Recipient information
    public Guid RecipientId { get; private set; }
    public User RecipientUser { get; private set; } = null!;
    
    // Sender information (optional)
    public Guid? SenderId { get; private set; }
    public User? SenderUser { get; private set; }
    
    // Status tracking
    public Core.Enums.Notifications.NotificationStatus Status { get; private set; }
    public DateTime? ReadDate { get; private set; }
    public DateTime? ArchivedDate { get; private set; }
    public DateTime? ExpirationDate { get; private set; }
    
    // Related entity information
    public Guid? RelatedEntityId { get; private set; }
    public string? RelatedEntityType { get; private set; }
    public string? RelatedEntityName { get; private set; }
    public string? ActionUrl { get; private set; }
    
    // Additional data stored as JSON
    public string? MetadataJson { get; private set; }
    public string? ActionsJson { get; private set; }
    
    // Channel-specific information
    public List<NotificationDelivery> Deliveries { get; private set; } = new();
    
    private Notification() { } // For EF Core
    
    public Notification(
        Core.Enums.Notifications.NotificationType type,
        Core.Enums.Notifications.NotificationPriority priority,
        string title,
        string message,
        Guid recipientId,
        Guid? senderId = null,
        string? category = null)
    {
        Type = type;
        Priority = priority;
        Title = title;
        Message = message;
        RecipientId = recipientId;
        SenderId = senderId;
        Category = category;
        Status = Core.Enums.Notifications.NotificationStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }
    
    public void SetRelatedEntity(Guid entityId, string entityType, string entityName, string? actionUrl = null)
    {
        RelatedEntityId = entityId;
        RelatedEntityType = entityType;
        RelatedEntityName = entityName;
        ActionUrl = actionUrl;
    }
    
    public void SetMetadata(string metadataJson)
    {
        MetadataJson = metadataJson;
    }
    
    public void SetActions(string actionsJson)
    {
        ActionsJson = actionsJson;
    }
    
    public void SetExpirationDate(DateTime expirationDate)
    {
        ExpirationDate = expirationDate;
    }
    
    public void MarkAsRead()
    {
        if (Status == Core.Enums.Notifications.NotificationStatus.Delivered || Status == Core.Enums.Notifications.NotificationStatus.Sent)
        {
            Status = Core.Enums.Notifications.NotificationStatus.Read;
            ReadDate = DateTime.UtcNow;
        }
    }
    
    public void MarkAsArchived()
    {
        Status = Core.Enums.Notifications.NotificationStatus.Archived;
        ArchivedDate = DateTime.UtcNow;
    }
    
    public void MarkAsSent()
    {
        if (Status == Core.Enums.Notifications.NotificationStatus.Pending)
        {
            Status = Core.Enums.Notifications.NotificationStatus.Sent;
        }
    }
    
    public void MarkAsDelivered()
    {
        if (Status == Core.Enums.Notifications.NotificationStatus.Sent)
        {
            Status = Core.Enums.Notifications.NotificationStatus.Delivered;
        }
    }
    
    public void MarkAsFailed()
    {
        Status = Core.Enums.Notifications.NotificationStatus.Failed;
    }
    
    public void MarkAsExpired()
    {
        Status = Core.Enums.Notifications.NotificationStatus.Expired;
    }
    
    public void AddDelivery(Core.Enums.Notifications.NotificationChannel channel, string? deliveryDetails = null)
    {
        var delivery = new NotificationDelivery(Id, channel, deliveryDetails);
        Deliveries.Add(delivery);
    }
}