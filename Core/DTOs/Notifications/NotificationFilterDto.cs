using Core.Enums.UI;
namespace Core.DTOs.Notifications;

/// <summary>
/// DTO for notification filter/search
/// </summary>
public class NotificationFilterDto
{
    public string? SearchTerm { get; set; }
    public NotificationStatus? Status { get; set; }
    public NotificationType? Type { get; set; }
    public NotificationPriority? Priority { get; set; }
    public bool? IsImportant { get; set; }

    // Date filters
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }

    // Context filters
    public Guid? ProjectId { get; set; }
    public Guid? CompanyId { get; set; }

    // Pagination
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
