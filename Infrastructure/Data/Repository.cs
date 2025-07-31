using Microsoft.Extensions.Logging;

namespace Infrastructure.Data;

/// <summary>
/// Generic repository implementation for Entity Framework Core
/// </summary>
/// <typeparam name="T">Entity type that inherits from BaseEntity</typeparam>
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<T> _dbSet;
    private readonly ILogger<Repository<T>> _logger;

    public Repository(ApplicationDbContext context, ILogger<Repository<T>> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dbSet = _context.Set<T>();
    }

    #region Read Operations

    /// <inheritdoc/>
    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting entity {EntityType} with id {Id}", typeof(T).Name, id);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbSet.ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all entities of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<T>> GetAllAsync(
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        string includeProperties = "",
        bool tracked = false)
    {
        try
        {
            IQueryable<T> query = tracked ? _dbSet : _dbSet.AsNoTracking();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            // Include related entities
            query = IncludeProperties(query, includeProperties);

            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }

            return await query.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetAllAsync for {EntityType}", typeof(T).Name);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<T?> GetAsync(
        Expression<Func<T, bool>> filter,
        string includeProperties = "",
        bool tracked = false,
        CancellationToken cancellationToken = default)
    {
        try
        {
            IQueryable<T> query = tracked ? _dbSet : _dbSet.AsNoTracking();

            query = query.Where(filter);
            query = IncludeProperties(query, includeProperties);

            return await query.FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetAsync for {EntityType}", typeof(T).Name);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        string includeProperties = "")
    {
        try
        {
            IQueryable<T> query = _dbSet.AsNoTracking();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            query = IncludeProperties(query, includeProperties);

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            // Apply pagination
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetPagedAsync for {EntityType}", typeof(T).Name);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbSet
                .AsNoTracking()
                .Where(predicate)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in FindAsync for {EntityType}", typeof(T).Name);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<T?> FindSingleAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(predicate, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in FindSingleAsync for {EntityType}", typeof(T).Name);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> AnyAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbSet.AnyAsync(predicate, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in AnyAsync for {EntityType}", typeof(T).Name);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<int> CountAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbSet.CountAsync(predicate, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in CountAsync for {EntityType}", typeof(T).Name);
            throw;
        }
    }

    #endregion

    #region Write Operations

    /// <inheritdoc/>
    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(entity);

            await _dbSet.AddAsync(entity, cancellationToken);
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding entity of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(entities);

            await _dbSet.AddRangeAsync(entities, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding range of entities of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    /// <inheritdoc/>
    public void Update(T entity)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(entity);

            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating entity of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    /// <inheritdoc/>
    public void UpdateRange(IEnumerable<T> entities)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(entities);

            _dbSet.UpdateRange(entities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating range of entities of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    /// <inheritdoc/>
    public void Remove(T entity)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(entity);

            if (_context.Entry(entity).State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }

            _dbSet.Remove(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing entity of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    /// <inheritdoc/>
    public void RemoveRange(IEnumerable<T> entities)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(entities);

            _dbSet.RemoveRange(entities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing range of entities of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    #endregion

    #region Query Methods

    /// <inheritdoc/>
    public IQueryable<T> Query()
    {
        return _dbSet;
    }

    /// <inheritdoc/>
    public IQueryable<T> QueryNoTracking()
    {
        return _dbSet.AsNoTracking();
    }

    /// <inheritdoc/>
    public IQueryable<T> Query(bool tracked = false)
    {
        return tracked ? _dbSet : _dbSet.AsNoTracking();
    }

    #endregion

    #region Private Helper Methods

    /// <summary>
    /// Includes navigation properties in the query
    /// </summary>
    /// <param name="query">The query to add includes to</param>
    /// <param name="includeProperties">Comma-separated list of navigation properties to include</param>
    /// <returns>Query with includes</returns>
    private static IQueryable<T> IncludeProperties(IQueryable<T> query, string includeProperties)
    {
        if (string.IsNullOrWhiteSpace(includeProperties))
            return query;

        foreach (var includeProperty in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        {
            query = query.Include(includeProperty.Trim());
        }

        return query;
    }

    #endregion
}