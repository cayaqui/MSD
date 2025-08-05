using Core.DTOs.Organization.OBSNode;

namespace Core.DTOs.Configuration.OBSTemplates;

public class OBSNodeDetailDto : OBSNodeDto
{
    public List<OBSNodeDto> Children { get; set; } = new();
    public List<OBSResourceAllocationDto> ResourceAllocations { get; set; } = new();
    public OBSCapacityDto Capacity { get; set; } = new();
}
