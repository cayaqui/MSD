using Application.Interfaces.Organization;
using Core.DTOs.Organization.RAM;
using Domain.Entities.Cost.Control;
using Domain.Entities.WBS;


namespace Application.Services.Organization
{
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

        /// <summary>
        /// Gets RAM assignments by project
        /// </summary>
        public async Task<IEnumerable<RAMDto>> GetByProjectAsync(Guid projectId, CancellationToken cancellationToken = default)
        {
            var entities = await _unitOfWork.Repository<RAM>()
                .GetAllAsync(
                    filter: r => r.ProjectId == projectId && r.IsActive,
                    includeProperties: "Project,WBSElement,OBSNode,ControlAccount",
                    orderBy: q => q.OrderBy(r => r.WBSElement.Code).ThenBy(r => r.OBSNode.Code),
                    cancellationToken: cancellationToken);

            return entities.Select(r =>
            {
                var dto = _mapper.Map<RAMDto>(r);
                MapComputedProperties(dto, r);
                return dto;
            });
        }

        /// <summary>
        /// Gets RAM assignments by WBS element
        /// </summary>
        public async Task<IEnumerable<RAMDto>> GetByWBSElementAsync(Guid wbsElementId, CancellationToken cancellationToken = default)
        {
            var entities = await _unitOfWork.Repository<RAM>()
                .GetAllAsync(
                    filter: r => r.WBSElementId == wbsElementId && r.IsActive,
                    includeProperties: "Project,WBSElement,OBSNode,ControlAccount",
                    orderBy: q => q.OrderByDescending(r => r.ResponsibilityType == RAM.ResponsibilityTypes.Accountable)
                                   .ThenByDescending(r => r.ResponsibilityType == RAM.ResponsibilityTypes.Responsible)
                                   .ThenBy(r => r.OBSNode.Code),
                    cancellationToken: cancellationToken);

            return entities.Select(r =>
            {
                var dto = _mapper.Map<RAMDto>(r);
                MapComputedProperties(dto, r);
                return dto;
            });
        }

        /// <summary>
        /// Gets RAM assignments by OBS node
        /// </summary>
        public async Task<IEnumerable<RAMDto>> GetByOBSNodeAsync(Guid obsNodeId, CancellationToken cancellationToken = default)
        {
            var entities = await _unitOfWork.Repository<RAM>()
                .GetAllAsync(
                    filter: r => r.OBSNodeId == obsNodeId && r.IsActive,
                    includeProperties: "Project,WBSElement,OBSNode,ControlAccount",
                    orderBy: q => q.OrderBy(r => r.WBSElement.Code),
                    cancellationToken: cancellationToken);

            return entities.Select(r =>
            {
                var dto = _mapper.Map<RAMDto>(r);
                MapComputedProperties(dto, r);
                return dto;
            });
        }

        /// <summary>
        /// Gets RAM assignments by responsibility type
        /// </summary>
        public async Task<IEnumerable<RAMDto>> GetByResponsibilityTypeAsync(
            Guid projectId, 
            string responsibilityType, 
            CancellationToken cancellationToken = default)
        {
            var entities = await _unitOfWork.Repository<RAM>()
                .GetAllAsync(
                    filter: r => r.ProjectId == projectId && 
                                r.ResponsibilityType == responsibilityType.ToUpper() && 
                                r.IsActive,
                    includeProperties: "Project,WBSElement,OBSNode,ControlAccount",
                    orderBy: q => q.OrderBy(r => r.WBSElement.Code).ThenBy(r => r.OBSNode.Code),
                    cancellationToken: cancellationToken);

            return entities.Select(r =>
            {
                var dto = _mapper.Map<RAMDto>(r);
                MapComputedProperties(dto, r);
                return dto;
            });
        }

        /// <summary>
        /// Updates allocation for RAM assignment
        /// </summary>
        public async Task<RAMDto?> UpdateAllocationAsync(
            Guid id, 
            UpdateRAMAllocationDto dto, 
            string? updatedBy = null, 
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<RAM>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null)
                return null;

