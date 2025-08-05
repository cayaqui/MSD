using Core.Enums.Cost;

namespace Core.DTOs.Projects.WBSElement;

/// <summary>
/// DTO for WBS Dictionary information
/// </summary>
public class WBSDictionaryDto
{
    public Guid WBSElementId { get; set; }
    public string? DeliverableDescription { get; set; }
    public string? AcceptanceCriteria { get; set; }
    public string? Assumptions { get; set; }
    public string? Constraints { get; set; }
    public string? ExclusionsInclusions { get; set; }
}
