namespace Domain.Interfaces;

/// <summary>
/// Specific repository for Control Account entities
/// </summary>
public interface IControlAccountRepository : IRepository<ControlAccount>
{
    Task<ControlAccount?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ControlAccount>> GetByProjectAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ControlAccount>> GetByPhaseAsync(Guid phaseId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ControlAccount>> GetByCAMAsync(string userId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalBudgetByProjectAsync(Guid projectId, CancellationToken cancellationToken = default);
}
