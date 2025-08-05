namespace Core.DTOs.Organization.Project;

/// <summary>
/// DTO for updating project budget
/// </summary>
public class UpdateProjectBudgetDto
{
    public decimal TotalBudget { get; set; }
    public decimal? ApprovedBudget { get; set; }
    public decimal? ContingencyBudget { get; set; }
}