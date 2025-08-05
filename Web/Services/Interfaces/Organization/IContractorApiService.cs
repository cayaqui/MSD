using Core.DTOs.Common;
using Core.DTOs.Organization.Contractor;
using Core.Enums.Projects;

namespace Web.Services.Interfaces.Organization;

/// <summary>
/// Service interface for contractor API operations
/// </summary>
public interface IContractorApiService
{
    /// <summary>
    /// Get all contractors with pagination
    /// </summary>
    Task<PagedResult<ContractorDto>?> GetContractorsAsync(int pageNumber = 1, int pageSize = 10, string? sortBy = null, bool isAscending = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get contractor by ID
    /// </summary>
    Task<ContractorDto?> GetContractorByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all active contractors
    /// </summary>
    Task<List<ContractorDto>?> GetActiveContractorsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get contractors by type
    /// </summary>
    Task<List<ContractorDto>?> GetContractorsByTypeAsync(ContractorType type, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get contractor with projects
    /// </summary>
    Task<ContractorWithProjectsDto?> GetContractorProjectsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get contractor performance metrics
    /// </summary>
    Task<ContractorPerformanceDto?> GetContractorPerformanceAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new contractor
    /// </summary>
    Task<ContractorDto?> CreateContractorAsync(CreateContractorDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing contractor
    /// </summary>
    Task<ContractorDto?> UpdateContractorAsync(Guid id, UpdateContractorDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Prequalify a contractor
    /// </summary>
    Task<bool> PrequalifyContractorAsync(Guid id, PrequalifyContractorDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Blacklist a contractor
    /// </summary>
    Task<bool> BlacklistContractorAsync(Guid id, BlacklistContractorDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a contractor (soft delete)
    /// </summary>
    Task<bool> DeleteContractorAsync(Guid id, CancellationToken cancellationToken = default);
}