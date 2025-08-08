using Core.Results;

namespace Application.Interfaces.Notifications;

/// <summary>
/// Service for sending emails
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Send a simple email
    /// </summary>
    Task<Core.Results.Result> SendEmailAsync(
        string to,
        string subject,
        string body,
        bool isHtml = false,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Send email to multiple recipients
    /// </summary>
    Task<Core.Results.Result> SendEmailAsync(
        List<string> to,
        string subject,
        string body,
        bool isHtml = false,
        List<string>? cc = null,
        List<string>? bcc = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Send email with attachments
    /// </summary>
    Task<Core.Results.Result> SendEmailWithAttachmentsAsync(
        string to,
        string subject,
        string body,
        List<EmailAttachment> attachments,
        bool isHtml = false,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Send templated email
    /// </summary>
    Task<Core.Results.Result> SendTemplatedEmailAsync(
        string to,
        string templateId,
        Dictionary<string, string> templateData,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Send bulk emails
    /// </summary>
    Task<Core.Results.Result<BulkEmailResult>> SendBulkEmailsAsync(
        List<BulkEmailRequest> requests,
        CancellationToken cancellationToken = default);
}

public class EmailAttachment
{
    public string FileName { get; set; } = string.Empty;
    public byte[] Content { get; set; } = Array.Empty<byte>();
    public string ContentType { get; set; } = string.Empty;
}

public class BulkEmailRequest
{
    public string To { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsHtml { get; set; }
    public Dictionary<string, string>? TemplateData { get; set; }
}

public class BulkEmailResult
{
    public int TotalRequests { get; set; }
    public int SuccessfulSends { get; set; }
    public int FailedSends { get; set; }
    public List<string> Errors { get; set; } = new();
}