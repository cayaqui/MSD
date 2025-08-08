using Core.Enums.Projects;

namespace Core.DTOs.Projects.PlanningPackage;

/// <summary>
/// DTO for distributing planning package budget across time periods
/// </summary>
public class DistributePlanningPackageBudgetDto
{
    public BudgetDistributionMethod Method { get; set; }
    public List<Guid> PhaseIds { get; set; } = new();
    public decimal? TotalBudget { get; set; } // If null, use existing total
    public Dictionary<Guid, decimal>? CustomDistribution { get; set; } // For custom distribution by phase
}