using Domain.Common;
using Microsoft.EntityFrameworkCore.Storage;

namespace Domain.Interfaces;

/// <summary>
/// Unit of Work interface for managing database transactions
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Gets a repository instance for the specified entity type
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    /// <returns>Repository instance for the entity type</returns>
    IRepository<T> Repository<T>() where T : BaseEntity;

    /// <summary>
    /// Saves all changes made in this context to the database
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The number of state entries written to the database</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begins a new database transaction
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A transaction object</returns>
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commits the specified transaction
    /// </summary>
    /// <param name="transaction">The transaction to commit</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task CommitTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken = default);

    /// <summary>
    /// Rolls back the specified transaction
    /// </summary>
    /// <param name="transaction">The transaction to rollback</param>
    Task RollbackTransactionAsync(IDbContextTransaction transaction);
}