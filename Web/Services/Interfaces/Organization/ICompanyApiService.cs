using Core.DTOs.Common;
using Core.DTOs.Organization.Company;

namespace Web.Services.Interfaces.Organization;

/// <summary>
/// Service interface for company API operations
/// </summary>
public interface ICompanyApiService
{
    /// <summary>
    /// Get all companies with pagination
    /// </summary>
    Task<PagedResult<CompanyDto>?> GetCompaniesAsync(int pageNumber = 1, int pageSize = 10, string? sortBy = null, bool isAscending = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get company by ID
    /// </summary>
    Task<CompanyDto?> GetCompanyByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get company with its operations
    /// </summary>
    Task<CompanyWithOperationsDto?> GetCompanyOperationsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all active companies
    /// </summary>
    Task<List<CompanyDto>?> GetActiveCompaniesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new company
    /// </summary>
    Task<CompanyDto?> CreateCompanyAsync(CreateCompanyDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing company
    /// </summary>
    Task<CompanyDto?> UpdateCompanyAsync(Guid id, UpdateCompanyDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Activate a company
    /// </summary>
    Task<bool> ActivateCompanyAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deactivate a company
    /// </summary>
    Task<bool> DeactivateCompanyAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a company (soft delete)
    /// </summary>
    Task<bool> DeleteCompanyAsync(Guid id, CancellationToken cancellationToken = default);
}