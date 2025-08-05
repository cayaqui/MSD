using Core.Enums.UI;

namespace Core.DTOs.UI.Notifications;

/// <summary>
/// DTO for project-specific notification
/// </summary>
public class ProjectNotificationDto : CreateNotificationDto
{
    public ProjectNotificationCategory Category { get; set; }
    public string? ReferenceId { get; set; } // e.g., PhaseId, PackageId
    public string? ReferenceName { get; set; }
}
