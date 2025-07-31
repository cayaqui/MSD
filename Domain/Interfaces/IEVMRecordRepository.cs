using Domain.Entities.EVM;

namespace Domain.Interfaces;

/// <summary>
/// Specific repository for EVM Record entities
/// </summary>
public interface IEVMRecordRepository : IRepository<EVMRecord>
{
    Task<EVMRecord?> GetLatestByControlAccountAsync(Guid controlAccountId, CancellationToken cancellationToken = default);
    Task<IEnumerable<EVMRecord>> GetByControlAccountAsync(Guid controlAccountId, CancellationToken cancellationToken = default);
    Task<IEnumerable<EVMRecord>> GetByDateRangeAsync(Guid controlAccountId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<EVMRecord>> GetProjectEVMAsync(Guid projectId, DateTime dataDate, CancellationToken cancellationToken = default);
    Task<bool> ExistsForPeriodAsync(Guid controlAccountId, DateTime dataDate, CancellationToken cancellationToken = default);
}
