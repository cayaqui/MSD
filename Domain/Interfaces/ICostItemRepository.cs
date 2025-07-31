namespace Domain.Interfaces;

/// <summary>
/// Specific repository for Cost Item entities
/// </summary>
public interface ICostItemRepository : IRepository<CostItem>
{
    Task<IEnumerable<CostItem>> GetByProjectAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<IEnumerable<CostItem>> GetByWorkPackageAsync(Guid workPackageId, CancellationToken cancellationToken = default);
    Task<IEnumerable<CostItem>> GetByControlAccountAsync(Guid controlAccountId, CancellationToken cancellationToken = default);
    Task<IEnumerable<CostItem>> GetByCategoryAsync(Guid projectId, CostCategory category, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalActualCostByProjectAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalCommittedCostByProjectAsync(Guid projectId, CancellationToken cancellationToken = default);
}
