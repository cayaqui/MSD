namespace Domain.Interfaces;

/// <summary>
/// Specific repository for Work Package entities
/// </summary>
public interface IWorkPackageRepository : IRepository<WorkPackage>
{
    Task<WorkPackage?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<WorkPackage>> GetByControlAccountAsync(Guid controlAccountId, CancellationToken cancellationToken = default);
    Task<IEnumerable<WorkPackage>> GetByProjectAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<IEnumerable<WorkPackage>> GetByResponsibleAsync(string userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<WorkPackage>> GetCriticalPathAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalActualCostByControlAccountAsync(Guid controlAccountId, CancellationToken cancellationToken = default);
}
