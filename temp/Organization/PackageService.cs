using Application.Interfaces.Organization;
using Application.Services.Base;
using Core.DTOs.Projects.Projects;
using Core.Enums.Projects;
using Domain.Entities.Organization.Core;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services.Organization;

/// <summary>
/// Service implementation for Package management
/// </summary>
public class PackageService : BaseService<Package, PackageDto, CreatePackageDto, UpdatePackageDto>, IPackageService
{
    public PackageService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<PackageService> logger)
        : base(unitOfWork, mapper, logger)
    {
    }

    public IUnitOfWork UnitOfWork => _unitOfWork;

    public async Task<IEnumerable<PackageDto>> GetByPhaseAsync(Guid phaseId, CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.Repository<Package>()
            .GetAllAsync(
                filter: p => p.PhaseId == phaseId && !p.IsDeleted,
                orderBy: q => q.OrderBy(p => p.Code),
                cancellationToken: cancellationToken);

        return _mapper.Map<IEnumerable<PackageDto>>(entities);
    }

    public async Task<IEnumerable<PackageDto>> GetByWBSElementAsync(Guid wbsElementId, CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.Repository<Package>()
            .GetAllAsync(
                filter: p => p.WBSElementId == wbsElementId && !p.IsDeleted,
                orderBy: q => q.OrderBy(p => p.Code),
                cancellationToken: cancellationToken);

        return _mapper.Map<IEnumerable<PackageDto>>(entities);
    }

    public async Task<IEnumerable<PackageDto>> GetByContractorAsync(Guid contractorId, CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.Repository<Package>()
            .GetAllAsync(
                filter: p => p.ContractorId == contractorId && !p.IsDeleted,
                orderBy: q => q.OrderBy(p => p.Code),
                cancellationToken: cancellationToken);

        return _mapper.Map<IEnumerable<PackageDto>>(entities);
    }

    public async Task<IEnumerable<PackageDto>> GetByTypeAsync(PackageType packageType, CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.Repository<Package>()
            .GetAllAsync(
                filter: p => p.Type == packageType && !p.IsDeleted,
                orderBy: q => q.OrderBy(p => p.Code),
                cancellationToken: cancellationToken);

        return _mapper.Map<IEnumerable<PackageDto>>(entities);
    }

    public async Task<PackageDto?> AssignToPhaseAsync(Guid id, Guid phaseId, string? updatedBy = null, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<Package>().GetByIdAsync(id);
        if (entity == null)
            return null;

        entity.PhaseId = phaseId;

        if (!string.IsNullOrEmpty(updatedBy))
        {
            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        _unitOfWork.Repository<Package>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Package {PackageCode} assigned to phase {PhaseId} by {User}", 
            entity.Code, phaseId, updatedBy ?? "System");

        return _mapper.Map<PackageDto>(entity);
    }

    public async Task<PackageDto?> AssignToWorkPackageAsync(Guid id, Guid wbsElementId, string? updatedBy = null, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<Package>().GetByIdAsync(id);
        if (entity == null)
            return null;

        entity.WBSElementId = wbsElementId;

        if (!string.IsNullOrEmpty(updatedBy))
        {
            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        _unitOfWork.Repository<Package>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Package {PackageCode} assigned to WBS element {WBSElementId} by {User}", 
            entity.Code, wbsElementId, updatedBy ?? "System");

        return _mapper.Map<PackageDto>(entity);
    }

    public async Task<PackageDto?> AssignContractorAsync(Guid id, Guid? contractorId, string? updatedBy = null, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<Package>().GetByIdAsync(id);
        if (entity == null)
            return null;

        entity.ContractorId = contractorId;

        if (!string.IsNullOrEmpty(updatedBy))
        {
            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        _unitOfWork.Repository<Package>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Package {PackageCode} assigned to contractor {ContractorId} by {User}", 
            entity.Code, contractorId?.ToString() ?? "None", updatedBy ?? "System");

        return _mapper.Map<PackageDto>(entity);
    }

    public async Task<PackageDto?> UpdateContractInfoAsync(Guid id, UpdatePackageContractDto dto, string? updatedBy = null, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<Package>().GetByIdAsync(id);
        if (entity == null)
            return null;

        if (!string.IsNullOrEmpty(dto.ContractNumber))
            entity.SetContractInfo(dto.ContractNumber, entity.ContractType);
        
        if (dto.ContractAmount.HasValue)
            entity.UpdateContractValue(dto.ContractAmount.Value);

        if (!string.IsNullOrEmpty(updatedBy))
        {
            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        _unitOfWork.Repository<Package>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Package {PackageCode} contract info updated by {User}", 
            entity.Code, updatedBy ?? "System");

        return _mapper.Map<PackageDto>(entity);
    }

    public async Task<PackageDto?> UpdateProgressAsync(Guid id, decimal progressPercentage, string? updatedBy = null, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<Package>().GetByIdAsync(id);
        if (entity == null)
            return null;

        entity.ProgressPercentage = progressPercentage;
        
        // Auto-complete if 100%
        if (progressPercentage >= 100 && entity.Status == PackageStatus.InProgress)
        {
            entity.Status = PackageStatus.Completed;
            entity.ActualEndDate = DateTime.UtcNow;
        }

        if (!string.IsNullOrEmpty(updatedBy))
        {
            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        _unitOfWork.Repository<Package>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Package {PackageCode} progress updated to {Progress}% by {User}", 
            entity.Code, progressPercentage, updatedBy ?? "System");

        return _mapper.Map<PackageDto>(entity);
    }

    public async Task<PackageDto?> UpdateDatesAsync(Guid id, UpdatePackageDatesDto dto, string? updatedBy = null, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<Package>().GetByIdAsync(id);
        if (entity == null)
            return null;

        if (dto.PlannedStartDate.HasValue)
            entity.PlannedStartDate = dto.PlannedStartDate.Value;
        
        if (dto.PlannedEndDate.HasValue)
            entity.PlannedEndDate = dto.PlannedEndDate.Value;
        
        if (dto.ActualStartDate.HasValue)
            entity.ActualStartDate = dto.ActualStartDate.Value;
        
        if (dto.ActualEndDate.HasValue)
            entity.ActualEndDate = dto.ActualEndDate.Value;

        if (!string.IsNullOrEmpty(updatedBy))
        {
            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        _unitOfWork.Repository<Package>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Package {PackageCode} dates updated by {User}", 
            entity.Code, updatedBy ?? "System");

        return _mapper.Map<PackageDto>(entity);
    }

    public async Task<PackageDto?> StartPackageAsync(Guid id, string? updatedBy = null, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<Package>().GetByIdAsync(id);
        if (entity == null)
            return null;

        if (entity.Status != PackageStatus.NotStarted)
        {
            throw new InvalidOperationException($"Package can only be started from NotStarted status. Current status: {entity.Status}");
        }

        entity.Start();

        if (!string.IsNullOrEmpty(updatedBy))
        {
            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        _unitOfWork.Repository<Package>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Package {PackageCode} started by {User}", entity.Code, updatedBy ?? "System");

        return _mapper.Map<PackageDto>(entity);
    }

    public async Task<PackageDto?> CompletePackageAsync(Guid id, string? updatedBy = null, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<Package>().GetByIdAsync(id);
        if (entity == null)
            return null;

        if (entity.Status != PackageStatus.InProgress)
        {
            throw new InvalidOperationException($"Package can only be completed from InProgress status. Current status: {entity.Status}");
        }

        entity.Status = PackageStatus.Completed;
        entity.ActualEndDate = DateTime.UtcNow;
        entity.ProgressPercentage = 100;

        if (!string.IsNullOrEmpty(updatedBy))
        {
            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        _unitOfWork.Repository<Package>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Package {PackageCode} completed by {User}", entity.Code, updatedBy ?? "System");

        return _mapper.Map<PackageDto>(entity);
    }

    public async Task<PackageDto?> CancelPackageAsync(Guid id, string reason, string? updatedBy = null, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<Package>().GetByIdAsync(id);
        if (entity == null)
            return null;

        entity.Status = PackageStatus.Cancelled;
        entity.CancellationReason = reason;

        if (!string.IsNullOrEmpty(updatedBy))
        {
            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        _unitOfWork.Repository<Package>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Package {PackageCode} cancelled by {User}. Reason: {Reason}", 
            entity.Code, updatedBy ?? "System", reason);

        return _mapper.Map<PackageDto>(entity);
    }

    public async Task<IEnumerable<PackageDto>> GetOverduePackagesAsync(CancellationToken cancellationToken = default)
    {
        var currentDate = DateTime.UtcNow;
        
        var entities = await _unitOfWork.Repository<Package>()
            .GetAllAsync(
                filter: p => p.Status == PackageStatus.InProgress && 
                           p.PlannedEndDate < currentDate && 
                           !p.IsDeleted,
                orderBy: q => q.OrderBy(p => p.PlannedEndDate),
                cancellationToken: cancellationToken);

        return _mapper.Map<IEnumerable<PackageDto>>(entities);
    }

    public async Task<PackageWithDisciplinesDto?> GetWithDisciplinesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<Package>()
            .GetAsync(
                filter: p => p.Id == id && !p.IsDeleted,
                includeProperties: "PackageDisciplines.Discipline",
                cancellationToken: cancellationToken);

        return entity != null ? _mapper.Map<PackageWithDisciplinesDto>(entity) : null;
    }

    public async Task<PackagePerformanceDto?> GetPerformanceMetricsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<Package>()
            .GetAsync(
                filter: p => p.Id == id && !p.IsDeleted,
                cancellationToken: cancellationToken);

        if (entity == null)
            return null;

        var performanceDto = new PackagePerformanceDto
        {
            PackageId = entity.Id,
            PackageCode = entity.Code,
            PackageName = entity.Name,
            ProgressPercentage = entity.ProgressPercentage,
            Status = entity.Status,
            ScheduleVariance = 0,
            CostVariance = 0,
            IsOverdue = false
        };

        // Calculate schedule variance
        if (entity.ActualStartDate.HasValue && entity.PlannedStartDate != default)
        {
            performanceDto.ScheduleVariance = (entity.ActualStartDate.Value - entity.PlannedStartDate).Days;
        }

        // Check if overdue
        if (entity.Status == PackageStatus.InProgress && entity.PlannedEndDate < DateTime.UtcNow)
        {
            performanceDto.IsOverdue = true;
            performanceDto.DaysOverdue = (DateTime.UtcNow - entity.PlannedEndDate).Days;
        }

        // Calculate estimated completion based on progress
        if (entity.ProgressPercentage > 0 && entity.ActualStartDate.HasValue)
        {
            var elapsedDays = (DateTime.UtcNow - entity.ActualStartDate.Value).Days;
            var estimatedTotalDays = elapsedDays / (entity.ProgressPercentage / 100);
            performanceDto.EstimatedCompletionDate = entity.ActualStartDate.Value.AddDays(estimatedTotalDays);
        }

        return performanceDto;
    }

    protected override async Task ValidateEntityAsync(Package entity, bool isNew)
    {
        // Validate dates
        if (entity.PlannedEndDate <= entity.PlannedStartDate)
        {
            throw new InvalidOperationException("Planned end date must be after planned start date.");
        }

        // Validate progress percentage
        if (entity.ProgressPercentage < 0 || entity.ProgressPercentage > 100)
        {
            throw new InvalidOperationException("Progress percentage must be between 0 and 100.");
        }

        // Validate contract value
        if (entity.ContractValue < 0)
        {
            throw new InvalidOperationException("Contract value cannot be negative.");
        }

        await base.ValidateEntityAsync(entity, isNew);
    }
}