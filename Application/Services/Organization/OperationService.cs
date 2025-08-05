using Application.Interfaces.Organization;
using Application.Services.Base;
using AutoMapper;
using Core.DTOs.Organization.Operation;
using Domain.Entities.Organization.Core;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services.Organization
{
    /// <summary>
    /// Service implementation for Operation management
    /// </summary>
    public class OperationService : BaseService<Operation, OperationDto, CreateOperationDto, UpdateOperationDto>, IOperationService
    {
        public OperationService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<OperationService> logger)
            : base(unitOfWork, mapper, logger)
        {
        }

        /// <summary>
        /// Gets an operation by its code
        /// </summary>
        public async Task<OperationDto?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(code))
                return null;

            var entity = await _unitOfWork.Repository<Operation>()
                .GetAsync(
                    filter: o => o.Code == code && !o.IsDeleted,
                    includeProperties: "Company",
                    cancellationToken: cancellationToken);

            return entity != null ? _mapper.Map<OperationDto>(entity) : null;
        }

        /// <summary>
        /// Gets operations by company
        /// </summary>
        public async Task<IEnumerable<OperationDto>> GetByCompanyAsync(
            Guid companyId, 
            CancellationToken cancellationToken = default)
        {
            var operations = await _unitOfWork.Repository<Operation>()
                .GetAllAsync(
                    filter: o => o.CompanyId == companyId && !o.IsDeleted,
                    includeProperties: "Company",
                    orderBy: q => q.OrderBy(o => o.Code),
                    cancellationToken: cancellationToken);

            return _mapper.Map<IEnumerable<OperationDto>>(operations);
        }

        /// <summary>
        /// Gets operations by country
        /// </summary>
        public async Task<IEnumerable<OperationDto>> GetByCountryAsync(
            string country, 
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(country))
                return Enumerable.Empty<OperationDto>();

            var operations = await _unitOfWork.Repository<Operation>()
                .GetAllAsync(
                    filter: o => o.Country == country && !o.IsDeleted,
                    includeProperties: "Company",
                    orderBy: q => q.OrderBy(o => o.Code),
                    cancellationToken: cancellationToken);

            return _mapper.Map<IEnumerable<OperationDto>>(operations);
        }

        /// <summary>
        /// Updates operation basic information
        /// </summary>
        public async Task<OperationDto?> UpdateBasicInfoAsync(
            Guid id,
            UpdateOperationBasicInfoDto dto,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Operation>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null || entity.IsDeleted)
                return null;

            entity.UpdateBasicInfo(dto.Name, dto.Description, dto.Location);
            entity.UpdatedBy = updatedBy ?? "System";
            entity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Operation>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Reload with Company
            entity = await _unitOfWork.Repository<Operation>()
                .GetAsync(
                    filter: o => o.Id == id,
                    includeProperties: "Company",
                    cancellationToken: cancellationToken);

            return _mapper.Map<OperationDto>(entity);
        }

        /// <summary>
        /// Updates operation address
        /// </summary>
        public async Task<OperationDto?> UpdateAddressAsync(
            Guid id,
            UpdateOperationAddressDto dto,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Operation>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null || entity.IsDeleted)
                return null;

            entity.UpdateAddress(dto.Address, dto.City, dto.State, dto.Country);
            entity.UpdatedBy = updatedBy ?? "System";
            entity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Operation>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Reload with Company
            entity = await _unitOfWork.Repository<Operation>()
                .GetAsync(
                    filter: o => o.Id == id,
                    includeProperties: "Company",
                    cancellationToken: cancellationToken);

            return _mapper.Map<OperationDto>(entity);
        }

        /// <summary>
        /// Updates operation manager
        /// </summary>
        public async Task<OperationDto?> UpdateManagerAsync(
            Guid id,
            UpdateOperationManagerDto dto,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Operation>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null || entity.IsDeleted)
                return null;

            entity.UpdateManager(dto.ManagerName, dto.ManagerEmail);
            entity.UpdatedBy = updatedBy ?? "System";
            entity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Operation>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Reload with Company
            entity = await _unitOfWork.Repository<Operation>()
                .GetAsync(
                    filter: o => o.Id == id,
                    includeProperties: "Company",
                    cancellationToken: cancellationToken);

            return _mapper.Map<OperationDto>(entity);
        }

        /// <summary>
        /// Updates operation cost center
        /// </summary>
        public async Task<OperationDto?> UpdateCostCenterAsync(
            Guid id,
            string costCenter,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Operation>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null || entity.IsDeleted)
                return null;

            entity.UpdateCostCenter(costCenter);
            entity.UpdatedBy = updatedBy ?? "System";
            entity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Operation>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Reload with Company
            entity = await _unitOfWork.Repository<Operation>()
                .GetAsync(
                    filter: o => o.Id == id,
                    includeProperties: "Company",
                    cancellationToken: cancellationToken);

            return _mapper.Map<OperationDto>(entity);
        }

        /// <summary>
        /// Gets operation with projects
        /// </summary>
        public async Task<OperationWithProjectsDto?> GetWithProjectsAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Operation>()
                .GetAsync(
                    filter: o => o.Id == id && !o.IsDeleted,
                    includeProperties: "Company,Projects",
                    cancellationToken: cancellationToken);

            return entity != null ? _mapper.Map<OperationWithProjectsDto>(entity) : null;
        }

        /// <summary>
        /// Checks if operation code is unique within company
        /// </summary>
        public async Task<bool> IsCodeUniqueAsync(
            string code,
            Guid companyId,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default)
        {
            var exists = await _unitOfWork.Repository<Operation>()
                .GetAsync(
                    filter: o => o.Code == code && 
                           o.CompanyId == companyId && 
                           !o.IsDeleted && 
                           o.Id != excludeId,
                    cancellationToken: cancellationToken);

            return exists == null;
        }

        /// <summary>
        /// Override create to handle entity creation properly
        /// </summary>
        public override async Task<OperationDto> CreateAsync(
            CreateOperationDto createDto,
            string? createdBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = new Operation(createDto.CompanyId, createDto.Code, createDto.Name);
            
            entity.Description = createDto.Description;
            
            if (!string.IsNullOrWhiteSpace(createDto.Location) || 
                !string.IsNullOrWhiteSpace(createDto.Country))
            {
                entity.SetLocation(
                    createDto.Location,
                    createDto.Address,
                    createDto.City,
                    createDto.State,
                    createDto.Country ?? string.Empty);
            }
            
            if (!string.IsNullOrWhiteSpace(createDto.ManagerName) || 
                !string.IsNullOrWhiteSpace(createDto.ManagerEmail))
            {
                entity.UpdateManager(createDto.ManagerName, createDto.ManagerEmail);
            }
            
            if (!string.IsNullOrWhiteSpace(createDto.CostCenter))
            {
                entity.SetCostCenter(createDto.CostCenter);
            }
            
            entity.CreatedBy = createdBy ?? "System";
            entity.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.Repository<Operation>().AddAsync(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Reload with Company
            entity = await _unitOfWork.Repository<Operation>()
                .GetAsync(
                    filter: o => o.Id == entity.Id,
                    includeProperties: "Company",
                    cancellationToken: cancellationToken);

            _logger.LogInformation("Operation with ID {OperationId} created by {User}",
                entity!.Id, createdBy ?? "System");

            return _mapper.Map<OperationDto>(entity);
        }

        /// <summary>
        /// Override update to use entity methods
        /// </summary>
        public override async Task<OperationDto?> UpdateAsync(
            Guid id,
            UpdateOperationDto updateDto,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Operation>()
                .GetByIdAsync(id, cancellationToken);
                
            if (entity == null || entity.IsDeleted)
                return null;

            // Update basic info if provided
            if (!string.IsNullOrWhiteSpace(updateDto.Name))
            {
                entity.UpdateBasicInfo(updateDto.Name, updateDto.Description, updateDto.Location);
            }

            // Update address if any field is provided
            if (updateDto.Address != null || updateDto.City != null || 
                updateDto.State != null || updateDto.Country != null)
            {
                entity.UpdateAddress(updateDto.Address, updateDto.City, updateDto.State, updateDto.Country);
            }

            // Update manager if any field is provided
            if (updateDto.ManagerName != null || updateDto.ManagerEmail != null)
            {
                entity.UpdateManager(updateDto.ManagerName, updateDto.ManagerEmail);
            }

            // Update cost center if provided
            if (updateDto.CostCenter != null)
            {
                entity.UpdateCostCenter(updateDto.CostCenter);
            }
            
            entity.UpdatedBy = updatedBy ?? "System";
            entity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Operation>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Reload with Company
            entity = await _unitOfWork.Repository<Operation>()
                .GetAsync(
                    filter: o => o.Id == id,
                    includeProperties: "Company",
                    cancellationToken: cancellationToken);

            _logger.LogInformation("Operation with ID {OperationId} updated by {User}",
                entity!.Id, updatedBy ?? "System");

            return _mapper.Map<OperationDto>(entity);
        }

        /// <summary>
        /// Override GetByIdAsync to include Company
        /// </summary>
        public override async Task<OperationDto?> GetByIdAsync(
            Guid id, 
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Operation>()
                .GetAsync(
                    filter: o => o.Id == id && !o.IsDeleted,
                    includeProperties: "Company",
                    cancellationToken: cancellationToken);

            return entity != null ? _mapper.Map<OperationDto>(entity) : null;
        }

        /// <summary>
        /// Override GetAllAsync to include Company
        /// </summary>
        public override async Task<IEnumerable<OperationDto>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            var entities = await _unitOfWork.Repository<Operation>()
                .GetAllAsync(
                    filter: o => !o.IsDeleted,
                    includeProperties: "Company",
                    orderBy: q => q.OrderBy(o => o.Code),
                    cancellationToken: cancellationToken);

            return _mapper.Map<IEnumerable<OperationDto>>(entities);
        }

        /// <summary>
        /// Gets the unit of work instance
        /// </summary>
        public IUnitOfWork UnitOfWork => _unitOfWork;
    }
}