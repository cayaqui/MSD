using Application.Interfaces.Contracts;
using Application.Interfaces.Common;
using Application.Interfaces.Auth;
using AutoMapper;
using Core.DTOs.Common;
using Core.DTOs.Contracts.Valuations;
using Core.DTOs.Contracts.ValuationItems;
using Core.Enums.Contracts;
using Domain.Entities.Contracts.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services.Contracts;

public class ValuationService : IValuationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<ValuationService> _logger;
    private readonly ICurrentUserService _currentUserService;

    public ValuationService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<ValuationService> logger,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _currentUserService = currentUserService;
    }

    #region IBaseService Implementation

    public async Task<ValuationDto> CreateAsync(CreateValuationDto createDto, string? createdBy = null, CancellationToken cancellationToken = default)
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

            // Check for duplicate valuation number
            var existingValuation = await _unitOfWork.Repository<Valuation>()
                .AnyAsync(
                    v => v.ValuationNumber == createDto.ValuationNumber && !v.IsDeleted,
                    cancellationToken);

            if (existingValuation)
                throw new InvalidOperationException($"Valuation number {createDto.ValuationNumber} already exists");

            // Get previous valuation to determine period
            var previousValuation = await GetLatestAsync(createDto.ContractId);
            var period = previousValuation != null ? previousValuation.ValuationPeriod + 1 : 1;

            var valuation = _mapper.Map<Valuation>(createDto);
            valuation.CreatedBy = createdBy ?? _currentUserService.UserId ?? "System";
            valuation.Status = ValuationStatus.Draft;
            valuation.ValuationPeriod = period;
            valuation.PreviousValuationId = previousValuation?.Id;
            valuation.PreparedDate = DateTime.UtcNow;

            await _unitOfWork.Repository<Valuation>().AddAsync(valuation);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return await GetByIdAsync(valuation.Id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create valuation");
            throw;
        }
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var valuation = await _unitOfWork.Repository<Valuation>()
                .GetAsync(
                    filter: v => v.Id == id && !v.IsDeleted,
                    cancellationToken: cancellationToken);

            if (valuation == null)
                return false;

            if (valuation.Status != ValuationStatus.Draft)
                throw new InvalidOperationException("Only draft valuations can be deleted");

            valuation.IsDeleted = true;
            valuation.DeletedAt = DateTime.UtcNow;
            valuation.DeletedBy = _currentUserService.UserId ?? "System";

            _unitOfWork.Repository<Valuation>().Update(valuation);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete valuation");
            throw;
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Repository<Valuation>()
            .AnyAsync(v => v.Id == id && !v.IsDeleted, cancellationToken);
    }

    public async Task<IEnumerable<ValuationDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var valuations = await _unitOfWork.Repository<Valuation>()
            .GetAllAsync(
                filter: v => !v.IsDeleted,
                includeProperties: "Contract",
                cancellationToken: cancellationToken);

        return _mapper.Map<IEnumerable<ValuationDto>>(valuations);
    }

    public async Task<PagedResult<ValuationDto>> GetAllPagedAsync(int pageNumber = 1, int pageSize = 20, string? orderBy = null, bool descending = false, CancellationToken cancellationToken = default)
    {
        var query = _unitOfWork.Repository<Valuation>()
            .Query()
            .Where(v => !v.IsDeleted)
            .Include(v => v.Contract);

        // Apply ordering
        query = orderBy?.ToLower() switch
        {
            "number" => descending ? query.OrderByDescending(v => v.ValuationNumber) : query.OrderBy(v => v.ValuationNumber),
            "period" => descending ? query.OrderByDescending(v => v.ValuationPeriod) : query.OrderBy(v => v.ValuationPeriod),
            "date" => descending ? query.OrderByDescending(v => v.PreparedDate) : query.OrderBy(v => v.PreparedDate),
            "amount" => descending ? query.OrderByDescending(v => v.NetValuation) : query.OrderBy(v => v.NetValuation),
            "status" => descending ? query.OrderByDescending(v => v.Status) : query.OrderBy(v => v.Status),
            _ => descending ? query.OrderByDescending(v => v.CreatedAt) : query.OrderBy(v => v.CreatedAt)
        };

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var dtos = _mapper.Map<List<ValuationDto>>(items);

        return new PagedResult<ValuationDto>(dtos, totalCount, pageNumber, pageSize);
    }

    public async Task<ValuationDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var valuation = await _unitOfWork.Repository<Valuation>()
            .GetAsync(
                filter: v => v.Id == id && !v.IsDeleted,
                includeProperties: "Contract,Contract.Project,Items",
                cancellationToken: cancellationToken);

        return valuation != null ? _mapper.Map<ValuationDto>(valuation) : null;
    }

    public async Task<ValuationDto?> UpdateAsync(Guid id, UpdateValuationDto updateDto, string? updatedBy = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var valuation = await _unitOfWork.Repository<Valuation>()
                .GetAsync(
                    filter: v => v.Id == id && !v.IsDeleted,
                    cancellationToken: cancellationToken);

            if (valuation == null)
                return null;

            if (valuation.Status != ValuationStatus.Draft)
                throw new InvalidOperationException("Can only update draft valuations");

            _mapper.Map(updateDto, valuation);
            valuation.UpdatedBy = updatedBy ?? _currentUserService.UserId ?? "System";
            valuation.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Valuation>().Update(valuation);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return await GetByIdAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update valuation");
            throw;
        }
    }

    #endregion

    #region IValuationService Implementation

    public async Task<IEnumerable<ValuationDto>> GetByContractAsync(Guid contractId)
    {
        var valuations = await _unitOfWork.Repository<Valuation>()
            .GetAllAsync(
                filter: v => v.ContractId == contractId && !v.IsDeleted,
                orderBy: q => q.OrderByDescending(v => v.ValuationPeriod),
                cancellationToken: default);

        return _mapper.Map<IEnumerable<ValuationDto>>(valuations);
    }

    public async Task<ValuationDto?> GetByValuationNumberAsync(string valuationNumber)
    {
        var valuation = await _unitOfWork.Repository<Valuation>()
            .GetAsync(
                filter: v => v.ValuationNumber == valuationNumber && !v.IsDeleted,
                includeProperties: "Contract",
                cancellationToken: default);

        return valuation != null ? _mapper.Map<ValuationDto>(valuation) : null;
    }

    public async Task<ValuationDto?> GetByPeriodAsync(Guid contractId, int period)
    {
        var valuation = await _unitOfWork.Repository<Valuation>()
            .GetAsync(
                filter: v => v.ContractId == contractId && 
                           v.ValuationPeriod == period && 
                           !v.IsDeleted,
                includeProperties: "Contract,Items",
                cancellationToken: default);

        return valuation != null ? _mapper.Map<ValuationDto>(valuation) : null;
    }

    public async Task<IEnumerable<ValuationDto>> SearchAsync(ValuationFilterDto filter)
    {
        var query = _unitOfWork.Repository<Valuation>()
            .Query()
            .Where(v => !v.IsDeleted);

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var searchTerm = filter.SearchTerm.ToLower();
            query = query.Where(v => 
                v.ValuationNumber.ToLower().Contains(searchTerm) ||
                v.Comments.ToLower().Contains(searchTerm));
        }

        if (filter.ContractId.HasValue)
            query = query.Where(v => v.ContractId == filter.ContractId.Value);

        if (filter.Status.HasValue)
            query = query.Where(v => v.Status == filter.Status.Value);

        if (filter.DateFrom.HasValue)
            query = query.Where(v => v.PreparedDate >= filter.DateFrom.Value);

        if (filter.DateTo.HasValue)
            query = query.Where(v => v.PreparedDate <= filter.DateTo.Value);

        if (filter.MinAmount.HasValue)
            query = query.Where(v => v.NetValuation >= filter.MinAmount.Value);

        if (filter.MaxAmount.HasValue)
            query = query.Where(v => v.NetValuation <= filter.MaxAmount.Value);

        var valuations = await query
            .Include(v => v.Contract)
            .OrderByDescending(v => v.ValuationPeriod)
            .ToListAsync();

        return _mapper.Map<IEnumerable<ValuationDto>>(valuations);
    }

    public async Task<ValuationDto?> GetLatestAsync(Guid contractId)
    {
        var valuation = await _unitOfWork.Repository<Valuation>()
            .GetAllAsync(
                filter: v => v.ContractId == contractId && !v.IsDeleted,
                orderBy: q => q.OrderByDescending(v => v.ValuationPeriod),
                includeProperties: "Contract,Items",
                cancellationToken: default);

        var latest = valuation.FirstOrDefault();
        return latest != null ? _mapper.Map<ValuationDto>(latest) : null;
    }

    public async Task<ValuationDto?> GetPreviousAsync(Guid valuationId)
    {
        var currentValuation = await _unitOfWork.Repository<Valuation>()
            .GetAsync(
                filter: v => v.Id == valuationId && !v.IsDeleted,
                cancellationToken: default);

        if (currentValuation == null || currentValuation.PreviousValuationId == null)
            return null;

        var previousValuation = await _unitOfWork.Repository<Valuation>()
            .GetAsync(
                filter: v => v.Id == currentValuation.PreviousValuationId.Value && !v.IsDeleted,
                includeProperties: "Contract,Items",
                cancellationToken: default);

        return previousValuation != null ? _mapper.Map<ValuationDto>(previousValuation) : null;
    }

    public async Task<ValuationDto?> SubmitAsync(Guid valuationId, SubmitValuationDto dto)
    {
        try
        {
            var valuation = await _unitOfWork.Repository<Valuation>()
                .GetAsync(
                    filter: v => v.Id == valuationId && !v.IsDeleted,
                    cancellationToken: default);

            if (valuation == null)
                return null;

            if (valuation.Status != ValuationStatus.Draft)
                throw new InvalidOperationException("Only draft valuations can be submitted");

            valuation.Status = ValuationStatus.Submitted;
            valuation.SubmittedDate = DateTime.UtcNow;
            valuation.SubmittedBy = dto.SubmittedBy;
            valuation.UpdatedBy = _currentUserService.UserId ?? "System";
            valuation.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Valuation>().Update(valuation);
            await _unitOfWork.SaveChangesAsync(default);

            return await GetByIdAsync(valuationId, default);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to submit valuation");
            throw;
        }
    }

    public async Task<ValuationDto?> ApproveAsync(Guid valuationId, ApproveValuationDto dto, string approvedBy)
    {
        try
        {
            var valuation = await _unitOfWork.Repository<Valuation>()
                .GetAsync(
                    filter: v => v.Id == valuationId && !v.IsDeleted,
                    cancellationToken: default);

            if (valuation == null)
                return null;

            if (valuation.Status != ValuationStatus.Submitted)
                throw new InvalidOperationException("Only submitted valuations can be approved");

            valuation.Status = ValuationStatus.Approved;
            valuation.ApprovedBy = approvedBy;
            valuation.ApprovedDate = DateTime.UtcNow;
            valuation.ApprovalComments = dto.Comments;
            valuation.UpdatedBy = _currentUserService.UserId ?? "System";
            valuation.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Valuation>().Update(valuation);
            await _unitOfWork.SaveChangesAsync(default);

            return await GetByIdAsync(valuationId, default);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to approve valuation");
            throw;
        }
    }

    public async Task<ValuationDto?> RejectAsync(Guid valuationId, RejectValuationDto dto, string rejectedBy)
    {
        try
        {
            var valuation = await _unitOfWork.Repository<Valuation>()
                .GetAsync(
                    filter: v => v.Id == valuationId && !v.IsDeleted,
                    cancellationToken: default);

            if (valuation == null)
                return null;

            if (valuation.Status != ValuationStatus.Submitted)
                throw new InvalidOperationException("Only submitted valuations can be rejected");

            valuation.Status = ValuationStatus.Rejected;
            valuation.ApprovedBy = rejectedBy; // Store rejector
            valuation.ApprovedDate = DateTime.UtcNow;
            valuation.ApprovalComments = $"Rejected: {dto.Reason}";
            valuation.UpdatedBy = _currentUserService.UserId ?? "System";
            valuation.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Valuation>().Update(valuation);
            await _unitOfWork.SaveChangesAsync(default);

            return await GetByIdAsync(valuationId, default);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to reject valuation");
            throw;
        }
    }

    public async Task<ValuationDto?> CertifyAsync(Guid valuationId)
    {
        try
        {
            var valuation = await _unitOfWork.Repository<Valuation>()
                .GetAsync(
                    filter: v => v.Id == valuationId && !v.IsDeleted,
                    cancellationToken: default);

            if (valuation == null)
                return null;

            if (valuation.Status != ValuationStatus.Approved)
                throw new InvalidOperationException("Only approved valuations can be certified");

            valuation.Status = ValuationStatus.Certified;
            valuation.CertifiedDate = DateTime.UtcNow;
            valuation.UpdatedBy = _currentUserService.UserId ?? "System";
            valuation.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Valuation>().Update(valuation);
            await _unitOfWork.SaveChangesAsync(default);

            return await GetByIdAsync(valuationId, default);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to certify valuation");
            throw;
        }
    }

    public async Task<ValuationDto?> CalculateAmountsAsync(Guid valuationId)
    {
        try
        {
            var valuation = await _unitOfWork.Repository<Valuation>()
                .GetAsync(
                    filter: v => v.Id == valuationId && !v.IsDeleted,
                    includeProperties: "Items,Contract",
                    cancellationToken: default);

            if (valuation == null)
                return null;

            // Calculate gross valuation from items
            valuation.GrossValuation = valuation.Items.Sum(i => i.Amount);
            
            // Add materials
            valuation.GrossValuation += valuation.MaterialsOnSite ?? 0;
            valuation.GrossValuation += valuation.MaterialsOffSite ?? 0;
            
            // Get previous valuation
            decimal previousGross = 0;
            if (valuation.PreviousValuationId.HasValue)
            {
                var previousValuation = await _unitOfWork.Repository<Valuation>()
                    .GetAsync(v => v.Id == valuation.PreviousValuationId.Value);
                previousGross = previousValuation?.GrossValuation ?? 0;
            }
            
            // Calculate net valuation (current period only)
            valuation.NetValuation = valuation.GrossValuation - previousGross;
            
            // Calculate retention
            valuation.RetentionPercentage = valuation.Contract.RetentionPercentage;
            valuation.RetentionAmount = valuation.GrossValuation * (valuation.RetentionPercentage / 100);
            
            // Calculate amount due
            valuation.AmountDue = valuation.NetValuation - (valuation.RetentionAmount - (previousGross * (valuation.RetentionPercentage / 100)));
            
            valuation.UpdatedBy = _currentUserService.UserId ?? "System";
            valuation.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Valuation>().Update(valuation);
            await _unitOfWork.SaveChangesAsync(default);

            return await GetByIdAsync(valuationId, default);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to calculate valuation amounts");
            throw;
        }
    }

    public async Task<ValuationDto?> RecalculateAsync(Guid valuationId)
    {
        return await CalculateAmountsAsync(valuationId);
    }

    public async Task<decimal> GetCumulativeValueAsync(Guid contractId)
    {
        try
        {
            var valuations = await _unitOfWork.Repository<Valuation>()
                .GetAllAsync(
                    filter: v => v.ContractId == contractId && 
                               v.Status == ValuationStatus.Certified &&
                               !v.IsDeleted,
                    cancellationToken: default);

            return valuations.Sum(v => v.GrossValuation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get cumulative value");
            throw;
        }
    }

    public async Task<ValuationDto?> AddItemAsync(Guid valuationId, CreateValuationItemDto item)
    {
        try
        {
            var valuation = await _unitOfWork.Repository<Valuation>()
                .GetAsync(
                    filter: v => v.Id == valuationId && !v.IsDeleted,
                    cancellationToken: default);

            if (valuation == null)
                return null;

            if (valuation.Status != ValuationStatus.Draft)
                throw new InvalidOperationException("Can only add items to draft valuations");

            var valuationItem = _mapper.Map<ValuationItem>(item);
            valuationItem.ValuationId = valuationId;
            valuationItem.CreatedBy = _currentUserService.UserId ?? "System";

            await _unitOfWork.Repository<ValuationItem>().AddAsync(valuationItem);
            await _unitOfWork.SaveChangesAsync(default);

            // Recalculate amounts
            return await CalculateAmountsAsync(valuationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add valuation item");
            throw;
        }
    }

    public async Task<ValuationDto?> UpdateItemAsync(Guid itemId, UpdateValuationItemDto item)
    {
        try
        {
            var valuationItem = await _unitOfWork.Repository<ValuationItem>()
                .GetAsync(
                    filter: vi => vi.Id == itemId && !vi.IsDeleted,
                    includeProperties: "Valuation",
                    cancellationToken: default);

            if (valuationItem == null)
                return null;

            if (valuationItem.Valuation.Status != ValuationStatus.Draft)
                throw new InvalidOperationException("Can only update items in draft valuations");

            _mapper.Map(item, valuationItem);
            valuationItem.UpdatedBy = _currentUserService.UserId ?? "System";
            valuationItem.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<ValuationItem>().Update(valuationItem);
            await _unitOfWork.SaveChangesAsync(default);

            // Recalculate amounts
            return await CalculateAmountsAsync(valuationItem.ValuationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update valuation item");
            throw;
        }
    }

    public async Task<bool> RemoveItemAsync(Guid valuationId, Guid itemId)
    {
        try
        {
            var valuation = await _unitOfWork.Repository<Valuation>()
                .GetAsync(v => v.Id == valuationId && !v.IsDeleted);

            if (valuation == null || valuation.Status != ValuationStatus.Draft)
                return false;

            var item = await _unitOfWork.Repository<ValuationItem>()
                .GetAsync(vi => vi.Id == itemId && vi.ValuationId == valuationId);

            if (item == null)
                return false;

            item.IsDeleted = true;
            item.DeletedAt = DateTime.UtcNow;
            item.DeletedBy = _currentUserService.UserId ?? "System";

            _unitOfWork.Repository<ValuationItem>().Update(item);
            await _unitOfWork.SaveChangesAsync(default);

            // Recalculate amounts
            await CalculateAmountsAsync(valuationId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove valuation item");
            throw;
        }
    }

    public async Task<IEnumerable<ValuationItemDto>> GetItemsAsync(Guid valuationId)
    {
        var items = await _unitOfWork.Repository<ValuationItem>()
            .GetAllAsync(
                filter: vi => vi.ValuationId == valuationId && !vi.IsDeleted,
                orderBy: q => q.OrderBy(vi => vi.ItemCode),
                cancellationToken: default);

        return _mapper.Map<IEnumerable<ValuationItemDto>>(items);
    }

    public async Task<bool> ImportItemsFromPreviousAsync(Guid valuationId)
    {
        try
        {
            var valuation = await _unitOfWork.Repository<Valuation>()
                .GetAsync(v => v.Id == valuationId && !v.IsDeleted);

            if (valuation == null || valuation.Status != ValuationStatus.Draft)
                return false;

            if (!valuation.PreviousValuationId.HasValue)
                return false;

            var previousItems = await _unitOfWork.Repository<ValuationItem>()
                .GetAllAsync(
                    filter: vi => vi.ValuationId == valuation.PreviousValuationId.Value && !vi.IsDeleted,
                    cancellationToken: default);

            foreach (var prevItem in previousItems)
            {
                var newItem = new ValuationItem
                {
                    ValuationId = valuationId,
                    ItemCode = prevItem.ItemCode,
                    Description = prevItem.Description,
                    Unit = prevItem.Unit,
                    UnitRate = prevItem.UnitRate,
                    Quantity = 0, // Start with zero quantity for new period
                    Amount = 0,
                    CreatedBy = _currentUserService.UserId ?? "System"
                };

                await _unitOfWork.Repository<ValuationItem>().AddAsync(newItem);
            }

            await _unitOfWork.SaveChangesAsync(default);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to import items from previous valuation");
            throw;
        }
    }

    public async Task<ValuationDto?> RecordInvoiceAsync(Guid valuationId, string invoiceNumber)
    {
        try
        {
            var valuation = await _unitOfWork.Repository<Valuation>()
                .GetAsync(
                    filter: v => v.Id == valuationId && !v.IsDeleted,
                    cancellationToken: default);

            if (valuation == null)
                return null;

            if (valuation.Status != ValuationStatus.Certified)
                throw new InvalidOperationException("Only certified valuations can be invoiced");

            valuation.IsInvoiced = true;
            valuation.InvoiceNumber = invoiceNumber;
            valuation.InvoiceDate = DateTime.UtcNow;
            valuation.UpdatedBy = _currentUserService.UserId ?? "System";
            valuation.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Valuation>().Update(valuation);
            await _unitOfWork.SaveChangesAsync(default);

            return await GetByIdAsync(valuationId, default);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to record invoice");
            throw;
        }
    }

    public async Task<ValuationDto?> RecordPaymentAsync(Guid valuationId, decimal amount)
    {
        try
        {
            var valuation = await _unitOfWork.Repository<Valuation>()
                .GetAsync(
                    filter: v => v.Id == valuationId && !v.IsDeleted,
                    cancellationToken: default);

            if (valuation == null)
                return null;

            if (!valuation.IsInvoiced)
                throw new InvalidOperationException("Valuation must be invoiced before payment");

            valuation.IsPaid = true;
            valuation.PaymentAmount = amount;
            valuation.PaymentDate = DateTime.UtcNow;
            valuation.UpdatedBy = _currentUserService.UserId ?? "System";
            valuation.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Valuation>().Update(valuation);
            await _unitOfWork.SaveChangesAsync(default);

            return await GetByIdAsync(valuationId, default);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to record payment");
            throw;
        }
    }

    public async Task<IEnumerable<ValuationDto>> GetUnpaidAsync(Guid? contractId = null)
    {
        var query = _unitOfWork.Repository<Valuation>()
            .Query()
            .Where(v => !v.IsDeleted && v.IsInvoiced && !v.IsPaid);

        if (contractId.HasValue)
            query = query.Where(v => v.ContractId == contractId.Value);

        var valuations = await query
            .Include(v => v.Contract)
            .OrderBy(v => v.InvoiceDate)
            .ToListAsync();

        return _mapper.Map<IEnumerable<ValuationDto>>(valuations);
    }

    public async Task<IEnumerable<ValuationDto>> GetOverduePaymentsAsync(int daysOverdue = 30)
    {
        var overdueDate = DateTime.UtcNow.AddDays(-daysOverdue);
        var valuations = await _unitOfWork.Repository<Valuation>()
            .GetAllAsync(
                filter: v => !v.IsDeleted && 
                           v.IsInvoiced && 
                           !v.IsPaid &&
                           v.InvoiceDate < overdueDate,
                includeProperties: "Contract",
                orderBy: q => q.OrderBy(v => v.InvoiceDate),
                cancellationToken: default);

        return _mapper.Map<IEnumerable<ValuationDto>>(valuations);
    }

    public async Task<ValuationDto?> UpdateMaterialsAsync(Guid valuationId, decimal onSite, decimal offSite)
    {
        try
        {
            var valuation = await _unitOfWork.Repository<Valuation>()
                .GetAsync(
                    filter: v => v.Id == valuationId && !v.IsDeleted,
                    cancellationToken: default);

            if (valuation == null)
                return null;

            if (valuation.Status != ValuationStatus.Draft)
                throw new InvalidOperationException("Can only update materials in draft valuations");

            valuation.MaterialsOnSite = onSite;
            valuation.MaterialsOffSite = offSite;
            valuation.UpdatedBy = _currentUserService.UserId ?? "System";
            valuation.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Valuation>().Update(valuation);
            await _unitOfWork.SaveChangesAsync(default);

            // Recalculate amounts
            return await CalculateAmountsAsync(valuationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update materials");
            throw;
        }
    }

    public async Task<decimal> GetTotalMaterialsValueAsync(Guid contractId)
    {
        try
        {
            var latestValuation = await GetLatestAsync(contractId);
            if (latestValuation == null)
                return 0;

            return (latestValuation.MaterialsOnSite ?? 0) + (latestValuation.MaterialsOffSite ?? 0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get total materials value");
            throw;
        }
    }

    public async Task<ValuationSummaryDto> GetSummaryAsync(Guid contractId)
    {
        try
        {
            var valuations = await _unitOfWork.Repository<Valuation>()
                .GetAllAsync(
                    filter: v => v.ContractId == contractId && !v.IsDeleted,
                    cancellationToken: default);

            var valuationsList = valuations.ToList();
            var latestValuation = valuationsList.OrderByDescending(v => v.ValuationPeriod).FirstOrDefault();

            return new ValuationSummaryDto
            {
                ContractId = contractId,
                TotalValuations = valuationsList.Count,
                LatestPeriod = latestValuation?.ValuationPeriod ?? 0,
                TotalCertified = valuationsList.Where(v => v.Status == ValuationStatus.Certified).Sum(v => v.NetValuation),
                TotalInvoiced = valuationsList.Where(v => v.IsInvoiced).Sum(v => v.AmountDue),
                TotalPaid = valuationsList.Where(v => v.IsPaid).Sum(v => v.PaymentAmount),
                OutstandingAmount = valuationsList.Where(v => v.IsInvoiced && !v.IsPaid).Sum(v => v.AmountDue),
                RetentionHeld = latestValuation?.RetentionAmount ?? 0,
                MaterialsValue = (latestValuation?.MaterialsOnSite ?? 0) + (latestValuation?.MaterialsOffSite ?? 0)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get valuation summary");
            throw;
        }
    }

    public async Task<IEnumerable<ValuationDto>> GetPendingApprovalAsync()
    {
        var valuations = await _unitOfWork.Repository<Valuation>()
            .GetAllAsync(
                filter: v => !v.IsDeleted && v.Status == ValuationStatus.Submitted,
                includeProperties: "Contract",
                orderBy: q => q.OrderBy(v => v.SubmittedDate),
                cancellationToken: default);

        return _mapper.Map<IEnumerable<ValuationDto>>(valuations);
    }

    public async Task<Dictionary<ValuationStatus, int>> GetStatusSummaryAsync(Guid? contractId = null)
    {
        try
        {
            var query = _unitOfWork.Repository<Valuation>()
                .Query()
                .Where(v => !v.IsDeleted);

            if (contractId.HasValue)
                query = query.Where(v => v.ContractId == contractId.Value);

            var valuations = await query.ToListAsync();

            return valuations
                .GroupBy(v => v.Status)
                .ToDictionary(g => g.Key, g => g.Count());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get status summary");
            throw;
        }
    }

    public async Task<decimal> GetRetentionHeldAsync(Guid contractId)
    {
        try
        {
            var latestValuation = await GetLatestAsync(contractId);
            return latestValuation?.RetentionAmount ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get retention held");
            throw;
        }
    }

    public async Task<bool> AttachDocumentAsync(Guid valuationId, Guid documentId, string documentType)
    {
        try
        {
            // This would add to a ValuationDocument table
            await Task.CompletedTask;
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to attach document to valuation");
            throw;
        }
    }

    public async Task<bool> RemoveDocumentAsync(Guid valuationId, Guid documentId)
    {
        try
        {
            await Task.CompletedTask;
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove document from valuation");
            throw;
        }
    }

    public async Task<IEnumerable<ValuationDocument>> GetDocumentsAsync(Guid valuationId)
    {
        try
        {
            await Task.CompletedTask;
            return new List<ValuationDocument>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get valuation documents");
            throw;
        }
    }

    public async Task<byte[]> ExportToExcelAsync(Guid valuationId)
    {
        try
        {
            // This would use IExcelExportService
            await Task.CompletedTask;
            return Array.Empty<byte>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to export valuation to Excel");
            throw;
        }
    }

    public async Task<byte[]> GenerateCertificateAsync(Guid valuationId)
    {
        try
        {
            // This would generate a PDF certificate
            await Task.CompletedTask;
            return Array.Empty<byte>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate certificate");
            throw;
        }
    }

    public async Task<ValuationDto?> CreateFromTemplateAsync(Guid contractId, Guid templateId)
    {
        try
        {
            // This would create a new valuation based on a template
            await Task.CompletedTask;
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create valuation from template");
            throw;
        }
    }

    #endregion
}