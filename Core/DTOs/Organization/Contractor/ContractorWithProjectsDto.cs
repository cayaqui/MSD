using Core.DTOs.Organization.Project;

namespace Core.DTOs.Organization.Contractor;

/// <summary>
/// Contractor with projects data transfer object
/// </summary>
public class ContractorWithProjectsDto : ContractorDto
{
    public List<ProjectSummaryDto> Projects { get; set; } = new();
    public int ActiveProjectsCount { get; set; }
    public int CompletedProjectsCount { get; set; }
}