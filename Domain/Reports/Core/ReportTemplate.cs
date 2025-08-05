using Core.Enums.Documents;
using Core.Enums.Reports;
using Domain.Common;

namespace Domain.Reports.Core;

public class ReportTemplate : BaseAuditableEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ReportType Type { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Version { get; set; } = "1.0";
    
    // Template Configuration
    public string TemplateFile { get; set; } = string.Empty; // Path to template file
    public byte[]? TemplateContent { get; set; } // Template binary content
    public string SupportedFormatsJson { get; set; } = "[]"; // JSON array of ExportFormat
    public ReportLayout Layout { get; set; }
    public PageOrientation DefaultOrientation { get; set; }
    public string DefaultPaperSize { get; set; } = "A4";
    
    // Data Sources (stored as JSON)
    public string DataSourcesJson { get; set; } = "[]";
    
    // Parameters (stored as JSON)
    public string ParametersJson { get; set; } = "[]";
    
    // Sections (stored as JSON)
    public string SectionsJson { get; set; } = "[]";
    
    // Styling (stored as JSON)
    public string StyleJson { get; set; } = "{}";
    
    // Permissions
    public bool IsPublic { get; set; }
    public string AllowedRolesJson { get; set; } = "[]"; // JSON array of role names
    
    // Status
    public bool IsActive { get; set; } = true;
    public bool IsDefault { get; set; }
    
    // Navigation properties
    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();
    public virtual ICollection<ReportSchedule> Schedules { get; set; } = new List<ReportSchedule>();
}

//public enum ReportLayout
//{
//    Portrait,
//    Landscape,
//    Custom
//}

//public enum PageOrientation
//{
//    Portrait,
//    Landscape
//}
