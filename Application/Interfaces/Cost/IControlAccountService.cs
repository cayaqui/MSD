using Core.DTOs.ControlAccounts;
using Core.DTOs.Common;
using Domain.Common;

namespace Application.Interfaces.Cost;

/// <summary>
/// Service interface for Control Account operations
/// </summary>
public interface IControlAccountService
{
    // Query operations
    Task<PagedResult<ControlAccountDto>> GetControlAccountsAsync(
        Guid? projectId,
        QueryParameters parameters,
        CancellationToken cancellationToken = default);

    Task<ControlAccountDetailDto?> GetControlAccountByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<List<ControlAccountDto>> GetControlAccountsByPhaseAsync(
        Guid phaseId,
        CancellationToken cancellationToken = default);

    Task<List<ControlAccountAssignmentDto>> GetControlAccountAssignmentsAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<EVMSummaryDto?> GetLatestEVMSummaryAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    // Command operations
    Task<Result<Guid>> CreateControlAccountAsync(
        CreateControlAccountDto dto,
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result> UpdateControlAccountAsync(
        Guid id,
        UpdateControlAccountDto dto,
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result> UpdateControlAccountStatusAsync(
        Guid id,
        UpdateControlAccountStatusDto dto,
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result> AssignUserToControlAccountAsync(
        Guid id,
        CreateControlAccountAssignmentDto dto,
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result> UpdateControlAccountProgressAsync(
        Guid id,
        decimal percentComplete,
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result> BaselineControlAccountAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result> CloseControlAccountAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteControlAccountAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result> RemoveUserFromControlAccountAsync(
        Guid id,
        string userToRemove,
        string userId,
        CancellationToken cancellationToken = default);
}