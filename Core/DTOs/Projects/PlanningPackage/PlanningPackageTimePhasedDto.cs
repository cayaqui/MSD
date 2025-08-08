namespace Core.DTOs.Projects.PlanningPackage;

/// <summary>
/// DTO for planning package time-phased budget view
/// </summary>
public class PlanningPackageTimePhasedDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal TotalBudget { get; set; }
    public List<TimePhasedBudgetDto> TimePhasedBudget { get; set; } = new();
}