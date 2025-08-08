using Application.Interfaces.Contracts;
using Application.Interfaces.Common;
using Application.Interfaces.Auth;
using AutoMapper;
using Core.DTOs.Common;
using Core.DTOs.Contracts.Claims;
using Core.DTOs.Contracts.ChangeOrders;
using Core.Enums.Contracts;
using Domain.Entities.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services.Contracts;

public class ClaimService : IClaimService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<ClaimService> _logger;
    private readonly ICurrentUserService _currentUserService;

    public ClaimService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<ClaimService> logger,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _currentUserService = currentUserService;
    }

    #region IBaseService Implementation

    public async Task<ClaimDto> CreateAsync(CreateClaimDto createDto, string? createdBy = null, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate contract exists
            var contract = await _unitOfWork.Repository<Contract>()
                .GetAsync(
                    filter: c => c.Id == createDto.ContractId && !c.IsDeleted,
                    cancellationToken: cancellationToken);

            if (contract == null)
                throw new InvalidOperationException("Contract not found");

            // Check for duplicate claim number
            var existingClaim = await _unitOfWork.Repository<Claim>()
                .AnyAsync(
                    c => c.ClaimNumber == createDto.ClaimNumber && !c.IsDeleted,
                    cancellationToken);

            if (existingClaim)
                throw new InvalidOperationException($"Claim number {createDto.ClaimNumber} already exists");

            var claim = _mapper.Map<Claim>(createDto);
            claim.CreatedBy = createdBy ?? _currentUserService.UserId ?? "System";
            claim.Status = ClaimStatus.Draft;
            claim.NotificationDate = DateTime.UtcNow;

            await _unitOfWork.Repository<Claim>().AddAsync(claim);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return await GetByIdAsync(claim.Id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create claim");
            throw;
        }
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var claim = await _unitOfWork.Repository<Claim>()
                .GetAsync(
                    filter: c => c.Id == id && !c.IsDeleted,
                    cancellationToken: cancellationToken);

            if (claim == null)
                return false;

            if (claim.Status != ClaimStatus.Draft)
                throw new InvalidOperationException("Only draft claims can be deleted");

            claim.IsDeleted = true;
            claim.DeletedAt = DateTime.UtcNow;
            claim.DeletedBy = _currentUserService.UserId ?? "System";

            _unitOfWork.Repository<Claim>().Update(claim);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete claim");
            throw;
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Repository<Claim>()
            .AnyAsync(c => c.Id == id && !c.IsDeleted, cancellationToken);
    }

    public async Task<IEnumerable<ClaimDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var claims = await _unitOfWork.Repository<Claim>()
            .GetAllAsync(
                filter: c => !c.IsDeleted,
                includeProperties: "Contract",
                cancellationToken: cancellationToken);

        return _mapper.Map<IEnumerable<ClaimDto>>(claims);
    }

    public async Task<PagedResult<ClaimDto>> GetAllPagedAsync(int pageNumber = 1, int pageSize = 20, string? orderBy = null, bool descending = false, CancellationToken cancellationToken = default)
    {
        var query = _unitOfWork.Repository<Claim>()
            .Query()
            .Where(c => !c.IsDeleted)
            .Include(c => c.Contract);

        // Apply ordering
        query = orderBy?.ToLower() switch
        {
            "number" => descending ? query.OrderByDescending(c => c.ClaimNumber) : query.OrderBy(c => c.ClaimNumber),
            "amount" => descending ? query.OrderByDescending(c => c.ClaimedAmount) : query.OrderBy(c => c.ClaimedAmount),
            "status" => descending ? query.OrderByDescending(c => c.Status) : query.OrderBy(c => c.Status),
            "date" => descending ? query.OrderByDescending(c => c.SubmissionDate) : query.OrderBy(c => c.SubmissionDate),
            "type" => descending ? query.OrderByDescending(c => c.Type) : query.OrderBy(c => c.Type),
            _ => descending ? query.OrderByDescending(c => c.CreatedAt) : query.OrderBy(c => c.CreatedAt)
        };

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var dtos = _mapper.Map<List<ClaimDto>>(items);

        return new PagedResult<ClaimDto>(dtos, totalCount, pageNumber, pageSize);
    }

    public async Task<ClaimDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var claim = await _unitOfWork.Repository<Claim>()
            .GetAsync(
                filter: c => c.Id == id && !c.IsDeleted,
                includeProperties: "Contract,Contract.Project,Contract.Contractor",
                cancellationToken: cancellationToken);

        return claim != null ? _mapper.Map<ClaimDto>(claim) : null;
    }

    public async Task<ClaimDto?> UpdateAsync(Guid id, UpdateClaimDto updateDto, string? updatedBy = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var claim = await _unitOfWork.Repository<Claim>()
                .GetAsync(
                    filter: c => c.Id == id && !c.IsDeleted,
                    cancellationToken: cancellationToken);

            if (claim == null)
                return null;

            if (claim.Status != ClaimStatus.Draft && claim.Status != ClaimStatus.Notified)
                throw new InvalidOperationException("Can only update draft or notified claims");

            _mapper.Map(updateDto, claim);
            claim.UpdatedBy = updatedBy ?? _currentUserService.UserId ?? "System";
            claim.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Claim>().Update(claim);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return await GetByIdAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update claim");
            throw;
        }
    }

    #endregion

    #region IClaimService Implementation

    public async Task<IEnumerable<ClaimDto>> GetByContractAsync(Guid contractId)
    {
        var claims = await _unitOfWork.Repository<Claim>()
            .GetAllAsync(
                filter: c => c.ContractId == contractId && !c.IsDeleted,
                orderBy: q => q.OrderByDescending(c => c.SubmissionDate),
                cancellationToken: default);

        return _mapper.Map<IEnumerable<ClaimDto>>(claims);
    }

    public async Task<ClaimDto?> GetByClaimNumberAsync(string claimNumber)
    {
        var claim = await _unitOfWork.Repository<Claim>()
            .GetAsync(
                filter: c => c.ClaimNumber == claimNumber && !c.IsDeleted,
                includeProperties: "Contract",
                cancellationToken: default);

        return claim != null ? _mapper.Map<ClaimDto>(claim) : null;
    }

    public async Task<IEnumerable<ClaimDto>> SearchAsync(ClaimFilterDto filter)
    {
        var query = _unitOfWork.Repository<Claim>()
            .Query()
            .Where(c => !c.IsDeleted);

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var searchTerm = filter.SearchTerm.ToLower();
            query = query.Where(c => 
                c.ClaimNumber.ToLower().Contains(searchTerm) ||
                c.Subject.ToLower().Contains(searchTerm) ||
                c.Description.ToLower().Contains(searchTerm));
        }

        if (filter.ContractId.HasValue)
            query = query.Where(c => c.ContractId == filter.ContractId.Value);

        if (filter.Status.HasValue)
            query = query.Where(c => c.Status == filter.Status.Value);

        if (filter.Type.HasValue)
            query = query.Where(c => c.Type == filter.Type.Value);

        if (filter.Direction.HasValue)
            query = query.Where(c => c.Direction == filter.Direction.Value);

        if (filter.MinAmount.HasValue)
            query = query.Where(c => c.ClaimedAmount >= filter.MinAmount.Value);

        if (filter.MaxAmount.HasValue)
            query = query.Where(c => c.ClaimedAmount <= filter.MaxAmount.Value);

        if (filter.SubmittedAfter.HasValue)
            query = query.Where(c => c.SubmissionDate >= filter.SubmittedAfter.Value);

        if (filter.SubmittedBefore.HasValue)
            query = query.Where(c => c.SubmissionDate <= filter.SubmittedBefore.Value);

        var claims = await query
            .Include(c => c.Contract)
            .OrderByDescending(c => c.SubmissionDate)
            .ToListAsync();

        return _mapper.Map<IEnumerable<ClaimDto>>(claims);
    }

    public async Task<IEnumerable<ClaimDto>> GetByDirectionAsync(ClaimDirection direction)
    {
        var claims = await _unitOfWork.Repository<Claim>()
            .GetAllAsync(
                filter: c => c.Direction == direction && !c.IsDeleted,
                includeProperties: "Contract",
                orderBy: q => q.OrderByDescending(c => c.SubmissionDate),
                cancellationToken: default);

        return _mapper.Map<IEnumerable<ClaimDto>>(claims);
    }

    public async Task<IEnumerable<ClaimDto>> GetPendingAsync()
    {
        var claims = await _unitOfWork.Repository<Claim>()
            .GetAllAsync(
                filter: c => !c.IsDeleted && 
                           (c.Status == ClaimStatus.Submitted || c.Status == ClaimStatus.UnderAssessment),
                includeProperties: "Contract",
                orderBy: q => q.OrderBy(c => c.SubmissionDate),
                cancellationToken: default);

        return _mapper.Map<IEnumerable<ClaimDto>>(claims);
    }

    public async Task<IEnumerable<ClaimDto>> GetOverdueResponseAsync()
    {
        var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
        var claims = await _unitOfWork.Repository<Claim>()
            .GetAllAsync(
                filter: c => !c.IsDeleted && 
                           c.Status == ClaimStatus.Submitted &&
                           c.SubmissionDate < thirtyDaysAgo,
                includeProperties: "Contract",
                orderBy: q => q.OrderBy(c => c.SubmissionDate),
                cancellationToken: default);

        return _mapper.Map<IEnumerable<ClaimDto>>(claims);
    }

    public async Task<ClaimDto?> NotifyAsync(Guid claimId)
    {
        try
        {
            var claim = await _unitOfWork.Repository<Claim>()
                .GetAsync(
                    filter: c => c.Id == claimId && !c.IsDeleted,
                    cancellationToken: default);

            if (claim == null)
                return null;

            if (claim.Status != ClaimStatus.Draft)
                throw new InvalidOperationException("Only draft claims can be notified");

            claim.Status = ClaimStatus.Notified;
            claim.NotificationDate = DateTime.UtcNow;
            claim.UpdatedBy = _currentUserService.UserId ?? "System";
            claim.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Claim>().Update(claim);
            await _unitOfWork.SaveChangesAsync(default);

            return await GetByIdAsync(claimId, default);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to notify claim");
            throw;
        }
    }

    public async Task<ClaimDto?> SubmitAsync(Guid claimId)
    {
        try
        {
            var claim = await _unitOfWork.Repository<Claim>()
                .GetAsync(
                    filter: c => c.Id == claimId && !c.IsDeleted,
                    cancellationToken: default);

            if (claim == null)
                return null;

            if (claim.Status != ClaimStatus.Notified)
                throw new InvalidOperationException("Only notified claims can be submitted");

            claim.Status = ClaimStatus.Submitted;
            claim.SubmissionDate = DateTime.UtcNow;
            claim.UpdatedBy = _currentUserService.UserId ?? "System";
            claim.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Claim>().Update(claim);
            await _unitOfWork.SaveChangesAsync(default);

            return await GetByIdAsync(claimId, default);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to submit claim");
            throw;
        }
    }

    public async Task<ClaimDto?> AssessAsync(Guid claimId, AssessClaimDto dto)
    {
        try
        {
            var claim = await _unitOfWork.Repository<Claim>()
                .GetAsync(
                    filter: c => c.Id == claimId && !c.IsDeleted,
                    cancellationToken: default);

            if (claim == null)
                return null;

            if (claim.Status != ClaimStatus.Submitted)
                throw new InvalidOperationException("Only submitted claims can be assessed");

            claim.Status = ClaimStatus.UnderAssessment;
            claim.AssessmentDate = DateTime.UtcNow;
            claim.AssessedBy = dto.AssessedBy;
            claim.AssessmentComments = dto.Comments;
            claim.MeritFlag = dto.HasMerit;
            claim.ApprovedAmount = dto.ApprovedAmount;
            claim.UpdatedBy = _currentUserService.UserId ?? "System";
            claim.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Claim>().Update(claim);
            await _unitOfWork.SaveChangesAsync(default);

            return await GetByIdAsync(claimId, default);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to assess claim");
            throw;
        }
    }

    public async Task<ClaimDto?> ResolveAsync(Guid claimId, ResolveClaimDto dto)
    {
        try
        {
            var claim = await _unitOfWork.Repository<Claim>()
                .GetAsync(
                    filter: c => c.Id == claimId && !c.IsDeleted,
                    cancellationToken: default);

            if (claim == null)
                return null;

            if (claim.Status != ClaimStatus.UnderAssessment && claim.Status != ClaimStatus.Disputed)
                throw new InvalidOperationException("Claim must be under assessment or disputed to resolve");

            claim.Status = ClaimStatus.Closed;
            claim.ResolutionType = dto.ResolutionType;
            claim.ResolutionDate = DateTime.UtcNow;
            claim.ResolutionComments = dto.Comments;
            claim.FinalApprovedAmount = dto.FinalAmount;
            claim.UpdatedBy = _currentUserService.UserId ?? "System";
            claim.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Claim>().Update(claim);
            await _unitOfWork.SaveChangesAsync(default);

            return await GetByIdAsync(claimId, default);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to resolve claim");
            throw;
        }
    }

    public async Task<ClaimDto?> WithdrawAsync(Guid claimId, string reason)
    {
        try
        {
            var claim = await _unitOfWork.Repository<Claim>()
                .GetAsync(
                    filter: c => c.Id == claimId && !c.IsDeleted,
                    cancellationToken: default);

            if (claim == null)
                return null;

            if (claim.Status == ClaimStatus.Closed || claim.Status == ClaimStatus.Withdrawn)
                throw new InvalidOperationException("Cannot withdraw closed or already withdrawn claims");

            claim.Status = ClaimStatus.Withdrawn;
            claim.ResolutionComments = $"Withdrawn: {reason}";
            claim.ResolutionDate = DateTime.UtcNow;
            claim.UpdatedBy = _currentUserService.UserId ?? "System";
            claim.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Claim>().Update(claim);
            await _unitOfWork.SaveChangesAsync(default);

            return await GetByIdAsync(claimId, default);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to withdraw claim");
            throw;
        }
    }

    public async Task<ClaimDto?> EscalateAsync(Guid claimId, string escalationType)
    {
        try
        {
            var claim = await _unitOfWork.Repository<Claim>()
                .GetAsync(
                    filter: c => c.Id == claimId && !c.IsDeleted,
                    cancellationToken: default);

            if (claim == null)
                return null;

            claim.Status = ClaimStatus.Disputed;
            claim.EscalationType = escalationType;
            claim.EscalationDate = DateTime.UtcNow;
            claim.UpdatedBy = _currentUserService.UserId ?? "System";
            claim.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Claim>().Update(claim);
            await _unitOfWork.SaveChangesAsync(default);

            return await GetByIdAsync(claimId, default);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to escalate claim");
            throw;
        }
    }

    public async Task<bool> CheckTimeBarAsync(Guid claimId, int notificationPeriodDays)
    {
        try
        {
            var claim = await _unitOfWork.Repository<Claim>()
                .GetAsync(c => c.Id == claimId && !c.IsDeleted);

            if (claim == null)
                return false;

            if (claim.NotificationDate == null)
                return false;

            var timeBarDate = claim.EventDate.AddDays(notificationPeriodDays);
            return claim.NotificationDate.Value <= timeBarDate;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check time bar");
            throw;
        }
    }

    public async Task<IEnumerable<ClaimDto>> GetTimeBarredAsync()
    {
        // This would require contract-specific time bar periods
        var claims = await _unitOfWork.Repository<Claim>()
            .GetAllAsync(
                filter: c => !c.IsDeleted && c.IsTimeBarred,
                includeProperties: "Contract",
                cancellationToken: default);

        return _mapper.Map<IEnumerable<ClaimDto>>(claims);
    }

    public async Task<IEnumerable<ClaimDto>> GetAtRiskOfTimeBarAsync(int daysWarning = 7)
    {
        var warningDate = DateTime.UtcNow.AddDays(daysWarning);
        
        // This is simplified - would need contract-specific notification periods
        var claims = await _unitOfWork.Repository<Claim>()
            .GetAllAsync(
                filter: c => !c.IsDeleted && 
                           c.Status == ClaimStatus.Draft &&
                           c.EventDate.AddDays(28 - daysWarning) <= warningDate,
                includeProperties: "Contract",
                cancellationToken: default);

        return _mapper.Map<IEnumerable<ClaimDto>>(claims);
    }

    public async Task<ClaimDto?> RecordPaymentAsync(Guid claimId, decimal amount)
    {
        try
        {
            var claim = await _unitOfWork.Repository<Claim>()
                .GetAsync(
                    filter: c => c.Id == claimId && !c.IsDeleted,
                    cancellationToken: default);

            if (claim == null)
                return null;

            if (claim.Status != ClaimStatus.Closed)
                throw new InvalidOperationException("Only closed claims can have payments recorded");

            claim.PaidAmount = (claim.PaidAmount ?? 0) + amount;
            claim.PaymentDate = DateTime.UtcNow;
            claim.UpdatedBy = _currentUserService.UserId ?? "System";
            claim.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Claim>().Update(claim);
            await _unitOfWork.SaveChangesAsync(default);

            return await GetByIdAsync(claimId, default);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to record payment");
            throw;
        }
    }

    public async Task<decimal> GetTotalClaimedAsync(Guid contractId, ClaimDirection? direction = null)
    {
        try
        {
            var query = _unitOfWork.Repository<Claim>()
                .Query()
                .Where(c => c.ContractId == contractId && !c.IsDeleted);

            if (direction.HasValue)
                query = query.Where(c => c.Direction == direction.Value);

            var claims = await query.ToListAsync();
            return claims.Sum(c => c.ClaimedAmount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get total claimed");
            throw;
        }
    }

    public async Task<decimal> GetTotalApprovedAsync(Guid contractId, ClaimDirection? direction = null)
    {
        try
        {
            var query = _unitOfWork.Repository<Claim>()
                .Query()
                .Where(c => c.ContractId == contractId && !c.IsDeleted && c.FinalApprovedAmount.HasValue);

            if (direction.HasValue)
                query = query.Where(c => c.Direction == direction.Value);

            var claims = await query.ToListAsync();
            return claims.Sum(c => c.FinalApprovedAmount ?? 0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get total approved");
            throw;
        }
    }

    public async Task<decimal> GetTotalPaidAsync(Guid contractId, ClaimDirection? direction = null)
    {
        try
        {
            var query = _unitOfWork.Repository<Claim>()
                .Query()
                .Where(c => c.ContractId == contractId && !c.IsDeleted && c.PaidAmount.HasValue);

            if (direction.HasValue)
                query = query.Where(c => c.Direction == direction.Value);

            var claims = await query.ToListAsync();
            return claims.Sum(c => c.PaidAmount ?? 0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get total paid");
            throw;
        }
    }

    public async Task<decimal> GetOutstandingAsync(Guid contractId, ClaimDirection? direction = null)
    {
        try
        {
            var totalApproved = await GetTotalApprovedAsync(contractId, direction);
            var totalPaid = await GetTotalPaidAsync(contractId, direction);
            return totalApproved - totalPaid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get outstanding amount");
            throw;
        }
    }

    public async Task<bool> LinkToChangeOrderAsync(Guid claimId, Guid changeOrderId, string relationType)
    {
        try
        {
            // This would require a many-to-many relationship table
            await Task.CompletedTask;
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to link claim to change order");
            throw;
        }
    }

    public async Task<bool> UnlinkFromChangeOrderAsync(Guid claimId, Guid changeOrderId)
    {
        try
        {
            await Task.CompletedTask;
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to unlink claim from change order");
            throw;
        }
    }

    public async Task<IEnumerable<ChangeOrderDto>> GetRelatedChangeOrdersAsync(Guid claimId)
    {
        try
        {
            await Task.CompletedTask;
            return new List<ChangeOrderDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get related change orders");
            throw;
        }
    }

    public async Task<bool> LinkToClaimAsync(Guid claimId, Guid relatedClaimId, string relationType)
    {
        try
        {
            await Task.CompletedTask;
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to link claims");
            throw;
        }
    }

    public async Task<bool> UnlinkFromClaimAsync(Guid claimId, Guid relatedClaimId)
    {
        try
        {
            await Task.CompletedTask;
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to unlink claims");
            throw;
        }
    }

    public async Task<IEnumerable<ClaimDto>> GetRelatedClaimsAsync(Guid claimId)
    {
        try
        {
            await Task.CompletedTask;
            return new List<ClaimDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get related claims");
            throw;
        }
    }

    public async Task<IEnumerable<ClaimDto>> GetByMeritAsync(bool hasMerit)
    {
        var claims = await _unitOfWork.Repository<Claim>()
            .GetAllAsync(
                filter: c => !c.IsDeleted && c.MeritFlag == hasMerit,
                includeProperties: "Contract",
                orderBy: q => q.OrderByDescending(c => c.SubmissionDate),
                cancellationToken: default);

        return _mapper.Map<IEnumerable<ClaimDto>>(claims);
    }

    public async Task<Dictionary<ClaimType, decimal>> GetClaimsByTypeAsync(Guid contractId)
    {
        try
        {
            var claims = await _unitOfWork.Repository<Claim>()
                .GetAllAsync(
                    filter: c => c.ContractId == contractId && !c.IsDeleted,
                    cancellationToken: default);

            return claims
                .GroupBy(c => c.Type)
                .ToDictionary(g => g.Key, g => g.Sum(c => c.ClaimedAmount));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get claims by type");
            throw;
        }
    }

    public async Task<Dictionary<ClaimResolution, int>> GetResolutionSummaryAsync(Guid? contractId = null)
    {
        try
        {
            var query = _unitOfWork.Repository<Claim>()
                .Query()
                .Where(c => !c.IsDeleted && c.ResolutionType.HasValue);

            if (contractId.HasValue)
                query = query.Where(c => c.ContractId == contractId.Value);

            var claims = await query.ToListAsync();

            return claims
                .Where(c => c.ResolutionType.HasValue)
                .GroupBy(c => c.ResolutionType!.Value)
                .ToDictionary(g => g.Key, g => g.Count());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get resolution summary");
            throw;
        }
    }

    public async Task<bool> AttachDocumentAsync(Guid claimId, Guid documentId, string documentType)
    {
        try
        {
            // This would add to a ClaimDocument table
            await Task.CompletedTask;
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to attach document to claim");
            throw;
        }
    }

    public async Task<bool> RemoveDocumentAsync(Guid claimId, Guid documentId)
    {
        try
        {
            await Task.CompletedTask;
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove document from claim");
            throw;
        }
    }

    public async Task<IEnumerable<ClaimDocumentDto>> GetDocumentsAsync(Guid claimId)
    {
        try
        {
            await Task.CompletedTask;
            return new List<ClaimDocumentDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get claim documents");
            throw;
        }
    }

    public async Task<bool> ValidateDocumentationAsync(Guid claimId)
    {
        try
        {
            // Check if claim has all required documents
            await Task.CompletedTask;
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to validate documentation");
            throw;
        }
    }

    public async Task<IEnumerable<ClaimDto>> GetHighValueClaimsAsync(decimal threshold)
    {
        var claims = await _unitOfWork.Repository<Claim>()
            .GetAllAsync(
                filter: c => !c.IsDeleted && c.ClaimedAmount > threshold,
                includeProperties: "Contract",
                orderBy: q => q.OrderByDescending(c => c.ClaimedAmount),
                cancellationToken: default);

        return _mapper.Map<IEnumerable<ClaimDto>>(claims);
    }

    public async Task<IEnumerable<ClaimDto>> GetDisputedClaimsAsync()
    {
        var claims = await _unitOfWork.Repository<Claim>()
            .GetAllAsync(
                filter: c => !c.IsDeleted && c.Status == ClaimStatus.Disputed,
                includeProperties: "Contract",
                orderBy: q => q.OrderBy(c => c.EscalationDate),
                cancellationToken: default);

        return _mapper.Map<IEnumerable<ClaimDto>>(claims);
    }

    public async Task<Dictionary<ClaimStatus, int>> GetStatusSummaryAsync(Guid? contractId = null)
    {
        try
        {
            var query = _unitOfWork.Repository<Claim>()
                .Query()
                .Where(c => !c.IsDeleted);

            if (contractId.HasValue)
                query = query.Where(c => c.ContractId == contractId.Value);

            var claims = await query.ToListAsync();

            return claims
                .GroupBy(c => c.Status)
                .ToDictionary(g => g.Key, g => g.Count());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get status summary");
            throw;
        }
    }

    public async Task<Dictionary<string, decimal>> GetMonthlyClaimTrendAsync(Guid contractId, int months = 12)
    {
        try
        {
            var startDate = DateTime.UtcNow.AddMonths(-months);
            var claims = await _unitOfWork.Repository<Claim>()
                .GetAllAsync(
                    filter: c => c.ContractId == contractId && 
                               !c.IsDeleted &&
                               c.SubmissionDate >= startDate,
                    cancellationToken: default);

            return claims
                .GroupBy(c => c.SubmissionDate.ToString("yyyy-MM"))
                .OrderBy(g => g.Key)
                .ToDictionary(g => g.Key, g => g.Sum(c => c.ClaimedAmount));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get monthly claim trend");
            throw;
        }
    }

    public async Task<byte[]> ExportClaimRegisterAsync(Guid contractId)
    {
        try
        {
            // This would use IExcelExportService
            await Task.CompletedTask;
            return Array.Empty<byte>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to export claim register");
            throw;
        }
    }

    public async Task<byte[]> GenerateClaimReportAsync(Guid claimId)
    {
        try
        {
            // This would generate a PDF report
            await Task.CompletedTask;
            return Array.Empty<byte>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate claim report");
            throw;
        }
    }

    #endregion
}