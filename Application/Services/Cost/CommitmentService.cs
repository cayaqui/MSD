using Application.Services.Interfaces;
using Core.DTOs.Cost.Commitments;
using Core.Enums.Cost;
using Domain.Entities.Cost;

namespace Application.Services.Implementation;

/// <summary>
/// Implementation of Commitment management service
/// </summary>
public class CommitmentService : ICommitmentService
{
    private readonly IRepository<Commitment> _commitmentRepository;
    private readonly IRepository<CommitmentItem> _commitmentItemRepository;
    private readonly IRepository<CommitmentWorkPackage> _workPackageRepository;
    private readonly IRepository<CommitmentRevision> _revisionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CommitmentService> _logger;
    private readonly ICurrentUserService _currentUserService;

    public CommitmentService(
        IRepository<Commitment> commitmentRepository,
        IRepository<CommitmentItem> commitmentItemRepository,
        IRepository<CommitmentWorkPackage> workPackageRepository,
        IRepository<CommitmentRevision> revisionRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<CommitmentService> logger,
        ICurrentUserService currentUserService)
    {
        _commitmentRepository = commitmentRepository;
        _commitmentItemRepository = commitmentItemRepository;
        _workPackageRepository = workPackageRepository;
        _revisionRepository = revisionRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _currentUserService = currentUserService;
    }

