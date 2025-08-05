using Core.Enums.Progress;

namespace Core.DTOs.Projects.WorkPackageDetails;

/// <summary>
/// DTO for converting Planning Package to Work Package
/// </summary>
public class ConvertPlanningToWorkPackageDto
{
    /// <summary>
    /// Code for the new work package
    /// </summary>
    public string Code { get; set; } = string.Empty;
    
    /// <summary>
    /// Name for the new work package
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Description of the work package
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Progress measurement method
    /// </summary>
    public ProgressMethod ProgressMethod { get; set; }
    
    /// <summary>
    /// Planned start date
    /// </summary>
    public DateTime PlannedStartDate { get; set; }
    
    /// <summary>
    /// Planned end date
    /// </summary>
    public DateTime PlannedEndDate { get; set; }
    
    /// <summary>
    /// Budget amount
    /// </summary>
    public decimal Budget { get; set; }
    
    /// <summary>
    /// User responsible for the work package
    /// </summary>
    public Guid? ResponsibleUserId { get; set; }
    
    /// <summary>
    /// Primary discipline for the work package
    /// </summary>
    public Guid? PrimaryDisciplineId { get; set; }
    
    /// <summary>
    /// Weight factor for progress calculation
    /// </summary>
    public decimal? WeightFactor { get; set; }
}
