using Core.DTOs.Common;
using Core.DTOs.Cost.ControlAccounts;

namespace Web.Services.Interfaces.Cost;

/// <summary>
/// Interface for control account API operations
/// </summary>
public interface IControlAccountApiService
{
    // Query Operations
    Task<PagedResult<ControlAccountDto>> GetControlAccountsAsync(ControlAccountFilterDto? filter = null, CancellationToken cancellationToken = default);
    Task<ControlAccountDetailDto?> GetControlAccountByIdAsync(Guid controlAccountId, CancellationToken cancellationToken = default);
    Task<List<ControlAccountDto>?> GetControlAccountsByPhaseAsync(Guid phaseId, CancellationToken cancellationToken = default);
    Task<List<ControlAccountAssignmentDto>?> GetControlAccountAssignmentsAsync(Guid controlAccountId, CancellationToken cancellationToken = default);
    Task<EVMSummaryDto?> GetLatestEVMSummaryAsync(Guid controlAccountId, CancellationToken cancellationToken = default);
    
    // Command Operations
    Task<Guid?> CreateControlAccountAsync(CreateControlAccountDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateControlAccountAsync(Guid controlAccountId, UpdateControlAccountDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateControlAccountStatusAsync(Guid controlAccountId, UpdateControlAccountStatusDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteControlAccountAsync(Guid controlAccountId, CancellationToken cancellationToken = default);
    
    // Assignment Operations
    Task<bool> AssignUserToControlAccountAsync(Guid controlAccountId, CreateControlAccountAssignmentDto dto, CancellationToken cancellationToken = default);
    Task<bool> RemoveUserFromControlAccountAsync(Guid controlAccountId, string userToRemove, CancellationToken cancellationToken = default);
    
    // Progress Operations
    Task<bool> UpdateControlAccountProgressAsync(Guid controlAccountId, decimal percentComplete, CancellationToken cancellationToken = default);
    
    // Workflow Operations
    Task<bool> BaselineControlAccountAsync(Guid controlAccountId, CancellationToken cancellationToken = default);
    Task<bool> CloseControlAccountAsync(Guid controlAccountId, CancellationToken cancellationToken = default);
}