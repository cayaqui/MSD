namespace Core.DTOs.Configuration.OBSTemplates;

/// <summary>
/// DTO for updating OBS node capacity
/// </summary>
public class UpdateOBSCapacityDto
{
    /// <summary>
    /// Total Full-Time Equivalent capacity
    /// </summary>
    public decimal? TotalFTE { get; set; }
    
    /// <summary>
    /// Currently allocated FTE
    /// </summary>
    public decimal? AllocatedFTE { get; set; }
    
    /// <summary>
    /// Capacity allocations by role
    /// </summary>
    public Dictionary<string, decimal>? CapacityByRole { get; set; }
    
    /// <summary>
    /// Capacity allocations by project
    /// </summary>
    public Dictionary<string, decimal>? CapacityByProject { get; set; }
}