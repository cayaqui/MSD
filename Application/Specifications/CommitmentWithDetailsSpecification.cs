using Domain.Entities.Cost.Commitments;

namespace Application.Specifications;

/// <summary>
/// Specification for getting commitment with all details
/// </summary>
public class CommitmentWithDetailsSpecification : BaseSpecification<Commitment>
{
    public CommitmentWithDetailsSpecification(Guid commitmentId)
    {
        AddCriteria(c => c.Id == commitmentId && !c.IsDeleted);

        // Add includes using string-based includes for nested properties
        AddInclude(c => c.Project);
        AddInclude(c => c.Contractor!);
        AddInclude(c => c.BudgetItem!);
        AddInclude(c => c.ControlAccount!);
        AddInclude(c => c.Items);
        AddInclude("WorkPackageAllocations.WBSElement");
        AddInclude(c => c.Revisions);
        AddInclude(c => c.Invoices);
    }
}
