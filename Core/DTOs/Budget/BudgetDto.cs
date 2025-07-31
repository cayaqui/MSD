using Core.Enums.Cost;

namespace Core.DTOs.Budget;

/// <summary>
/// DTO for Budget list view
/// </summary>
public class BudgetDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public BudgetStatus Status { get; set; }
    public BudgetType Type { get; set; }
    public bool IsBaseline { get; set; }
    public string Currency { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal ContingencyAmount { get; set; }
    public decimal ManagementReserve { get; set; }
    public decimal TotalBudget { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}
