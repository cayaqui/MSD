namespace Core.DTOs.Projects;

/// <summary>
/// DTO for cancelling a project
/// </summary>
public class CancelProjectDto
{
    public string Reason { get; set; } = string.Empty;
    public string? ImpactDescription { get; set; }
}
