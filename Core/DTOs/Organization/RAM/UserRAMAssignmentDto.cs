namespace Core.DTOs.Organization.RAM;

/// <summary>
/// User RAM assignment data transfer object
/// </summary>
public class UserRAMAssignmentDto
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public Guid WBSElementId { get; set; }
    public string WBSCode { get; set; } = string.Empty;
    public string WBSName { get; set; } = string.Empty;
    public string ResponsibilityType { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public bool IsActive { get; set; }
    public DateTime AssignedDate { get; set; }
}