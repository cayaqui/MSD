namespace Core.DTOs.Configuration.SystemParameters;

public class CreateSystemParameterDto
{
    public string Category { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string? Description { get; set; }
    public string DataType { get; set; } = "String";
    public bool IsEncrypted { get; set; }
    public bool IsRequired { get; set; } = true;
    public bool IsPublic { get; set; }
    public string? ValidationRule { get; set; }
    public string? DefaultValue { get; set; }
    public int? MinValue { get; set; }
    public int? MaxValue { get; set; }
    public List<string>? AllowedValues { get; set; }
}
