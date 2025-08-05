using Core.DTOs.Organization.Project;

namespace Core.DTOs.Organization.Phase;

/// <summary>
/// Phase with milestones data transfer object
/// </summary>
public class PhaseWithMilestonesDto : PhaseDto
{
    public List<MilestoneDto> Milestones { get; set; } = new();
}