namespace Core.DTOs.Organization.Operation;

/// <summary>
/// DTO for operation with projects
/// </summary>
public class OperationWithProjectsDto : OperationDto
{
    public IEnumerable<OperationProjectDto> Projects { get; set; } = new List<OperationProjectDto>();
    public int TotalProjects { get; set; }
    public int ActiveProjects { get; set; }
    public decimal TotalBudget { get; set; }
    public decimal TotalActualCost { get; set; }
}

/// <summary>
/// Simplified project DTO for operation listing
/// </summary>
public class OperationProjectDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal TotalBudget { get; set; }
    public decimal? ActualCost { get; set; }
    public decimal ProgressPercentage { get; set; }
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
}