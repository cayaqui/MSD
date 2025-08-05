using Application.Interfaces.Organization;
using Application.Services.Base;
using Core.DTOs.Projects.Projects;
using Core.Enums.Projects;
using Domain.Entities.Organization.Core;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services.Organization;

/// <summary>
/// Service implementation for Project Phase management
/// </summary>
public class PhaseService : BaseService<Phase, PhaseDto, CreatePhaseDto, UpdatePhaseDto>, IPhaseService
{
    public PhaseService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<PhaseService> logger)
        : base(unitOfWork, mapper, logger)
    {
    }

    public IUnitOfWork UnitOfWork => _unitOfWork;

    public async Task<IEnumerable<PhaseDto>> GetByProjectAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.Repository<Phase>()
            .GetAllAsync(
                filter: p => p.ProjectId == projectId && !p.IsDeleted,
                orderBy: q => q.OrderBy(p => p.Sequence).ThenBy(p => p.PlannedStartDate),
                cancellationToken: cancellationToken);

        return _mapper.Map<IEnumerable<PhaseDto>>(entities);
    }

    public async Task<IEnumerable<PhaseDto>> GetByTypeAsync(PhaseType type, CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.Repository<Phase>()
            .GetAllAsync(
                filter: p => p.Type == type && !p.IsDeleted,
                orderBy: q => q.OrderBy(p => p.PlannedStartDate),
                cancellationToken: cancellationToken);

        return _mapper.Map<IEnumerable<PhaseDto>>(entities);
    }

    public async Task<IEnumerable<PhaseDto>> GetByStatusAsync(PhaseStatus status, CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.Repository<Phase>()
            .GetAllAsync(
                filter: p => p.Status == status && !p.IsDeleted,
                orderBy: q => q.OrderBy(p => p.PlannedStartDate),
                cancellationToken: cancellationToken);

        return _mapper.Map<IEnumerable<PhaseDto>>(entities);
    }

    public async Task<PhaseDto?> StartPhaseAsync(Guid id, string? userId = null, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<Phase>().GetByIdAsync(id);
        if (entity == null)
            return null;

        if (entity.Status != PhaseStatus.NotStarted)
        {
            throw new InvalidOperationException($"Phase can only be started from NotStarted status. Current status: {entity.Status}");
        }

        entity.Status = PhaseStatus.InProgress;
        entity.ActualStartDate = DateTime.UtcNow;

        if (!string.IsNullOrEmpty(userId))
        {
            entity.UpdatedBy = userId;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        _unitOfWork.Repository<Phase>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Phase {PhaseId} started by {User}", id, userId ?? "System");

        return _mapper.Map<PhaseDto>(entity);
    }

    public async Task<PhaseDto?> CompletePhaseAsync(Guid id, string? userId = null, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<Phase>().GetByIdAsync(id);
        if (entity == null)
            return null;

        if (entity.Status != PhaseStatus.InProgress)
        {
            throw new InvalidOperationException($"Phase can only be completed from InProgress status. Current status: {entity.Status}");
        }

        entity.Status = PhaseStatus.Completed;
        entity.ActualEndDate = DateTime.UtcNow;
        entity.ProgressPercentage = 100;

        if (!string.IsNullOrEmpty(userId))
        {
            entity.UpdatedBy = userId;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        _unitOfWork.Repository<Phase>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Phase {PhaseId} completed by {User}", id, userId ?? "System");

        return _mapper.Map<PhaseDto>(entity);
    }

    public async Task<PhaseDto?> ApproveGateAsync(Guid id, string? approvedBy = null, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<Phase>().GetByIdAsync(id);
        if (entity == null)
            return null;

        if (!entity.RequiresGateApproval)
        {
            throw new InvalidOperationException("This phase does not require gate approval.");
        }

        entity.GateApproved = true;
        entity.GateApprovedBy = approvedBy;
        entity.GateApprovedDate = DateTime.UtcNow;

        if (!string.IsNullOrEmpty(approvedBy))
        {
            entity.UpdatedBy = approvedBy;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        _unitOfWork.Repository<Phase>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Phase {PhaseId} gate approved by {User}", id, approvedBy ?? "System");

        return _mapper.Map<PhaseDto>(entity);
    }

    public async Task<PhaseDto?> UpdateProgressAsync(Guid id, UpdatePhaseProgressDto dto, string? userId = null, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<Phase>().GetByIdAsync(id);
        if (entity == null)
            return null;

        entity.ProgressPercentage = dto.ProgressPercentage;
        entity.CostProgress = dto.CostProgress ?? entity.CostProgress;
        entity.ScheduleProgress = dto.ScheduleProgress ?? entity.ScheduleProgress;
        entity.QualityScore = dto.QualityScore ?? entity.QualityScore;

        if (dto.ActualCost.HasValue)
            entity.ActualCost = dto.ActualCost.Value;

        if (!string.IsNullOrEmpty(userId))
        {
            entity.UpdatedBy = userId;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        _unitOfWork.Repository<Phase>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Phase {PhaseId} progress updated to {Progress}% by {User}", 
            id, dto.ProgressPercentage, userId ?? "System");

        return _mapper.Map<PhaseDto>(entity);
    }

    public async Task<PhaseDetailDto?> GetPhaseDetailAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<Phase>()
            .GetAsync(
                filter: p => p.Id == id && !p.IsDeleted,
                includeProperties: "Project,Deliverables,Milestones",
                cancellationToken: cancellationToken);

        return entity != null ? _mapper.Map<PhaseDetailDto>(entity) : null;
    }

    public async Task<IEnumerable<PhaseDto>> GetPendingGateApprovalsAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.Repository<Phase>()
            .GetAllAsync(
                filter: p => p.ProjectId == projectId && 
                           p.RequiresGateApproval && 
                           !p.GateApproved && 
                           p.Status == PhaseStatus.InProgress &&
                           !p.IsDeleted,
                orderBy: q => q.OrderBy(p => p.Sequence),
                cancellationToken: cancellationToken);

        return _mapper.Map<IEnumerable<PhaseDto>>(entities);
    }

    public async Task<IEnumerable<PhaseDto>> GetOverBudgetPhasesAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.Repository<Phase>()
            .GetAllAsync(
                filter: p => p.ProjectId == projectId && 
                           p.ActualCost > p.PlannedBudget && 
                           !p.IsDeleted,
                orderBy: q => q.OrderByDescending(p => (p.ActualCost - p.PlannedBudget)),
                cancellationToken: cancellationToken);

        return _mapper.Map<IEnumerable<PhaseDto>>(entities);
    }

    public async Task<IEnumerable<PhaseDto>> GetDelayedPhasesAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var currentDate = DateTime.UtcNow;
        
        var entities = await _unitOfWork.Repository<Phase>()
            .GetAllAsync(
                filter: p => p.ProjectId == projectId && 
                           p.Status != PhaseStatus.Completed &&
                           p.PlannedEndDate < currentDate && 
                           !p.IsDeleted,
                orderBy: q => q.OrderBy(p => p.PlannedEndDate),
                cancellationToken: cancellationToken);

        return _mapper.Map<IEnumerable<PhaseDto>>(entities);
    }

    public async Task<IEnumerable<PhaseDto>> UpdateSequenceAsync(Guid projectId, IEnumerable<PhaseSequenceDto> sequences, 
        string? updatedBy = null, CancellationToken cancellationToken = default)
    {
        var phases = await _unitOfWork.Repository<Phase>()
            .GetAllAsync(
                filter: p => p.ProjectId == projectId && !p.IsDeleted,
                cancellationToken: cancellationToken);

        foreach (var sequence in sequences)
        {
            var phase = phases.FirstOrDefault(p => p.Id == sequence.PhaseId);
            if (phase != null)
            {
                phase.Sequence = sequence.Sequence;
                
                if (!string.IsNullOrEmpty(updatedBy))
                {
                    phase.UpdatedBy = updatedBy;
                    phase.UpdatedAt = DateTime.UtcNow;
                }
                
                _unitOfWork.Repository<Phase>().Update(phase);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Phase sequences updated for project {ProjectId} by {User}", 
            projectId, updatedBy ?? "System");

        // Return updated phases ordered by sequence
        var updatedPhases = await GetByProjectAsync(projectId, cancellationToken);
        return updatedPhases;
    }

    public async Task<decimal> CalculateProjectProgressAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var phases = await _unitOfWork.Repository<Phase>()
            .GetAllAsync(
                filter: p => p.ProjectId == projectId && !p.IsDeleted,
                cancellationToken: cancellationToken);

        if (!phases.Any())
            return 0;

        var totalWeight = phases.Sum(p => p.Weight);
        if (totalWeight == 0)
            return phases.Average(p => p.ProgressPercentage);

        var weightedProgress = phases.Sum(p => p.ProgressPercentage * p.Weight) / totalWeight;
        return Math.Round(weightedProgress, 2);
    }

    protected override async Task ValidateEntityAsync(Phase entity, bool isNew)
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

        // Validate weight
        if (entity.Weight < 0)
        {
            throw new InvalidOperationException("Phase weight cannot be negative.");
        }

        // Validate budget
        if (entity.PlannedBudget < 0)
        {
            throw new InvalidOperationException("Planned budget cannot be negative.");
        }

        // Validate actual cost
        if (entity.ActualCost < 0)
        {
            throw new InvalidOperationException("Actual cost cannot be negative.");
        }

        // Validate quality score
        if (entity.QualityScore.HasValue && (entity.QualityScore < 0 || entity.QualityScore > 100))
        {
            throw new InvalidOperationException("Quality score must be between 0 and 100.");
        }

        await base.ValidateEntityAsync(entity, isNew);
    }
}