using Application.Interfaces.Contracts;
using Application.Services.Base;
using Core.DTOs.Contracts.ValuationItems;
using Core.DTOs.Contracts.Valuations;
using Core.Enums.Contracts;
using Domain.Entities.Contracts.Core;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Application.Services.Contracts;

public class ValuationService : BaseService<Valuation, ValuationDto, CreateValuationDto, UpdateValuationDto>, IValuationService
{
    public ValuationService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<ValuationService> logger)
        : base(unitOfWork, mapper, logger)
    {
    }

    // Override GetByIdAsync to include related data
    public override async Task<ValuationDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var valuation = await Repository.GetAsync(
            v => v.Id == id,
            includeProperties: "Contract,Items,Documents",
            cancellationToken: cancellationToken);

        if (valuation == null)
            throw new NotFoundException(nameof(Valuation), id);

        return Mapper.Map<ValuationDto>(valuation);
    }

    // Override CreateAsync to handle valuation-specific logic
    public override async Task<ValuationDto> CreateAsync(CreateValuationDto createDto, CancellationToken cancellationToken = default)
    {
        // Check if valuation number already exists
        var existingValuation = await Repository.GetAsync(
            v => v.ValuationNumber == createDto.ValuationNumber,
            cancellationToken: cancellationToken);

        if (existingValuation != null)
            throw new ConflictException($"Valuation number {createDto.ValuationNumber} already exists");

        var valuation = Mapper.Map<Valuation>(createDto);

        // Set default values
        valuation.Status = ValuationStatus.Draft;
        valuation.CreatedBy = await GetCurrentUserIdAsync();

        await Repository.AddAsync(valuation, cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(valuation.Id, cancellationToken);
    }

    // Valuation queries
    public async Task<IEnumerable<ValuationDto>> GetByContractAsync(Guid contractId, CancellationToken cancellationToken = default)
    {
        var valuations = await Repository.GetAllAsync(
            filter: v => v.ContractId == contractId,
            orderBy: q => q.OrderByDescending(v => v.ValuationPeriod),
            includeProperties: "Items",
            cancellationToken: cancellationToken);

        return Mapper.Map<IEnumerable<ValuationDto>>(valuations);
    }

    public async Task<IEnumerable<ValuationDto>> GetByStatusAsync(ValuationStatus status, CancellationToken cancellationToken = default)
    {
        var valuations = await Repository.GetAllAsync(
            filter: v => v.Status == status,
            includeProperties: "Contract",
            cancellationToken: cancellationToken);

        return Mapper.Map<IEnumerable<ValuationDto>>(valuations);
    }

    public async Task<ValuationDto?> GetByPeriodAsync(Guid contractId, int period, CancellationToken cancellationToken = default)
    {
        var valuation = await Repository.GetAsync(
            v => v.ContractId == contractId && v.ValuationPeriod == period,
            includeProperties: "Items",
            cancellationToken: cancellationToken);

        return valuation != null ? Mapper.Map<ValuationDto>(valuation) : null;
    }

    public async Task<PagedResult<ValuationDto>> GetPagedAsync(ValuationQueryParameters parameters, CancellationToken cancellationToken = default)
    {
        // Build filter predicate
        Expression<Func<Valuation, bool>>? predicate = null;

        if (!string.IsNullOrEmpty(parameters.SearchTerm) || parameters.ContractId.HasValue ||
            parameters.Status.HasValue || parameters.IsPaid.HasValue)
        {
            predicate = v =>
                (string.IsNullOrEmpty(parameters.SearchTerm) ||
                 v.ValuationNumber.Contains(parameters.SearchTerm) ||
                 v.Title.Contains(parameters.SearchTerm)) &&
                (!parameters.ContractId.HasValue || v.ContractId == parameters.ContractId.Value) &&
                (!parameters.Status.HasValue || v.Status == parameters.Status.Value) &&
                (!parameters.IsPaid.HasValue || v.IsPaid == parameters.IsPaid.Value);
        }

        var pagedResult = await Repository.GetPagedAsync(
            pageNumber: parameters.PageNumber,
            pageSize: parameters.PageSize,
            filter: predicate,
            orderBy: GetOrderBy(parameters.SortBy, parameters.IsAscending),
            includeProperties: "Contract");

        var dtos = Mapper.Map<IEnumerable<ValuationDto>>(pagedResult.Items);

        return new PagedResult<ValuationDto>(
            dtos,
            pagedResult.TotalCount,
            parameters.PageNumber,
            parameters.PageSize);
    }

    // Valuation workflow methods
    public async Task<ValuationDto> SubmitAsync(Guid id, string submittedBy, CancellationToken cancellationToken = default)
    {
        var valuation = await Repository.GetAsync(
            v => v.Id == id,
            tracked: true,
            cancellationToken: cancellationToken);

        if (valuation == null)
            throw new NotFoundException(nameof(Valuation), id);

        valuation.Submit(submittedBy);

        Repository.Update(valuation);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return Mapper.Map<ValuationDto>(valuation);
    }

    public async Task<ValuationDto> ApproveAsync(Guid id, ApproveValuationDto approveDto, CancellationToken cancellationToken = default)
    {
        var valuation = await Repository.GetAsync(
            v => v.Id == id,
            tracked: true,
            cancellationToken: cancellationToken);

        if (valuation == null)
            throw new NotFoundException(nameof(Valuation), id);

        valuation.Approve(
            approveDto.ApprovedBy,
            approveDto.Comments,
            approveDto.AdjustedAmount);

        Repository.Update(valuation);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return Mapper.Map<ValuationDto>(valuation);
    }

    public async Task<ValuationDto> RejectAsync(Guid id, RejectValuationDto rejectDto, CancellationToken cancellationToken = default)
    {
        var valuation = await Repository.GetAsync(
            v => v.Id == id,
            tracked: true,
            cancellationToken: cancellationToken);

        if (valuation == null)
            throw new NotFoundException(nameof(Valuation), id);

        valuation.Reject(rejectDto.RejectedBy, rejectDto.Reason);

        Repository.Update(valuation);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return Mapper.Map<ValuationDto>(valuation);
    }

    public async Task<ValuationDto> CertifyAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var valuation = await Repository.GetAsync(
            v => v.Id == id,
            tracked: true,
            cancellationToken: cancellationToken);

        if (valuation == null)
            throw new NotFoundException(nameof(Valuation), id);

        valuation.Certify();

        Repository.Update(valuation);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return Mapper.Map<ValuationDto>(valuation);
    }

    public async Task<ValuationDto> RecordInvoiceAsync(Guid id, string invoiceNumber, CancellationToken cancellationToken = default)
    {
        var valuation = await Repository.GetAsync(
            v => v.Id == id,
            tracked: true,
            cancellationToken: cancellationToken);

        if (valuation == null)
            throw new NotFoundException(nameof(Valuation), id);

        valuation.RecordInvoice(invoiceNumber);

        Repository.Update(valuation);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return Mapper.Map<ValuationDto>(valuation);
    }

    public async Task<ValuationDto> RecordPaymentAsync(Guid id, RecordValuationPaymentDto paymentDto, CancellationToken cancellationToken = default)
    {
        var valuation = await Repository.GetAsync(
            v => v.Id == id,
            tracked: true,
            cancellationToken: cancellationToken);

        if (valuation == null)
            throw new NotFoundException(nameof(Valuation), id);

        valuation.RecordPayment(paymentDto.Amount);

        Repository.Update(valuation);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return Mapper.Map<ValuationDto>(valuation);
    }

    // Calculation methods
    public async Task<ValuationDto> CalculateAmountsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var valuation = await Repository.GetAsync(
            v => v.Id == id,
            includeProperties: "Contract,Items",
            tracked: true,
            cancellationToken: cancellationToken);

        if (valuation == null)
            throw new NotFoundException(nameof(Valuation), id);

        valuation.CalculateAmounts();

        Repository.Update(valuation);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return Mapper.Map<ValuationDto>(valuation);
    }

    // Valuation Items management
    public async Task<ValuationItemDto> AddItemAsync(Guid valuationId, CreateValuationItemDto createDto, CancellationToken cancellationToken = default)
    {
        var valuation = await Repository.GetAsync(
            v => v.Id == valuationId,
            cancellationToken: cancellationToken);

        if (valuation == null)
            throw new NotFoundException(nameof(Valuation), valuationId);

        var item = Mapper.Map<ValuationItem>(createDto);
        item.ValuationId = valuationId;

        await UnitOfWork.Repository<ValuationItem>().AddAsync(item, cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        // Recalculate valuation
        await CalculateAmountsAsync(valuationId, cancellationToken);

        return Mapper.Map<ValuationItemDto>(item);
    }

    public async Task<ValuationItemDto> UpdateItemAsync(Guid itemId, UpdateValuationItemDto updateDto, CancellationToken cancellationToken = default)
    {
        var item = await UnitOfWork.Repository<ValuationItem>().GetAsync(
            i => i.Id == itemId,
            tracked: true,
            cancellationToken: cancellationToken);

        if (item == null)
            throw new NotFoundException(nameof(ValuationItem), itemId);

        Mapper.Map(updateDto, item);

        UnitOfWork.Repository<ValuationItem>().Update(item);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        // Recalculate valuation
        await CalculateAmountsAsync(item.ValuationId, cancellationToken);

        return Mapper.Map<ValuationItemDto>(item);
    }

    public async Task<bool> RemoveItemAsync(Guid valuationId, Guid itemId, CancellationToken cancellationToken = default)
    {
        var item = await UnitOfWork.Repository<ValuationItem>().GetAsync(
            i => i.Id == itemId && i.ValuationId == valuationId,
            tracked: true,
            cancellationToken: cancellationToken);

        if (item == null)
            return false;

        UnitOfWork.Repository<ValuationItem>().Remove(item);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        // Recalculate valuation
        await CalculateAmountsAsync(valuationId, cancellationToken);

        return true;
    }

    public async Task<IEnumerable<ValuationItemDto>> GetItemsAsync(Guid valuationId, CancellationToken cancellationToken = default)
    {
        var items = await UnitOfWork.Repository<ValuationItem>().GetAllAsync(
            filter: i => i.ValuationId == valuationId,
            orderBy: q => q.OrderBy(i => i.ItemCode),
            cancellationToken: cancellationToken);

        return Mapper.Map<IEnumerable<ValuationItemDto>>(items);
    }

    // Financial summary methods
    public async Task<ValuationFinancialSummary> GetFinancialSummaryAsync(Guid contractId, CancellationToken cancellationToken = default)
    {
        var valuations = await Repository.GetAllAsync(
            filter: v => v.ContractId == contractId,
            cancellationToken: cancellationToken);

        var valuationsList = valuations.ToList();

        return new ValuationFinancialSummary
        {
            ContractId = contractId,
            TotalCertified = valuationsList.Where(v => v.Status == ValuationStatus.Certified).Sum(v => v.NetValuation),
            TotalInvoiced = valuationsList.Where(v => v.IsInvoiced).Sum(v => v.AmountDue),
            TotalPaid = valuationsList.Where(v => v.IsPaid).Sum(v => v.PaymentAmount),
            RetentionHeld = valuationsList.Where(v => v.Status == ValuationStatus.Approved).Sum(v => v.RetentionAmount),
            PendingCertification = valuationsList.Where(v => v.Status == ValuationStatus.Submitted).Sum(v => v.NetValuation),
            OutstandingPayment = valuationsList.Where(v => v.IsInvoiced && !v.IsPaid).Sum(v => v.AmountDue),
            TotalMaterialsOnSite = valuationsList.OrderByDescending(v => v.ValuationPeriod).FirstOrDefault()?.MaterialsOnSite ?? 0,
            TotalMaterialsOffSite = valuationsList.OrderByDescending(v => v.ValuationPeriod).FirstOrDefault()?.MaterialsOffSite ?? 0
        };
    }

    // Analytics methods
    public async Task<Dictionary<ValuationStatus, int>> GetStatusSummaryAsync(Guid? contractId = null, CancellationToken cancellationToken = default)
    {
        Expression<Func<Valuation, bool>>? filter = null;
        if (contractId.HasValue)
        {
            filter = v => v.ContractId == contractId.Value;
        }

        var valuations = await Repository.GetAllAsync(
            filter: filter,
            cancellationToken: cancellationToken);

        return valuations
            .GroupBy(v => v.Status)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    public async Task<IEnumerable<ValuationTrendData>> GetPaymentTrendAsync(Guid contractId, int periods = 12, CancellationToken cancellationToken = default)
    {
        var valuations = await Repository.GetAllAsync(
            filter: v => v.ContractId == contractId && v.ValuationPeriod <= periods,
            orderBy: q => q.OrderBy(v => v.ValuationPeriod),
            cancellationToken: cancellationToken);

        return valuations.Select(v => new ValuationTrendData
        {
            Period = v.ValuationPeriod,
            Date = v.ValuationDate,
            CumulativeValue = v.TotalCompletedWork,
            PeriodValue = v.CurrentPeriodWork,
            RetentionAmount = v.RetentionAmount,
            NetAmount = v.NetValuation
        });
    }

    // Private helper methods
    private Func<IQueryable<Valuation>, IOrderedQueryable<Valuation>> GetOrderBy(string? sortBy, bool isAscending)
    {
        Expression<Func<Valuation, object>> orderExpression = sortBy?.ToLower() switch
        {
            "number" => v => v.ValuationNumber,
            "period" => v => v.ValuationPeriod,
            "date" => v => v.ValuationDate,
            "amount" => v => v.NetValuation,
            "status" => v => v.Status,
            _ => v => v.CreatedAt
        };

        return isAscending
            ? q => q.OrderBy(orderExpression)
            : q => q.OrderByDescending(orderExpression);
    }

    private async Task<string> GetCurrentUserIdAsync()
    {
        // This should be injected from ICurrentUserService
        return "System";
    }
}