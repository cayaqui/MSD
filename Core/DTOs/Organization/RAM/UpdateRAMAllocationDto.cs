namespace Core.DTOs.Organization.RAM;

/// <summary>
/// DTO for updating RAM allocation
/// </summary>
public class UpdateRAMAllocationDto
{
    /// <summary>
    /// Allocation percentage (0-100)
    /// </summary>
    public decimal AllocationPercentage { get; set; }

    /// <summary>
    /// Planned man hours
    /// </summary>
    public decimal? PlannedManHours { get; set; }

    /// <summary>
    /// Planned cost
    /// </summary>
    public decimal? PlannedCost { get; set; }
}