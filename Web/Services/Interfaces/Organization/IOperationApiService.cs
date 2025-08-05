using Core.DTOs.Common;
using Core.DTOs.Organization.Operation;

namespace Web.Services.Interfaces.Organization;

/// <summary>
/// Service interface for operation API operations
/// </summary>
public interface IOperationApiService
{
    /// <summary>
    /// Get all operations with pagination
    /// </summary>
    Task<PagedResult<OperationDto>?> GetOperationsAsync(int pageNumber = 1, int pageSize = 10, string? sortBy = null, bool isAscending = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get operation by ID
    /// </summary>
    Task<OperationDto?> GetOperationByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get operations by company ID
    /// </summary>
    Task<List<OperationDto>?> GetOperationsByCompanyAsync(Guid companyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get operation with its projects
    /// </summary>
    Task<OperationWithProjectsDto?> GetOperationProjectsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new operation
    /// </summary>
    Task<OperationDto?> CreateOperationAsync(CreateOperationDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing operation
    /// </summary>
    Task<OperationDto?> UpdateOperationAsync(Guid id, UpdateOperationDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Activate an operation
    /// </summary>
    Task<bool> ActivateOperationAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deactivate an operation
    /// </summary>
    Task<bool> DeactivateOperationAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete an operation (soft delete)
    /// </summary>
    Task<bool> DeleteOperationAsync(Guid id, CancellationToken cancellationToken = default);
}