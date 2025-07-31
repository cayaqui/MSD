using System.Collections;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Data;

/// <summary>
/// Unit of Work implementation for managing database transactions
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly ILoggerFactory _loggerFactory;
    private readonly Hashtable _repositories;
    private bool _disposed;

    public UnitOfWork(ApplicationDbContext context, ILoggerFactory loggerFactory)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        _repositories = new Hashtable();
    }

    /// <inheritdoc/>
    public IRepository<T> Repository<T>() where T : BaseEntity
    {
        var type = typeof(T).Name;

        if (!_repositories.ContainsKey(type))
        {
            var repositoryType = typeof(Repository<>);
            var repositoryInstance = Activator.CreateInstance(
                repositoryType.MakeGenericType(typeof(T)),
                _context,
                _loggerFactory.CreateLogger<Repository<T>>()
            );

            _repositories.Add(type, repositoryInstance);
        }

        return (IRepository<T>)_repositories[type]!;
    }

    /// <inheritdoc/>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            // Handle concurrency conflicts
            throw new InvalidOperationException("A concurrency conflict occurred while saving changes.", ex);
        }
        catch (DbUpdateException ex)
        {
            // Handle database update exceptions
            throw new InvalidOperationException("An error occurred while saving changes to the database.", ex);
        }
    }

    /// <inheritdoc/>
    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task CommitTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken = default)
    {
        try
        {
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await RollbackTransactionAsync(transaction);
            throw;
        }
        finally
        {
            await transaction.DisposeAsync();
        }
    }

    /// <inheritdoc/>
    public async Task RollbackTransactionAsync(IDbContextTransaction transaction)
    {
        try
        {
            await transaction.RollbackAsync();
        }
        finally
        {
            await transaction.DisposeAsync();
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
        _disposed = true;
    }
}