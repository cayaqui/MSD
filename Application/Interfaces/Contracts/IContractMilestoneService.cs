using Application.Interfaces.Common;
using Core.DTOs.Contracts.ContractMilestones;
using Core.Enums.Contracts;
using Core.Enums.Projects;
using Domain.Entities.Contracts;

namespace Application.Interfaces.Contracts;

public interface IContractMilestoneService : IBaseService<ContractMilestoneDto, CreateContractMilestoneDto, UpdateContractMilestoneDto>
{
    // Milestone queries
    Task<IEnumerable<ContractMilestoneDto>> GetByContractAsync(Guid contractId);
    Task<ContractMilestoneDto?> GetByMilestoneCodeAsync(Guid contractId, string milestoneCode);
    Task<IEnumerable<ContractMilestoneDto>> SearchAsync(ContractMilestoneFilterDto filter);
    Task<IEnumerable<ContractMilestoneDto>> GetUpcomingAsync(int days = 30);
    Task<IEnumerable<ContractMilestoneDto>> GetOverdueAsync();
    Task<IEnumerable<ContractMilestoneDto>> GetCriticalPathAsync(Guid contractId);
    
    // Progress tracking
    Task<ContractMilestoneDto?> UpdateProgressAsync(Guid milestoneId, UpdateMilestoneProgressDto dto);
    Task<ContractMilestoneDto?> CompleteAsync(Guid milestoneId, DateTime actualDate);
    Task<IEnumerable<ContractMilestoneDto>> GetInProgressAsync(Guid? contractId = null);
    
    // Approval workflow
    Task<ContractMilestoneDto?> SubmitForApprovalAsync(Guid milestoneId);
    Task<ContractMilestoneDto?> ApproveAsync(Guid milestoneId, ApproveMilestoneDto dto, string approvedBy);
    Task<ContractMilestoneDto?> RejectAsync(Guid milestoneId, string reason, string rejectedBy);
    
    // Payment milestones
    Task<IEnumerable<ContractMilestoneDto>> GetPaymentMilestonesAsync(Guid contractId);
    Task<IEnumerable<ContractMilestoneDto>> GetUnpaidMilestonesAsync(Guid contractId);
    Task<ContractMilestoneDto?> RecordInvoiceAsync(Guid milestoneId, string invoiceNumber, decimal amount);
    Task<ContractMilestoneDto?> RecordPaymentAsync(Guid milestoneId, decimal amount);
    
    // Dependencies
    Task<bool> AddDependencyAsync(Guid predecessorId, Guid successorId, string dependencyType, int lagDays);
    Task<bool> RemoveDependencyAsync(Guid predecessorId, Guid successorId);
    Task<IEnumerable<ContractMilestoneDto>> GetPredecessorsAsync(Guid milestoneId);
    Task<IEnumerable<ContractMilestoneDto>> GetSuccessorsAsync(Guid milestoneId);
    Task<bool> ValidateDependenciesAsync(Guid milestoneId);
    
    // Schedule analysis
    Task<ContractMilestoneDto?> UpdateForecastDateAsync(Guid milestoneId, DateTime forecastDate, string explanation);
    Task<Dictionary<Guid, int>> CalculateScheduleVariancesAsync(Guid contractId);
    Task<DateTime?> CalculateProjectedCompletionAsync(Guid contractId);
    
    // Documents
    Task<bool> AttachDocumentAsync(Guid milestoneId, Guid documentId, string documentType);
    Task<bool> RemoveDocumentAsync(Guid milestoneId, Guid documentId);
    Task<IEnumerable<MilestoneDocumentDto>> GetDocumentsAsync(Guid milestoneId);
    
    // Reporting
    Task<decimal> GetTotalMilestoneValueAsync(Guid contractId);
    Task<decimal> GetCompletedMilestoneValueAsync(Guid contractId);
    Task<Dictionary<MilestoneStatus, int>> GetStatusSummaryAsync(Guid contractId);
    Task<IEnumerable<ContractMilestoneDto>> GetDelayedMilestonesAsync(Guid contractId);
}