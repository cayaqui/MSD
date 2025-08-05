using Application.Interfaces.Organization;
using Application.Services.Base;
using AutoMapper;
using Core.DTOs.Organization.Discipline;
using Domain.Entities.Organization.Core;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services.Organization
{
    /// <summary>
    /// Service implementation for Discipline management
    /// </summary>
    public class DisciplineService : BaseService<Discipline, DisciplineDto, CreateDisciplineDto, UpdateDisciplineDto>, IDisciplineService
    {
        public DisciplineService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<DisciplineService> logger)
            : base(unitOfWork, mapper, logger)
        {
        }

        /// <summary>
        /// Gets a discipline by its code
        /// </summary>
        public async Task<DisciplineDto?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Discipline>()
                .GetAsync(
                    filter: d => d.Code == code && !d.IsDeleted,
                    cancellationToken: cancellationToken);

            return entity != null ? _mapper.Map<DisciplineDto>(entity) : null;
        }

        /// <summary>
        /// Gets all engineering disciplines
        /// </summary>
        public async Task<IEnumerable<DisciplineDto>> GetEngineeringDisciplinesAsync(CancellationToken cancellationToken = default)
        {
            var entities = await _unitOfWork.Repository<Discipline>()
                .GetAllAsync(
                    filter: d => d.IsEngineering && !d.IsDeleted,
                    orderBy: q => q.OrderBy(d => d.Order).ThenBy(d => d.Name),
                    cancellationToken: cancellationToken);

            return _mapper.Map<IEnumerable<DisciplineDto>>(entities);
        }

        /// <summary>
        /// Gets all management disciplines
        /// </summary>
        public async Task<IEnumerable<DisciplineDto>> GetManagementDisciplinesAsync(CancellationToken cancellationToken = default)
        {
            var entities = await _unitOfWork.Repository<Discipline>()
                .GetAllAsync(
                    filter: d => d.IsManagement && !d.IsDeleted,
                    orderBy: q => q.OrderBy(d => d.Order).ThenBy(d => d.Name),
                    cancellationToken: cancellationToken);

            return _mapper.Map<IEnumerable<DisciplineDto>>(entities);
        }

        /// <summary>
        /// Updates discipline basic information
        /// </summary>
        public async Task<DisciplineDto?> UpdateBasicInfoAsync(
            Guid id, 
            string name, 
            string? description, 
            int order, 
            string? updatedBy = null, 
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Discipline>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null)
                return null;

            entity.UpdateBasicInfo(name, description, order);
            entity.UpdatedBy = updatedBy ?? "System";
            entity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Discipline>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Discipline {DisciplineCode} basic info updated by {User}",
                entity.Code, updatedBy ?? "System");

            return _mapper.Map<DisciplineDto>(entity);
        }

        /// <summary>
        /// Updates discipline visual settings
        /// </summary>
        public async Task<DisciplineDto?> UpdateVisualAsync(
            Guid id, 
            string colorHex, 
            string? icon, 
            string? updatedBy = null, 
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Discipline>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null)
                return null;

            entity.UpdateVisual(colorHex, icon);
            entity.UpdatedBy = updatedBy ?? "System";
            entity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Discipline>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Discipline {DisciplineCode} visual settings updated by {User}",
                entity.Code, updatedBy ?? "System");

            return _mapper.Map<DisciplineDto>(entity);
        }

        /// <summary>
        /// Updates discipline category
        /// </summary>
        public async Task<DisciplineDto?> UpdateCategoryAsync(
            Guid id, 
            bool isEngineering, 
            bool isManagement, 
            string? updatedBy = null, 
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Discipline>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null)
                return null;

            entity.UpdateCategory(isEngineering, isManagement);
            entity.UpdatedBy = updatedBy ?? "System";
            entity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Discipline>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Discipline {DisciplineCode} category updated by {User}",
                entity.Code, updatedBy ?? "System");

            return _mapper.Map<DisciplineDto>(entity);
        }

        /// <summary>
        /// Gets disciplines ordered by their order field
        /// </summary>
        public async Task<IEnumerable<DisciplineDto>> GetOrderedAsync(CancellationToken cancellationToken = default)
        {
            var entities = await _unitOfWork.Repository<Discipline>()
                .GetAllAsync(
                    filter: d => !d.IsDeleted,
                    orderBy: q => q.OrderBy(d => d.Order).ThenBy(d => d.Name),
                    cancellationToken: cancellationToken);

            return _mapper.Map<IEnumerable<DisciplineDto>>(entities);
        }

        /// <summary>
        /// Checks if discipline code is unique
        /// </summary>
        public async Task<bool> IsCodeUniqueAsync(string code, Guid? excludeId = null, CancellationToken cancellationToken = default)
        {
            var existing = await _unitOfWork.Repository<Discipline>()
                .GetAsync(
                    filter: d => d.Code == code && !d.IsDeleted && (excludeId == null || d.Id != excludeId),
                    cancellationToken: cancellationToken);

            return existing == null;
        }

        /// <summary>
        /// Override create to handle entity creation properly
        /// </summary>
        public override async Task<DisciplineDto> CreateAsync(
            CreateDisciplineDto createDto,
            string? createdBy = null,
            CancellationToken cancellationToken = default)
        {
            // Check if code is unique
            var isUnique = await IsCodeUniqueAsync(createDto.Code, cancellationToken: cancellationToken);
            if (!isUnique)
            {
                throw new InvalidOperationException($"Discipline with code '{createDto.Code}' already exists");
            }

            var entity = new Discipline(
                createDto.Code,
                createDto.Name,
                createDto.ColorHex,
                createDto.Order,
                createDto.IsEngineering);

            entity.Description = createDto.Description;
            
            if (!string.IsNullOrWhiteSpace(createDto.Icon))
            {
                entity.SetIcon(createDto.Icon);
            }

            entity.CreatedBy = createdBy ?? "System";
            entity.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.Repository<Discipline>().AddAsync(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Discipline {DisciplineCode} created by {User}",
                entity.Code, createdBy ?? "System");

            return _mapper.Map<DisciplineDto>(entity);
        }

        /// <summary>
        /// Override update to use entity methods
        /// </summary>
        public override async Task<DisciplineDto?> UpdateAsync(
            Guid id,
            UpdateDisciplineDto updateDto,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Discipline>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null)
                return null;

            // Update basic info
            entity.UpdateBasicInfo(updateDto.Name, updateDto.Description, updateDto.Order);

            // Update visual settings
            entity.UpdateVisual(updateDto.ColorHex, updateDto.Icon);

            // Update category
            entity.UpdateCategory(updateDto.IsEngineering, updateDto.IsManagement);

            entity.UpdatedBy = updatedBy ?? "System";
            entity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Discipline>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Discipline {DisciplineCode} updated by {User}",
                entity.Code, updatedBy ?? "System");

            return _mapper.Map<DisciplineDto>(entity);
        }

        /// <summary>
        /// Override GetBaseFilter to filter by IsDeleted
        /// </summary>
        protected override Expression<Func<Discipline, bool>> GetBaseFilter()
        {
            return entity => !entity.IsDeleted;
        }

        /// <summary>
        /// Gets the unit of work instance
        /// </summary>
        public IUnitOfWork UnitOfWork => _unitOfWork;
    }
}