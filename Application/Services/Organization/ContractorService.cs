using Application.Interfaces.Organization;
using Application.Services.Base;
using AutoMapper;
using Core.DTOs.Organization.Contractor;
using Core.Enums.Projects;
using Domain.Entities.Organization.Core;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Application.Services.Organization
{
    /// <summary>
    /// Service implementation for Contractor/Vendor management
    /// </summary>
    public class ContractorService : BaseService<Contractor, ContractorDto, CreateContractorDto, UpdateContractorDto>, IContractorService
    {
        public ContractorService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<ContractorService> logger)
            : base(unitOfWork, mapper, logger)
        {
        }

        /// <summary>
        /// Gets a contractor by code
        /// </summary>
        public async Task<ContractorDto?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(code))
                return null;

            var entity = await _unitOfWork.Repository<Contractor>()
                .GetAsync(
                    filter: c => c.Code == code && !c.IsDeleted,
                    cancellationToken: cancellationToken);

            if (entity == null)
                return null;

            var dto = _mapper.Map<ContractorDto>(entity);
            MapComputedProperties(dto, entity);
            
            return dto;
        }

        /// <summary>
        /// Gets contractors by type
        /// </summary>
        public async Task<IEnumerable<ContractorDto>> GetByTypeAsync(ContractorType type, CancellationToken cancellationToken = default)
        {
            var contractors = await _unitOfWork.Repository<Contractor>()
                .GetAllAsync(
                    filter: c => c.Type == type && !c.IsDeleted && c.IsActive,
                    orderBy: q => q.OrderBy(c => c.Name),
                    cancellationToken: cancellationToken);

            var dtos = contractors.Select(c =>
            {
                var dto = _mapper.Map<ContractorDto>(c);
                MapComputedProperties(dto, c);
                return dto;
            });

            return dtos;
        }

        /// <summary>
        /// Gets prequalified contractors
        /// </summary>
        public async Task<IEnumerable<ContractorDto>> GetPrequalifiedAsync(CancellationToken cancellationToken = default)
        {
            var contractors = await _unitOfWork.Repository<Contractor>()
                .GetAllAsync(
                    filter: c => c.IsPrequalified && !c.IsDeleted && c.IsActive,
                    orderBy: q => q.OrderBy(c => c.Name),
                    cancellationToken: cancellationToken);

            var dtos = contractors.Select(c =>
            {
                var dto = _mapper.Map<ContractorDto>(c);
                MapComputedProperties(dto, c);
                return dto;
            });

            return dtos;
        }

        /// <summary>
        /// Prequalifies a contractor
        /// </summary>
        public async Task<ContractorDto?> PrequalifyAsync(
            Guid id,
            PrequalifyContractorDto dto,
            string? approvedBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Contractor>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null || entity.IsDeleted)
                return null;

            entity.Prequalify(dto.Notes, approvedBy ?? "System");

            _unitOfWork.Repository<Contractor>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Contractor {ContractorCode} prequalified by {User}",
                entity.Code, approvedBy ?? "System");

            var resultDto = _mapper.Map<ContractorDto>(entity);
            MapComputedProperties(resultDto, entity);
            
            return resultDto;
        }

        /// <summary>
        /// Updates contractor contact information
        /// </summary>
        public async Task<ContractorDto?> UpdateContactInfoAsync(
            Guid id,
            string? contactName,
            string? email,
            string? phone,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Contractor>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null || entity.IsDeleted)
                return null;

            entity.UpdateContactInfo(contactName, email, phone);
            entity.UpdatedBy = updatedBy ?? "System";

            _unitOfWork.Repository<Contractor>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = _mapper.Map<ContractorDto>(entity);
            MapComputedProperties(dto, entity);
            
            return dto;
        }

        /// <summary>
        /// Updates contractor financial information
        /// </summary>
        public async Task<ContractorDto?> UpdateFinancialInfoAsync(
            Guid id,
            string? bankName,
            string? bankAccount,
            string? paymentTerms,
            decimal creditLimit,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Contractor>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null || entity.IsDeleted)
                return null;

            entity.UpdateFinancialInfo(bankName, bankAccount, paymentTerms, creditLimit);
            entity.UpdatedBy = updatedBy ?? "System";

            _unitOfWork.Repository<Contractor>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = _mapper.Map<ContractorDto>(entity);
            MapComputedProperties(dto, entity);
            
            return dto;
        }

        /// <summary>
        /// Updates contractor insurance information
        /// </summary>
        public async Task<ContractorDto?> UpdateInsuranceAsync(
            Guid id,
            UpdateContractorInsuranceDto dto,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Contractor>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null || entity.IsDeleted)
                return null;

            entity.UpdateInsurance(dto.ExpiryDate, dto.Amount, dto.Company, dto.PolicyNumber);
            entity.UpdatedBy = updatedBy ?? "System";

            _unitOfWork.Repository<Contractor>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Contractor {ContractorCode} insurance updated by {User}",
                entity.Code, updatedBy ?? "System");

            var resultDto = _mapper.Map<ContractorDto>(entity);
            MapComputedProperties(resultDto, entity);
            
            return resultDto;
        }

        /// <summary>
        /// Updates contractor status
        /// </summary>
        public async Task<ContractorDto?> UpdateStatusAsync(
            Guid id,
            ContractorStatus status,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Contractor>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null || entity.IsDeleted)
                return null;

            entity.UpdateStatus(status, updatedBy ?? "System");

            _unitOfWork.Repository<Contractor>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Contractor {ContractorCode} status changed to {Status} by {User}",
                entity.Code, status, updatedBy ?? "System");

            var dto = _mapper.Map<ContractorDto>(entity);
            MapComputedProperties(dto, entity);
            
            return dto;
        }

        /// <summary>
        /// Updates contractor performance rating
        /// </summary>
        public async Task<ContractorDto?> UpdatePerformanceRatingAsync(
            Guid id,
            decimal rating,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Contractor>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null || entity.IsDeleted)
                return null;

            entity.UpdatePerformanceRating(rating, updatedBy ?? "System");

            _unitOfWork.Repository<Contractor>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Contractor {ContractorCode} performance rating updated to {Rating} by {User}",
                entity.Code, rating, updatedBy ?? "System");

            var dto = _mapper.Map<ContractorDto>(entity);
            MapComputedProperties(dto, entity);
            
            return dto;
        }

        /// <summary>
        /// Gets contractors with expired insurance
        /// </summary>
        public async Task<IEnumerable<ContractorDto>> GetExpiredInsuranceAsync(CancellationToken cancellationToken = default)
        {
            var contractors = await _unitOfWork.Repository<Contractor>()
                .GetAllAsync(
                    filter: c => !c.IsDeleted && c.HasInsurance && c.InsuranceExpiryDate < DateTime.UtcNow,
                    orderBy: q => q.OrderBy(c => c.InsuranceExpiryDate),
                    cancellationToken: cancellationToken);

            var dtos = contractors.Select(c =>
            {
                var dto = _mapper.Map<ContractorDto>(c);
                MapComputedProperties(dto, c);
                return dto;
            });

            return dtos;
        }

        /// <summary>
        /// Gets contractors eligible for award
        /// </summary>
        public async Task<IEnumerable<ContractorDto>> GetEligibleForAwardAsync(CancellationToken cancellationToken = default)
        {
            var contractors = await _unitOfWork.Repository<Contractor>()
                .GetAllAsync(
                    filter: c => !c.IsDeleted && c.IsActive && c.IsPrequalified && 
                             c.Status == ContractorStatus.Active &&
                             (!c.HasInsurance || c.InsuranceExpiryDate > DateTime.UtcNow),
                    orderBy: q => q.OrderBy(c => c.Name),
                    cancellationToken: cancellationToken);

            var dtos = contractors.Select(c =>
            {
                var dto = _mapper.Map<ContractorDto>(c);
                MapComputedProperties(dto, c);
                return dto;
            });

            return dtos;
        }

        /// <summary>
        /// Gets contractor with commitments summary
        /// </summary>
        public async Task<ContractorDto?> GetWithCommitmentsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Contractor>()
                .GetAsync(
                    filter: c => c.Id == id && !c.IsDeleted,
                    includeProperties: "Commitments",
                    cancellationToken: cancellationToken);

            if (entity == null)
                return null;

            var dto = _mapper.Map<ContractorDto>(entity);
            MapComputedProperties(dto, entity);
            
            return dto;
        }

        /// <summary>
        /// Checks if contractor code is unique
        /// </summary>
        public async Task<bool> IsCodeUniqueAsync(
            string code,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default)
        {
            var exists = await _unitOfWork.Repository<Contractor>()
                .GetAsync(
                    filter: c => c.Code == code && c.Id != excludeId && !c.IsDeleted,
                    cancellationToken: cancellationToken);

            return exists == null;
        }

        /// <summary>
        /// Override create to handle entity creation properly
        /// </summary>
        public override async Task<ContractorDto> CreateAsync(
            CreateContractorDto createDto,
            string? createdBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = new Contractor(
                createDto.Code,
                createDto.Name,
                createDto.TaxId,
                createDto.Type);

            // Set basic properties
            entity.TradeName = createDto.TradeName;
            entity.Classification = createDto.Classification;
            entity.DefaultCurrency = createDto.DefaultCurrency;

            // Set contact info
            if (!string.IsNullOrWhiteSpace(createDto.ContactName) || 
                !string.IsNullOrWhiteSpace(createDto.Email) || 
                !string.IsNullOrWhiteSpace(createDto.Phone))
            {
                entity.UpdateContactInfo(createDto.ContactName, createDto.Email, createDto.Phone);
            }
            entity.ContactTitle = createDto.ContactTitle;
            entity.MobilePhone = createDto.MobilePhone;
            entity.Website = createDto.Website;

            // Set address
            if (!string.IsNullOrWhiteSpace(createDto.Address) || 
                !string.IsNullOrWhiteSpace(createDto.City) || 
                !string.IsNullOrWhiteSpace(createDto.Country))
            {
                entity.UpdateAddress(
                    createDto.Address,
                    createDto.City,
                    createDto.State,
                    createDto.Country,
                    createDto.PostalCode);
            }

            // Set financial info
            if (!string.IsNullOrWhiteSpace(createDto.BankName) || 
                !string.IsNullOrWhiteSpace(createDto.BankAccount) || 
                createDto.CreditLimit > 0)
            {
                entity.UpdateFinancialInfo(
                    createDto.BankName,
                    createDto.BankAccount,
                    createDto.PaymentTerms,
                    createDto.CreditLimit);
            }
            entity.BankRoutingNumber = createDto.BankRoutingNumber;

            // Set insurance info if provided
            if (createDto.HasInsurance && createDto.InsuranceExpiryDate.HasValue)
            {
                entity.UpdateInsurance(
                    createDto.InsuranceExpiryDate.Value,
                    createDto.InsuranceAmount ?? 0,
                    createDto.InsuranceCompany ?? string.Empty,
                    createDto.InsurancePolicyNumber ?? string.Empty);
            }

            // Set prequalification if needed
            if (createDto.IsPrequalified)
            {
                entity.Prequalify(createDto.PrequalificationNotes ?? string.Empty, createdBy ?? "System");
            }

            // Set certifications
            entity.Certifications = createDto.Certifications;
            entity.SpecialtyAreas = createDto.SpecialtyAreas;

            entity.CreatedBy = createdBy ?? "System";
            entity.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.Repository<Contractor>().AddAsync(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Contractor {ContractorCode} created by {User}",
                entity.Code, createdBy ?? "System");

            var dto = _mapper.Map<ContractorDto>(entity);
            MapComputedProperties(dto, entity);
            
            return dto;
        }

        /// <summary>
        /// Override update to use entity methods
        /// </summary>
        public override async Task<ContractorDto?> UpdateAsync(
            Guid id,
            UpdateContractorDto updateDto,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Contractor>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null || entity.IsDeleted)
                return null;

            // Update basic properties
            entity.Name = updateDto.Name;
            entity.TradeName = updateDto.TradeName;
            entity.Classification = updateDto.Classification;
            entity.DefaultCurrency = updateDto.DefaultCurrency;

            // Update contact info
            entity.UpdateContactInfo(updateDto.ContactName, updateDto.Email, updateDto.Phone);
            entity.ContactTitle = updateDto.ContactTitle;
            entity.MobilePhone = updateDto.MobilePhone;
            entity.Website = updateDto.Website;

            // Update address
            entity.UpdateAddress(
                updateDto.Address,
                updateDto.City,
                updateDto.State,
                updateDto.Country,
                updateDto.PostalCode);

            // Update financial info
            entity.UpdateFinancialInfo(
                updateDto.BankName,
                updateDto.BankAccount,
                updateDto.PaymentTerms,
                updateDto.CreditLimit);
            entity.BankRoutingNumber = updateDto.BankRoutingNumber;

            // Update certifications
            entity.Certifications = updateDto.Certifications;
            entity.SpecialtyAreas = updateDto.SpecialtyAreas;

            // Update active status
            entity.IsActive = updateDto.IsActive;

            entity.UpdatedBy = updatedBy ?? "System";
            entity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Contractor>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Contractor {ContractorCode} updated by {User}",
                entity.Code, updatedBy ?? "System");

            var dto = _mapper.Map<ContractorDto>(entity);
            MapComputedProperties(dto, entity);
            
            return dto;
        }

        /// <summary>
        /// Override GetByIdAsync to include computed properties
        /// </summary>
        public override async Task<ContractorDto?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Contractor>()
                .GetByIdAsync(id, cancellationToken);

            if (entity == null || entity.IsDeleted)
                return null;

            var dto = _mapper.Map<ContractorDto>(entity);
            MapComputedProperties(dto, entity);
            
            return dto;
        }

        /// <summary>
        /// Override GetAllAsync to include computed properties
        /// </summary>
        public override async Task<IEnumerable<ContractorDto>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            var entities = await _unitOfWork.Repository<Contractor>()
                .GetAllAsync(
                    filter: c => !c.IsDeleted,
                    orderBy: q => q.OrderBy(c => c.Name),
                    cancellationToken: cancellationToken);

            var dtos = entities.Select(c =>
            {
                var dto = _mapper.Map<ContractorDto>(c);
                MapComputedProperties(dto, c);
                return dto;
            });

            return dtos;
        }

        /// <summary>
        /// Maps computed properties from entity to DTO
        /// </summary>
        private void MapComputedProperties(ContractorDto dto, Contractor entity)
        {
            dto.IsInsuranceExpired = entity.IsInsuranceExpired();
            dto.CanBeAwarded = entity.CanBeAwarded();
            dto.TotalCommitments = entity.GetTotalCommitments();
            dto.OpenCommitments = entity.GetOpenCommitments();
        }

        /// <summary>
        /// Gets the unit of work instance
        /// </summary>
        public IUnitOfWork UnitOfWork => _unitOfWork;
    }
}