using Application.Interfaces.Organization;
using Application.Services.Base;
using Core.DTOs.Projects.Projects;
using Domain.Entities.Organization.Core;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services.Organization;

/// <summary>
/// Service implementation for Package Discipline management
/// </summary>
public class PackageDisciplineService : BaseService<PackageDiscipline, PackageDisciplineDto, CreatePackageDisciplineDto, UpdatePackageDisciplineDto>, IPackageDisciplineService
{
    public PackageDisciplineService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<PackageDisciplineService> logger)
        : base(unitOfWork, mapper, logger)
    {
    }

    public IUnitOfWork UnitOfWork => _unitOfWork;

    public async Task<IEnumerable<PackageDisciplineDto>> GetByPackageAsync(Guid packageId, CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.Repository<PackageDiscipline>()
            .GetAllAsync(
                filter: pd => pd.PackageId == packageId && !pd.IsDeleted,
                includeProperties: "Discipline",
                orderBy: q => q.OrderByDescending(pd => pd.IsLead).ThenBy(pd => pd.Discipline.Name),
                cancellationToken: cancellationToken);

        return _mapper.Map<IEnumerable<PackageDisciplineDto>>(entities);
    }

    public async Task<IEnumerable<PackageDisciplineDto>> GetByDisciplineAsync(Guid disciplineId, CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.Repository<PackageDiscipline>()
            .GetAllAsync(
                filter: pd => pd.DisciplineId == disciplineId && !pd.IsDeleted,
                includeProperties: "Package",
                orderBy: q => q.OrderBy(pd => pd.Package.Code),
                cancellationToken: cancellationToken);

        return _mapper.Map<IEnumerable<PackageDisciplineDto>>(entities);
    }

    public async Task<PackageDisciplineDto?> GetLeadDisciplineAsync(Guid packageId, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<PackageDiscipline>()
            .GetAsync(
                filter: pd => pd.PackageId == packageId && pd.IsLead && !pd.IsDeleted,
                includeProperties: "Discipline",
                cancellationToken: cancellationToken);

        return entity != null ? _mapper.Map<PackageDisciplineDto>(entity) : null;
    }

    public async Task<PackageDisciplineDto?> UpdateEstimateAsync(Guid id, UpdateDisciplineEstimateDto dto, string? updatedBy = null, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<PackageDiscipline>().GetByIdAsync(id);
        if (entity == null)
            return null;

        entity.EstimatedManHours = dto.EstimatedManHours;
        entity.EstimatedCost = dto.EstimatedCost;
        entity.PlannedStartDate = dto.PlannedStartDate ?? entity.PlannedStartDate;
        entity.PlannedEndDate = dto.PlannedEndDate ?? entity.PlannedEndDate;

        if (!string.IsNullOrEmpty(updatedBy))
        {
            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        _unitOfWork.Repository<PackageDiscipline>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Package discipline {Id} estimate updated by {User}", id, updatedBy ?? "System");

        return _mapper.Map<PackageDisciplineDto>(entity);
    }

    public async Task<PackageDisciplineDto?> UpdateActualsAsync(Guid id, UpdateDisciplineActualsDto dto, string? updatedBy = null, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<PackageDiscipline>().GetByIdAsync(id);
        if (entity == null)
            return null;

        entity.ActualManHours = dto.ActualManHours;
        entity.ActualCost = dto.ActualCost;
        entity.ProgressPercentage = dto.ProgressPercentage;
        entity.ActualStartDate = dto.ActualStartDate ?? entity.ActualStartDate;
        entity.ActualEndDate = dto.ActualEndDate ?? entity.ActualEndDate;

        if (!string.IsNullOrEmpty(updatedBy))
        {
            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        _unitOfWork.Repository<PackageDiscipline>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Package discipline {Id} actuals updated by {User}", id, updatedBy ?? "System");

        return _mapper.Map<PackageDisciplineDto>(entity);
    }

    public async Task<PackageDisciplineDto?> AssignLeadEngineerAsync(Guid id, Guid? leadEngineerId, string? updatedBy = null, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<PackageDiscipline>().GetByIdAsync(id);
        if (entity == null)
            return null;

        entity.LeadEngineerId = leadEngineerId;

        if (!string.IsNullOrEmpty(updatedBy))
        {
            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        _unitOfWork.Repository<PackageDiscipline>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Lead engineer {LeadEngineerId} assigned to package discipline {Id} by {User}", 
            leadEngineerId?.ToString() ?? "None", id, updatedBy ?? "System");

        return _mapper.Map<PackageDisciplineDto>(entity);
    }

    public async Task<PackageDisciplineDto?> SetAsLeadDisciplineAsync(Guid id, bool isLead, string? updatedBy = null, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<PackageDiscipline>().GetByIdAsync(id);
        if (entity == null)
            return null;

        if (isLead)
        {
            // Ensure only one lead discipline per package
            var existingLead = await _unitOfWork.Repository<PackageDiscipline>()
                .GetAsync(pd => pd.PackageId == entity.PackageId && pd.IsLead && pd.Id != id && !pd.IsDeleted);
            
            if (existingLead != null)
            {
                existingLead.IsLead = false;
                _unitOfWork.Repository<PackageDiscipline>().Update(existingLead);
            }
        }

        entity.IsLead = isLead;

        if (!string.IsNullOrEmpty(updatedBy))
        {
            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        _unitOfWork.Repository<PackageDiscipline>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Package discipline {Id} set as {Status} by {User}", 
            id, isLead ? "lead" : "non-lead", updatedBy ?? "System");

        return _mapper.Map<PackageDisciplineDto>(entity);
    }

    public async Task<DisciplinePerformanceSummaryDto> GetPerformanceSummaryAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<PackageDiscipline>()
            .GetAsync(
                filter: pd => pd.Id == id && !pd.IsDeleted,
                includeProperties: "Discipline,Package",
                cancellationToken: cancellationToken);

        if (entity == null)
            throw new InvalidOperationException($"Package discipline with ID {id} not found.");

        var summary = new DisciplinePerformanceSummaryDto
        {
            Id = entity.Id,
            PackageCode = entity.Package?.Code ?? string.Empty,
            PackageName = entity.Package?.Name ?? string.Empty,
            DisciplineCode = entity.Discipline?.Code ?? string.Empty,
            DisciplineName = entity.Discipline?.Name ?? string.Empty,
            IsLead = entity.IsLead,
            ProgressPercentage = entity.ProgressPercentage,
            EstimatedManHours = entity.EstimatedManHours,
            ActualManHours = entity.ActualManHours,
            EstimatedCost = entity.EstimatedCost,
            ActualCost = entity.ActualCost,
            ManHoursVariance = entity.EstimatedManHours - entity.ActualManHours,
            CostVariance = entity.EstimatedCost - entity.ActualCost,
            ManHoursVariancePercentage = entity.EstimatedManHours > 0 ? 
                ((entity.EstimatedManHours - entity.ActualManHours) / entity.EstimatedManHours) * 100 : 0,
            CostVariancePercentage = entity.EstimatedCost > 0 ? 
                ((entity.EstimatedCost - entity.ActualCost) / entity.EstimatedCost) * 100 : 0
        };

        // Calculate schedule variance
        if (entity.ActualStartDate.HasValue && entity.PlannedStartDate != default)
        {
            summary.ScheduleVarianceDays = (entity.ActualStartDate.Value - entity.PlannedStartDate).Days;
        }

        // Calculate efficiency
        if (entity.ActualManHours > 0 && entity.EstimatedManHours > 0)
        {
            summary.Efficiency = (entity.EstimatedManHours / entity.ActualManHours) * 100;
        }

        return summary;
    }

    public async Task<IEnumerable<PackageDisciplineDto>> GetByLeadEngineerAsync(Guid leadEngineerId, CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.Repository<PackageDiscipline>()
            .GetAllAsync(
                filter: pd => pd.LeadEngineerId == leadEngineerId && !pd.IsDeleted,
                includeProperties: "Package,Discipline",
                orderBy: q => q.OrderBy(pd => pd.Package.Code),
                cancellationToken: cancellationToken);

        return _mapper.Map<IEnumerable<PackageDisciplineDto>>(entities);
    }

    public async Task<IEnumerable<PackageDisciplineDto>> BulkCreateAsync(Guid packageId, IEnumerable<CreatePackageDisciplineDto> disciplines, 
        string? createdBy = null, CancellationToken cancellationToken = default)
    {
        var createdEntities = new List<PackageDiscipline>();
        
        foreach (var dto in disciplines)
        {
            var entity = _mapper.Map<PackageDiscipline>(dto);
            entity.PackageId = packageId;

            if (!string.IsNullOrEmpty(createdBy))
            {
                entity.CreatedBy = createdBy;
                entity.CreatedAt = DateTime.UtcNow;
            }

            await _unitOfWork.Repository<PackageDiscipline>().AddAsync(entity);
            createdEntities.Add(entity);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("{Count} package disciplines created for package {PackageId} by {User}", 
            createdEntities.Count, packageId, createdBy ?? "System");

        return _mapper.Map<IEnumerable<PackageDisciplineDto>>(createdEntities);
    }

    public async Task<PackageDisciplineAllocationDto> GetAllocationSummaryAsync(Guid packageId, CancellationToken cancellationToken = default)
    {
        var disciplines = await _unitOfWork.Repository<PackageDiscipline>()
            .GetAllAsync(
                filter: pd => pd.PackageId == packageId && !pd.IsDeleted,
                includeProperties: "Discipline",
                cancellationToken: cancellationToken);

        var allocation = new PackageDisciplineAllocationDto
        {
            PackageId = packageId,
            TotalDisciplines = disciplines.Count(),
            TotalEstimatedManHours = disciplines.Sum(d => d.EstimatedManHours),
            TotalActualManHours = disciplines.Sum(d => d.ActualManHours),
            TotalEstimatedCost = disciplines.Sum(d => d.EstimatedCost),
            TotalActualCost = disciplines.Sum(d => d.ActualCost),
            AverageProgress = disciplines.Any() ? disciplines.Average(d => d.ProgressPercentage) : 0,
            DisciplineBreakdown = disciplines.Select(d => new DisciplineAllocationBreakdownDto
            {
                DisciplineId = d.DisciplineId,
                DisciplineCode = d.Discipline?.Code ?? string.Empty,
                DisciplineName = d.Discipline?.Name ?? string.Empty,
                IsLead = d.IsLead,
                EstimatedManHours = d.EstimatedManHours,
                ActualManHours = d.ActualManHours,
                EstimatedCost = d.EstimatedCost,
                ActualCost = d.ActualCost,
                ProgressPercentage = d.ProgressPercentage,
                ManHoursPercentage = disciplines.Sum(x => x.EstimatedManHours) > 0 ? 
                    (d.EstimatedManHours / disciplines.Sum(x => x.EstimatedManHours)) * 100 : 0,
                CostPercentage = disciplines.Sum(x => x.EstimatedCost) > 0 ? 
                    (d.EstimatedCost / disciplines.Sum(x => x.EstimatedCost)) * 100 : 0
            }).ToList()
        };

        return allocation;
    }

    public async Task<bool> ValidateLeadDisciplineAsync(Guid packageId, Guid? excludeDisciplineId = null, CancellationToken cancellationToken = default)
    {
        var leadCount = await _unitOfWork.Repository<PackageDiscipline>()
            .CountAsync(pd => pd.PackageId == packageId && 
                             pd.IsLead && 
                             (!excludeDisciplineId.HasValue || pd.Id != excludeDisciplineId) &&
                             !pd.IsDeleted,
                       cancellationToken);

        return leadCount <= 1;
    }

    protected override async Task ValidateEntityAsync(PackageDiscipline entity, bool isNew)
    {
        // Validate estimated values
        if (entity.EstimatedManHours < 0)
        {
            throw new InvalidOperationException("Estimated man hours cannot be negative.");
        }

        if (entity.EstimatedCost < 0)
        {
            throw new InvalidOperationException("Estimated cost cannot be negative.");
        }

        // Validate actual values
        if (entity.ActualManHours < 0)
        {
            throw new InvalidOperationException("Actual man hours cannot be negative.");
        }

        if (entity.ActualCost < 0)
        {
            throw new InvalidOperationException("Actual cost cannot be negative.");
        }

        // Validate progress percentage
        if (entity.ProgressPercentage < 0 || entity.ProgressPercentage > 100)
        {
            throw new InvalidOperationException("Progress percentage must be between 0 and 100.");
        }

        // Validate dates
        if (entity.PlannedEndDate <= entity.PlannedStartDate)
        {
            throw new InvalidOperationException("Planned end date must be after planned start date.");
        }

        // Validate lead discipline uniqueness
        if (entity.IsLead)
        {
            var isValid = await ValidateLeadDisciplineAsync(entity.PackageId, isNew ? null : entity.Id);
            if (!isValid)
            {
                throw new InvalidOperationException("A package can only have one lead discipline.");
            }
        }

        await base.ValidateEntityAsync(entity, isNew);
    }
}