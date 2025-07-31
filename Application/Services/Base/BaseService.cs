using Application.Interfaces.Common;
using Microsoft.Extensions.Logging;

namespace Application.Services.Base;

/// <summary>
/// Base service implementation for common CRUD operations
/// </summary>
public abstract class BaseService<TEntity, TDto, TCreateDto, TUpdateDto> : IBaseService<TDto, TCreateDto, TUpdateDto> where TEntity : BaseEntity
    where TDto : class
    where TCreateDto : class
    where TUpdateDto : class
{
    protected readonly IUnitOfWork _unitOfWork;
    protected readonly IMapper _mapper;
    protected readonly ILogger _logger;

    protected BaseService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    #region Read Operations

    public virtual async Task<TDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<TEntity>().GetByIdAsync(id);
        return entity != null ? _mapper.Map<TDto>(entity) : null;
    }

    public virtual async Task<IEnumerable<TDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.Repository<TEntity>()
            .GetAllAsync(filter: GetBaseFilter());
        return _mapper.Map<IEnumerable<TDto>>(entities);
    }

    public virtual async Task<PagedResult<TDto>> GetAllPagedAsync(
        int pageNumber = 1,
        int pageSize = 20,
        string? orderBy = null,
        bool descending = false,
        CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _unitOfWork.Repository<TEntity>()
            .GetPagedAsync(
                pageNumber,
                pageSize,
                filter: GetBaseFilter(),
                orderBy: GetOrderByExpression(orderBy, descending));

        var dtos = _mapper.Map<IEnumerable<TDto>>(items);

        return new PagedResult<TDto>(dtos.ToList(), pageNumber, pageSize);
    }

    public virtual async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Repository<TEntity>().AnyAsync(e => e.Id == id);
    }

    #endregion

    #region Write Operations

    public virtual async Task<TDto> CreateAsync(
        TCreateDto createDto,
        string? createdBy = null,
        CancellationToken cancellationToken = default)
    {
        var entity = _mapper.Map<TEntity>(createDto);

        // Set audit fields if entity is auditable
        if (entity is IAuditable auditable && !string.IsNullOrEmpty(createdBy))
        {
            auditable.CreatedBy = createdBy;
            auditable.CreatedAt = DateTime.UtcNow;
        }

        // Validate entity before saving
        await ValidateEntityAsync(entity, true);

        await _unitOfWork.Repository<TEntity>().AddAsync(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("{EntityType} with ID {EntityId} created by {User}",
            typeof(TEntity).Name, entity.Id, createdBy ?? "System");

        return _mapper.Map<TDto>(entity);
    }

    public virtual async Task<TDto?> UpdateAsync(
        Guid id,
        TUpdateDto updateDto,
        string? updatedBy = null,
        CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<TEntity>().GetByIdAsync(id);
        if (entity == null)
            return null;

        // Map updates to entity
        _mapper.Map(updateDto, entity);

        // Set audit fields if entity is auditable
        if (entity is IAuditable auditable && !string.IsNullOrEmpty(updatedBy))
        {
            auditable.UpdatedBy = updatedBy;
            auditable.UpdatedAt = DateTime.UtcNow;
        }

        // Validate entity before saving
        await ValidateEntityAsync(entity, false);

        _unitOfWork.Repository<TEntity>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("{EntityType} with ID {EntityId} updated by {User}",
            typeof(TEntity).Name, entity.Id, updatedBy ?? "System");

        return _mapper.Map<TDto>(entity);
    }

    public virtual async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<TEntity>().GetByIdAsync(id);
        if (entity == null)
            return false;

        // Check if entity supports soft delete
        if (entity is ISoftDelete softDelete)
        {
            softDelete.IsDeleted = true;
            softDelete.DeletedAt = DateTime.UtcNow;
            _unitOfWork.Repository<TEntity>().Update(entity);
        }
        else
        {
            _unitOfWork.Repository<TEntity>().Remove(entity);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("{EntityType} with ID {EntityId} deleted",
            typeof(TEntity).Name, entity.Id);

        return true;
    }

    #endregion

    #region Protected Methods for Override

    /// <summary>
    /// Gets the base filter for queries (e.g., exclude soft deleted)
    /// </summary>
    protected virtual Expression<Func<TEntity, bool>>? GetBaseFilter()
    {
        // If entity supports soft delete, exclude deleted items by default
        if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
        {
            return e => !((ISoftDelete)e).IsDeleted;
        }
        return null;
    }

    /// <summary>
    /// Gets the order by expression based on field name
    /// </summary>
    protected virtual Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? GetOrderByExpression(
        string? orderBy,
        bool descending)
    {
        if (string.IsNullOrEmpty(orderBy))
        {
            // Default ordering by CreatedAt if auditable
            if (typeof(IAuditable).IsAssignableFrom(typeof(TEntity)))
            {
                return descending
                    ? q => q.OrderByDescending(e => ((IAuditable)e).CreatedAt)
                    : q => q.OrderBy(e => ((IAuditable)e).CreatedAt);
            }
            return null;
        }

        // TODO: Implement dynamic ordering by property name
        // This would require reflection or expression building
        return null;
    }

    /// <summary>
    /// Validates entity before saving (override in derived classes)
    /// </summary>
    protected virtual Task ValidateEntityAsync(TEntity entity, bool isNew)
    {
        return Task.CompletedTask;
    }

    #endregion
}