namespace Core.DTOs.Projects.WBSElement;

/// <summary>
/// DTO for updating WBS Dictionary
/// </summary>
public class UpdateWBSDictionaryDto
{
    public string? DeliverableDescription { get; set; }
    public string? AcceptanceCriteria { get; set; }
    public string? Assumptions { get; set; }
    public string? Constraints { get; set; }
    public string? ExclusionsInclusions { get; set; }
}
