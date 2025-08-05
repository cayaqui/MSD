namespace Core.DTOs.Reports;

public class ReportSectionDto
{
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int Order { get; set; }
    public bool IsVisible { get; set; } = true;
    public string? Condition { get; set; } // Expression to evaluate visibility
    public List<ReportElementDto> Elements { get; set; } = new();
}

