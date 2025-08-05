namespace Core.DTOs.Organization.Phase;

/// <summary>
/// Update phase budget data transfer object
/// </summary>
public class UpdatePhaseBudgetDto
{
    public decimal PlannedBudget { get; set; }
    public string? Justification { get; set; }
}