            // Map DTO properties to entity properties
            decimal? hours = dto.PlannedManHours;
            decimal? percentage = dto.AllocationPercentage;

            entity.UpdateAllocation(hours, percentage);
            entity.UpdatedBy = updatedBy ?? "System";
            entity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<RAM>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("RAM assignment {Id} allocation updated by {User}",
                id, updatedBy ?? "System");

            // Reload with navigation properties
            entity = await _unitOfWork.Repository<RAM>()
                .GetAsync(
                    filter: r => r.Id == id,
                    includeProperties: "Project,WBSElement,OBSNode,ControlAccount",
                    cancellationToken: cancellationToken);

            var resultDto = _mapper.Map<RAMDto>(entity);
            MapComputedProperties(resultDto, entity);
            return resultDto;
        }

        /// <summary>
        /// Sets period for RAM assignment
        /// </summary>
        public async Task<RAMDto?> SetPeriodAsync(
            Guid id, 
            DateTime startDate, 
            DateTime endDate, 
            string? updatedBy = null, 
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<RAM>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null)
                return null;

            entity.SetPeriod(startDate, endDate);
            entity.UpdatedBy = updatedBy ?? "System";
            entity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<RAM>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("RAM assignment {Id} period set by {User}",
                id, updatedBy ?? "System");

            // Reload with navigation properties
            entity = await _unitOfWork.Repository<RAM>()
                .GetAsync(
                    filter: r => r.Id == id,
                    includeProperties: "Project,WBSElement,OBSNode,ControlAccount",
                    cancellationToken: cancellationToken);

            var dto = _mapper.Map<RAMDto>(entity);
            MapComputedProperties(dto, entity);
            return dto;
        }

        /// <summary>
        /// Links RAM assignment to control account
        /// </summary>
        public async Task<RAMDto?> LinkToControlAccountAsync(
            Guid id, 
            Guid controlAccountId, 
            string? updatedBy = null, 
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<RAM>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null)
                return null;

            // Validate control account exists
            var controlAccountExists = await _unitOfWork.Repository<ControlAccount>()
                .GetByIdAsync(controlAccountId, cancellationToken) != null;

            if (!controlAccountExists)
                throw new InvalidOperationException($"Control account {controlAccountId} not found");

            entity.LinkToControlAccount(controlAccountId);
            entity.UpdatedBy = updatedBy ?? "System";
            entity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<RAM>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("RAM assignment {Id} linked to control account {ControlAccountId} by {User}",
                id, controlAccountId, updatedBy ?? "System");

            // Reload with navigation properties
            entity = await _unitOfWork.Repository<RAM>()
                .GetAsync(
                    filter: r => r.Id == id,
                    includeProperties: "Project,WBSElement,OBSNode,ControlAccount",
                    cancellationToken: cancellationToken);

            var dto = _mapper.Map<RAMDto>(entity);
            MapComputedProperties(dto, entity);
            return dto;
        }

        /// <summary>
        /// Gets RAM matrix for project
        /// </summary>
        public async Task<RAMMatrixDto> GetRAMMatrixAsync(Guid projectId, CancellationToken cancellationToken = default)
        {
            // Get all RAM assignments for the project
            var assignments = await _unitOfWork.Repository<RAM>()
                .GetAllAsync(
                    filter: r => r.ProjectId == projectId && r.IsActive,
                    includeProperties: "WBSElement,OBSNode",
                    cancellationToken: cancellationToken);

            var assignmentsList = assignments.ToList();

            // Get unique WBS elements
            var wbsElements = assignmentsList
                .Select(a => a.WBSElement)
                .Where(w => w != null)
                .DistinctBy(w => w.Id)
                .OrderBy(w => w.Code)
                .Select(w => new RAMMatrixWBSDto
                {
                    Id = w.Id,
                    Code = w.Code,
                    Name = w.Name
                })
                .ToList();

            // Get unique OBS nodes
            var obsNodes = assignmentsList
                .Select(a => a.OBSNode)
                .Where(o => o != null)
                .DistinctBy(o => o.Id)
                .OrderBy(o => o.Code)
                .Select(o => new RAMMatrixOBSDto
                {
                    Id = o.Id,
                    Code = o.Code,
                    Name = o.Name
                })
                .ToList();

            // Map assignments
            var matrixAssignments = assignmentsList
                .Select(a => new RAMMatrixAssignmentDto
                {
                    WBSElementId = a.WBSElementId,
                    OBSNodeId = a.OBSNodeId,
                    ResponsibilityType = a.ResponsibilityType,
                    AllocationPercentage = a.AllocatedPercentage ?? 0
                })
                .ToList();

            return new RAMMatrixDto
            {
                ProjectId = projectId,
                WBSElements = wbsElements,
                OBSNodes = obsNodes,
                Assignments = matrixAssignments
            };
        }

        /// <summary>
        /// Gets accountable assignments for WBS element
        /// </summary>
        public async Task<IEnumerable<RAMDto>> GetAccountableAssignmentsAsync(Guid wbsElementId, CancellationToken cancellationToken = default)
        {
            var entities = await _unitOfWork.Repository<RAM>()
                .GetAllAsync(
                    filter: r => r.WBSElementId == wbsElementId && 
                                r.ResponsibilityType == RAM.ResponsibilityTypes.Accountable &&
                                r.IsActive,
                    includeProperties: "Project,WBSElement,OBSNode,ControlAccount",
                    cancellationToken: cancellationToken);

            return entities.Select(r =>
            {
                var dto = _mapper.Map<RAMDto>(r);
                MapComputedProperties(dto, r);
                return dto;
            });
        }

        /// <summary>
        /// Validates RACI assignments (ensures single Accountable per WBS)
        /// </summary>
        public async Task<RAMValidationResult> ValidateAssignmentsAsync(Guid projectId, CancellationToken cancellationToken = default)
        {
            var result = new RAMValidationResult { IsValid = true };

            // Get all assignments for the project
            var assignments = await _unitOfWork.Repository<RAM>()
                .GetAllAsync(
                    filter: r => r.ProjectId == projectId && r.IsActive,
                    includeProperties: "WBSElement,OBSNode",
                    cancellationToken: cancellationToken);

            var assignmentsList = assignments.ToList();

            // Group by WBS element
            var wbsGroups = assignmentsList.GroupBy(a => a.WBSElementId);

            foreach (var wbsGroup in wbsGroups)
            {
                var wbsElement = wbsGroup.First().WBSElement;
                var accountableCount = wbsGroup.Count(a => a.ResponsibilityType == RAM.ResponsibilityTypes.Accountable);

                if (accountableCount == 0)
                {
                    result.IsValid = false;
                    result.Errors.Add($"WBS element '{wbsElement?.Code} - {wbsElement?.Name}' has no Accountable assignment");
                }
                else if (accountableCount > 1)
                {
                    result.IsValid = false;
                    result.Errors.Add($"WBS element '{wbsElement?.Code} - {wbsElement?.Name}' has {accountableCount} Accountable assignments (should have only 1)");
                }

                // Check if there's at least one Responsible
                var responsibleCount = wbsGroup.Count(a => a.ResponsibilityType == RAM.ResponsibilityTypes.Responsible);
                if (responsibleCount == 0)
                {
                    result.IsValid = false;
                    result.Errors.Add($"WBS element '{wbsElement?.Code} - {wbsElement?.Name}' has no Responsible assignment");
                }
            }

            return result;
        }

        /// <summary>
        /// Bulk creates RAM assignments
        /// </summary>
        public async Task<IEnumerable<RAMDto>> BulkCreateAsync(
            IEnumerable<CreateRAMDto> assignments, 
            string? createdBy = null, 
            CancellationToken cancellationToken = default)
        {
            var createdEntities = new List<RAM>();

            foreach (var dto in assignments)
            {
                // Validate entities exist
                var projectExists = await _unitOfWork.Repository<Project>()
                    .GetByIdAsync(dto.ProjectId, cancellationToken) != null;
                
                if (!projectExists)
                    throw new InvalidOperationException($"Project {dto.ProjectId} not found");

                var wbsExists = await _unitOfWork.Repository<WBSElement>()
                    .GetByIdAsync(dto.WBSElementId, cancellationToken) != null;
                
                if (!wbsExists)
                    throw new InvalidOperationException($"WBS element {dto.WBSElementId} not found");

                var obsExists = await _unitOfWork.Repository<OBSNode>()
                    .GetByIdAsync(dto.OBSNodeId, cancellationToken) != null;
                
                if (!obsExists)
                    throw new InvalidOperationException($"OBS node {dto.OBSNodeId} not found");

                // Check for existing assignment
                var existing = await _unitOfWork.Repository<RAM>()
                    .GetAsync(
                        filter: r => r.ProjectId == dto.ProjectId &&
                                    r.WBSElementId == dto.WBSElementId &&
                                    r.OBSNodeId == dto.OBSNodeId &&
                                    r.IsActive,
                        cancellationToken: cancellationToken);

                if (existing != null)
                {
                    _logger.LogWarning("RAM assignment already exists for WBS {WBSElementId} and OBS {OBSNodeId}",
                        dto.WBSElementId, dto.OBSNodeId);
                    continue;
                }

                var entity = new RAM(
                    dto.ProjectId,
                    dto.WBSElementId,
                    dto.OBSNodeId,
                    dto.ResponsibilityType);

                // Set allocation
                if (dto.PlannedManHours > 0 || dto.AllocationPercentage > 0)
                {
                    entity.UpdateAllocation(dto.PlannedManHours, dto.AllocationPercentage);
                }

                // Set period
                if (dto.StartDate.HasValue && dto.EndDate.HasValue)
                {
                    entity.SetPeriod(dto.StartDate.Value, dto.EndDate.Value);
                }

                // Link to control account
                if (dto.ControlAccountId.HasValue)
                {
                    entity.LinkToControlAccount(dto.ControlAccountId.Value);
                }

                // Add notes
                if (!string.IsNullOrWhiteSpace(dto.Notes))
                {
                    entity.AddNotes(dto.Notes);
                }

                entity.CreatedBy = createdBy ?? "System";
                entity.CreatedAt = DateTime.UtcNow;

                await _unitOfWork.Repository<RAM>().AddAsync(entity);
                createdEntities.Add(entity);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("{Count} RAM assignments created by {User}",
                createdEntities.Count, createdBy ?? "System");

            // Reload with navigation properties
            var entityIds = createdEntities.Select(e => e.Id).ToList();
            var reloadedEntities = await _unitOfWork.Repository<RAM>()
                .GetAllAsync(
                    filter: r => entityIds.Contains(r.Id),
                    includeProperties: "Project,WBSElement,OBSNode,ControlAccount",
                    cancellationToken: cancellationToken);

            return reloadedEntities.Select(r =>
            {
                var dto = _mapper.Map<RAMDto>(r);
                MapComputedProperties(dto, r);
                return dto;
            });
        }

        /// <summary>
        /// Copies RAM assignments from template
        /// </summary>
        public async Task<IEnumerable<RAMDto>> CopyFromTemplateAsync(
            Guid sourceProjectId, 
            Guid targetProjectId, 
            string? createdBy = null, 
            CancellationToken cancellationToken = default)
        {
            // Get all assignments from source project
            var sourceAssignments = await _unitOfWork.Repository<RAM>()
                .GetAllAsync(
                    filter: r => r.ProjectId == sourceProjectId && r.IsActive,
                    includeProperties: "WBSElement,OBSNode",
                    cancellationToken: cancellationToken);

            var createdEntities = new List<RAM>();

            foreach (var source in sourceAssignments)
            {
                // Find corresponding WBS element in target project
                var targetWBS = await _unitOfWork.Repository<WBSElement>()
                    .GetAsync(
                        filter: w => w.ProjectId == targetProjectId && w.Code == source.WBSElement.Code,
                        cancellationToken: cancellationToken);

                if (targetWBS == null)
                {
                    _logger.LogWarning("WBS element {WBSCode} not found in target project {ProjectId}",
                        source.WBSElement.Code, targetProjectId);
                    continue;
                }

                // Find corresponding OBS node
                var targetOBS = await _unitOfWork.Repository<OBSNode>()
                    .GetAsync(
                        filter: o => o.ProjectId == targetProjectId && o.Code == source.OBSNode.Code,
                        cancellationToken: cancellationToken);

                if (targetOBS == null)
                {
                    _logger.LogWarning("OBS node {OBSCode} not found in target project {ProjectId}",
                        source.OBSNode.Code, targetProjectId);
                    continue;
                }

                // Check if assignment already exists
                var existing = await _unitOfWork.Repository<RAM>()
                    .GetAsync(
                        filter: r => r.ProjectId == targetProjectId &&
                                    r.WBSElementId == targetWBS.Id &&
                                    r.OBSNodeId == targetOBS.Id &&
                                    r.IsActive,
                        cancellationToken: cancellationToken);

                if (existing != null)
                    continue;

                var entity = new RAM(
                    targetProjectId,
                    targetWBS.Id,
                    targetOBS.Id,
                    source.ResponsibilityType);

                // Copy allocation
                if (source.AllocatedHours.HasValue || source.AllocatedPercentage.HasValue)
                {
                    entity.UpdateAllocation(source.AllocatedHours, source.AllocatedPercentage);
                }

                // Copy period
                if (source.StartDate.HasValue && source.EndDate.HasValue)
                {
                    entity.SetPeriod(source.StartDate.Value, source.EndDate.Value);
                }

                // Copy notes
                if (!string.IsNullOrWhiteSpace(source.Notes))
                {
                    entity.AddNotes($"Copied from template: {source.Notes}");
                }

                entity.CreatedBy = createdBy ?? "System";
                entity.CreatedAt = DateTime.UtcNow;

                await _unitOfWork.Repository<RAM>().AddAsync(entity);
                createdEntities.Add(entity);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("{Count} RAM assignments copied from project {SourceProjectId} to {TargetProjectId} by {User}",
                createdEntities.Count, sourceProjectId, targetProjectId, createdBy ?? "System");

            // Reload with navigation properties
            var entityIds = createdEntities.Select(e => e.Id).ToList();
            var reloadedEntities = await _unitOfWork.Repository<RAM>()
                .GetAllAsync(
                    filter: r => entityIds.Contains(r.Id),
                    includeProperties: "Project,WBSElement,OBSNode,ControlAccount",
                    cancellationToken: cancellationToken);

            return reloadedEntities.Select(r =>
            {
                var dto = _mapper.Map<RAMDto>(r);
                MapComputedProperties(dto, r);
                return dto;
            });
        }

        /// <summary>
        /// Gets resource allocation summary
        /// </summary>
        public async Task<ResourceAllocationSummaryDto> GetResourceAllocationSummaryAsync(
            Guid obsNodeId, 
            CancellationToken cancellationToken = default)
        {
            var assignments = await _unitOfWork.Repository<RAM>()
                .GetAllAsync(
                    filter: r => r.OBSNodeId == obsNodeId && r.IsActive,
                    includeProperties: "Project,WBSElement",
                    cancellationToken: cancellationToken);

            var assignmentsList = assignments.ToList();

            var summary = new ResourceAllocationSummaryDto
            {
                OBSNodeId = obsNodeId,
                TotalAssignments = assignmentsList.Count,
                TotalPlannedManHours = assignmentsList.Sum(a => a.AllocatedHours ?? 0),
                TotalPlannedCost = 0, // Entity doesn't have planned cost
                AverageAllocation = assignmentsList.Any() 
                    ? assignmentsList.Average(a => a.AllocatedPercentage ?? 0) : 0
            };

            // Group by responsibility type
            var typeGroups = assignmentsList.GroupBy(a => a.ResponsibilityType);
            foreach (var typeGroup in typeGroups)
            {
                summary.AssignmentsByType.Add(new ResponsibilityTypeSummaryDto
                {
                    ResponsibilityType = typeGroup.Key,
                    Count = typeGroup.Count(),
                    TotalAllocation = typeGroup.Sum(a => a.AllocatedPercentage ?? 0)
                });
            }

            // Group by WBS element
            var wbsGroups = assignmentsList.GroupBy(a => a.WBSElementId);
            foreach (var wbsGroup in wbsGroups)
            {
                var wbsElement = wbsGroup.First().WBSElement;
                var responsibilityTypes = string.Join(",", wbsGroup.Select(a => a.ResponsibilityType).Distinct());
                
                summary.WBSAssignments.Add(new WBSAllocationSummaryDto
                {
                    WBSElementId = wbsGroup.Key,
                    WBSCode = wbsElement?.Code ?? string.Empty,
                    WBSName = wbsElement?.Name ?? string.Empty,
                    ResponsibilityTypes = responsibilityTypes,
                    TotalAllocation = wbsGroup.Sum(a => a.AllocatedPercentage ?? 0)
                });
            }

            return summary;
        }

        /// <summary>
        /// Checks for conflicts in assignments
        /// </summary>
        public async Task<IEnumerable<RAMConflictDto>> CheckConflictsAsync(Guid projectId, CancellationToken cancellationToken = default)
        {
            var conflicts = new List<RAMConflictDto>();

            var assignments = await _unitOfWork.Repository<RAM>()
                .GetAllAsync(
                    filter: r => r.ProjectId == projectId && r.IsActive,
                    includeProperties: "WBSElement,OBSNode",
                    cancellationToken: cancellationToken);

            var assignmentsList = assignments.ToList();

            // Check for multiple Accountable per WBS
            var wbsGroups = assignmentsList.GroupBy(a => a.WBSElementId);
            foreach (var wbsGroup in wbsGroups)
            {
                var accountableAssignments = wbsGroup
                    .Where(a => a.ResponsibilityType == RAM.ResponsibilityTypes.Accountable)
                    .ToList();

                if (accountableAssignments.Count > 1)
                {
                    conflicts.Add(new RAMConflictDto
                    {
                        Type = "MultipleAccountable",
                        WBSElementId = wbsGroup.Key,
                        Description = $"WBS element '{wbsGroup.First().WBSElement?.Code} - {wbsGroup.First().WBSElement?.Name}' has {accountableAssignments.Count} Accountable assignments",
                        Severity = "High"
                    });
                }
            }

            // Check for overallocation (total allocation > 100% for OBS node)
            var obsGroups = assignmentsList.GroupBy(a => a.OBSNodeId);
            foreach (var obsGroup in obsGroups)
            {
                var totalAllocation = obsGroup.Sum(a => a.AllocatedPercentage ?? 0);
                if (totalAllocation > 100)
                {
                    conflicts.Add(new RAMConflictDto
                    {
                        Type = "OverAllocation",
                        OBSNodeId = obsGroup.Key,
                        Description = $"OBS node '{obsGroup.First().OBSNode?.Code} - {obsGroup.First().OBSNode?.Name}' total allocation is {totalAllocation}% (exceeds 100%)",
                        Severity = "Medium"
                    });
                }
            }

            return conflicts;
        }

        /// <summary>
        /// Override create to handle entity creation properly
        /// </summary>
        public override async Task<RAMDto> CreateAsync(
            CreateRAMDto createDto,
            string? createdBy = null,
            CancellationToken cancellationToken = default)
        {
            // Validate entities exist
            var projectExists = await _unitOfWork.Repository<Project>()
                .GetByIdAsync(createDto.ProjectId, cancellationToken) != null;
            
            if (!projectExists)
                throw new InvalidOperationException($"Project {createDto.ProjectId} not found");

            var wbsExists = await _unitOfWork.Repository<WBSElement>()
                .GetByIdAsync(createDto.WBSElementId, cancellationToken) != null;
            
            if (!wbsExists)
                throw new InvalidOperationException($"WBS element {createDto.WBSElementId} not found");

            var obsExists = await _unitOfWork.Repository<OBSNode>()
                .GetByIdAsync(createDto.OBSNodeId, cancellationToken) != null;
            
            if (!obsExists)
                throw new InvalidOperationException($"OBS node {createDto.OBSNodeId} not found");

            // Check for existing assignment
            var existing = await _unitOfWork.Repository<RAM>()
                .GetAsync(
                    filter: r => r.ProjectId == createDto.ProjectId &&
                                r.WBSElementId == createDto.WBSElementId &&
                                r.OBSNodeId == createDto.OBSNodeId &&
                                r.IsActive,
                    cancellationToken: cancellationToken);

            if (existing != null)
                throw new InvalidOperationException("RAM assignment already exists for this WBS and OBS combination");

            var entity = new RAM(
                createDto.ProjectId,
                createDto.WBSElementId,
                createDto.OBSNodeId,
                createDto.ResponsibilityType);

            // Set allocation
            if (createDto.PlannedManHours > 0 || createDto.AllocationPercentage > 0)
            {
                entity.UpdateAllocation(createDto.PlannedManHours, createDto.AllocationPercentage);
            }

            // Set period
            if (createDto.StartDate.HasValue && createDto.EndDate.HasValue)
            {
                entity.SetPeriod(createDto.StartDate.Value, createDto.EndDate.Value);
            }

            // Link to control account
            if (createDto.ControlAccountId.HasValue)
            {
                entity.LinkToControlAccount(createDto.ControlAccountId.Value);
            }

            // Add notes
            if (!string.IsNullOrWhiteSpace(createDto.Notes))
            {
                entity.AddNotes(createDto.Notes);
            }

            entity.CreatedBy = createdBy ?? "System";
            entity.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.Repository<RAM>().AddAsync(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("RAM assignment created for WBS {WBSElementId} and OBS {OBSNodeId} by {User}",
                createDto.WBSElementId, createDto.OBSNodeId, createdBy ?? "System");

            // Reload with navigation properties
            entity = await _unitOfWork.Repository<RAM>()
                .GetAsync(
                    filter: r => r.Id == entity.Id,
                    includeProperties: "Project,WBSElement,OBSNode,ControlAccount",
                    cancellationToken: cancellationToken);

            var dto = _mapper.Map<RAMDto>(entity);
            MapComputedProperties(dto, entity);
            return dto;
        }

        /// <summary>
        /// Override update to use entity methods
        /// </summary>
        public override async Task<RAMDto?> UpdateAsync(
            Guid id,
            UpdateRAMDto updateDto,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<RAM>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null)
                return null;

            // Update responsibility type
            if (!string.IsNullOrWhiteSpace(updateDto.ResponsibilityType) && 
                updateDto.ResponsibilityType != entity.ResponsibilityType)
            {
                // Re-create entity with new responsibility type since it's set in constructor
                var newEntity = new RAM(
                    entity.ProjectId,
                    entity.WBSElementId,
                    entity.OBSNodeId,
                    updateDto.ResponsibilityType);

                // Copy all other properties
                newEntity.UpdateAllocation(updateDto.PlannedManHours, updateDto.AllocationPercentage);
                
                if (updateDto.StartDate.HasValue && updateDto.EndDate.HasValue)
                {
                    newEntity.SetPeriod(updateDto.StartDate.Value, updateDto.EndDate.Value);
                }
                
                if (updateDto.ControlAccountId.HasValue)
                {
                    newEntity.LinkToControlAccount(updateDto.ControlAccountId.Value);
                }
                
                if (!string.IsNullOrWhiteSpace(updateDto.Notes))
                {
                    newEntity.AddNotes(updateDto.Notes);
                }

                newEntity.IsActive = updateDto.IsActive;
                newEntity.CreatedBy = entity.CreatedBy;
                newEntity.CreatedAt = entity.CreatedAt;
                newEntity.UpdatedBy = updatedBy ?? "System";
                newEntity.UpdatedAt = DateTime.UtcNow;

                // Remove old entity and add new one
                _unitOfWork.Repository<RAM>().Remove(entity);
                await _unitOfWork.Repository<RAM>().AddAsync(newEntity);
            }
            else
            {
                // Update allocation
                entity.UpdateAllocation(updateDto.PlannedManHours, updateDto.AllocationPercentage);

                // Update period
                if (updateDto.StartDate.HasValue && updateDto.EndDate.HasValue)
                {
                    entity.SetPeriod(updateDto.StartDate.Value, updateDto.EndDate.Value);
                }

                // Update control account link
                if (updateDto.ControlAccountId.HasValue && updateDto.ControlAccountId != entity.ControlAccountId)
                {
                    entity.LinkToControlAccount(updateDto.ControlAccountId.Value);
                }

                // Update notes
                if (updateDto.Notes != entity.Notes)
                {
                    entity.AddNotes(updateDto.Notes);
                }

                // Update active status
                entity.IsActive = updateDto.IsActive;
                entity.UpdatedBy = updatedBy ?? "System";
                entity.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Repository<RAM>().Update(entity);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("RAM assignment {Id} updated by {User}",
                id, updatedBy ?? "System");

            // Reload with navigation properties
            var reloadedEntity = await _unitOfWork.Repository<RAM>()
                .GetAsync(
                    filter: r => r.Id == id || 
                               (r.ProjectId == entity.ProjectId && 
                                r.WBSElementId == entity.WBSElementId && 
                                r.OBSNodeId == entity.OBSNodeId && 
                                r.IsActive),
                    includeProperties: "Project,WBSElement,OBSNode,ControlAccount",
                    cancellationToken: cancellationToken);

            var dto = _mapper.Map<RAMDto>(reloadedEntity);
            MapComputedProperties(dto, reloadedEntity);
            return dto;
        }

        /// <summary>
        /// Override GetByIdAsync to include navigation properties
        /// </summary>
        public override async Task<RAMDto?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<RAM>()
                .GetAsync(
                    filter: r => r.Id == id,
                    includeProperties: "Project,WBSElement,OBSNode,ControlAccount",
                    cancellationToken: cancellationToken);

            if (entity == null)
                return null;

            var dto = _mapper.Map<RAMDto>(entity);
            MapComputedProperties(dto, entity);
            return dto;
        }

        /// <summary>
        /// Override GetAllAsync to include navigation properties
        /// </summary>
        public override async Task<IEnumerable<RAMDto>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            var entities = await _unitOfWork.Repository<RAM>()
                .GetAllAsync(
                    filter: r => r.IsActive,
                    includeProperties: "Project,WBSElement,OBSNode,ControlAccount",
                    orderBy: q => q.OrderBy(r => r.Project.Code)
                                   .ThenBy(r => r.WBSElement.Code)
                                   .ThenBy(r => r.OBSNode.Code),
                    cancellationToken: cancellationToken);

            return entities.Select(r =>
            {
                var dto = _mapper.Map<RAMDto>(r);
                MapComputedProperties(dto, r);
                return dto;
            });
        }

        /// <summary>
        /// Override GetBaseFilter to filter by IsActive
        /// </summary>
        protected override Expression<Func<RAM, bool>> GetBaseFilter()
        {
            return entity => entity.IsActive;
        }

        /// <summary>
        /// Maps computed properties from entity to DTO
        /// </summary>
        private void MapComputedProperties(RAMDto dto, RAM entity)
        {
            // Navigation property names
            dto.ProjectName = entity.Project?.Name ?? string.Empty;
            dto.WBSCode = entity.WBSElement?.Code ?? string.Empty;
            dto.WBSName = entity.WBSElement?.Name ?? string.Empty;
            dto.OBSCode = entity.OBSNode?.Code ?? string.Empty;
            dto.OBSName = entity.OBSNode?.Name ?? string.Empty;
            dto.ControlAccountCode = entity.ControlAccount?.Code;

            // Map entity properties to DTO properties
            dto.AllocationPercentage = entity.AllocatedPercentage ?? 0;
            dto.PlannedManHours = entity.AllocatedHours ?? 0;
            dto.PlannedCost = 0; // Entity doesn't have PlannedCost
            
            // Get responsibility description
            dto.ResponsibilityDescription = RAM.ResponsibilityTypes.GetDescription(entity.ResponsibilityType);
        }

        /// <summary>
        /// Gets the unit of work instance
        /// </summary>
        public IUnitOfWork UnitOfWork => _unitOfWork;
    }
}