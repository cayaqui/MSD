using Application.Interfaces.Organization;
using Application.Services.Base;
using Core.DTOs.Organization.OBSNode;
using Domain.Entities.Organization.Core;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services.Organization;

/// <summary>
/// Service implementation for Organizational Breakdown Structure (OBS) management
/// </summary>
public class OBSNodeService : BaseService<OBSNode, OBSNodeDto, CreateOBSNodeDto, UpdateOBSNodeDto>, IOBSNodeService
{
    public OBSNodeService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<OBSNodeService> logger)
        : base(unitOfWork, mapper, logger)
    {
    }

    public IUnitOfWork UnitOfWork => _unitOfWork;

    public async Task<IEnumerable<OBSNodeDto>> GetByProjectAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.Repository<OBSNode>()
            .GetAllAsync(
                filter: o => o.ProjectId == projectId && !o.IsDeleted,
                includeProperties: "Parent,Manager",
                orderBy: q => q.OrderBy(o => o.Level).ThenBy(o => o.Code),
                cancellationToken: cancellationToken);

        return _mapper.Map<IEnumerable<OBSNodeDto>>(entities);
    }

    public async Task<IEnumerable<OBSNodeDto>> GetChildrenAsync(Guid parentId, CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.Repository<OBSNode>()
            .GetAllAsync(
                filter: o => o.ParentId == parentId && !o.IsDeleted,
                includeProperties: "Manager",
                orderBy: q => q.OrderBy(o => o.Code),
                cancellationToken: cancellationToken);

        return _mapper.Map<IEnumerable<OBSNodeDto>>(entities);
    }

    public async Task<OBSNodeTreeDto> GetHierarchyTreeAsync(Guid? projectId = null, CancellationToken cancellationToken = default)
    {
        var query = _unitOfWork.Repository<OBSNode>().GetQueryable();
        
        if (projectId.HasValue)
            query = query.Where(o => o.ProjectId == projectId);
        
        var nodes = await query
            .Where(o => !o.IsDeleted)
            .Include(o => o.Manager)
            .ToListAsync(cancellationToken);

        // Build tree structure
        var rootNodes = nodes.Where(n => n.ParentId == null).ToList();
        var tree = new OBSNodeTreeDto
        {
            ProjectId = projectId,
            RootNodes = BuildNodeTree(rootNodes, nodes)
        };

        return tree;
    }

    private List<OBSNodeHierarchyDto> BuildNodeTree(List<OBSNode> currentLevelNodes, List<OBSNode> allNodes)
    {
        var result = new List<OBSNodeHierarchyDto>();
        
        foreach (var node in currentLevelNodes)
        {
            var hierarchyDto = _mapper.Map<OBSNodeHierarchyDto>(node);
            var children = allNodes.Where(n => n.ParentId == node.Id).ToList();
            
            if (children.Any())
            {
                hierarchyDto.Children = BuildNodeTree(children, allNodes);
            }
            
            result.Add(hierarchyDto);
        }
        
        return result;
    }

    public async Task<OBSNodeDto?> SetParentAsync(Guid id, Guid? parentId, string? updatedBy = null, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<OBSNode>().GetByIdAsync(id);
        if (entity == null)
            return null;

        if (parentId.HasValue)
        {
            // Validate no circular reference
            var canMove = await CanMoveNodeAsync(id, parentId.Value, cancellationToken);
            if (!canMove)
            {
                throw new InvalidOperationException("Cannot set parent due to circular reference.");
            }

            // Update level based on parent
            var parent = await _unitOfWork.Repository<OBSNode>().GetByIdAsync(parentId.Value);
            if (parent != null)
            {
                entity.Level = parent.Level + 1;
            }
        }
        else
        {
            entity.Level = 0; // Root level
        }

        entity.ParentId = parentId;

        if (!string.IsNullOrEmpty(updatedBy))
        {
            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        _unitOfWork.Repository<OBSNode>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("OBS node {NodeCode} parent set to {ParentId} by {User}", 
            entity.Code, parentId?.ToString() ?? "None", updatedBy ?? "System");

        return _mapper.Map<OBSNodeDto>(entity);
    }

    public async Task<OBSNodeDto?> AssignToProjectAsync(Guid id, Guid projectId, string? updatedBy = null, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<OBSNode>().GetByIdAsync(id);
        if (entity == null)
            return null;

        entity.ProjectId = projectId;

        if (!string.IsNullOrEmpty(updatedBy))
        {
            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        _unitOfWork.Repository<OBSNode>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("OBS node {NodeCode} assigned to project {ProjectId} by {User}", 
            entity.Code, projectId, updatedBy ?? "System");

        return _mapper.Map<OBSNodeDto>(entity);
    }

    public async Task<OBSNodeDto?> SetManagerAsync(Guid id, Guid managerId, string? updatedBy = null, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<OBSNode>().GetByIdAsync(id);
        if (entity == null)
            return null;

        entity.ManagerId = managerId;

        if (!string.IsNullOrEmpty(updatedBy))
        {
            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        _unitOfWork.Repository<OBSNode>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Manager {ManagerId} assigned to OBS node {NodeCode} by {User}", 
            managerId, entity.Code, updatedBy ?? "System");

        return _mapper.Map<OBSNodeDto>(entity);
    }

    public async Task<OBSNodeDto?> UpdateCapacityAsync(Guid id, decimal totalFTE, decimal availableFTE, string? updatedBy = null, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<OBSNode>().GetByIdAsync(id);
        if (entity == null)
            return null;

        entity.TotalFTE = totalFTE;
        entity.AvailableFTE = availableFTE;

        if (!string.IsNullOrEmpty(updatedBy))
        {
            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        _unitOfWork.Repository<OBSNode>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("OBS node {NodeCode} capacity updated to Total: {TotalFTE}, Available: {AvailableFTE} by {User}", 
            entity.Code, totalFTE, availableFTE, updatedBy ?? "System");

        return _mapper.Map<OBSNodeDto>(entity);
    }

    public async Task<OBSNodeDto?> AddMemberAsync(Guid id, AddOBSNodeMemberDto dto, string? updatedBy = null, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<OBSNode>()
            .GetAsync(
                filter: o => o.Id == id && !o.IsDeleted,
                includeProperties: "TeamMembers",
                cancellationToken: cancellationToken);

        if (entity == null)
            return null;

        // Check if user is already a member
        if (entity.TeamMembers.Any(tm => tm.UserId == dto.UserId))
        {
            throw new InvalidOperationException("User is already a member of this OBS node.");
        }

        // Add team member
        var teamMember = new Domain.Entities.Auth.Security.ProjectTeamMember
        {
            UserId = dto.UserId,
            ProjectId = entity.ProjectId,
            RoleId = dto.RoleId,
            AllocationPercentage = dto.AllocationPercentage,
            StartDate = dto.StartDate ?? DateTime.UtcNow,
            EndDate = dto.EndDate
        };

        if (!string.IsNullOrEmpty(updatedBy))
        {
            teamMember.CreatedBy = updatedBy;
            teamMember.CreatedAt = DateTime.UtcNow;
        }

        entity.TeamMembers.Add(teamMember);

        _unitOfWork.Repository<OBSNode>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User {UserId} added to OBS node {NodeCode} by {User}", 
            dto.UserId, entity.Code, updatedBy ?? "System");

        return _mapper.Map<OBSNodeDto>(entity);
    }

    public async Task<OBSNodeDto?> RemoveMemberAsync(Guid id, Guid userId, string? updatedBy = null, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<OBSNode>()
            .GetAsync(
                filter: o => o.Id == id && !o.IsDeleted,
                includeProperties: "TeamMembers",
                cancellationToken: cancellationToken);

        if (entity == null)
            return null;

        var memberToRemove = entity.TeamMembers.FirstOrDefault(tm => tm.UserId == userId);
        if (memberToRemove == null)
        {
            throw new InvalidOperationException("User is not a member of this OBS node.");
        }

        entity.TeamMembers.Remove(memberToRemove);

        _unitOfWork.Repository<OBSNode>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User {UserId} removed from OBS node {NodeCode} by {User}", 
            userId, entity.Code, updatedBy ?? "System");

        return _mapper.Map<OBSNodeDto>(entity);
    }

    public async Task<int> GetMemberCountAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<OBSNode>()
            .GetAsync(
                filter: o => o.Id == id && !o.IsDeleted,
                includeProperties: "TeamMembers",
                cancellationToken: cancellationToken);

        return entity?.TeamMembers.Count ?? 0;
    }

    public async Task<decimal> GetUtilizationRateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<OBSNode>().GetByIdAsync(id);
        if (entity == null || entity.TotalFTE == 0)
            return 0;

        var utilizedFTE = entity.TotalFTE - entity.AvailableFTE;
        return Math.Round((utilizedFTE / entity.TotalFTE) * 100, 2);
    }

    public async Task<OBSNodeDto?> MoveNodeAsync(Guid id, MoveOBSNodeDto dto, string? updatedBy = null, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<OBSNode>().GetByIdAsync(id);
        if (entity == null)
            return null;

        // Validate move
        if (dto.NewParentId.HasValue)
        {
            var canMove = await CanMoveNodeAsync(id, dto.NewParentId.Value, cancellationToken);
            if (!canMove)
            {
                throw new InvalidOperationException("Cannot move node due to circular reference.");
            }
        }

        // Update parent and recalculate levels
        entity.ParentId = dto.NewParentId;
        await RecalculateNodeLevels(entity, cancellationToken);

        if (!string.IsNullOrEmpty(updatedBy))
        {
            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        _unitOfWork.Repository<OBSNode>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("OBS node {NodeCode} moved to parent {ParentId} by {User}", 
            entity.Code, dto.NewParentId?.ToString() ?? "Root", updatedBy ?? "System");

        return _mapper.Map<OBSNodeDto>(entity);
    }

    public async Task<IEnumerable<OBSNodeDto>> GetDescendantsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var allNodes = await _unitOfWork.Repository<OBSNode>()
            .GetAllAsync(filter: o => !o.IsDeleted, cancellationToken: cancellationToken);

        var descendants = new List<OBSNode>();
        await GetDescendantsRecursive(id, allNodes.ToList(), descendants);

        return _mapper.Map<IEnumerable<OBSNodeDto>>(descendants);
    }

    private async Task GetDescendantsRecursive(Guid parentId, List<OBSNode> allNodes, List<OBSNode> descendants)
    {
        var children = allNodes.Where(n => n.ParentId == parentId).ToList();
        descendants.AddRange(children);

        foreach (var child in children)
        {
            await GetDescendantsRecursive(child.Id, allNodes, descendants);
        }
    }

    public async Task<bool> CanMoveNodeAsync(Guid nodeId, Guid newParentId, CancellationToken cancellationToken = default)
    {
        if (nodeId == newParentId)
            return false; // Cannot be its own parent

        // Check if newParentId is a descendant of nodeId
        var descendants = await GetDescendantsAsync(nodeId, cancellationToken);
        return !descendants.Any(d => d.Id == newParentId);
    }

    private async Task RecalculateNodeLevels(OBSNode node, CancellationToken cancellationToken)
    {
        if (node.ParentId.HasValue)
        {
            var parent = await _unitOfWork.Repository<OBSNode>().GetByIdAsync(node.ParentId.Value);
            node.Level = parent != null ? parent.Level + 1 : 0;
        }
        else
        {
            node.Level = 0;
        }

        // Update descendants
        var descendants = await GetDescendantsAsync(node.Id, cancellationToken);
        foreach (var descendant in descendants)
        {
            var descendantEntity = await _unitOfWork.Repository<OBSNode>().GetByIdAsync(descendant.Id);
            if (descendantEntity != null && descendantEntity.ParentId.HasValue)
            {
                var parentEntity = await _unitOfWork.Repository<OBSNode>().GetByIdAsync(descendantEntity.ParentId.Value);
                if (parentEntity != null)
                {
                    descendantEntity.Level = parentEntity.Level + 1;
                    _unitOfWork.Repository<OBSNode>().Update(descendantEntity);
                }
            }
        }
    }

    protected override async Task ValidateEntityAsync(OBSNode entity, bool isNew)
    {
        // Validate FTE values
        if (entity.TotalFTE < 0)
        {
            throw new InvalidOperationException("Total FTE cannot be negative.");
        }

        if (entity.AvailableFTE < 0)
        {
            throw new InvalidOperationException("Available FTE cannot be negative.");
        }

        if (entity.AvailableFTE > entity.TotalFTE)
        {
            throw new InvalidOperationException("Available FTE cannot exceed total FTE.");
        }

        // Validate level
        if (entity.Level < 0)
        {
            throw new InvalidOperationException("Node level cannot be negative.");
        }

        // Validate code uniqueness within project
        var codeExists = await _unitOfWork.Repository<OBSNode>()
            .AnyAsync(o => o.Code == entity.Code && 
                          o.ProjectId == entity.ProjectId &&
                          (!isNew || o.Id != entity.Id) &&
                          !o.IsDeleted);

        if (codeExists)
        {
            throw new InvalidOperationException($"OBS node code '{entity.Code}' already exists in this project.");
        }

        await base.ValidateEntityAsync(entity, isNew);
    }
}