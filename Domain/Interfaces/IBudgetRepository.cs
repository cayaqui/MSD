namespace Domain.Interfaces;

/// <summary>
/// Specific repository for Budget entities
/// </summary>
public interface IBudgetRepository : IRepository<Budget>
{
    Task<Budget?> GetWithItemsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Budget?> GetCurrentBaselineAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Budget>> GetByProjectAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByVersionAsync(Guid projectId, string version, CancellationToken cancellationToken = default);
    Task<int> GetNextRevisionNumberAsync(Guid budgetId, CancellationToken cancellationToken = default);
}
