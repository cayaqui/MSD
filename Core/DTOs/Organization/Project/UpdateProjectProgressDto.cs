namespace Core.DTOs.Organization.Project;

/// <summary>
/// DTO for updating project progress
/// </summary>
public class UpdateProjectProgressDto
{
    public decimal ProgressPercentage { get; set; }
    public string? Notes { get; set; }
    public DateTime? ProgressDate { get; set; }
}