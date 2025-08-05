namespace Core.DTOs.Projects.WBSElement;

/// <summary>
/// DTO for WBS-CBS mapping
/// </summary>
public class WBSCBSMappingDto
{
    public Guid Id { get; set; }
    public Guid WBSElementId { get; set; }
    public Guid CBSId { get; set; }
    public string CBSCode { get; set; } = string.Empty;
    public string CBSName { get; set; } = string.Empty;
    public decimal AllocationPercentage { get; set; }
    public bool IsPrimary { get; set; }
    public string? AllocationBasis { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime? EndDate { get; set; }
}
