namespace Core.DTOs.Reports;

public class ReportParameterDto
{
    public string Name { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty; // String, Date, Number, Boolean, List
    public bool IsRequired { get; set; }
    public object? DefaultValue { get; set; }
    public List<ParameterOptionDto>? Options { get; set; }
    public string? ValidationRule { get; set; }
}

