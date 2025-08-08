namespace Core.DTOs.Projects.PlanningPackage;

/// <summary>
/// DTO for planning package basic information
/// </summary>
public class PlanningPackageDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public Guid ControlAccountId { get; set; }
    public string ControlAccountCode { get; set; } = string.Empty;
    public Guid PhaseId { get; set; }
    public string PhaseName { get; set; } = string.Empty;
    public decimal TotalBudget { get; set; }
    public decimal DistributedBudget { get; set; }
    public decimal RemainingBudget { get; set; }
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
}