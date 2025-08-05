namespace Core.DTOs.Configuration.SystemParameters;

public class SystemParameterCategoryDto
{
    public string Category { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int ParameterCount { get; set; }
    public List<SystemParameterDto> Parameters { get; set; } = new();
}
