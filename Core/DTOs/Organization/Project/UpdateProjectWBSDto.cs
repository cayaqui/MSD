namespace Core.DTOs.Organization.Project;

/// <summary>
/// DTO for updating project WBS profile
/// </summary>
public class UpdateProjectWBSDto
{
    public string WBSCode { get; set; } = string.Empty;
    public string? Client { get; set; }
    public string? ContractNumber { get; set; }
}