using Core.DTOs.Contracts.ChangeOrders;
using Core.DTOs.Contracts.Claims;
using Core.DTOs.Contracts.Contracts;
using Core.DTOs.Contracts.Valuations;


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
        var contract = await _unitOfWork.Repository<Contract>().GetAsync(
            c => c.Id == id,
            includeProperties: "Project,Contractor,ChangeOrders,Milestones,Valuations,Claims,Documents",
            cancellationToken: cancellationToken);

        if (contract == null)
            throw new NotFoundException(nameof(Contract), id);

        return _mapper.Map<ContractDto>(contract);
    }

    // Override CreateAsync to handle contract-specific logic
    public new async Task<ContractDto> CreateAsync(CreateContractDto createDto, string? createdBy = null, CancellationToken cancellationToken = default)
    {
        // Check if contract number already exists
        var existingContract = await _unitOfWork.Repository<Contract>().GetAsync(
            c => c.ContractNumber == createDto.ContractNumber,
            cancellationToken: cancellationToken);

        if (existingContract != null)
            throw new ConflictException($"Contract number {createDto.ContractNumber} already exists");

        var contract = _mapper.Map<Contract>(createDto);
        
        // Set default values
        contract.Status = ContractStatus.Draft;
        contract.CurrentValue = contract.OriginalValue;
        contract.CurrentEndDate = contract.OriginalEndDate;
        contract.CreatedBy = await GetCurrentUserIdAsync();

        await _unitOfWork.Repository<Contract>().AddAsync(contract, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(contract.Id, cancellationToken);
    }

    // Contract queries
    public async Task<IEnumerable<ContractDto>> GetByProjectAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var contracts = await _unitOfWork.Repository<Contract>().GetAllAsync(
            filter: c => c.ProjectId == projectId,
            includeProperties: "Contractor",
            cancellationToken: cancellationToken);

        return _mapper.Map<IEnumerable<ContractDto>>(contracts);
    }

    public async Task<IEnumerable<ContractDto>> GetByContractorAsync(Guid contractorId, CancellationToken cancellationToken = default)
    {
        var contracts = await _unitOfWork.Repository<Contract>().GetAllAsync(
            filter: c => c.ContractorId == contractorId,
            includeProperties: "Project",
            cancellationToken: cancellationToken);

        return _mapper.Map<IEnumerable<ContractDto>>(contracts);
    }

    public async Task<IEnumerable<ContractDto>> GetByStatusAsync(ContractStatus status, CancellationToken cancellationToken = default)
    {
        var contracts = await _unitOfWork.Repository<Contract>().GetAllAsync(
            filter: c => c.Status == status,
            includeProperties: "Project,Contractor",
            cancellationToken: cancellationToken);

        return _mapper.Map<IEnumerable<ContractDto>>(contracts);
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

        var pagedResult = await _unitOfWork.Repository<Contract>().GetPagedAsync(
            pageNumber: parameters.PageNumber,
            pageSize: parameters.PageSize,
            filter: predicate,
            orderBy: GetOrderBy(parameters.SortBy, parameters.IsAscending),
            includeProperties: "Project,Contractor");

        var dtos = _mapper.Map<IEnumerable<ContractDto>>(pagedResult.Items);

        return new PagedResult<ContractDto>(
            dtos,
            parameters.PageNumber,
            parameters.PageSize);
    }

    // Contract lifecycle methods
    public async Task<ContractDto> ApproveAsync(Guid id, ApproveContractDto approveDto, CancellationToken cancellationToken = default)
    {
        var contract = await _unitOfWork.Repository<Contract>().GetAsync(
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

        _unitOfWork.Repository<Contract>().Update(contract);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ContractDto>(contract);
    }

    public async Task<ContractDto> ActivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var contract = await _unitOfWork.Repository<Contract>().GetAsync(
            c => c.Id == id,
            tracked: true,
            cancellationToken: cancellationToken);

        if (contract == null)
            throw new NotFoundException(nameof(Contract), id);

        if (!contract.IsApproved)
            throw new InvalidOperationException("Contract must be approved before activation");

        contract.Status = ContractStatus.Active;
        contract.ActualStartDate = DateTime.UtcNow;

        _unitOfWork.Repository<Contract>().Update(contract);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ContractDto>(contract);
    }

    public async Task<ContractDto> SuspendAsync(Guid id, string reason, CancellationToken cancellationToken = default)
    {
        var contract = await _unitOfWork.Repository<Contract>().GetAsync(
            c => c.Id == id,
            tracked: true,
            cancellationToken: cancellationToken);

        if (contract == null)
            throw new NotFoundException(nameof(Contract), id);

        contract.Status = ContractStatus.OnHold;
        contract.Notes = $"Suspended: {reason}";

        _unitOfWork.Repository<Contract>().Update(contract);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ContractDto>(contract);
    }

    public async Task<ContractDto> TerminateAsync(Guid id, TerminateContractDto terminateDto, CancellationToken cancellationToken = default)
    {
        var contract = await _unitOfWork.Repository<Contract>().GetAsync(
            c => c.Id == id,
            tracked: true,
            cancellationToken: cancellationToken);

        if (contract == null)
            throw new NotFoundException(nameof(Contract), id);

        contract.Status = ContractStatus.Terminated;
        contract.ActualEndDate = DateTime.UtcNow;
        contract.TerminationClauses = terminateDto.Reason;

        _unitOfWork.Repository<Contract>().Update(contract);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ContractDto>(contract);
    }

    public async Task<ContractDto> CompleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var contract = await _unitOfWork.Repository<Contract>().GetAsync(
            c => c.Id == id,
            tracked: true,
            cancellationToken: cancellationToken);

        if (contract == null)
            throw new NotFoundException(nameof(Contract), id);

        contract.Status = ContractStatus.Completed;
        contract.ActualEndDate = DateTime.UtcNow;
        contract.PercentageComplete = 100;

        _unitOfWork.Repository<Contract>().Update(contract);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ContractDto>(contract);
    }

    // Financial methods
    public async Task<ContractDto> UpdateCurrentValueAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var contract = await _unitOfWork.Repository<Contract>().GetAsync(
            c => c.Id == id,
            includeProperties: "ChangeOrders",
            tracked: true,
            cancellationToken: cancellationToken);

        if (contract == null)
            throw new NotFoundException(nameof(Contract), id);

        contract.UpdateCurrentValue();

        _unitOfWork.Repository<Contract>().Update(contract);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ContractDto>(contract);
    }

    public async Task<ContractFinancialSummary> GetFinancialSummaryAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var contract = await _unitOfWork.Repository<Contract>().GetAsync(
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
        var contract = await _unitOfWork.Repository<Contract>().GetAsync(
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
        var contract = await _unitOfWork.Repository<Contract>().GetAsync(
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

    // Additional interface implementations
    public async Task<IEnumerable<ContractDto>> GetByProjectAsync(Guid projectId)
    {
        var contracts = await _unitOfWork.Repository<Contract>().GetAllAsync(
            filter: c => c.ProjectId == projectId,
            includeProperties: "Contractor",
            cancellationToken: default);

        return _mapper.Map<IEnumerable<ContractDto>>(contracts);
    }

    public async Task<IEnumerable<ContractDto>> GetByContractorAsync(Guid contractorId)
    {
        var contracts = await _unitOfWork.Repository<Contract>().GetAllAsync(
            filter: c => c.ContractorId == contractorId,
            includeProperties: "Project",
            cancellationToken: default);

        return _mapper.Map<IEnumerable<ContractDto>>(contracts);
    }

    public async Task<ContractDto?> GetByContractNumberAsync(string contractNumber)
    {
        var contract = await _unitOfWork.Repository<Contract>().GetAsync(
            c => c.ContractNumber == contractNumber,
            includeProperties: "Project,Contractor",
            cancellationToken: default);

        return contract != null ? _mapper.Map<ContractDto>(contract) : null;
    }

    public async Task<IEnumerable<ContractDto>> SearchAsync(ContractFilterDto filter)
    {
        Expression<Func<Contract, bool>>? predicate = c => true;

        if (!string.IsNullOrEmpty(filter.SearchTerm))
            predicate = c => c.ContractNumber.Contains(filter.SearchTerm) || 
                           c.Title.Contains(filter.SearchTerm) ||
                           c.Description.Contains(filter.SearchTerm);

        if (filter.ProjectId.HasValue)
            predicate = CombineExpressions(predicate, c => c.ProjectId == filter.ProjectId.Value);

        if (filter.ContractorId.HasValue)
            predicate = CombineExpressions(predicate, c => c.ContractorId == filter.ContractorId.Value);

        if (filter.Status.HasValue)
            predicate = CombineExpressions(predicate, c => c.Status == filter.Status.Value);

        var contracts = await _unitOfWork.Repository<Contract>().GetAllAsync(
            filter: predicate,
            includeProperties: "Project,Contractor",
            cancellationToken: default);

        return _mapper.Map<IEnumerable<ContractDto>>(contracts);
    }

    public async Task<ContractSummaryDto> GetSummaryAsync(Guid? projectId = null, Guid? contractorId = null)
    {
        Expression<Func<Contract, bool>>? filter = null;
        
        if (projectId.HasValue && contractorId.HasValue)
            filter = c => c.ProjectId == projectId.Value && c.ContractorId == contractorId.Value;
        else if (projectId.HasValue)
            filter = c => c.ProjectId == projectId.Value;
        else if (contractorId.HasValue)
            filter = c => c.ContractorId == contractorId.Value;

        var contracts = await _unitOfWork.Repository<Contract>().GetAllAsync(
            filter: filter,
            cancellationToken: default);

        var contractsList = contracts.ToList();

        return new ContractSummaryDto
        {
            TotalContracts = contractsList.Count,
            ActiveContracts = contractsList.Count(c => c.Status == ContractStatus.Active),
            TotalValue = contractsList.Sum(c => c.CurrentValue),
            TotalPaid = contractsList.Sum(c => c.AmountPaid),
            TotalOutstanding = contractsList.Sum(c => c.AmountInvoiced - c.AmountPaid)
        };
    }

    public async Task<ContractDto?> ApproveAsync(Guid contractId, string approvedBy, string comments)
    {
        var approveDto = new ApproveContractDto
        {
            ApprovedBy = approvedBy,
            Comments = comments
        };
        return await ApproveAsync(contractId, approveDto);
    }

    public async Task<ContractDto?> TerminateAsync(Guid contractId, string reason)
    {
        var terminateDto = new TerminateContractDto
        {
            Reason = reason
        };
        return await TerminateAsync(contractId, terminateDto);
    }

    public async Task<ContractDto?> CloseAsync(Guid contractId)
    {
        return await CompleteAsync(contractId);
    }

    public async Task<ContractDto?> UpdateStatusAsync(Guid contractId, ContractStatus status)
    {
        var contract = await _unitOfWork.Repository<Contract>().GetAsync(
            c => c.Id == contractId,
            tracked: true,
            cancellationToken: default);

        if (contract == null)
            return null;

        contract.Status = status;
        _unitOfWork.Repository<Contract>().Update(contract);
        await _unitOfWork.SaveChangesAsync(default);

        return _mapper.Map<ContractDto>(contract);
    }

    public async Task<ContractDto?> UpdateFinancialsAsync(Guid contractId)
    {
        return await UpdateCurrentValueAsync(contractId);
    }

    public async Task<decimal> CalculateRetentionAsync(Guid contractId)
    {
        var contract = await _unitOfWork.Repository<Contract>().GetAsync(
            c => c.Id == contractId,
            includeProperties: "Valuations",
            cancellationToken: default);

        if (contract == null)
            return 0;

        return contract.RetentionAmount;
    }

    public async Task<decimal> GetOutstandingAmountAsync(Guid contractId)
    {
        var contract = await _unitOfWork.Repository<Contract>().GetAsync(
            c => c.Id == contractId,
            cancellationToken: default);

        if (contract == null)
            return 0;

        return contract.AmountInvoiced - contract.AmountPaid;
    }

    public async Task<ContractDto?> ApplyChangeOrderAsync(Guid contractId, Guid changeOrderId)
    {
        var contract = await _unitOfWork.Repository<Contract>().GetAsync(
            c => c.Id == contractId,
            tracked: true,
            cancellationToken: default);

        if (contract == null)
            return null;

        // Update contract value based on approved change order
        contract.UpdateCurrentValue();

        _unitOfWork.Repository<Contract>().Update(contract);
        await _unitOfWork.SaveChangesAsync(default);

        return _mapper.Map<ContractDto>(contract);
    }

    public async Task<IEnumerable<ChangeOrderDto>> GetChangeOrdersAsync(Guid contractId)
    {
        var changeOrders = await _unitOfWork.Repository<ContractChangeOrder>().GetAllAsync(
            filter: co => co.ContractId == contractId,
            orderBy: q => q.OrderByDescending(co => co.SubmissionDate),
            cancellationToken: default);

        return _mapper.Map<IEnumerable<ChangeOrderDto>>(changeOrders);
    }

    public async Task<IEnumerable<ContractMilestoneDto>> GetMilestonesAsync(Guid contractId)
    {
        var milestones = await _unitOfWork.Repository<ContractMilestone>().GetAllAsync(
            filter: m => m.ContractId == contractId,
            orderBy: q => q.OrderBy(m => m.PlannedDate),
            cancellationToken: default);

        return _mapper.Map<IEnumerable<ContractMilestoneDto>>(milestones);
    }

    public async Task<ContractMilestoneDto?> GetNextMilestoneAsync(Guid contractId)
    {
        var milestone = await _unitOfWork.Repository<ContractMilestone>().GetAllAsync(
            m => m.ContractId == contractId && m.Status != MilestoneStatus.Completed,
            orderBy: q => q.OrderBy(m => m.PlannedDate),
            cancellationToken: default);

        return milestone != null ? _mapper.Map<ContractMilestoneDto>(milestone) : null;
    }

    public async Task<IEnumerable<ContractMilestoneDto>> GetOverdueMilestonesAsync(Guid contractId)
    {
        var currentDate = DateTime.UtcNow;
        var milestones = await _unitOfWork.Repository<ContractMilestone>().GetAllAsync(
            filter: m => m.ContractId == contractId && 
                        m.PlannedDate < currentDate && 
                        m.Status != MilestoneStatus.Completed,
            orderBy: q => q.OrderBy(m => m.PlannedDate),
            cancellationToken: default);

        return _mapper.Map<IEnumerable<ContractMilestoneDto>>(milestones);
    }

    public async Task<IEnumerable<ValuationDto>> GetValuationsAsync(Guid contractId)
    {
        var valuations = await _unitOfWork.Repository<Valuation>().GetAllAsync(
            filter: v => v.ContractId == contractId,
            orderBy: q => q.OrderByDescending(v => v.ValuationPeriod),
            cancellationToken: default);

        return _mapper.Map<IEnumerable<ValuationDto>>(valuations);
    }

    public async Task<ValuationDto?> GetLatestValuationAsync(Guid contractId)
    {
        var valuation = await _unitOfWork.Repository<Valuation>().GetAllAsync(
            filter: v => v.ContractId == contractId,
            orderBy: q => q.OrderByDescending(v => v.ValuationPeriod),
            cancellationToken: default);

        var latest = valuation.FirstOrDefault();
        return latest != null ? _mapper.Map<ValuationDto>(latest) : null;
    }

    public async Task<ValuationSummaryDto> GetValuationSummaryAsync(Guid contractId)
    {
        var valuations = await _unitOfWork.Repository<Valuation>().GetAllAsync(
            filter: v => v.ContractId == contractId,
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

    public async Task<IEnumerable<ClaimDto>> GetClaimsAsync(Guid contractId)
    {
        var claims = await _unitOfWork.Repository<Claim>().GetAllAsync(
            filter: c => c.ContractId == contractId,
            orderBy: q => q.OrderByDescending(c => c.SubmissionDate),
            cancellationToken: default);

        return _mapper.Map<IEnumerable<ClaimDto>>(claims);
    }

    public async Task<int> GetOpenClaimsCountAsync(Guid contractId)
    {
        var claims = await _unitOfWork.Repository<Claim>().GetAllAsync(
            filter: c => c.ContractId == contractId && c.Status != ClaimStatus.Closed,
            cancellationToken: default);

        return claims.Count();
    }

    public async Task<decimal> GetTotalClaimsValueAsync(Guid contractId)
    {
        var claims = await _unitOfWork.Repository<Claim>().GetAllAsync(
            filter: c => c.ContractId == contractId,
            cancellationToken: default);

        return claims.Sum(c => c.ClaimedAmount);
    }

    public async Task<ContractDto?> UpdateRiskLevelAsync(Guid contractId, ContractRiskLevel riskLevel)
    {
        var contract = await _unitOfWork.Repository<Contract>().GetAsync(
            c => c.Id == contractId,
            tracked: true,
            cancellationToken: default);

        if (contract == null)
            return null;

        contract.RiskLevel = riskLevel;
        _unitOfWork.Repository<Contract>().Update(contract);
        await _unitOfWork.SaveChangesAsync(default);

        return _mapper.Map<ContractDto>(contract);
    }

    public async Task<IEnumerable<ContractDto>> GetHighRiskContractsAsync()
    {
        var contracts = await _unitOfWork.Repository<Contract>().GetAllAsync(
            filter: c => c.RiskLevel == ContractRiskLevel.High || c.RiskLevel == ContractRiskLevel.Critical,
            includeProperties: "Project,Contractor",
            cancellationToken: default);

        return _mapper.Map<IEnumerable<ContractDto>>(contracts);
    }

    public async Task<bool> AttachDocumentAsync(Guid contractId, Guid documentId, string documentType)
    {
        var contract = await _unitOfWork.Repository<Contract>().GetAsync(
            c => c.Id == contractId,
            cancellationToken: default);

        if (contract == null)
            return false;

        var document = new ContractDocument
        {
            ContractId = contractId,
            DocumentId = documentId,
            DocumentType = documentType,
            AttachedDate = DateTime.UtcNow,
            AttachedBy = await GetCurrentUserIdAsync()
        };

        await _unitOfWork.Repository<ContractDocument>().AddAsync(document, default);
        await _unitOfWork.SaveChangesAsync(default);

        return true;
    }

    public async Task<bool> RemoveDocumentAsync(Guid contractId, Guid documentId)
    {
        var document = await _unitOfWork.Repository<ContractDocument>().GetAsync(
            d => d.ContractId == contractId && d.DocumentId == documentId,
            tracked: true,
            cancellationToken: default);

        if (document == null)
            return false;

        _unitOfWork.Repository<ContractDocument>().Remove(document);
        await _unitOfWork.SaveChangesAsync(default);

        return true;
    }

    public async Task<IEnumerable<ContractDocument>> GetDocumentsAsync(Guid contractId)
    {
        var documents = await _unitOfWork.Repository<ContractDocument>().GetAllAsync(
            filter: d => d.ContractId == contractId,
            cancellationToken: default);

        return documents;
    }

    public async Task<IEnumerable<ContractDto>> GetExpiringContractsAsync(int daysAhead = 30)
    {
        var futureDate = DateTime.UtcNow.AddDays(daysAhead);
        var contracts = await _unitOfWork.Repository<Contract>().GetAllAsync(
            filter: c => c.CurrentEndDate <= futureDate && c.Status == ContractStatus.Active,
            includeProperties: "Project,Contractor",
            cancellationToken: default);

        return _mapper.Map<IEnumerable<ContractDto>>(contracts);
    }

    public async Task<IEnumerable<ContractDto>> GetContractsWithExpiredBondsAsync()
    {
        var contracts = await _unitOfWork.Repository<Contract>().GetAllAsync(
            filter: c => c.HasExpiredBonds() && c.Status == ContractStatus.Active,
            includeProperties: "Project,Contractor",
            cancellationToken: default);

        return _mapper.Map<IEnumerable<ContractDto>>(contracts);
    }

    public async Task<IEnumerable<ContractDto>> GetDelayedContractsAsync()
    {
        var contracts = await _unitOfWork.Repository<Contract>().GetAllAsync(
            filter: c => c.IsDelayed() && c.Status == ContractStatus.Active,
            includeProperties: "Project,Contractor",
            cancellationToken: default);

        return _mapper.Map<IEnumerable<ContractDto>>(contracts);
    }

    private Expression<Func<T, bool>> CombineExpressions<T>(
        Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
    {
        var parameter = Expression.Parameter(typeof(T));
        var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
        var left = leftVisitor.Visit(expr1.Body);
        var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
        var right = rightVisitor.Visit(expr2.Body);
        
        return Expression.Lambda<Func<T, bool>>(
            Expression.AndAlso(left, right), parameter);
    }

    private class ReplaceExpressionVisitor : ExpressionVisitor
    {
        private readonly Expression _oldValue;
        private readonly Expression _newValue;

        public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
        {
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public override Expression Visit(Expression node)
        {
            if (node == _oldValue)
                return _newValue;
            return base.Visit(node);
        }
    }
}