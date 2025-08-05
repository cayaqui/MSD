using Core.Enums.Documents;
using Core.Enums.Reports;

namespace Core.DTOs.Reports;

public class ReportTemplateDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ReportType Type { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    
    // Template Configuration
    public string TemplateFile { get; set; } = string.Empty; // Path or content
    public ExportFormat[] SupportedFormats { get; set; } = Array.Empty<ExportFormat>();
    public ReportLayout Layout { get; set; }
    public PageOrientation DefaultOrientation { get; set; }
    public string DefaultPaperSize { get; set; } = "A4";
    
    // Data Sources
    public List<DataSourceDto> DataSources { get; set; } = new();
    
    // Parameters
    public List<ReportParameterDto> Parameters { get; set; } = new();
    
    // Sections
    public List<ReportSectionDto> Sections { get; set; } = new();
    
    // Styling
    public ReportStyleDto Style { get; set; } = new();
    
    // Permissions
    public bool IsPublic { get; set; }
    public List<string> AllowedRoles { get; set; } = new();
    
    // Status
    public bool IsActive { get; set; }
    public bool IsDefault { get; set; }
    
    // Audit
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? ModifiedDate { get; set; }
    public string? ModifiedBy { get; set; }
}

