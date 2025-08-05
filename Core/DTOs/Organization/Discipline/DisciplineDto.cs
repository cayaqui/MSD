namespace Core.DTOs.Organization.Discipline;

/// <summary>
/// Discipline data transfer object
/// </summary>
public class DisciplineDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Order { get; set; }
    
    // Visual
    public string ColorHex { get; set; } = "#000000";
    public string? Icon { get; set; }
    
    // Category
    public bool IsEngineering { get; set; }
    public bool IsManagement { get; set; }
    
    // Audit
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}