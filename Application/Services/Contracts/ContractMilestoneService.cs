using Application.Interfaces.Contracts;
using Application.Services.Base;
using Core.DTOs.Contracts.ContractMilestones;
using Core.Enums.Contracts;
using Core.Enums.Projects;
using Domain.Entities.Contracts.Core;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Contracts;

public class ContractMilestoneService : BaseService<ContractMilestone, ContractMilestoneDto, CreateContractMilestoneDto, UpdateContractMilestoneDto>, IContractMilestoneService
{
    public ContractMilestoneService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<ContractMilestoneService> logger)
        : base(unitOfWork, mapper, logger)
    {
    }

    // Milestone queries
    public async Task<IEnumerable<ContractMilestoneDto>> GetByContractAsync(Guid contractId)
    {
        var milestones = await _unitOfWork.Repository<ContractMilestone>()
            .GetAllAsync(
                filter: m => m.ContractId == contractId,
                orderBy: query => query.OrderBy(m => m.DueDate));

        return _mapper.Map<IEnumerable<ContractMilestoneDto>>(milestones);
    }

    public async Task<ContractMilestoneDto?> GetByMilestoneCodeAsync(Guid contractId, string milestoneCode)
    {
        var milestone = await _unitOfWork.Repository<ContractMilestone>()
            .GetAsync(filter: m => m.ContractId == contractId && m.MilestoneCode == milestoneCode);

        return milestone != null ? _mapper.Map<ContractMilestoneDto>(milestone) : null;
    }

    public async Task<IEnumerable<ContractMilestoneDto>> SearchAsync(ContractMilestoneFilterDto filter)
    {
        Expression<Func<ContractMilestone, bool>>? searchFilter = null;

        if (!string.IsNullOrEmpty(filter.SearchTerm) || filter.ContractId.HasValue || filter.Status.HasValue ||
            filter.Type.HasValue || filter.DueDateFrom.HasValue || filter.DueDateTo.HasValue ||
            filter.IsCritical.HasValue || filter.IsPaymentMilestone.HasValue)
        {
            searchFilter = m =>
                (string.IsNullOrEmpty(filter.SearchTerm) ||
                 m.MilestoneCode.Contains(filter.SearchTerm) ||
                 m.Name.Contains(filter.SearchTerm) ||
                 m.Description.Contains(filter.SearchTerm)) &&
                (!filter.ContractId.HasValue || m.ContractId == filter.ContractId.Value) &&
                (!filter.Status.HasValue || m.Status == filter.Status.Value) &&
                (!filter.Type.HasValue || m.Type == filter.Type.Value) &&
                (!filter.DueDateFrom.HasValue || m.DueDate >= filter.DueDateFrom.Value) &&
                (!filter.DueDateTo.HasValue || m.DueDate <= filter.DueDateTo.Value) &&
                (!filter.IsCritical.HasValue || !filter.IsCritical.Value || m.IsCritical) &&
                (!filter.IsPaymentMilestone.HasValue || !filter.IsPaymentMilestone.Value || m.Type == MilestoneType.Payment);
        }

        var milestones = await _unitOfWork.Repository<ContractMilestone>()
            .GetAllAsync(filter: searchFilter);

        return _mapper.Map<IEnumerable<ContractMilestoneDto>>(milestones);
    }

    public async Task<IEnumerable<ContractMilestoneDto>> GetUpcomingAsync(int days = 30)
    {
        var targetDate = DateTime.UtcNow.AddDays(days);
        var currentDate = DateTime.UtcNow;
        
        var milestones = await _unitOfWork.Repository<ContractMilestone>()
            .GetAllAsync(
                filter: m => m.Status != MilestoneStatus.Completed && 
                           m.DueDate >= currentDate && 
                           m.DueDate <= targetDate,
                include: query => query.Include(m => m.Contract),
                orderBy: query => query.OrderBy(m => m.DueDate));

        return _mapper.Map<IEnumerable<ContractMilestoneDto>>(milestones);
    }

    public async Task<IEnumerable<ContractMilestoneDto>> GetOverdueAsync()
    {
        var currentDate = DateTime.UtcNow;
        
        var milestones = await _unitOfWork.Repository<ContractMilestone>()
            .GetAllAsync(
                filter: m => m.Status != MilestoneStatus.Completed && 
                           m.DueDate < currentDate,
                include: query => query.Include(m => m.Contract),
                orderBy: query => query.OrderBy(m => m.DueDate));

        return _mapper.Map<IEnumerable<ContractMilestoneDto>>(milestones);
    }

    public async Task<IEnumerable<ContractMilestoneDto>> GetCriticalPathAsync(Guid contractId)
    {
        var milestones = await _unitOfWork.Repository<ContractMilestone>()
            .GetAllAsync(
                filter: m => m.ContractId == contractId && m.IsCritical,
                orderBy: query => query.OrderBy(m => m.DueDate));

        return _mapper.Map<IEnumerable<ContractMilestoneDto>>(milestones);
    }

    // Progress tracking
    public async Task<ContractMilestoneDto?> UpdateProgressAsync(Guid milestoneId, UpdateMilestoneProgressDto dto)
    {
        var milestone = await _unitOfWork.Repository<ContractMilestone>().GetByIdAsync(milestoneId);
        if (milestone == null) return null;

        milestone.PercentageComplete = dto.PercentageComplete;
        milestone.ProgressNotes = dto.Notes;
        milestone.LastProgressUpdate = DateTime.UtcNow;
        
        if (dto.PercentageComplete > 0 && milestone.Status == MilestoneStatus.NotStarted)
        {
            milestone.Status = MilestoneStatus.InProgress;
            milestone.ActualStartDate = DateTime.UtcNow;
        }

        milestone.UpdatedAt = DateTime.UtcNow;
        milestone.UpdatedBy = "System";

        _unitOfWork.Repository<ContractMilestone>().Update(milestone);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ContractMilestoneDto>(milestone);
    }

    public async Task<ContractMilestoneDto?> CompleteAsync(Guid milestoneId, DateTime actualDate)
    {
        var milestone = await _unitOfWork.Repository<ContractMilestone>().GetByIdAsync(milestoneId);
        if (milestone == null) return null;

        milestone.Status = MilestoneStatus.Completed;
        milestone.ActualCompletionDate = actualDate;
        milestone.PercentageComplete = 100;
        milestone.UpdatedAt = DateTime.UtcNow;
        milestone.UpdatedBy = "System";

        // Calculate schedule variance
        milestone.ScheduleVariance = (milestone.DueDate - actualDate).Days;

        _unitOfWork.Repository<ContractMilestone>().Update(milestone);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ContractMilestoneDto>(milestone);
    }

    public async Task<IEnumerable<ContractMilestoneDto>> GetInProgressAsync(Guid? contractId = null)
    {
        Expression<Func<ContractMilestone, bool>> progressFilter = m => m.Status == MilestoneStatus.InProgress;

        if (contractId.HasValue)
        {
            progressFilter = m => m.Status == MilestoneStatus.InProgress && m.ContractId == contractId.Value;
        }

        var milestones = await _unitOfWork.Repository<ContractMilestone>()
            .GetAllAsync(
                filter: progressFilter,
                include: query => query.Include(m => m.Contract));

        return _mapper.Map<IEnumerable<ContractMilestoneDto>>(milestones);
    }

    // Approval workflow
    public async Task<ContractMilestoneDto?> SubmitForApprovalAsync(Guid milestoneId)
    {
        var milestone = await _unitOfWork.Repository<ContractMilestone>().GetByIdAsync(milestoneId);
        if (milestone == null) return null;

        milestone.Status = MilestoneStatus.PendingApproval;
        milestone.SubmittedForApprovalDate = DateTime.UtcNow;
        milestone.SubmittedBy = "System";
        milestone.UpdatedAt = DateTime.UtcNow;
        milestone.UpdatedBy = "System";

        _unitOfWork.Repository<ContractMilestone>().Update(milestone);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ContractMilestoneDto>(milestone);
    }

    public async Task<ContractMilestoneDto?> ApproveAsync(Guid milestoneId, ApproveMilestoneDto dto, string approvedBy)
    {
        var milestone = await _unitOfWork.Repository<ContractMilestone>().GetByIdAsync(milestoneId);
        if (milestone == null) return null;

        milestone.Status = MilestoneStatus.Approved;
        milestone.IsApproved = true;
        milestone.ApprovedDate = DateTime.UtcNow;
        milestone.ApprovedBy = approvedBy;
        milestone.ApprovalComments = dto.Comments;
        milestone.UpdatedAt = DateTime.UtcNow;
        milestone.UpdatedBy = approvedBy;

        _unitOfWork.Repository<ContractMilestone>().Update(milestone);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ContractMilestoneDto>(milestone);
    }

    public async Task<ContractMilestoneDto?> RejectAsync(Guid milestoneId, string reason, string rejectedBy)
    {
        var milestone = await _unitOfWork.Repository<ContractMilestone>().GetByIdAsync(milestoneId);
        if (milestone == null) return null;

        milestone.Status = MilestoneStatus.Rejected;
        milestone.IsApproved = false;
        milestone.RejectedDate = DateTime.UtcNow;
        milestone.RejectedBy = rejectedBy;
        milestone.RejectionReason = reason;
        milestone.UpdatedAt = DateTime.UtcNow;
        milestone.UpdatedBy = rejectedBy;

        _unitOfWork.Repository<ContractMilestone>().Update(milestone);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ContractMilestoneDto>(milestone);
    }

    // Payment milestones
    public async Task<IEnumerable<ContractMilestoneDto>> GetPaymentMilestonesAsync(Guid contractId)
    {
        var milestones = await _unitOfWork.Repository<ContractMilestone>()
            .GetAllAsync(
                filter: m => m.ContractId == contractId && m.Type == MilestoneType.Payment,
                orderBy: query => query.OrderBy(m => m.DueDate));

        return _mapper.Map<IEnumerable<ContractMilestoneDto>>(milestones);
    }

    public async Task<IEnumerable<ContractMilestoneDto>> GetUnpaidMilestonesAsync(Guid contractId)
    {
        var milestones = await _unitOfWork.Repository<ContractMilestone>()
            .GetAllAsync(
                filter: m => m.ContractId == contractId && 
                           m.Type == MilestoneType.Payment && 
                           m.IsApproved && 
                           !m.IsPaid,
                orderBy: query => query.OrderBy(m => m.DueDate));

        return _mapper.Map<IEnumerable<ContractMilestoneDto>>(milestones);
    }

    public async Task<ContractMilestoneDto?> RecordInvoiceAsync(Guid milestoneId, string invoiceNumber, decimal amount)
    {
        var milestone = await _unitOfWork.Repository<ContractMilestone>().GetByIdAsync(milestoneId);
        if (milestone == null) return null;

        milestone.IsInvoiced = true;
        milestone.InvoiceNumber = invoiceNumber;
        milestone.InvoiceDate = DateTime.UtcNow;
        milestone.InvoiceAmount = amount;
        milestone.UpdatedAt = DateTime.UtcNow;
        milestone.UpdatedBy = "System";

        _unitOfWork.Repository<ContractMilestone>().Update(milestone);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ContractMilestoneDto>(milestone);
    }

    public async Task<ContractMilestoneDto?> RecordPaymentAsync(Guid milestoneId, decimal amount)
    {
        var milestone = await _unitOfWork.Repository<ContractMilestone>().GetByIdAsync(milestoneId);
        if (milestone == null) return null;

        milestone.IsPaid = true;
        milestone.PaymentDate = DateTime.UtcNow;
        milestone.PaymentAmount = amount;
        milestone.UpdatedAt = DateTime.UtcNow;
        milestone.UpdatedBy = "System";

        _unitOfWork.Repository<ContractMilestone>().Update(milestone);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ContractMilestoneDto>(milestone);
    }

    // Dependencies
    public async Task<bool> AddDependencyAsync(Guid predecessorId, Guid successorId, string dependencyType, int lagDays)
    {
        // Check if dependency already exists
        var existingDependencies = await _unitOfWork.Repository<MilestoneDependency>()
            .GetAllAsync(filter: d => d.PredecessorId == predecessorId && d.SuccessorId == successorId);

        if (existingDependencies.Any()) return false;

        var dependency = new MilestoneDependency
        {
            PredecessorId = predecessorId,
            SuccessorId = successorId,
            DependencyType = dependencyType,
            LagDays = lagDays,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        };

        await _unitOfWork.Repository<MilestoneDependency>().AddAsync(dependency);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RemoveDependencyAsync(Guid predecessorId, Guid successorId)
    {
        var dependencies = await _unitOfWork.Repository<MilestoneDependency>()
            .GetAllAsync(filter: d => d.PredecessorId == predecessorId && d.SuccessorId == successorId);

        var dependency = dependencies.FirstOrDefault();
        if (dependency == null) return false;

        _unitOfWork.Repository<MilestoneDependency>().Remove(dependency);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<ContractMilestoneDto>> GetPredecessorsAsync(Guid milestoneId)
    {
        var predecessorDependencies = await _unitOfWork.Repository<MilestoneDependency>()
            .GetAllAsync(filter: d => d.SuccessorId == milestoneId);

        var predecessorIds = predecessorDependencies.Select(d => d.PredecessorId).ToList();

        var milestones = await _unitOfWork.Repository<ContractMilestone>()
            .GetAllAsync(filter: m => predecessorIds.Contains(m.Id));

        return _mapper.Map<IEnumerable<ContractMilestoneDto>>(milestones);
    }

    public async Task<IEnumerable<ContractMilestoneDto>> GetSuccessorsAsync(Guid milestoneId)
    {
        var successorDependencies = await _unitOfWork.Repository<MilestoneDependency>()
            .GetAllAsync(filter: d => d.PredecessorId == milestoneId);

        var successorIds = successorDependencies.Select(d => d.SuccessorId).ToList();

        var milestones = await _unitOfWork.Repository<ContractMilestone>()
            .GetAllAsync(filter: m => successorIds.Contains(m.Id));

        return _mapper.Map<IEnumerable<ContractMilestoneDto>>(milestones);
    }

    public async Task<bool> ValidateDependenciesAsync(Guid milestoneId)
    {
        // Check for circular dependencies
        var visited = new HashSet<Guid>();
        var recursionStack = new HashSet<Guid>();

        return await ValidateDependencyRecursive(milestoneId, visited, recursionStack);
    }

    private async Task<bool> ValidateDependencyRecursive(Guid milestoneId, HashSet<Guid> visited, HashSet<Guid> recursionStack)
    {
        visited.Add(milestoneId);
        recursionStack.Add(milestoneId);

        var successorDependencies = await _unitOfWork.Repository<MilestoneDependency>()
            .GetAllAsync(filter: d => d.PredecessorId == milestoneId);

        var successorIds = successorDependencies.Select(d => d.SuccessorId).ToList();

        foreach (var successorId in successorIds)
        {
            if (!visited.Contains(successorId))
            {
                if (!await ValidateDependencyRecursive(successorId, visited, recursionStack))
                    return false;
            }
            else if (recursionStack.Contains(successorId))
            {
                // Circular dependency detected
                return false;
            }
        }

        recursionStack.Remove(milestoneId);
        return true;
    }

    // Schedule analysis
    public async Task<ContractMilestoneDto?> UpdateForecastDateAsync(Guid milestoneId, DateTime forecastDate, string explanation)
    {
        var milestone = await _unitOfWork.Repository<ContractMilestone>().GetByIdAsync(milestoneId);
        if (milestone == null) return null;

        milestone.ForecastDate = forecastDate;
        milestone.ForecastExplanation = explanation;
        milestone.UpdatedAt = DateTime.UtcNow;
        milestone.UpdatedBy = "System";

        _unitOfWork.Repository<ContractMilestone>().Update(milestone);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ContractMilestoneDto>(milestone);
    }

    public async Task<Dictionary<Guid, int>> CalculateScheduleVariancesAsync(Guid contractId)
    {
        var milestones = await _unitOfWork.Repository<ContractMilestone>()
            .GetAllAsync(filter: m => m.ContractId == contractId);

        var variances = new Dictionary<Guid, int>();

        foreach (var milestone in milestones)
        {
            int variance = 0;

            if (milestone.Status == MilestoneStatus.Completed && milestone.ActualCompletionDate.HasValue)
            {
                variance = (milestone.DueDate - milestone.ActualCompletionDate.Value).Days;
            }
            else if (milestone.ForecastDate.HasValue)
            {
                variance = (milestone.DueDate - milestone.ForecastDate.Value).Days;
            }

            variances[milestone.Id] = variance;
        }

        return variances;
    }

    public async Task<DateTime?> CalculateProjectedCompletionAsync(Guid contractId)
    {
        var milestones = await _unitOfWork.Repository<ContractMilestone>()
            .GetAllAsync(
                filter: m => m.ContractId == contractId,
                orderBy: query => query.OrderByDescending(m => m.DueDate));

        var lastMilestone = milestones.FirstOrDefault();
        if (lastMilestone == null) return null;

        if (lastMilestone.ForecastDate.HasValue)
            return lastMilestone.ForecastDate;

        // Calculate based on average delay
        var completedMilestones = await _unitOfWork.Repository<ContractMilestone>()
            .GetAllAsync(filter: m => m.ContractId == contractId && 
                               m.Status == MilestoneStatus.Completed && 
                               m.ActualCompletionDate.HasValue);

        if (!completedMilestones.Any())
            return lastMilestone.DueDate;

        var averageDelay = completedMilestones
            .Average(m => (m.ActualCompletionDate!.Value - m.DueDate).Days);

        return lastMilestone.DueDate.AddDays(averageDelay);
    }

    // Documents
    public async Task<bool> AttachDocumentAsync(Guid milestoneId, Guid documentId, string documentType)
    {
        var milestoneDocument = new MilestoneDocument
        {
            ContractMilestoneId = milestoneId,
            DocumentId = documentId,
            DocumentType = documentType,
            AttachedDate = DateTime.UtcNow,
            AttachedBy = "System"
        };

        await _unitOfWork.Repository<MilestoneDocument>().AddAsync(milestoneDocument);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RemoveDocumentAsync(Guid milestoneId, Guid documentId)
    {
        var documents = await _unitOfWork.Repository<MilestoneDocument>()
            .GetAllAsync(filter: md => md.ContractMilestoneId == milestoneId && md.DocumentId == documentId);

        var document = documents.FirstOrDefault();
        if (document == null) return false;

        _unitOfWork.Repository<MilestoneDocument>().Remove(document);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<MilestoneDocumentDto>> GetDocumentsAsync(Guid milestoneId)
    {
        var documents = await _unitOfWork.Repository<MilestoneDocument>()
            .GetAllAsync(filter: md => md.ContractMilestoneId == milestoneId);

        return _mapper.Map<IEnumerable<MilestoneDocumentDto>>(documents);
    }

    // Reporting
    public async Task<decimal> GetTotalMilestoneValueAsync(Guid contractId)
    {
        var milestones = await _unitOfWork.Repository<ContractMilestone>()
            .GetAllAsync(filter: m => m.ContractId == contractId && m.Type == MilestoneType.Payment);

        return milestones.Sum(m => m.PaymentAmount);
    }

    public async Task<decimal> GetCompletedMilestoneValueAsync(Guid contractId)
    {
        var milestones = await _unitOfWork.Repository<ContractMilestone>()
            .GetAllAsync(filter: m => m.ContractId == contractId && 
                               m.Type == MilestoneType.Payment && 
                               m.Status == MilestoneStatus.Completed);

        return milestones.Sum(m => m.PaymentAmount);
    }

    public async Task<Dictionary<MilestoneStatus, int>> GetStatusSummaryAsync(Guid contractId)
    {
        var milestones = await _unitOfWork.Repository<ContractMilestone>()
            .GetAllAsync(filter: m => m.ContractId == contractId);

        var summary = milestones
            .GroupBy(m => m.Status)
            .ToDictionary(g => g.Key, g => g.Count());

        // Ensure all statuses are included
        foreach (MilestoneStatus status in Enum.GetValues(typeof(MilestoneStatus)))
        {
            if (!summary.ContainsKey(status))
            {
                summary[status] = 0;
            }
        }

        return summary;
    }

    public async Task<IEnumerable<ContractMilestoneDto>> GetDelayedMilestonesAsync(Guid contractId)
    {
        var currentDate = DateTime.UtcNow;
        
        var milestones = await _unitOfWork.Repository<ContractMilestone>()
            .GetAllAsync(
                filter: m => m.ContractId == contractId &&
                           m.Status != MilestoneStatus.Completed &&
                           ((m.ForecastDate.HasValue && m.ForecastDate > m.DueDate) ||
                            (!m.ForecastDate.HasValue && m.DueDate < currentDate)),
                orderBy: query => query.OrderBy(m => m.DueDate));

        return _mapper.Map<IEnumerable<ContractMilestoneDto>>(milestones);
    }
}