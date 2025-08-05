using Application.Interfaces.Organization;
using Application.Services.Base;
using AutoMapper;
using Core.DTOs.Organization.Company;
using Domain.Entities.Organization.Core;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services.Organization
{
    /// <summary>
    /// Service implementation for Company management
    /// </summary>
    public class CompanyService : BaseService<Company, CompanyDto, CreateCompanyDto, UpdateCompanyDto>, ICompanyService
    {
        public CompanyService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CompanyService> logger)
            : base(unitOfWork, mapper, logger)
        {
        }

        /// <summary>
        /// Gets a company by its code
        /// </summary>
        public async Task<CompanyDto?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(code))
                return null;

            var entity = await _unitOfWork.Repository<Company>()
                .GetAsync(
                    filter: c => c.Code == code && !c.IsDeleted,
                    cancellationToken: cancellationToken);

            return entity != null ? _mapper.Map<CompanyDto>(entity) : null;
        }

        /// <summary>
        /// Updates company business information
        /// </summary>
        public async Task<CompanyDto?> UpdateBusinessInfoAsync(
            Guid id,
            UpdateCompanyBusinessInfoDto dto,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Company>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null || entity.IsDeleted)
                return null;

            entity.UpdateBusinessInfo(dto.Name, dto.Description, dto.TaxId);
            entity.UpdatedBy = updatedBy ?? "System";
            entity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Company>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<CompanyDto>(entity);
        }

        /// <summary>
        /// Updates company address
        /// </summary>
        public async Task<CompanyDto?> UpdateAddressAsync(
            Guid id,
            UpdateCompanyAddressDto dto,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Company>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null || entity.IsDeleted)
                return null;

            entity.UpdateAddress(
                dto.Address,
                dto.City,
                dto.State,
                dto.Country,
                dto.PostalCode);

            entity.UpdatedBy = updatedBy ?? "System";
            entity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Company>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<CompanyDto>(entity);
        }

        /// <summary>
        /// Updates company contact information
        /// </summary>
        public async Task<CompanyDto?> UpdateContactInfoAsync(
            Guid id,
            UpdateCompanyContactDto dto,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Company>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null || entity.IsDeleted)
                return null;

            entity.UpdateContactInfo(dto.Phone, dto.Email, dto.Website);
            entity.UpdatedBy = updatedBy ?? "System";
            entity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Company>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<CompanyDto>(entity);
        }

        /// <summary>
        /// Updates company logo
        /// </summary>
        public async Task<CompanyDto?> UpdateLogoAsync(
            Guid id,
            UpdateCompanyLogoDto dto,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Company>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null || entity.IsDeleted)
                return null;

            entity.UpdateLogo(dto.Logo, dto.ContentType);
            entity.UpdatedBy = updatedBy ?? "System";
            entity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Company>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<CompanyDto>(entity);
        }

        /// <summary>
        /// Updates company configuration
        /// </summary>
        public async Task<CompanyDto?> UpdateConfigurationAsync(
            Guid id,
            UpdateCompanyConfigDto dto,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Company>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null || entity.IsDeleted)
                return null;

            entity.UpdateConfiguration(dto.DefaultCurrency, dto.FiscalYearStart);
            entity.UpdatedBy = updatedBy ?? "System";
            entity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Company>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<CompanyDto>(entity);
        }

        /// <summary>
        /// Gets companies with their operations
        /// </summary>
        public async Task<IEnumerable<CompanyWithOperationsDto>> GetCompaniesWithOperationsAsync(
            CancellationToken cancellationToken = default)
        {
            var companies = await _unitOfWork.Repository<Company>()
                .GetAllAsync(
                    filter: c => !c.IsDeleted,
                    includeProperties: "Operations",
                    cancellationToken: cancellationToken);

            return _mapper.Map<IEnumerable<CompanyWithOperationsDto>>(companies);
        }

        /// <summary>
        /// Checks if a company code is unique
        /// </summary>
        public async Task<bool> IsCodeUniqueAsync(
            string code,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default)
        {
            var exists = await _unitOfWork.Repository<Company>()
                .GetAsync(
                    filter: c => c.Code == code && !c.IsDeleted && c.Id != excludeId,
                    cancellationToken: cancellationToken);

            return exists == null;
        }

        /// <summary>
        /// Override create to handle entity creation properly
        /// </summary>
        public override async Task<CompanyDto> CreateAsync(
            CreateCompanyDto createDto,
            string? createdBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = new Company(createDto.Code, createDto.Name, createDto.TaxId);
            
            entity.Description = createDto.Description;
            
            if (!string.IsNullOrEmpty(createDto.DefaultCurrency))
                entity.SetCurrency(createDto.DefaultCurrency);
            
            entity.CreatedBy = createdBy ?? "System";
            entity.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.Repository<Company>().AddAsync(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Company with ID {CompanyId} created by {User}",
                entity.Id, createdBy ?? "System");

            return _mapper.Map<CompanyDto>(entity);
        }

        /// <summary>
        /// Override update to use entity methods
        /// </summary>
        public override async Task<CompanyDto?> UpdateAsync(
            Guid id,
            UpdateCompanyDto updateDto,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Company>().GetByIdAsync(id);
            if (entity == null || entity.IsDeleted)
                return null;

            if (updateDto.Name != null && updateDto.TaxId != null)
                entity.UpdateBusinessInfo(updateDto.Name, updateDto.Description, updateDto.TaxId);
            entity.UpdateAddress(
                updateDto.Address,
                updateDto.City,
                updateDto.State,
                updateDto.Country,
                updateDto.PostalCode);
            entity.UpdateContactInfo(updateDto.Phone, updateDto.Email, updateDto.Website);
            
            if (!string.IsNullOrEmpty(updateDto.DefaultCurrency))
                entity.SetCurrency(updateDto.DefaultCurrency);
            
            entity.UpdatedBy = updatedBy ?? "System";
            entity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Company>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Company with ID {CompanyId} updated by {User}",
                entity.Id, updatedBy ?? "System");

            return _mapper.Map<CompanyDto>(entity);
        }

        /// <summary>
        /// Gets the unit of work instance
        /// </summary>
        public IUnitOfWork UnitOfWork => _unitOfWork;
    }
}