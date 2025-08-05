using Application.Interfaces.Organization;
using Application.Services.Base;
using Core.Enums.Projects;

namespace Application.Services.Organization
{
    /// <summary>
    /// Service implementation for Project management
    /// </summary>
    public class ProjectService : BaseService<Project, ProjectDto, CreateProjectDto, UpdateProjectDto>, IProjectService
    {
        public ProjectService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<ProjectService> logger)
            : base(unitOfWork, mapper, logger)
        {
        }

        /// <summary>
        /// Gets a project by its code
        /// </summary>
        public async Task<ProjectDto?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(code))
                return null;

            var entity = await _unitOfWork.Repository<Project>()
                .GetAsync(
                    filter: p => p.Code == code && !p.IsDeleted,
                    includeProperties: "Operation,ProjectTeamMembers,Phases",
                    cancellationToken: cancellationToken);

            if (entity == null)
                return null;

            var dto = _mapper.Map<ProjectDto>(entity);
            dto.TeamMemberCount = entity.ProjectTeamMembers?.Count ?? 0;
            dto.PhaseCount = entity.Phases?.Count ?? 0;
            
            return dto;
        }

        /// <summary>
        /// Gets projects by operation
        /// </summary>
        public async Task<IEnumerable<ProjectDto>> GetByOperationAsync(
            Guid operationId,
            CancellationToken cancellationToken = default)
        {
            var projects = await _unitOfWork.Repository<Project>()
                .GetAllAsync(
                    filter: p => p.OperationId == operationId && !p.IsDeleted,
                    includeProperties: "Operation,ProjectTeamMembers,Phases",
                    orderBy: q => q.OrderBy(p => p.Code),
                    cancellationToken: cancellationToken);

            var dtos = projects.Select(p =>
            {
                var dto = _mapper.Map<ProjectDto>(p);
                dto.TeamMemberCount = p.ProjectTeamMembers?.Count ?? 0;
                dto.PhaseCount = p.Phases?.Count ?? 0;
                return dto;
            });

            return dtos;
        }

        /// <summary>
        /// Gets projects by status
        /// </summary>
        public async Task<IEnumerable<ProjectDto>> GetByStatusAsync(
            ProjectStatus status,
            CancellationToken cancellationToken = default)
        {
            var projects = await _unitOfWork.Repository<Project>()
                .GetAllAsync(
                    filter: p => p.Status == status && !p.IsDeleted,
                    includeProperties: "Operation,ProjectTeamMembers,Phases",
                    orderBy: q => q.OrderBy(p => p.Code),
                    cancellationToken: cancellationToken);

            var dtos = projects.Select(p =>
            {
                var dto = _mapper.Map<ProjectDto>(p);
                dto.TeamMemberCount = p.ProjectTeamMembers?.Count ?? 0;
                dto.PhaseCount = p.Phases?.Count ?? 0;
                return dto;
            });

            return dtos;
        }

        /// <summary>
        /// Gets active projects
        /// </summary>
        public async Task<IEnumerable<ProjectDto>> GetActiveProjectsAsync(
            CancellationToken cancellationToken = default)
        {
            var projects = await _unitOfWork.Repository<Project>()
                .GetAllAsync(
                    filter: p => p.IsActive && !p.IsDeleted && p.Status == ProjectStatus.Active,
                    includeProperties: "Operation,ProjectTeamMembers,Phases",
                    orderBy: q => q.OrderBy(p => p.Code),
                    cancellationToken: cancellationToken);

            var dtos = projects.Select(p =>
            {
                var dto = _mapper.Map<ProjectDto>(p);
                dto.TeamMemberCount = p.ProjectTeamMembers?.Count ?? 0;
                dto.PhaseCount = p.Phases?.Count ?? 0;
                return dto;
            });

            return dtos;
        }

        /// <summary>
        /// Updates project charter information
        /// </summary>
        public async Task<ProjectDto?> UpdateProjectCharterAsync(
            Guid id,
            UpdateProjectCharterDto dto,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Project>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null || entity.IsDeleted)
                return null;

            entity.UpdateProjectCharter(
                dto.ProjectCharter,
                dto.BusinessCase,
                dto.Objectives,
                dto.Scope,
                dto.Deliverables,
                dto.SuccessCriteria,
                dto.Assumptions,
                dto.Constraints);

            entity.UpdatedBy = updatedBy ?? "System";

            _unitOfWork.Repository<Project>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return await GetByIdAsync(id, cancellationToken);
        }

        /// <summary>
        /// Updates project dates
        /// </summary>
        public async Task<ProjectDto?> UpdateDatesAsync(
            Guid id,
            UpdateProjectDatesDto dto,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Project>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null || entity.IsDeleted)
                return null;

            entity.UpdatePlannedDates(dto.PlannedStartDate, dto.PlannedEndDate);
            entity.UpdatedBy = updatedBy ?? "System";

            _unitOfWork.Repository<Project>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return await GetByIdAsync(id, cancellationToken);
        }

        /// <summary>
        /// Updates project budget
        /// </summary>
        public async Task<ProjectDto?> UpdateBudgetAsync(
            Guid id,
            UpdateProjectBudgetDto dto,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Project>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null || entity.IsDeleted)
                return null;

            entity.UpdateBudget(dto.TotalBudget, dto.ApprovedBudget, dto.ContingencyBudget);
            entity.UpdatedBy = updatedBy ?? "System";

            _unitOfWork.Repository<Project>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return await GetByIdAsync(id, cancellationToken);
        }

        /// <summary>
        /// Updates project status
        /// </summary>
        public async Task<ProjectDto?> UpdateStatusAsync(
            Guid id,
            ProjectStatus status,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Project>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null || entity.IsDeleted)
                return null;

            entity.UpdateStatus(status);
            entity.UpdatedBy = updatedBy ?? "System";

            _unitOfWork.Repository<Project>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Project {ProjectId} status changed to {Status} by {User}",
                id, status, updatedBy ?? "System");

            return await GetByIdAsync(id, cancellationToken);
        }

        /// <summary>
        /// Assigns project manager
        /// </summary>
        public async Task<ProjectDto?> AssignProjectManagerAsync(
            Guid id,
            AssignProjectManagerDto dto,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Project>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null || entity.IsDeleted)
                return null;

            entity.AssignProjectManager(dto.ProjectManagerId, dto.ProjectManagerName);
            entity.UpdatedBy = updatedBy ?? "System";

            _unitOfWork.Repository<Project>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return await GetByIdAsync(id, cancellationToken);
        }

        /// <summary>
        /// Updates project progress
        /// </summary>
        public async Task<ProjectDto?> UpdateProgressAsync(
            Guid id,
            UpdateProjectProgressDto dto,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Project>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null || entity.IsDeleted)
                return null;

            entity.UpdateProgress(dto.ProgressPercentage);
            entity.UpdatedBy = updatedBy ?? "System";

            _unitOfWork.Repository<Project>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return await GetByIdAsync(id, cancellationToken);
        }

        /// <summary>
        /// Updates project costs
        /// </summary>
        public async Task<ProjectDto?> UpdateCostsAsync(
            Guid id,
            UpdateProjectCostsDto dto,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Project>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null || entity.IsDeleted)
                return null;

            entity.UpdateCosts(dto.ActualCost, dto.CommittedCost);
            entity.UpdatedBy = updatedBy ?? "System";

            _unitOfWork.Repository<Project>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return await GetByIdAsync(id, cancellationToken);
        }

        /// <summary>
        /// Sets project baseline
        /// </summary>
        public async Task<ProjectDto?> SetBaselineAsync(
            Guid id,
            DateTime baselineDate,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Project>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null || entity.IsDeleted)
                return null;

            entity.SetBaseline(baselineDate);
            entity.UpdatedBy = updatedBy ?? "System";

            _unitOfWork.Repository<Project>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Project {ProjectId} baseline set to {BaselineDate} by {User}",
                id, baselineDate, updatedBy ?? "System");

            return await GetByIdAsync(id, cancellationToken);
        }

        /// <summary>
        /// Gets project summary with financial data
        /// </summary>
        public async Task<ProjectSummaryDto?> GetProjectSummaryAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Project>()
                .GetAsync(
                    filter: p => p.Id == id && !p.IsDeleted,
                    includeProperties: "Operation,ProjectTeamMembers,Phases,ControlAccounts,Budgets",
                    cancellationToken: cancellationToken);

            return entity != null ? _mapper.Map<ProjectSummaryDto>(entity) : null;
        }

        /// <summary>
        /// Gets project dashboard data
        /// </summary>
        public async Task<ProjectDashboardDto?> GetProjectDashboardAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Project>()
                .GetAsync(
                    filter: p => p.Id == id && !p.IsDeleted,
                    includeProperties: "Operation,ProjectTeamMembers,Phases,ControlAccounts,Budgets,Milestones",
                    cancellationToken: cancellationToken);

            if (entity == null)
                return null;

            var dashboard = new ProjectDashboardDto
            {
                Id = entity.Id,
                Code = entity.Code,
                Name = entity.Name,
                Status = entity.Status,
                ProgressPercentage = entity.ProgressPercentage,
                BudgetVariance = entity.GetBudgetVariance(),
                BudgetVariancePercentage = entity.GetBudgetVariancePercentage(),
                ScheduleVariance = entity.GetScheduleVariance(),
                PlannedDuration = entity.GetDuration(),
                ActualDuration = entity.GetActualDuration(),
                DaysOverdue = entity.IsDelayed() ? (DateTime.UtcNow - entity.PlannedEndDate).Days : (int?)null,
                TotalTeamMembers = entity.ProjectTeamMembers?.Count ?? 0,
                ActiveTeamMembers = entity.ProjectTeamMembers?.Count(m => m.IsActive) ?? 0,
                TotalBudget = entity.TotalBudget,
                ActualCost = entity.ActualCost,
                CommittedCost = entity.CommittedCost,
                PlannedStartDate = entity.PlannedStartDate,
                PlannedEndDate = entity.PlannedEndDate,
                ActualStartDate = entity.ActualStartDate,
                PlannedProgress = entity.PlannedProgress,
                ChangeOrderCount = entity.ChangeOrderCount,
                // Risk counts would need to be fetched separately
                HighRisks = 0,
                MediumRisks = 0,
                LowRisks = 0
            };

            return dashboard;
        }

        /// <summary>
        /// Gets overdue projects
        /// </summary>
        public async Task<IEnumerable<ProjectDto>> GetOverdueProjectsAsync(
            CancellationToken cancellationToken = default)
        {
            var projects = await _unitOfWork.Repository<Project>()
                .GetAllAsync(
                    filter: p => !p.IsDeleted && 
                           p.Status == ProjectStatus.Active &&
                           p.PlannedEndDate < DateTime.UtcNow,
                    includeProperties: "Operation,ProjectTeamMembers,Phases",
                    orderBy: q => q.OrderBy(p => p.PlannedEndDate),
                    cancellationToken: cancellationToken);

            var dtos = projects.Select(p =>
            {
                var dto = _mapper.Map<ProjectDto>(p);
                dto.TeamMemberCount = p.ProjectTeamMembers?.Count ?? 0;
                dto.PhaseCount = p.Phases?.Count ?? 0;
                return dto;
            });

            return dtos;
        }

        /// <summary>
        /// Gets over budget projects
        /// </summary>
        public async Task<IEnumerable<ProjectDto>> GetOverBudgetProjectsAsync(
            CancellationToken cancellationToken = default)
        {
            var projects = await _unitOfWork.Repository<Project>()
                .GetAllAsync(
                    filter: p => !p.IsDeleted && 
                           p.ActualCost.HasValue &&
                           p.ActualCost.Value > p.TotalBudget,
                    includeProperties: "Operation,ProjectTeamMembers,Phases",
                    orderBy: q => q.OrderByDescending(p => (p.ActualCost!.Value - p.TotalBudget) / p.TotalBudget),
                    cancellationToken: cancellationToken);

            var dtos = projects.Select(p =>
            {
                var dto = _mapper.Map<ProjectDto>(p);
                dto.TeamMemberCount = p.ProjectTeamMembers?.Count ?? 0;
                dto.PhaseCount = p.Phases?.Count ?? 0;
                return dto;
            });

            return dtos;
        }

        /// <summary>
        /// Checks if project code is unique
        /// </summary>
        public async Task<bool> IsCodeUniqueAsync(
            string code,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default)
        {
            var exists = await _unitOfWork.Repository<Project>()
                .GetAsync(
                    filter: p => p.Code == code && !p.IsDeleted && p.Id != excludeId,
                    cancellationToken: cancellationToken);

            return exists == null;
        }

        /// <summary>
        /// Override create to handle entity creation properly
        /// </summary>
        public override async Task<ProjectDto> CreateAsync(
            CreateProjectDto createDto,
            string? createdBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = new Project(
                createDto.Code,
                createDto.Name,
                createDto.OperationId,
                createDto.PlannedStartDate,
                createDto.PlannedEndDate,
                createDto.TotalBudget,
                createDto.Currency);

            // Description is set through UpdateBasicInfo
            
            if (!string.IsNullOrWhiteSpace(createDto.Location) ||
                !string.IsNullOrWhiteSpace(createDto.Client) ||
                !string.IsNullOrWhiteSpace(createDto.ContractNumber))
            {
                entity.UpdateBasicInfo(
                    entity.Name,
                    createDto.Description,
                    createDto.Location,
                    createDto.Client,
                    createDto.ContractNumber,
                    createDto.Currency);
            }

            entity.CreatedBy = createdBy ?? "System";

            await _unitOfWork.Repository<Project>().AddAsync(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Project with ID {ProjectId} created by {User}",
                entity.Id, createdBy ?? "System");

            return await GetByIdAsync(entity.Id, cancellationToken) ?? throw new InvalidOperationException("Failed to retrieve created project");
        }

        /// <summary>
        /// Override update to use entity methods
        /// </summary>
        public override async Task<ProjectDto?> UpdateAsync(
            Guid id,
            UpdateProjectDto updateDto,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Project>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null || entity.IsDeleted)
                return null;

            // Update basic info
            entity.UpdateBasicInfo(
                updateDto.Name,
                updateDto.Description,
                updateDto.Location,
                updateDto.Client,
                updateDto.ContractNumber,
                entity.Currency);

            // Update dates if provided
            if (updateDto.PlannedStartDate.HasValue && updateDto.PlannedEndDate.HasValue)
            {
                entity.UpdatePlannedDates(updateDto.PlannedStartDate.Value, updateDto.PlannedEndDate.Value);
            }

            // Update budget if provided
            if (updateDto.TotalBudget.HasValue)
            {
                entity.UpdateBudget(updateDto.TotalBudget.Value);
            }

            entity.UpdatedBy = updatedBy ?? "System";

            _unitOfWork.Repository<Project>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Project with ID {ProjectId} updated by {User}",
                entity.Id, updatedBy ?? "System");

            return await GetByIdAsync(id, cancellationToken);
        }

        /// <summary>
        /// Override GetByIdAsync to include related data
        /// </summary>
        public override async Task<ProjectDto?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Project>()
                .GetAsync(
                    filter: p => p.Id == id && !p.IsDeleted,
                    includeProperties: "Operation,ProjectTeamMembers,Phases",
                    cancellationToken: cancellationToken);

            if (entity == null)
                return null;

            var dto = _mapper.Map<ProjectDto>(entity);
            dto.TeamMemberCount = entity.ProjectTeamMembers?.Count ?? 0;
            dto.PhaseCount = entity.Phases?.Count ?? 0;
            
            return dto;
        }

        /// <summary>
        /// Override GetAllAsync to include related data
        /// </summary>
        public override async Task<IEnumerable<ProjectDto>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            var entities = await _unitOfWork.Repository<Project>()
                .GetAllAsync(
                    filter: p => !p.IsDeleted,
                    includeProperties: "Operation,ProjectTeamMembers,Phases",
                    orderBy: q => q.OrderBy(p => p.Code),
                    cancellationToken: cancellationToken);

            var dtos = entities.Select(p =>
            {
                var dto = _mapper.Map<ProjectDto>(p);
                dto.TeamMemberCount = p.ProjectTeamMembers?.Count ?? 0;
                dto.PhaseCount = p.Phases?.Count ?? 0;
                return dto;
            });

            return dtos;
        }

        /// <summary>
        /// Gets the unit of work instance
        /// </summary>
        public IUnitOfWork UnitOfWork => _unitOfWork;
    }
}