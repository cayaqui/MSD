using Application.Interfaces.Contracts;
using Application.Interfaces.Common;
using Application.Interfaces.Auth;
using AutoMapper;
using Core.DTOs.Common;
using Core.DTOs.Contracts.ContractMilestones;
using Core.Enums.Contracts;
using Core.Enums.Projects;
using Domain.Entities.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services.Contracts;

public class ContractMilestoneService : IContractMilestoneService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<ContractMilestoneService> _logger;
    private readonly ICurrentUserService _currentUserService;

    public ContractMilestoneService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<ContractMilestoneService> logger,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _currentUserService = currentUserService;
    }

    #region IBaseService Implementation

    public async Task<ContractMilestoneDto> CreateAsync(CreateContractMilestoneDto createDto, string? createdBy = null, CancellationToken cancellationToken = default)
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

            // Check for duplicate milestone code
            var existingMilestone = await _unitOfWork.Repository<ContractMilestone>()
                .AnyAsync(
                    m => m.ContractId == createDto.ContractId && 
                         m.MilestoneCode == createDto.MilestoneCode && 
                         !m.IsDeleted,
                    cancellationToken);

            if (existingMilestone)
                throw new InvalidOperationException($"Milestone code {createDto.MilestoneCode} already exists in this contract");

            var milestone = _mapper.Map<ContractMilestone>(createDto);
            milestone.CreatedBy = createdBy ?? _currentUserService.UserId ?? "System";
            milestone.Status = MilestoneStatus.Planned;

            await _unitOfWork.Repository<ContractMilestone>().AddAsync(milestone);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return await GetByIdAsync(milestone.Id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create contract milestone");
            throw;
        }
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var milestone = await _unitOfWork.Repository<ContractMilestone>()
                .GetAsync(
                    filter: m => m.Id == id && !m.IsDeleted,
                    cancellationToken: cancellationToken);

            if (milestone == null)
                return false;

            if (milestone.Status != MilestoneStatus.Planned)
                throw new InvalidOperationException("Only planned milestones can be deleted");

            milestone.IsDeleted = true;
            milestone.DeletedAt = DateTime.UtcNow;
            milestone.DeletedBy = _currentUserService.UserId ?? "System";

            _unitOfWork.Repository<ContractMilestone>().Update(milestone);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete contract milestone");
            throw;
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Repository<ContractMilestone>()
            .AnyAsync(m => m.Id == id && !m.IsDeleted, cancellationToken);
    }

    public async Task<IEnumerable<ContractMilestoneDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var milestones = await _unitOfWork.Repository<ContractMilestone>()
            .GetAllAsync(
                filter: m => !m.IsDeleted,
                includeProperties: "Contract",
                cancellationToken: cancellationToken);

        return _mapper.Map<IEnumerable<ContractMilestoneDto>>(milestones);
    }

    public async Task<PagedResult<ContractMilestoneDto>> GetAllPagedAsync(int pageNumber = 1, int pageSize = 20, string? orderBy = null, bool descending = false, CancellationToken cancellationToken = default)
    {
        var query = _unitOfWork.Repository<ContractMilestone>()
            .Query()
            .Where(m => !m.IsDeleted)
            .Include(m => m.Contract);

        // Apply ordering
        query = orderBy?.ToLower() switch
        {
            "code" => descending ? query.OrderByDescending(m => m.MilestoneCode) : query.OrderBy(m => m.MilestoneCode),
            "name" => descending ? query.OrderByDescending(m => m.MilestoneName) : query.OrderBy(m => m.MilestoneName),
            "date" => descending ? query.OrderByDescending(m => m.PlannedDate) : query.OrderBy(m => m.PlannedDate),
            "value" => descending ? query.OrderByDescending(m => m.PaymentAmount) : query.OrderBy(m => m.PaymentAmount),
            "status" => descending ? query.OrderByDescending(m => m.Status) : query.OrderBy(m => m.Status),
            _ => descending ? query.OrderByDescending(m => m.PlannedDate) : query.OrderBy(m => m.PlannedDate)
        };

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var dtos = _mapper.Map<List<ContractMilestoneDto>>(items);

        return new PagedResult<ContractMilestoneDto>(dtos, totalCount, pageNumber, pageSize);
    }

    public async Task<ContractMilestoneDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var milestone = await _unitOfWork.Repository<ContractMilestone>()
            .GetAsync(
                filter: m => m.Id == id && !m.IsDeleted,
                includeProperties: "Contract,Contract.Project,Contract.Contractor",
                cancellationToken: cancellationToken);

        return milestone != null ? _mapper.Map<ContractMilestoneDto>(milestone) : null;
    }

    public async Task<ContractMilestoneDto?> UpdateAsync(Guid id, UpdateContractMilestoneDto updateDto, string? updatedBy = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var milestone = await _unitOfWork.Repository<ContractMilestone>()
                .GetAsync(
                    filter: m => m.Id == id && !m.IsDeleted,
                    cancellationToken: cancellationToken);

            if (milestone == null)
                return null;

            _mapper.Map(updateDto, milestone);
            milestone.UpdatedBy = updatedBy ?? _currentUserService.UserId ?? "System";
            milestone.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<ContractMilestone>().Update(milestone);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return await GetByIdAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update contract milestone");
            throw;
        }
    }

    #endregion

    #region IContractMilestoneService Implementation

    public async Task<IEnumerable<ContractMilestoneDto>> GetByContractAsync(Guid contractId)
    {
        var milestones = await _unitOfWork.Repository<ContractMilestone>()
            .GetAllAsync(
                filter: m => m.ContractId == contractId && !m.IsDeleted,
                orderBy: q => q.OrderBy(m => m.PlannedDate),
                cancellationToken: default);

        return _mapper.Map<IEnumerable<ContractMilestoneDto>>(milestones);
    }

    public async Task<ContractMilestoneDto?> GetByMilestoneCodeAsync(Guid contractId, string milestoneCode)
    {
        var milestone = await _unitOfWork.Repository<ContractMilestone>()
            .GetAsync(
                filter: m => m.ContractId == contractId && 
                           m.MilestoneCode == milestoneCode && 
                           !m.IsDeleted,
                cancellationToken: default);

        return milestone != null ? _mapper.Map<ContractMilestoneDto>(milestone) : null;
    }

    public async Task<IEnumerable<ContractMilestoneDto>> SearchAsync(ContractMilestoneFilterDto filter)
    {
        var query = _unitOfWork.Repository<ContractMilestone>()
            .Query()
            .Where(m => !m.IsDeleted);

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var searchTerm = filter.SearchTerm.ToLower();
            query = query.Where(m => 
                m.MilestoneCode.ToLower().Contains(searchTerm) ||
                m.MilestoneName.ToLower().Contains(searchTerm) ||
                m.Description.ToLower().Contains(searchTerm));
        }

        if (filter.ContractId.HasValue)
            query = query.Where(m => m.ContractId == filter.ContractId.Value);

        if (filter.Status.HasValue)
            query = query.Where(m => m.Status == filter.Status.Value);

        if (filter.Type.HasValue)
            query = query.Where(m => m.Type == filter.Type.Value);

        if (filter.IsPaymentMilestone.HasValue)
            query = query.Where(m => m.IsPaymentMilestone == filter.IsPaymentMilestone.Value);

        if (filter.IsCritical.HasValue)
            query = query.Where(m => m.IsCritical == filter.IsCritical.Value);

        if (filter.PlannedDateFrom.HasValue)
            query = query.Where(m => m.PlannedDate >= filter.PlannedDateFrom.Value);

        if (filter.PlannedDateTo.HasValue)
            query = query.Where(m => m.PlannedDate <= filter.PlannedDateTo.Value);

        var milestones = await query
            .Include(m => m.Contract)
            .OrderBy(m => m.PlannedDate)
            .ToListAsync();

        return _mapper.Map<IEnumerable<ContractMilestoneDto>>(milestones);
    }

    public async Task<IEnumerable<ContractMilestoneDto>> GetUpcomingAsync(int days = 30)
    {
        var futureDate = DateTime.UtcNow.AddDays(days);
        var milestones = await _unitOfWork.Repository<ContractMilestone>()
            .GetAllAsync(
                filter: m => !m.IsDeleted && 
                           m.Status != MilestoneStatus.Completed &&
                           m.PlannedDate >= DateTime.UtcNow &&
                           m.PlannedDate <= futureDate,
                includeProperties: "Contract",
                orderBy: q => q.OrderBy(m => m.PlannedDate),
                cancellationToken: default);

        return _mapper.Map<IEnumerable<ContractMilestoneDto>>(milestones);
    }

    public async Task<IEnumerable<ContractMilestoneDto>> GetOverdueAsync()
    {
        var today = DateTime.UtcNow.Date;
        var milestones = await _unitOfWork.Repository<ContractMilestone>()
            .GetAllAsync(
                filter: m => !m.IsDeleted && 
                           m.Status != MilestoneStatus.Completed &&
                           m.PlannedDate < today,
                includeProperties: "Contract",
                orderBy: q => q.OrderBy(m => m.PlannedDate),
                cancellationToken: default);

        return _mapper.Map<IEnumerable<ContractMilestoneDto>>(milestones);
    }

    public async Task<IEnumerable<ContractMilestoneDto>> GetCriticalPathAsync(Guid contractId)
    {
        var milestones = await _unitOfWork.Repository<ContractMilestone>()
            .GetAllAsync(
                filter: m => m.ContractId == contractId && 
                           m.IsCritical && 
                           !m.IsDeleted,
                orderBy: q => q.OrderBy(m => m.PlannedDate),
                cancellationToken: default);

        return _mapper.Map<IEnumerable<ContractMilestoneDto>>(milestones);
    }

    public async Task<ContractMilestoneDto?> UpdateProgressAsync(Guid milestoneId, UpdateMilestoneProgressDto dto)
    {
        try
        {
            var milestone = await _unitOfWork.Repository<ContractMilestone>()
                .GetAsync(
                    filter: m => m.Id == milestoneId && !m.IsDeleted,
                    cancellationToken: default);

            if (milestone == null)
                return null;

            milestone.ProgressPercentage = dto.ProgressPercentage;
            milestone.ProgressComments = dto.Comments;
            
            if (dto.ProgressPercentage > 0 && milestone.Status == MilestoneStatus.Planned)
                milestone.Status = MilestoneStatus.InProgress;

            milestone.UpdatedBy = _currentUserService.UserId ?? "System";
            milestone.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<ContractMilestone>().Update(milestone);
            await _unitOfWork.SaveChangesAsync(default);

            return await GetByIdAsync(milestoneId, default);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update milestone progress");
            throw;
        }
    }

    public async Task<ContractMilestoneDto?> CompleteAsync(Guid milestoneId, DateTime actualDate)
    {
        try
        {
            var milestone = await _unitOfWork.Repository<ContractMilestone>()
                .GetAsync(
                    filter: m => m.Id == milestoneId && !m.IsDeleted,
                    cancellationToken: default);

            if (milestone == null)
                return null;

            milestone.Status = MilestoneStatus.Completed;
            milestone.ActualDate = actualDate;
            milestone.ProgressPercentage = 100;
            milestone.UpdatedBy = _currentUserService.UserId ?? "System";
            milestone.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<ContractMilestone>().Update(milestone);
            await _unitOfWork.SaveChangesAsync(default);

            return await GetByIdAsync(milestoneId, default);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to complete milestone");
            throw;
        }
    }

    public async Task<IEnumerable<ContractMilestoneDto>> GetInProgressAsync(Guid? contractId = null)
    {
        var query = _unitOfWork.Repository<ContractMilestone>()
            .Query()
            .Where(m => !m.IsDeleted && m.Status == MilestoneStatus.InProgress);

        if (contractId.HasValue)
            query = query.Where(m => m.ContractId == contractId.Value);

        var milestones = await query
            .Include(m => m.Contract)
            .OrderBy(m => m.PlannedDate)
            .ToListAsync();

        return _mapper.Map<IEnumerable<ContractMilestoneDto>>(milestones);
    }

    public async Task<ContractMilestoneDto?> SubmitForApprovalAsync(Guid milestoneId)
    {
        try
        {
            var milestone = await _unitOfWork.Repository<ContractMilestone>()
                .GetAsync(
                    filter: m => m.Id == milestoneId && !m.IsDeleted,
                    cancellationToken: default);

            if (milestone == null)
                return null;

            if (milestone.Status != MilestoneStatus.Completed)
                throw new InvalidOperationException("Only completed milestones can be submitted for approval");

            milestone.Status = MilestoneStatus.UnderApproval;
            milestone.UpdatedBy = _currentUserService.UserId ?? "System";
            milestone.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<ContractMilestone>().Update(milestone);
            await _unitOfWork.SaveChangesAsync(default);

            return await GetByIdAsync(milestoneId, default);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to submit milestone for approval");
            throw;
        }
    }

    public async Task<ContractMilestoneDto?> ApproveAsync(Guid milestoneId, ApproveMilestoneDto dto, string approvedBy)
    {
        try
        {
            var milestone = await _unitOfWork.Repository<ContractMilestone>()
                .GetAsync(
                    filter: m => m.Id == milestoneId && !m.IsDeleted,
                    cancellationToken: default);

            if (milestone == null)
                return null;

            if (milestone.Status != MilestoneStatus.UnderApproval)
                throw new InvalidOperationException("Only milestones under approval can be approved");

            milestone.Status = MilestoneStatus.Approved;
            milestone.ApprovedBy = approvedBy;
            milestone.ApprovalDate = DateTime.UtcNow;
            milestone.ApprovalComments = dto.Comments;
            milestone.UpdatedBy = _currentUserService.UserId ?? "System";
            milestone.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<ContractMilestone>().Update(milestone);
            await _unitOfWork.SaveChangesAsync(default);

            return await GetByIdAsync(milestoneId, default);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to approve milestone");
            throw;
        }
    }

    public async Task<ContractMilestoneDto?> RejectAsync(Guid milestoneId, string reason, string rejectedBy)
    {
        try
        {
            var milestone = await _unitOfWork.Repository<ContractMilestone>()
                .GetAsync(
                    filter: m => m.Id == milestoneId && !m.IsDeleted,
                    cancellationToken: default);

            if (milestone == null)
                return null;

            if (milestone.Status != MilestoneStatus.UnderApproval)
                throw new InvalidOperationException("Only milestones under approval can be rejected");

            milestone.Status = MilestoneStatus.Rejected;
            milestone.ApprovedBy = rejectedBy; // Store rejector
            milestone.ApprovalDate = DateTime.UtcNow;
            milestone.ApprovalComments = $"Rejected: {reason}";
            milestone.UpdatedBy = _currentUserService.UserId ?? "System";
            milestone.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<ContractMilestone>().Update(milestone);
            await _unitOfWork.SaveChangesAsync(default);

            return await GetByIdAsync(milestoneId, default);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to reject milestone");
            throw;
        }
    }

    public async Task<IEnumerable<ContractMilestoneDto>> GetPaymentMilestonesAsync(Guid contractId)
    {
        var milestones = await _unitOfWork.Repository<ContractMilestone>()
            .GetAllAsync(
                filter: m => m.ContractId == contractId && 
                           m.IsPaymentMilestone && 
                           !m.IsDeleted,
                orderBy: q => q.OrderBy(m => m.PlannedDate),
                cancellationToken: default);

        return _mapper.Map<IEnumerable<ContractMilestoneDto>>(milestones);
    }

    public async Task<IEnumerable<ContractMilestoneDto>> GetUnpaidMilestonesAsync(Guid contractId)
    {
        var milestones = await _unitOfWork.Repository<ContractMilestone>()
            .GetAllAsync(
                filter: m => m.ContractId == contractId && 
                           m.IsPaymentMilestone && 
                           !m.IsPaid &&
                           m.Status == MilestoneStatus.Approved &&
                           !m.IsDeleted,
                orderBy: q => q.OrderBy(m => m.PlannedDate),
                cancellationToken: default);

        return _mapper.Map<IEnumerable<ContractMilestoneDto>>(milestones);
    }

    public async Task<ContractMilestoneDto?> RecordInvoiceAsync(Guid milestoneId, string invoiceNumber, decimal amount)
    {
        try
        {
            var milestone = await _unitOfWork.Repository<ContractMilestone>()
                .GetAsync(
                    filter: m => m.Id == milestoneId && !m.IsDeleted,
                    cancellationToken: default);

            if (milestone == null)
                return null;

            if (!milestone.IsPaymentMilestone)
                throw new InvalidOperationException("This is not a payment milestone");

            milestone.IsInvoiced = true;
            milestone.InvoiceNumber = invoiceNumber;
            milestone.InvoicedAmount = amount;
            milestone.InvoiceDate = DateTime.UtcNow;
            milestone.UpdatedBy = _currentUserService.UserId ?? "System";
            milestone.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<ContractMilestone>().Update(milestone);
            await _unitOfWork.SaveChangesAsync(default);

            return await GetByIdAsync(milestoneId, default);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to record invoice for milestone");
            throw;
        }
    }

    public async Task<ContractMilestoneDto?> RecordPaymentAsync(Guid milestoneId, decimal amount)
    {
        try
        {
            var milestone = await _unitOfWork.Repository<ContractMilestone>()
                .GetAsync(
                    filter: m => m.Id == milestoneId && !m.IsDeleted,
                    cancellationToken: default);

            if (milestone == null)
                return null;

            if (!milestone.IsPaymentMilestone)
                throw new InvalidOperationException("This is not a payment milestone");

            milestone.IsPaid = true;
            milestone.PaidAmount = amount;
            milestone.PaymentDate = DateTime.UtcNow;
            milestone.UpdatedBy = _currentUserService.UserId ?? "System";
            milestone.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<ContractMilestone>().Update(milestone);
            await _unitOfWork.SaveChangesAsync(default);

            return await GetByIdAsync(milestoneId, default);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to record payment for milestone");
            throw;
        }
    }

    public async Task<bool> AddDependencyAsync(Guid predecessorId, Guid successorId, string dependencyType, int lagDays)
    {
        try
        {
            // This would require a many-to-many relationship table
            // For now, returning true as placeholder
            await Task.CompletedTask;
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add milestone dependency");
            throw;
        }
    }

    public async Task<bool> RemoveDependencyAsync(Guid predecessorId, Guid successorId)
    {
        try
        {
            await Task.CompletedTask;
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove milestone dependency");
            throw;
        }
    }

    public async Task<IEnumerable<ContractMilestoneDto>> GetPredecessorsAsync(Guid milestoneId)
    {
        try
        {
            // This would query the many-to-many relationship
            await Task.CompletedTask;
            return new List<ContractMilestoneDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get milestone predecessors");
            throw;
        }
    }

    public async Task<IEnumerable<ContractMilestoneDto>> GetSuccessorsAsync(Guid milestoneId)
    {
        try
        {
            await Task.CompletedTask;
            return new List<ContractMilestoneDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get milestone successors");
            throw;
        }
    }

    public async Task<bool> ValidateDependenciesAsync(Guid milestoneId)
    {
        try
        {
            // Check if all predecessor milestones are completed
            await Task.CompletedTask;
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to validate milestone dependencies");
            throw;
        }
    }

    public async Task<ContractMilestoneDto?> UpdateForecastDateAsync(Guid milestoneId, DateTime forecastDate, string explanation)
    {
        try
        {
            var milestone = await _unitOfWork.Repository<ContractMilestone>()
                .GetAsync(
                    filter: m => m.Id == milestoneId && !m.IsDeleted,
                    cancellationToken: default);

            if (milestone == null)
                return null;

            milestone.ForecastDate = forecastDate;
            milestone.ProgressComments = $"Forecast updated: {explanation}";
            milestone.UpdatedBy = _currentUserService.UserId ?? "System";
            milestone.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<ContractMilestone>().Update(milestone);
            await _unitOfWork.SaveChangesAsync(default);

            return await GetByIdAsync(milestoneId, default);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update milestone forecast date");
            throw;
        }
    }

    public async Task<Dictionary<Guid, int>> CalculateScheduleVariancesAsync(Guid contractId)
    {
        try
        {
            var milestones = await _unitOfWork.Repository<ContractMilestone>()
                .GetAllAsync(
                    filter: m => m.ContractId == contractId && !m.IsDeleted,
                    cancellationToken: default);

            var variances = new Dictionary<Guid, int>();
            
            foreach (var milestone in milestones)
            {
                if (milestone.ActualDate.HasValue)
                {
                    variances[milestone.Id] = (milestone.ActualDate.Value - milestone.PlannedDate).Days;
                }
                else if (milestone.ForecastDate.HasValue)
                {
                    variances[milestone.Id] = (milestone.ForecastDate.Value - milestone.PlannedDate).Days;
                }
                else
                {
                    variances[milestone.Id] = 0;
                }
            }

            return variances;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to calculate schedule variances");
            throw;
        }
    }

    public async Task<DateTime?> CalculateProjectedCompletionAsync(Guid contractId)
    {
        try
        {
            var milestones = await _unitOfWork.Repository<ContractMilestone>()
                .GetAllAsync(
                    filter: m => m.ContractId == contractId && 
                               m.IsCritical && 
                               !m.IsDeleted,
                    orderBy: q => q.OrderByDescending(m => m.PlannedDate),
                    cancellationToken: default);

            var lastMilestone = milestones.FirstOrDefault();
            if (lastMilestone == null)
                return null;

            if (lastMilestone.ForecastDate.HasValue)
                return lastMilestone.ForecastDate.Value;

            return lastMilestone.PlannedDate;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to calculate projected completion");
            throw;
        }
    }

    public async Task<bool> AttachDocumentAsync(Guid milestoneId, Guid documentId, string documentType)
    {
        try
        {
            // This would add to a MilestoneDocument table
            await Task.CompletedTask;
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to attach document to milestone");
            throw;
        }
    }

    public async Task<bool> RemoveDocumentAsync(Guid milestoneId, Guid documentId)
    {
        try
        {
            await Task.CompletedTask;
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove document from milestone");
            throw;
        }
    }

    public async Task<IEnumerable<MilestoneDocumentDto>> GetDocumentsAsync(Guid milestoneId)
    {
        try
        {
            await Task.CompletedTask;
            return new List<MilestoneDocumentDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get milestone documents");
            throw;
        }
    }

    public async Task<decimal> GetTotalMilestoneValueAsync(Guid contractId)
    {
        try
        {
            var milestones = await _unitOfWork.Repository<ContractMilestone>()
                .GetAllAsync(
                    filter: m => m.ContractId == contractId && 
                               m.IsPaymentMilestone && 
                               !m.IsDeleted,
                    cancellationToken: default);

            return milestones.Sum(m => m.PaymentAmount ?? 0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get total milestone value");
            throw;
        }
    }

    public async Task<decimal> GetCompletedMilestoneValueAsync(Guid contractId)
    {
        try
        {
            var milestones = await _unitOfWork.Repository<ContractMilestone>()
                .GetAllAsync(
                    filter: m => m.ContractId == contractId && 
                               m.IsPaymentMilestone && 
                               m.Status == MilestoneStatus.Completed &&
                               !m.IsDeleted,
                    cancellationToken: default);

            return milestones.Sum(m => m.PaymentAmount ?? 0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get completed milestone value");
            throw;
        }
    }

    public async Task<Dictionary<MilestoneStatus, int>> GetStatusSummaryAsync(Guid contractId)
    {
        try
        {
            var milestones = await _unitOfWork.Repository<ContractMilestone>()
                .GetAllAsync(
                    filter: m => m.ContractId == contractId && !m.IsDeleted,
                    cancellationToken: default);

            return milestones
                .GroupBy(m => m.Status)
                .ToDictionary(g => g.Key, g => g.Count());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get milestone status summary");
            throw;
        }
    }

    public async Task<IEnumerable<ContractMilestoneDto>> GetDelayedMilestonesAsync(Guid contractId)
    {
        try
        {
            var today = DateTime.UtcNow.Date;
            var milestones = await _unitOfWork.Repository<ContractMilestone>()
                .GetAllAsync(
                    filter: m => m.ContractId == contractId && 
                               !m.IsDeleted &&
                               m.Status != MilestoneStatus.Completed &&
                               m.PlannedDate < today,
                    orderBy: q => q.OrderBy(m => m.PlannedDate),
                    cancellationToken: default);

            return _mapper.Map<IEnumerable<ContractMilestoneDto>>(milestones);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get delayed milestones");
            throw;
        }
    }

    #endregion
}