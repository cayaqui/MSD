namespace Core.DTOs.Projects.PlanningPackage;

/// <summary>
/// DTO for updating planning package budget
/// </summary>
public class UpdatePlanningPackageBudgetDto
{
    public decimal TotalBudget { get; set; }
    public List<UpdatePlanningPackageEntryDto> Entries { get; set; } = new();
}