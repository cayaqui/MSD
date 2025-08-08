using Core.DTOs.Common;
using Core.DTOs.Notifications;
using Core.Enums.Notifications;
using Core.Results;

namespace Application.Interfaces.Notifications;

/// <summary>
/// Service for managing notifications
/// </summary>
public interface INotificationService
{
    #region Notification Operations
    
    /// <summary>
    /// Get notifications for a user
    /// </summary>
    Task<PagedResult<NotificationDto>> GetUserNotificationsAsync(
        Guid userId,
        NotificationQueryParameters parameters,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get notification by ID
    /// </summary>
    Task<NotificationDto?> GetNotificationByIdAsync(
        Guid notificationId,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get unread notification count
    /// </summary>
    Task<int> GetUnreadCountAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Create a notification
    /// </summary>
    Task<Core.Results.Result<Guid>> CreateNotificationAsync(
        CreateNotificationDto dto,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Create notifications for multiple recipients
    /// </summary>
    Task<Core.Results.Result<int>> CreateBulkNotificationsAsync(
        BulkCreateNotificationDto dto,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Mark notification as read
    /// </summary>
    Task<Core.Results.Result> MarkAsReadAsync(
        Guid notificationId,
        Guid userId,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Mark multiple notifications as read
    /// </summary>
    Task<Core.Results.Result> MarkMultipleAsReadAsync(
        List<Guid> notificationIds,
        Guid userId,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Mark all notifications as read
    /// </summary>
    Task<Core.Results.Result> MarkAllAsReadAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Archive notification
    /// </summary>
    Task<Core.Results.Result> ArchiveNotificationAsync(
        Guid notificationId,
        Guid userId,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Delete notification
    /// </summary>
    Task<Core.Results.Result> DeleteNotificationAsync(
        Guid notificationId,
        Guid userId,
        CancellationToken cancellationToken = default);
    
    #endregion
    
    #region Template Operations
    
    /// <summary>
    /// Get all notification templates
    /// </summary>
    Task<List<NotificationTemplateDto>> GetTemplatesAsync(
        bool? activeOnly = true,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get template by code
    /// </summary>
    Task<NotificationTemplateDto?> GetTemplateByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Create notification template
    /// </summary>
    Task<Core.Results.Result<Guid>> CreateTemplateAsync(
        CreateNotificationTemplateDto dto,
        string userId,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Update notification template
    /// </summary>
    Task<Core.Results.Result> UpdateTemplateAsync(
        Guid templateId,
        UpdateNotificationTemplateDto dto,
        string userId,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Delete notification template
    /// </summary>
    Task<Core.Results.Result> DeleteTemplateAsync(
        Guid templateId,
        string userId,
        CancellationToken cancellationToken = default);
    
    #endregion
    
    #region Preference Operations
    
    /// <summary>
    /// Get user notification preferences
    /// </summary>
    Task<NotificationPreferenceDto?> GetUserPreferencesAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Update user notification preferences
    /// </summary>
    Task<Core.Results.Result> UpdateUserPreferencesAsync(
        Guid userId,
        UpdateNotificationPreferenceDto dto,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get user subscriptions
    /// </summary>
    Task<List<NotificationSubscriptionDto>> GetUserSubscriptionsAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Subscribe to entity notifications
    /// </summary>
    Task<Core.Results.Result<Guid>> SubscribeToEntityAsync(
        Guid userId,
        CreateNotificationSubscriptionDto dto,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Unsubscribe from entity notifications
    /// </summary>
    Task<Core.Results.Result> UnsubscribeFromEntityAsync(
        Guid subscriptionId,
        Guid userId,
        CancellationToken cancellationToken = default);
    
    #endregion
    
    #region Notification Sending
    
    /// <summary>
    /// Send notification using template
    /// </summary>
    Task<Core.Results.Result> SendNotificationFromTemplateAsync(
        string templateCode,
        Guid recipientId,
        Dictionary<string, string> placeholders,
        Guid? relatedEntityId = null,
        string? relatedEntityType = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Send notification to project team
    /// </summary>
    Task<Core.Results.Result> SendProjectNotificationAsync(
        Guid projectId,
        NotificationType type,
        string title,
        string message,
        Guid? excludeUserId = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Send notification to role
    /// </summary>
    Task<Core.Results.Result> SendRoleNotificationAsync(
        string roleName,
        NotificationType type,
        string title,
        string message,
        CancellationToken cancellationToken = default);
    
    #endregion
}

/// <summary>
/// Query parameters for notifications
/// </summary>
public class NotificationQueryParameters : Core.DTOs.Common.BaseQueryParameters
{
    public NotificationType? Type { get; set; }
    public NotificationPriority? Priority { get; set; }
    public string? Category { get; set; }
    public bool? IsRead { get; set; }
    public bool? IsArchived { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public Guid? RelatedEntityId { get; set; }
}