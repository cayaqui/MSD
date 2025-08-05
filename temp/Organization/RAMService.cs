using Application.Interfaces.Organization;
using Application.Services.Base;
using Core.DTOs.Organization.RAM;
using Domain.Entities.Organization.Core;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services.Organization;

/// <summary>
/// Service implementation for Responsibility Assignment Matrix (RAM/RACI) management
/// </summary>
public class RAMService : BaseService<RAM, RAMDto, CreateRAMDto, UpdateRAMDto>, IRAMService
{
    public RAMService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<RAMService> logger)
        : base(unitOfWork, mapper, logger)
    {
    }

    public IUnitOfWork UnitOfWork => _unitOfWork;

    public async Task<IEnumerable<RAMDto>> GetByProjectAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.Repository<RAM>()
            .GetAllAsync(
                filter: r => r.ProjectId == projectId && !r.IsDeleted,
                includeProperties: "WBSElement,OBSNode,ControlAccount",
                orderBy: q => q.OrderBy(r => r.WBSElement.Code).ThenBy(r => r.OBSNode.Code),
                cancellationToken: cancellationToken);

        return _mapper.Map<IEnumerable<RAMDto>>(entities);
    }

    public async Task<IEnumerable<RAMDto>> GetByWBSElementAsync(Guid wbsElementId, CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.Repository<RAM>()
            .GetAllAsync(
                filter: r => r.WBSElementId == wbsElementId && !r.IsDeleted,
                includeProperties: "OBSNode,ControlAccount",
                orderBy: q => q.OrderBy(r => r.ResponsibilityType).ThenBy(r => r.OBSNode.Code),
                cancellationToken: cancellationToken);

        return _mapper.Map<IEnumerable<RAMDto>>(entities);
    }

    public async Task<IEnumerable<RAMDto>> GetByOBSNodeAsync(Guid obsNodeId, CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.Repository<RAM>()
            .GetAllAsync(
                filter: r => r.OBSNodeId == obsNodeId && !r.IsDeleted,
                includeProperties: "WBSElement,ControlAccount",
                orderBy: q => q.OrderBy(r => r.WBSElement.Code),
                cancellationToken: cancellationToken);

        return _mapper.Map<IEnumerable<RAMDto>>(entities);
    }

    public async Task<IEnumerable<RAMDto>> GetByResponsibilityTypeAsync(Guid projectId, string responsibilityType, CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.Repository<RAM>()
            .GetAllAsync(
                filter: r => r.ProjectId == projectId && 
                           r.ResponsibilityType == responsibilityType && 
                           !r.IsDeleted,
                includeProperties: "WBSElement,OBSNode",
                orderBy: q => q.OrderBy(r => r.WBSElement.Code),
                cancellationToken: cancellationToken);

        return _mapper.Map<IEnumerable<RAMDto>>(entities);
    }

    public async Task<RAMDto?> UpdateAllocationAsync(Guid id, UpdateRAMAllocationDto dto, string? updatedBy = null, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<RAM>().GetByIdAsync(id);
        if (entity == null)
            return null;

        entity.AllocationPercentage = dto.AllocationPercentage;
        entity.PlannedManHours = dto.PlannedManHours ?? entity.PlannedManHours;
        entity.PlannedCost = dto.PlannedCost ?? entity.PlannedCost;

        if (!string.IsNullOrEmpty(updatedBy))
        {
            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        _unitOfWork.Repository<RAM>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("RAM {Id} allocation updated to {Allocation}% by {User}", 
            id, dto.AllocationPercentage, updatedBy ?? "System");

        return _mapper.Map<RAMDto>(entity);
    }

    public async Task<RAMDto?> SetPeriodAsync(Guid id, DateTime startDate, DateTime endDate, string? updatedBy = null, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<RAM>().GetByIdAsync(id);
        if (entity == null)
            return null;

        entity.StartDate = startDate;
        entity.EndDate = endDate;

        if (!string.IsNullOrEmpty(updatedBy))
        {
            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        _unitOfWork.Repository<RAM>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("RAM {Id} period set from {StartDate} to {EndDate} by {User}", 
            id, startDate, endDate, updatedBy ?? "System");

        return _mapper.Map<RAMDto>(entity);
    }

    public async Task<RAMDto?> LinkToControlAccountAsync(Guid id, Guid controlAccountId, string? updatedBy = null, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<RAM>().GetByIdAsync(id);
        if (entity == null)
            return null;

        entity.ControlAccountId = controlAccountId;

        if (!string.IsNullOrEmpty(updatedBy))
        {
            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        _unitOfWork.Repository<RAM>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("RAM {Id} linked to control account {ControlAccountId} by {User}", 
            id, controlAccountId, updatedBy ?? "System");

        return _mapper.Map<RAMDto>(entity);
    }

    public async Task<RAMMatrixDto> GetRAMMatrixAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var assignments = await _unitOfWork.Repository<RAM>()
            .GetAllAsync(
                filter: r => r.ProjectId == projectId && !r.IsDeleted,
                includeProperties: "WBSElement,OBSNode",
                cancellationToken: cancellationToken);

        var wbsElements = assignments.Select(a => a.WBSElement).Distinct().OrderBy(w => w.Code);
        var obsNodes = assignments.Select(a => a.OBSNode).Distinct().OrderBy(o => o.Code);

        var matrix = new RAMMatrixDto
        {
            ProjectId = projectId,
            WBSElements = wbsElements.Select(w => new RAMMatrixWBSDto
            {
                Id = w.Id,
                Code = w.Code,
                Name = w.Name
            }).ToList(),
            OBSNodes = obsNodes.Select(o => new RAMMatrixOBSDto
            {
                Id = o.Id,
                Code = o.Code,
                Name = o.Name
            }).ToList(),
            Assignments = new List<RAMMatrixAssignmentDto>()
        };

        foreach (var assignment in assignments)
        {
            matrix.Assignments.Add(new RAMMatrixAssignmentDto
            {
                WBSElementId = assignment.WBSElementId,
                OBSNodeId = assignment.OBSNodeId,
                ResponsibilityType = assignment.ResponsibilityType,
                AllocationPercentage = assignment.AllocationPercentage
            });
        }

        return matrix;
    }

    public async Task<IEnumerable<RAMDto>> GetAccountableAssignmentsAsync(Guid wbsElementId, CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.Repository<RAM>()
            .GetAllAsync(
                filter: r => r.WBSElementId == wbsElementId && 
                           r.ResponsibilityType == "A" && // Accountable
                           !r.IsDeleted,
                includeProperties: "OBSNode",
                cancellationToken: cancellationToken);

        return _mapper.Map<IEnumerable<RAMDto>>(entities);
    }

    public async Task<RAMValidationResult> ValidateAssignmentsAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var result = new RAMValidationResult
        {
            IsValid = true,
            Errors = new List<string>()
        };

        var assignments = await _unitOfWork.Repository<RAM>()
            .GetAllAsync(
                filter: r => r.ProjectId == projectId && !r.IsDeleted,
                includeProperties: "WBSElement,OBSNode",
                cancellationToken: cancellationToken);

        // Group by WBS element
        var wbsGroups = assignments.GroupBy(a => a.WBSElementId);

        foreach (var group in wbsGroups)
        {
            var wbsElement = group.First().WBSElement;
            
            // Check for single Accountable
            var accountableCount = group.Count(a => a.ResponsibilityType == "A");
            if (accountableCount == 0)
            {
                result.IsValid = false;
                result.Errors.Add($"WBS element {wbsElement.Code} has no Accountable assignment.");
            }
            else if (accountableCount > 1)
            {
                result.IsValid = false;
                result.Errors.Add($"WBS element {wbsElement.Code} has {accountableCount} Accountable assignments. Only one is allowed.");
            }

            // Check allocation totals don't exceed 100%
            var totalAllocation = group.Sum(a => a.AllocationPercentage);
            if (totalAllocation > 100)
            {
                result.IsValid = false;
                result.Errors.Add($"WBS element {wbsElement.Code} has total allocation of {totalAllocation}%, exceeding 100%.");
            }
        }

        return result;
    }

    public async Task<IEnumerable<RAMDto>> BulkCreateAsync(IEnumerable<CreateRAMDto> assignments, string? createdBy = null, CancellationToken cancellationToken = default)
    {
        var createdEntities = new List<RAM>();

        foreach (var dto in assignments)
        {
            var entity = _mapper.Map<RAM>(dto);

            if (!string.IsNullOrEmpty(createdBy))
            {
                entity.CreatedBy = createdBy;
                entity.CreatedAt = DateTime.UtcNow;
            }

            await _unitOfWork.Repository<RAM>().AddAsync(entity);
            createdEntities.Add(entity);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("{Count} RAM assignments created by {User}", 
            createdEntities.Count, createdBy ?? "System");

        return _mapper.Map<IEnumerable<RAMDto>>(createdEntities);
    }

    public async Task<IEnumerable<RAMDto>> CopyFromTemplateAsync(Guid sourceProjectId, Guid targetProjectId, string? createdBy = null, CancellationToken cancellationToken = default)
    {
        var sourceAssignments = await _unitOfWork.Repository<RAM>()
            .GetAllAsync(
                filter: r => r.ProjectId == sourceProjectId && !r.IsDeleted,
                cancellationToken: cancellationToken);

        var copiedEntities = new List<RAM>();

        foreach (var source in sourceAssignments)
        {
            var copy = new RAM
            {
                ProjectId = targetProjectId,
                WBSElementId = source.WBSElementId, // This would need mapping to new project's WBS
                OBSNodeId = source.OBSNodeId, // This would need mapping to new project's OBS
                ResponsibilityType = source.ResponsibilityType,
                AllocationPercentage = source.AllocationPercentage,
                PlannedManHours = source.PlannedManHours,
                PlannedCost = source.PlannedCost,
                StartDate = source.StartDate,
                EndDate = source.EndDate,
                Notes = $"Copied from project {sourceProjectId}"
            };

            if (!string.IsNullOrEmpty(createdBy))
            {
                copy.CreatedBy = createdBy;
                copy.CreatedAt = DateTime.UtcNow;
            }

            await _unitOfWork.Repository<RAM>().AddAsync(copy);
            copiedEntities.Add(copy);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("{Count} RAM assignments copied from project {SourceProject} to {TargetProject} by {User}", 
            copiedEntities.Count, sourceProjectId, targetProjectId, createdBy ?? "System");

        return _mapper.Map<IEnumerable<RAMDto>>(copiedEntities);
    }

    public async Task<ResourceAllocationSummaryDto> GetResourceAllocationSummaryAsync(Guid obsNodeId, CancellationToken cancellationToken = default)
    {
        var assignments = await _unitOfWork.Repository<RAM>()
            .GetAllAsync(
                filter: r => r.OBSNodeId == obsNodeId && !r.IsDeleted,
                includeProperties: "WBSElement",
                cancellationToken: cancellationToken);

        var summary = new ResourceAllocationSummaryDto
        {
            OBSNodeId = obsNodeId,
            TotalAssignments = assignments.Count(),
            TotalPlannedManHours = assignments.Sum(a => a.PlannedManHours),
            TotalPlannedCost = assignments.Sum(a => a.PlannedCost),
            AverageAllocation = assignments.Any() ? assignments.Average(a => a.AllocationPercentage) : 0,
            AssignmentsByType = assignments.GroupBy(a => a.ResponsibilityType)
                .Select(g => new ResponsibilityTypeSummaryDto
                {
                    ResponsibilityType = g.Key,
                    Count = g.Count(),
                    TotalAllocation = g.Sum(a => a.AllocationPercentage)
                }).ToList(),
            WBSAssignments = assignments.GroupBy(a => a.WBSElement)
                .Select(g => new WBSAllocationSummaryDto
                {
                    WBSElementId = g.Key.Id,
                    WBSCode = g.Key.Code,
                    WBSName = g.Key.Name,
                    ResponsibilityTypes = string.Join(", ", g.Select(a => a.ResponsibilityType)),
                    TotalAllocation = g.Sum(a => a.AllocationPercentage)
                }).ToList()
        };

        return summary;
    }

    public async Task<IEnumerable<RAMConflictDto>> CheckConflictsAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var conflicts = new List<RAMConflictDto>();

        var assignments = await _unitOfWork.Repository<RAM>()
            .GetAllAsync(
                filter: r => r.ProjectId == projectId && !r.IsDeleted,
                includeProperties: "WBSElement,OBSNode",
                cancellationToken: cancellationToken);

        // Check for over-allocation by OBS node
        var obsGroups = assignments.GroupBy(a => a.OBSNodeId);
        foreach (var group in obsGroups)
        {
            var obsNode = group.First().OBSNode;
            var totalAllocation = group.Sum(a => a.AllocationPercentage);
            
            if (totalAllocation > 100)
            {
                conflicts.Add(new RAMConflictDto
                {
                    Type = "OverAllocation",
                    Description = $"OBS node {obsNode.Code} is allocated {totalAllocation}% across all assignments",
                    OBSNodeId = obsNode.Id,
                    Severity = "High"
                });
            }
        }

        // Check for date conflicts
        foreach (var assignment in assignments.Where(a => a.StartDate.HasValue && a.EndDate.HasValue))
        {
            var overlapping = assignments.Where(a => 
                a.Id != assignment.Id &&
                a.OBSNodeId == assignment.OBSNodeId &&
                a.StartDate.HasValue && a.EndDate.HasValue &&
                ((a.StartDate <= assignment.EndDate && a.EndDate >= assignment.StartDate)));

            foreach (var overlap in overlapping)
            {
                var totalOverlapAllocation = assignment.AllocationPercentage + overlap.AllocationPercentage;
                if (totalOverlapAllocation > 100)
                {
                    conflicts.Add(new RAMConflictDto
                    {
                        Type = "DateOverlap",
                        Description = $"OBS node {assignment.OBSNode.Code} has overlapping assignments " +
                                    $"for WBS {assignment.WBSElement.Code} and {overlap.WBSElement.Code} " +
                                    $"with combined allocation of {totalOverlapAllocation}%",
                        OBSNodeId = assignment.OBSNodeId,
                        WBSElementId = assignment.WBSElementId,
                        Severity = "Medium"
                    });
                }
            }
        }

        return conflicts.Distinct(); // Remove duplicates
    }

    protected override async Task ValidateEntityAsync(RAM entity, bool isNew)
    {
        // Validate allocation percentage
        if (entity.AllocationPercentage < 0 || entity.AllocationPercentage > 100)
        {
            throw new InvalidOperationException("Allocation percentage must be between 0 and 100.");
        }

        // Validate planned values
        if (entity.PlannedManHours < 0)
        {
            throw new InvalidOperationException("Planned man hours cannot be negative.");
        }

        if (entity.PlannedCost < 0)
        {
            throw new InvalidOperationException("Planned cost cannot be negative.");
        }

        // Validate dates
        if (entity.StartDate.HasValue && entity.EndDate.HasValue && entity.EndDate < entity.StartDate)
        {
            throw new InvalidOperationException("End date must be after start date.");
        }

        // Validate responsibility type
        var validTypes = new[] { "R", "A", "C", "I" }; // RACI
        if (!validTypes.Contains(entity.ResponsibilityType))
        {
            throw new InvalidOperationException($"Invalid responsibility type. Must be one of: {string.Join(", ", validTypes)}");
        }

        // Validate unique assignment
        var exists = await _unitOfWork.Repository<RAM>()
            .AnyAsync(r => r.ProjectId == entity.ProjectId &&
                          r.WBSElementId == entity.WBSElementId &&
                          r.OBSNodeId == entity.OBSNodeId &&
                          r.ResponsibilityType == entity.ResponsibilityType &&
                          (!isNew || r.Id != entity.Id) &&
                          !r.IsDeleted);

        if (exists)
        {
            throw new InvalidOperationException("This responsibility assignment already exists.");
        }

        await base.ValidateEntityAsync(entity, isNew);
    }
}