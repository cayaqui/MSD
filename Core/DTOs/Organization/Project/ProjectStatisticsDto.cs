namespace Core.DTOs.Organization.Project;

public class ProjectStatisticsDto
{
    public int TotalProjects { get; set; }
    public int ActiveProjects { get; set; }
    public int CompletedProjects { get; set; }
    public int OnHoldProjects { get; set; }
    public int CancelledProjects { get; set; }
    public decimal TotalBudget { get; set; }
    public decimal TotalActualCost { get; set; }
    public double AverageProgress { get; set; }
    public int TotalTeamMembers { get; set; }
    public Dictionary<string, int> ProjectsByType { get; set; } = new();
    public Dictionary<string, decimal> BudgetByType { get; set; } = new();
}