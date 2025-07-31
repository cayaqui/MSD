namespace Domain.Interfaces;

/// <summary>
/// Specific repository for Planning Package entities
/// </summary>
public interface IPlanningPackageRepository : IRepository<PlanningPackage>
{
    Task<IEnumerable<PlanningPackage>> GetByControlAccountAsync(Guid controlAccountId, CancellationToken cancellationToken = default);
    Task<IEnumerable<PlanningPackage>> GetUnconvertedByControlAccountAsync(Guid controlAccountId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);
}
