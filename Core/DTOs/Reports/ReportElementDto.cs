namespace Core.DTOs.Reports;

public class ReportElementDto
{
    public string Type { get; set; } = string.Empty; // Text, Table, Chart, Image
    public string DataSource { get; set; } = string.Empty;
    public Dictionary<string, object> Properties { get; set; } = new();
    public Dictionary<string, string> Bindings { get; set; } = new();
}

