using Core.Enums.Projects;

namespace Core.DTOs.Organization.Project;

/// <summary>
/// Milestone data transfer object
/// </summary>
public class MilestoneDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? PhaseId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime PlannedDate { get; set; }
    public DateTime? ActualDate { get; set; }
    public ProjectMilestoneType Type { get; set; }
    public ProjectMilestoneStatus Status { get; set; }
    public bool IsCritical { get; set; }
    public string? CompletionCriteria { get; set; }
    public string? Notes { get; set; }
}