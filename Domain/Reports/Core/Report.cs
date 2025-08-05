using Core.Enums.Reports;

namespace Domain.Reports.Core;

public class Report : BaseAuditableEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ReportType Type { get; set; }
    public string Category { get; set; } = string.Empty;
    
    // Template Information
    public Guid? TemplateId { get; set; }
    public virtual ReportTemplate? Template { get; set; }
    public string TemplateVersion { get; set; } = string.Empty;
    
    // Project Association
    public Guid? ProjectId { get; set; }
    public virtual Project? Project { get; set; }
    
    // Report Parameters (stored as JSON)
    public string ParametersJson { get; set; } = "{}";
    public DateTime? ReportingPeriodStart { get; set; }
    public DateTime? ReportingPeriodEnd { get; set; }
    
    // Generation Details
    public DateTime GeneratedDate { get; set; }
    public Guid GeneratedById { get; set; }
    public virtual User GeneratedBy { get; set; } = null!;
    public ReportStatus Status { get; set; }
    public string? ErrorMessage { get; set; }
    
    // File Information
    public string? FileName { get; set; }
    public string? FilePath { get; set; }
    public long? FileSize { get; set; }
    public string? ContentType { get; set; }
    public byte[]? FileContent { get; set; } // For storing in database
    
    // Distribution
    public string RecipientsJson { get; set; } = "[]"; // JSON array of email addresses
    public DateTime? DistributedDate { get; set; }
    public bool IsPublic { get; set; }
    
    // Navigation properties
    public virtual ICollection<ReportDistribution> Distributions { get; set; } = new List<ReportDistribution>();
}
