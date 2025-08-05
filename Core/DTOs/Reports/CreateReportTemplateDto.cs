using Core.Enums.Documents;
using Core.Enums.Reports;

namespace Core.DTOs.Reports;


public class CreateReportTemplateDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ReportType Type { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Version { get; set; } = "1.0";
    public ExportFormat[] SupportedFormats { get; set; } = Array.Empty<ExportFormat>();
    public ReportLayout Layout { get; set; }
    public PageOrientation DefaultOrientation { get; set; }
    public string DefaultPaperSize { get; set; } = "A4";
    public List<DataSourceDto> DataSources { get; set; } = new();
    public List<ReportParameterDto> Parameters { get; set; } = new();
    public List<ReportSectionDto> Sections { get; set; } = new();
    public ReportStyleDto Style { get; set; } = new();
    public bool IsPublic { get; set; }
    public List<string> AllowedRoles { get; set; } = new();
}
