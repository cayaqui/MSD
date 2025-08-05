using Application.Interfaces.Organization;
using Application.Services.Base;
using Core.Enums.Projects;

namespace Application.Services.Organization
{
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

        /// <summary>
        /// Gets phases by project
        /// </summary>
        public async Task<IEnumerable<PhaseDto>> GetByProjectAsync(Guid projectId, CancellationToken cancellationToken = default)
        {
            var phases = await _unitOfWork.Repository<Phase>()
                .GetAllAsync(
                    filter: p => p.ProjectId == projectId && !p.IsDeleted,
                    includeProperties: "Project",
                    orderBy: q => q.OrderBy(p => p.SequenceNumber),
                    cancellationToken: cancellationToken);

            var dtos = phases.Select(p =>
            {
                var dto = _mapper.Map<PhaseDto>(p);
                MapComputedProperties(dto, p);
                return dto;
            });

            return dtos;
        }

        /// <summary>
        /// Gets phases by type
        /// </summary>
        public async Task<IEnumerable<PhaseDto>> GetByTypeAsync(PhaseType type, CancellationToken cancellationToken = default)
        {
            var phases = await _unitOfWork.Repository<Phase>()
                .GetAllAsync(
                    filter: p => p.PhaseType == type && !p.IsDeleted && p.IsActive,
                    includeProperties: "Project",
                    orderBy: q => q.OrderBy(p => p.ProjectId).ThenBy(p => p.SequenceNumber),
                    cancellationToken: cancellationToken);

            var dtos = phases.Select(p =>
            {
                var dto = _mapper.Map<PhaseDto>(p);
                MapComputedProperties(dto, p);
                return dto;
            });

            return dtos;
        }

        /// <summary>
        /// Gets phases by status
        /// </summary>
        public async Task<IEnumerable<PhaseDto>> GetByStatusAsync(PhaseStatus status, CancellationToken cancellationToken = default)
        {
            var phases = await _unitOfWork.Repository<Phase>()
                .GetAllAsync(
                    filter: p => p.Status == status && !p.IsDeleted && p.IsActive,
                    includeProperties: "Project",
                    orderBy: q => q.OrderBy(p => p.ProjectId).ThenBy(p => p.SequenceNumber),
                    cancellationToken: cancellationToken);

            var dtos = phases.Select(p =>
            {
                var dto = _mapper.Map<PhaseDto>(p);
                MapComputedProperties(dto, p);
                return dto;
            });

            return dtos;
        }

        /// <summary>
        /// Starts a phase
        /// </summary>
        public async Task<PhaseDto?> StartPhaseAsync(
            Guid id,
            string? userId = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Phase>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null || entity.IsDeleted)
                return null;

            entity.Start(userId ?? "System");

