using Application.Interfaces.Contracts;
using Application.Services.Base;
using Core.DTOs.Contracts.ChangeOrders;
using Core.DTOs.Contracts.Claims;
using Core.DTOs.Contracts.ContractMilestones;
using Core.DTOs.Contracts.Contracts;
using Core.DTOs.Contracts.Valuations;
using Core.Enums.Contracts;
using Domain.Entities.Contracts.Core;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Contracts;

public class ContractService : BaseService<Contract, ContractDto, CreateContractDto, UpdateContractDto>, IContractService
{
    public ContractService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<ContractService> logger)
        : base(unitOfWork, mapper, logger)
    {
    }

    // Override GetByIdAsync to include related data
    public override async Task<ContractDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var contract = await Repository.GetAsync(
            c => c.Id == id,
            includeProperties: "Project,Contractor,ChangeOrders,Milestones,Valuations,Claims,Documents",
            cancellationToken: cancellationToken);

        if (contract == null)
            throw new NotFoundException(nameof(Contract), id);

        return Mapper.Map<ContractDto>(contract);
    }

    // Override CreateAsync to handle contract-specific logic
    public override async Task<ContractDto> CreateAsync(CreateContractDto createDto, CancellationToken cancellationToken = default)
    {
        // Check if contract number already exists
        var existingContract = await Repository.GetAsync(
            c => c.ContractNumber == createDto.ContractNumber,
            cancellationToken: cancellationToken);

        if (existingContract != null)
            throw new ConflictException($"Contract number {createDto.ContractNumber} already exists");

        var contract = Mapper.Map<Contract>(createDto);
        
        // Set default values
        contract.Status = ContractStatus.Draft;
        contract.CurrentValue = contract.OriginalValue;
        contract.CurrentEndDate = contract.OriginalEndDate;
        contract.CreatedBy = await GetCurrentUserIdAsync();

        await Repository.AddAsync(contract, cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(contract.Id, cancellationToken);
    }

    // Contract queries
    public async Task<IEnumerable<ContractDto>> GetByProjectAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var contracts = await Repository.GetAllAsync(
            filter: c => c.ProjectId == projectId,
            includeProperties: "Contractor",
            cancellationToken: cancellationToken);

        return Mapper.Map<IEnumerable<ContractDto>>(contracts);
    }

    public async Task<IEnumerable<ContractDto>> GetByContractorAsync(Guid contractorId, CancellationToken cancellationToken = default)
    {
        var contracts = await Repository.GetAllAsync(
            filter: c => c.ContractorId == contractorId,
            includeProperties: "Project",
            cancellationToken: cancellationToken);

        return Mapper.Map<IEnumerable<ContractDto>>(contracts);
    }

    public async Task<IEnumerable<ContractDto>> GetByStatusAsync(ContractStatus status, CancellationToken cancellationToken = default)
    {
        var contracts = await Repository.GetAllAsync(
            filter: c => c.Status == status,
            includeProperties: "Project,Contractor",
            cancellationToken: cancellationToken);

        return Mapper.Map<IEnumerable<ContractDto>>(contracts);
    }

    public async Task<PagedResult<ContractDto>> GetPagedAsync(ContractQueryParameters parameters, CancellationToken cancellationToken = default)
    {
        // Build filter predicate
        Expression<Func<Contract, bool>>? predicate = null;
        
        if (!string.IsNullOrEmpty(parameters.SearchTerm) || parameters.ProjectId.HasValue || 
            parameters.ContractorId.HasValue || parameters.Status.HasValue || parameters.Type.HasValue)
        {
            predicate = c => 
                (string.IsNullOrEmpty(parameters.SearchTerm) || 
                 c.ContractNumber.Contains(parameters.SearchTerm) ||
                 c.Title.Contains(parameters.SearchTerm) ||
                 c.Description.Contains(parameters.SearchTerm)) &&
                (!parameters.ProjectId.HasValue || c.ProjectId == parameters.ProjectId.Value) &&
                (!parameters.ContractorId.HasValue || c.ContractorId == parameters.ContractorId.Value) &&
                (!parameters.Status.HasValue || c.Status == parameters.Status.Value) &&
                (!parameters.Type.HasValue || c.Type == parameters.Type.Value);
        }

        var pagedResult = await Repository.GetPagedAsync(
            pageNumber: parameters.PageNumber,
            pageSize: parameters.PageSize,
            filter: predicate,
            orderBy: GetOrderBy(parameters.SortBy, parameters.IsAscending),
            includeProperties: "Project,Contractor");

        var dtos = Mapper.Map<IEnumerable<ContractDto>>(pagedResult.Items);

        return new PagedResult<ContractDto>(
            dtos,
            pagedResult.TotalCount,
            parameters.PageNumber,
            parameters.PageSize);
    }

    // Contract lifecycle methods
    public async Task<ContractDto> ApproveAsync(Guid id, ApproveContractDto approveDto, CancellationToken cancellationToken = default)
    {
        var contract = await Repository.GetAsync(
            c => c.Id == id,
            tracked: true,
            cancellationToken: cancellationToken);

        if (contract == null)
            throw new NotFoundException(nameof(Contract), id);

        contract.IsApproved = true;
        contract.ApprovedBy = approveDto.ApprovedBy;
        contract.ApprovalDate = DateTime.UtcNow;
        contract.ApprovalComments = approveDto.Comments;
        contract.Status = ContractStatus.Active;

        Repository.Update(contract);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return Mapper.Map<ContractDto>(contract);
    }

    public async Task<ContractDto> ActivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var contract = await Repository.GetAsync(
            c => c.Id == id,
            tracked: true,
            cancellationToken: cancellationToken);

        if (contract == null)
            throw new NotFoundException(nameof(Contract), id);

        if (!contract.IsApproved)
            throw new BusinessException("Contract must be approved before activation");

        contract.Status = ContractStatus.Active;
        contract.ActualStartDate = DateTime.UtcNow;

        Repository.Update(contract);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return Mapper.Map<ContractDto>(contract);
    }

    public async Task<ContractDto> SuspendAsync(Guid id, string reason, CancellationToken cancellationToken = default)
    {
        var contract = await Repository.GetAsync(
            c => c.Id == id,
            tracked: true,
            cancellationToken: cancellationToken);

        if (contract == null)
            throw new NotFoundException(nameof(Contract), id);

        contract.Status = ContractStatus.Suspended;
        contract.Notes = $"Suspended: {reason}";

        Repository.Update(contract);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return Mapper.Map<ContractDto>(contract);
    }

    public async Task<ContractDto> TerminateAsync(Guid id, TerminateContractDto terminateDto, CancellationToken cancellationToken = default)
    {
        var contract = await Repository.GetAsync(
            c => c.Id == id,
            tracked: true,
            cancellationToken: cancellationToken);

        if (contract == null)
            throw new NotFoundException(nameof(Contract), id);

        contract.Status = ContractStatus.Terminated;
        contract.ActualEndDate = DateTime.UtcNow;
        contract.TerminationClauses = terminateDto.Reason;

        Repository.Update(contract);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return Mapper.Map<ContractDto>(contract);
    }

    public async Task<ContractDto> CompleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var contract = await Repository.GetAsync(
            c => c.Id == id,
            tracked: true,
            cancellationToken: cancellationToken);

        if (contract == null)
            throw new NotFoundException(nameof(Contract), id);

        contract.Status = ContractStatus.Completed;
        contract.ActualEndDate = DateTime.UtcNow;
        contract.PercentageComplete = 100;

        Repository.Update(contract);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return Mapper.Map<ContractDto>(contract);
    }

    // Financial methods
    public async Task<ContractDto> UpdateCurrentValueAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var contract = await Repository.GetAsync(
            c => c.Id == id,
            includeProperties: "ChangeOrders",
            tracked: true,
            cancellationToken: cancellationToken);

        if (contract == null)
            throw new NotFoundException(nameof(Contract), id);

        contract.UpdateCurrentValue();

        Repository.Update(contract);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return Mapper.Map<ContractDto>(contract);
    }

    public async Task<ContractFinancialSummary> GetFinancialSummaryAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var contract = await Repository.GetAsync(
            c => c.Id == id,
            includeProperties: "ChangeOrders,Valuations,Claims",
            cancellationToken: cancellationToken);

        if (contract == null)
            throw new NotFoundException(nameof(Contract), id);

        return new ContractFinancialSummary
        {
            ContractId = id,
            OriginalValue = contract.OriginalValue,
            CurrentValue = contract.CurrentValue,
            ChangeOrderValue = contract.ChangeOrderValue,
            PendingChangeOrderValue = contract.PendingChangeOrderValue,
            AmountInvoiced = contract.AmountInvoiced,
            AmountPaid = contract.AmountPaid,
            RetentionAmount = contract.RetentionAmount,
            OutstandingAmount = contract.AmountInvoiced - contract.AmountPaid,
            PercentageComplete = contract.PercentageComplete,
            TotalClaims = contract.TotalClaimsValue,
            OpenClaims = contract.OpenClaimsCount
        };
    }

    // Analytics methods
    public async Task<ContractPerformanceMetrics> GetPerformanceMetricsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var contract = await Repository.GetAsync(
            c => c.Id == id,
            includeProperties: "Milestones,Valuations",
            cancellationToken: cancellationToken);

        if (contract == null)
            throw new NotFoundException(nameof(Contract), id);

        var plannedDuration = (contract.OriginalEndDate - contract.StartDate).Days;
        var actualDuration = contract.ActualEndDate.HasValue 
            ? (contract.ActualEndDate.Value - contract.StartDate).Days 
            : (DateTime.UtcNow - contract.StartDate).Days;
        
        var scheduleVariance = actualDuration - plannedDuration;
        var costVariance = contract.CurrentValue - contract.AmountPaid;

        return new ContractPerformanceMetrics
        {
            ContractId = id,
            SchedulePerformanceIndex = plannedDuration > 0 ? (decimal)actualDuration / plannedDuration : 0,
            CostPerformanceIndex = contract.AmountPaid > 0 ? contract.CurrentValue / contract.AmountPaid : 0,
            ScheduleVariance = scheduleVariance,
            CostVariance = costVariance,
            TimeElapsed = actualDuration,
            TimeRemaining = Math.Max(0, plannedDuration - actualDuration),
            MilestonesCompleted = contract.CompletedMilestones,
            MilestonesTotal = contract.TotalMilestones,
            IsDelayed = contract.IsDelayed(),
            HasExpiredBonds = contract.HasExpiredBonds()
        };
    }

    public async Task<IEnumerable<ContractRiskIndicator>> GetRiskIndicatorsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var contract = await Repository.GetAsync(
            c => c.Id == id,
            includeProperties: "Claims,ChangeOrders",
            cancellationToken: cancellationToken);

        if (contract == null)
            throw new NotFoundException(nameof(Contract), id);

        var indicators = new List<ContractRiskIndicator>();

        // Schedule risk
        if (contract.IsDelayed())
        {
            indicators.Add(new ContractRiskIndicator
            {
                Type = "Schedule",
                Level = "High",
                Description = "Contract is delayed beyond current end date"
            });
        }

        // Financial risk
        if (contract.ChangeOrderValue > contract.OriginalValue * 0.1m)
        {
            indicators.Add(new ContractRiskIndicator
            {
                Type = "Financial",
                Level = "Medium",
                Description = "Change orders exceed 10% of original value"
            });
        }

        // Claims risk
        if (contract.OpenClaimsCount > 0)
        {
            indicators.Add(new ContractRiskIndicator
            {
                Type = "Claims",
                Level = contract.OpenClaimsCount > 3 ? "High" : "Medium",
                Description = $"{contract.OpenClaimsCount} open claims"
            });
        }

        // Bond risk
        if (contract.HasExpiredBonds())
        {
            indicators.Add(new ContractRiskIndicator
            {
                Type = "Compliance",
                Level = "Critical",
                Description = "Contract has expired bonds"
            });
        }

        return indicators;
    }

    // Private helper methods
    private Func<IQueryable<Contract>, IOrderedQueryable<Contract>> GetOrderBy(string? sortBy, bool isAscending)
    {
        Expression<Func<Contract, object>> orderExpression = sortBy?.ToLower() switch
        {
            "number" => c => c.ContractNumber,
            "title" => c => c.Title,
            "value" => c => c.CurrentValue,
            "status" => c => c.Status,
            "startdate" => c => c.StartDate,
            "enddate" => c => c.CurrentEndDate,
            _ => c => c.CreatedAt
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