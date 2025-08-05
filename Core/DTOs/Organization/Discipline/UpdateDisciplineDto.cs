namespace Core.DTOs.Organization.Discipline;

/// <summary>
/// DTO for updating discipline information
/// </summary>
public class UpdateDisciplineDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Order { get; set; }
    public string ColorHex { get; set; } = "#000000";
    public string? Icon { get; set; }
    public bool IsEngineering { get; set; }
    public bool IsManagement { get; set; }
}