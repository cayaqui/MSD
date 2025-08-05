namespace Core.DTOs.Reports;

public class ReportStyleDto
{
    public string FontFamily { get; set; } = "Arial";
    public int FontSize { get; set; } = 11;
    public string PrimaryColor { get; set; } = "#1976D2";
    public string SecondaryColor { get; set; } = "#424242";
    public bool ShowPageNumbers { get; set; } = true;
    public bool ShowGenerationDate { get; set; } = true;
    public string? HeaderTemplate { get; set; }
    public string? FooterTemplate { get; set; }
    public Dictionary<string, string> CustomStyles { get; set; } = new();
}

