using Application.Interfaces.Organization;


namespace Application.Services.Organization
{
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

        /// <summary>
        /// Gets disciplines by package
        /// </summary>
        public async Task<IEnumerable<PackageDisciplineDto>> GetByPackageAsync(Guid packageId, CancellationToken cancellationToken = default)
        {
            var entities = await _unitOfWork.Repository<PackageDiscipline>()
                .GetAllAsync(
                    filter: pd => pd.PackageId == packageId,
                    includeProperties: "Package,Discipline,LeadEngineer",
                    orderBy: q => q.OrderByDescending(pd => pd.IsLeadDiscipline).ThenBy(pd => pd.Discipline.Order),
                    cancellationToken: cancellationToken);

            return entities.Select(pd =>
            {
                var dto = _mapper.Map<PackageDisciplineDto>(pd);
                MapComputedProperties(dto, pd);
                return dto;
            });
        }

        /// <summary>
        /// Gets packages by discipline
        /// </summary>
        public async Task<IEnumerable<PackageDisciplineDto>> GetByDisciplineAsync(Guid disciplineId, CancellationToken cancellationToken = default)
        {
            var entities = await _unitOfWork.Repository<PackageDiscipline>()
                .GetAllAsync(
                    filter: pd => pd.DisciplineId == disciplineId,
                    includeProperties: "Package,Discipline,LeadEngineer",
                    orderBy: q => q.OrderBy(pd => pd.Package.Code),
                    cancellationToken: cancellationToken);

            return entities.Select(pd =>
            {
                var dto = _mapper.Map<PackageDisciplineDto>(pd);
                MapComputedProperties(dto, pd);
                return dto;
            });
        }

        /// <summary>
        /// Gets lead discipline for a package
        /// </summary>
        public async Task<PackageDisciplineDto?> GetLeadDisciplineAsync(Guid packageId, CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<PackageDiscipline>()
                .GetAsync(
                    filter: pd => pd.PackageId == packageId && pd.IsLeadDiscipline,
                    includeProperties: "Package,Discipline,LeadEngineer",
                    cancellationToken: cancellationToken);

            if (entity == null)
                return null;

            var dto = _mapper.Map<PackageDisciplineDto>(entity);
            MapComputedProperties(dto, entity);
            return dto;
        }

        /// <summary>
        /// Updates discipline estimate
        /// </summary>
        public async Task<PackageDisciplineDto?> UpdateEstimateAsync(
            Guid id, 
            UpdateDisciplineEstimateDto dto, 
            string? updatedBy = null, 
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<PackageDiscipline>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null)
                return null;

            entity.UpdateEstimate(dto.EstimatedHours, dto.EstimatedCost);
            entity.UpdatedBy = updatedBy ?? "System";

