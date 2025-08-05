using Core.DTOs.Organization.Project;

namespace Core.DTOs.Organization.Phase;

/// <summary>
/// Phase with deliverables data transfer object
/// </summary>
public class PhaseWithDeliverablesDto : PhaseDto
{
    public List<DeliverableDto> Deliverables { get; set; } = new();
}