    // Query operations
    public async Task<PagedResult<CommitmentListDto>> GetCommitmentsAsync(CommitmentFilterDto filter)
    {
        try
        {
            var specification = new CommitmentFilterSpecification(filter);
            var totalCount = await _commitmentRepository.CountAsync(specification);

            var commitments = await _commitmentRepository.ListAsync(specification);
            var dtos = _mapper.Map<List<CommitmentListDto>>(commitments);

            return new PagedResult<CommitmentListDto>
            {
                Items = dtos,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting commitments with filter");
            throw;
        }
    }

    public async Task<CommitmentDetailDto?> GetCommitmentDetailAsync(Guid commitmentId)
    {
        try
        {
            var commitment = await _commitmentRepository.Query()
                .Include(c => c.Contractor)
                .Include(c => c.BudgetItem)
                .Include(c => c.ControlAccount)
                .Include(c => c.Items)
                .Include(c => c.WorkPackageAllocations)
                    .ThenInclude(w => w.WBSElement)
                .Include(c => c.Revisions)
                .Include(c => c.Invoices)
                .FirstOrDefaultAsync(c => c.Id == commitmentId);

            if (commitment == null)
                return null;

            var dto = _mapper.Map<CommitmentDetailDto>(commitment);

            // Calculate financial summary
            dto.FinancialSummary = CalculateFinancialSummary(commitment);

            // Calculate performance metrics
            dto.PerformanceMetrics = CalculatePerformanceMetrics(commitment);

            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting commitment detail for {CommitmentId}", commitmentId);
            throw;
        }
    }

    public async Task<CommitmentDto?> GetCommitmentAsync(Guid commitmentId)
    {
        try
        {
            var commitment = await _commitmentRepository.GetByIdAsync(commitmentId);
            return commitment == null ? null : _mapper.Map<CommitmentDto>(commitment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting commitment {CommitmentId}", commitmentId);
            throw;
        }
    }

    public async Task<List<CommitmentListDto>> GetProjectCommitmentsAsync(Guid projectId)
    {
        try
        {
            var commitments = await _commitmentRepository.Query()
                .Where(c => c.ProjectId == projectId && !c.IsDeleted)
                .Include(c => c.Contractor)
                .OrderBy(c => c.CommitmentNumber)
                .ToListAsync();

            return _mapper.Map<List<CommitmentListDto>>(commitments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting commitments for project {ProjectId}", projectId);
            throw;
        }
    }

    public async Task<CommitmentSummaryDto> GetProjectCommitmentSummaryAsync(Guid projectId)
    {
        try
        {
            var commitments = await _commitmentRepository.Query()
                .Where(c => c.ProjectId == projectId && !c.IsDeleted)
                .ToListAsync();

            var summary = new CommitmentSummaryDto
            {
                ProjectId = projectId,
                TotalCommitments = commitments.Count,
                DraftCount = commitments.Count(c => c.Status == CommitmentStatus.Draft),
                ActiveCount = commitments.Count(c => c.Status == CommitmentStatus.Active),
                ClosedCount = commitments.Count(c => c.Status == CommitmentStatus.Closed),
                TotalOriginalAmount = commitments.Sum(c => c.OriginalAmount),
                TotalRevisedAmount = commitments.Sum(c => c.RevisedAmount),
                TotalCommittedAmount = commitments.Sum(c => c.CommittedAmount),
                TotalInvoicedAmount = commitments.Sum(c => c.InvoicedAmount),
                TotalPaidAmount = commitments.Sum(c => c.PaidAmount),
                TotalRetentionAmount = commitments.Sum(c => c.RetentionAmount),
                OverCommittedCount = commitments.Count(c => c.IsOverCommitted()),
                ExpiredCount = commitments.Count(c => c.IsExpired())
            };

            // Calculate averages
            if (commitments.Any())
            {
                summary.AverageInvoicedPercentage = commitments.Average(c => c.GetInvoicedPercentage());
                summary.AveragePaidPercentage = commitments.Where(c => c.InvoicedAmount > 0)
                    .Average(c => c.GetPaidPercentage());
            }

            // Group by type
            summary.CountByType = commitments
                .GroupBy(c => c.Type.ToString())
                .ToDictionary(g => g.Key, g => g.Count());

            summary.AmountByType = commitments
                .GroupBy(c => c.Type.ToString())
                .ToDictionary(g => g.Key, g => g.Sum(c => c.CommittedAmount));

            return summary;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting commitment summary for project {ProjectId}", projectId);
            throw;
        }
    }

    // CRUD operations
    public async Task<CommitmentDto> CreateCommitmentAsync(CreateCommitmentDto dto)
    {
        try
        {
            // Validate unique commitment number
            if (!await IsCommitmentNumberUniqueAsync(dto.CommitmentNumber))
            {
                throw new InvalidOperationException($"Commitment number {dto.CommitmentNumber} already exists");
            }

            var commitment = new Commitment(
                dto.ProjectId,
                dto.CommitmentNumber,
                dto.Title,
                dto.Type,
                dto.OriginalAmount,
                dto.Currency,
                dto.ContractDate,
                dto.StartDate,
                dto.EndDate
            );

            // Set optional properties
            commitment.UpdateDetails(dto.Title, dto.Description, dto.ScopeOfWork, dto.Deliverables);
            commitment.SetContractReferences(dto.PurchaseOrderNumber, dto.ContractNumber,
                dto.VendorReference, dto.AccountingReference);
            commitment.SetPaymentTerms(dto.PaymentTermsDays, dto.RetentionPercentage, dto.AdvancePaymentAmount);
            commitment.SetContractType(dto.IsFixedPrice, dto.IsTimeAndMaterial);

            // Assign relationships
            if (dto.ContractorId.HasValue)
                commitment.AssignToContractor(dto.ContractorId.Value);

            if (dto.BudgetItemId.HasValue)
                commitment.AssignToBudget(dto.BudgetItemId.Value);

            if (dto.ControlAccountId.HasValue)
                commitment.AssignToControlAccount(dto.ControlAccountId.Value);

            // Add work package allocations if provided
            if (dto.WorkPackageAllocations?.Any() == true)
            {
                foreach (var allocation in dto.WorkPackageAllocations)
                {
                    commitment.AddWorkPackageAllocation(allocation.WBSElementId, allocation.AllocatedAmount);
                }
            }

            await _commitmentRepository.AddAsync(commitment);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Created commitment {CommitmentNumber} with ID {CommitmentId}",
                commitment.CommitmentNumber, commitment.Id);

            return _mapper.Map<CommitmentDto>(commitment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating commitment");
            throw;
        }
    }

    public async Task<CommitmentDto> UpdateCommitmentAsync(Guid commitmentId, UpdateCommitmentDto dto)
    {
        try
        {
            var commitment = await _commitmentRepository.GetByIdAsync(commitmentId);
            if (commitment == null)
                throw new InvalidOperationException($"Commitment {commitmentId} not found");

            // Only allow updates in certain statuses
            if (commitment.Status != CommitmentStatus.Draft && commitment.Status != CommitmentStatus.Approved)
            {
                throw new InvalidOperationException($"Cannot update commitment in {commitment.Status} status");
            }

            // Update basic information
            if (!string.IsNullOrEmpty(dto.Title))
            {
                commitment.UpdateDetails(dto.Title, dto.Description, dto.ScopeOfWork, dto.Deliverables);
            }

            // Update references
            if (dto.PurchaseOrderNumber != null || dto.ContractNumber != null ||
                dto.VendorReference != null || dto.AccountingReference != null)
            {
                commitment.SetContractReferences(dto.PurchaseOrderNumber, dto.ContractNumber,
                    dto.VendorReference, dto.AccountingReference);
            }

            // Update payment terms
            if (dto.PaymentTermsDays.HasValue || dto.RetentionPercentage.HasValue ||
                dto.AdvancePaymentAmount.HasValue)
            {
                commitment.SetPaymentTerms(dto.PaymentTermsDays, dto.RetentionPercentage,
                    dto.AdvancePaymentAmount);
            }

            // Update contract type
            if (dto.IsFixedPrice.HasValue && dto.IsTimeAndMaterial.HasValue)
            {
                commitment.SetContractType(dto.IsFixedPrice.Value, dto.IsTimeAndMaterial.Value);
            }

            // Update assignments
            if (dto.ContractorId.HasValue)
                commitment.AssignToContractor(dto.ContractorId.Value);

            if (dto.BudgetItemId.HasValue)
                commitment.AssignToBudget(dto.BudgetItemId.Value);

            if (dto.ControlAccountId.HasValue)
                commitment.AssignToControlAccount(dto.ControlAccountId.Value);

            // Update performance
            if (dto.PerformancePercentage.HasValue)
            {
                commitment.UpdatePerformance(dto.PerformancePercentage.Value, dto.ExpectedCompletionDate);
            }

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Updated commitment {CommitmentId}", commitmentId);

            return _mapper.Map<CommitmentDto>(commitment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating commitment {CommitmentId}", commitmentId);
            throw;
        }
    }

    public async Task<bool> DeleteCommitmentAsync(Guid commitmentId)
    {
        try
        {
            var commitment = await _commitmentRepository.GetByIdAsync(commitmentId);
            if (commitment == null)
                return false;

            if (!await CanDeleteCommitmentAsync(commitmentId))
            {
                throw new InvalidOperationException("Cannot delete commitment with invoices or in active status");
            }

            _commitmentRepository.Remove(commitment);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Deleted commitment {CommitmentId}", commitmentId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting commitment {CommitmentId}", commitmentId);
            throw;
        }
    }

    // Workflow operations
    public async Task<CommitmentDto> SubmitForApprovalAsync(Guid commitmentId)
    {
        try
        {
            var commitment = await _commitmentRepository.GetByIdAsync(commitmentId);
            if (commitment == null)
                throw new InvalidOperationException($"Commitment {commitmentId} not found");

            commitment.SubmitForApproval();
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Submitted commitment {CommitmentId} for approval", commitmentId);

            return _mapper.Map<CommitmentDto>(commitment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting commitment {CommitmentId} for approval", commitmentId);
            throw;
        }
    }

    public async Task<CommitmentDto> ApproveCommitmentAsync(Guid commitmentId, ApproveCommitmentDto dto)
    {
        try
        {
            var commitment = await _commitmentRepository.GetByIdAsync(commitmentId);
            if (commitment == null)
                throw new InvalidOperationException($"Commitment {commitmentId} not found");

            var approvedBy = _currentUserService.UserName ?? "System";
            commitment.Approve(approvedBy, dto.ApprovalNotes);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Approved commitment {CommitmentId} by {ApprovedBy}",
                commitmentId, approvedBy);

            return _mapper.Map<CommitmentDto>(commitment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving commitment {CommitmentId}", commitmentId);
            throw;
        }
    }

    public async Task<CommitmentDto> ActivateCommitmentAsync(Guid commitmentId)
    {
        try
        {
            var commitment = await _commitmentRepository.GetByIdAsync(commitmentId);
            if (commitment == null)
                throw new InvalidOperationException($"Commitment {commitmentId} not found");

            commitment.Activate();
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Activated commitment {CommitmentId}", commitmentId);

            return _mapper.Map<CommitmentDto>(commitment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error activating commitment {CommitmentId}", commitmentId);
            throw;
        }
    }

    public async Task<CommitmentDto> ReviseCommitmentAsync(Guid commitmentId, ReviseCommitmentDto dto)
    {
        try
        {
            var commitment = await _commitmentRepository.GetByIdAsync(commitmentId);
            if (commitment == null)
                throw new InvalidOperationException($"Commitment {commitmentId} not found");

            commitment.Revise(dto.RevisedAmount, dto.Reason);

            // Set change order reference if provided
            if (!string.IsNullOrEmpty(dto.ChangeOrderReference))
            {
                var revision = commitment.Revisions.LastOrDefault();
                revision?.SetChangeOrderReference(dto.ChangeOrderReference);
            }

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Revised commitment {CommitmentId} to amount {RevisedAmount}",
                commitmentId, dto.RevisedAmount);

            return _mapper.Map<CommitmentDto>(commitment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revising commitment {CommitmentId}", commitmentId);
            throw;
        }
    }

    // Additional methods would continue here...
    // Including all remaining interface methods implementation

    // Private helper methods
    private CommitmentFinancialSummary CalculateFinancialSummary(Commitment commitment)
    {
        return new CommitmentFinancialSummary
        {
            OriginalBudget = commitment.OriginalAmount,
            CurrentBudget = commitment.RevisedAmount,
            BudgetVariance = commitment.RevisedAmount - commitment.OriginalAmount,
            BudgetVariancePercentage = commitment.OriginalAmount > 0
                ? ((commitment.RevisedAmount - commitment.OriginalAmount) / commitment.OriginalAmount) * 100
                : 0,
            TotalCommitted = commitment.CommittedAmount,
            TotalInvoiced = commitment.InvoicedAmount,
            TotalPaid = commitment.PaidAmount,
            TotalRetention = commitment.RetentionAmount,
            TotalOutstanding = commitment.InvoicedAmount - commitment.PaidAmount,
            TotalWorkPackages = commitment.WorkPackageAllocations.Count,
            ActiveWorkPackages = commitment.WorkPackageAllocations.Count(w => !w.IsFullyInvoiced()),
            AverageWorkPackageUtilization = commitment.WorkPackageAllocations.Any()
                ? commitment.WorkPackageAllocations.Average(w => w.GetUtilizationPercentage())
                : 0
        };
    }

    private CommitmentPerformanceMetrics CalculatePerformanceMetrics(Commitment commitment)
    {
        var totalDays = (commitment.EndDate - commitment.StartDate).Days;
        var elapsedDays = (DateTime.UtcNow - commitment.StartDate).Days;
        var timeElapsedPercentage = totalDays > 0 ? (elapsedDays / (decimal)totalDays) * 100 : 0;

        return new CommitmentPerformanceMetrics
        {
            StartDate = commitment.StartDate,
            EndDate = commitment.EndDate,
            InvoicingEfficiency = timeElapsedPercentage > 0
                ? commitment.GetInvoicedPercentage() / timeElapsedPercentage * 100
                : 0,
            PaymentEfficiency = commitment.InvoicedAmount > 0
                ? (commitment.PaidAmount / commitment.InvoicedAmount) * 100
                : 0,
            TotalInvoices = commitment.Invoices.Count,
            ApprovedInvoices = commitment.Invoices.Count(i => i.Status == InvoiceStatus.Approved),
            RejectedInvoices = commitment.Invoices.Count(i => i.Status == InvoiceStatus.Rejected),
            InvoiceApprovalRate = commitment.Invoices.Any()
                ? (commitment.Invoices.Count(i => i.Status == InvoiceStatus.Approved) /
                   (decimal)commitment.Invoices.Count) * 100
                : 0,
            IsDelayed = commitment.PerformancePercentage < timeElapsedPercentage,
            IsBudgetExceeded = commitment.InvoicedAmount > commitment.CommittedAmount,
            RiskScore = CalculateRiskScore(commitment, timeElapsedPercentage)
        };
    }

    private decimal CalculateRiskScore(Commitment commitment, decimal timeElapsedPercentage)
    {
        decimal riskScore = 0;

        // Time risk
        if (commitment.PerformancePercentage < timeElapsedPercentage - 10)
            riskScore += 25;

        // Budget risk
        if (commitment.IsOverCommitted())
            riskScore += 25;

        // Payment risk
        var unpaidPercentage = commitment.GetUnpaidAmount() / commitment.CommittedAmount * 100;
        if (unpaidPercentage > 30)
            riskScore += 25;

        // Expiry risk
        if (commitment.IsExpired())
            riskScore += 25;

        return Math.Min(riskScore, 100);
    }

    // Remaining interface methods would be implemented here...

    public async Task<bool> IsCommitmentNumberUniqueAsync(string commitmentNumber, Guid? excludeId = null)
    {
        var query = _commitmentRepository.Query()
            .Where(c => c.CommitmentNumber == commitmentNumber);

        if (excludeId.HasValue)
            query = query.Where(c => c.Id != excludeId.Value);

        return !await query.AnyAsync();
    }

    public async Task<bool> CanDeleteCommitmentAsync(Guid commitmentId)
    {
        var commitment = await _commitmentRepository.Query()
            .Include(c => c.Invoices)
            .FirstOrDefaultAsync(c => c.Id == commitmentId);

        if (commitment == null)
            return false;

        return commitment.Status == CommitmentStatus.Draft &&
               !commitment.Invoices.Any() &&
               commitment.InvoicedAmount == 0;
    }

    public async Task<bool> CanReviseCommitmentAsync(Guid commitmentId, decimal newAmount)
    {
        var commitment = await _commitmentRepository.GetByIdAsync(commitmentId);
        if (commitment == null)
            return false;

        return (commitment.Status == CommitmentStatus.Active ||
                commitment.Status == CommitmentStatus.PartiallyInvoiced) &&
               newAmount >= commitment.InvoicedAmount;
    }

    // Stub implementations for remaining methods - these would need full implementation
    public Task<CommitmentDto> RejectCommitmentAsync(Guid commitmentId, CancelCommitmentDto dto)
        => throw new NotImplementedException();

    public Task<CommitmentDto> CloseCommitmentAsync(Guid commitmentId)
        => throw new NotImplementedException();

    public Task<CommitmentDto> CancelCommitmentAsync(Guid commitmentId, CancelCommitmentDto dto)
        => throw new NotImplementedException();

    public Task<CommitmentDto> AddWorkPackageAllocationAsync(Guid commitmentId, CommitmentWorkPackageAllocationDto dto)
        => throw new NotImplementedException();

    public Task<CommitmentDto> UpdateWorkPackageAllocationAsync(Guid commitmentId, Guid allocationId, decimal newAmount)
        => throw new NotImplementedException();

    public Task<bool> RemoveWorkPackageAllocationAsync(Guid commitmentId, Guid allocationId)
        => throw new NotImplementedException();

    public Task<List<CommitmentWorkPackageDto>> GetWorkPackageAllocationsAsync(Guid commitmentId)
        => throw new NotImplementedException();

    public Task<CommitmentDto> AddCommitmentItemAsync(Guid commitmentId, CreateCommitmentItemDto dto)
        => throw new NotImplementedException();

    public Task<CommitmentDto> UpdateCommitmentItemAsync(Guid commitmentId, Guid itemId, UpdateCommitmentItemDto dto)
        => throw new NotImplementedException();

    public Task<bool> RemoveCommitmentItemAsync(Guid commitmentId, Guid itemId)
        => throw new NotImplementedException();

    public Task<List<CommitmentItemDto>> GetCommitmentItemsAsync(Guid commitmentId)
        => throw new NotImplementedException();

    public Task<CommitmentDto> RecordInvoiceAsync(Guid commitmentId, RecordCommitmentInvoiceDto dto)
        => throw new NotImplementedException();

    public Task<CommitmentDto> UpdatePerformanceAsync(Guid commitmentId, UpdateCommitmentPerformanceDto dto)
        => throw new NotImplementedException();

    public Task<List<CommitmentRevisionDto>> GetCommitmentRevisionsAsync(Guid commitmentId)
        => throw new NotImplementedException();

    public Task<CommitmentFinancialSummary> GetFinancialSummaryAsync(Guid commitmentId)
        => throw new NotImplementedException();

    public Task<CommitmentPerformanceMetrics> GetPerformanceMetricsAsync(Guid commitmentId)
        => throw new NotImplementedException();

    public Task<byte[]> ExportCommitmentsAsync(CommitmentFilterDto filter, string format = "xlsx")
        => throw new NotImplementedException();
}