            _unitOfWork.Repository<Phase>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Phase {PhaseCode} started by {User}",
                entity.Code, userId ?? "System");

            // Reload with navigation properties
            entity = await _unitOfWork.Repository<Phase>()
                .GetAsync(
                    filter: p => p.Id == id,
                    includeProperties: "Project",
                    cancellationToken: cancellationToken);

            var dto = _mapper.Map<PhaseDto>(entity);
            MapComputedProperties(dto, entity);
            
            return dto;
        }

        /// <summary>
        /// Completes a phase
        /// </summary>
        public async Task<PhaseDto?> CompletePhaseAsync(
            Guid id,
            string? userId = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Phase>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null || entity.IsDeleted)
                return null;

            entity.Complete(userId ?? "System");

            _unitOfWork.Repository<Phase>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Phase {PhaseCode} completed by {User}",
                entity.Code, userId ?? "System");

            // Reload with navigation properties
            entity = await _unitOfWork.Repository<Phase>()
                .GetAsync(
                    filter: p => p.Id == id,
                    includeProperties: "Project",
                    cancellationToken: cancellationToken);

            var dto = _mapper.Map<PhaseDto>(entity);
            MapComputedProperties(dto, entity);
            
            return dto;
        }

        /// <summary>
        /// Approves phase gate
        /// </summary>
        public async Task<PhaseDto?> ApproveGateAsync(
            Guid id,
            string? approvedBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Phase>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null || entity.IsDeleted)
                return null;

            entity.ApproveGate(approvedBy ?? "System");

            _unitOfWork.Repository<Phase>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Phase {PhaseCode} gate approved by {User}",
                entity.Code, approvedBy ?? "System");

            // Reload with navigation properties
            entity = await _unitOfWork.Repository<Phase>()
                .GetAsync(
                    filter: p => p.Id == id,
                    includeProperties: "Project",
                    cancellationToken: cancellationToken);

            var dto = _mapper.Map<PhaseDto>(entity);
            MapComputedProperties(dto, entity);
            
            return dto;
        }

        /// <summary>
        /// Updates phase progress
        /// </summary>
        public async Task<PhaseDto?> UpdateProgressAsync(
            Guid id,
            UpdatePhaseProgressDto dto,
            string? userId = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Phase>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null || entity.IsDeleted)
                return null;

            entity.UpdateProgress(dto.ProgressPercentage, dto.ActualCost, userId ?? "System");
            entity.CommittedCost = dto.CommittedCost;

            _unitOfWork.Repository<Phase>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Phase {PhaseCode} progress updated to {Progress}% by {User}",
                entity.Code, dto.ProgressPercentage, userId ?? "System");

            // Reload with navigation properties
            entity = await _unitOfWork.Repository<Phase>()
                .GetAsync(
                    filter: p => p.Id == id,
                    includeProperties: "Project",
                    cancellationToken: cancellationToken);

            var resultDto = _mapper.Map<PhaseDto>(entity);
            MapComputedProperties(resultDto, entity);
            
            return resultDto;
        }

        /// <summary>
        /// Gets phase with details
        /// </summary>
        public async Task<PhaseDetailDto?> GetPhaseDetailAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Phase>()
                .GetAsync(
                    filter: p => p.Id == id && !p.IsDeleted,
                    includeProperties: "Project,WBSElements,ControlAccounts,Milestones,PlanningPackages",
                    cancellationToken: cancellationToken);

            if (entity == null)
                return null;

            var dto = _mapper.Map<PhaseDetailDto>(entity);
            MapComputedProperties(dto, entity);

            // Map additional detail properties
            dto.WBSElementCount = entity.WBSElements?.Count(w => !w.IsDeleted) ?? 0;
            dto.ControlAccountCount = entity.ControlAccounts?.Count(c => !c.IsDeleted) ?? 0;
            dto.MilestoneCount = entity.Milestones?.Count(m => !m.IsDeleted) ?? 0;
            dto.CompletedMilestones = entity.Milestones?.Count(m => !m.IsDeleted && m.IsCompleted) ?? 0;
            dto.PlanningPackageCount = entity.PlanningPackages?.Count(p => !p.IsDeleted) ?? 0;

            // Financial details
            dto.BudgetVariance = entity.BudgetVariance();
            dto.BudgetVariancePercentage = entity.BudgetVariancePercentage();
            dto.CostPerformanceIndex = entity.ApprovedBudget > 0 ? entity.ActualCost / entity.ApprovedBudget : 1;

            // Schedule details
            dto.PlannedDuration = (entity.PlannedEndDate - entity.PlannedStartDate).Days;
            dto.ActualDuration = entity.ActualStartDate.HasValue 
                ? (entity.ActualEndDate ?? DateTime.UtcNow).Subtract(entity.ActualStartDate.Value).Days 
                : null;
            dto.DaysRemaining = entity.Status != PhaseStatus.Completed && entity.PlannedEndDate > DateTime.UtcNow
                ? (entity.PlannedEndDate - DateTime.UtcNow).Days
                : null;
            dto.SchedulePerformanceIndex = CalculateSchedulePerformanceIndex(entity);

            // Resource summary (simplified - would need actual resource tracking)
            dto.AssignedResources = 0; // TODO: Get from resource assignments
            dto.PlannedEffort = 0; // TODO: Get from resource planning
            dto.ActualEffort = 0; // TODO: Get from timesheet data
            
            return dto;
        }

        /// <summary>
        /// Gets phases requiring gate approval
        /// </summary>
        public async Task<IEnumerable<PhaseDto>> GetPendingGateApprovalsAsync(Guid projectId, CancellationToken cancellationToken = default)
        {
            var phases = await _unitOfWork.Repository<Phase>()
                .GetAllAsync(
                    filter: p => p.ProjectId == projectId && 
                               !p.IsDeleted && 
                               p.IsActive &&
                               p.RequiresGateApproval &&
                               !p.GateApprovalDate.HasValue &&
                               p.Status == PhaseStatus.InProgress,
                    includeProperties: "Project",
                    orderBy: q => q.OrderBy(p => p.SequenceNumber),
                    cancellationToken: cancellationToken);

            var dtos = phases.Select(p =>
            {
                var dto = _mapper.Map<PhaseDto>(p);
                MapComputedProperties(dto, p);
                return dto;
            });

            return dtos;
        }

        /// <summary>
        /// Gets over budget phases
        /// </summary>
        public async Task<IEnumerable<PhaseDto>> GetOverBudgetPhasesAsync(Guid projectId, CancellationToken cancellationToken = default)
        {
            var phases = await _unitOfWork.Repository<Phase>()
                .GetAllAsync(
                    filter: p => p.ProjectId == projectId && 
                               !p.IsDeleted && 
                               p.IsActive &&
                               p.ActualCost > p.ApprovedBudget,
                    includeProperties: "Project",
                    orderBy: q => q.OrderBy(p => p.SequenceNumber),
                    cancellationToken: cancellationToken);

            var dtos = phases.Select(p =>
            {
                var dto = _mapper.Map<PhaseDto>(p);
                MapComputedProperties(dto, p);
                return dto;
            });

            return dtos;
        }

        /// <summary>
        /// Gets delayed phases
        /// </summary>
        public async Task<IEnumerable<PhaseDto>> GetDelayedPhasesAsync(Guid projectId, CancellationToken cancellationToken = default)
        {
            var phases = await _unitOfWork.Repository<Phase>()
                .GetAllAsync(
                    filter: p => p.ProjectId == projectId && 
                               !p.IsDeleted && 
                               p.IsActive &&
                               ((p.ActualEndDate > p.PlannedEndDate) ||
                                (DateTime.UtcNow > p.PlannedEndDate && p.Status != PhaseStatus.Completed)),
                    includeProperties: "Project",
                    orderBy: q => q.OrderBy(p => p.SequenceNumber),
                    cancellationToken: cancellationToken);

            var dtos = phases.Select(p =>
            {
                var dto = _mapper.Map<PhaseDto>(p);
                MapComputedProperties(dto, p);
                return dto;
            });

            return dtos;
        }

        /// <summary>
        /// Updates phase sequence
        /// </summary>
        public async Task<IEnumerable<PhaseDto>> UpdateSequenceAsync(
            Guid projectId,
            IEnumerable<PhaseSequenceDto> sequences,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            var phases = await _unitOfWork.Repository<Phase>()
                .GetAllAsync(
                    filter: p => p.ProjectId == projectId && !p.IsDeleted,
                    cancellationToken: cancellationToken);

            foreach (var sequence in sequences)
            {
                var phase = phases.FirstOrDefault(p => p.Id == sequence.Id);
                if (phase != null)
                {
                    phase.SequenceNumber = sequence.SequenceNumber;
                    phase.UpdatedBy = updatedBy ?? "System";
                    phase.UpdatedAt = DateTime.UtcNow;
                    _unitOfWork.Repository<Phase>().Update(phase);
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Reload with proper ordering
            phases = await _unitOfWork.Repository<Phase>()
                .GetAllAsync(
                    filter: p => p.ProjectId == projectId && !p.IsDeleted,
                    includeProperties: "Project",
                    orderBy: q => q.OrderBy(p => p.SequenceNumber),
                    cancellationToken: cancellationToken);

            var dtos = phases.Select(p =>
            {
                var dto = _mapper.Map<PhaseDto>(p);
                MapComputedProperties(dto, p);
                return dto;
            });

            return dtos;
        }

        /// <summary>
        /// Calculates weighted project progress from phases
        /// </summary>
        public async Task<decimal> CalculateProjectProgressAsync(Guid projectId, CancellationToken cancellationToken = default)
        {
            var phases = await _unitOfWork.Repository<Phase>()
                .GetAllAsync(
                    filter: p => p.ProjectId == projectId && !p.IsDeleted && p.IsActive,
                    cancellationToken: cancellationToken);

            if (!phases.Any())
                return 0;

            var totalWeight = phases.Sum(p => p.WeightFactor);
            if (totalWeight == 0)
                return 0;

            var weightedProgress = phases.Sum(p => p.ProgressPercentage * p.WeightFactor);
            return weightedProgress / totalWeight;
        }

        /// <summary>
        /// Override create to handle entity creation properly
        /// </summary>
        public override async Task<PhaseDto> CreateAsync(
            CreatePhaseDto createDto,
            string? createdBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = new Phase(
                createDto.Code,
                createDto.Name,
                createDto.ProjectId,
                createDto.PhaseType,
                createDto.SequenceNumber);

            // Set basic properties
            entity.Description = createDto.Description;
            entity.PlannedStartDate = createDto.PlannedStartDate;
            entity.PlannedEndDate = createDto.PlannedEndDate;
            entity.PlannedBudget = createDto.PlannedBudget;
            entity.ApprovedBudget = createDto.ApprovedBudget;
            entity.WeightFactor = createDto.WeightFactor;
            entity.RequiresGateApproval = createDto.RequiresGateApproval;
            entity.KeyDeliverables = createDto.KeyDeliverables;

            entity.CreatedBy = createdBy ?? "System";
            entity.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.Repository<Phase>().AddAsync(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Phase {PhaseCode} created by {User}",
                entity.Code, createdBy ?? "System");

            // Reload with navigation properties
            entity = await _unitOfWork.Repository<Phase>()
                .GetAsync(
                    filter: p => p.Id == entity.Id,
                    includeProperties: "Project",
                    cancellationToken: cancellationToken);

            var dto = _mapper.Map<PhaseDto>(entity);
            MapComputedProperties(dto, entity);
            
            return dto;
        }

        /// <summary>
        /// Override update to use entity methods
        /// </summary>
        public override async Task<PhaseDto?> UpdateAsync(
            Guid id,
            UpdatePhaseDto updateDto,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Phase>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null || entity.IsDeleted)
                return null;

            // Update basic properties
            entity.Name = updateDto.Name;
            entity.Description = updateDto.Description;
            entity.PlannedStartDate = updateDto.PlannedStartDate;
            entity.PlannedEndDate = updateDto.PlannedEndDate;
            entity.PlannedBudget = updateDto.PlannedBudget;
            entity.ApprovedBudget = updateDto.ApprovedBudget;
            entity.WeightFactor = updateDto.WeightFactor;
            entity.RequiresGateApproval = updateDto.RequiresGateApproval;
            entity.KeyDeliverables = updateDto.KeyDeliverables;

            entity.UpdatedBy = updatedBy ?? "System";
            entity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Phase>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Phase {PhaseCode} updated by {User}",
                entity.Code, updatedBy ?? "System");

            // Reload with navigation properties
            entity = await _unitOfWork.Repository<Phase>()
                .GetAsync(
                    filter: p => p.Id == id,
                    includeProperties: "Project",
                    cancellationToken: cancellationToken);

            var dto = _mapper.Map<PhaseDto>(entity);
            MapComputedProperties(dto, entity);
            
            return dto;
        }

        /// <summary>
        /// Override GetByIdAsync to include navigation properties
        /// </summary>
        public override async Task<PhaseDto?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Phase>()
                .GetAsync(
                    filter: p => p.Id == id && !p.IsDeleted,
                    includeProperties: "Project",
                    cancellationToken: cancellationToken);

            if (entity == null)
                return null;

            var dto = _mapper.Map<PhaseDto>(entity);
            MapComputedProperties(dto, entity);
            
            return dto;
        }

        /// <summary>
        /// Override GetAllAsync to include navigation properties
        /// </summary>
        public override async Task<IEnumerable<PhaseDto>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            var entities = await _unitOfWork.Repository<Phase>()
                .GetAllAsync(
                    filter: p => !p.IsDeleted,
                    includeProperties: "Project",
                    orderBy: q => q.OrderBy(p => p.ProjectId).ThenBy(p => p.SequenceNumber),
                    cancellationToken: cancellationToken);

            var dtos = entities.Select(p =>
            {
                var dto = _mapper.Map<PhaseDto>(p);
                MapComputedProperties(dto, p);
                return dto;
            });

            return dtos;
        }

        /// <summary>
        /// Maps computed properties from entity to DTO
        /// </summary>
        private void MapComputedProperties(PhaseDto dto, Phase entity)
        {
            dto.IsOverBudget = entity.IsOverBudget();
            dto.IsDelayed = entity.IsDelayed();
            
            // Map navigation property names
            dto.ProjectName = entity.Project?.Name ?? string.Empty;
        }

        /// <summary>
        /// Calculates the schedule performance index
        /// </summary>
        private decimal CalculateSchedulePerformanceIndex(Phase entity)
        {
            if (!entity.ActualStartDate.HasValue)
                return 1;

            var plannedDuration = (entity.PlannedEndDate - entity.PlannedStartDate).TotalDays;
            var actualDuration = entity.ActualEndDate.HasValue
                ? (entity.ActualEndDate.Value - entity.ActualStartDate.Value).TotalDays
                : (DateTime.UtcNow - entity.ActualStartDate.Value).TotalDays;

            var plannedProgress = CalculatePlannedProgress(entity);
            return plannedProgress > 0 ? entity.ProgressPercentage / plannedProgress : 1;
        }

        /// <summary>
        /// Calculates the planned progress based on dates
        /// </summary>
        private decimal CalculatePlannedProgress(Phase entity)
        {
            if (DateTime.UtcNow <= entity.PlannedStartDate)
                return 0;
            
            if (DateTime.UtcNow >= entity.PlannedEndDate)
                return 100;
            
            var totalDays = (entity.PlannedEndDate - entity.PlannedStartDate).TotalDays;
            var elapsedDays = (DateTime.UtcNow - entity.PlannedStartDate).TotalDays;
            
            return totalDays > 0 ? (decimal)(elapsedDays / totalDays * 100) : 0;
        }

        /// <summary>
        /// Gets the unit of work instance
        /// </summary>
        public IUnitOfWork UnitOfWork => _unitOfWork;
    }
}