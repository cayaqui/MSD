namespace Core.DTOs.Configuration.SystemParameters;

public class SystemParameterFilterDto
{
    public string? Category { get; set; }
    public string? SearchTerm { get; set; }
    public bool? IsSystem { get; set; }
    public bool? IsPublic { get; set; }
    public string? DataType { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}
