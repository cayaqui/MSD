using Core.DTOs.Notifications;
using Core.Enums.UI;

namespace Application.Interfaces.UIUX;

public interface INotificationService
{
    Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync();
    Task<PagedResult<NotificationDto>> GetPagedNotificationsAsync(NotificationFilterDto filter);
    Task<int> GetUnreadCountAsync();
    Task<NotificationSummaryDto> GetSummaryAsync();
    Task MarkAsReadAsync(Guid notificationId);
    Task MarkAllAsReadAsync();
    Task<NotificationDto> CreateAsync(CreateNotificationDto dto);
    Task CreateProjectNotificationAsync(Guid projectId, string title, string message, NotificationType type = NotificationType.Information, ProjectNotificationCategory category = ProjectNotificationCategory.General);
    Task CreateBulkNotificationAsync(string title, string message, NotificationType type, Expression<Func<User, bool>> userFilter);
    Task ProcessBatchActionAsync(BatchNotificationDto dto);
    Task DeleteNotificationAsync(Guid notificationId);
    Task<NotificationPreferencesDto> GetUserPreferencesAsync();
    Task UpdateUserPreferencesAsync(NotificationPreferencesDto preferences);
}