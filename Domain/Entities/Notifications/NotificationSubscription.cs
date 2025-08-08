using Domain.Common;

namespace Domain.Entities.Notifications;

public class NotificationSubscription : BaseEntity
{
    public Guid UserId { get; private set; }
    public NotificationPreference UserPreference { get; private set; } = null!;
    
    public string SubscriptionType { get; private set; } = string.Empty;
    public Guid EntityId { get; private set; }
    public string EntityName { get; private set; } = string.Empty;
    
    public bool IsActive { get; private set; }
    public DateTime SubscribedDate { get; private set; }
    public DateTime? UnsubscribedDate { get; private set; }
    
    // Events to subscribe to stored as JSON array
    public string SubscribedEventsJson { get; private set; } = string.Empty;
    
    private NotificationSubscription() { } // For EF Core
    
    public NotificationSubscription(
        Guid userId,
        string subscriptionType,
        Guid entityId,
        string entityName,
        string subscribedEventsJson)
    {
        UserId = userId;
        SubscriptionType = subscriptionType;
        EntityId = entityId;
        EntityName = entityName;
        SubscribedEventsJson = subscribedEventsJson;
        IsActive = true;
        SubscribedDate = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
    }
    
    public void UpdateSubscribedEvents(string subscribedEventsJson)
    {
        SubscribedEventsJson = subscribedEventsJson;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void Activate()
    {
        IsActive = true;
        UnsubscribedDate = null;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void Deactivate()
    {
        IsActive = false;
        UnsubscribedDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}