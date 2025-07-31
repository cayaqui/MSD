namespace Core.DTOs.Projects;

/// <summary>
/// DTO for filtering projects in search/list operations
/// </summary>
public class ProjectFilterDto
{
    // Basic filters
    public string? SearchTerm { get; set; }
    public Guid? OperationId { get; set; }
    public Guid? CompanyId { get; set; }

    // Status filters
    public string? Status { get; set; }
    public bool? IsActive { get; set; }

    // Date range filters
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateTo { get; set; }
    public DateTime? EndDateFrom { get; set; }
    public DateTime? EndDateTo { get; set; }

    // Budget filters
    public decimal? MinBudget { get; set; }
    public decimal? MaxBudget { get; set; }
    public string? Currency { get; set; }

    // Additional filters
    public string? Client { get; set; }
    public string? Location { get; set; }
    public string? ProjectManagerId { get; set; }

    // User context filters
    public bool? OnlyMyProjects { get; set; }
    public string? UserRole { get; set; }
}
