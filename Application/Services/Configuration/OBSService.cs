using Application.Interfaces.Auth;
using Application.Interfaces.Common;
using Application.Interfaces.Configuration;
using Application.Interfaces.Organization;
using Application.Services.Base;
using AutoMapper;
using Core.DTOs.Organization.OBSNode;
using Domain.Entities.Organization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services.Configuration;

/// <summary>
/// Service for managing Organization Breakdown Structure configuration
/// This service acts as a facade over IOBSNodeService for configuration-specific operations
/// </summary>
public class OBSService : BaseService<OBSNode, OBSNodeDto, CreateOBSNodeDto, UpdateOBSNodeDto>, IOBSService
{
    private readonly IOBSNodeService _obsNodeService;
    private readonly ICurrentUserService _currentUserService;

    public OBSService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<OBSService> logger,
        IOBSNodeService obsNodeService,
        ICurrentUserService currentUserService)
        : base(unitOfWork, mapper, logger)
    {
        _obsNodeService = obsNodeService;
        _currentUserService = currentUserService;
    }

    public async Task<IEnumerable<OBSNodeDto>> GetByProjectAsync(Guid projectId)
    {
        return await _obsNodeService.GetByProjectAsync(projectId);
    }

    public async Task<IEnumerable<OBSNodeDto>> GetRootNodesAsync(Guid? projectId = null)
    {
        var query = _unitOfWork.Repository<OBSNode>().Query()
            .Where(n => n.ParentId == null);

        if (projectId.HasValue)
            query = query.Where(n => n.ProjectId == projectId.Value || n.ProjectId == null);

        var nodes = await query
            .Include(n => n.Manager)
            .Include(n => n.Project)
            .OrderBy(n => n.Code)
            .ToListAsync();

        return _mapper.Map<IEnumerable<OBSNodeDto>>(nodes);
    }

    public async Task<IEnumerable<OBSNodeDto>> GetChildrenAsync(Guid parentId)
    {
        return await _obsNodeService.GetChildrenAsync(parentId);
    }

    public async Task<OBSNodeDetailDto?> GetHierarchyAsync(Guid nodeId)
    {
        var node = await _unitOfWork.Repository<OBSNode>()
            .GetAsync(
                filter: n => n.Id == nodeId,
                includeProperties: "Parent,Manager,Project,Members,Children");

        if (node == null)
            return null;

        var dto = new OBSNodeDetailDto();
        _mapper.Map(node, dto);
        
        // Build hierarchy
        dto.Children = await BuildHierarchyAsync(node.Id);
        
        // Get ancestors
        var ancestors = new List<OBSNodeDto>();
        var current = node.Parent;
        while (current != null)
        {
            ancestors.Add(_mapper.Map<OBSNodeDto>(current));
            current = current.Parent;
        }
        ancestors.Reverse();
        dto.Ancestors = ancestors;

        return dto;
    }

    private async Task<List<OBSNodeDetailDto>> BuildHierarchyAsync(Guid parentId)
    {
        var children = await _unitOfWork.Repository<OBSNode>()
            .GetAllAsync(
                filter: n => n.ParentId == parentId,
                includeProperties: "Manager,Project,Members");

        var result = new List<OBSNodeDetailDto>();
        foreach (var child in children)
        {
            var childDto = new OBSNodeDetailDto();
            _mapper.Map(child, childDto);
            childDto.Children = await BuildHierarchyAsync(child.Id);
            result.Add(childDto);
        }

        return result;
    }

    public async Task<IEnumerable<OBSNodeDto>> SearchAsync(OBSNodeFilterDto filter)
    {
        var query = _unitOfWork.Repository<OBSNode>().Query();

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var searchTerm = filter.SearchTerm.ToLower();
            query = query.Where(n =>
                n.Code.ToLower().Contains(searchTerm) ||
                n.Name.ToLower().Contains(searchTerm) ||
                (n.Description != null && n.Description.ToLower().Contains(searchTerm)));
        }

        if (filter.ProjectId.HasValue)
            query = query.Where(n => n.ProjectId == filter.ProjectId.Value);

        if (!string.IsNullOrWhiteSpace(filter.NodeType))
            query = query.Where(n => n.NodeType == filter.NodeType);

        if (filter.ManagerId.HasValue)
            query = query.Where(n => n.ManagerId == filter.ManagerId.Value);

        if (filter.Level.HasValue)
            query = query.Where(n => n.Level == filter.Level.Value);

        if (filter.IsActive.HasValue)
            query = query.Where(n => n.IsActive == filter.IsActive.Value);

        var nodes = await query
            .Include(n => n.Manager)
            .Include(n => n.Project)
            .Include(n => n.Parent)
            .OrderBy(n => n.HierarchyPath)
            .ToListAsync();

        return _mapper.Map<IEnumerable<OBSNodeDto>>(nodes);
    }

    public async Task<bool> MoveNodeAsync(MoveOBSNodeDto dto)
    {
        // The MoveOBSNodeDto doesn't have NodeId, it needs to be passed separately
        throw new NotImplementedException("MoveNodeAsync needs to be implemented with proper parameters");
    }

    public async Task<bool> AddMemberAsync(Guid nodeId, AddOBSMemberDto dto)
    {
        var memberDto = new AddOBSNodeMemberDto
        {
            UserId = dto.UserId,
            AllocationPercentage = dto.AllocationPercentage
        };

        var result = await _obsNodeService.AddMemberAsync(
            nodeId,
            memberDto,
            _currentUserService.UserId);
        
        return result != null;
    }

    public async Task<bool> RemoveMemberAsync(Guid nodeId, Guid userId)
    {
        var result = await _obsNodeService.RemoveMemberAsync(
            nodeId,
            userId,
            _currentUserService.UserId);
        
        return result != null;
    }

    public async Task<IEnumerable<OBSMemberDto>> GetMembersAsync(Guid nodeId)
    {
        var node = await _unitOfWork.Repository<OBSNode>()
            .GetAsync(
                filter: n => n.Id == nodeId,
                includeProperties: "Members");

        if (node == null)
            return Enumerable.Empty<OBSMemberDto>();

        return node.Members.Select(m => new OBSMemberDto
        {
            UserId = m.Id,
            UserName = m.DisplayName,
            Email = m.Email,
            Role = "Member", // This would come from a junction table in a real implementation
            AllocationPercentage = 100, // Default value
            IsActive = m.IsActive
        });
    }

    public async Task<bool> UpdateCapacityAsync(Guid nodeId, decimal totalFTE, decimal availableFTE)
    {
        var result = await _obsNodeService.UpdateCapacityAsync(
            nodeId,
            totalFTE,
            availableFTE,
            _currentUserService.UserId);
        
        return result != null;
    }

    public async Task<OBSCapacityDto> GetCapacityAsync(Guid nodeId)
    {
        var node = await _unitOfWork.Repository<OBSNode>()
            .GetAsync(
                filter: n => n.Id == nodeId,
                includeProperties: "Members,Children");

        if (node == null)
            throw new InvalidOperationException($"OBS node {nodeId} not found");

        var capacity = new OBSCapacityDto
        {
            NodeId = node.Id,
            NodeName = node.Name,
            TotalFTE = node.TotalFTE,
            AvailableFTE = node.AvailableFTE,
            AllocatedFTE = node.TotalFTE.HasValue && node.AvailableFTE.HasValue 
                ? node.TotalFTE.Value - node.AvailableFTE.Value 
                : (decimal?)null,
            UtilizationRate = node.GetUtilizationRate(),
            MemberCount = node.Members?.Count ?? 0
        };

        // Calculate child capacity
        if (node.Children?.Any() == true)
        {
            capacity.ChildCapacity = new List<OBSCapacityDto>();
            foreach (var child in node.Children)
            {
                var childCapacity = await GetCapacityAsync(child.Id);
                capacity.ChildCapacity.Add(childCapacity);
            }
        }

        return capacity;
    }

    public async Task<IEnumerable<OBSResourceAllocationDto>> GetResourceAllocationsAsync(Guid nodeId)
    {
        var node = await _unitOfWork.Repository<OBSNode>()
            .GetAsync(
                filter: n => n.Id == nodeId,
                includeProperties: "Members");

        if (node == null)
            return Enumerable.Empty<OBSResourceAllocationDto>();

        // This would typically query resource allocations from project assignments
        // For now, returning mock data based on members
        return node.Members.Select(m => new OBSResourceAllocationDto
        {
            ResourceId = m.Id,
            ResourceName = m.DisplayName,
            NodeId = node.Id,
            NodeName = node.Name,
            AllocationPercentage = 100,
            AllocatedHours = 40,
            AvailableHours = 40,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(3)
        });
    }

    public async Task<bool> AssignToProjectAsync(Guid nodeId, Guid projectId)
    {
        var result = await _obsNodeService.AssignToProjectAsync(
            nodeId,
            projectId,
            _currentUserService.UserId);
        
        return result != null;
    }

    public async Task<OBSNodeDto?> CopyStructureAsync(Guid sourceNodeId, Guid? targetParentId, Guid? targetProjectId)
    {
        var sourceNode = await _unitOfWork.Repository<OBSNode>()
            .GetAsync(
                filter: n => n.Id == sourceNodeId,
                includeProperties: "Children,Members");

        if (sourceNode == null)
            return null;

        // Create new node
        var newNode = new OBSNode(
            $"{sourceNode.Code}_COPY",
            $"{sourceNode.Name} (Copy)",
            sourceNode.NodeType,
            targetParentId.HasValue ? sourceNode.Level : 0);

        if (targetParentId.HasValue)
        {
            var parent = await _unitOfWork.Repository<OBSNode>()
                .GetByIdAsync(targetParentId.Value);
            if (parent != null)
                newNode.SetParent(parent);
        }

        if (targetProjectId.HasValue)
            newNode.AssignToProject(targetProjectId.Value);

        newNode.CreatedBy = _currentUserService.UserId ?? "System";
        newNode.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.Repository<OBSNode>().AddAsync(newNode);
        await _unitOfWork.SaveChangesAsync();

        // Recursively copy children
        if (sourceNode.Children?.Any() == true)
        {
            foreach (var child in sourceNode.Children)
            {
                await CopyStructureAsync(child.Id, newNode.Id, targetProjectId);
            }
        }

        _logger.LogInformation("OBS structure copied from {SourceId} to {NewId} by {User}",
            sourceNodeId, newNode.Id, _currentUserService.UserId);

        return _mapper.Map<OBSNodeDto>(newNode);
    }

    public async Task<byte[]> ExportAsync(Guid? projectId = null, string format = "Excel")
    {
        // This would export OBS structure to Excel/CSV/JSON format
        // Implementation would depend on the export library being used
        throw new NotImplementedException("Export functionality to be implemented");
    }

    public async Task<int> ImportAsync(byte[] data, Guid? projectId = null, bool merge = false)
    {
        // This would import OBS structure from Excel/CSV/JSON format
        // Implementation would depend on the import library being used
        throw new NotImplementedException("Import functionality to be implemented");
    }

    public async Task<bool> IsCodeUniqueAsync(string code, Guid? projectId = null, Guid? excludeId = null)
    {
        var query = _unitOfWork.Repository<OBSNode>()
            .Query()
            .Where(n => n.Code == code);

        if (projectId.HasValue)
            query = query.Where(n => n.ProjectId == projectId.Value || n.ProjectId == null);

        if (excludeId.HasValue)
            query = query.Where(n => n.Id != excludeId.Value);

        return !await query.AnyAsync();
    }

    // These methods delegate to the base service methods
    public async Task<OBSNodeDto> CreateAsync(CreateOBSNodeDto dto)
    {
        return await base.CreateAsync(dto, _currentUserService.UserId);
    }

    public async Task<OBSNodeDto?> UpdateAsync(Guid id, UpdateOBSNodeDto dto)
    {
        return await base.UpdateAsync(id, dto, _currentUserService.UserId);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await base.DeleteAsync(id);
    }
}