namespace Core.DTOs.Organization.Project;

/// <summary>
/// DTO for project status change
/// </summary>
public class ChangeProjectStatusDto
{
    public string Status { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public DateTime? EffectiveDate { get; set; }
}