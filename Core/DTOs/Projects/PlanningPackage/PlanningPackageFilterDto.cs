using Core.DTOs.Common;

namespace Core.DTOs.Projects.PlanningPackage;

/// <summary>
/// Filter DTO for planning packages
/// </summary>
public class PlanningPackageFilterDto : BaseFilterDto
{
    public Guid? ControlAccountId { get; set; }
    public Guid? PhaseId { get; set; }
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateTo { get; set; }
    public DateTime? EndDateFrom { get; set; }
    public DateTime? EndDateTo { get; set; }
    public decimal? MinBudget { get; set; }
    public decimal? MaxBudget { get; set; }
    public bool? OnlyActive { get; set; }
}