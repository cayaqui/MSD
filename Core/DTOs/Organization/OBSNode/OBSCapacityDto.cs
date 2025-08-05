namespace Core.DTOs.Organization.OBSNode;

/// <summary>
/// DTO for OBS node capacity information
/// </summary>
public class OBSCapacityDto
{
    public Guid NodeId { get; set; }
    public string NodeName { get; set; } = string.Empty;
    public decimal? TotalFTE { get; set; }
    public decimal? AvailableFTE { get; set; }
    public decimal? AllocatedFTE { get; set; }
    public decimal UtilizationRate { get; set; }
    public int MemberCount { get; set; }
    public List<OBSCapacityDto>? ChildCapacity { get; set; }
}