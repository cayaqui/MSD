namespace Core.DTOs.Configuration.SystemParameters;

public class UpdateSystemParameterDto
{
    public string Value { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string? Description { get; set; }
    public bool IsRequired { get; set; }
    public bool IsPublic { get; set; }
    public string? ValidationRule { get; set; }
    public int? MinValue { get; set; }
    public int? MaxValue { get; set; }
    public List<string>? AllowedValues { get; set; }
}
