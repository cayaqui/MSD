using Application.Interfaces.Organization;
using Application.Services.Base;
using Core.DTOs.Organization.Contractor;
using Core.Enums.Projects;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services.Organization;

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

    public IUnitOfWork UnitOfWork => _unitOfWork;

    public async Task<ContractorDto?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<Contractor>()
            .GetAsync(
                filter: c => c.Code == code && !c.IsDeleted,
                cancellationToken: cancellationToken);

        return entity != null ? _mapper.Map<ContractorDto>(entity) : null;
    }

    public async Task<IEnumerable<ContractorDto>> GetByTypeAsync(ContractorType type, CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.Repository<Contractor>()
            .GetAllAsync(
                filter: c => c.Type == type && !c.IsDeleted,
                orderBy: q => q.OrderBy(c => c.Name),
                cancellationToken: cancellationToken);

        return _mapper.Map<IEnumerable<ContractorDto>>(entities);
    }

    public async Task<IEnumerable<ContractorDto>> GetPrequalifiedAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.Repository<Contractor>()
            .GetAllAsync(
                filter: c => c.IsPrequalified && !c.IsDeleted,
                orderBy: q => q.OrderBy(c => c.Name),
                cancellationToken: cancellationToken);

        return _mapper.Map<IEnumerable<ContractorDto>>(entities);
    }

    public async Task<ContractorDto?> PrequalifyAsync(Guid id, PrequalifyContractorDto dto, string? approvedBy = null, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<Contractor>().GetByIdAsync(id);
        if (entity == null)
            return null;

        entity.IsPrequalified = true;
        entity.PrequalificationDate = dto.PrequalificationDate ?? DateTime.UtcNow;
        entity.PrequalificationExpiry = dto.PrequalificationExpiry;
        entity.PrequalificationScore = dto.PrequalificationScore;
        entity.PrequalificationComments = dto.PrequalificationComments;

        if (!string.IsNullOrEmpty(approvedBy))
        {
            entity.UpdatedBy = approvedBy;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        _unitOfWork.Repository<Contractor>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Contractor {ContractorCode} prequalified by {User}", entity.Code, approvedBy ?? "System");

        return _mapper.Map<ContractorDto>(entity);
    }

    public async Task<ContractorDto?> UpdateContactInfoAsync(Guid id, string? contactName, string? email, string? phone, string? updatedBy = null, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<Contractor>().GetByIdAsync(id);
        if (entity == null)
            return null;

        if (!string.IsNullOrEmpty(contactName))
            entity.ContactName = contactName;
        
        if (!string.IsNullOrEmpty(email))
            entity.Email = email;
        
        if (!string.IsNullOrEmpty(phone))
            entity.Phone = phone;

        if (!string.IsNullOrEmpty(updatedBy))
        {
            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        _unitOfWork.Repository<Contractor>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Contractor {ContractorCode} contact info updated by {User}", entity.Code, updatedBy ?? "System");

        return _mapper.Map<ContractorDto>(entity);
    }

    public async Task<ContractorDto?> UpdateFinancialInfoAsync(Guid id, string? bankName, string? bankAccount, string? paymentTerms, decimal creditLimit, string? updatedBy = null, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<Contractor>().GetByIdAsync(id);
        if (entity == null)
            return null;

        if (!string.IsNullOrEmpty(bankName))
            entity.BankName = bankName;
        
        if (!string.IsNullOrEmpty(bankAccount))
            entity.BankAccount = bankAccount;
        
        if (!string.IsNullOrEmpty(paymentTerms))
            entity.PaymentTerms = paymentTerms;
        
        entity.CreditLimit = creditLimit;

        if (!string.IsNullOrEmpty(updatedBy))
        {
            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        _unitOfWork.Repository<Contractor>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Contractor {ContractorCode} financial info updated by {User}", entity.Code, updatedBy ?? "System");

        return _mapper.Map<ContractorDto>(entity);
    }

    public async Task<ContractorDto?> UpdateInsuranceAsync(Guid id, UpdateContractorInsuranceDto dto, string? updatedBy = null, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<Contractor>().GetByIdAsync(id);
        if (entity == null)
            return null;

        entity.InsuranceCompany = dto.InsuranceCompany ?? entity.InsuranceCompany;
        entity.InsurancePolicyNumber = dto.InsurancePolicyNumber ?? entity.InsurancePolicyNumber;
        entity.InsuranceAmount = dto.InsuranceAmount;
        entity.InsuranceExpiry = dto.InsuranceExpiry;

        if (!string.IsNullOrEmpty(updatedBy))
        {
            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        _unitOfWork.Repository<Contractor>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Contractor {ContractorCode} insurance info updated by {User}", entity.Code, updatedBy ?? "System");

        return _mapper.Map<ContractorDto>(entity);
    }

    public async Task<ContractorDto?> UpdateStatusAsync(Guid id, ContractorStatus status, string? updatedBy = null, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<Contractor>().GetByIdAsync(id);
        if (entity == null)
            return null;

        entity.Status = status;

        if (!string.IsNullOrEmpty(updatedBy))
        {
            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        _unitOfWork.Repository<Contractor>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Contractor {ContractorCode} status changed to {Status} by {User}", entity.Code, status, updatedBy ?? "System");

        return _mapper.Map<ContractorDto>(entity);
    }

    public async Task<ContractorDto?> UpdatePerformanceRatingAsync(Guid id, decimal rating, string? updatedBy = null, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<Contractor>().GetByIdAsync(id);
        if (entity == null)
            return null;

        entity.PerformanceRating = rating;
        entity.LastEvaluationDate = DateTime.UtcNow;

        if (!string.IsNullOrEmpty(updatedBy))
        {
            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        _unitOfWork.Repository<Contractor>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Contractor {ContractorCode} performance rating updated to {Rating} by {User}", entity.Code, rating, updatedBy ?? "System");

        return _mapper.Map<ContractorDto>(entity);
    }

    public async Task<IEnumerable<ContractorDto>> GetExpiredInsuranceAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.Repository<Contractor>()
            .GetAllAsync(
                filter: c => c.InsuranceExpiry.HasValue && c.InsuranceExpiry.Value < DateTime.UtcNow && !c.IsDeleted,
                orderBy: q => q.OrderBy(c => c.InsuranceExpiry),
                cancellationToken: cancellationToken);

        return _mapper.Map<IEnumerable<ContractorDto>>(entities);
    }

    public async Task<IEnumerable<ContractorDto>> GetEligibleForAwardAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.Repository<Contractor>()
            .GetAllAsync(
                filter: c => c.Status == ContractorStatus.Active && 
                            c.IsPrequalified && 
                            (!c.InsuranceExpiry.HasValue || c.InsuranceExpiry.Value >= DateTime.UtcNow) &&
                            !c.IsDeleted,
                orderBy: q => q.OrderBy(c => c.Name),
                cancellationToken: cancellationToken);

        return _mapper.Map<IEnumerable<ContractorDto>>(entities);
    }

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
        
        // Calculate total commitments if there are any
        if (entity.Commitments?.Any() == true)
        {
            dto.TotalCommitments = entity.Commitments
                .Where(c => !c.IsDeleted)
                .Sum(c => c.ContractAmount);
        }

        return dto;
    }

    public async Task<bool> IsCodeUniqueAsync(string code, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var exists = await _unitOfWork.Repository<Contractor>()
            .AnyAsync(c => c.Code == code && 
                          (!excludeId.HasValue || c.Id != excludeId.Value) && 
                          !c.IsDeleted,
                      cancellationToken);

        return !exists;
    }

    protected override async Task ValidateEntityAsync(Contractor entity, bool isNew)
    {
        // Validate unique code
        if (isNew || entity.Code != null)
        {
            var isUnique = await IsCodeUniqueAsync(entity.Code, isNew ? null : entity.Id);
            if (!isUnique)
            {
                throw new InvalidOperationException($"Contractor code '{entity.Code}' already exists.");
            }
        }

        // Validate performance rating range
        if (entity.PerformanceRating.HasValue && (entity.PerformanceRating < 0 || entity.PerformanceRating > 5))
        {
            throw new InvalidOperationException("Performance rating must be between 0 and 5.");
        }

        // Validate credit limit
        if (entity.CreditLimit < 0)
        {
            throw new InvalidOperationException("Credit limit cannot be negative.");
        }

        await base.ValidateEntityAsync(entity, isNew);
    }
}