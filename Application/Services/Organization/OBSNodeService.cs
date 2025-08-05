using Application.Interfaces.Organization;
using Application.Services.Base;
using AutoMapper;
using Core.DTOs.Organization.OBSNode;
using Domain.Entities.Organization.Core;
using Domain.Entities.Auth.Security;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Application.Services.Organization
{
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

        /// <summary>
        /// Gets OBS nodes by project
        /// </summary>
        public async Task<IEnumerable<OBSNodeDto>> GetByProjectAsync(Guid projectId, CancellationToken cancellationToken = default)
        {
            var nodes = await _unitOfWork.Repository<OBSNode>()
                .GetAllAsync(
                    filter: n => n.ProjectId == projectId,
                    includeProperties: "Parent,Manager,Project,Members",
                    orderBy: q => q.OrderBy(n => n.HierarchyPath),
                    cancellationToken: cancellationToken);

            var dtos = nodes.Select(n =>
            {
                var dto = _mapper.Map<OBSNodeDto>(n);
                MapComputedProperties(dto, n);
                return dto;
            });

            return dtos;
        }

        /// <summary>
        /// Gets OBS nodes by parent
        /// </summary>
        public async Task<IEnumerable<OBSNodeDto>> GetChildrenAsync(Guid parentId, CancellationToken cancellationToken = default)
        {
            var nodes = await _unitOfWork.Repository<OBSNode>()
                .GetAllAsync(
                    filter: n => n.ParentId == parentId,
                    includeProperties: "Parent,Manager,Project,Members",
                    orderBy: q => q.OrderBy(n => n.Code),
                    cancellationToken: cancellationToken);

            var dtos = nodes.Select(n =>
            {
                var dto = _mapper.Map<OBSNodeDto>(n);
                MapComputedProperties(dto, n);
                return dto;
            });

            return dtos;
        }

        /// <summary>
        /// Gets OBS hierarchy tree
        /// </summary>
        public async Task<OBSNodeTreeDto> GetHierarchyTreeAsync(Guid? projectId = null, CancellationToken cancellationToken = default)
        {
            Expression<Func<OBSNode, bool>> filter = n => true;
            if (projectId.HasValue)
            {
                filter = n => n.ProjectId == projectId.Value || n.ProjectId == null;
            }

            var allNodes = await _unitOfWork.Repository<OBSNode>()
                .GetAllAsync(
                    filter: filter,
                    includeProperties: "Parent,Manager,Project,Members,Children",
                    orderBy: q => q.OrderBy(n => n.HierarchyPath),
                    cancellationToken: cancellationToken);

            var nodesList = allNodes.ToList();
            var rootNodes = nodesList.Where(n => n.ParentId == null).ToList();

            var tree = new OBSNodeTreeDto
            {
                ProjectId = projectId
            };

            foreach (var rootNode in rootNodes)
            {
                var hierarchyDto = BuildHierarchyDto(rootNode, nodesList);
                tree.RootNodes.Add(hierarchyDto);
            }

            return tree;
        }

        /// <summary>
        /// Sets node parent
        /// </summary>
        public async Task<OBSNodeDto?> SetParentAsync(
            Guid id,
            Guid? parentId,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<OBSNode>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null)
                return null;

            if (parentId.HasValue)
            {
                var parent = await _unitOfWork.Repository<OBSNode>()
                    .GetByIdAsync(parentId.Value, cancellationToken);

                if (parent == null)
                    throw new InvalidOperationException($"Parent node {parentId} not found");

                // Check for circular reference
                if (parent.IsDescendantOf(entity))
                    throw new InvalidOperationException("Cannot create circular reference in hierarchy");

                entity.SetParent(parent);
            }
            else
            {
                // Remove parent (make it root node)
                entity.RemoveParent();
            }

            entity.UpdatedBy = updatedBy ?? "System";
            entity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<OBSNode>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("OBS node {NodeCode} parent changed by {User}",
                entity.Code, updatedBy ?? "System");

            // Reload with navigation properties
            entity = await _unitOfWork.Repository<OBSNode>()
                .GetAsync(
                    filter: n => n.Id == id,
                    includeProperties: "Parent,Manager,Project,Members",
                    cancellationToken: cancellationToken);

            var dto = _mapper.Map<OBSNodeDto>(entity);
            MapComputedProperties(dto, entity);
            
            return dto;
        }

        /// <summary>
        /// Assigns node to project
        /// </summary>
        public async Task<OBSNodeDto?> AssignToProjectAsync(
            Guid id,
            Guid projectId,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<OBSNode>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null)
                return null;

            entity.AssignToProject(projectId);
            entity.UpdatedBy = updatedBy ?? "System";
            entity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<OBSNode>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("OBS node {NodeCode} assigned to project {ProjectId} by {User}",
                entity.Code, projectId, updatedBy ?? "System");

            // Reload with navigation properties
            entity = await _unitOfWork.Repository<OBSNode>()
                .GetAsync(
                    filter: n => n.Id == id,
                    includeProperties: "Parent,Manager,Project,Members",
                    cancellationToken: cancellationToken);

            var dto = _mapper.Map<OBSNodeDto>(entity);
            MapComputedProperties(dto, entity);
            
            return dto;
        }

        /// <summary>
        /// Sets node manager
        /// </summary>
        public async Task<OBSNodeDto?> SetManagerAsync(
            Guid id,
            Guid managerId,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<OBSNode>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null)
                return null;

            entity.SetManager(managerId);
            entity.UpdatedBy = updatedBy ?? "System";
            entity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<OBSNode>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("OBS node {NodeCode} manager set to {ManagerId} by {User}",
                entity.Code, managerId, updatedBy ?? "System");

            // Reload with navigation properties
            entity = await _unitOfWork.Repository<OBSNode>()
                .GetAsync(
                    filter: n => n.Id == id,
                    includeProperties: "Parent,Manager,Project,Members",
                    cancellationToken: cancellationToken);

            var dto = _mapper.Map<OBSNodeDto>(entity);
            MapComputedProperties(dto, entity);
            
            return dto;
        }

        /// <summary>
        /// Updates node capacity
        /// </summary>
        public async Task<OBSNodeDto?> UpdateCapacityAsync(
            Guid id,
            decimal totalFTE,
            decimal availableFTE,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<OBSNode>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null)
                return null;

            entity.UpdateCapacity(totalFTE, availableFTE);
            entity.UpdatedBy = updatedBy ?? "System";
            entity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<OBSNode>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("OBS node {NodeCode} capacity updated to {TotalFTE}/{AvailableFTE} FTE by {User}",
                entity.Code, totalFTE, availableFTE, updatedBy ?? "System");

            var dto = _mapper.Map<OBSNodeDto>(entity);
            MapComputedProperties(dto, entity);
            
            return dto;
        }

        /// <summary>
        /// Adds member to OBS node
        /// </summary>
        public async Task<OBSNodeDto?> AddMemberAsync(
            Guid id,
            AddOBSNodeMemberDto dto,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<OBSNode>()
                .GetAsync(
                    filter: n => n.Id == id,
                    includeProperties: "Members",
                    cancellationToken: cancellationToken);

            if (entity == null)
                return null;

            var user = await _unitOfWork.Repository<User>()
                .GetByIdAsync(dto.UserId, cancellationToken);

            if (user == null)
                throw new InvalidOperationException($"User {dto.UserId} not found");

            entity.AddMember(user);
            entity.UpdatedBy = updatedBy ?? "System";
            entity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<OBSNode>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User {UserId} added to OBS node {NodeCode} by {User}",
                dto.UserId, entity.Code, updatedBy ?? "System");

            // Reload with navigation properties
            entity = await _unitOfWork.Repository<OBSNode>()
                .GetAsync(
                    filter: n => n.Id == id,
                    includeProperties: "Parent,Manager,Project,Members",
                    cancellationToken: cancellationToken);

            var resultDto = _mapper.Map<OBSNodeDto>(entity);
            MapComputedProperties(resultDto, entity);
            
            return resultDto;
        }

        /// <summary>
        /// Removes member from OBS node
        /// </summary>
        public async Task<OBSNodeDto?> RemoveMemberAsync(
            Guid id,
            Guid userId,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<OBSNode>()
                .GetAsync(
                    filter: n => n.Id == id,
                    includeProperties: "Members",
                    cancellationToken: cancellationToken);

            if (entity == null)
                return null;

            var user = entity.Members.FirstOrDefault(m => m.Id == userId);
            if (user != null)
            {
                entity.RemoveMember(user);
                entity.UpdatedBy = updatedBy ?? "System";
                entity.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Repository<OBSNode>().Update(entity);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("User {UserId} removed from OBS node {NodeCode} by {User}",
                    userId, entity.Code, updatedBy ?? "System");
            }

            // Reload with navigation properties
            entity = await _unitOfWork.Repository<OBSNode>()
                .GetAsync(
                    filter: n => n.Id == id,
                    includeProperties: "Parent,Manager,Project,Members",
                    cancellationToken: cancellationToken);

            var dto = _mapper.Map<OBSNodeDto>(entity);
            MapComputedProperties(dto, entity);
            
            return dto;
        }

        /// <summary>
        /// Gets node member count
        /// </summary>
        public async Task<int> GetMemberCountAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<OBSNode>()
                .GetAsync(
                    filter: n => n.Id == id,
                    includeProperties: "Members",
                    cancellationToken: cancellationToken);

            return entity?.Members?.Count ?? 0;
        }

        /// <summary>
        /// Gets node utilization rate
        /// </summary>
        public async Task<decimal> GetUtilizationRateAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<OBSNode>()
                .GetByIdAsync(id, cancellationToken);

            return entity?.GetUtilizationRate() ?? 0;
        }

        /// <summary>
        /// Moves node to different parent
        /// </summary>
        public async Task<OBSNodeDto?> MoveNodeAsync(
            Guid id,
            MoveOBSNodeDto dto,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            // Validate move operation first
            var canMove = await CanMoveNodeAsync(id, dto.NewParentId, cancellationToken);
            if (!canMove)
                throw new InvalidOperationException("Cannot move node to the specified parent (circular reference would be created)");

            return await SetParentAsync(id, dto.NewParentId, updatedBy, cancellationToken);
        }

        /// <summary>
        /// Gets all descendants of a node
        /// </summary>
        public async Task<IEnumerable<OBSNodeDto>> GetDescendantsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<OBSNode>()
                .GetAsync(
                    filter: n => n.Id == id,
                    includeProperties: "Children",
                    cancellationToken: cancellationToken);

            if (entity == null)
                return Enumerable.Empty<OBSNodeDto>();

            var allNodes = await _unitOfWork.Repository<OBSNode>()
                .GetAllAsync(
                    filter: n => n.HierarchyPath.StartsWith(entity.HierarchyPath) && n.Id != id,
                    includeProperties: "Parent,Manager,Project,Members",
                    orderBy: q => q.OrderBy(n => n.HierarchyPath),
                    cancellationToken: cancellationToken);

            var dtos = allNodes.Select(n =>
            {
                var dto = _mapper.Map<OBSNodeDto>(n);
                MapComputedProperties(dto, n);
                return dto;
            });

            return dtos;
        }

        /// <summary>
        /// Validates if move operation is valid (no circular references)
        /// </summary>
        public async Task<bool> CanMoveNodeAsync(
            Guid nodeId,
            Guid newParentId,
            CancellationToken cancellationToken = default)
        {
            if (nodeId == newParentId)
                return false;

            var node = await _unitOfWork.Repository<OBSNode>()
                .GetByIdAsync(nodeId, cancellationToken);

            var newParent = await _unitOfWork.Repository<OBSNode>()
                .GetAsync(
                    filter: n => n.Id == newParentId,
                    includeProperties: "Parent",
                    cancellationToken: cancellationToken);

            if (node == null || newParent == null)
                return false;

            // Check if new parent is a descendant of the node being moved
            return !newParent.IsDescendantOf(node);
        }

        /// <summary>
        /// Override create to handle entity creation properly
        /// </summary>
        public override async Task<OBSNodeDto> CreateAsync(
            CreateOBSNodeDto createDto,
            string? createdBy = null,
            CancellationToken cancellationToken = default)
        {
            // Determine level based on parent
            int level = 0;
            OBSNode? parent = null;
            
            if (createDto.ParentId.HasValue)
            {
                parent = await _unitOfWork.Repository<OBSNode>()
                    .GetByIdAsync(createDto.ParentId.Value, cancellationToken);
                
                if (parent == null)
                    throw new InvalidOperationException($"Parent node {createDto.ParentId} not found");
                
                level = parent.Level + 1;
            }

            var entity = new OBSNode(
                createDto.Code,
                createDto.Name,
                createDto.NodeType,
                level);

            // Set cost center if provided
            if (!string.IsNullOrWhiteSpace(createDto.CostCenter))
            {
                entity.SetCostCenter(createDto.CostCenter);
            }

            // Set parent if provided
            if (parent != null)
            {
                entity.SetParent(parent);
            }

            // Assign to project if provided
            if (createDto.ProjectId.HasValue)
            {
                entity.AssignToProject(createDto.ProjectId.Value);
            }

            // Set manager if provided
            if (createDto.ManagerId.HasValue)
            {
                entity.SetManager(createDto.ManagerId.Value);
            }

            // Set capacity if provided
            if (createDto.TotalFTE.HasValue && createDto.AvailableFTE.HasValue)
            {
                entity.UpdateCapacity(createDto.TotalFTE.Value, createDto.AvailableFTE.Value);
            }

            entity.CreatedBy = createdBy ?? "System";
            entity.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.Repository<OBSNode>().AddAsync(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("OBS node {NodeCode} created by {User}",
                entity.Code, createdBy ?? "System");

            // Reload with navigation properties
            entity = await _unitOfWork.Repository<OBSNode>()
                .GetAsync(
                    filter: n => n.Id == entity.Id,
                    includeProperties: "Parent,Manager,Project,Members",
                    cancellationToken: cancellationToken);

            var dto = _mapper.Map<OBSNodeDto>(entity);
            MapComputedProperties(dto, entity);
            
            return dto;
        }

        /// <summary>
        /// Override update to use entity methods
        /// </summary>
        public override async Task<OBSNodeDto?> UpdateAsync(
            Guid id,
            UpdateOBSNodeDto updateDto,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<OBSNode>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null)
                return null;

            // Update basic properties
            entity.UpdateBasicInfo(updateDto.Name, updateDto.Description, updateDto.NodeType);

            // Update cost center
            if (updateDto.CostCenter != entity.CostCenter)
            {
                entity.SetCostCenter(updateDto.CostCenter);
            }

            // Update manager if changed
            if (updateDto.ManagerId != entity.ManagerId && updateDto.ManagerId.HasValue)
            {
                entity.SetManager(updateDto.ManagerId.Value);
            }

            // Update capacity if provided
            if (updateDto.TotalFTE.HasValue && updateDto.AvailableFTE.HasValue)
            {
                entity.UpdateCapacity(updateDto.TotalFTE.Value, updateDto.AvailableFTE.Value);
            }

            // Update active status
            entity.IsActive = updateDto.IsActive;

            entity.UpdatedBy = updatedBy ?? "System";
            entity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<OBSNode>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("OBS node {NodeCode} updated by {User}",
                entity.Code, updatedBy ?? "System");

            // Reload with navigation properties
            entity = await _unitOfWork.Repository<OBSNode>()
                .GetAsync(
                    filter: n => n.Id == id,
                    includeProperties: "Parent,Manager,Project,Members",
                    cancellationToken: cancellationToken);

            var dto = _mapper.Map<OBSNodeDto>(entity);
            MapComputedProperties(dto, entity);
            
            return dto;
        }

        /// <summary>
        /// Override GetByIdAsync to include navigation properties
        /// </summary>
        public override async Task<OBSNodeDto?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<OBSNode>()
                .GetAsync(
                    filter: n => n.Id == id,
                    includeProperties: "Parent,Manager,Project,Members,Children",
                    cancellationToken: cancellationToken);

            if (entity == null)
                return null;

            var dto = _mapper.Map<OBSNodeDto>(entity);
            MapComputedProperties(dto, entity);
            
            return dto;
        }

        /// <summary>
        /// Override GetAllAsync to include navigation properties
        /// </summary>
        public override async Task<IEnumerable<OBSNodeDto>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            var entities = await _unitOfWork.Repository<OBSNode>()
                .GetAllAsync(
                    includeProperties: "Parent,Manager,Project,Members",
                    orderBy: q => q.OrderBy(n => n.HierarchyPath),
                    cancellationToken: cancellationToken);

            var dtos = entities.Select(n =>
            {
                var dto = _mapper.Map<OBSNodeDto>(n);
                MapComputedProperties(dto, n);
                return dto;
            });

            return dtos;
        }

        /// <summary>
        /// Maps computed properties from entity to DTO
        /// </summary>
        private void MapComputedProperties(OBSNodeDto dto, OBSNode entity)
        {
            dto.UtilizationRate = entity.GetUtilizationRate();
            dto.HasChildren = entity.Children?.Any() ?? false;
            dto.MemberCount = entity.Members?.Count ?? 0;
            
            // Map navigation property names
            dto.ParentName = entity.Parent?.Name;
            dto.ProjectName = entity.Project?.Name;
            dto.ProjectCode = entity.Project?.Code;
            dto.ManagerName = entity.Manager?.DisplayName;
            dto.ManagerEmail = entity.Manager?.Email;
        }

        /// <summary>
        /// Builds hierarchical DTO from entity
        /// </summary>
        private OBSNodeHierarchyDto BuildHierarchyDto(OBSNode node, List<OBSNode> allNodes)
        {
            var dto = new OBSNodeHierarchyDto();
            _mapper.Map(node, dto);
            MapComputedProperties(dto, node);

            var children = allNodes.Where(n => n.ParentId == node.Id).ToList();
            foreach (var child in children)
            {
                var childDto = BuildHierarchyDto(child, allNodes);
                dto.Children.Add(childDto);
            }

            return dto;
        }

        /// <summary>
        /// Gets the unit of work instance
        /// </summary>
        public IUnitOfWork UnitOfWork => _unitOfWork;
    }
}