            _unitOfWork.Repository<PackageDiscipline>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Package discipline {Id} estimate updated by {User}",
                id, updatedBy ?? "System");

            // Reload with navigation properties
            entity = await _unitOfWork.Repository<PackageDiscipline>()
                .GetAsync(
                    filter: pd => pd.Id == id,
                    includeProperties: "Package,Discipline,LeadEngineer",
                    cancellationToken: cancellationToken);

            var resultDto = _mapper.Map<PackageDisciplineDto>(entity);
            MapComputedProperties(resultDto, entity);
            return resultDto;
        }

        /// <summary>
        /// Updates discipline actuals
        /// </summary>
        public async Task<PackageDisciplineDto?> UpdateActualsAsync(
            Guid id, 
            UpdateDisciplineActualsDto dto, 
            string? updatedBy = null, 
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<PackageDiscipline>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null)
                return null;

            entity.UpdateActuals(dto.ActualHours, dto.ActualCost, dto.ProgressPercentage);
            entity.UpdatedBy = updatedBy ?? "System";

            _unitOfWork.Repository<PackageDiscipline>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Package discipline {Id} actuals updated by {User}",
                id, updatedBy ?? "System");

            // Reload with navigation properties
            entity = await _unitOfWork.Repository<PackageDiscipline>()
                .GetAsync(
                    filter: pd => pd.Id == id,
                    includeProperties: "Package,Discipline,LeadEngineer",
                    cancellationToken: cancellationToken);

            var resultDto = _mapper.Map<PackageDisciplineDto>(entity);
            MapComputedProperties(resultDto, entity);
            return resultDto;
        }

        /// <summary>
        /// Assigns lead engineer to discipline
        /// </summary>
        public async Task<PackageDisciplineDto?> AssignLeadEngineerAsync(
            Guid id, 
            Guid? leadEngineerId, 
            string? updatedBy = null, 
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<PackageDiscipline>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null)
                return null;

            // Validate lead engineer exists if provided
            if (leadEngineerId.HasValue)
            {
                var userExists = await _unitOfWork.Repository<User>()
                    .GetByIdAsync(leadEngineerId.Value, cancellationToken) != null;
                
                if (!userExists)
                    throw new InvalidOperationException($"User {leadEngineerId} not found");
            }

            entity.AssignLeadEngineer(leadEngineerId);
            entity.UpdatedBy = updatedBy ?? "System";

            _unitOfWork.Repository<PackageDiscipline>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Lead engineer {LeadEngineerId} assigned to package discipline {Id} by {User}",
                leadEngineerId, id, updatedBy ?? "System");

            // Reload with navigation properties
            entity = await _unitOfWork.Repository<PackageDiscipline>()
                .GetAsync(
                    filter: pd => pd.Id == id,
                    includeProperties: "Package,Discipline,LeadEngineer",
                    cancellationToken: cancellationToken);

            var dto = _mapper.Map<PackageDisciplineDto>(entity);
            MapComputedProperties(dto, entity);
            return dto;
        }

        /// <summary>
        /// Sets discipline as lead
        /// </summary>
        public async Task<PackageDisciplineDto?> SetAsLeadDisciplineAsync(
            Guid id, 
            bool isLead, 
            string? updatedBy = null, 
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<PackageDiscipline>()
                .GetAsync(
                    filter: pd => pd.Id == id,
                    includeProperties: "Package",
                    cancellationToken: cancellationToken);

            if (entity == null)
                return null;

            // If setting as lead, validate no other lead discipline exists
            if (isLead)
            {
                var existingLead = await _unitOfWork.Repository<PackageDiscipline>()
                    .GetAsync(
                        filter: pd => pd.PackageId == entity.PackageId && pd.IsLeadDiscipline && pd.Id != id,
                        cancellationToken: cancellationToken);

                if (existingLead != null)
                {
                    // Remove lead status from existing lead
                    existingLead.SetAsLeadDiscipline(false);
                    existingLead.UpdatedBy = updatedBy ?? "System";
                    _unitOfWork.Repository<PackageDiscipline>().Update(existingLead);
                }
            }

            entity.SetAsLeadDiscipline(isLead);
            entity.UpdatedBy = updatedBy ?? "System";

            _unitOfWork.Repository<PackageDiscipline>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Package discipline {Id} lead status set to {IsLead} by {User}",
                id, isLead, updatedBy ?? "System");

            // Reload with navigation properties
            entity = await _unitOfWork.Repository<PackageDiscipline>()
                .GetAsync(
                    filter: pd => pd.Id == id,
                    includeProperties: "Package,Discipline,LeadEngineer",
                    cancellationToken: cancellationToken);

            var dto = _mapper.Map<PackageDisciplineDto>(entity);
            MapComputedProperties(dto, entity);
            return dto;
        }

        /// <summary>
        /// Gets discipline performance summary
        /// </summary>
        public async Task<DisciplinePerformanceSummaryDto> GetPerformanceSummaryAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<PackageDiscipline>()
                .GetAsync(
                    filter: pd => pd.Id == id,
                    includeProperties: "Package,Discipline",
                    cancellationToken: cancellationToken);

            if (entity == null)
                throw new System.InvalidOperationException($"Package discipline {id} not found");

            var summary = new DisciplinePerformanceSummaryDto
            {
                Id = entity.Id,
                PackageId = entity.PackageId,
                PackageCode = entity.Package?.Code ?? string.Empty,
                PackageName = entity.Package?.Name ?? string.Empty,
                DisciplineId = entity.DisciplineId,
                DisciplineCode = entity.Discipline?.Code ?? string.Empty,
                DisciplineName = entity.Discipline?.Name ?? string.Empty,
                
                // Performance Metrics
                EstimatedHours = entity.EstimatedHours,
                ActualHours = entity.ActualHours,
                HoursVariance = entity.HoursVariance,
                HoursVariancePercentage = entity.EstimatedHours > 0 ? (entity.HoursVariance / entity.EstimatedHours) * 100 : 0,
                ProductivityRate = entity.ProductivityRate,
                
                // Cost Performance
                EstimatedCost = entity.EstimatedCost,
                ActualCost = entity.ActualCost,
                CostVariance = entity.CostVariance,
                CostVariancePercentage = entity.EstimatedCost > 0 ? (entity.CostVariance / entity.EstimatedCost) * 100 : 0,
                CostPerformanceIndex = entity.ActualCost > 0 ? entity.EstimatedCost / entity.ActualCost : 1,
                
                // Progress
                ProgressPercentage = entity.ProgressPercentage,
                LastProgressUpdate = entity.LastProgressUpdate,
                
                // Status
                IsOnBudget = entity.ActualCost <= entity.EstimatedCost,
                IsOnSchedule = entity.ActualHours <= entity.EstimatedHours,
                IsLeadDiscipline = entity.IsLeadDiscipline
            };

            return summary;
        }

        /// <summary>
        /// Gets disciplines by lead engineer
        /// </summary>
        public async Task<IEnumerable<PackageDisciplineDto>> GetByLeadEngineerAsync(Guid leadEngineerId, CancellationToken cancellationToken = default)
        {
            var entities = await _unitOfWork.Repository<PackageDiscipline>()
                .GetAllAsync(
                    filter: pd => pd.LeadEngineerId == leadEngineerId,
                    includeProperties: "Package,Discipline,LeadEngineer",
                    orderBy: q => q.OrderBy(pd => pd.Package.Code).ThenBy(pd => pd.Discipline.Order),
                    cancellationToken: cancellationToken);

            return entities.Select(pd =>
            {
                var dto = _mapper.Map<PackageDisciplineDto>(pd);
                MapComputedProperties(dto, pd);
                return dto;
            });
        }

        /// <summary>
        /// Bulk creates package disciplines
        /// </summary>
        public async Task<IEnumerable<PackageDisciplineDto>> BulkCreateAsync(
            Guid packageId, 
            IEnumerable<CreatePackageDisciplineDto> disciplines, 
            string? createdBy = null, 
            CancellationToken cancellationToken = default)
        {
            var createdEntities = new List<PackageDiscipline>();

            // Validate package exists
            var packageExists = await _unitOfWork.Repository<Package>()
                .GetByIdAsync(packageId, cancellationToken) != null;
            
            if (!packageExists)
                throw new InvalidOperationException($"Package {packageId} not found");

            // Ensure only one lead discipline
            var leadCount = disciplines.Count(d => d.IsLeadDiscipline);
            if (leadCount > 1)
                throw new InvalidOperationException("Only one discipline can be marked as lead");

            foreach (var dto in disciplines)
            {
                // Check if discipline already exists for this package
                var existing = await _unitOfWork.Repository<PackageDiscipline>()
                    .GetAsync(
                        filter: pd => pd.PackageId == packageId && pd.DisciplineId == dto.DisciplineId,
                        cancellationToken: cancellationToken);

                if (existing != null)
                {
                    _logger.LogWarning("Discipline {DisciplineId} already exists for package {PackageId}",
                        dto.DisciplineId, packageId);
                    continue;
                }

                var entity = new PackageDiscipline(
                    packageId,
                    dto.DisciplineId,
                    dto.EstimatedHours,
                    dto.EstimatedCost,
                    dto.IsLeadDiscipline);

                if (dto.LeadEngineerId.HasValue)
                {
                    entity.AssignLeadEngineer(dto.LeadEngineerId.Value);
                }

                if (!string.IsNullOrWhiteSpace(dto.Notes))
                {
                    entity.UpdateNotes(dto.Notes);
                }

                entity.CreatedBy = createdBy ?? "System";

                await _unitOfWork.Repository<PackageDiscipline>().AddAsync(entity);
                createdEntities.Add(entity);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("{Count} disciplines created for package {PackageId} by {User}",
                createdEntities.Count, packageId, createdBy ?? "System");

            // Reload with navigation properties
            var entityIds = createdEntities.Select(e => e.Id).ToList();
            var reloadedEntities = await _unitOfWork.Repository<PackageDiscipline>()
                .GetAllAsync(
                    filter: pd => entityIds.Contains(pd.Id),
                    includeProperties: "Package,Discipline,LeadEngineer",
                    cancellationToken: cancellationToken);

            return reloadedEntities.Select(pd =>
            {
                var dto = _mapper.Map<PackageDisciplineDto>(pd);
                MapComputedProperties(dto, pd);
                return dto;
            });
        }

        /// <summary>
        /// Gets package discipline allocation summary
        /// </summary>
        public async Task<PackageDisciplineAllocationDto> GetAllocationSummaryAsync(Guid packageId, CancellationToken cancellationToken = default)
        {
            var disciplines = await _unitOfWork.Repository<PackageDiscipline>()
                .GetAllAsync(
                    filter: pd => pd.PackageId == packageId,
                    includeProperties: "Discipline",
                    cancellationToken: cancellationToken);

            var disciplinesList = disciplines.ToList();

            var summary = new PackageDisciplineAllocationDto
            {
                PackageId = packageId,
                TotalDisciplines = disciplinesList.Count,
                TotalEstimatedManHours = disciplinesList.Sum(d => d.EstimatedHours),
                TotalActualManHours = disciplinesList.Sum(d => d.ActualHours),
                TotalEstimatedCost = disciplinesList.Sum(d => d.EstimatedCost),
                TotalActualCost = disciplinesList.Sum(d => d.ActualCost),
                AverageProgress = disciplinesList.Any() ? disciplinesList.Average(d => d.ProgressPercentage) : 0
            };

            // Build discipline breakdown
            foreach (var pd in disciplinesList)
            {
                var breakdown = new DisciplineAllocationBreakdownDto
                {
                    DisciplineId = pd.DisciplineId,
                    DisciplineCode = pd.Discipline?.Code ?? string.Empty,
                    DisciplineName = pd.Discipline?.Name ?? string.Empty,
                    IsLead = pd.IsLeadDiscipline,
                    EstimatedManHours = pd.EstimatedHours,
                    ActualManHours = pd.ActualHours,
                    EstimatedCost = pd.EstimatedCost,
                    ActualCost = pd.ActualCost,
                    ProgressPercentage = pd.ProgressPercentage,
                    ManHoursPercentage = summary.TotalEstimatedManHours > 0 
                        ? (pd.EstimatedHours / summary.TotalEstimatedManHours) * 100 : 0,
                    CostPercentage = summary.TotalEstimatedCost > 0 
                        ? (pd.EstimatedCost / summary.TotalEstimatedCost) * 100 : 0
                };

                summary.DisciplineBreakdown.Add(breakdown);
            }

            // Sort breakdown by percentage (descending)
            summary.DisciplineBreakdown = summary.DisciplineBreakdown
                .OrderByDescending(d => d.IsLead)
                .ThenByDescending(d => d.ManHoursPercentage)
                .ToList();

            return summary;
        }

        /// <summary>
        /// Validates discipline assignments (ensures single lead discipline)
        /// </summary>
        public async Task<bool> ValidateLeadDisciplineAsync(Guid packageId, Guid? excludeDisciplineId = null, CancellationToken cancellationToken = default)
        {
            var leadCount = await _unitOfWork.Repository<PackageDiscipline>()
                .CountAsync(
                    predicate: pd => pd.PackageId == packageId && 
                                    pd.IsLeadDiscipline && 
                                    (excludeDisciplineId == null || pd.Id != excludeDisciplineId),
                    cancellationToken: cancellationToken);

            return leadCount == 0;
        }

        /// <summary>
        /// Override create to handle entity creation properly
        /// </summary>
        public override async Task<PackageDisciplineDto> CreateAsync(
            CreatePackageDisciplineDto createDto,
            string? createdBy = null,
            CancellationToken cancellationToken = default)
        {
            // This method assumes packageId is passed separately
            // Since CreatePackageDisciplineDto doesn't have PackageId
            // This would typically be handled at the API level
            throw new NotImplementedException("Use BulkCreateAsync with explicit packageId instead");
        }

        /// <summary>
        /// Override update to use entity methods
        /// </summary>
        public override async Task<PackageDisciplineDto?> UpdateAsync(
            Guid id,
            UpdatePackageDisciplineDto updateDto,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<PackageDiscipline>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null)
                return null;

            // Update estimate
            entity.UpdateEstimate(updateDto.EstimatedHours, updateDto.EstimatedCost);

            // Update actuals
            entity.UpdateActuals(updateDto.ActualHours, updateDto.ActualCost, updateDto.ProgressPercentage);

            // Update lead status
            if (updateDto.IsLeadDiscipline != entity.IsLeadDiscipline)
            {
                // Validate lead discipline change
                if (updateDto.IsLeadDiscipline)
                {
                    var isValid = await ValidateLeadDisciplineAsync(entity.PackageId, id, cancellationToken);
                    if (!isValid)
                        throw new InvalidOperationException("Package already has a lead discipline");
                }
                
                entity.SetAsLeadDiscipline(updateDto.IsLeadDiscipline);
            }

            // Update lead engineer
            if (updateDto.LeadEngineerId != entity.LeadEngineerId)
            {
                entity.AssignLeadEngineer(updateDto.LeadEngineerId);
            }

            // Update notes
            if (updateDto.Notes != entity.Notes)
            {
                entity.UpdateNotes(updateDto.Notes);
            }

            entity.UpdatedBy = updatedBy ?? "System";

            _unitOfWork.Repository<PackageDiscipline>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Package discipline {Id} updated by {User}",
                id, updatedBy ?? "System");

            // Reload with navigation properties
            entity = await _unitOfWork.Repository<PackageDiscipline>()
                .GetAsync(
                    filter: pd => pd.Id == id,
                    includeProperties: "Package,Discipline,LeadEngineer",
                    cancellationToken: cancellationToken);

            var dto = _mapper.Map<PackageDisciplineDto>(entity);
            MapComputedProperties(dto, entity);
            return dto;
        }

        /// <summary>
        /// Override GetByIdAsync to include navigation properties
        /// </summary>
        public override async Task<PackageDisciplineDto?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<PackageDiscipline>()
                .GetAsync(
                    filter: pd => pd.Id == id,
                    includeProperties: "Package,Discipline,LeadEngineer",
                    cancellationToken: cancellationToken);

            if (entity == null)
                return null;

            var dto = _mapper.Map<PackageDisciplineDto>(entity);
            MapComputedProperties(dto, entity);
            return dto;
        }

        /// <summary>
        /// Override GetAllAsync to include navigation properties
        /// </summary>
        public override async Task<IEnumerable<PackageDisciplineDto>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            var entities = await _unitOfWork.Repository<PackageDiscipline>()
                .GetAllAsync(
                    includeProperties: "Package,Discipline,LeadEngineer",
                    orderBy: q => q.OrderBy(pd => pd.Package.Code).ThenBy(pd => pd.Discipline.Order),
                    cancellationToken: cancellationToken);

            return entities.Select(pd =>
            {
                var dto = _mapper.Map<PackageDisciplineDto>(pd);
                MapComputedProperties(dto, pd);
                return dto;
            });
        }

        /// <summary>
        /// Maps computed properties from entity to DTO
        /// </summary>
        private void MapComputedProperties(PackageDisciplineDto dto, PackageDiscipline entity)
        {
            // Navigation property names
            dto.PackageCode = entity.Package?.Code ?? string.Empty;
            dto.PackageName = entity.Package?.Name ?? string.Empty;
            dto.DisciplineCode = entity.Discipline?.Code ?? string.Empty;
            dto.DisciplineName = entity.Discipline?.Name ?? string.Empty;
            dto.LeadEngineerName = entity.LeadEngineer?.DisplayName;

            // Calculated fields
            dto.ProductivityRate = entity.ProductivityRate;
            dto.CostVariance = entity.CostVariance;
            dto.HoursVariance = entity.HoursVariance;
        }

        /// <summary>
        /// Gets the unit of work instance
        /// </summary>
        public IUnitOfWork UnitOfWork => _unitOfWork;
    }
}