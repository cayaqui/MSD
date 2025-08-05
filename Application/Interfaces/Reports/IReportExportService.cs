using Core.Enums.Documents;

namespace Application.Interfaces.Reports;

public interface IReportExportService
{
    // PDF Export
    Task<byte[]> ExportToPdfAsync(object reportData, string templatePath, Dictionary<string, object>? options = null);
    Task<byte[]> ExportToPdfFromHtmlAsync(string htmlContent, Dictionary<string, object>? options = null);
    
    // Excel Export
    Task<byte[]> ExportToExcelAsync(object reportData, string templatePath, Dictionary<string, object>? options = null);
    Task<byte[]> ExportDataToExcelAsync<T>(IEnumerable<T> data, string sheetName, bool includeHeaders = true);
    Task<byte[]> ExportMultipleSheetsToExcelAsync(Dictionary<string, object> sheets, Dictionary<string, object>? options = null);
    
    // Word Export
    Task<byte[]> ExportToWordAsync(object reportData, string templatePath, Dictionary<string, object>? options = null);
    Task<byte[]> ExportToWordFromHtmlAsync(string htmlContent, Dictionary<string, object>? options = null);
    
    // PowerPoint Export
    Task<byte[]> ExportToPowerPointAsync(object reportData, string templatePath, Dictionary<string, object>? options = null);
    Task<byte[]> CreatePowerPointPresentationAsync(List<SlideData> slides, Dictionary<string, object>? options = null);
    
    // CSV Export
    Task<byte[]> ExportToCsvAsync<T>(IEnumerable<T> data, bool includeHeaders = true);
    Task<byte[]> ExportToCsvWithCustomHeadersAsync<T>(IEnumerable<T> data, Dictionary<string, string> headerMappings);
    
    // HTML Export
    Task<string> ExportToHtmlAsync(object reportData, string templatePath, Dictionary<string, object>? options = null);
    Task<string> RenderHtmlTemplateAsync(string template, object data);
    
    // JSON/XML Export
    Task<byte[]> ExportToJsonAsync(object data, bool formatted = true);
    Task<byte[]> ExportToXmlAsync(object data, string rootElement = "Report");
    
    // Combined Export
    Task<byte[]> ExportToZipAsync(Dictionary<string, byte[]> files);
    Task<byte[]> MergeDocumentsAsync(List<byte[]> documents, ExportFormat format);
}

public class SlideData
{
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public List<SlideContent> Contents { get; set; } = new();
    public string? BackgroundImage { get; set; }
    public Dictionary<string, object>? CustomProperties { get; set; }
}

public class SlideContent
{
    public SlideContentType Type { get; set; }
    public object Data { get; set; } = null!;
    public SlideContentPosition Position { get; set; } = new();
    public Dictionary<string, object>? Formatting { get; set; }
}

public enum SlideContentType
{
    Text,
    Image,
    Chart,
    Table,
    Shape
}

public class SlideContentPosition
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }
}