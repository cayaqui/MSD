using Core.Enums.Projects;

namespace Core.DTOs.Organization.Project;

/// <summary>
/// Project status history data transfer object
/// </summary>
public class ProjectStatusHistoryDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public ProjectStatus FromStatus { get; set; }
    public ProjectStatus ToStatus { get; set; }
    public string? Reason { get; set; }
    public DateTime ChangedAt { get; set; }
    public string? ChangedBy { get; set; }
    public string? ChangedByName { get; set; }
}