using Core.Enums.Documents;
using Core.Enums.Projects;

namespace Core.DTOs.Reports;

public class GenerateReportDto
{
    public Guid TemplateId { get; set; }
    public Guid? ProjectId { get; set; }
    public ExportFormat Format { get; set; }
    public Dictionary<string, object> Parameters { get; set; } = new();
    public DateTime? ReportingPeriodStart { get; set; }
    public DateTime? ReportingPeriodEnd { get; set; }
    public List<string>? Recipients { get; set; }
    public bool SendByEmail { get; set; }
    public string? CustomFileName { get; set; }
    public ReportOptionsDto? Options { get; set; }
}

public class ReportOptionsDto
{
    public bool IncludeCoverPage { get; set; } = true;
    public bool IncludeTableOfContents { get; set; } = true;
    public bool IncludeExecutiveSummary { get; set; } = true;
    public bool IncludeCharts { get; set; } = true;
    public bool IncludeAppendix { get; set; } = false;
    public PageOrientation? Orientation { get; set; }
    public string? PaperSize { get; set; }
    public bool CompressImages { get; set; } = true;
    public int? ImageQuality { get; set; } = 85; // 1-100
    public bool ProtectDocument { get; set; } = false;
    public string? DocumentPassword { get; set; }
    public Dictionary<string, bool> SectionVisibility { get; set; } = new();
}