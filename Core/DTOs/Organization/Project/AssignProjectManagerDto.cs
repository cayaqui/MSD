namespace Core.DTOs.Organization.Project;

/// <summary>
/// DTO for assigning project manager
/// </summary>
public class AssignProjectManagerDto
{
    public string ProjectManagerId { get; set; } = string.Empty;
    public string ProjectManagerName { get; set; } = string.Empty;
}