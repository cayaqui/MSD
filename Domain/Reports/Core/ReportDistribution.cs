using Domain.Entities.Auth.Security;

namespace Domain.Reports.Core;

public class ReportDistribution : BaseAuditableEntity
{
    public Guid ReportId { get; set; }
    public virtual Report Report { get; set; } = null!;
    
    public string RecipientEmail { get; set; } = string.Empty;
    public string? RecipientName { get; set; }
    public Guid? RecipientUserId { get; set; }
    public virtual User? RecipientUser { get; set; }
    
    public DateTime SentDate { get; set; }
    public bool IsSuccessful { get; set; }
    public string? ErrorMessage { get; set; }
    
    public string? EmailSubject { get; set; }
    public string? EmailBody { get; set; }
    public int? AttachmentSize { get; set; }
}