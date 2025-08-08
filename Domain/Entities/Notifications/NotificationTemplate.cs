using Domain.Common;

namespace Domain.Entities.Notifications;

public class NotificationTemplate : BaseEntity, ISoftDelete
{
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public Core.Enums.Notifications.NotificationType Type { get; private set; }
    public string Category { get; private set; } = string.Empty;
    
    // Template content
    public string SubjectTemplate { get; private set; } = string.Empty;
    public string BodyTemplate { get; private set; } = string.Empty;
    
    // Channel settings
    public bool SendEmail { get; private set; }
    public string? EmailSubjectTemplate { get; private set; }
    public string? EmailBodyTemplate { get; private set; }
    public bool IsHtmlEmail { get; private set; }
    
    public bool SendInApp { get; private set; }
    public NotificationPriority DefaultPriority { get; private set; }
    public int? ExpirationDays { get; private set; }
    
    public bool SendPush { get; private set; }
    public string? PushTitleTemplate { get; private set; }
    public string? PushBodyTemplate { get; private set; }
    
    // Status
    public bool IsActive { get; private set; }
    
    // Available placeholders and actions stored as JSON
    public string? PlaceholdersJson { get; private set; }
    public string? DefaultActionsJson { get; private set; }
    
    // Soft delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
    
    private NotificationTemplate() { } // For EF Core
    
    public NotificationTemplate(
        string code,
        string name,
        string description,
        Core.Enums.Notifications.NotificationType type,
        string category,
        string subjectTemplate,
        string bodyTemplate)
    {
        Code = code;
        Name = name;
        Description = description;
        Type = type;
        Category = category;
        SubjectTemplate = subjectTemplate;
        BodyTemplate = bodyTemplate;
        SendInApp = true;
        DefaultPriority = NotificationPriority.Normal;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }
    
    public void UpdateDetails(
        string name,
        string description,
        string subjectTemplate,
        string bodyTemplate)
    {
        Name = name;
        Description = description;
        SubjectTemplate = subjectTemplate;
        BodyTemplate = bodyTemplate;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void ConfigureEmail(
        bool sendEmail,
        string? emailSubjectTemplate,
        string? emailBodyTemplate,
        bool isHtmlEmail)
    {
        SendEmail = sendEmail;
        EmailSubjectTemplate = emailSubjectTemplate;
        EmailBodyTemplate = emailBodyTemplate;
        IsHtmlEmail = isHtmlEmail;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void ConfigureInApp(
        bool sendInApp,
        NotificationPriority defaultPriority,
        int? expirationDays)
    {
        SendInApp = sendInApp;
        DefaultPriority = defaultPriority;
        ExpirationDays = expirationDays;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void ConfigurePush(
        bool sendPush,
        string? pushTitleTemplate,
        string? pushBodyTemplate)
    {
        SendPush = sendPush;
        PushTitleTemplate = pushTitleTemplate;
        PushBodyTemplate = pushBodyTemplate;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void SetPlaceholders(string placeholdersJson)
    {
        PlaceholdersJson = placeholdersJson;
    }
    
    public void SetDefaultActions(string defaultActionsJson)
    {
        DefaultActionsJson = defaultActionsJson;
    }
    
    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}