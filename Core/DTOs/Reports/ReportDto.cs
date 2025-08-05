using Core.Enums.Reports;

namespace Core.DTOs.Reports;

public class ReportDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ReportType Type { get; set; }
    public string Category { get; set; } = string.Empty;
    
    // Template Information
    public Guid? TemplateId { get; set; }
    public string? TemplateName { get; set; }
    public string TemplateVersion { get; set; } = string.Empty;
    
    // Project Association
    public Guid? ProjectId { get; set; }
    public string? ProjectName { get; set; }
    public string? ProjectCode { get; set; }
    
    // Report Parameters
    public Dictionary<string, object> Parameters { get; set; } = new();
    public DateTime? ReportingPeriodStart { get; set; }
    public DateTime? ReportingPeriodEnd { get; set; }
    
    // Generation Details
    public DateTime GeneratedDate { get; set; }
    public Guid GeneratedById { get; set; }
    public string GeneratedByName { get; set; } = string.Empty;
    public ReportStatus Status { get; set; }
    public string? ErrorMessage { get; set; }
    
    // File Information
    public string? FileName { get; set; }
    public string? FilePath { get; set; }
    public long? FileSize { get; set; }
    public string? ContentType { get; set; }
    
    // Distribution
    public List<string> Recipients { get; set; } = new();
    public DateTime? DistributedDate { get; set; }
    public bool IsPublic { get; set; }
    
    // Audit
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? ModifiedDate { get; set; }
    public string? ModifiedBy { get; set; }
}
