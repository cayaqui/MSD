using Application.Interfaces.Organization;
using Application.Services.Base;
using Core.DTOs.Organization.Discipline;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services.Organization;

/// <summary>
/// Service implementation for Discipline management
/// </summary>
public class DisciplineService : BaseService<Discipline, DisciplineDto, CreateDisciplineDto, UpdateDisciplineDto>, IDisciplineService
{
    public DisciplineService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<DisciplineService> logger)
        : base(unitOfWork, mapper, logger)
    {
    }

    public IUnitOfWork UnitOfWork => _unitOfWork;

    public async Task<DisciplineDto?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<Discipline>()
            .GetAsync(
                filter: d => d.Code == code && !d.IsDeleted,
                cancellationToken: cancellationToken);

        return entity != null ? _mapper.Map<DisciplineDto>(entity) : null;
    }

    public async Task<IEnumerable<DisciplineDto>> GetEngineeringDisciplinesAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.Repository<Discipline>()
            .GetAllAsync(
                filter: d => d.IsEngineering && !d.IsDeleted,
                orderBy: q => q.OrderBy(d => d.Order).ThenBy(d => d.Name),
                cancellationToken: cancellationToken);

        return _mapper.Map<IEnumerable<DisciplineDto>>(entities);
    }

    public async Task<IEnumerable<DisciplineDto>> GetManagementDisciplinesAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.Repository<Discipline>()
            .GetAllAsync(
                filter: d => d.IsManagement && !d.IsDeleted,
                orderBy: q => q.OrderBy(d => d.Order).ThenBy(d => d.Name),
                cancellationToken: cancellationToken);

        return _mapper.Map<IEnumerable<DisciplineDto>>(entities);
    }

    public async Task<DisciplineDto?> UpdateBasicInfoAsync(Guid id, string name, string? description, int order, string? updatedBy = null, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<Discipline>().GetByIdAsync(id);
        if (entity == null)
            return null;

        entity.Name = name;
        entity.Description = description ?? string.Empty;
        entity.Order = order;

        if (!string.IsNullOrEmpty(updatedBy))
        {
            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        _unitOfWork.Repository<Discipline>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Discipline {DisciplineCode} basic info updated by {User}", entity.Code, updatedBy ?? "System");

        return _mapper.Map<DisciplineDto>(entity);
    }

    public async Task<DisciplineDto?> UpdateVisualAsync(Guid id, string colorHex, string? icon, string? updatedBy = null, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<Discipline>().GetByIdAsync(id);
        if (entity == null)
            return null;

        entity.ColorHex = colorHex;
        entity.Icon = icon;

        if (!string.IsNullOrEmpty(updatedBy))
        {
            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        _unitOfWork.Repository<Discipline>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Discipline {DisciplineCode} visual settings updated by {User}", entity.Code, updatedBy ?? "System");

        return _mapper.Map<DisciplineDto>(entity);
    }

    public async Task<DisciplineDto?> UpdateCategoryAsync(Guid id, bool isEngineering, bool isManagement, string? updatedBy = null, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<Discipline>().GetByIdAsync(id);
        if (entity == null)
            return null;

        entity.IsEngineering = isEngineering;
        entity.IsManagement = isManagement;

        if (!string.IsNullOrEmpty(updatedBy))
        {
            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        _unitOfWork.Repository<Discipline>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Discipline {DisciplineCode} category updated by {User}", entity.Code, updatedBy ?? "System");

        return _mapper.Map<DisciplineDto>(entity);
    }

    public async Task<IEnumerable<DisciplineDto>> GetOrderedAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.Repository<Discipline>()
            .GetAllAsync(
                filter: d => !d.IsDeleted,
                orderBy: q => q.OrderBy(d => d.Order).ThenBy(d => d.Name),
                cancellationToken: cancellationToken);

        return _mapper.Map<IEnumerable<DisciplineDto>>(entities);
    }

    public async Task<bool> IsCodeUniqueAsync(string code, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var exists = await _unitOfWork.Repository<Discipline>()
            .AnyAsync(d => d.Code == code && 
                          (!excludeId.HasValue || d.Id != excludeId.Value) && 
                          !d.IsDeleted,
                      cancellationToken);

        return !exists;
    }

    protected override async Task ValidateEntityAsync(Discipline entity, bool isNew)
    {
        // Validate unique code
        if (isNew || entity.Code != null)
        {
            var isUnique = await IsCodeUniqueAsync(entity.Code, isNew ? null : entity.Id);
            if (!isUnique)
            {
                throw new InvalidOperationException($"Discipline code '{entity.Code}' already exists.");
            }
        }

        // Validate color hex format
        if (!string.IsNullOrEmpty(entity.ColorHex))
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(entity.ColorHex, "^#[0-9A-Fa-f]{6}$"))
            {
                throw new InvalidOperationException("Color must be in hex format (e.g., #FF0000).");
            }
        }

        // Validate order
        if (entity.Order < 0)
        {
            throw new InvalidOperationException("Order must be a non-negative number.");
        }

        // Validate category
        if (!entity.IsEngineering && !entity.IsManagement)
        {
            throw new InvalidOperationException("Discipline must be either Engineering or Management (or both).");
        }

        await base.ValidateEntityAsync(entity, isNew);
    }
}