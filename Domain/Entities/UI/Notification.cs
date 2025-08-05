using Domain.Common;
using Domain.Entities.Auth.Security;
using Domain.Entities.Organization.Core;
using Core.Enums.UI;
using System;

namespace Domain.Entities.UI;

public class Notification : BaseEntity, IAuditable, ISoftDelete
{
    // Propiedades básicas
    public string Title { get; private set; } = string.Empty;
    public string Message { get; private set; } = string.Empty;
    public NotificationType Type { get; private set; }
    public NotificationPriority Priority { get; private set; }
    public NotificationStatus Status { get; private set; }

    // Metadatos
    public DateTime? ReadAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public bool IsImportant { get; private set; }

    // Relaciones
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    // Contexto opcional (puede estar relacionada con proyecto, costo, etc.)
    public Guid? ProjectId { get; private set; }
    public Project? Project { get; private set; }

    public Guid? CompanyId { get; private set; }
    public Company? Company { get; private set; }

    // Datos adicionales en JSON
    public string? MetadataJson { get; private set; }
    public string? ActionUrl { get; private set; }

    // Soft Delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    private Notification() { } // EF Core

    public Notification(
        string title,
        string message,
        Guid userId,
        NotificationType type = NotificationType.Info,
        NotificationPriority priority = NotificationPriority.Normal
    )
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Message = message ?? throw new ArgumentNullException(nameof(message));
        UserId = userId;
        Type = type;
        Priority = priority;
        Status = NotificationStatus.Unread;
        CreatedAt = DateTime.UtcNow;
    }

    // Métodos de dominio
    public void MarkAsRead()
    {
        if (Status == NotificationStatus.Read)
            return;

        Status = NotificationStatus.Read;
        ReadAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsUnread()
    {
        Status = NotificationStatus.Unread;
        ReadAt = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Archive()
    {
        Status = NotificationStatus.Archived;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetImportant(bool isImportant)
    {
        IsImportant = isImportant;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetProjectContext(Guid projectId)
    {
        ProjectId = projectId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetCompanyContext(Guid companyId)
    {
        CompanyId = companyId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetMetadata(string metadataJson)
    {
        MetadataJson = metadataJson;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetActionUrl(string actionUrl)
    {
        ActionUrl = actionUrl;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetExpiration(DateTime expiresAt)
    {
        ExpiresAt = expiresAt;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetUser(User user)
    {
        UserId = user.Id;
        User = user;
    }

    public void SetProject(Project project)
    {
        ProjectId = project.Id;
        Project = project;
    }

    public void SetCompany(Company company)
    {
        CompanyId = company.Id;
        Company = company;
    }

    public bool IsExpired => ExpiresAt.HasValue && ExpiresAt.Value < DateTime.UtcNow;
}
