namespace Core.DTOs.Projects;

/// <summary>
/// DTO for updating an existing project
/// </summary>
public class UpdateProjectDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }

    // Dates (optional updates)
    public DateTime? PlannedStartDate { get; set; }
    public DateTime? PlannedEndDate { get; set; }

    // Financial (optional updates)
    public decimal? TotalBudget { get; set; }

    // Additional Information (optional updates)
    public string? Location { get; set; }
    public string? Client { get; set; }
    public string? ContractNumber { get; set; }

    // Status changes (handled by separate endpoints usually)
    public string? Status { get; set; }
    public bool? IsActive { get; set; }
}
