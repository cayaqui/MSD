using Application.Interfaces.Organization;
using Application.Services.Base;
using AutoMapper;
using Core.DTOs.Organization.Package;
using Core.Enums.Projects;
using Domain.Entities.Organization.Core;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Application.Services.Organization
{
    /// <summary>
    /// Service implementation for Package management
    /// </summary>
    public class PackageService : BaseService<Package, PackageDto, CreatePackageDto, UpdatePackageDto>, IPackageService
    {
        public PackageService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<PackageService> logger)
            : base(unitOfWork, mapper, logger)
        {
        }

        /// <summary>
        /// Gets packages by phase
        /// </summary>
        public async Task<IEnumerable<PackageDto>> GetByPhaseAsync(Guid phaseId, CancellationToken cancellationToken = default)
        {
            var packages = await _unitOfWork.Repository<Package>()
                .GetAllAsync(
                    filter: p => p.PhaseId == phaseId && !p.IsDeleted,
                    includeProperties: "Phase,Contractor",
                    orderBy: q => q.OrderBy(p => p.Code),
                    cancellationToken: cancellationToken);

            var dtos = packages.Select(p =>
            {
                var dto = _mapper.Map<PackageDto>(p);
                MapComputedProperties(dto, p);
                return dto;
            });

            return dtos;
        }

        /// <summary>
        /// Gets packages by WBS element
        /// </summary>
        public async Task<IEnumerable<PackageDto>> GetByWBSElementAsync(Guid wbsElementId, CancellationToken cancellationToken = default)
        {
            var packages = await _unitOfWork.Repository<Package>()
                .GetAllAsync(
                    filter: p => p.WBSElementId == wbsElementId && !p.IsDeleted,
                    includeProperties: "WBSElement,Contractor",
                    orderBy: q => q.OrderBy(p => p.Code),
                    cancellationToken: cancellationToken);

            var dtos = packages.Select(p =>
            {
                var dto = _mapper.Map<PackageDto>(p);
                MapComputedProperties(dto, p);
                return dto;
            });

            return dtos;
        }

        /// <summary>
        /// Gets packages by contractor
        /// </summary>
        public async Task<IEnumerable<PackageDto>> GetByContractorAsync(Guid contractorId, CancellationToken cancellationToken = default)
        {
            var packages = await _unitOfWork.Repository<Package>()
                .GetAllAsync(
                    filter: p => p.ContractorId == contractorId && !p.IsDeleted,
                    includeProperties: "Phase,WBSElement,Contractor",
                    orderBy: q => q.OrderBy(p => p.Code),
                    cancellationToken: cancellationToken);

            var dtos = packages.Select(p =>
            {
                var dto = _mapper.Map<PackageDto>(p);
                MapComputedProperties(dto, p);
                return dto;
            });

            return dtos;
        }

        /// <summary>
        /// Gets packages by type
        /// </summary>
        public async Task<IEnumerable<PackageDto>> GetByTypeAsync(PackageType packageType, CancellationToken cancellationToken = default)
        {
            var packages = await _unitOfWork.Repository<Package>()
                .GetAllAsync(
                    filter: p => p.PackageType == packageType && !p.IsDeleted,
                    includeProperties: "Phase,WBSElement,Contractor",
                    orderBy: q => q.OrderBy(p => p.Code),
                    cancellationToken: cancellationToken);

            var dtos = packages.Select(p =>
            {
                var dto = _mapper.Map<PackageDto>(p);
                MapComputedProperties(dto, p);
                return dto;
            });

            return dtos;
        }

        /// <summary>
        /// Assigns package to phase
        /// </summary>
        public async Task<PackageDto?> AssignToPhaseAsync(
            Guid id,
            Guid phaseId,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Package>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null || entity.IsDeleted)
                return null;

            entity.AssignToPhase(phaseId);
            entity.UpdatedBy = updatedBy ?? "System";
            entity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Package>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Package {PackageCode} assigned to phase {PhaseId} by {User}",
                entity.Code, phaseId, updatedBy ?? "System");

            // Reload with navigation properties
            entity = await _unitOfWork.Repository<Package>()
                .GetAsync(
                    filter: p => p.Id == id,
                    includeProperties: "Phase,WBSElement,Contractor",
                    cancellationToken: cancellationToken);

            var dto = _mapper.Map<PackageDto>(entity);
            MapComputedProperties(dto, entity);
            
            return dto;
        }

        /// <summary>
        /// Assigns package to work package
        /// </summary>
        public async Task<PackageDto?> AssignToWorkPackageAsync(
            Guid id,
            Guid wbsElementId,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Package>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null || entity.IsDeleted)
                return null;

            entity.AssignToWorkPackage(wbsElementId);
            entity.UpdatedBy = updatedBy ?? "System";
            entity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Package>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Package {PackageCode} assigned to WBS element {WBSElementId} by {User}",
                entity.Code, wbsElementId, updatedBy ?? "System");

            // Reload with navigation properties
            entity = await _unitOfWork.Repository<Package>()
                .GetAsync(
                    filter: p => p.Id == id,
                    includeProperties: "Phase,WBSElement,Contractor",
                    cancellationToken: cancellationToken);

            var dto = _mapper.Map<PackageDto>(entity);
            MapComputedProperties(dto, entity);
            
            return dto;
        }

        /// <summary>
        /// Assigns contractor to package
        /// </summary>
        public async Task<PackageDto?> AssignContractorAsync(
            Guid id,
            Guid? contractorId,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Package>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null || entity.IsDeleted)
                return null;

            entity.AssignContractor(contractorId);
            entity.UpdatedBy = updatedBy ?? "System";
            entity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Package>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Package {PackageCode} assigned to contractor {ContractorId} by {User}",
                entity.Code, contractorId, updatedBy ?? "System");

            // Reload with navigation properties
            entity = await _unitOfWork.Repository<Package>()
                .GetAsync(
                    filter: p => p.Id == id,
                    includeProperties: "Phase,WBSElement,Contractor",
                    cancellationToken: cancellationToken);

            var dto = _mapper.Map<PackageDto>(entity);
            MapComputedProperties(dto, entity);
            
            return dto;
        }

        /// <summary>
        /// Updates package contract information
        /// </summary>
        public async Task<PackageDto?> UpdateContractInfoAsync(
            Guid id,
            UpdatePackageContractDto dto,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Package>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null || entity.IsDeleted)
                return null;

            entity.UpdateContractInfo(
                dto.ContractNumber,
                dto.ContractType,
                dto.ContractValue,
                dto.Currency);

            entity.UpdatedBy = updatedBy ?? "System";
            entity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Package>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var resultDto = _mapper.Map<PackageDto>(entity);
            MapComputedProperties(resultDto, entity);
            
            return resultDto;
        }

        /// <summary>
        /// Updates package progress
        /// </summary>
        public async Task<PackageDto?> UpdateProgressAsync(
            Guid id,
            decimal progressPercentage,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Package>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null || entity.IsDeleted)
                return null;

            entity.UpdateProgress(progressPercentage);
            entity.UpdatedBy = updatedBy ?? "System";
            entity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Package>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Package {PackageCode} progress updated to {Progress}% by {User}",
                entity.Code, progressPercentage, updatedBy ?? "System");

            var dto = _mapper.Map<PackageDto>(entity);
            MapComputedProperties(dto, entity);
            
            return dto;
        }

        /// <summary>
        /// Updates package dates
        /// </summary>
        public async Task<PackageDto?> UpdateDatesAsync(
            Guid id,
            UpdatePackageDatesDto dto,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Package>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null || entity.IsDeleted)
                return null;

            entity.UpdatePlannedDates(dto.PlannedStartDate, dto.PlannedEndDate);
            entity.UpdateActualDates(dto.ActualStartDate, dto.ActualEndDate);

            entity.UpdatedBy = updatedBy ?? "System";
            entity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Package>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var resultDto = _mapper.Map<PackageDto>(entity);
            MapComputedProperties(resultDto, entity);
            
            return resultDto;
        }

        /// <summary>
        /// Starts a package
        /// </summary>
        public async Task<PackageDto?> StartPackageAsync(
            Guid id,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Package>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null || entity.IsDeleted)
                return null;

            entity.Start();
            entity.UpdatedBy = updatedBy ?? "System";
            entity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Package>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Package {PackageCode} started by {User}",
                entity.Code, updatedBy ?? "System");

            var dto = _mapper.Map<PackageDto>(entity);
            MapComputedProperties(dto, entity);
            
            return dto;
        }

        /// <summary>
        /// Completes a package
        /// </summary>
        public async Task<PackageDto?> CompletePackageAsync(
            Guid id,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Package>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null || entity.IsDeleted)
                return null;

            entity.Complete();
            entity.UpdatedBy = updatedBy ?? "System";
            entity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Package>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Package {PackageCode} completed by {User}",
                entity.Code, updatedBy ?? "System");

            var dto = _mapper.Map<PackageDto>(entity);
            MapComputedProperties(dto, entity);
            
            return dto;
        }

        /// <summary>
        /// Cancels a package
        /// </summary>
        public async Task<PackageDto?> CancelPackageAsync(
            Guid id,
            string reason,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Package>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null || entity.IsDeleted)
                return null;

            entity.Cancel();
            entity.Description = $"{entity.Description}\n\nCANCELLED: {reason}";
            entity.UpdatedBy = updatedBy ?? "System";
            entity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Package>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Package {PackageCode} cancelled by {User}. Reason: {Reason}",
                entity.Code, updatedBy ?? "System", reason);

            var dto = _mapper.Map<PackageDto>(entity);
            MapComputedProperties(dto, entity);
            
            return dto;
        }

        /// <summary>
        /// Gets overdue packages
        /// </summary>
        public async Task<IEnumerable<PackageDto>> GetOverduePackagesAsync(CancellationToken cancellationToken = default)
        {
            var packages = await _unitOfWork.Repository<Package>()
                .GetAllAsync(
                    filter: p => !p.IsDeleted && 
                               !p.ActualEndDate.HasValue && 
                               p.ProgressPercentage < 100 &&
                               p.PlannedEndDate < DateTime.UtcNow,
                    includeProperties: "Phase,WBSElement,Contractor",
                    orderBy: q => q.OrderBy(p => p.PlannedEndDate),
                    cancellationToken: cancellationToken);

            var dtos = packages.Select(p =>
            {
                var dto = _mapper.Map<PackageDto>(p);
                MapComputedProperties(dto, p);
                return dto;
            });

            return dtos;
        }

        /// <summary>
        /// Gets package with disciplines
        /// </summary>
        public async Task<PackageWithDisciplinesDto?> GetWithDisciplinesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Package>()
                .GetAsync(
                    filter: p => p.Id == id && !p.IsDeleted,
                    includeProperties: "Phase,WBSElement,Contractor",
                    cancellationToken: cancellationToken);

            if (entity == null)
                return null;

            var dto = _mapper.Map<PackageWithDisciplinesDto>(entity);
            MapComputedProperties(dto, entity);

            // TODO: Load disciplines when PackageDiscipline entity is available
            // This would require a repository call to get disciplines associated with this package
            
            return dto;
        }

        /// <summary>
        /// Gets package performance metrics
        /// </summary>
        public async Task<PackagePerformanceDto?> GetPerformanceMetricsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Package>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null || entity.IsDeleted)
                return null;

            var dto = new PackagePerformanceDto
            {
                Id = entity.Id,
                Code = entity.Code,
                Name = entity.Name,
                
                // Schedule Performance
                PlannedStartDate = entity.PlannedStartDate,
                PlannedEndDate = entity.PlannedEndDate,
                ActualStartDate = entity.ActualStartDate,
                ActualEndDate = entity.ActualEndDate,
                PlannedDuration = (entity.PlannedEndDate - entity.PlannedStartDate).Days,
                ActualDuration = entity.ActualStartDate.HasValue 
                    ? (entity.ActualEndDate ?? DateTime.UtcNow).Subtract(entity.ActualStartDate.Value).Days 
                    : null,
                ScheduleVariance = entity.GetScheduleVariance(),
                SchedulePerformanceIndex = CalculateSchedulePerformanceIndex(entity),
                IsOnSchedule = !entity.IsOverdue(),
                DaysAheadBehind = CalculateDaysAheadBehind(entity),
                
                // Cost Performance (simplified - would need actual cost data)
                ContractValue = entity.ContractValue,
                ActualCost = 0, // TODO: Get from cost tracking
                CostVariance = 0,
                CostPerformanceIndex = 1,
                IsOnBudget = true,
                
                // Progress Performance
                ProgressPercentage = entity.ProgressPercentage,
                PlannedProgress = CalculatePlannedProgress(entity),
                ProgressVariance = entity.ProgressPercentage - CalculatePlannedProgress(entity),
                
                // Quality Metrics (simplified - would need issue tracking)
                TotalIssues = 0,
                OpenIssues = 0,
                ClosedIssues = 0,
                IssueClosureRate = 0,
                
                // Resource Performance (simplified - would need resource tracking)
                PlannedManHours = 0,
                ActualManHours = 0,
                ProductivityIndex = 1
            };
            
            return dto;
        }

        /// <summary>
        /// Override create to handle entity creation properly
        /// </summary>
        public override async Task<PackageDto> CreateAsync(
            CreatePackageDto createDto,
            string? createdBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = new Package(
                createDto.Code,
                createDto.Name,
                createDto.WBSCode,
                createDto.PackageType,
                createDto.PlannedStartDate,
                createDto.PlannedEndDate);

            // Set basic info
            entity.UpdateBasicInfo(createDto.Name, createDto.Description);

            // Set contract info if provided
            if (!string.IsNullOrWhiteSpace(createDto.ContractNumber) || createDto.ContractValue > 0)
            {
                entity.UpdateContractInfo(
                    createDto.ContractNumber,
                    createDto.ContractType,
                    createDto.ContractValue,
                    createDto.Currency);
            }

            // Assign to phase or WBS element
            if (createDto.PhaseId.HasValue)
            {
                entity.AssignToPhase(createDto.PhaseId.Value);
            }
            else if (createDto.WBSElementId.HasValue)
            {
                entity.AssignToWorkPackage(createDto.WBSElementId.Value);
            }

            // Assign contractor if provided
            if (createDto.ContractorId.HasValue)
            {
                entity.AssignContractor(createDto.ContractorId.Value);
            }

            entity.CreatedBy = createdBy ?? "System";
            entity.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.Repository<Package>().AddAsync(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Package {PackageCode} created by {User}",
                entity.Code, createdBy ?? "System");

            // Reload with navigation properties
            entity = await _unitOfWork.Repository<Package>()
                .GetAsync(
                    filter: p => p.Id == entity.Id,
                    includeProperties: "Phase,WBSElement,Contractor",
                    cancellationToken: cancellationToken);

            var dto = _mapper.Map<PackageDto>(entity);
            MapComputedProperties(dto, entity);
            
            return dto;
        }

        /// <summary>
        /// Override update to use entity methods
        /// </summary>
        public override async Task<PackageDto?> UpdateAsync(
            Guid id,
            UpdatePackageDto updateDto,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Package>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null || entity.IsDeleted)
                return null;

            // Update basic info
            entity.UpdateBasicInfo(updateDto.Name, updateDto.Description);
            entity.UpdateWBSCode(updateDto.WBSCode);
            entity.UpdatePackageType(updateDto.PackageType);

            // Update contract info
            entity.UpdateContractInfo(
                updateDto.ContractNumber,
                updateDto.ContractType,
                updateDto.ContractValue,
                updateDto.Currency);

            // Update dates
            entity.UpdatePlannedDates(updateDto.PlannedStartDate, updateDto.PlannedEndDate);
            if (updateDto.ActualStartDate.HasValue || updateDto.ActualEndDate.HasValue)
            {
                entity.UpdateActualDates(updateDto.ActualStartDate, updateDto.ActualEndDate);
            }

            // Update progress
            entity.UpdateProgress(updateDto.ProgressPercentage);

            // Update contractor
            entity.AssignContractor(updateDto.ContractorId);

            entity.UpdatedBy = updatedBy ?? "System";
            entity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Package>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Package {PackageCode} updated by {User}",
                entity.Code, updatedBy ?? "System");

            // Reload with navigation properties
            entity = await _unitOfWork.Repository<Package>()
                .GetAsync(
                    filter: p => p.Id == id,
                    includeProperties: "Phase,WBSElement,Contractor",
                    cancellationToken: cancellationToken);

            var dto = _mapper.Map<PackageDto>(entity);
            MapComputedProperties(dto, entity);
            
            return dto;
        }

        /// <summary>
        /// Override GetByIdAsync to include navigation properties
        /// </summary>
        public override async Task<PackageDto?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Package>()
                .GetAsync(
                    filter: p => p.Id == id && !p.IsDeleted,
                    includeProperties: "Phase,WBSElement,Contractor",
                    cancellationToken: cancellationToken);

            if (entity == null)
                return null;

            var dto = _mapper.Map<PackageDto>(entity);
            MapComputedProperties(dto, entity);
            
            return dto;
        }

        /// <summary>
        /// Override GetAllAsync to include navigation properties
        /// </summary>
        public override async Task<IEnumerable<PackageDto>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            var entities = await _unitOfWork.Repository<Package>()
                .GetAllAsync(
                    filter: p => !p.IsDeleted,
                    includeProperties: "Phase,WBSElement,Contractor",
                    orderBy: q => q.OrderBy(p => p.Code),
                    cancellationToken: cancellationToken);

            var dtos = entities.Select(p =>
            {
                var dto = _mapper.Map<PackageDto>(p);
                MapComputedProperties(dto, p);
                return dto;
            });

            return dtos;
        }

        /// <summary>
        /// Maps computed properties from entity to DTO
        /// </summary>
        private void MapComputedProperties(PackageDto dto, Package entity)
        {
            dto.IsOverdue = entity.IsOverdue();
            dto.DaysOverdue = entity.GetDaysOverdue();
            dto.ScheduleVariance = entity.GetScheduleVariance();
            
            // Map navigation property names
            dto.PhaseName = entity.Phase?.Name;
            dto.WBSElementName = entity.WBSElement?.Name;
            dto.ContractorName = entity.Contractor?.Name;
        }

        /// <summary>
        /// Calculates the planned progress based on dates
        /// </summary>
        private decimal CalculatePlannedProgress(Package entity)
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
        /// Calculates the schedule performance index
        /// </summary>
        private decimal CalculateSchedulePerformanceIndex(Package entity)
        {
            var plannedProgress = CalculatePlannedProgress(entity);
            return plannedProgress > 0 ? entity.ProgressPercentage / plannedProgress : 1;
        }

        /// <summary>
        /// Calculates days ahead or behind schedule
        /// </summary>
        private int? CalculateDaysAheadBehind(Package entity)
        {
            if (!entity.ActualStartDate.HasValue)
                return null;
            
            var plannedDays = (entity.PlannedEndDate - entity.PlannedStartDate).Days;
            var progressDays = (int)(plannedDays * entity.ProgressPercentage / 100);
            var expectedDate = entity.PlannedStartDate.AddDays(progressDays);
            
            return (DateTime.UtcNow - expectedDate).Days;
        }

        /// <summary>
        /// Gets the unit of work instance
        /// </summary>
        public IUnitOfWork UnitOfWork => _unitOfWork;
    }
}