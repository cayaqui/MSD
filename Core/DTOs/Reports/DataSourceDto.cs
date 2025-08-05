namespace Core.DTOs.Reports;

public class DataSourceDto
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // Query, API, Service
    public string Source { get; set; } = string.Empty; // SQL, Endpoint, Method
    public Dictionary<string, string> Parameters { get; set; } = new();
}

