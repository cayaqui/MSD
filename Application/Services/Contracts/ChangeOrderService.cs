using Application.Interfaces.Contracts;
using Application.Services.Base;
using Core.DTOs.Contracts.ChangeOrders;
using Core.DTOs.Contracts.ContractMilestones;
using Core.Enums.Change;
using Core.Enums.Cost;
using Domain.Entities.Change.Core;
using Domain.Entities.Contracts.Core;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Contracts;

public class ChangeOrderService : BaseService<ChangeOrder, ChangeOrderDto, CreateChangeOrderDto, UpdateChangeOrderDto>, IChangeOrderService
{
    public ChangeOrderService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<ChangeOrderService> logger)
        : base(unitOfWork, mapper, logger)
    {
    }

    // Change Order queries
    public async Task<IEnumerable<ChangeOrderDto>> GetByContractAsync(Guid contractId)
    {
        var changeOrders = await _unitOfWork.Repository<ChangeOrder>()
            .GetAllAsync(
                filter: co => co.ContractId == contractId,
                include: query => query.Include(co => co.Initiator),
                orderBy: query => query.OrderByDescending(co => co.CreatedAt));

        return _mapper.Map<IEnumerable<ChangeOrderDto>>(changeOrders);
    }

    public async Task<ChangeOrderDto?> GetByChangeOrderNumberAsync(string changeOrderNumber)
    {
        var changeOrder = await _unitOfWork.Repository<ChangeOrder>()
            .GetAsync(
                filter: co => co.ChangeOrderNumber == changeOrderNumber,
                include: query => query.Include(co => co.Contract).Include(co => co.Initiator));

        return changeOrder != null ? _mapper.Map<ChangeOrderDto>(changeOrder) : null;
    }

    public async Task<IEnumerable<ChangeOrderDto>> SearchAsync(ChangeOrderFilterDto filter)
    {
        Expression<Func<ChangeOrder, bool>>? searchFilter = null;

        if (!string.IsNullOrEmpty(filter.SearchTerm) || filter.ContractId.HasValue || filter.Status.HasValue ||
            filter.Type.HasValue || filter.Priority.HasValue || filter.MinCostImpact.HasValue || 
            filter.MaxCostImpact.HasValue || filter.DateFrom.HasValue || filter.DateTo.HasValue)
        {
            searchFilter = co =>
                (string.IsNullOrEmpty(filter.SearchTerm) ||
                 co.ChangeOrderNumber.Contains(filter.SearchTerm) ||
                 co.Title.Contains(filter.SearchTerm) ||
                 co.Description.Contains(filter.SearchTerm)) &&
                (!filter.ContractId.HasValue || co.ContractId == filter.ContractId.Value) &&
                (!filter.Status.HasValue || co.Status == filter.Status.Value) &&
                (!filter.Type.HasValue || co.Type == filter.Type.Value) &&
                (!filter.Priority.HasValue || co.Priority == filter.Priority.Value) &&
                (!filter.MinCostImpact.HasValue || co.CostImpact >= filter.MinCostImpact.Value) &&
                (!filter.MaxCostImpact.HasValue || co.CostImpact <= filter.MaxCostImpact.Value) &&
                (!filter.DateFrom.HasValue || co.CreatedAt >= filter.DateFrom.Value) &&
                (!filter.DateTo.HasValue || co.CreatedAt <= filter.DateTo.Value);
        }

        var changeOrders = await _unitOfWork.Repository<ChangeOrder>()
            .GetAllAsync(filter: searchFilter);

        return _mapper.Map<IEnumerable<ChangeOrderDto>>(changeOrders);
    }

    public async Task<IEnumerable<ChangeOrderDto>> GetPendingAsync(Guid? contractId = null)
    {
        Expression<Func<ChangeOrder, bool>> pendingFilter = co => 
            co.Status == ChangeOrderStatus.Pending || co.Status == ChangeOrderStatus.UnderReview;

        if (contractId.HasValue)
        {
            pendingFilter = co => 
                (co.Status == ChangeOrderStatus.Pending || co.Status == ChangeOrderStatus.UnderReview) &&
                co.ContractId == contractId.Value;
        }

        var changeOrders = await _unitOfWork.Repository<ChangeOrder>()
            .GetAllAsync(
                filter: pendingFilter,
                include: query => query.Include(co => co.Contract).Include(co => co.Initiator));

        return _mapper.Map<IEnumerable<ChangeOrderDto>>(changeOrders);
    }

    public async Task<IEnumerable<ChangeOrderDto>> GetByStatusAsync(ChangeOrderStatus status, Guid? contractId = null)
    {
        Expression<Func<ChangeOrder, bool>> statusFilter = co => co.Status == status;

        if (contractId.HasValue)
        {
            statusFilter = co => co.Status == status && co.ContractId == contractId.Value;
        }

        var changeOrders = await _unitOfWork.Repository<ChangeOrder>()
            .GetAllAsync(
                filter: statusFilter,
                include: query => query.Include(co => co.Contract).Include(co => co.Initiator));

        return _mapper.Map<IEnumerable<ChangeOrderDto>>(changeOrders);
    }

    // Workflow operations
    public async Task<ChangeOrderDto?> SubmitAsync(Guid changeOrderId, string submittedBy)
    {
        var changeOrder = await _unitOfWork.Repository<ChangeOrder>().GetByIdAsync(changeOrderId);
        if (changeOrder == null) return null;

        changeOrder.Status = ChangeOrderStatus.Submitted;
        changeOrder.SubmittedDate = DateTime.UtcNow;
        changeOrder.SubmittedBy = submittedBy;
        changeOrder.UpdatedAt = DateTime.UtcNow;
        changeOrder.UpdatedBy = submittedBy;

        _unitOfWork.Repository<ChangeOrder>().Update(changeOrder);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ChangeOrderDto>(changeOrder);
    }

    public async Task<ChangeOrderDto?> ReviewAsync(Guid changeOrderId, string reviewedBy, string comments)
    {
        var changeOrder = await _unitOfWork.Repository<ChangeOrder>().GetByIdAsync(changeOrderId);
        if (changeOrder == null) return null;

        changeOrder.Status = ChangeOrderStatus.UnderReview;
        changeOrder.ReviewedDate = DateTime.UtcNow;
        changeOrder.ReviewedBy = reviewedBy;
        changeOrder.ReviewComments = comments;
        changeOrder.UpdatedAt = DateTime.UtcNow;
        changeOrder.UpdatedBy = reviewedBy;

        _unitOfWork.Repository<ChangeOrder>().Update(changeOrder);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ChangeOrderDto>(changeOrder);
    }

    public async Task<ChangeOrderDto?> ApproveAsync(Guid changeOrderId, ApproveChangeOrderDto dto, string approvedBy)
    {
        var changeOrder = await _unitOfWork.Repository<ChangeOrder>().GetByIdAsync(changeOrderId);
        if (changeOrder == null) return null;

        changeOrder.Status = ChangeOrderStatus.Approved;
        changeOrder.ApprovedDate = DateTime.UtcNow;
        changeOrder.ApprovedBy = approvedBy;
        changeOrder.ApprovalComments = dto.Comments;
        changeOrder.ApprovedCostImpact = dto.ApprovedCostImpact ?? changeOrder.CostImpact;
        changeOrder.ApprovedScheduleImpact = dto.ApprovedScheduleImpact ?? changeOrder.ScheduleImpact;
        changeOrder.UpdatedAt = DateTime.UtcNow;
        changeOrder.UpdatedBy = approvedBy;

        _unitOfWork.Repository<ChangeOrder>().Update(changeOrder);
        
        // Update contract values if applicable
        if (changeOrder.ContractId.HasValue)
        {
            var contract = await _unitOfWork.Repository<Contract>().GetByIdAsync(changeOrder.ContractId.Value);
            if (contract != null)
            {
                contract.ChangeOrderCount++;
                contract.ChangeOrderValue += changeOrder.ApprovedCostImpact ?? changeOrder.CostImpact;
                contract.CurrentValue = contract.OriginalValue + contract.ChangeOrderValue;
                
                if (changeOrder.ApprovedScheduleImpact > 0)
                {
                    contract.CurrentEndDate = contract.CurrentEndDate.AddDays(changeOrder.ApprovedScheduleImpact);
                }

                _unitOfWork.Repository<Contract>().Update(contract);
            }
        }

        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ChangeOrderDto>(changeOrder);
    }

    public async Task<ChangeOrderDto?> RejectAsync(Guid changeOrderId, RejectChangeOrderDto dto, string rejectedBy)
    {
        var changeOrder = await _unitOfWork.Repository<ChangeOrder>().GetByIdAsync(changeOrderId);
        if (changeOrder == null) return null;

        changeOrder.Status = ChangeOrderStatus.Rejected;
        changeOrder.RejectedDate = DateTime.UtcNow;
        changeOrder.RejectedBy = rejectedBy;
        changeOrder.RejectionReason = dto.Reason;
        changeOrder.UpdatedAt = DateTime.UtcNow;
        changeOrder.UpdatedBy = rejectedBy;

        _unitOfWork.Repository<ChangeOrder>().Update(changeOrder);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ChangeOrderDto>(changeOrder);
    }

    public async Task<ChangeOrderDto?> WithdrawAsync(Guid changeOrderId, string reason)
    {
        var changeOrder = await _unitOfWork.Repository<ChangeOrder>().GetByIdAsync(changeOrderId);
        if (changeOrder == null) return null;

        changeOrder.Status = ChangeOrderStatus.Withdrawn;
        changeOrder.Notes = $"Withdrawn: {reason}";
        changeOrder.UpdatedAt = DateTime.UtcNow;
        changeOrder.UpdatedBy = "System";

        _unitOfWork.Repository<ChangeOrder>().Update(changeOrder);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ChangeOrderDto>(changeOrder);
    }

    // Implementation tracking
    public async Task<ChangeOrderDto?> StartImplementationAsync(Guid changeOrderId)
    {
        var changeOrder = await _unitOfWork.Repository<ChangeOrder>().GetByIdAsync(changeOrderId);
        if (changeOrder == null || changeOrder.Status != ChangeOrderStatus.Approved) return null;

        changeOrder.Status = ChangeOrderStatus.InProgress;
        changeOrder.ImplementationStartDate = DateTime.UtcNow;
        changeOrder.UpdatedAt = DateTime.UtcNow;
        changeOrder.UpdatedBy = "System";

        _unitOfWork.Repository<ChangeOrder>().Update(changeOrder);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ChangeOrderDto>(changeOrder);
    }

    public async Task<ChangeOrderDto?> UpdateProgressAsync(Guid changeOrderId, decimal percentageComplete)
    {
        var changeOrder = await _unitOfWork.Repository<ChangeOrder>().GetByIdAsync(changeOrderId);
        if (changeOrder == null) return null;

        changeOrder.PercentageComplete = percentageComplete;
        changeOrder.UpdatedAt = DateTime.UtcNow;
        changeOrder.UpdatedBy = "System";

        _unitOfWork.Repository<ChangeOrder>().Update(changeOrder);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ChangeOrderDto>(changeOrder);
    }

    public async Task<ChangeOrderDto?> CompleteImplementationAsync(Guid changeOrderId, decimal actualCost)
    {
        var changeOrder = await _unitOfWork.Repository<ChangeOrder>().GetByIdAsync(changeOrderId);
        if (changeOrder == null) return null;

        changeOrder.Status = ChangeOrderStatus.Completed;
        changeOrder.ImplementationEndDate = DateTime.UtcNow;
        changeOrder.ActualCostImpact = actualCost;
        changeOrder.PercentageComplete = 100;
        changeOrder.UpdatedAt = DateTime.UtcNow;
        changeOrder.UpdatedBy = "System";

        _unitOfWork.Repository<ChangeOrder>().Update(changeOrder);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ChangeOrderDto>(changeOrder);
    }

    // Related items
    public async Task<bool> LinkToMilestoneAsync(Guid changeOrderId, Guid milestoneId, string impactType, string impactDescription)
    {
        var changeOrderMilestone = new ChangeOrderMilestone
        {
            ChangeOrderId = changeOrderId,
            ContractMilestoneId = milestoneId,
            ImpactType = impactType,
            ImpactDescription = impactDescription,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        };

        await _unitOfWork.Repository<ChangeOrderMilestone>().AddAsync(changeOrderMilestone);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UnlinkFromMilestoneAsync(Guid changeOrderId, Guid milestoneId)
    {
        var links = await _unitOfWork.Repository<ChangeOrderMilestone>()
            .GetAllAsync(filter: com => com.ChangeOrderId == changeOrderId && com.ContractMilestoneId == milestoneId);
        
        var link = links.FirstOrDefault();
        if (link == null) return false;

        _unitOfWork.Repository<ChangeOrderMilestone>().Remove(link);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<ContractMilestoneDto>> GetAffectedMilestonesAsync(Guid changeOrderId)
    {
        var milestoneLinks = await _unitOfWork.Repository<ChangeOrderMilestone>()
            .GetAllAsync(filter: com => com.ChangeOrderId == changeOrderId);

        var milestoneIds = milestoneLinks.Select(com => com.ContractMilestoneId).ToList();

        var milestones = await _unitOfWork.Repository<ContractMilestone>()
            .GetAllAsync(filter: m => milestoneIds.Contains(m.Id));

        return _mapper.Map<IEnumerable<ContractMilestoneDto>>(milestones);
    }

    public async Task<bool> LinkToChangeOrderAsync(Guid changeOrderId, Guid relatedChangeOrderId, string relationType)
    {
        // This would require a ChangeOrderRelation entity or similar
        // For now, we can store it in a metadata field or create a new entity
        throw new NotImplementedException("Requires ChangeOrderRelation entity");
    }

    public async Task<bool> UnlinkFromChangeOrderAsync(Guid changeOrderId, Guid relatedChangeOrderId)
    {
        throw new NotImplementedException("Requires ChangeOrderRelation entity");
    }

    public async Task<IEnumerable<ChangeOrderDto>> GetRelatedChangeOrdersAsync(Guid changeOrderId)
    {
        throw new NotImplementedException("Requires ChangeOrderRelation entity");
    }

    // Financial impact
    public async Task<decimal> CalculateTotalImpactAsync(Guid contractId)
    {
        var changeOrders = await _unitOfWork.Repository<ChangeOrder>()
            .GetAllAsync(filter: co => co.ContractId == contractId && co.Status == ChangeOrderStatus.Approved);

        return changeOrders.Sum(co => co.ApprovedCostImpact ?? co.CostImpact);
    }

    public async Task<int> CalculateTotalScheduleImpactAsync(Guid contractId)
    {
        var changeOrders = await _unitOfWork.Repository<ChangeOrder>()
            .GetAllAsync(filter: co => co.ContractId == contractId && co.Status == ChangeOrderStatus.Approved);

        return changeOrders.Sum(co => co.ApprovedScheduleImpact ?? co.ScheduleImpact);
    }

    // Documents
    public async Task<bool> AttachDocumentAsync(Guid changeOrderId, Guid documentId, string documentType)
    {
        var changeOrderDocument = new ChangeOrderDocument
        {
            ChangeOrderId = changeOrderId,
            DocumentId = documentId,
            DocumentType = documentType,
            AttachedDate = DateTime.UtcNow,
            AttachedBy = "System"
        };

        await _unitOfWork.Repository<ChangeOrderDocument>().AddAsync(changeOrderDocument);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RemoveDocumentAsync(Guid changeOrderId, Guid documentId)
    {
        var documents = await _unitOfWork.Repository<ChangeOrderDocument>()
            .GetAllAsync(filter: cod => cod.ChangeOrderId == changeOrderId && cod.DocumentId == documentId);

        var document = documents.FirstOrDefault();
        if (document == null) return false;

        _unitOfWork.Repository<ChangeOrderDocument>().Remove(document);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<ChangeOrderDocumentDto>> GetDocumentsAsync(Guid changeOrderId)
    {
        var documents = await _unitOfWork.Repository<ChangeOrderDocument>()
            .GetAllAsync(filter: cod => cod.ChangeOrderId == changeOrderId);

        return _mapper.Map<IEnumerable<ChangeOrderDocumentDto>>(documents);
    }

    // Reporting
    public async Task<IEnumerable<ChangeOrderDto>> GetOverdueAsync()
    {
        var currentDate = DateTime.UtcNow;
        
        var changeOrders = await _unitOfWork.Repository<ChangeOrder>()
            .GetAllAsync(
                filter: co => co.Status == ChangeOrderStatus.InProgress && 
                            co.DueDate.HasValue && 
                            co.DueDate < currentDate,
                include: query => query.Include(co => co.Contract).Include(co => co.Initiator));

        return _mapper.Map<IEnumerable<ChangeOrderDto>>(changeOrders);
    }

    public async Task<IEnumerable<ChangeOrderDto>> GetHighPriorityAsync()
    {
        var changeOrders = await _unitOfWork.Repository<ChangeOrder>()
            .GetAllAsync(
                filter: co => co.Priority == ChangeOrderPriority.High || co.Priority == ChangeOrderPriority.Critical,
                include: query => query.Include(co => co.Contract).Include(co => co.Initiator));

        return _mapper.Map<IEnumerable<ChangeOrderDto>>(changeOrders);
    }

    public async Task<Dictionary<ChangeOrderStatus, int>> GetStatusSummaryAsync(Guid contractId)
    {
        var changeOrders = await _unitOfWork.Repository<ChangeOrder>()
            .GetAllAsync(filter: co => co.ContractId == contractId);

        var summary = changeOrders
            .GroupBy(co => co.Status)
            .ToDictionary(g => g.Key, g => g.Count());

        // Ensure all statuses are included
        foreach (ChangeOrderStatus status in Enum.GetValues(typeof(ChangeOrderStatus)))
        {
            if (!summary.ContainsKey(status))
            {
                summary[status] = 0;
            }
        }

        return summary;
    }

    public async Task<Dictionary<ChangeOrderType, decimal>> GetCostByTypeAsync(Guid contractId)
    {
        var changeOrders = await _unitOfWork.Repository<ChangeOrder>()
            .GetAllAsync(filter: co => co.ContractId == contractId && co.Status == ChangeOrderStatus.Approved);

        var costByType = changeOrders
            .GroupBy(co => co.Type)
            .ToDictionary(g => g.Key, g => g.Sum(co => co.ApprovedCostImpact ?? co.CostImpact));

        // Ensure all types are included
        foreach (ChangeOrderType type in Enum.GetValues(typeof(ChangeOrderType)))
        {
            if (!costByType.ContainsKey(type))
            {
                costByType[type] = 0;
            }
        }

        return costByType;
    }
}