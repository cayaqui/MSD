namespace Core.DTOs.Cost.PlanningPackages;

public record PlanningPackageBulkOperationResult(int SuccessCount, int TotalCount);

public record BudgetAllocation(Guid PlanningPackageId, decimal NewBudget);

public record PlanningPackageBudgetSummaryDto
{
    public Guid ControlAccountId { get; set; }
    public decimal TotalBudget { get; set; }
    public decimal AllocatedBudget { get; set; }
    public decimal UnallocatedBudget { get; set; }
    public int PlanningPackageCount { get; set; }
}