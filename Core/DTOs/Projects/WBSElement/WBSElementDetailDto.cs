using Core.DTOs.Projects.WorkPackageDetails;

namespace Core.DTOs.Projects.WBSElement;

/// <summary>
/// DTO for WBS Element detailed view
/// </summary>
public class WBSElementDetailDto : WBSElementDto
{
    public WBSDictionaryDto? Dictionary { get; set; }
    public WorkPackageDetailsDto? WorkPackageDetails { get; set; }
    public List<WBSElementDto> Children { get; set; } = [];
    public List<WBSCBSMappingDto> CBSMappings { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}
